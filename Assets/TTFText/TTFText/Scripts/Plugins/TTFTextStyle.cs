using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// TTF text styles are in charge of memorizing style a set
/// of graphical settings used when rendering the Text.
/// </summary>
[System.Serializable]
public class TTFTextStyle : System.Object
{
	// :  UnityEngine.Object {//ScriptableObject {
	//
	// TTF Text Style are not currently with 
	// TTFTextStore (WIP)
	// 
	private static int idctr = 1;
	private int xid = 0;
	
	public int getid ()
	{
		if (xid == 0) {
			//lock (idctr) {
			renewid();
			//}
		}
		;
		return xid;
	}
	
	public void renewid() {
		xid = ++idctr;
	}
	
	TTFTextStyle stackparent = null;

	public TTFTextStyle ()
	{
		renewid(); 	
	}
	
	public TTFTextStyle (TTFTextStyle s)
	{
//		Debug.Log("copy ctor s "+s.GetInstanceID()+(s!=null));
		PropertyInfo[] properties = typeof(TTFTextStyle).GetProperties ();
		foreach (PropertyInfo pi in properties) {
			try {
				if ((pi.Name != "name") && (pi.Name != "hideFlags")) {
					if ((pi.CanRead) && (pi.CanWrite)) {
						pi.SetValue (this, pi.GetValue (s, null), null);
					}
				}
			} catch (System.Exception e) {
				Debug.LogError ("(TTF Text)Error during copy of property :" + pi.Name + ":" + e);
			}
			  
		}	
		
		
		
		
		  
		if (s.fontengine_specific_fontid != null) {
			fontengine_specific_fontid = new System.Collections.Generic.List<string> (s.fontengine_specific_fontid);
		}
		if (s.fontengine_parameters != null) {
			fontengine_parameters = new System.Collections.Generic.List<object> (s.fontengine_parameters);
		}
		if (s.fontengine_parameters_serialized != null) {
			fontengine_parameters_serialized = new System.Collections.Generic.List<ByteArray> (s.fontengine_parameters_serialized);
		}
		
		orientationReversed = s.orientationReversed;
		materialSource = s.materialSource;
		//xid=0;
		renewid();
//		Debug.Log("New style :"+getid().ToString());
	}
	
	public TTFTextStyle Push ()
	{
//		TTFTextStyle tp=(TTFTextStyle)ScriptableObject.Instantiate(this);
		TTFTextStyle tp = new TTFTextStyle (this);
		tp.stackparent = this;
		//tp.renewid();
		return tp;
	}
	
	public TTFTextStyle U ()
	{
		return Push ();
	}
	
	public TTFTextStyle Pop ()
	{
		if (stackparent != null) {
			/* STYLES ARE RELEASED AFTER LAYOUT COMPUTATIONS
			if ((Application.isEditor)&&(!Application.isPlaying)) {
				DestroyImmediate(this);
			}
			else {
				Destroy(this);
			}*/
			return stackparent;
		} else 
			return this;
	}
	
	public TTFTextStyle D ()
	{
		return Pop ();
	}
	
	public TTFTextStyle PushF (string name, bool createstyle, ref bool notfound)
	{
		GameObject go = (GameObject)GameObject.Find ("/TTFText Styles/" + name);
		if (go == null) {
			Object o=Resources.Load("/TTFText/Styles/"+name+".prefab");
			if (o!=null) {
				GameObject ngo=(GameObject)o;
				ngo.name=name;
				ngo.transform.parent=GameObject.Find("/TTFText Styles").transform;
				go=ngo;
			}
			
			
			if (go==null) {
			if (createstyle) {						
				GameObject gop = (GameObject)GameObject.Find ("/TTFText Styles");
				if (gop == null) {
					gop = new GameObject ();
					gop.name = "TTFText Styles";
				}
				go = new GameObject ();
				go.transform.parent = gop.transform;
				go.name = name;
				go.AddComponent<TTFText> ();
				TTFText tm = go.GetComponent<TTFText> ();
				go.AddComponent<MeshRenderer> ();
				go.AddComponent<TTFTextExtra_SetMaterialColor> ();
				tm.Text = "";
				tm.IsStyleObject = true;
				tm.updateObjectName = false;
				//tm.InitTextStyle=(TTFTextStyle)ScriptableObject.Instantiate(this);
				tm.InitTextStyle = new TTFTextStyle (this);
				tm.InitTextStyle.materialSource = go;				
				tm.InitTextStyle.ResetStyle ();
			} else {				
				notfound = true;
				return new TTFTextStyle (this);
			}
			}	
		}
		//TTFTextStyle tp=(TTFTextStyle)ScriptableObject.Instantiate(go.GetComponent<TTFText>().InitTextStyle);
		TTFTextStyle tp = new TTFTextStyle (go.GetComponent<TTFText> ().InitTextStyle);
		tp.stackparent = this;
		//tp.renewid();
		return tp;
	}
	
#region Appearance Fields
	public bool overrideFontId = false;
	[SerializeField]
	private string fontId = ""; // <- this is the default selector for font used when no other selector is available
	
	
	
	[SerializeField]
	bool _usefontengine_specific_fontid=false;
	[SerializeField]
	private System.Collections.Generic.List<string> _fontengine_specific_fontid;
	private System.Collections.Generic.List<string> fontengine_specific_fontid {
		get { return _fontengine_specific_fontid;}
		set {
			_usefontengine_specific_fontid=(value!=null);
			_fontengine_specific_fontid=value;
		}
	}
	
	private System.Collections.Generic.List<object> fontengine_parameters;
	[SerializeField]
	private System.Collections.Generic.List<ByteArray> fontengine_parameters_serialized;
	
	
	public bool useDifferentFontIdForEachFontEngine {
		get {return _usefontengine_specific_fontid;}
		set {
#if ! TTFTEXT_LITE			
			bool cmode=_usefontengine_specific_fontid;
			if (cmode!=value) {
				//Debug.Log(value);
				if (value) {
					fontengine_specific_fontid = new System.Collections.Generic.List<string> ();
					for (int ci=0; ci<TTFTextInternal.TTFTextFontEngine.font_engines.Length; ci++) {
							fontengine_specific_fontid.Add (fontId);
					}								
					//for (int i=0;i<TTFTextInternal.TTFTextFontEngine.font_engines.Length;i++) {
					//	SetFontEngineFontId(i,fontId);						
					//}
				}
				else {
					fontId=GetFontEngineFontIdD(0);
					fontengine_specific_fontid=null;				
				}
			}
#endif			
		}
	}
	
	public object GetFontEngineParameters (int i)
	{
#if ! TTFTEXT_LITE					
		if ((!overrideFontId) && (stackparent != null)) {
			return stackparent.GetFontEngineParameters (i);
		}
		
		if ((fontengine_parameters == null) ||
			(fontengine_parameters.Count < TTFTextInternal.TTFTextFontEngine.font_engines.Length)
			//||((fontengine_parameters[0]==null)&&(fontengine_parameters_serialized[0].bytes.Length>0))
			) {
			
			if (fontengine_parameters == null) {
				fontengine_parameters = new System.Collections.Generic.List<object> ();			
			}
			if (fontengine_parameters_serialized == null) {
				fontengine_parameters_serialized = new System.Collections.Generic.List<ByteArray> ();
			}
			int mi = fontengine_parameters.Count;
			for (int ci=0; ci<mi; ci++) {
				if (fontengine_parameters [i] == null) {
					mi = ci;
					break;
				}
			} 
			
			for (int ci=mi;
				ci<TTFTextInternal.TTFTextFontEngine.font_engines.Length;
				ci++
				) {
				// try deserialize
				if ((fontengine_parameters_serialized != null)
					&& (fontengine_parameters_serialized.Count > ci)
					&& (fontengine_parameters_serialized [ci] != null)
					) {
					object to = DeserializeObject (fontengine_parameters_serialized [ci]);
					if (fontengine_parameters.Count == ci) {
						fontengine_parameters.Add (to);
					} else {
						fontengine_parameters [ci] = to;
					}
				} else {
					if ((fontengine_parameters.Count == ci)) {
						System.Type t = TTFTextInternal.TTFTextFontEngine.font_engines [ci].GetType ().GetNestedType ("Parameters");
						fontengine_parameters.Add (t.InvokeMember (t.Name, BindingFlags.CreateInstance, null, null, null)); 
						fontengine_parameters_serialized.Add (SerializeObject (fontengine_parameters [ci]));
					
					}
					// try instantiate
					if ((fontengine_parameters [ci] == null)) {
						System.Type t = TTFTextInternal.TTFTextFontEngine.font_engines [ci].GetType ().GetNestedType ("Parameters");
						fontengine_parameters [ci] = t.InvokeMember (t.Name, BindingFlags.CreateInstance, null, null, null); 					
						//if (fontengine_specific_fontid [ci] == null)
						//	SetFontengineFontId (i, fontengine_specific_fontid [0]);
					}	
				}   
			}			
		}
		if (i == 0) {			
			if (fontengine_parameters [i] == null) {
				Debug.LogWarning ("(TTFText) For some reason, the serialized font parameters have not been recovered... Are you sure the font engine parameters were declared serializable ? Are you upgrading the project from a previous version of TTF Text ?");
				fontengine_parameters = null;
				fontengine_parameters_serialized = null;
				return GetFontEngineParameters (0);
			}
		
		}
		if (i==-1) {i=0;}

		return fontengine_parameters [i];
#endif		
#if TTFTEXT_LITE		
			if (fontengine_parameters == null) {
				fontengine_parameters = new System.Collections.Generic.List<object> ();	
				fontengine_parameters.Add (null);
			}
		
	    return fontengine_parameters [0];		
#endif				
	}
	
	public void SetFontEngineParameters (int i, object o)
	{
#if ! TTFTEXT_LITE							
		if ((fontengine_parameters == null)) {
			GetFontEngineParameters (i);
		}
		try {
			fontengine_parameters [i] = o;
			fontengine_parameters_serialized [i] = SerializeObject (o);	
		} catch {
			if (i==-1) {i=0;}			
			GetFontEngineParameters (i);
			fontengine_parameters [i] = o;
			fontengine_parameters_serialized [i] = SerializeObject (o);						
		}
		if (fontengine_parameters_serialized [i].bytes == null) {
			Debug.LogWarning ("FontEngine Pararemeters do not seem to be serializable !");
		}
		overrideFontId = true;
#endif
#if TTFTEXT_LITE				
		if ((fontengine_parameters == null)) {
			GetFontEngineParameters (i);
		}
		fontengine_parameters[0]=o;		
#endif		
	}
	
		
	
	/// <summary>
	/// The prefered engine according to the platform
	/// </summary>
	public int[] _PreferedEngine = null;
	
	
	public int PreferredEngine (RuntimePlatform idx)
	{
#if !TTFTEXT_LITE		
		if ((_PreferedEngine == null) || (_PreferedEngine.Length < System.Enum.GetNames (typeof(UnityEngine.RuntimePlatform)).Length)) {
			_PreferedEngine = new int [System.Enum.GetNames (typeof(UnityEngine.RuntimePlatform)).Length];
			for (int i=0; i<_PreferedEngine.Length; i++) {
				_PreferedEngine [i] = 0;
				for (int j=0; j<TTFTextInternal.TTFTextFontEngine.font_engines.Length; j++) {
					if (TTFTextInternal.TTFTextFontEngine.font_engines [j].IsCompatible ((RuntimePlatform)i)) {
						_PreferedEngine [i] = j;
						j = TTFTextInternal.TTFTextFontEngine.font_engines.Length;
					}
				}
			}
		}
		return _PreferedEngine [(int)idx];		
#endif
#if TTFTEXT_LITE		
		return 0;
#endif 
		
	}
	
	
	public int DefaultNativeEngine (RuntimePlatform idx)
	{
#if ! TTFTEXT_LITE			
		for (int j=0; j<TTFTextInternal.TTFTextFontEngine.font_engines.Length; j++) {
			if (TTFTextInternal.TTFTextFontEngine.font_engines [j].IsCompatible (idx)) {
				if (TTFTextInternal.TTFTextFontEngine.font_engines [j].IsConsideredNative (idx)) {
					return j;							
				}
			}
		}
#endif		
		return -1;
	}
	
	
	public int DefaultEmbeddedEngine (RuntimePlatform idx)
	{
#if ! TTFTEXT_LITE			
		for (int j=0; j<TTFTextInternal.TTFTextFontEngine.font_engines.Length; j++) {
			if (TTFTextInternal.TTFTextFontEngine.font_engines [j].IsCompatible (idx)) {
				if (TTFTextInternal.TTFTextFontEngine.font_engines [j].IsConsideredEmbedded (idx)) {
					return j;							
				}
			}
		}
#endif		
		return -1;
	}
	
	
	
	
	public void  SetPreferedEngine (RuntimePlatform idx, int v)
	{
#if! TTFTEXT_LITE			
		if (_PreferedEngine == null) {
			PreferredEngine (idx);
		}
		_PreferedEngine [(int)idx] = v;
#endif		
	}
	
	
	
	
	
#if ! TTFTEXT_LITE	
	/// <summary>
	/// Gets the font associated w
	/// </summary>
	/// <returns>
	/// The font.
	/// </returns>
	/// <param name='engine'>
	/// Engine.
	/// </param>
	/// <param name='fontid'>
	/// Fontid.
	/// </param>
	/// <param name='parameters'>
	/// Parameters.
	/// </param>
	public object GetFont (ref int engine, ref string fontid, ref object parameters)
	{		
		object f = null;
		int fp = engine;
		engine = -1;
			
		if ((!overrideFontId) && (stackparent != null)) {
			return stackparent.GetFont (ref engine, ref fontid, ref parameters);
		}
		
		//fp = PreferredEngine (Application.platform);
		try {
			parameters = GetFontEngineParameters (fp);
			if (parameters != null) {
				fontid = GetFontEngineFontIdD (fp);					
				f = TTFTextInternal.TTFTextFontEngine.font_engines [fp].GetFont (parameters, fontid);
			}
		} catch {
		}
			
			
			
		if (f == null) {
			Debug.Log(System.String.Format("GetFont : Provider={0} Trying alternate engine..",fp));
			for (fp=0; fp<TTFTextInternal.TTFTextFontEngine.font_engines.Length; fp++) {
				try {
					parameters = GetFontEngineParameters (fp);
					if (parameters != null) {
						fontid = GetFontEngineFontIdD (fp);
						f = TTFTextInternal.TTFTextFontEngine.font_engines [fp].GetFont (parameters, fontid);
						if (f != null) {
							break;
						}
					}
				} catch {
				}
			}
		}
		
		engine = fp;

		return f;		
	}
#endif	
	
	
	
	/// <summary>
	/// Returns the font engine to be used on current platform according to user preferences
	/// </summary>
	/// <returns>
	/// The used font engine.
	/// </returns>
	public int GetUsedFontEngine ()
	{
#if !TTFTEXT_LITE		
		object f = null;

		int fp = 0;
		int engine = -1;
		object parameters = null;
		string fontid = "";

		if ((!overrideFontId) && (stackparent != null)) {
			return stackparent.GetUsedFontEngine ();
		}

		
		//if (AdvancedFontSelectionPolicy) {

		fp = PreferredEngine (Application.platform);
			
			
		try {
			parameters = GetFontEngineParameters (fp);
			if (parameters != null) {
				fontid = GetFontEngineFontIdD (fp);
				if (fontid == null) {
					fontid = fontId;
				}
				f = TTFTextInternal.TTFTextFontEngine.font_engines [fp].GetFont (parameters, fontid);
				if (f != null) {
					TTFTextInternal.TTFTextFontEngine.font_engines [fp].DisposeFont (f);
					return fp;		
				}					
			}
		} catch {
		}
			
		if (f == null) {			
			for (fp=0; fp<TTFTextInternal.TTFTextFontEngine.font_engines.Length; fp++) {
				try {
					parameters = GetFontEngineParameters (fp);
					if (parameters != null) {
						fontid = GetFontEngineFontIdD (fp);
						f = TTFTextInternal.TTFTextFontEngine.font_engines [fp].GetFont (parameters, fontid);
						if (f != null) {
							TTFTextInternal.TTFTextFontEngine.font_engines [fp].DisposeFont (f);
							return fp;		
						}
							
					}				
				} catch {
				}
			}
		}
		//} else {
		/*	fp = 0;
			try {
				parameters = GetFontEngineParameters (fp);
				if (parameters != null) {
					fontid = GetFontEngineFontIdD (fp);
					f = TTFTextInternal.TTFTextFontEngine.font_engines [fp].GetFont (parameters, fontid);
				}
			} catch {
			}				
		}*/
		engine = fp;
		if (f != null) {
			TTFTextInternal.TTFTextFontEngine.font_engines [fp].DisposeFont (f);
		}
		return engine;		
#endif
#if TTFTEXT_LITE				
		return 0;
#endif		
	}
	

	
	public string GetFontEngineFontId (int i)
	{
		return GetFontEngineFontIdD(i);
	}
	
	public string GetFontEngineFontIdD (int i)
	{		
		if ((!overrideFontId) && (stackparent != null)) {
			return stackparent.GetFontEngineFontIdD (i);
		}		
		if (fontengine_specific_fontid != null) {
			if ((i >= 0) && (i < fontengine_specific_fontid.Count)) {
				if (fontengine_specific_fontid [i] != null) {
					return fontengine_specific_fontid [i];
				}
			}
		}
		return fontId;
	}
	
	public bool embedFont;
	public bool EmbedFont { get {return embedFont;} set {
			if (embedFont!=value) {
			embedFont=value;
				if (embedFont) {
					SetFontEngineFontId(1,GetFontEngineFontId(0));
				}
				else {
					SetFontEngineFontId(1,null);
				}
			}
		} 
	}
	
	public void SetFontEngineFontId (int i, string v)
	{	
#if TTFTEXT_LITE	
			fontengine_specific_fontid=null;
			fontId=v;
			return;		
#else		
		if (i==-1) {
			fontengine_specific_fontid=null;
			fontId=v;
			return;
		}
		if (fontengine_specific_fontid == null) {			
			fontengine_specific_fontid = new System.Collections.Generic.List<string> ();
			for (int ci=0; ci<TTFTextInternal.TTFTextFontEngine.font_engines.Length; ci++) {
				fontengine_specific_fontid.Add (null);
			}			
		}
		while (fontengine_specific_fontid.Count<TTFTextInternal.TTFTextFontEngine.font_engines.Length) {
			fontengine_specific_fontid.Add (null);
		}
		
		if ((i >= 0) && (i < fontengine_specific_fontid.Count)) {
			try {
				TTFTextInternal.TTFTextFontEngine.font_engines [i].DecRef (GetFontEngineParameters (i), GetFontEngineFontIdD( i));
			} catch {
			}
			fontengine_specific_fontid [i] = v;
			if (v != null) {
				try {
				TTFTextInternal.TTFTextFontEngine.font_engines [i].IncRef (GetFontEngineParameters (i), GetFontEngineFontIdD( i));
				}
				catch {}
			}
		}
		overrideFontId = true;
#endif		
	}
	
	
	// ----------------------------------------------------------------------
	// 
	// ----------------------------------------------------------------------
	
	public bool overrideSize = false;
	[SerializeField]
	private float size = 1; // meters
	
	public bool overrideEmbold = false;
	[SerializeField]
	private float embold = 0;
	public bool overrideSlant = false;
	[SerializeField]
	private float slant = 0;
	public bool overrideSimplifyAmount = false;
	[SerializeField]
	private float simplifyAmount = 0;
	public bool overrideOutlineStrength = false;
	[SerializeField]
	private float outlineStrength = 0;
	public bool overrideExtrusionDepth = false;
	[SerializeField]
	private float extrusionDepth = 0.3f;
	public bool overrideBevelForce = false;
	[SerializeField]
	private float bevelForce = 0.4f;
	public bool overrideWordSplitMode = false;
	[SerializeField] 
	private TTFText.WordSplitModeEnum wordSplitMode;
	public bool overrideGamma = false;
	[SerializeField]
	private float gamma = 0.5f;
	public bool overrideRadius = false;
	[SerializeField]
	private float radius = 0.05f;
	public bool overrideBevelDepth = false;
	[SerializeField]
	private float bevelDepth = 0.1f;
	public bool overrideNbDiv = false;
	[SerializeField]
	private int nbDiv = 5;
	public bool overrideNumPipeEdges = false;
	[SerializeField]
	private int numPipeEdges = 10;
	public bool overrideHspacing = false;
	[SerializeField]
	public float hspacing = 1f;
	public bool overrideLineWidth = false;
	[SerializeField]
	private float lineWidth = 15f;
	public bool overrideFirstLineOffset = false;
	[SerializeField]
	private float firstLineOffset = 0f;
	public bool overrideParagraphAlignement = false;
	[SerializeField]
	private TTFText.ParagraphAlignmentEnum paragraphAlignment = TTFText.ParagraphAlignmentEnum.Left;
	[SerializeField]
	private float wordSpacingFactor = 0.5f;
	[SerializeField]
	private float hSpacingMultFactor = 1f; // default additif
	[SerializeField]
	private TTFText.HSpacingModeEnum hSpacingMode = TTFText.HSpacingModeEnum.GlyphAdvance; //HSpacingModeEnum.HardBoundingBoxSpace;
	
	
	public bool overrideExtrusionSteps = false;
	[SerializeField]
	private float[] extrusionSteps = {0, 1};
	public bool overrideFreeHandCurve = false;
	[SerializeField]
	private AnimationCurve freeHandCurve;
	public bool overrideMaterialOffset = false;
	[SerializeField]
	private int materialOffset = 0;
	public bool overrideGlyphPrefab = false;
	[SerializeField]
	private GameObject glyphPrefab;
	[SerializeField]
	public int glyphPrefabHash;
	
	// Texture mapping
	public bool overrideSplitPrefab = false;
	[SerializeField]
	private bool splitSides = false;
	public bool overrideUVType = false;
	[SerializeField]
	private TTFText.UVTypeEnum uvType = TTFText.UVTypeEnum.Box;
	public bool overrideNormalizeUV = false;
	[SerializeField]
	private bool normalizeUV = false;
	public bool overrideUVScaling = false;
	[SerializeField]
	private Vector3 uvScaling = Vector3.one;
	//public bool overrideInterpolationSteps = false;
	//[SerializeField]
	//private int interpolationSteps = 4; // When curve are interpolated, number of interpolation steps that are done

	
	public bool overrideExtrusionMode = false;
	public TTFText.ExtrusionModeEnum extrusionMode = TTFText.ExtrusionModeEnum.None;
	public bool overrideBackFace = false;
	public bool backFace = false;
	public bool overrideLineSpacingMult = false;
	[SerializeField]
	private float lineSpacingMult = 1;
	public bool overrideLineSpacingAdd = false;
	[SerializeField]
	private float lineSpacingAdd = 0;
	public string runtimeFontFallback = "Arial (Regular);Helvetica (Regular);Times (Regular)";
	public GameObject materialSource = null;

	public Material [] sharedMaterials {
		get {
			if (materialSource == null)
				return null;
			if (materialSource.renderer == null)
				return null;
			return materialSource.renderer.sharedMaterials;
		}
	}
	
	public bool savelayoutinsubtext = true;
	
	[System.Serializable]
	public class TTFTextOutlineEffectStackElement : System.Object
	{		
		public int id;
		public object parameters;
	}
	
	[System.Serializable]
	public class ByteArray : System.Object
	{
		public byte[] bytes;

		public ByteArray (byte [] b)
		{
			bytes = b;
		}
	}
	
	
	
#if TTFTEXT_LITE	
	/// <summary>
	/// Utility function use to serializes an object.
	/// Sometime Unity serialization just don't work as we expect
	/// </summary>
	/// <returns>
	/// The object.
	/// </returns>
	/// <param name='o'>
	/// O.
	/// </param>
	public static byte [] XSerializeObject(object o) {
		if (o==null) return new byte[0]{};
    	if (!o.GetType().IsSerializable) { return null; }

    	using (MemoryStream stream = new MemoryStream())  {
        	new BinaryFormatter().Serialize(stream, o);
        	return stream.ToArray();
    	}
	}
	
	/// <summary>
	/// Utility function used to deserializes the object serialized by the previous function
	/// </summary>
	/// <returns>
	/// The object.
	/// </returns>
	/// <param name='bytes'>
	/// Bytes.
	/// </param>
	public static object XDeserializeObject(byte [] bytes) {
		if (bytes.Length==0) return null;
		using (MemoryStream stream = new MemoryStream(bytes))
    	{
			try {
        		return new BinaryFormatter().Deserialize(stream);
			}
			catch (System.Exception se) {
					Debug.LogError(se.ToString());
					return null;
				}
    	}
	}
	public static ByteArray SerializeObject (object o)
	{
		return new ByteArray (XSerializeObject (o));
	}
	
	public static object DeserializeObject (ByteArray bytes)
	{
		return XDeserializeObject (bytes.bytes);
	}

#endif	
	
#if !TTFTEXT_LITE
	public static ByteArray SerializeObject (object o)
	{
		return new ByteArray (TTFTextInternal.Utilities.SerializeObject (o));
	}
	
	public static object DeserializeObject (ByteArray bytes)
	{
		return TTFTextInternal.Utilities.DeserializeObject (bytes.bytes);
	}
#endif	
	
	[System.Serializable]
	public class SerializableAnimationCurve : System.Object
	{
		public int skey = -1;
		public float[] xkeys;
		public float[] ykeys;
		public float[] tikeys;
		public float[] tokeys;
		public int[] tkeys;
		[System.NonSerialized]
		private int bkey = 0;
		[System.NonSerialized]
		private AnimationCurve ac;
		
		public  AnimationCurve GetAnimcurve ()
		{
			if (ac == null) {
				;
				ac = AnimationCurve.Linear (0, 0, 1, 1);				
			}
			
			if (skey > bkey) { 
				int acl = ac.length;
				for (int i=0; i<acl; i++) {
					ac.RemoveKey (0);
				}
				
				UnityEngine.Keyframe [] kc = new UnityEngine.Keyframe[xkeys.Length];
				for (int i=0; i<xkeys.Length; i++) {
					kc [i].tangentMode = tkeys [i];
					kc [i].inTangent = tikeys [i];
					kc [i].outTangent = tokeys [i];
					kc [i].time = xkeys [i];
					kc [i].value = ykeys [i];
					ac.AddKey (kc [i]);
				}
				
				bkey = skey;
			}
			
			return ac;
		}
		
		public  void SetAnimcurve (AnimationCurve tac)
		{
			ac = tac;
			bkey++;
			UnityEngine.Keyframe [] kc = ac.keys;
			tkeys = new int[kc.Length];
			tikeys = new float[kc.Length];
			tokeys = new float[kc.Length];
			xkeys = new float[kc.Length];
			ykeys = new float[kc.Length];
				
			for (int i=0; i<xkeys.Length; i++) {
				tkeys [i] =  kc [i].tangentMode;
				tikeys [i] = kc [i].inTangent;
				tokeys [i] = kc [i].outTangent;
				xkeys [i] =  kc [i].time;
				ykeys [i] =  kc [i].value;
			}
			skey = bkey;
		}
		
		
	}
	

	public TTFTextOutlineEffectStackElement GetOutlineEffectStackElement (int i)
	{
		if (outlineeffectstack_c != null) {
			if (i >= outlineeffectstack_c.Count) {
				return null;
			}
			if (outlineeffectstack_c [i].parameters == null) {
				if (outlineeffectstack_serialized_parameters [i] != null) { 
					outlineeffectstack_c [i].parameters = DeserializeObject (outlineeffectstack_serialized_parameters [i]);					
				}
			} 
			return outlineeffectstack_c [i];
		} else {
			return null;			    
		}

	}
	
	
	
	
	public int GetOutlineEffectStackElementLength ()
	{
		if (outlineeffectstack_c != null) {
			return outlineeffectstack_c.Count;
		} else {
			outlineeffectstack_c = new System.Collections.Generic.List<TTFTextOutlineEffectStackElement> ();	
			outlineeffectstack_serialized_parameters = new System.Collections.Generic.List<ByteArray> ();
			return outlineeffectstack_c.Count;
		}

	}
	
	
	
	
	public void SetOutlineEffectStackElement (int i, TTFTextOutlineEffectStackElement tvalue)
	{
		if (outlineeffectstack_c == null) {
			outlineeffectstack_c = new System.Collections.Generic.List<TTFTextOutlineEffectStackElement> ();	
			outlineeffectstack_serialized_parameters = new System.Collections.Generic.List<ByteArray> ();
		}
		if ((i >= outlineeffectstack_c.Count) && (tvalue != null)) {
			outlineeffectstack_c.Add (tvalue);
			outlineeffectstack_serialized_parameters.Add (SerializeObject (tvalue.parameters));
		} else {
			if (tvalue == null) {
				outlineeffectstack_c.RemoveAt (i);
				outlineeffectstack_serialized_parameters.RemoveAt (i);
			} else {
				outlineeffectstack_c [i] = tvalue;
				outlineeffectstack_serialized_parameters [i] = SerializeObject (tvalue.parameters);
			}
		}
			
	}
	
	public void ResetOutlineEffectStack ()
	{
		outlineeffectstack_c = null;
		outlineeffectstack_serialized_parameters = null;
	}
	
	public bool override_outlineeffectstack = false;
	private System.Collections.Generic.List<TTFTextOutlineEffectStackElement> outlineeffectstack_c;
	[SerializeField]
	private System.Collections.Generic.List<ByteArray> outlineeffectstack_serialized_parameters;
	
	
	
	// -----------------
	private int rebuildmeshes;
	private int rebuildoutlines;
	public bool rebuildLayout;
	
	
	public bool SetBitmapMode(int fe, bool b) {
		try {
		object p=GetFontEngineParameters(fe);
		System.Reflection.FieldInfo fi=p.GetType().GetField("bitmapFontMode");
		if (fi!=null) {
			fi.SetValue(p,b);
			SetFontEngineParameters(fe,p);			
			return true;
		}
		return false;
		} catch (System.Exception e) {
			Debug.LogError(e);
			return false;
		}
	}
	
#endregion	

		
#region PROPERTIES
	public string FontId {
		get { return 
			((stackparent != null) && (!overrideFontId)) ? stackparent.FontId : GetFontEngineFontIdD(GetUsedFontEngine()); }

		set {			
			if (fontId != value) {				
				fontId = value;
				if (useDifferentFontIdForEachFontEngine) {				
					SetFontEngineFontId (GetUsedFontEngine (), value); 
				}
				if (embedFont) {
					SetFontEngineFontId(1,value);
				}

				overrideFontId = true;
				rebuildoutlines++;
			}
		}
	}
	
	
	
	
/*	
	public int InterpolationSteps {
		get { 
#if !TTFTEXT_LITE
#if !TTFTEXT_DELUXE			
			return ((stackparent != null) && (!overrideInterpolationSteps)) ? stackparent.InterpolationSteps : interpolationSteps;
#else
			return 4;
#endif
#else
			return 4;
#endif			
		}
		set { 
			if (interpolationSteps != value) {
				overrideInterpolationSteps = true;
				interpolationSteps = value;
				rebuildoutlines++;
			}
		}
	}
	*/
		
	public float Size { 
		get { return ((stackparent != null) && (!overrideSize)) ? stackparent.Size : size; } 
		set {
			if (size != value) {
				
				/*
				if (lineSpacing == size * fontHeight) { // update lineSpacing
					lineSpacing = value * fontHeight;
				}
				*/
				
				size = value;
				overrideSize = true;
				rebuildmeshes++;
				rebuildLayout = true;
			} 
		}
	}

	public TTFTextStyle SetSize (float s)
	{
		Size = s;
		return this;
	}

	public TTFTextStyle AddSize (float s)
	{
		Size -= s;
		return this;
	}

	public TTFTextStyle MulSize (float s)
	{
		Size *= s;
		return this;
	}
	

	public float Embold {
		get { return ((stackparent != null) && (!overrideEmbold)) ? stackparent.Embold : embold; } 
		set {
			if (embold != value) {
				embold = value;
				overrideEmbold = true;
				rebuildmeshes++;
			}
		} 
		
	}

	public TTFTextStyle SetEmbold (float s)
	{
		Embold = s;
		return this;
	}

	public TTFTextStyle AddEmbold (float s)
	{
		Embold += s;
		return this;
	}

	public TTFTextStyle MulEmbold (float s)
	{
		Embold *= s;
		return this;
	}
	
	public float Slant {
		get { 
			return ((stackparent != null) && (!overrideSlant)) ? stackparent.Slant : slant;
		} 
		set {			
			if (slant != value) {
				slant = value;
				overrideSlant = true;
				rebuildmeshes++;
			} 	
		}
	}
	
	
	
	
	public TTFTextStyle SetSlant (float s)
	{
		Slant = s;
		return this;
	}

	public TTFTextStyle MulSlant (float s)
	{
		Slant *= s;
		return this;
	}

	public TTFTextStyle AddSlant (float s)
	{
		Slant += s;
		return this;
	}
	
	
	
	
	public int MaterialOffset {
		get {
			return ((stackparent != null) && (!overrideMaterialOffset)) ? stackparent.MaterialOffset : materialOffset;
		}
		set {
			if (materialOffset != value) {
				materialOffset = value; 
				overrideMaterialOffset = true;
				rebuildmeshes++;
			} 
		}
	}
	
	
	
	
	public float SimplifyAmount {
		get { 
#if ! TTFTEXT_DELUXE				
			return ((stackparent != null) && ((!overrideSimplifyAmount))) ? stackparent.SimplifyAmount : simplifyAmount ; // TODO (DECIDE IF RELATIVELY TO THE RUNTIME ENGINE)
#else
			return 0.001f;
#endif		
		} 
		set {
			if (simplifyAmount != value) {
				simplifyAmount = value; 
				overrideSimplifyAmount = true;
				rebuildmeshes++;
			}
		}
	}

	public TTFTextStyle SetSimplifyAmount (float s)
	{
		SimplifyAmount = s;
		return this;
	}
	
	
	
	
	
	public float OutlineEmbold {
		get {
			return ((stackparent != null) && ((!overrideOutlineStrength))) ? stackparent.OutlineEmbold : outlineStrength;
		}
		
		set {
			if (outlineStrength != value) {
				outlineStrength = value;
				overrideOutlineStrength = true;
				++rebuildmeshes;
			}
		}
	}

	public TTFTextStyle SetOutlineEmbold (float s)
	{
		OutlineEmbold = s;
		return this;
	}	
	
	
	
	
	// EXTRUSION/MESH GENERATION ALGORITHM PARAMETERS [ ]
	public TTFText.ExtrusionModeEnum ExtrusionMode { 
		get { return ((stackparent != null) && (!overrideExtrusionMode)) ? stackparent.ExtrusionMode : extrusionMode ;}
		set {
			if (extrusionMode != value) {
				extrusionMode = value;
				overrideExtrusionMode = true;
				rebuildmeshes++;
			}
		} 
	}
	
	public float ExtrusionDepth { 
		get { return ((stackparent != null) && (!overrideExtrusionDepth)) ? stackparent.ExtrusionDepth : extrusionDepth;}
		
		set {
			if (extrusionDepth != value) {
				extrusionDepth = value;
				overrideExtrusionDepth = true;
				rebuildmeshes++;
			}
		}
	}

	public TTFTextStyle SetExtrusionDepth (float s)
	{
		ExtrusionDepth = s;
		return this;
	}
	
	public bool BackFace {
		get { return ((stackparent != null) && (!overrideBackFace)) ? stackparent.BackFace : backFace; }
		set {
			if (backFace != value) {
				backFace = value;
				overrideBackFace = true;
				rebuildmeshes++;
			}
		}
	}

	public AnimationCurve FreeHandCurve {
		get {
			if (freeHandCurve == null)
				FreeHandCurve = AnimationCurve.Linear (0, 0, 1, 1);
			return ((stackparent != null) && (!overrideFreeHandCurve)) ? stackparent.FreeHandCurve : freeHandCurve;
		}
		set {
			freeHandCurve = value;
			overrideFreeHandCurve = true;
			rebuildmeshes++;
		}
	}

	public float BevelForce {
		get { return ((stackparent != null) && (!overrideBevelForce)) ? stackparent.BevelForce : bevelForce; }
		set {
			if (bevelForce != value) {
				bevelForce = value;
				overrideBevelForce = true;
				rebuildmeshes++;
			}
		}
	}

	public float[] ExtrusionSteps {
		get { return ((stackparent != null) && (!overrideExtrusionSteps)) ? stackparent.ExtrusionSteps : extrusionSteps; }
		set {
			extrusionSteps = value;
			overrideExtrusionSteps = true;
			rebuildmeshes++;
		}
	}

	public int NbDiv {
		get { return ((stackparent != null) && (!overrideNbDiv)) ? stackparent.NbDiv : nbDiv; }
		set {
			if (nbDiv != value) {
				nbDiv = value;
				overrideNbDiv = true;
				rebuildmeshes++;
			}
		}
	}

	public float Gamma {
		get { return ((stackparent != null) && (!overrideGamma)) ? stackparent.Gamma : gamma; }
		set {
			if (gamma != value) {
				gamma = value;
				overrideGamma = true;
				rebuildmeshes++;
			}
		}
	}

	public float BevelDepth {
		get { return ((stackparent != null) && (!overrideBevelDepth)) ? stackparent.BevelDepth : bevelDepth; }
		set {
			if (bevelDepth != value) {
				bevelDepth = value;
				overrideBevelDepth = true;
				rebuildmeshes++;
			}
		}
	}

	public float Radius {
		get { return ((stackparent != null) && (!overrideRadius)) ? stackparent.Radius : radius; }
		set {
			if (radius != value) {
				radius = Mathf.Max (0, value);
				overrideRadius = true;
				rebuildmeshes++;
			}
		}
	}

	public int NumPipeEdges {
		get { return ((stackparent != null) && (!overrideNumPipeEdges)) ? stackparent.NumPipeEdges : numPipeEdges; }
		set {
			if (numPipeEdges != value) {
				numPipeEdges = Mathf.Max (1, value);
				overrideNumPipeEdges = true;
				rebuildmeshes++;
			}
		}
	}
	
	public TTFText.ParagraphAlignmentEnum ParagraphAlignment { 
		get { return ((stackparent != null) && (!overrideParagraphAlignement)) ? stackparent.ParagraphAlignment : paragraphAlignment; } 
		set {
			if (paragraphAlignment != value) {
				paragraphAlignment = value; 
				overrideParagraphAlignement = true;
				rebuildLayout = true;
				rebuildmeshes++;
			}
		}
	}

	public TTFTextStyle SetParagraphAlignment (TTFText.ParagraphAlignmentEnum s)
	{
		ParagraphAlignment = s;
		return this;
	}
	
	
	
	public float LineWidth { 
		get { return ((stackparent != null) && (!overrideLineWidth)) ? stackparent.LineWidth : lineWidth; } 
		set { 
			if (lineWidth != value) {
				lineWidth = value;
				overrideLineWidth = true;
				rebuildLayout = true;
				rebuildmeshes++;
			}
		}
	}
	
	
	
	
	public float LineSpacingMult {
		get { return ((stackparent != null) && (!overrideLineSpacingMult)) ? stackparent.LineSpacingMult : lineSpacingMult; }
		set {
			if (lineSpacingMult != value) {
				lineSpacingMult = value;
				overrideLineSpacingMult = true;
				rebuildLayout = true;
				++rebuildmeshes;
			}
		}
	}

	public TTFTextStyle SetLineSpacingMult (float s)
	{
		LineSpacingMult = s;
		return this;
	}
	
	
	
	
	public float LineSpacingAdd {
		get { return ((stackparent != null) && (!overrideLineSpacingAdd)) ? stackparent.LineSpacingAdd : lineSpacingAdd; }
		set {
			if (lineSpacingAdd != value) {
				lineSpacingAdd = value;
				overrideLineSpacingAdd = true;
				rebuildLayout = true;
				++rebuildmeshes;
			}
		}
	}

	public TTFTextStyle SetLineWidthAdd (float s)
	{
		LineSpacingAdd = s;
		return this;
	}
	
	
	
	
	public float Hspacing {
		get { 		
			return ((stackparent != null) && (!overrideHspacing)) ? (stackparent.Hspacing) : hspacing; 
		} 
		set {
			if (hspacing != value) { 
				hspacing = value; 
				overrideHspacing = true;
				rebuildLayout = true;
				rebuildmeshes++;
			}
		} 
	}
	
	
	
	
	
	public bool overrideWordSpacingFactor = false;
	public bool overrideHSpacingMultFactor = false;
	public bool overrideHSpacingMode = false;
	public bool overrideSplitSides = false;
	
	public float WordSpacingFactor { 
		get { 
			return ((stackparent != null) && (!overrideWordSpacingFactor)) ? stackparent.WordSpacingFactor : wordSpacingFactor; 
		} 
		set { 
			if (wordSpacingFactor != value) {
#if !TTFTEXT_DELUXE				
				wordSpacingFactor = value; 
#endif				
				overrideWordSpacingFactor = true;
				rebuildLayout = true;
				rebuildmeshes++;
			}
		} 
	}
	
	public float HSpacingMultFactor { 
		get { return ((stackparent != null) && (!overrideHSpacingMultFactor)) ? stackparent.HSpacingMultFactor : hSpacingMultFactor ; } 
		set { 
			if (hSpacingMultFactor != value) {
#if !TTFTEXT_DELUXE				
				overrideHSpacingMultFactor = true;
				hSpacingMultFactor = value;
#endif								
				rebuildLayout = true;
				rebuildmeshes++;
			} 
		} 
	}
	
	public TTFText.HSpacingModeEnum HSpacingMode { 
		get { return ((stackparent != null) && (!overrideHSpacingMode)) ? stackparent.HSpacingMode : hSpacingMode ; } 
		set { 
			if (hSpacingMode != value) {
#if !TTFTEXT_DELUXE								
				hSpacingMode = value; 
				overrideHSpacingMode = true;
#endif				
				rebuildLayout = true;
				rebuildmeshes++;
			} 
		}
	}

	public float FirstLineOffset { 
		get { return ((stackparent != null) && (!overrideFirstLineOffset)) ? stackparent.FirstLineOffset : firstLineOffset ; } 
		set { 
			if (firstLineOffset != value) {
#if !TTFTEXT_DELUXE								
				firstLineOffset = value; 
				overrideFirstLineOffset = true;
#endif				
				rebuildLayout = true;
				rebuildmeshes++;
			} 
		}
	}
		
	public TTFText.WordSplitModeEnum WordSplitMode {
		get { 
			return ((stackparent != null) && (!overrideWordSplitMode)) ? stackparent.WordSplitMode : wordSplitMode; 
		} 
		set {
			if (wordSplitMode != value) {
				wordSplitMode = value;
				overrideWordSplitMode = true;
				rebuildLayout = true;
				rebuildmeshes++;
			}
		} 
	}

	
	//public bool useKerning = false; // currently disabled
	public GameObject GlyphPrefab {
		get { return ((stackparent != null) && (!overrideGlyphPrefab)) ? stackparent.GlyphPrefab : glyphPrefab; }
		set {
			overrideGlyphPrefab = true;
			glyphPrefab = value;
			if (glyphPrefab != null)
				glyphPrefabHash = glyphPrefab.GetHashCode ();
			rebuildmeshes++;
		}
	}	
	
	// Texture mapping	
	public bool SplitSides {
		get { 
			return ((stackparent != null) && (!overrideSplitSides)) ? stackparent.SplitSides : splitSides; 
		} 
		set {
			if (splitSides != value) {
				splitSides = value; 
				overrideSplitSides = true;
				rebuildmeshes++;
			}
		}
	}
	
	public TTFText.UVTypeEnum UvType {
		get {
			return ((stackparent != null) && (!overrideUVType)) ? stackparent.UvType : uvType; 
		} 
		set {
			if (uvType != value) {
				uvType = value;
				overrideUVType = true;
				rebuildmeshes++;
			}
		} 
	}
	
	public bool NormalizeUV {
		get { 
			return ((stackparent != null) && (!overrideNormalizeUV)) ? stackparent.NormalizeUV : normalizeUV; 
		} 
		set {
			if (normalizeUV != value) {
				normalizeUV = value;
				overrideNormalizeUV = true;
				rebuildmeshes++;
			}
		} 
	}
	
	public Vector3 UvScaling {
		get {
			return ((stackparent != null) && (!overrideUVScaling)) ? stackparent.UvScaling : uvScaling; 
		} 
		set {
			if (uvScaling != value) {
				uvScaling = value;
				overrideUVScaling = true;
				rebuildmeshes++;
			}
		} 
	}
	
	
	public bool orientationReversed = false;
	public bool OrientationReversed  { get { return orientationReversed; } }

	

#endregion

	
	bool ShouldRebuildLayout ()
	{
		return rebuildLayout;
	}

	
	
	public void ResetStyle ()
	{
		foreach (System.Reflection.FieldInfo mi in typeof(TTFTextStyle).GetFields()) {
			if ((mi.Name.ToLower ().StartsWith ("override")) && (mi.FieldType == typeof(bool))) {
				mi.SetValue (this, false);
			}	
			
		}
		
	}
}
