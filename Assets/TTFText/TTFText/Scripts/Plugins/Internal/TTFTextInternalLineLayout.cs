using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

namespace TTFTextInternal {
#region Line Layouts
	
	// THIS IS THE BASE CLASS FOR ALL THE "SPECIAL CHARS"
	[System.Serializable]
	public class PseudoCharacter
	{
			public float width;
			public float height;
			
	}
	
	
	[System.Serializable]
	public class NamedTextureElement : PseudoCharacter
	{
			public string texturename;
			public float UVstartx=0;
			public float UVstarty=0;
			public float UVwidth=1;
			public float UVheight=1;
	}

	public class TextureElement : PseudoCharacter
	{
			public bool shouldReleaseMaterial=false;
			public Material material;
			public bool shouldReleaseTexture=false;
			public Texture texture;
			public float UVstartx=0;
			public float UVstarty=0;
			public float UVwidth=1;
			public float UVheight=1;
	}
	
	
	
	[System.Serializable]
    public class LineLayout
	{
		private const int default_alloc = 8;
		private string linetext;    // text 
		public string line { get { return linetext; } }
		
		public float hspacing; // spacing to be used for characters
		public float offset;   // offset for the beginning of the line
		public Vector3 advancedir;
		public float advancelen;  // advance for the whole line
		public Vector3 advance { get { return advancedir * advancelen ; } }
		// NEW FROM 1.0.15
		public TTFText tm;
		public TTFText.ParagraphAlignmentEnum align;
		public float linewidth;
		public float[] charadvances;  // for each character this is the ideal advance
		public float[] charheights;
		public float[] charpositions; // we shall allow user to set up each character individually
		public int lineno;             // <- counter specifying current line
		// NEW FROM 1.0.16
		//public TTFTextStyle [] charstyle; // each character may have a different style...
		public int[]   charstyleindex;
		
		// NEW FROM 1.1
		public float prevadvance = 0; // <- previous advance vector
		// NEW FROM 1.2
		public object[] charmetadata; // <- serialized metadata for special characters
		//public string[] charmetadata_serialized; // <- serialized metadata for special characters
		
		
		
		
		
		
		
		
		/// <summary>
		/// Returns the sum of all charadvance until position c
		/// </summary>
		/// <returns>
		/// The sum advance.
		/// </returns>
		/// <param name='c'>
		/// C.
		/// </param>
		public float CharSumAdvance (int c)
		{
			float s = prevadvance;
			for (int i=0; i<c; i++) {
				s += charadvances [c];
			}
			return s;
		}
		
		
		
		
		/// <summary>
		/// Returns the style of a character at a specified position
		/// </summary>
		/// <returns>
		/// The char style.
		/// </returns>
		/// <param name='pos'>
		/// Position.
		/// </param>
		public TTFTextStyle GetCharStyle (int pos)
		{
#if TTFTEXT_LITE
			if (!tm.DemoMode) {return tm.InitTextStyle;}			
#endif		
			if (charstyleindex==null) {return tm.InitTextStyle;}	
			//if (charstyleindex==null) {return tm.CurrentTextStyle;}	
			if (charstyleindex [pos] == -1) {			
				if ((charmetadata!=null)&&(charmetadata[pos]!=null)) {
					return tm.InitTextStyle; // <= may also happen for special characters
				}
				Debug.Log (pos);
			}
			return tm.UsedStyles [charstyleindex [pos]];
		}
		
		float Xmin0 = 0; // leftmost boundary for the first glyph of this line
		float XmaxN = 0; // rightmost boundary for the last glyph of this line
		
		
		/// <summary>
		/// Returns the  maximum character size of character in the line, which is meant to be used
		/// to compute the line height.
		/// TODO: Check this function for vertical layout compatibility
		/// </summary>
		/// <value>
		/// The size of the max character.
		/// </value>
		public float MaxCharacterSize {
			get {
				float msz = 0;
				for (int i=0; i<linetext.Length; i++) {
					//Debug.Log(System.String.Format("{0} {1} {2} {3}",i,msz,GetCharStyle(i).Size,charheights[i]));
					if (/*(charstyleindex[i]!=-1)&&*/
				     (linetext [i] != ' ')
					 && (Mathf.Abs (GetCharStyle (i).Size * charheights [i]) > msz)) {
						msz = Mathf.Abs (GetCharStyle (i).Size * charheights [i]);
					}
					/*	
				else {
						
						Debug.Log(
							System.String.Format("{0} {1} {2}",
							(GetCharStyle(i)!=null),
							(linetext[i]!=' '),
							(Mathf.Abs(GetCharStyle(i).Size*charheights[i])>msz)));
				}
				*/
				}
				return msz;
			}
		}
		
		
		/// <summary>
		/// Sets the character style for the character for the line from character f to character t
		/// for easier serialization character styles are refered indirectly
		/// by the mean of style idx
		/// </summary>
		/// <param name='f'>
		/// From
		/// </param>
		/// <param name='t'>
		/// To
		/// </param>
		/// <param name='stidx'>
		/// Style index.
		/// </param>
		public void SetCharStyle (int f, int t, int stidx)
		{
			for (int i=f; i<t; i++) {
				while (i>=charadvances.Length) {
					System.Array.Resize<float> (ref charadvances, charadvances.Length * 2);
				}
				while (i>=charpositions.Length) {
					System.Array.Resize<float> (ref charpositions, charpositions.Length * 2);
				}				
				while (i>=charstyleindex.Length) {					
					System.Array.Resize<int> (ref charstyleindex, charstyleindex.Length * 2);
					for (int j=t; j<charstyleindex.Length; j++)
						charstyleindex [j] = -1;
				}
				while (i>=charheights.Length) {					
					System.Array.Resize<float> (ref charheights, charheights.Length * 2);
				}
				/*
				while (i>=charmetadata.Length) {					
					System.Array.Resize<string> (ref charmetadata, charmetadata.Length * 2);
				}
				*/
				while (i>=charmetadata.Length) {					
					System.Array.Resize<object> (ref charmetadata, charmetadata.Length * 2);
				}
				
				charstyleindex [i] = stidx;
			}
		}
		
		
		
		/// <summary>
		/// Try to append a special element on the line
		/// </summary>
		/// <param name='o'>
		/// o : the special character
		/// </param>
		public void AppendSpecialChar (PseudoCharacter o)
		{
			char [] escapechar =System.Text.ASCIIEncoding.ASCII.GetChars(new byte [] { 0x1b});
			int l0 = linetext.Length;			
			linetext += escapechar[0];
			
			SetCharStyle (l0, l0+1, tm.currentStyleIdx);
			charadvances [l0] = o.width*tm.CurrentTextStyle.Size;
			charheights [l0 ] = o.height*tm.CurrentTextStyle.Size;
			// maybe be better for incremental  layout ??
			//charmetadata [l0] = System.Text.ASCIIEncoding.ASCII.GetString(TTFTextInternal.SerializeObject(o));
			charmetadata [l0] = o;
			if (float.IsNaN (XmaxN) || float.IsInfinity (XmaxN)) {
				XmaxN = 0;
			}
			if (float.IsNaN (Xmin0) || float.IsInfinity (Xmin0)) {
				Xmin0 = 0;
			}					
		}
		
		
		
		public void AddImage(float w, float h,string texturename) {
			AppendSpecialChar(new NamedTextureElement {width=w,height=h,texturename=texturename});
			//tm.gameObject.renderer.sharedMaterials[int.Parse(texturename)].mainTexture));
		}
		
		
		public void AddImage(float w, float h,string texturename, float stx,float sty,float dx, float dy) { 
			NamedTextureElement tbi=new NamedTextureElement {width=w,height=h,texturename=texturename};
			tbi.UVstartx=stx;
			tbi.UVstarty=sty;
			tbi.UVwidth=dx;
			tbi.UVheight=dy;
			AppendSpecialChar(tbi);
			//tm.gameObject.renderer.sharedMaterials[int.Parse(texturename)].mainTexture));
		}
		
		
		
		/// <summary>
		/// Appends some text to the line.
		/// </summary>
		/// <param name='s'>
		///  text to be appended.
		/// </param>
		/// <exception cref='System.Exception'>
		/// Is thrown when the font may not be found, or when we
		/// are unable to compute the layout info
		/// </exception>
		public void AppendText (string s)
		{
			if (s.Length == 0)
				return;
			int l0 = linetext.Length;			
			linetext += s;
#if TTFTEXT_LITE						
			if (tm.DemoMode) {
#endif				
			SetCharStyle (l0, l0 + s.Length, tm.currentStyleIdx);
#if TTFTEXT_LITE						
			}
			else {
			  SetCharStyle(l0,l0+s.Length,0);	
			}
#endif				
			
			// UPDATE THE METRIC INFO
			object f = null;
			object parameters = null;
			;
			int fp = 0;
			float sumadvance=0;
			for (int i=0;i<l0;i++) {sumadvance +=charadvances[i];}
			string currentfontid = "";
			
			//f = tm.InitTextStyle.GetFont (ref fp, ref currentfontid, ref parameters);
			//f = tm.CurrentTextStyle.GetFont (ref fp, ref currentfontid, ref parameters);
#if ! TTFTEXT_LITE				
				f = tm.CurrentTextStyle.GetFont (ref fp, ref currentfontid, ref parameters);
#else 
				fp=0; 
				currentfontid=tm.CurrentTextStyle.FontId;
				parameters=tm.CurrentTextStyle.GetFontEngineParameters(0);
				if (parameters==null) {
							System.Type t = TTFTextInternal.TTFTextFontEngine.font_engines [0].GetType ().GetNestedType ("Parameters");						
							tm.CurrentTextStyle.SetFontEngineParameters(0,t.InvokeMember (t.Name, BindingFlags.CreateInstance, null, null, null));
							parameters=tm.CurrentTextStyle.GetFontEngineParameters(0);
				}			
				f= TTFTextInternal.TTFTextFontEngine.font_engines[0].GetFont(parameters,currentfontid);				
#endif				
			
			if (f == null) {
				//throw new System.Exception ("Font not found :" + fp + "/" + tm.InitTextStyle.GetFontEngineFontId (fp));
				throw new System.Exception ("Font not found :" + fp + "/" + tm.CurrentTextStyle.GetFontEngineFontId (fp));
			}
				
			for (int i=0; i<s.Length; i++) {
				TTFTextStyle cttfstyle = GetCharStyle (l0 + i);					
					
				if ((cttfstyle != null) && (currentfontid != cttfstyle.GetFontEngineFontIdD (fp))) {
					TTFTextFontEngine.font_engines [fp].DisposeFont (f);					    
//					f = cttfstyle.GetFont (ref fp, ref currentfontid, ref parameters);
#if ! TTFTEXT_LITE				
				f = cttfstyle.GetFont (ref fp, ref currentfontid, ref parameters);
#else 
				fp=0; 
				currentfontid=cttfstyle.FontId;
				parameters=cttfstyle.GetFontEngineParameters(0);
				if (parameters==null) {
							System.Type t = TTFTextInternal.TTFTextFontEngine.font_engines [0].GetType ().GetNestedType ("Parameters");						
							cttfstyle.SetFontEngineParameters(0,t.InvokeMember (t.Name, BindingFlags.CreateInstance, null, null, null));
							parameters=tm.CurrentTextStyle.GetFontEngineParameters(0);
				}					
				f= TTFTextInternal.TTFTextFontEngine.font_engines[0].GetFont(parameters,currentfontid);
					
#endif				

					
					if (f == null) {
						//throw new System.Exception ("Font not found :" + fp + "/" + tm.InitTextStyle.GetFontEngineFontId (fp));
						throw new System.Exception ("Font not found :" + fp + "/" + tm.CurrentTextStyle.GetFontEngineFontId (fp));
					}
				}
					
				    
				charadvances [l0 + i] = TTFTextFontEngine.font_engines [fp].GetAdvance (parameters, f, s [i]).x * cttfstyle.Size;
				charheights [l0 + i] = TTFTextFontEngine.font_engines [fp].GetHeight (parameters, f);
				charmetadata [l0 + i] = null;
				
				if (!TTFTextFontEngine.font_engines[fp].IsBitmapFontProvider(parameters)) {
			  	  TTFTextOutline o = TTFTextFontEngine.font_engines [fp].GetGlyphOutline (parameters, f, s [i]);
				  o.Rescale (cttfstyle.Size);				
				  if (l0 + i == 0) {
					Xmin0 = o.Min.x;
				  }
				  if (l0 + i == line.Length - 1) {
					XmaxN = o.Max.x;
				  }
				}
				else {
				  //Debug.LogWarning("NYI !");
				  sumadvance+=	charadvances [l0 + i];
				  if (l0 + i == 0) {
					Xmin0 = 0;
				  }
				  if (l0 + i == line.Length - 1) {
					XmaxN = sumadvance;
				  }

				}
			}
			
			TTFTextFontEngine.font_engines [fp].DisposeFont (f);
			
			
			// can happen when the first or last char is non printable, like a space for example
			if (float.IsNaN (XmaxN) || float.IsInfinity (XmaxN)) {
				XmaxN = 0;
			}
			if (float.IsNaN (Xmin0) || float.IsInfinity (Xmin0)) {
				Xmin0 = 0;
			}
		
		}
		
		
		
		
		
		public LineLayout (string l, TTFText atm, bool initialize)
		{
			offset = 0;
			linetext = "";
			hspacing = atm.Hspacing;		
			tm = atm;
			linewidth = tm.CurrentTextStyle.LineWidth;
			advancedir = Vector3.right;
			charadvances = new float[default_alloc];  // for each character this is the ideal advance			
			charpositions = new float[default_alloc]; // we shall allow user to set up each character individually
			charheights = new float[default_alloc]; 
			charstyleindex = new int[default_alloc];
			charmetadata = new object[default_alloc];
			
			for (int j=0; j<default_alloc; j++) {
				charstyleindex [j] = -1;
			}				

			advancelen = 0;
			lineno = 0;
			align = tm.CurrentTextStyle.ParagraphAlignment;
			AppendText (l);
			//ComputeMetricInfo();
			if (initialize) {
				ComputeCharacterPositions ();
			}
		}
		
		
		/// <summary>
		/// Set the content of the line to be shorter portion
		/// of text that has been already suggested.
		/// this is used to revert to the initial content of a 
		/// line in which we tried to put too much text.
		/// </summary>
		/// <param name='s'>
		/// S.
		/// </param>
		public void RewindLine (string s)
		{
			if (linetext.StartsWith (s)) {
				linetext = s;
			}
		}
		
		
		/// <summary>
		/// Extra incomprssible width from the left of first char, and the right of last one
		/// </summary>
		/// <value>
		/// The width of the bounds.
		/// </value>
		public float BoundsWidth { 
			get {
				if (line.Length == 0) {
					return 0;
				}
				return XmaxN - Xmin0;
			}
		}
		
		
		// inside width = sum of advances between chars (all advances but the last one)
		public float SumNM1AdvanceAmounts ()
		{ // the sum of the advances
			float w = 0;
			for (int i = 0; i < line.Length - 1; ++i) {
				w += charadvances [i];
			}
			return w;
		}
		
		public float AdvanceLastCharacter ()
		{
			if (line.Length == 0) {
				return 0;
			}
			return charadvances [line.Length - 1];
		}
		
		
		
		
		// THIS IS THE AMOUNT OF TEXT INSIDE OF THE LINE
		public float AdvanceBasedLineWidth ()
		{
			return AdvanceLastCharacter () + SumNM1AdvanceAmounts ();
		}
		
		public float GetDefaultLineWidth (float hspacing)
		{
			
			float outside;
			if (tm.HSpacingMode == TTFText.HSpacingModeEnum.GlyphBoundaries) {
				outside = BoundsWidth;
			} else {
				outside = AdvanceLastCharacter ();
			}
			
			return SumNM1AdvanceAmounts () * tm.Hspacing + outside;
		}
		
		
		
		

		// THIS IS WIDTH OF THE LINE WITH ITS LAYOUT
		public float GetActualWidthBoundariesBased ()
		{
			if (line.Length == 0) {
				return 0;
			}
			return charpositions [line.Length - 1] - charpositions [0] + BoundsWidth;
		}
		
		public float GetActualWidthAdvanceBased ()
		{
			if (line.Length == 0) {
				return 0;
			}
			//Debug.Log(System.String.Format("P : {0} {1} {2}",charpositions[linetext.Length - 1] , charpositions[0] , AdvanceLastCharacter()));
			return charpositions [linetext.Length - 1] - charpositions [0] + AdvanceLastCharacter ();
		}
		
		public float GetActualLinewidth ()
		{			
			float f = 0;
			switch (tm.HSpacingMode) {
			case TTFText.HSpacingModeEnum.GlyphBoundaries:
				f = GetActualWidthBoundariesBased ();
				break;
			case TTFText.HSpacingModeEnum.GlyphAdvance:			
				f = GetActualWidthAdvanceBased ();
				break;
			}
			return f;
		}
		
		
		
		
		
		public void ComputeCharacterPositionsBoundaryBased ()
		{
			if (tm.HSpacingMode != TTFText.HSpacingModeEnum.GlyphBoundaries) {
				Debug.LogError ("(TTFText) Hspacing = " + tm.CurrentTextStyle.HSpacingMode);
			}
			
			float defW = BoundsWidth + SumNM1AdvanceAmounts ();
			float Inside = SumNM1AdvanceAmounts ();
			float Xtra = linewidth - offset - defW;
		
#if TTFTEXT_LITE        
		if (tm.DemoMode) {	
#endif			
			
			if (tm.LayoutMode == TTFText.LayoutModeEnum.No // One line mode
		    || ((align != TTFText.ParagraphAlignmentEnum.Justified) 
			  && (align != TTFText.ParagraphAlignmentEnum.FullyJustified))) { // do not justify
				
				//Xtra = (tm.Hspacing -1) * defW;
				Xtra = (tm.CurrentTextStyle.Hspacing - 1) * Inside;
			}
#if TTFTEXT_LITE										
		}
		else {
				Xtra = (tm.CurrentTextStyle.Hspacing -1) * Inside;
		}			
#endif			
			
			// Xtra = XtraSpace + XtraAdd + XtraMul
			float XtraSpace = 0;
			float XtraAdd = 0;
			float XtraMul = 0;			
			
			int nspaces = 0;
			foreach (char c in line) {
				if (c == ' ') {
					++nspaces;
				}
			}
			
			float addSpace = 0;
			if (nspaces == 0) {				
				XtraSpace = 0;
				XtraAdd = tm.CurrentTextStyle.HSpacingMultFactor * Xtra;
				XtraMul = (1 - tm.CurrentTextStyle.HSpacingMultFactor) * Xtra;
					
			} else {				
				XtraSpace = tm.CurrentTextStyle.WordSpacingFactor * Xtra;
				addSpace = XtraSpace / nspaces;
				XtraAdd = tm.CurrentTextStyle.HSpacingMultFactor * (1 - tm.CurrentTextStyle.WordSpacingFactor) * Xtra;
				XtraMul = (1 - tm.CurrentTextStyle.HSpacingMultFactor) * (1 - tm.CurrentTextStyle.WordSpacingFactor) * Xtra;
			}
			
			
			float addq = 0;
			float mulf = 1;
			
			if (line.Length > 1) {
				addq = XtraAdd / (line.Length - 1);
			}				
			if (Inside != 0) {
				mulf = (Inside + XtraMul) / Inside;
			}
			
			// change offset so that leftmost outline point is at coordinate 0			
			float pos = - Xmin0;
			for (int i = 0; i < line.Length; i++) {				
				charpositions [i] = pos;				
				pos += charadvances [i] * mulf + addq;				
				if (line [i] == ' ') {
					pos += addSpace;
				}
			}
			
			advancelen = pos;
		}
		
		
		
		
		
		
		
		public void ComputeCharacterPositionsAdvanceBased ()
		{		
			if (tm.HSpacingMode != TTFText.HSpacingModeEnum.GlyphAdvance) {
				Debug.LogError ("(TTFText) Hspacing = " + tm.HSpacingMode);
			}			
			// Advance Simple
			
			float defW = AdvanceBasedLineWidth ();
			float Inside = SumNM1AdvanceAmounts ();
			float Xtra = linewidth - offset - defW;
		
#if TTFTEXT_LITE        
		if (tm.DemoMode) {	
#endif			
			
			if (tm.LayoutMode == TTFText.LayoutModeEnum.No // One line mode
				|| ((align != TTFText.ParagraphAlignmentEnum.Justified) 
				     && (align != TTFText.ParagraphAlignmentEnum.FullyJustified))) { // do not justify				
				// => Xtra = (hspacing - 1) * Inside 
				Xtra = (tm.CurrentTextStyle.Hspacing - 1) * Inside;
			}
#if TTFTEXT_LITE											
			}
			else  {
					Xtra = (tm.CurrentTextStyle.Hspacing-1) * Inside;
			}
#endif			
			
			int nspaces = 0;
			// Xtra = XtraSpace + XtraAdd + XtraMul
			float XtraSpace = 0;
			float XtraAdd = 0;
			float XtraMul = 0;
			
			
			foreach (char c in line) {
				if (c == ' ') {
					++nspaces;
				}
			}
			
			float addSpace = 0;
			if (nspaces == 0) {
				XtraSpace = 0;
				XtraAdd = tm.CurrentTextStyle.HSpacingMultFactor * Xtra;
				XtraMul = (1 - tm.CurrentTextStyle.HSpacingMultFactor) * Xtra;
					
			} else {				
				XtraSpace = tm.CurrentTextStyle.WordSpacingFactor * Xtra;
				addSpace = XtraSpace / nspaces;
				XtraAdd = tm.CurrentTextStyle.HSpacingMultFactor * (1 - tm.CurrentTextStyle.WordSpacingFactor) * Xtra;
				XtraMul = (1 - tm.CurrentTextStyle.HSpacingMultFactor) * (1 - tm.CurrentTextStyle.WordSpacingFactor) * Xtra;
			}
			
			float addq = 0;
			float mulf = 1;
			
			if (line.Length > 1) {
				addq = XtraAdd / (line.Length - 1);
			}				
			if (Inside != 0) {
				mulf = (Inside + XtraMul) / Inside;
			}
			
			float pos = 0;
			for (int i = 0; i < line.Length; i++) {
				charpositions [i] = pos;
				pos += charadvances [i] * mulf + addq;
				if (line [i] == ' ') {
					pos += addSpace;
				}
			}
			
			advancelen = pos;
		}
		
		public void ComputeCharacterPositions ()
		{
			switch (tm.HSpacingMode) {
			case TTFText.HSpacingModeEnum.GlyphBoundaries:
				ComputeCharacterPositionsBoundaryBased ();
				break;
			case TTFText.HSpacingModeEnum.GlyphAdvance:			
				ComputeCharacterPositionsAdvanceBased ();
				break;
			}
		}
	}
	
#endregion	
	
}
