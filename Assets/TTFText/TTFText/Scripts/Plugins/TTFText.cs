//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !TTFTEXT_LITE
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
using TTF = FTSharp;
#endif
#endif


/// <summary>
/// This is the main TTF Text Monobehaviour script.
/// It basically does what necessary does what is necessary 
/// to attach text to an object of the game.
/// </summary>
[AddComponentMenu("Text/TTF Text")]
[ExecuteInEditMode]
public class TTFText : MonoBehaviour
{
	
#region ENUMERATIONS
	//public enum DynamicTextRuntimeFontEngineMethodEnum
	//{
	//    DirectNative,
	//    EmbeddedAndNetworkFonts
	// }
	// public static string[] DynamicTextRuntimeFontEngineMethodTxt = { "Native Library (FreeType)", "Embedded Fonts"};
	
	
	public enum DynamicTextRuntimeTriangulationMethodEnum
	{
		NativeGLUT,
		CSharpLibs
	}
	public static string[] DynamicTextRuntimeTriangulationMethodTxt = { "Native Library (GLUT)", "CSharp Implementation (Clipper,Poly2Tri)"};
	
	public enum HJustEnum
	{
		Origin,
		Center,
		Left,
		Right
	}
	public static string[] HJustTxt = { "Origin", "Center", "Left", "Right" };

	public enum VJustEnum
	{
		Origin,
		Center,
		Top,
		Bottom
	}
	public static string[] VJustTxt = { "Origin", "Center", "Top", "Bottom" };


	public enum ParagraphAlignmentEnum
	{
		Left,
		Center,
		Right,
		Justified,
		FullyJustified
	}
	public static string[] ParagraphAlignmentTxt = { "Left", "Center", "Right", "Justified", "Fully Justified" };


	public enum ExtrusionModeEnum
	{
		None
        ,
		Simple
        ,
		Bevel
        ,
		Bent
        ,
		FreeHand
        ,
		Pipe

	}
	
	
	
	
	
	
	public static string[] ExtrusionTxt = { "None", "Simple"

                                              , "Bevel", "Bent", "Free Hand", "Pipe" 

                                          };

#if TTFTEXT_LITE
	[SerializeField]
	private bool demoMode=false;
	public bool DemoMode { get { return demoMode;} set { if (demoMode!=value) {demoMode=value; Text="Hello world"; Slant=0; Embold=0; InitTextStyle.ResetOutlineEffectStack(); SetDirty();}}}
#endif	

	public enum UVTypeEnum
	{
		Box,
		Spherical
	}
	public static string[] UVTypeTxt = { "Box", "Spherical" };


	public enum TokenModeEnum
	{
		Text,
		Line,
		Word,
		Character
	}
	public static GUIContent[] TokenTxt = { new GUIContent ("Text"),new GUIContent ("Line"), new GUIContent ("Word"), new GUIContent ("Character")};

	public enum LayoutModeEnum
	{
		No
        ,
		Wrap,
		StyleEnabled
	}
	public static string[] LayoutTxt = { "Simple"
                                           ,"Legacy" 
										   ,"Advanced (Legacy + Markup)"
                                           };

	public enum WordSplitModeEnum
	{
		SpaceBased,
		Character		
	}
	public static string[] WordSplitModeTxt = { "Space characters"
                                    , "Individual Characters" 
	};

	
	public enum HSpacingModeEnum
	{
		GlyphBoundaries,
		GlyphAdvance
	}
	public static string[] HSpacingModeTxt = { "Glyph Boundaries", "Glyph Position" };

#endregion

	[SerializeField]
	private TTFTextStyle initTextStyle = null;
	[SerializeField]
	// <- Expose to animator and save
	private string text = "";
	[SerializeField]
	private TTFText.TokenModeEnum tokenMode = TTFText.TokenModeEnum.Character;
	[SerializeField]
	private TTFText.LayoutModeEnum layoutMode =
#if ! TTFTEXT_LITE 		
		TTFText.LayoutModeEnum.StyleEnabled
#else 
	    TTFText.LayoutModeEnum.No
#endif			
			;
	
	public bool OrientationReversed  { get { return initTextStyle.OrientationReversed; } }
	
	[SerializeField]
	private bool isStyleObject = false;
	
	public bool IsStyleObject {
		get { return isStyleObject;}
		set {
			isStyleObject = value;
			if (value) {
				//gameObject.hideFlags=HideFlags.HideAndDontSave;
				gameObject.hideFlags = HideFlags.DontSave;
			} else {
				gameObject.hideFlags = 0;
			}
		}
	}
	
	public bool rebuildLayout = false;
	public bool embedCharset = false;
	[SerializeField]
	private TTFText.HJustEnum hJust = TTFText.HJustEnum.Center;
	[SerializeField]
	private TTFText.VJustEnum vJust = TTFText.VJustEnum.Center;

	public TTFText.HJustEnum HJust { 
		get { return hJust; } 
		set {
			if (hJust != value) {
				hJust = value; 
				rebuildLayout = true;
				rebuildmeshes++;
			}
		}
	}

	public TTFText.VJustEnum VJust { 
		get { return vJust; } 
		set {
			if (vJust != value) {
				vJust = value;
				rebuildLayout = true;
				rebuildmeshes++;
			}
		}
	}
	
	[SerializeField]
	private DynamicTextRuntimeTriangulationMethodEnum dynamicTextRuntimeTriangulationMethod;
	[SerializeField] 
	private bool autoRebuild = true;
	
	
//#if !TTFTEXT_LITE				
//	private TTFTextFontStoreFont tfsf=null;
//#endif	
	
	// ======================================================
	// Main parameters
	// ======================================================

	public string FontId {

		get { return InitTextStyle.FontId; }

		set {
			if (InitTextStyle.FontId != value) {				
				InitTextStyle.FontId = value;
				InitTextStyle.SetFontEngineFontId (initTextStyle.PreferredEngine (Application.platform), value);								
			}
		}
	}
	

	public string Text { 
		get { 
#if TTFTEXT_LITE
			if (DemoMode) {
				if (LayoutMode==TTFText.LayoutModeEnum.StyleEnabled) {
					return "<@Size=*1.5@>Big<@pop@>,<@Embold=+1@>Bold<@pop@>,<@Slant=+0.5@>Italic<@pop@> <@style=ttftext@><@Embold=+1@>TTF<@pop@>Text<@pop@>.";
				}
				else {
					return " Amazing !";
				}
				
			}
#endif			
			
			return text; 
		} 
		set {
			if (text != value) {
				text = value; 
				rebuildoutlines++;		
			}
		} 
	}
	
	void OnEnable ()
	{
		//Trace.DEBUG("OnEnable:" + Text);
	}
	
	public int instanceID_ = 0;
	
	public void Reset ()
	{
		//Trace.DEBUG("Reset: " + Text);
	}
	
	void Awake ()
	{
		//	Trace.DEBUG("Awake:" + Text + " id:" + instanceID_ + " playing=" + Application.isPlaying);
		// We want to clone the mesh on Object duplication.
		// TODO: find a better way to detect GameObject duplication
		// Not Perfect as this still get called on scene load, 
		// but no more on Play/Edit mode switch
		// Debug.Log("id:" + instanceID_ );
#if !TTFTEXT_LITE					
		if ((mesh == null)
			&& (TokenMode == TTFText.TokenModeEnum.Text)) {
			if (initTextStyle != null) {
				initTextStyle = new TTFTextStyle (initTextStyle);
			} else {
				initTextStyle = new TTFTextStyle ();
			}
			instanceID_ = GetInstanceID ();
			
			RebuildText ();
			
		}	
#endif	
		
		
		
		if (instanceID_ != GetInstanceID ()) {
			if (mesh != null) { //Clone the mesh		
				if (!Application.isEditor || Application.isPlaying) { // Unexpected context, log a warning
					// MAY HAPPEN WITH PREFAB
					//Debug.LogWarning("TTFTextMesh Duplicate: isEditor=" + Application.isEditor + " isPlaying=" + Application.isPlaying);
				
				} else { // We are in Edit Mode
					//Debug.LogWarning("Duplicate Mesh:" + instanceID_ + "->" + GetInstanceID());
					
					initTextStyle = new TTFTextStyle (initTextStyle);
					mesh = (Mesh)Instantiate (mesh);
#if !TTFTEXT_LITE					
					TTFTextInternal.Engine.UpdateComponentsWithNewMesh (gameObject, mesh);
#else
					MeshFilter mf = GetComponent<MeshFilter>();
					if (mf != null) {
						mf.sharedMesh = mesh;
					}
		
					InteractiveCloth ic = GetComponent<InteractiveCloth>();
					if (ic != null) {
						ic.mesh = mesh;
					}		
#endif					
				}
			}
		}

		
		instanceID_ = GetInstanceID ();
		
		currentStyleIdx = 0;
	}
	
	public void Start ()
	{
		stylesavedtimestamp = rebuildmeshes;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
#if !TTFTEXT_LITE		
		if (InitTextStyle.FontId.Length==0) {						
			try {
				System.Collections.Generic.IEnumerator<string> e=TTFTextFontListManager.Instance.LocalFonts.Keys.GetEnumerator();
				e.MoveNext();				
				InitTextStyle.FontId=e.Current;
			}
			catch {
				System.Collections.Generic.IEnumerator<string> e=TTFTextFontListManager.Instance.SystemFonts.Keys.GetEnumerator();
				e.MoveNext();				
				InitTextStyle.FontId=e.Current;
			}
		}
		
		TTFTextInternal.TTFTextFontEngine.font_engines[InitTextStyle.GetUsedFontEngine()].RegisterClient(this);
#endif		
#endif		
	}
	
	public int DynamicTextRuntimeFontEngineMethod {
		
		get { return InitTextStyle.GetUsedFontEngine ();}
	
		
		/*
		set { 
			
			//Debug.LogWarning("Runtime method=" + value);
			
			if (dynamicTextRuntimeFontEngineMethod!=value) {
				dynamicTextRuntimeFontEngineMethod=value;
				
#if !TTFTEXT_LITE
				
				if (DynamicTextRuntimeFontEngineMethod==DynamicTextRuntimeFontEngineMethodEnum.EmbeddedAndNetworkFonts) {
					
					TTFTextFontStore tfs =  TTFTextFontStore.Instance;
	
					tfs.RegisterClient(this);
					
					EmbedCharset=true;
					
					tfsf=tfs.EnsureFont(initTextStyle.FontId);
					
					if (tfsf!=null) {
						tfsf.BuildCharSet(this);
						tfsf.incref();
					}
					
				} else {
					
					if (tfsf!=null) {
						tfsf.decref();
						tfsf=null;
					}					
					EmbedCharset=false;
					
					if (TTFTextFontStore.IsInstanciated) {
						// NOTIFY THE FONT STORE WE ARE NO MORE A CLIENT
						TTFTextFontStore tfs= TTFTextFontStore.Instance;
						tfs.UnregisterClient(this);						
					}
					
				}
#endif				
				rebuildmeshes++;
			}
		}
		
		*/
	}
	
	public DynamicTextRuntimeTriangulationMethodEnum DynamicTextRuntimeTriangulationMethod {
		
		get {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN  || UNITY_STANDALONE_OSX
			return dynamicTextRuntimeTriangulationMethod;
#else
			// no choice
			return DynamicTextRuntimeTriangulationMethodEnum.CSharpLibs;
#endif			
		}
	
		set { 
			if (dynamicTextRuntimeTriangulationMethod != value) {
				dynamicTextRuntimeTriangulationMethod = value;
				rebuildmeshes++;
			}
		}
	}
	
	
	
	
#if FALSE	
	public float Size { 
		get { return size; } 
		set {
			if (size!=value) {
				
				/*
				if (lineSpacing == size * fontHeight) { // update lineSpacing
					lineSpacing = value * fontHeight;
				}
				*/
				
				size=value; 
				rebuildmeshes++;
				rebuildLayout = true;
			} 
		}
	}
	
	public float SetSize(float s) {Size=s; return s;}
#endif	

	
	
	public float Size { 
		get { return initTextStyle.Size; } 
		set {
			if (initTextStyle.Size != value) {
				initTextStyle.Size = value;
				rebuildmeshes++;
			}
		}
	}

	public float Embold { 
		get { 
#if TTFTEXT_LITE
			if (!DemoMode) {
				return 0;
			}
#endif			
			return initTextStyle.Embold; 
		} 
		set {
			if (initTextStyle.Embold != value) {
				initTextStyle.Embold = value;
				rebuildmeshes++;				
			}
		}
	}

	public float Slant { 
		get { 
#if TTFTEXT_LITE
			if (!DemoMode) {
				return 0;
			}
#endif			
			return initTextStyle.Slant; 
		} 
		set {
			if (initTextStyle.Slant != value) {
				initTextStyle.Slant = value;
				rebuildmeshes++;				
			}
		}
	}

	public float SimplifyAmount { 
		get { 
#if TTFTEXT_LITE
			if (!DemoMode) {
				return 0;
			}
#endif						
			return initTextStyle.SimplifyAmount;
		} 
		set {
			if (initTextStyle.SimplifyAmount != value) {
				initTextStyle.SimplifyAmount = value;
				rebuildmeshes++;				
			}
		}
	}

	public float OutlineEmbold { 
		get { 
#if TTFTEXT_LITE
			if (!DemoMode) {
				return 0;
			}
#endif			
			
			return initTextStyle.OutlineEmbold;
		} 
		set {
			if (initTextStyle.OutlineEmbold != value) {
				initTextStyle.OutlineEmbold = value;
				rebuildmeshes++;				
			}
		}
	}

	public TTFText.ExtrusionModeEnum ExtrusionMode {
		get {  
#if TTFTEXT_LITE
			if (!DemoMode) {
				if ((initTextStyle.ExtrusionMode!=ExtrusionModeEnum.Simple)&&(initTextStyle.ExtrusionMode!=ExtrusionModeEnum.None)) {
					return ExtrusionModeEnum.None;
				}
			}
#endif			
			
			return InitTextStyle.ExtrusionMode;
		}
		set { if (InitTextStyle.ExtrusionMode != value) {
				initTextStyle.ExtrusionMode = value;
				rebuildmeshes++;
			}}
	}

	public float ExtrusionDepth { get { return  initTextStyle.ExtrusionDepth; } set { if (initTextStyle.ExtrusionDepth != value) {
				initTextStyle.ExtrusionDepth = value;
				rebuildmeshes++;
			} } }
		
	// No Extrusion Mode
	public bool BackFace  { get { return  initTextStyle.BackFace; } set { if (initTextStyle.BackFace != value) {
				initTextStyle.BackFace = value;
				rebuildmeshes++;
			} } }
	
	// Free form extrusion
	public AnimationCurve FreeHandCurve {
		get { return initTextStyle.FreeHandCurve; }
		set { initTextStyle.FreeHandCurve = value;
			rebuildmeshes++;}
	}
	
	public float BevelForce { get { return initTextStyle.BevelForce; } set { if (initTextStyle.BevelForce != value) {
				initTextStyle.BevelForce = value;
				rebuildmeshes++;
			} } }

	public float[] ExtrusionSteps {
		get { return initTextStyle.ExtrusionSteps; }
		set { initTextStyle.ExtrusionSteps = value;
			rebuildmeshes++;}
	}

	public int NbDiv { get { return initTextStyle.NbDiv; } set { if (initTextStyle.NbDiv != value) {
				initTextStyle.NbDiv = value;
				rebuildmeshes++;
			} } }

	public float Gamma { get { return initTextStyle.Gamma; } set { if (initTextStyle.Gamma != value) {
				initTextStyle.Gamma = value;
				rebuildmeshes++;
			} } }

	public float BevelDepth { get { return initTextStyle.BevelDepth; } set { if (initTextStyle.BevelDepth != value) {
				initTextStyle.BevelDepth = value;
				rebuildmeshes++;
			} } }
	
	// Pipe
	public float Radius { get { return initTextStyle.Radius; } set { if (initTextStyle.Radius != value) {
				initTextStyle.Radius = value;
				rebuildmeshes++;
			} } }

	public int NumPipeEdges { get { return initTextStyle.NumPipeEdges; } set { if (initTextStyle.NumPipeEdges != value) {
				initTextStyle.NumPipeEdges = value;
				rebuildmeshes++;
			} } }

	public LayoutModeEnum LayoutMode { 
		get { 
#if TTFTEXT_LITE
			if (!DemoMode) {
				return TTFText.LayoutModeEnum.No;
			}
#endif			
			return layoutMode;
		} 
		set {
			if (layoutMode != value) {
				layoutMode = value; 
				//rebuildLayout = true;
				rebuildmeshes++;
			}
		}
	}
	
	public ParagraphAlignmentEnum ParagraphAlignment { 
		get { return initTextStyle.ParagraphAlignment; } 
		set {
			if (initTextStyle.ParagraphAlignment != value) {
				initTextStyle.ParagraphAlignment = value; 
				//rebuildLayout = true;
				rebuildmeshes++;
			}
		}
	}
	
	
	
	
	//public bool extendsWidth = false;
	public float LineWidth { 
		get { return initTextStyle.LineWidth; } 
		set { 
			if (initTextStyle.LineWidth != value) {
				initTextStyle.LineWidth = value;
				//rebuildLayout = true;
				rebuildmeshes++;
			}
		}
	}
	
	public float LineSpacingMult {
		get { return initTextStyle.LineSpacingMult; }
		set {
			if (initTextStyle.LineSpacingMult != value) {
				initTextStyle.LineSpacingMult = value;
				//rebuildLayout = true;
				++rebuildmeshes;
			}
		}
	}
	
	public float LineSpacingAdd {
		get { return initTextStyle.LineSpacingAdd; }
		set {
			if (initTextStyle.LineSpacingAdd != value) {
				initTextStyle.LineSpacingAdd = value;
				++rebuildmeshes;
			}
		}
	}

	public float Hspacing {
		get { 
#if !TTFTEXT_LITE			
			return initTextStyle.Hspacing;
#else
        return 1;
#endif
		} 
		set {
			if (initTextStyle.Hspacing != value) { 
				initTextStyle.Hspacing = value; 
				rebuildmeshes++;
			}
		} 
	}

	public float WordSpacingFactor { 
		get { return initTextStyle.WordSpacingFactor; } 
		set { 
			if (initTextStyle.WordSpacingFactor != value) {
				initTextStyle.WordSpacingFactor = value; 
				rebuildmeshes++;
			}
		} 
	}

	public float HSpacingMultFactor { 
		get { return initTextStyle.HSpacingMultFactor ; } 
		set { 
			if (initTextStyle.HSpacingMultFactor != value) {
				initTextStyle.HSpacingMultFactor = value;
				rebuildmeshes++;
			} 
		} 
	}
	
	public HSpacingModeEnum HSpacingMode { 
		get { return initTextStyle.HSpacingMode ; } 
		set { 
			if (initTextStyle.HSpacingMode != value) {
				initTextStyle.HSpacingMode = value; 
				rebuildmeshes++;
			} 
		}
	}

	public float FirstLineOffset { 
		get { return initTextStyle.FirstLineOffset ; } 
		set { 
			if (initTextStyle.FirstLineOffset != value) {
				initTextStyle.FirstLineOffset = value; 
				rebuildmeshes++;
			} 
		}
	}
	
	public GameObject GlyphPrefab {
		get { 
#if TTFTEXT_LITE
			if (!DemoMode) {
				return null;
			}
#endif						
			return initTextStyle.GlyphPrefab;
		}
		set { initTextStyle.GlyphPrefab = value;
			rebuildmeshes++;}
	}	
	
	
	// Texture mapping	
	public bool SplitSides {
		get { 
#if TTFTEXT_LITE
			if (!DemoMode) {
				return false;
			}
#endif			

			return initTextStyle.SplitSides; 
		} 
		set {
			if (initTextStyle.SplitSides != value) {
				initTextStyle.SplitSides = value;
				rebuildmeshes++;
			}
		}
	}
	
	public int MaterialOffset { 
		get { 
#if TTFTEXT_LITE
			if (!DemoMode) {
				return 0;
			}
#endif
			

			return initTextStyle.MaterialOffset; 
		} 
		set {
			if (initTextStyle.MaterialOffset != value) {
				initTextStyle.MaterialOffset = value; 
				rebuildmeshes++;
			}
		}
	}
	
	public TTFText.UVTypeEnum UvType {
		get {
#if TTFTEXT_LITE
			if (!DemoMode) {
				return UVTypeEnum.Box;
			}
#endif			
			
			return initTextStyle.UvType; 
		} 
		set { if (initTextStyle.UvType != value) {
				initTextStyle.UvType = value;
				rebuildmeshes++;
			}} 
	}
	
	public TTFText.WordSplitModeEnum WordSplitMode {
		get {
			return initTextStyle.WordSplitMode; 
		} 
		set { if (initTextStyle.WordSplitMode != value) {
				initTextStyle.WordSplitMode = value;
				rebuildLayout = true;
				rebuildmeshes++;
			}} 
	}
	
	public bool NormalizeUV {
		get { 
#if TTFTEXT_LITE
			if (!DemoMode) {
				return true;
			}
#endif			
			
			return initTextStyle.NormalizeUV; 
		} 
		set { if (initTextStyle.NormalizeUV != value) {
				initTextStyle.NormalizeUV = value;
				rebuildmeshes++;
			}} 
	}
	
	public Vector3 UvScaling {
		get {
#if TTFTEXT_LITE
			if (!DemoMode) {
				return Vector3.one;
			}
#endif			
			

			return initTextStyle.UvScaling; 
		} 
		set { if (initTextStyle.UvScaling != value) {
				initTextStyle.UvScaling = value;
				rebuildmeshes++;
			}} 
	}
	
	
		
	//public bool useKerning = false; // currently disabled
	

	
	public TokenModeEnum TokenMode { 
		get { 
#if TTFTEXT_LITE
			if (!DemoMode) {
				return TokenModeEnum.Text;
			}
#endif			
			
			return tokenMode;
		} 
		set {
			if (tokenMode != value) {
				tokenMode = value;
				rebuildmeshes++;
			}
		}
	}
	
	public bool AutoRebuild {
		get { return autoRebuild;}
		set { autoRebuild = value;}
	}
	
/*	public bool EmbedCharset { 
		
		get { return DynamicTextRuntimeFontEngineMethod==DynamicTextRuntimeFontEngineMethodEnum.EmbeddedAndNetworkFonts; } 
		
		
		set {
			
#if !TTFTEXT_LITE
			if (value) {
				 DynamicTextRuntimeFontEngineMethod=DynamicTextRuntimeFontEngineMethodEnum.EmbeddedAndNetworkFonts; 
			}
			else {
				 DynamicTextRuntimeFontEngineMethod=DynamicTextRuntimeFontEngineMethodEnum.DirectNative; 				
			}
#endif
		} 
	}
	*/ 
	
	public TTFTextStyle InitTextStyle {
		get { if (initTextStyle == null)
				initTextStyle = new TTFTextStyle ();
			return initTextStyle;}
		set { initTextStyle = value;}
	}
	
	
	
	// ==========================================================
	//
	// ==========================================================
#if !TTFTEXT_LITE		

/*	
	
    public string SpecialChars {
        get { return additionalChar; }
        set {
            if (additionalChar != value) {
                additionalChar = value;
                if (DynamicTextRuntimeFontEngineMethod==TTFText.DynamicTextRuntimeFontEngineMethodEnum.EmbeddedAndNetworkFonts) {
                    BuildCharSet();
                }
                rebuildmeshes++;
            }
        }
    }
    
	//public string runtimeFontPath = "";//"Assets/TTFText/Fonts/Monaco.ttf";
	
	[SerializeField] public string additionalChar = "";
	
*/	
	
	
	
	
	
	
	
	
	
	
	// THIS IS BEING MOVED TO THE TTF TEXT FONT STORE
	/*
	// Ascii Charset
	[SerializeField] public Outline [] charset=null;

    // Additional Chars
    
    [SerializeField] public Outline[] addCharset = null;

	//[SerializeField] private List<List<Vector3>> [] charset_boundaries=null;
	[SerializeField] private Vector3 [] charset_advance=null;
	*/
	
	
#endif
	public Mesh mesh = null;
	public Vector3 advance = Vector3.zero;
	public bool autoCreateStyles = false;
	public bool nonFoundStyles = false;
	public int statistics_num_vertices = 0;
	public int statistics_num_subobjects = 0;
	
	
	
	// For Editor state
	//[HideInInspector]
	//public bool showSystemFonts = true;
	[HideInInspector]
	public bool updateObjectName = false; // false by default when scripting but true by default when created from the editor
	
	
	#region SAVE_ATOM_POS	
	/// <summary>
	/// Updated and used in TTFTextInternal.BuildTextMesh function 
	/// to keep track of previous token positions
	/// </summary>	
	public struct TrInfo
	{
		public Vector3 localPosition;
		public Quaternion localRotation;
		public Vector3 localScale;
	
		public TrInfo (Transform tr)
		{
			localPosition = tr.localPosition;
			localRotation = tr.localRotation;
			localScale = tr.localScale;
		}
	}
	
	
	
	
	
	
	
	
#if !TTFTEXT_DELUXE	
	public bool SaveTokenPos = false;
#else
	public bool SaveTokenPos { get {return false;} }
#endif
	public List<TrInfo> TokenPos = new List<TrInfo> ();
	#endregion
	
	
	
	
	
	#region REBUILD_MANAGEMENT
	[SerializeField]
	// <- Expose to animator and save
	private float rebuildoutlines = 0;
	[SerializeField]
	private float lrebuildoutlines = -1;
	[SerializeField]
	// <- Expose to animator and save
	private float rebuildmeshes = 0;
	[SerializeField]
	private float lrebuildmeshes = -1;
	private float stylesavedtimestamp;
	
	public bool isDirty ()
	{ 
		return rebuildLayout || (rebuildoutlines != lrebuildoutlines) || (rebuildmeshes != lrebuildmeshes); 
	}
	
	public void SetClean ()
	{ 
		lrebuildoutlines = rebuildoutlines; 
		lrebuildmeshes = rebuildmeshes;
		rebuildLayout = false;
	}
	
	public void SetDirty ()
	{
		rebuildLayout = true;
		++rebuildoutlines;
		++rebuildmeshes;
	}

	public bool Dirty {
		get { return isDirty (); }
		set {
			if (value) {
				SetDirty ();
			} else { 
				SetClean ();
			}
		}
	}
	#endregion
	
	
	
	public int currentStyleIdx;
	public System.Collections.Generic.List<TTFTextStyle> UsedStyles;
	
	public void ResetUsedStyles ()
	{
		if (UsedStyles == null) {
			UsedStyles = new System.Collections.Generic.List<TTFTextStyle> ();
		}
		UsedStyles.Clear ();
		UsedStyles.Add (InitTextStyle);
		currentStyleIdx = 0;
	}

	public System.Collections.Generic.List<TTFTextStyle> AltStyleList {
		get { 
			return UsedStyles.GetRange (1, UsedStyles.Count - 1);
		}
	}
	
	public TTFTextStyle CurrentTextStyle {
		get { return UsedStyles [currentStyleIdx];}
		set {
			int idx = 0;
			foreach (TTFTextStyle s in UsedStyles) {				
				if (s.getid () == value.getid ()) {
					currentStyleIdx = idx;					
					return;
				}
				idx++;
			}
			currentStyleIdx = idx;					
//			Debug.Log("Adding style :" + idx.ToString() + "v:"+value.FontId+" id:"+value.getid());
			UsedStyles.Add (value);  
		} 
	}
	
	public int GetPreferedEngine (RuntimePlatform p)
	{
		return InitTextStyle.PreferredEngine (p);
	}

	public int GetDefaultNativeEngine (RuntimePlatform p)
	{
		return InitTextStyle.DefaultNativeEngine (p);
	}

	public int GetDefaultEmbeddedEngine (RuntimePlatform p)
	{
		return InitTextStyle.DefaultEmbeddedEngine (p);
	}
	
	public void SetPreferedEngine (RuntimePlatform p, int v)
	{
		InitTextStyle.SetPreferedEngine (p, v);
	}
	
//	public bool busy=false;
#if !TTFTEXT_LITE			

	
	void Update ()
	{		

#if !TTFTEXT_DELUXE
#if UNITY_EDITOR
		if ((Application.isEditor)/*&&(!Application.isPlaying)*/) {			
			if ((UnityEditor.Selection.activeGameObject!=null)&&(UnityEditor.Selection.activeGameObject==InitTextStyle.GlyphPrefab)) {           	
				if (TTFTextGlobalSettings.Instance.RecreateTextsWhenStylePrefabModified) {
				if (isStyleObject) {									
					RebuildAllRelevent();
					InitTextStyle.rebuildLayout=false;
					SetClean();					
				}
				else {
					RebuildText ();
					InitTextStyle.rebuildLayout=false;				
				}
				

			
				UnityEditor.Selection.activeGameObject=InitTextStyle.GlyphPrefab;	
				}
			}
		}
#endif		
#endif				
		if ((Dirty) && (AutoRebuild)) {			
		
				if (isStyleObject) {
					SaveStyleAsAsset ();
					RebuildAllRelevent ();
					InitTextStyle.rebuildLayout = false;
				    SetClean();
				} else {
					// Startcoroutine
					if (rebuildWithCoroutine) {
						RebuildText ();
					}
				
					else {
						try {
							RebuildText ();
						} 
						catch (System.Exception e) {
							if (!Application.isEditor) {
								Debug.Log ("Error during text rebuild " + e);
							}
						}
					};
					InitTextStyle.rebuildLayout = false;				
				}
		
				
			//SetClean ();
		}
		if (rebuilding!=null) {
			try {
				rebuilding.MoveNext();
			}
			catch (System.Exception e) {
				Debug.LogError(e);
				rebuilding=null;
			}
		}
	}
	
	public void OnDestroy ()
	{
		TTFTextInternal.TTFTextFontEngine.font_engines [InitTextStyle.GetUsedFontEngine ()].UnregisterClient (this);		
	}
	
	public void RebuildAll ()
	{
		foreach (TTFText tm in GameObject.FindSceneObjectsOfType(typeof(TTFText))) {			
			tm.RebuildText ();
		}
	}
	
	public void RebuildAllRelevent ()
	{
		foreach (TTFText tm in GameObject.FindSceneObjectsOfType(typeof(TTFText))) {
			if (tm.Text.Contains (gameObject.name)) {
				//tm.ResetUsedStyles();
				tm.RebuildText ();
			}
		}
	}
	
	public bool rebuildWithCoroutine = false;
	public IEnumerator rebuilding=null;
	

	
	
	private IEnumerable DoRebuildText ()
	{
		SetClean ();
		
		foreach(object o in TTFTextInternal.Engine.BuildTextASync(this)) {
			yield return o;
		}
		
		
		rebuilding = null;
		
		if (updateObjectName) {
					switch (TokenMode) {
					case TTFText.TokenModeEnum.Character:
						gameObject.name = "CharTTF " + Text;
						break;
					case TTFText.TokenModeEnum.Word:
						gameObject.name = "WordTTF " + Text;
						break;
					case TTFText.TokenModeEnum.Line:
						gameObject.name = "TTF " + Text;
						break;
					default:
						gameObject.name = "TTF " + Text;
						break;				
					}
		}
	}
	
	public void RebuildText ()
	{
		if ((rebuildWithCoroutine)
#if UNITY_EDITOR			
			&&(Application.isPlaying)
#endif			
			) {
			if (rebuilding==null) {
				rebuilding=DoRebuildText().GetEnumerator();
			}	      
		} else {
			try {
				TTFTextInternal.Engine.BuildText (this);
			}
			catch (System.Exception e) {
					Debug.Log("Error during text rebuild "+e);
			}

			if (updateObjectName) {
				switch (TokenMode) {
				case TTFText.TokenModeEnum.Character:
					gameObject.name = "CharTTF " + Text;
					break;
				case TTFText.TokenModeEnum.Word:
					gameObject.name = "WordTTF " + Text;
					break;
				case TTFText.TokenModeEnum.Line:
					gameObject.name = "TTF " + Text;
					break;
				default:
					gameObject.name = "TTF " + Text;
					break;				
				}
			}
			SetClean ();			
		}
		
	}
	
	public bool StyleNeedSaving ()
	{
		return (stylesavedtimestamp != rebuildmeshes);
	}
	
	public void SaveStyleAsAsset ()
	{
#if UNITY_EDITOR	
		if (!Application.isPlaying) {
						string d=System.IO.Path.Combine(
								  System.IO.Path.Combine(
									System.IO.Path.Combine(Application.dataPath,"Resources")
								    ,"TTFText")
							       ,"Styles"
					              );
						if (!System.IO.Directory.Exists(d)) {
							System.IO.Directory.CreateDirectory(d);
							UnityEditor.AssetDatabase.Refresh();
						}
						UnityEditor.PrefabUtility.CreatePrefab ("Assets/Resources/TTFText/Styles/"+this.name+".prefab",this.gameObject);
		}
#endif
		stylesavedtimestamp = rebuildmeshes;
	}
#endif
}
