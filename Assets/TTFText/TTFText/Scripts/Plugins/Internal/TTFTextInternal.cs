//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
using TTF = FTSharp;
#else
public class TTF
{
	public class Font : System.Object
	{
	}
}
#endif




namespace TTFTextInternal
{



// TODO: Do AppendMetricInfo (Not only get metric info)
	public static class Engine
	{
		//byte [] b=System.Convert.FromBase64String()
		public static string open_markup = "<@";
		public static string close_markup = "@>";

		// TTFText Markup works as follow
		// <@Size=3@> to set size to 3
		// <@Size=3|Embold=*2@> to set size to 3, and multiply the embold by 2
		// <@Style=name_of_the style@>	
		// <@pop@> to return at the previous style
		// <@#Image(2,1,"texturename")@>
		// <@#L.AddImage(2,1,"texturename")@>
		
	
#region MAIN_FUNCTION	
		/// <summary>
		/// This is the main functon that is used to generate any text mesh
		/// </summary>
		/// <param name='tm'>
		/// The textmesh object to be updated
		/// </param>
		/// 
		/// 
		/// 
		/// 
		public static void BuildText (TTFText tm)
		{
			ResetTextMesh (tm);
			tm.ResetUsedStyles ();
			tm.CurrentTextStyle = tm.InitTextStyle;
		
			if (tm.TokenMode != TTFText.TokenModeEnum.Text) {
				BuildTextModeSubobjects (tm);							
			} else {			
				BuildTextModeText (tm);											
			}
		}

		public static IEnumerable BuildTextASync (TTFText tm)
		{
			ResetTextMesh (tm);
			tm.ResetUsedStyles ();
			tm.CurrentTextStyle = tm.InitTextStyle;
		
			if (tm.TokenMode != TTFText.TokenModeEnum.Text) {
				foreach (object o in BuildTextModeSubobjectsAsync (tm)) {
					yield return o;
				}
			} else {			
				foreach (object o in BuildTextModeTextAsync (tm)) {
					yield return o;
				}
			}
		}
		
		public static void BuildTextModeSubobjects (TTFText tm)
		{
#if TTFTEXT_LITE
			if (tm.DemoMode) {
#endif				
			// CASE 1 : WE GENERATE DIFFERENT GAME OBJECTS FOR EACH TOKEN			
			Bounds bounds = new Bounds (Vector3.zero, Vector3.zero);
								
			//BuildTextModeSubobjectsP1(tm,ref bounds);
			foreach (object o in BuildTextModeSubobjectsP1(tm)) {
				TTFTextInternal.Utilities.MergeBounds (ref bounds, (Bounds)o);
			}
			BuildTextModeSubobjectsP2 (tm, ref bounds);				
#if TTFTEXT_LITE
			}
#endif				
		}
	
		public static IEnumerable BuildTextModeSubobjectsAsync (TTFText tm)
		{
#if TTFTEXT_LITE
			if (tm.DemoMode) {
#endif				
			// CASE 1 : WE GENERATE DIFFERENT GAME OBJECTS FOR EACH TOKEN			
			Bounds bounds = new Bounds (Vector3.zero, Vector3.zero);
								
			foreach (object o in BuildTextModeSubobjectsP1(tm)) {
				TTFTextInternal.Utilities.MergeBounds (ref bounds, (Bounds)o);
				yield return o;		
			}
				
			BuildTextModeSubobjectsP2 (tm, ref bounds);
				
#if TTFTEXT_LITE
			}
#endif				
		}
		
		public static IEnumerable BuildTextModeSubobjectsP1 (TTFText tm)
		{	
			Bounds bounds = new Bounds (Vector3.zero, Vector3.zero);
			Vector3 otr = Vector3.zero;
			Vector3 pos = Vector3.zero;
			int idx = 0;
			int lineno = 0;
			float lm = tm.LineSpacingMult;
			float la = tm.LineSpacingAdd;
			
			foreach (TTFTextInternal.LineLayout ll in GenerateTextLayout(tm.Text,  tm)) {
				Bounds bl= new Bounds(Vector3.zero,Vector3.zero);
				// COMPUTE LINE POSITION	
				ll.lineno = lineno;				
				float msz = Mathf.Abs (ll.MaxCharacterSize);
				if (msz <= 0)
					msz = tm.Size;
				if ((ll.line.Length > 0) && (ll.charstyleindex [0] != -1)) {
					lm = ll.GetCharStyle (0).LineSpacingMult;
					la = ll.GetCharStyle (0).LineSpacingAdd;
				}

				if (lineno != 0) {
					pos += new Vector3 (0, -  (msz * lm + la), 0);				
				}
					
					
				//	BUILD TEXT				
//				int num;
				foreach( object tbl in BuildSubtext (ll, lineSplitters [(int)tm.TokenMode], idx, pos + new Vector3 (ll.offset, 0, 0), tm)) {
					bl=(Bounds)(tbl);
					TTFTextInternal.Utilities.MergeBounds (ref bounds, bl);
					
// do alignment 
				if (true) {
					TTFTextInternal.Utilities.MergeBounds (ref bounds, bl);
					Vector3 ootr=otr;
					Vector3 tr = Utilities.TranslateObjectFromBounds (tm, bounds); 
											
					Transform lt=null;
						
					foreach (Transform t in tm.transform) {
						t.localPosition += (tr - otr); // TODO ... TO BE CORRECTED !
						lt=t;						
						if (tm.gameObject) {
							TTFSubtext stx = t.gameObject.GetComponent<TTFSubtext> ();
							if (stx != null) {
								stx.LocalSoftPosition += (tr - otr);
							}
						}
					}
					otr = tr;	
					if (lt!=null) {
						lt.localPosition+=ootr;
						if (tm.gameObject) {
							TTFSubtext stx = lt.gameObject.GetComponent<TTFSubtext> ();
							if (stx != null) {
								stx.LocalSoftPosition += ootr;
							}
						}		
					}
				}
									
					yield return bl;								
					idx++;
				}
				
				
				
				// FINALIZE	
				tm.advance = Vector3.Max (tm.advance, ll.advance);
				lineno++;
	
				
				
				//yield return bl;
			}
		}
	
		public static void BuildTextModeSubobjectsP2 (TTFText tm, ref Bounds bounds)
		{
			//if (tm.rebuildLayout) {			
		//	Vector3 tr = Utilities.TranslateObjectFromBounds (tm, bounds); 
			foreach (Transform t in tm.transform) {
				//t.localPosition += tr;
				if (tm.gameObject) {
					//TTFSubtext stx = t.gameObject.GetComponent<TTFSubtext> ();
					//if (stx != null) {
					//	stx.LocalSoftPosition += tr;
					//}
#if UNITY_EDITOR					
						if (true) {
							MeshFilter smf=t.GetComponent<MeshFilter>();
							if (smf!=null) {
								Mesh stm =smf.sharedMesh;
								if (stm!=null) {
									tm.statistics_num_vertices+=(stm!=null)?stm.vertexCount:0;
									tm.statistics_num_subobjects++;
								}
							}
						}
#endif								
				}
			}
		}
		
		public static void BuildTextModeText (TTFText tm)
		{
			// CASE 2 : WE MERGE ALL THE LAYOUT WE HAVE GENERATED IN A SINGLE MESH	
			int nsubmeshes = ExpectedNumberOfSubmeshes (tm);
			List<CombineInstance []> cis = new List<CombineInstance[]> ();			
			
			BuildTextModeTextP1 (tm, nsubmeshes, ref cis);
			BuildTextModeTextP2 (tm, nsubmeshes, ref cis);
			
			
		}

		public static IEnumerable BuildTextModeTextAsync (TTFText tm)
		{
			// CASE 2 : WE MERGE ALL THE LAYOUT WE HAVE GENERATED IN A SINGLE MESH	
			int nsubmeshes = ExpectedNumberOfSubmeshes (tm);
			List<CombineInstance []> cis = new List<CombineInstance[]> ();			
			
			List<TTFTextInternal.LineLayout> lls = new List<TTFTextInternal.LineLayout> (GenerateTextLayout (tm.Text, tm));
			for (int i=0; i<nsubmeshes; i++) {
				cis.Add (new CombineInstance[lls.Count]);
			}
			
			float lm = tm.LineSpacingMult;
			float la = tm.LineSpacingAdd;
			int idx = 0;
			
			foreach (TTFTextInternal.LineLayout ll in lls) {				
				Vector3 adv;
				Mesh mesh = new Mesh ();								
				BuildMesh (ref mesh, ll, tm, out adv);
				yield return mesh;
				
				float msz = Mathf.Abs (ll.MaxCharacterSize);
								
				if (msz <= 0)
					msz = tm.Size;
				
				if ((ll.line.Length > 0) && (ll.charstyleindex != null) && (ll.charstyleindex [0] != -1)) {
					lm = ll.GetCharStyle (0).LineSpacingMult;
					la = ll.GetCharStyle (0).LineSpacingAdd;
				}
				
				Vector3 offset = new Vector3 (ll.offset, - (msz * lm + la) * idx, 0); // we offseteach line vertically
				
				for (int i=0; i<nsubmeshes; i++) { 
					cis [i] [idx].mesh = mesh; 
					cis [i] [idx].subMeshIndex = i;
					cis [i] [idx].transform = Matrix4x4.TRS (offset, Quaternion.identity, new Vector3 (1, 1, 1));					
				}
				idx++;								   
				tm.advance = Vector3.Max (tm.advance, ll.advance);
					
				yield return null;
			}

			BuildTextModeTextP2 (tm, nsubmeshes, ref cis);
		}
		
		public static void BuildTextModeTextP1 (TTFText tm, int nsubmeshes, ref List<CombineInstance []> cis)
		{
			List<TTFTextInternal.LineLayout> lls = new List<TTFTextInternal.LineLayout> (GenerateTextLayout (tm.Text, tm));
			for (int i=0; i<nsubmeshes; i++) {
				cis.Add (new CombineInstance[lls.Count]);
			}
			
			float lm = tm.LineSpacingMult;
			float la = tm.LineSpacingAdd;			
			int idx = 0;
			
			foreach (TTFTextInternal.LineLayout ll in lls) {				
				Vector3 adv;
				Mesh mesh = new Mesh ();								
				BuildMesh (ref mesh, ll, tm, out adv);
				
				float msz = Mathf.Abs (ll.MaxCharacterSize);
								
				if (msz <= 0)
					msz = tm.Size;
				
				if ((ll.line.Length > 0) && (ll.charstyleindex != null) && (ll.charstyleindex [0] != -1)) {
					lm = ll.GetCharStyle (0).LineSpacingMult;
					la = ll.GetCharStyle (0).LineSpacingAdd;
				}
				
				Vector3 offset = new Vector3 (ll.offset, - (msz * lm + la) * idx, 0); // we offseteach line vertically
				
				for (int i=0; i<nsubmeshes; i++) { 
					cis [i] [idx].mesh = mesh; 
					cis [i] [idx].subMeshIndex = i;
					cis [i] [idx].transform = Matrix4x4.TRS (offset, Quaternion.identity, new Vector3 (1, 1, 1));					
				}
				
				
				idx++;								   
				tm.advance = Vector3.Max (tm.advance, ll.advance);
				
			}
			
		}

		public static void BuildTextModeTextP2 (TTFText tm, int nsubmeshes, ref List<CombineInstance []> cis)
		{		
			if (tm.mesh == null) {
				tm.mesh = new Mesh ();
			}			
			tm.mesh.Clear ();
			
			CombineInstance [] tci = new CombineInstance[nsubmeshes];
			for (int i=0; i<nsubmeshes; i++) {
				tci [i].mesh = new Mesh ();
				tci [i].mesh.CombineMeshes (cis [i], true, true);
				tci [i].subMeshIndex = 0;
				tci [i].transform = Matrix4x4.identity;
			}
			tm.mesh.CombineMeshes (tci, false, false);	
			
			// Free intermediate meshes
			for (int i = 0; i < nsubmeshes; ++i) {
				foreach (CombineInstance c in cis[i]) {
					Utilities.DestroyObj (c.mesh);
				}
				Utilities.DestroyObj (tci [i].mesh);
			}
			
			tm.mesh.name = "TTF Text AutoGenerated";			
			tm.mesh.RecalculateBounds ();			
			
			Vector3 tr = Utilities.TranslateObjectFromBounds (tm, tm.mesh.bounds); 
			TTFTextInternal.Utilities.TranslateMesh (tm.mesh, tr);			
			tm.mesh.Optimize (); 
#if UNITY_EDITOR							
				tm.statistics_num_vertices+=tm.mesh.vertexCount;
				tm.statistics_num_subobjects++;
#endif								
			
			UpdateComponentsWithNewMesh (tm.gameObject, tm.mesh);
		
#endregion
		}
		
	
#region LAYOUT_RELATED_FUNCTIONS
		/// <summary>
		/// The layout atoms are line
		/// </summary>
		/// <returns>
		/// The one per line.
		/// </returns>
		/// <param name='l'>
		/// L.
		/// </param>
		static IEnumerable<TTFTextInternal.LineLayout> EOnePerLine (TTFTextInternal.LineLayout l)
		{
			yield return l;
		}
		
		
		
		/// <summary>
		/// Subdivide lines into words
		/// </summary>
		/// <returns>
		/// The one per line.
		/// </returns>
		/// <param name='l'>
		/// L.
		/// </param>	
		static IEnumerable<TTFTextInternal.LineLayout> EOnePerWord (TTFTextInternal.LineLayout l)
		{

			char[] seps = new char[] { ' ' };
			string[] words = l.line.Split (seps, System.StringSplitOptions.RemoveEmptyEntries);
		
			int p = 0;
			float sa = 0;
			foreach (string w in words) {			
				int wlen = w.Length;
				int p0 = p;
			
				while (l.line[p] != w[0])
					p++; // <- skipping spaces : BUG TO BE FIXED !
		    
				TTFTextInternal.LineLayout r = new TTFTextInternal.LineLayout (w, l.tm, false);
				r.prevadvance = l.prevadvance + sa;
				if (wlen > 0) {
					r.linewidth = l.GetCharStyle (p).LineWidth;
				}
				r.charadvances = new float[w.Length];
				r.charpositions = new float[w.Length];
				r.charstyleindex = new int[w.Length];
				r.charmetadata = new object[w.Length];
			
				for (int i = 0; i < wlen; i++) {
					r.charadvances [i] = l.charadvances [p + i];	
					r.charpositions [i] = l.charpositions [p + i] - l.charpositions [p0];		
					r.charstyleindex [i] = l.charstyleindex [p + i];	
					r.charmetadata [i] = l.charmetadata [p + i];
				}
			
				if (p + wlen < l.line.Length) {
					r.advancelen = l.charpositions [p + wlen] - l.charpositions [p0];
				} else {
					r.advancelen = l.advancelen - l.charpositions [p0];
				}
				r.lineno = l.lineno;
				yield return r;
				sa += r.advancelen;
				p += wlen;
			}
		}
	
	
		/// <summary>
		/// Subdivides line into individual characters
		/// </summary>
		/// <returns>
		/// The one per char.
		/// </returns>
		/// <param name='l'>
		/// L.
		/// </param> 
		static IEnumerable<TTFTextInternal.LineLayout> EOnePerChar (TTFTextInternal.LineLayout l)
		{		
			
			int i = 0;
			float sa = 0;
			foreach (char c in l.line) {
				TTFTextInternal.LineLayout r = new TTFTextInternal.LineLayout ("" + c, l.tm, false);		
				r.charadvances = new float[1];
				r.charpositions = new float[1];												
				r.charstyleindex = new int[1];	
				r.charmetadata = new object[1];
				r.charadvances [0] = l.charadvances [i];	
				r.charpositions [0] = 0;	
				r.charstyleindex [0] = l.charstyleindex [i];	
				r.charmetadata [0] = l.charmetadata [i];
				r.prevadvance = l.prevadvance + sa;
				r.linewidth = l.GetCharStyle (i).LineWidth;
				r.lineno = l.lineno;
				if (i + 1 < l.line.Length) {
					r.advancelen = l.charpositions [i + 1] - l.charpositions [i];
				} else {
					r.advancelen = l.advancelen - l.charpositions [i];					
				}
				yield return r;
				sa += r.advancelen;
			
				i += 1;
			}
		} 
	
	
	
	
		/// <summary>
		/// We declare an array of delegate for the different modes that we support
		/// </summary>
		public delegate IEnumerable<TTFTextInternal.LineLayout> LineSplitter (TTFTextInternal.LineLayout l);

		static LineSplitter[] lineSplitters = {EOnePerLine, EOnePerLine,EOnePerWord,EOnePerChar};
	
	
	
	
	
		/// <summary>
		/// Wraps a paragraphs that does not contain any markup.
		/// </summary>
		/// <returns>
		/// The paragraphs simple.
		/// </returns>
		/// <param name='text'>
		/// The Text to be wrapped up.
		/// </param>
		/// <param name='tm'>
		/// The textmesh object associated with it.
		/// </param>
		public static IEnumerable<TTFTextInternal.LineLayout> WrapParagraphsSimple (string text, TTFText tm)
		{	
			// This function proceeds as follow it generates outline adding words one by one
			// if the line becomes too long for the desired textwidth it starts a new line
		 
			float d;
			int lno = 0;
			char[] lseps = new char[] { '\n' };
			char[] wseps = new char[] { ' ' };
			string [] paragraphs = text.Split (lseps);
		
			if (paragraphs.Length == 0) {
				yield break;
			}
			float firstlineoffset = 0;
			if ((tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Right) && (tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Center)) {
				firstlineoffset = tm.FirstLineOffset;
			}
		
			foreach (string paragraph in paragraphs) {
		
			
				string[] words = null;
		
				if (tm.WordSplitMode == TTFText.WordSplitModeEnum.SpaceBased) {
					words = paragraph.Split (wseps, System.StringSplitOptions.RemoveEmptyEntries);
				}	
				if (tm.WordSplitMode == TTFText.WordSplitModeEnum.Character) {				
					words = new string[paragraph.Length];
					int ci = 0;
					foreach (char c in paragraph) {
						words [ci] = "" + c;
						ci++;
					}
				}
			
			
				if (words.Length == 0) {
					yield return new TTFTextInternal.LineLayout("",tm,true);
					if ((tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Right) && (tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Center)) {
						firstlineoffset = tm.FirstLineOffset;
					}
				} else {
						
					TTFTextInternal.LineLayout ll = new TTFTextInternal.LineLayout (words [0], tm, true);
	    
					for (int idx = 1; idx < words.Length; ++idx) {			
						string w = words [idx];
						string otext = ll.line;
						if (tm.WordSplitMode == TTFText.WordSplitModeEnum.SpaceBased) {
							ll.AppendText (" " + w);
						} else {
							ll.AppendText (w);
						}
						
				
						d = ll.GetDefaultLineWidth (ll.hspacing);
			
						if (d > (ll.linewidth - firstlineoffset)) {
							ll.RewindLine (otext);
							ll.offset = 0;
						
							if (((ll.align != TTFText.ParagraphAlignmentEnum.Right))		
					&& ((ll.align != TTFText.ParagraphAlignmentEnum.Center))) {
								ll.offset = firstlineoffset;		
							}
							ll.ComputeCharacterPositions ();
		
							if ((ll.align == TTFText.ParagraphAlignmentEnum.Right)) {
								// Debug.Log(System.String.Format("{0} {1}",ll.linewidth, ll.GetActualLinewidth()));
								ll.offset = ll.linewidth - ll.GetActualLinewidth ();
							} else {
								if ((ll.align == TTFText.ParagraphAlignmentEnum.Center)) {
									ll.offset = (ll.linewidth - ll.GetActualLinewidth ()) / 2;
								} else {
									ll.offset = firstlineoffset;
								}
							}
				
							ll.lineno = lno++;
							yield return ll;
							firstlineoffset = 0;
							ll = new TTFTextInternal.LineLayout (w, tm, true);
				
						} 
					}
		
					// last line
					if ((tm.ParagraphAlignment == TTFText.ParagraphAlignmentEnum.Justified)) {
						ll.align = TTFText.ParagraphAlignmentEnum.Left;
					}
		
					ll.offset = 0;
					if (((ll.align != TTFText.ParagraphAlignmentEnum.Right))		
					&& ((ll.align != TTFText.ParagraphAlignmentEnum.Center))) {
						ll.offset = firstlineoffset;		
					}
				
					ll.ComputeCharacterPositions ();								
					if ((tm.ParagraphAlignment == TTFText.ParagraphAlignmentEnum.Right)) {
						ll.offset = ll.linewidth - ll.GetActualLinewidth ();
					} else {
						if ((tm.ParagraphAlignment == TTFText.ParagraphAlignmentEnum.Center)) {
							ll.offset = (ll.linewidth - ll.GetActualLinewidth ()) / 2;
						} else {
							ll.offset = firstlineoffset;
						}
					
					}
				
					ll.lineno = lno++;
					yield return ll;
					if ((tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Right) && (tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Center)) {
						firstlineoffset = tm.FirstLineOffset;
					}		
				}
			}
		}
	
		static void FinalizeLL (ref TTFTextInternal.LineLayout ll, float firstlineoffset)
		{
			ll.offset = 0;

			
			if (((ll.align != TTFText.ParagraphAlignmentEnum.Right))		
					&& ((ll.align != TTFText.ParagraphAlignmentEnum.Center))) {
				ll.offset = firstlineoffset;		
			}
			ll.ComputeCharacterPositions ();
		
			if ((ll.align == TTFText.ParagraphAlignmentEnum.Right)) {
//					Debug.Log(System.String.Format("{0} {1}",ll.linewidth, ll.GetActualLinewidth()));
				ll.offset = ll.linewidth - ll.GetActualLinewidth ();
			} else {
				if ((ll.align == TTFText.ParagraphAlignmentEnum.Center)) {
					ll.offset = (ll.linewidth - ll.GetActualLinewidth ()) / 2;
				} else {
					ll.offset = firstlineoffset;								
				}
									
			}		
		}
	
	
	
	
	
	
	
	
	
	
		/// <summary>
		/// Wraps a paragraph in a text that possibly contains some markup
		/// </summary>
		/// <returns>
		/// Description of linelayout.
		/// </returns>
		/// <param name='text'>
		/// The text to be wrapped
		/// </param>
		/// <param name='tm'>
		/// The textmesh object that contains information about style and rendering
		/// </param>
		public static IEnumerable<TTFTextInternal.LineLayout> WrapParagraphsExt (string text, TTFText tm)
		{
			float d;
			int lno = 0;
			char[] lseps = new char[] { '\n' };
			char[] wseps = new char[] { ' ' };
			string [] paragraphs = text.Split (lseps);		
			if (paragraphs.Length == 0) {
				yield break;
			}
		
		
			float firstlineoffset = 0;
			if ((tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Right) && (tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Center)) {
				firstlineoffset = tm.FirstLineOffset;
			}
		
			foreach (string paragraph in paragraphs) {		
				System.Collections.Generic.List<TTFTextScriptTextToken> ttstts = new System.Collections.Generic.List<TTFTextScriptTextToken> (ParseScriptText (paragraph));	
				string otext;
			
			
				//string[] words = paragraph.Split(wseps, System.StringSplitOptions.RemoveEmptyEntries);
				int ntexts = 0;
				foreach (TTFTextScriptTextToken t in ttstts) {
					if (!t.is_script)
						ntexts++;
				}
			
				//return new TTFTextInternal.LineLayout(words[0],tm,true);
				if (ntexts == 0) {
					yield return new TTFTextInternal.LineLayout("",tm,true);
					if ((tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Right) && (tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Center)) {
						firstlineoffset = tm.FirstLineOffset;
					}
				} else {
					TTFTextInternal.LineLayout ll = new TTFTextInternal.LineLayout ("", tm, true);	    
				
					foreach (TTFTextScriptTextToken t in ttstts) {			
						if (t.is_script) {
							otext = ll.line;
							EvalEasyMarkUp (tm, t.text, ref ll);
							d = ll.GetDefaultLineWidth (ll.hspacing); // TODO: <- THIS IS WRONG !			
							if (d > (ll.linewidth - firstlineoffset)) {
								ll.RewindLine (otext);
								FinalizeLL (ref ll, firstlineoffset);								
								ll.lineno = lno++;
								yield return ll;
								firstlineoffset = 0;
								ll = new TTFTextInternal.LineLayout ("", tm, true);
								EvalEasyMarkUp (tm, t.text, ref ll);
							}														
						} else {
									
							string[] words = null;
		
							if (tm.CurrentTextStyle.WordSplitMode == TTFText.WordSplitModeEnum.SpaceBased) {
								words = t.text.Split (wseps, System.StringSplitOptions.RemoveEmptyEntries);
							}
						
							if (tm.CurrentTextStyle.WordSplitMode == TTFText.WordSplitModeEnum.Character) {				
								words = new string[t.text.Length];
								int ci = 0;
								foreach (char c in t.text) {
									words [ci] = "" + c;
									ci++;
								}
							}

						
							foreach (string w in words/*t.text.Split(wseps)*/) {
								otext = ll.line;
								if (otext.Length != 0) {
									if (tm.CurrentTextStyle.WordSplitMode == TTFText.WordSplitModeEnum.SpaceBased) {
										ll.AppendText (' ' + w);
									} else {
										ll.AppendText (w);
									}
								} else {
									ll.AppendText (w);								
								}
							
							
							
								d = ll.GetDefaultLineWidth (ll.hspacing); // TODO: <- THIS IS WRONG !			
								if (d > (ll.linewidth - firstlineoffset)) {
									ll.RewindLine (otext);								
									FinalizeLL (ref ll, firstlineoffset);								
									ll.lineno = lno++;
									yield return ll;
									firstlineoffset = 0;
									ll = new TTFTextInternal.LineLayout (w, tm, true);
								} 
							}
						}
					}
		
					// last line
					if ((tm.ParagraphAlignment == TTFText.ParagraphAlignmentEnum.Justified)) {
						ll.align = TTFText.ParagraphAlignmentEnum.Left;
					}
		
					ll.offset = 0;
					if (((ll.align != TTFText.ParagraphAlignmentEnum.Right))		
					&& ((ll.align != TTFText.ParagraphAlignmentEnum.Center))) {
						ll.offset = firstlineoffset;		
					}
				
					ll.ComputeCharacterPositions ();								
					if ((tm.ParagraphAlignment == TTFText.ParagraphAlignmentEnum.Right)) {
						ll.offset = ll.linewidth - ll.GetActualLinewidth ();
					} else {
						if ((tm.ParagraphAlignment == TTFText.ParagraphAlignmentEnum.Center)) {
							ll.offset = (ll.linewidth - ll.GetActualLinewidth ()) / 2;
						} else {
							ll.offset = firstlineoffset;								
						}
					}
					ll.lineno = lno++;
				
					yield return ll;
					if ((tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Right) && (tm.ParagraphAlignment != TTFText.ParagraphAlignmentEnum.Center)) {
						firstlineoffset = tm.FirstLineOffset;
					}		
				}
			
			}
		}
	
	
	
		/// <summary>
		/// Our minimal lexer output elements of this type
		/// </summary>	
		public class TTFTextScriptTextToken
		{
			public bool is_script;
			public string text;
		}
	
	
		/// <summary>
		/// This functions creates "text tokens" consisting of block of normal text
		/// interweave with normal text.
		/// </summary>	
		/// <example>
		/// text <@#S.Push().SetSlant(0.3f)@> continue <@#S.Pop()@>
		/// text <@#S.get_style("/template/TTFred")@> truc
		/// </example>
		static public IEnumerable<TTFTextScriptTextToken> ParseScriptText (string s)
		{
			int state = 0;
			// 0 : text
			// 1 : 0+@
			// 2 : 0+\
			// 3 : code
			// 4 : 3 + }
			// 5 : 3 + \
			string restext = "";
			foreach (char c in s) { 
				switch (state) {
				case 0:
					if (c == open_markup [0]) {
						state = 1;
					} else {
						if (c == '\\') {
							state = 2;
						} else {
							restext += c;
						}
					}
					break;
				case 1:
					if (c == open_markup [1]) {
						state = 3;					
						if (restext.Length != 0) {
							TTFTextScriptTextToken ttstt = new TTFTextScriptTextToken ();
							ttstt.text = restext;
							ttstt.is_script = false;
							yield return ttstt;
						}
						restext = "";	
					} else {
						state = 0;
						restext += open_markup [0] + c;
					}
					break;
				case 2:
					state = 0;
					restext += c;
					break;
				
				case 3:
					if (c == close_markup [0]) {
						state = 4;
					} else {
						if (c == '\\') {
							state = 5;
						} else {
							restext += c;
						}
					}
					break;
				case 4:
					if (c == close_markup [1]) {
						state = 0;
						if (restext.Length != 0) {
							TTFTextScriptTextToken ttstt = new TTFTextScriptTextToken ();
							ttstt.text = restext;
							ttstt.is_script = true;
							yield return ttstt;
						}
						restext = "";	
					} else {
						state = 3;
						restext += close_markup [0] + c;
					}
					break;
				case 5:
					state = 3;
					restext += c;
					break;
				
				}
			}
			if (state != 0) {
				throw new System.Exception ("Parse error");
			}
			if (restext.Length != 0) {
				TTFTextScriptTextToken ttstt = new TTFTextScriptTextToken ();
				ttstt.text = restext;
				ttstt.is_script = false;
				yield return ttstt;			
			}
		
		}
	
		
		
		
		
		
		/// <summary>
		/// The following script text is experimental
		/// it is based on JyC. 
		/// It provide direct access to C# methods and objects
		/// but it may not be the most intuitive for our users
		/// It is accessbile through tags <@# .... @>
		/// 
		/// </summary>	
		public static void EvalScriptText (TTFText tm, string s, ref TTFTextInternal.LineLayout ll)
		{
#if! TTFTEXT_LITE

			//
			TTFTextStyle ts = tm.CurrentTextStyle;
			Jyc.Expr.Parser ep = new Jyc.Expr.Parser ();
			Jyc.Expr.Tree tree;
			try {
				tree = ep.Parse (s);
				Jyc.Expr.Evaluator evaluater = new Jyc.Expr.Evaluator (); 
				Jyc.Expr.ParameterVariableHolder pvh = new Jyc.Expr.ParameterVariableHolder ();
				pvh.Parameters ["T"] = pvh.Parameters ["text"] = new Jyc.Expr.Parameter (tm);
				pvh.Parameters ["S"] = pvh.Parameters ["style"] = new Jyc.Expr.Parameter (ts);
				pvh.Parameters ["L"] = pvh.Parameters ["linelayout"] = new Jyc.Expr.Parameter (ll);
				//pvh.Parameters ["I"] = pvh.Parameters ["img"] = new Jyc.Expr.Parameter(TTFTextInternal.Get);
				evaluater.VariableHolder = pvh;
				ts = (TTFTextStyle)evaluater.Eval (tree);
				if (ts != null) {
					//Debug.Log (	ts.Size);
					tm.CurrentTextStyle = ts;
				}
			} catch (System.Exception e) {
				Debug.LogError (e);
			}			
#endif				

		}
	
		private const string tpn_float = "Single";
		private const string tpn_bool = "Bool";
		private const string tpn_int = "Int";
		private const string tpn_string = "String";
	
		
		
		
		// EASY MARKUP
		public static void EvalEasyMarkUp (TTFText tm, string bs, ref TTFTextInternal.LineLayout ll)
		{
//			Debug.Log("Eval "+bs);
#if TTFTEXT_LITE
			if (tm.DemoMode) {
#endif				

			TTFTextStyle ts = tm.CurrentTextStyle;
			try {
				bs = bs.Trim ();
				if (bs [0] == '#') {
					EvalScriptText (tm, bs.Substring (1), ref ll);
				}
				char [] separators = {'|'};	
				
				foreach (string s in bs.Split(separators)) {				
					if (s.Contains ("=")) {				
						char [] seps = {'='};
						string [] vals = s.Split (seps);
						string lval = vals [0].Trim ();
						string rval = vals [1].Trim ();
				
						if (lval == "style") {
							if (rval.Contains ("<")) {
								// probably invalid
								return;
							}
							ts = ts.PushF (rval, tm.autoCreateStyles, ref tm.nonFoundStyles);
						} else {
							if (!lval.Contains ("#")) {
								// we modify our component
								System.Type tp = typeof(TTFTextStyle);						
								System.Reflection.PropertyInfo pi = tp.GetProperty (lval);
					
								string tpn = pi.PropertyType.Name;
						
								ts = ts.Push ();
								if (tpn == tpn_float) {							
									if (rval [0] == '+') {
										pi.SetValue (ts, ((float)pi.GetValue (ts, null)) + System.Single.Parse (rval.Substring (1)), null);
									} else {
										if (rval [0] == '*') {
											pi.SetValue (ts, ((float)pi.GetValue (ts, null)) * System.Single.Parse (rval.Substring (1)), null);
										} else {
											pi.SetValue (ts, System.Single.Parse (rval), null);
										}
									}
								} else {						
									if (tpn == tpn_bool) {
										pi.SetValue (ts, System.Boolean.Parse (rval), null);
									} else {						
										if (tpn == tpn_int) {								
											pi.SetValue (ts, System.Int32.Parse (rval), null);								
										} else {
											if (tpn == tpn_string) {
												pi.SetValue (ts, rval, null);								
											} else {
												Debug.LogWarning (pi.PropertyType.Name + " : Behaviour not defined");
											}
										}
									}
						
								}	
							} else {
								int isharp = lval.IndexOf ('#');
								string comppart = lval.Substring (0, isharp);
								string fieldname = lval.Substring (isharp + 1);
								object o = (object)tm.gameObject.GetComponent (comppart);
								System.Type tp = o.GetType ();						
								System.Reflection.FieldInfo fi = tp.GetField (fieldname);
					
								string tpn = fi.FieldType.Name;
						
								if (tpn == tpn_float) {							
									if (rval [0] == '+') {
										fi.SetValue (o, ((float)fi.GetValue (o)) + System.Single.Parse (rval.Substring (1)));
									} else {
										if (rval [0] == '*') {
											fi.SetValue (o, ((float)fi.GetValue (o)) * System.Single.Parse (rval.Substring (1)));
										} else {
											fi.SetValue (o, System.Single.Parse (rval));
										}
									}
								} else {						
									if (tpn == tpn_bool) {
										fi.SetValue (o, System.Boolean.Parse (rval));
									} else {						
										if (tpn == tpn_int) {								
											fi.SetValue (o, System.Int32.Parse (rval));								
										} else {
											if (tpn == tpn_string) {
												fi.SetValue (o, rval);								
											} else {
												Debug.LogWarning (fi.FieldType.Name + " : Behaviour not defined");
											}
										}
									}
						
								}										
							}
						}
				
					} else {
						if ((s.ToLower () == "end") || (s.ToLower () == "pop")) {
							ts = ts.Pop ();
						} else {
							Debug.LogError ("(TTFText) Unknown command : " + s);
						}
					}
//					Debug.Log("Setting ts fontid :  "+ts.FontId+" getid : " +ts.getid());
					tm.CurrentTextStyle = ts;
				}
			} catch (System.Exception e) {
				Debug.LogError (e);
			}			
#if TTFTEXT_LITE
		}
#endif				

		}
	
		public static IEnumerable<TTFTextInternal.LineLayout> GenerateTextLayout (string text, TTFText tm)
		{				
			switch (tm.LayoutMode) {
			case TTFText.LayoutModeEnum.No:
				char [] seps = {'\n'};
				int lineno = 0;
				foreach (string s in text.Split(seps)) {
					TTFTextInternal.LineLayout l = new TTFTextInternal.LineLayout (s, tm, true);
					l.offset = 0;
					l.lineno = lineno++;
					l.advancedir = Vector3.right;
					yield return l;
				}
				break;
			case TTFText.LayoutModeEnum.Wrap:
#if TTFTEXT_LITE
			if (tm.DemoMode) {
#endif				
			
				foreach (TTFTextInternal.LineLayout ll in WrapParagraphsSimple(text, tm)) {
					yield return ll;
				}
#if TTFTEXT_LITE
			}
#endif				
				
				break;
			case TTFText.LayoutModeEnum.StyleEnabled:
#if TTFTEXT_LITE
			if (tm.DemoMode) {
#endif				
			
				foreach (TTFTextInternal.LineLayout ll in WrapParagraphsExt(text, tm)) {
					yield return ll;
				}
#if TTFTEXT_LITE
			}
#endif				
				
				break;			
			}
		}

#endregion
	
	
		// THIS FUNCTION BUILD INDIVIDUAL SUBOBJECTS FOR EACH OBJECT		
		public static IEnumerable BuildSubtext (TTFTextInternal.LineLayout ll,
									        LineSplitter split, 
		                                    int idx, 
											Vector3 pos, 
											TTFText tm)
											//out Bounds bounds, 
											//out int num)
		{				
			Vector3 adv = Vector3.zero;
			Bounds bounds = new Bounds (Vector3.zero, Vector3.zero);				
			int num = 0;
			
			
			
			
			foreach (TTFTextInternal.LineLayout l in split(ll)) {
				//Debug.Log("ll=" + l.line + " pos=" + pos + " charpos0=" + l.charpositions[0] + " advlen=" + l.advance);			
				GameObject go;
				
				
				
				// instantiate the letter
				bool b0 = ((l.line.Length > 0) 
					    && (l.GetCharStyle (0).GlyphPrefab != null));
				if (b0 || (tm.GlyphPrefab != null)) {
					if (b0) {
						go = GameObject.Instantiate (l.GetCharStyle (0).GlyphPrefab) as GameObject;
					} else {
						go = GameObject.Instantiate (tm.GlyphPrefab) as GameObject;
					}
				} else {
					go = new GameObject ();
					go.AddComponent<MeshFilter> ();
					go.AddComponent<MeshRenderer> ();
				
					if (tm.gameObject.renderer != null) {
						
						if (tm.gameObject.renderer.sharedMaterials != null) {
							if ((tm.gameObject.renderer.sharedMaterials.Length != 0) && (tm.gameObject.renderer.sharedMaterials [0] != null)) {
								go.renderer.sharedMaterials = tm.gameObject.renderer.sharedMaterials;
							}
						} else {
							if (tm.gameObject.renderer.sharedMaterial != null) {
								go.renderer.sharedMaterial = tm.gameObject.renderer.sharedMaterial;
							}	
						}
					}					
				}
			
				
			
				// ---------------------------------------------------------------------
				// set up the letter/text positon and scale
				// ---------------------------------------------------------------------
				int no = idx + num;
				go.name = System.String.Format ("{0}. {1}", no, l.line);
				go.transform.parent = tm.transform;

				if (/*tm.rebuildLayout ||*/ (!tm.SaveTokenPos) || tm.TokenPos.Count <= no) {				
					go.transform.localPosition = pos;
					go.transform.localRotation = Quaternion.identity;				
				} else {
					TTFText.TrInfo tr = tm.TokenPos [no];
					go.transform.localPosition = tr.localPosition;
					go.transform.localRotation = tr.localRotation;
					go.transform.localScale = tr.localScale;
				}
				if (l.line.Length > 0) {
					if ((l.GetCharStyle (0).sharedMaterials != null)
					&& (l.GetCharStyle (0).sharedMaterials.Length > 0)
					&& (l.GetCharStyle (0).sharedMaterials [0] != null)) {
						go.renderer.sharedMaterials = l.GetCharStyle (0).sharedMaterials;
					}
				}
			
				
				
				// ----------------------------------------------------------
				// build the mesh associated with letter/text
				// ----------------------------------------------------------			
				Mesh mesh = new Mesh ();
				TTFTextInternal.Engine.BuildMesh (ref mesh, l, tm, out adv);
				Bounds b = mesh.bounds;
				b.center = b.center + pos;
				TTFTextInternal.Utilities.MergeBounds (ref bounds, b);				

				
				
				// --------------------------------------------------------------------------
				// set up some metadata so that the script attached to the letters may update
				// --------------------------------------------------------------------------
#if TTFTEXT_LITE
			if (tm.DemoMode) {
#endif				

				TTFSubtext st = go.GetComponent<TTFSubtext> ();
				if (st == null) {
					st = go.AddComponent<TTFSubtext> ();
				}
			
#if !TTFTEXT_LITE				
				st.Layout = l;
#endif				
				st.Text = l.line;
				st.LineNo = l.lineno;
				st.SequenceNo = idx + num;
				st.Advance = adv;
#if TTFTEXT_LITE
			}
#endif				
				
				
				
				// ---------------------------------------------------------------------
				//
				// IF A SPECIAL TEXTURE IS AFFECTED TO THIS CHARACTER THIS IS THE MOMENT
				//
				// ---------------------------------------------------------------------
				if (l.line.Length >= 1) {					
					if (((l.charmetadata [0] as TextureElement) != null)) {
						TextureElement tel = (l.charmetadata [0] as TextureElement);
						Material m = tel.material;
						Material [] mx = new Material[1];
						
						if (m == null) {
							//m=new Material(Shader.Find("GUI/Text Shader"));
							m = new Material (Shader.Find ("Self-Illumin/Diffuse"));
							m.name = "[generated material for text]";
							m.mainTexture = tel.texture;
						}

						mx [0] = m;
						go.renderer.sharedMaterials = mx;
							
						TTFTextReleaseTempResources rmod = go.GetComponent<TTFTextReleaseTempResources> ();
						if (rmod == null) {
							rmod = go.AddComponent<TTFTextReleaseTempResources> ();
						}
						
						if ((tel.material == null) && (tel.shouldReleaseMaterial)) {
							rmod.material = m;
						}
						if (tel.shouldReleaseTexture) {
							rmod.texture = tel.texture;
							
						}
					} else {
				
						int cmo = l.GetCharStyle (0).MaterialOffset;
						if (cmo != 0) {
							//Debug.Log(cmo);
							if (go.renderer != null) {
								if (go.renderer.sharedMaterials != null) {
									int ml = go.renderer.sharedMaterials.Length;
									int tml = 1;
									if (l.GetCharStyle (0).SplitSides) {
										tml = 3;
										TTFText.ExtrusionModeEnum em = l.GetCharStyle (0).ExtrusionMode;
										if (em == TTFText.ExtrusionModeEnum.None) {
											tml = 1;
											if (l.GetCharStyle (0).BackFace) {
												tml += 1;
											}

										}
										if (em == TTFText.ExtrusionModeEnum.Pipe) {
											tml = 1;
										}
									}
									int mo = ((cmo % ml) + ml) % ml;
									Material [] omx = go.renderer.sharedMaterials;
									Material [] nmx = new Material[tml];
									//Debug.Log(mo);
									for (int i=0; i<tml; i++) {
										nmx [i] = omx [(i + mo) % ml];
									}
									go.renderer.sharedMaterial = nmx [0];
									//go.renderer.sharedMaterials=new Material[0] {nmx[0]};
									//go.renderer.
									go.renderer.sharedMaterials = nmx;
									
								}
							}
						}
					
					}

				}	
			
				//
				// time to update every one
				//
				UpdateComponentsWithNewMesh (go, mesh);
				
				
				
				pos += l.advance;
				num++;
				yield return bounds;
			}
		}

		
		
		
		
		
		
		
		
		
#region FUNCTIONS_GENERATING_EXTRUSIONS_AND_MESHES_FOR_THE_WHOLE_TEXT
		
		
		public static Mesh BuildMesh (ref Mesh mesh, TTFTextInternal.LineLayout ll, TTFText tm, out Vector3 advance)
		{
		
			Mesh front = null;
			Mesh back = null;
			TTFTextOutline[] outlines = {};
			bool[] mask;
			advance = Vector3.zero;
		
			TTFTextStyle ts = tm.InitTextStyle;
			object charmetadata = null;
		
			if ((ll.charstyleindex != null) && (ll.charstyleindex [0] != -1)) {
				ts = ll.GetCharStyle (0);			
				charmetadata = ll.charmetadata [0];
			}
		
		
		
			if ((charmetadata != null) || (ts.ExtrusionMode == TTFText.ExtrusionModeEnum.None)) { // No extrusion		
				TTFTextOutline o = MakeOutline (ll, ts.Embold, tm);
				charmetadata = ll.charmetadata [0];
				o = o.Simplify (ts.SimplifyAmount, ts.Size);
				if (ts.Slant != 0) {
					o.Slant (ts.Slant);
				}
				advance = o.advance;		
				front = TTFTextInternal.Tesselators.Triangulate (o, tm.DynamicTextRuntimeTriangulationMethod == TTFText.DynamicTextRuntimeTriangulationMethodEnum.CSharpLibs);
			
				if (tm.DynamicTextRuntimeTriangulationMethod == TTFText.DynamicTextRuntimeTriangulationMethodEnum.CSharpLibs) {
					TTFTextInternal.Utilities.ReverseTriangles (front);
				}	
		
				if (tm.BackFace) {
					back = new Mesh ();
					back.vertices = front.vertices;
					back.triangles = front.triangles;
					TTFTextInternal.Utilities.ReverseTriangles (back);
				
					CombineInstance[] combine = new CombineInstance[2];
					combine [0].mesh = front;
					combine [1].mesh = back;
				
					mesh.CombineMeshes (combine, false, false);
				
					Utilities.DestroyObj (front);
					Utilities.DestroyObj (back);
		
				} else {
					CombineInstance[] combine = new CombineInstance[1];
					combine [0].mesh = front;
					mesh.CombineMeshes (combine, false, false);
					Utilities.DestroyObj (front);
				}
		
			} else if (ts.ExtrusionMode == TTFText.ExtrusionModeEnum.Pipe) {
#if TTFTEXT_LITE        
		if (tm.DemoMode) {	
#endif			
				TTFTextOutline o = MakeOutline (ll, ts.Embold, tm);

				o = o.Simplify (ts.SimplifyAmount, ts.Size);
				if (ts.Slant != 0) {
					o.Slant (ts.Slant);
				}
				advance = o.advance;
	
				TTFTextInternalMeshGenerators.Piped (ref mesh, o, ts.Radius, ts.NumPipeEdges);
#if TTFTEXT_LITE        
			}
#endif			
			
			} else {
		
				switch (ts.ExtrusionMode) {

				case TTFText.ExtrusionModeEnum.Simple:
					outlines = MakeSimpleOutlines (ll, tm, ts);
					break;

				case TTFText.ExtrusionModeEnum.Bent:
#if TTFTEXT_LITE        
		if (tm.DemoMode) {	
#endif							
					outlines = MakeBentOutlines (ll, tm, ts);
#if TTFTEXT_LITE        
			}
#endif			
					
					break;

				case TTFText.ExtrusionModeEnum.Bevel:
#if TTFTEXT_LITE        
		if (tm.DemoMode) {	
#endif								
					outlines = MakeBevelOutlines (ll, tm, ts);
#if TTFTEXT_LITE        
			}
#endif			
					
					break;

				case TTFText.ExtrusionModeEnum.FreeHand:
#if TTFTEXT_LITE        
		if (tm.DemoMode) {	
#endif			
					outlines = MakeFreeHandOutlines (ll, tm, ts);
#if TTFTEXT_LITE        
			}
#endif			
					
					break;
				default:
					Debug.LogError ("(TTFText) Unexpected Extrusion Mode:" + tm.ExtrusionMode);
					break;
				}		
	
				if (outlines.Length == 0) {
					mesh.Clear ();
					advance = Vector3.zero;
					return mesh;
				}
			
			
				//if (ts.OutlineEmbold >= 0) {
				//	outlines = OutlineOutlines(outlines, ts.OutlineEmbold);
				//}
			
				mask = outlines [0].SimplifyMask (ts.SimplifyAmount, tm.Size);
		
				// front
				TTFTextOutline o = outlines [0].ApplyMask (mask);
				front = TTFTextInternal.Tesselators.Triangulate (o,
				tm.DynamicTextRuntimeTriangulationMethod == TTFText.DynamicTextRuntimeTriangulationMethodEnum.CSharpLibs);//tm.EmbedCharset);
				
				advance = o.advance;
				advance.z = 0;
				
				//back
				o = outlines [outlines.Length - 1].ApplyMask (mask);
				back = TTFTextInternal.Tesselators.Triangulate (o,
				tm.DynamicTextRuntimeTriangulationMethod == TTFText.DynamicTextRuntimeTriangulationMethodEnum.CSharpLibs);//tm.EmbedCharset);
			

				
				if (tm.DynamicTextRuntimeTriangulationMethod == TTFText.DynamicTextRuntimeTriangulationMethodEnum.CSharpLibs) {
					Utilities.ReverseTriangles (front);
				} else {
					Utilities.ReverseTriangles (back);
				}
			
				TTFTextInternalMeshGenerators.ExtrudeOutlines (ref mesh, front, back, outlines, mask);
			
				Utilities.DestroyObj (front);
				Utilities.DestroyObj (back);

				if (! tm.SplitSides) { // merge submeshes
					Utilities.MergeSubmeshes (mesh);
					//CombineInstance ci[] = new CombineInstance[mesh.subMeshCount];
					//Mesh tmesh=new Mesh();
					//mesh.CombineMeshes(true, false);
				}

			}
	
			ShaderMaps.ComputeUVs2 (mesh, ts.UvType, ts.NormalizeUV, ts.UvScaling, charmetadata);
			mesh.RecalculateNormals ();
			ShaderMaps.ComputeTangents (mesh);
				
			return mesh;
		}
	
#endregion
	
	
	
#region FUNCTIONS_GENERATING_OF_OUTLINES_FOR_DIFFERENT_EXTRUSIONS
		
		public static TTFTextOutline[] OutlineOutlines (TTFTextOutline[] outlines, float str)
		{
			int len = outlines.Length;
			if (len == 0 || str <= 0) {
				return outlines;
			}
		
			TTFTextOutline[] res = new TTFTextOutline[len + 2];
		
			res [0] = outlines [0].Embolden (-str);
			for (int i = 0; i < len; ++i) {
				res [i + 1] = outlines [i];//new Outline(outlines[i]);
			}
		
			res [len + 1] = outlines [len - 1].Embolden (-str);
			return res;
		}
	
		// Simple extrusion
		public static TTFTextOutline[] MakeSimpleOutlines (TTFTextInternal.LineLayout ll, TTFText tm, TTFTextStyle ts)
		{	
			TTFTextOutline o = MakeOutline (ll, ts.Embold, tm);
			if (ts.Slant != 0) {
				o.Slant (ts.Slant);
			}
		
			TTFTextOutline[] outlines = new TTFTextOutline[2];
			outlines [0] = o;
			outlines [1] = new TTFTextOutline (o);
			outlines [0].Translate (Vector3.forward * (-ts.ExtrusionDepth) / 2);
			outlines [1].Translate (Vector3.forward * ts.ExtrusionDepth / 2);
		
			return outlines;
		}

		// FreeHand Curve Extrusion
		public static TTFTextOutline[] MakeFreeHandOutlines (TTFTextInternal.LineLayout ll, TTFText tm, TTFTextStyle ts)
		{
			//Vector3 min=Vector3.zero; Vector3 max=Vector3.zero;
			Keyframe[] keys = ts.FreeHandCurve.keys;
		
			int NS = keys.Length;
		
			TTFTextOutline[] outlines = new TTFTextOutline[NS];
			float[] z = new float[NS];

			//min = Vector3.zero;
			//max = Vector3.zero;
			//Vector3 [] szs = new Vector3[NS];
		
			for (int i=0; i<NS; i++) {			
// 			Vector3 min_, max_;
				z [i] = keys [i].time * ts.ExtrusionDepth;
			
				outlines [i] = MakeOutline (ll, ts.Embold + keys [i].value * ts.BevelForce, tm);			
				outlines [i].Translate (Vector3.forward * z [i]);
			
				if (ts.Slant != 0) {
					outlines [i].Slant (ts.Slant);
				}
			
				//outlines[i].GetMinMax(out min_, out max_);
				//szs[i]=max_-min_;
				//min = Vector3.Min(min, min_);
				//max = Vector3.Max(max, max_);
			}
			//Vector3 sz=max-min;
			//for (int i=0;i<NS;i++) {
			//	outlines[i].Translate((sz-szs[i])/2);
			//}
			return outlines;
		}
	
		static TTFTextOutline[] MakeBentOutlines (TTFTextInternal.LineLayout ll, TTFText tm, TTFTextStyle ts)
		{			
			//Vector3 min=Vector3.zero; Vector3 max=Vector3.zero;
			int NS = ts.ExtrusionSteps.Length; 
		
			TTFTextOutline[] outlines = new TTFTextOutline[NS];
			//Vector3 [] szs = new Vector3[NS];
		
			for (int i = 0; i < NS; i++) {
			
				//Vector3 min_, max_;
			
		    
				float embold = ts.Embold + ts.BevelForce * Mathf.Sin (i * Mathf.PI / (NS - 1));
		    
				// wierd error with poly2tri/embold for the backface
				if (i == NS - 1) {
					embold = ts.Embold;		
				}
				
				outlines [i] = MakeOutline (ll, embold, tm);
		
				float z = (ts.ExtrusionSteps [i] - 0.5f) * ts.ExtrusionDepth;// - tm.exstrusionDepth / 2;
				outlines [i].Translate (Vector3.forward * z);
			
				if (ts.Slant != 0) {
					outlines [i].Slant (ts.Slant);
				}
			
				//outlines[i].GetMinMax(out min_, out max_);
				//szs[i]=max_-min_;
				//min = Vector3.Min(min, min_);
				//max = Vector3.Max(max, max_);
			}
		
			//Vector3 sz=max-min;
			//for (int i=0;i<NS;i++) {
			//	outlines[i].Translate((sz-szs[i])/2);
			//}

			return outlines;
		}
	
		static TTFTextOutline[] MakeBevelOutlines (TTFTextInternal.LineLayout ll, TTFText tm, TTFTextStyle ts)
		{
			//Vector3 min=Vector3.zero; Vector3 max=Vector3.zero;
			//Vector3 min_; Vector3 max_;
			int NS = tm.NbDiv;
			if (NS < 2) {
				NS = 2;
			}
		
			TTFTextOutline[] outlines = new TTFTextOutline[NS * 2];
			//Vector3[] szs = new Vector3[NS*2];
		
			float bevelDepth = ts.BevelDepth * ts.ExtrusionDepth / 2;
		
			for (int i = 0; i < NS; ++i) {
			
				float f = ((float)i) / ((float)NS - 1); // [0,1]
			
				float embold = ts.Embold + Mathf.Sin (Mathf.Acos (1 - f)) * ts.BevelForce;
			
				TTFTextOutline o = MakeOutline (ll, embold, tm);
			
				o.Slant (ts.Slant);
			
				outlines [i] = o;
				outlines [2 * NS - 1 - i] = new TTFTextOutline (o);
			
				//outlines[i].GetMinMax(out min_, out max_);
				//szs[i]=max_-min_;
				float z = f * bevelDepth;
			
				outlines [i].Translate (Vector3.forward * z);
				outlines [2 * NS - 1 - i].Translate (Vector3.forward * (ts.ExtrusionDepth - z));
			
				//szs[2*NS-1-i]=max_-min_;
				//min = Vector3.Min(min, min_);
				//max = Vector3.Max(max, max_);			
			}


			//Vector3 sz=max-min;
			//for (int i=0;i<(2*NS);i++) {
			//	outlines[i].Translate((sz-szs[i])/2);
			//}
		
	
			return outlines;
		}
    
#endregion

		
		
		
		
		
		
		
		
		
		/// This function simply uses Freetype to get an outline
		/// it used by other engines..
		public static TTFTextOutline MakeNativeOutline (string txt, 
						float charSpacing, 
						float embold, 
						TTF.Font font, 
						bool reversed, 
		                int interpolationstep
					)
		{
			TTFTextOutline o = null;
#if !TTFTEXT_LITE
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR

				TTF.Outline.Point tadv = new TTF.Outline.Point();
				TTF.Outline ttfoutline = font.GetStringOutline(
					txt, ref tadv,
					TTF.HorizontalJustification.Origin,
					TTF.VerticalJustification.Origin ,
					false,  
					charSpacing,
			        0,
			        interpolationstep);

		o= TTFTextOutline.TTF2Outline(ttfoutline, tadv,reversed).Embolden(embold);
#endif
#endif				

			return o;
		}
	
	
	
	
		/// <summary>
		/// Core function generating the outline for a portion of text
		/// </summary>
		/// <returns>
		/// The outline.
		/// </returns>
		/// <param name='txt'>
		/// The text to be rendered
		/// </param>
		/// <param name='charSpacing'>
		/// The default character spacing
		/// </param>
		/// <param name='embold'>
		/// Current embold
		/// </param>
		/// <param name='tm'>
		/// Link to the TTFText object
		/// </param>
		/// <param name='reversed'>
		/// Orientation of the outlines
		/// </param>
		/// <param name='charpositions'>
		/// Position of the character
		/// </param>
		/// <param name='charstyleidx'>
		/// Style index of each character
		/// </param>
		/// <param name='charmetadata'>
		/// Metadata associated to special character such as images and bitmap characters
		/// </param>
		/// <exception cref='System.Exception'>
		/// Is thrown when the exception.
		/// </exception>
		public static TTFTextOutline MakeOutline (string txt, 
						float charSpacing, 
						float embold, 
						TTFText tm, 
						bool reversed, 
						float [] charpositions,
						int [] charstyleidx,
					    object [] charmetadata)
		{

		
		
		
			TTFTextOutline outline = new TTFTextOutline ();
			int fp = 0;
			string currentfontid = "";
			object cfont = null;
			object parameters = null;
			TTFTextStyle cttfstyle = null;
		
			if (charstyleidx == null) {
				cttfstyle = tm.InitTextStyle;
				fp = cttfstyle.PreferredEngine (Application.platform);
#if ! TTFTEXT_LITE				
				cfont = cttfstyle.GetFont (ref fp, ref currentfontid, ref parameters);
#else 
				fp=0; 
				currentfontid=cttfstyle.FontId;
				parameters=cttfstyle.GetFontEngineParameters(0);
				if (parameters==null) {
							System.Type t = TTFTextInternal.TTFTextFontEngine.font_engines [0].GetType ().GetNestedType ("Parameters");						
							cttfstyle.SetFontEngineParameters(0,t.InvokeMember (t.Name, BindingFlags.CreateInstance, null, null, null));
							parameters=cttfstyle.GetFontEngineParameters(0);					
				}				
				cfont= TTFTextInternal.TTFTextFontEngine.font_engines[0].GetFont(parameters,currentfontid);				
#endif				
			
				if (cfont == null) {
					throw new System.Exception ("(TTFText) Font not found :" + tm.InitTextStyle.FontId);
				}

			}
			if (charpositions != null && charpositions.Length < txt.Length) {
				Debug.LogError ("(TTFText) Bad char position len=" + charpositions.Length + " txt = " + txt + " (len=" + txt.Length + ")");
				charpositions = null;
			}
				
		
		
		
		
		
		
			int i = 0;
			foreach (char c in txt) {
				TTFTextOutline o = null;
					
				if (charstyleidx != null) {
					if ((cfont == null) 
						|| (currentfontid != tm.UsedStyles [charstyleidx [i]].GetFontEngineFontId (tm.UsedStyles [charstyleidx [i]].PreferredEngine (Application.platform)))) {
						cttfstyle = tm.UsedStyles [charstyleidx [i]];			
						fp = cttfstyle.PreferredEngine (Application.platform);
						//Debug.Log(fp);
#if ! TTFTEXT_LITE				
						cfont = cttfstyle.GetFont (ref fp, ref currentfontid, ref parameters);
#else 
				fp=0; 
				currentfontid=cttfstyle.FontId;
				parameters=cttfstyle.GetFontEngineParameters(0);
				if (parameters==null) {
							System.Type t = TTFTextInternal.TTFTextFontEngine.font_engines [0].GetType ().GetNestedType ("Parameters");						
							cttfstyle.SetFontEngineParameters(0,t.InvokeMember (t.Name, BindingFlags.CreateInstance, null, null, null));
							parameters=cttfstyle.GetFontEngineParameters(0);
				}
				cfont= TTFTextInternal.TTFTextFontEngine.font_engines[0].GetFont(parameters,currentfontid);
#endif				
			
						if (cfont == null) {
							throw new System.Exception ("Font not found :" + tm.InitTextStyle.FontId);
						}

					}
				}
        			
			
			
				if ((charmetadata == null) || (charmetadata [i] == null)) {
					// a normal character
					
					
					if (TTFTextFontEngine.font_engines [fp].IsBitmapFontProvider (parameters)) {	
						TTFTextTexturePortion p = TTFTextFontEngine.font_engines [fp].GetGlyphBitmap (parameters, cfont, c);     
						o = new TTFTextOutline ();
						float w = ((float)p.w);
						float h = ((float)p.h);
						Vector3 b = new Vector3 (p.x, p.y, 0);
						//Vector3[] quad = {Vector3.zero,Vector3.right*w,new Vector3(w,h,0),Vector3.up*h};
						Vector3[] quad = {b,b + Vector3.up * h,b + new Vector3 (w, h, 0),b + Vector3.right * w};
						o.AddBoundary (quad);

						TTFTextInternal.TextureElement tel = new TTFTextInternal.TextureElement ();
						tel.width = w;
						tel.height = h;
						tel.material = p.material;
						tel.shouldReleaseMaterial = p.shouldReleaseMaterial;
						tel.texture = p.texture;
						tel.shouldReleaseTexture = p.shouldReleaseTexture;
						tel.UVstartx = p.sx;
						tel.UVstarty = p.sy;
						tel.UVwidth = p.dx;
						tel.UVheight = p.dy;
						charmetadata [i] = tel;						
					} else {
						o = TTFTextFontEngine.font_engines [fp].GetGlyphOutline (parameters, cfont, c);                    
										
						if (charstyleidx != null) {					
							for (int ii=0; ii<tm.UsedStyles[charstyleidx[i]].GetOutlineEffectStackElementLength(); ii++) {
								TTFTextStyle.TTFTextOutlineEffectStackElement tse = tm.InitTextStyle.GetOutlineEffectStackElement (ii);
								o = TTFTextOutline.AvailableOutlineEffects [tse.id].Apply (o, tse.parameters);
							}
						} else {
							for (int ii=0; ii<tm.InitTextStyle.GetOutlineEffectStackElementLength(); ii++) {
								TTFTextStyle.TTFTextOutlineEffectStackElement tse = tm.InitTextStyle.GetOutlineEffectStackElement (ii);
								o = TTFTextOutline.AvailableOutlineEffects [tse.id].Apply (o, tse.parameters);
							}
						}
					}
				} else {
					// it is some kind of special object (an embedded image... ?)
					//Debug.Log("Not yet implemented");
					o = new TTFTextOutline ();
					float w = 1;
					float h = 1;
					Vector3[] quad = {Vector3.zero,Vector3.right * w,new Vector3 (w, h, 0),Vector3.up * h};
					o.AddBoundary (quad);
				}
			
			
				
				o = o.Embolden (((charstyleidx != null) ? tm.UsedStyles [charstyleidx [i]].Embold : tm.Embold) + embold);
				o.Rescale ((charstyleidx != null) ? tm.UsedStyles [charstyleidx [i]].Size : tm.Size);

				if (charpositions == null) {
					outline.Append (o, outline.advance);
				} else {
					outline.Append (o, Vector3.right * charpositions [i]);
				}	
			
				i += 1;
			
			}
			return outline;	
		}

		public static TTFTextOutline MakeOutline (string txt, float charSpacing, float embold, TTFText tm)
		{
			return MakeOutline (txt, charSpacing, embold, tm, tm.OrientationReversed, null, null, null);
		}

		public static TTFTextOutline MakeOutline (TTFTextInternal.LineLayout ll, float embold, TTFText tm)
		{
			return MakeOutline (ll.line, ll.hspacing, embold, tm, tm.OrientationReversed, ll.charpositions, ll.charstyleindex, ll.charmetadata);
		}
	
	
	
	
	
		
		
		
		
		

		
#region UTILITY_FUNCTIONS
		public static void UpdateComponentsWithNewMesh (GameObject go, Mesh mesh)
		{
			// Update MeshFilter and/or InteractiveCloth components if present
		
			MeshFilter mf = go.GetComponent<MeshFilter> ();
			if (mf != null) {
				mf.sharedMesh = mesh;
			}
		
#if !UNITY_FLASH		
			InteractiveCloth ic = go.GetComponent<InteractiveCloth> ();
			if (ic != null) {
				ic.mesh = mesh;
			}
#endif		
		
			// Update Box/Mesh Collider Bounds if present
			if (go.collider is BoxCollider) {
				BoxCollider bc = go.collider as BoxCollider;
				Bounds b = mesh.bounds;
				bc.center = b.center;
				bc.size = b.size;
			} else if (go.collider is MeshCollider) {
				MeshCollider mc = go.collider as MeshCollider;
				mc.sharedMesh = mesh;
			}
		
#if TTFTEXT_LITE       
		TTFText tm=go.GetComponent<TTFText>();
		if (tm==null) {
			tm=go.transform.parent.GetComponent<TTFText>();
		}
		if (tm.DemoMode) {
#endif			

			// Release the Mesh on GameObject destruction
			TTFTextReleaseTempResources rm = go.GetComponent<TTFTextReleaseTempResources> ();
			if (rm == null) {
				rm = go.AddComponent<TTFTextReleaseTempResources> ();
			}
			rm.mesh = mesh;
		
			go.SendMessage ("MeshUpdated", SendMessageOptions.DontRequireReceiver);
#if TTFTEXT_LITE        
			}
#endif			

		
		}
	
		public static void ResetChildren (TTFText tm)
		{
		
			List<Transform> l = new List<Transform> ();
		
		
			// keep track of token previous positions
			if (tm.SaveTokenPos) {
				tm.TokenPos.Clear ();
			}
			foreach (Transform t in tm.transform) {
				if (tm.SaveTokenPos) {
					tm.TokenPos.Add (new TTFText.TrInfo (t));
				}
				l.Add (t);
			
			}
		

			foreach (Transform t in l) {
				if (t.GetComponent<TTFSubtext> () != null) {
					Utilities.DestroyObj (t.gameObject);
				}
			}
		}
	
		static void ResetTextMesh (TTFText tm)
		{
		
			ResetChildren (tm);
		
			MeshFilter mf = tm.GetComponent<MeshFilter> ();
			if (mf != null) {
				mf.sharedMesh = null;
			}
		
			if (tm.mesh != null) {
				Utilities.DestroyObj (tm.mesh);
				tm.mesh = null;
			}
		
			tm.advance = Vector3.zero;
			tm.nonFoundStyles = false;
			tm.statistics_num_vertices = 0;
			tm.statistics_num_subobjects = 0;
		}
	
		static int ExpectedNumberOfSubmeshes (TTFText tm)
		{	
			int nsubmeshes = 1;
			if ((tm.SplitSides)) {	
				nsubmeshes = 3;
				if (tm.ExtrusionMode == TTFText.ExtrusionModeEnum.Pipe) {
					nsubmeshes = 1;
				}
				if (tm.ExtrusionMode == TTFText.ExtrusionModeEnum.None) {
					if (! tm.BackFace)
						nsubmeshes = 1;
					else 
						nsubmeshes = 2;
				}
			}
			return nsubmeshes;
		}


	
#endregion

	}
}