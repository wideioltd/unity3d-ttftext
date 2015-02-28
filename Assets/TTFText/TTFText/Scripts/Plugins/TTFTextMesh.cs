//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

#if !TTFTEXT_LITE
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
using TTF = FTSharp;
#endif
#endif


/// <summary>
/// TTF text mesh used to be the previous name of "TTFText components"
/// This class ensures backward compatibility..
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("Text/Internal/TTFTextMesh (Deprecated)")]
public class TTFTextMesh : MonoBehaviour {
    // DO NOT USE THIS CLASS,  IT IS COMPLETELY DEPRECATED
	// 
	// THIS IS FOR BACKWARD COMPATIBILITY ONLY


	[SerializeField]// <- Expose to animator and save
	private string fontId = "";
	[SerializeField] // <- Expose to animator and save
	private string text = "Hello";
	[SerializeField] // <- Expose to animator and save
	private float size = 1; // meters
	[SerializeField] // <- Expose to animator and save
	private float embold = 0;
	[SerializeField] // <- Expose to animator and save
	private float slant = 0;
	[SerializeField] // <- Expose to animator and save
	private float simplifyAmount = 0;
	[SerializeField]
	private float outlineStrength = 0;
	[SerializeField] // <- Expose to animator and save
	private float extrusionDepth = 0.3f;
	[SerializeField] // <- Expose to animator and save
	private float bevelForce = 0.4f;
	[SerializeField] // <- Expose to animator and save
	private float gamma = 0f;
	[SerializeField] // <- Expose to animator and save
	private float radius = 0.05f;
	[SerializeField] // <- Expose to animator and save
	private float bevelDepth = 0.1f;
	[SerializeField] // <- Expose to animator and save
	private int nbDiv = 2;
	[SerializeField] // <- Expose to animator and save
	private int numPipeEdges = 10;
	
	
	[SerializeField] // <- Expose to animator and save
	public float hspacing = 1f;
	[SerializeField] // <- Expose to animator and save
	private float lineWidth = 5f;
	
	[SerializeField]
	private  TTFText.LayoutModeEnum layoutMode =  TTFText.LayoutModeEnum.No;
	[SerializeField]
	private TTFText.ParagraphAlignmentEnum paragraphAlignment =  TTFText.ParagraphAlignmentEnum.Left;
	
	[SerializeField]
	private  TTFText.HJustEnum hJust =  TTFText.HJustEnum.Center;
	[SerializeField]
	private  TTFText.VJustEnum vJust =  TTFText.VJustEnum.Center;
	
	[SerializeField]
	private float wordSpacingFactor=0.5f;
	[SerializeField]
	private float hSpacingMultFactor=1f; // default additif
	[SerializeField]
	private  TTFText.HSpacingModeEnum hSpacingMode=  TTFText.HSpacingModeEnum.GlyphAdvance; //HSpacingModeEnum.HardBoundingBoxSpace;
	
	[SerializeField]
	private float[] extrusionSteps = {0, 1};	
	[SerializeField]
	private AnimationCurve animCurve;
	
	[SerializeField]
	private TTFText.TokenModeEnum tokenMode = TTFText.TokenModeEnum.Text;
	
	[SerializeField]
	private GameObject glyphPrefab;
	
	
	// Texture mapping
	[SerializeField]
	private bool splitSides = false;
	
	
	[SerializeField]
	private  TTFText.UVTypeEnum uvType =  TTFText.UVTypeEnum.Box;
	
	[SerializeField]
	private bool normalizeUV = false;
	
	[SerializeField]
	private Vector3 uvScaling = Vector3.one;
	
	[SerializeField]
	private int interpolationSteps=4; // When curve are interpolated, number of interpolation steps that are done

	
	public TTFText.ExtrusionModeEnum extrusionMode = TTFText.ExtrusionModeEnum.None;	
	
	public bool backFace = false;

	public bool embedCharset=false;	
	
	//
	
	//[SerializeField]
	//private  TTFText.DynamicTextRuntimeFontProviderMethodEnum dynamicTextRuntimeFontProviderMethod;

	[SerializeField]
	private  TTFText.DynamicTextRuntimeTriangulationMethodEnum dynamicTextRuntimeTriangulationMethod;
	
	[SerializeField]
	private float lineSpacingMult = 1;
	[SerializeField]
	private float lineSpacingAdd = 0;
	
	
	[SerializeField] 
	private bool autoRebuild=true;
	
	
	
    // ======================================================
	// Main parameters
	// ======================================================

	public string FontId {
        get { return fontId; }
    }
	
	public int InterpolationSteps {
        get { return interpolationSteps; }
	}
	

	public string Text   { 
		get { return text; } 
	}
		
	public float Size { 
		get { return size; } 
	}
	

	
	
	public int instanceID_ = 0;
	
	public void Start() {
		Debug.Log("Automatic Backward Compatibility : Transforming TTFTextMesh to TTF Text ");
		
		RebuildMesh();
	}
	
	
	
	
	//public  TTFText.DynamicTextRuntimeFontProviderMethodEnum DynamicTextRuntimeFontProviderMethod {
	//	get {return dynamicTextRuntimeFontProviderMethod;}
	//}
	

	
	
	public  TTFText.DynamicTextRuntimeTriangulationMethodEnum DynamicTextRuntimeTriangulationMethod {
		
		get {return dynamicTextRuntimeTriangulationMethod;}
	
	}
	
	
	
	public float Embold { get {
#if !TTFTEXT_LITE
        return embold;
#else
        return 0;
#endif
        } 
	}

	public float Slant {
        get { 
#if !TTFTEXT_LITE
            return slant;
#else
            return 0;
#endif
            } 
	}
	
	public float SimplifyAmount {
		get { 
#if !TTFTEXT_LITE
			return simplifyAmount; 
#else
            return simplifyAmount;
#endif
		} 
	}	
			
	
	public float OutlineEmbold {
		get {
			return outlineStrength;
		}
	}
	
	
	// ======================================================
	// Extrusion
	// ======================================================
	public TTFText.ExtrusionModeEnum ExtrusionMode { get { return extrusionMode; } }
	public float ExtrusionDepth { get { return extrusionDepth; } }
		
	// No Extrusion Mode
	public bool BackFace  { get { return backFace; } }
	
	// Free form extrusion
	public AnimationCurve AnimCurve { get { return animCurve; } }
	
	
	// Sinusoid
	public float BevelForce { get { return bevelForce; }  }
	public float[] ExtrusionSteps { get { return extrusionSteps; }}
	public int NbDiv { get { return nbDiv; }  }
	public float Gamma { get { return gamma; }}	
	
	// Bevel
	public float BevelDepth { get { return bevelDepth; }  }
	
	// Pipe
	public float Radius { get { return radius; }  }
	public int NumPipeEdges { get { return numPipeEdges; } }
	
	// ======================================================
	// Text Layout	
	// ======================================================
	
	public TTFText.LayoutModeEnum LayoutMode { 
		get { return layoutMode; } 
	}	
	
	public TTFText.ParagraphAlignmentEnum ParagraphAlignment { 
		get { return paragraphAlignment; } 
	}
	
	
	public TTFText.HJustEnum HJust  { 
		get { return hJust; } 
	}	
	public TTFText.VJustEnum VJust  { 
		get { return vJust; } 
	}	
	
	
	//public bool extendsWidth = false;
	public float LineWidth { 
		get { return lineWidth; } 
	}	
	
	
	public float LineSpacingMult {
		get { return lineSpacingMult; }
	}
	
	public float LineSpacingAdd {
		get { return lineSpacingAdd; }
	}
	

	
	
	public float Hspacing { 
	get { 
#if !TTFTEXT_LITE			
        return hspacing; 
#else
        return 1;
#endif
        } 
	}	

	public float WordSpacingFactor { 
		get { return wordSpacingFactor; } 
	}
	public float HSpacingMultFactor { 
		get { return hSpacingMultFactor ; } 
	}
	public TTFText.HSpacingModeEnum HSpacingMode { 
		get { return hSpacingMode ; } 
	}
	
	public TTFText.TokenModeEnum TokenMode { 
		get { return tokenMode; } 
	}
	
	public bool AutoRebuild {
		get {return autoRebuild;}
	}
	
	public GameObject GlyphPrefab { get { return glyphPrefab; } }	
	
	
	// Texture mapping	
	public bool SplitSides { get { 
#if !TTFTEXT_LITE			
        return splitSides; 
#else
        return false;
#endif
        } 
	}	
	
	public TTFText.UVTypeEnum UvType { get {
#if !TTFTEXT_LITE			
        return uvType; 
#else
        return TTFText.UVTypeEnum.Box;
#endif
        } 
	}	
	
	public bool NormalizeUV { get { 
#if !TTFTEXT_LITE			
        return normalizeUV; 
#else
        return true;
#endif
        } 
	}
	
	public Vector3 UvScaling  { get {
#if !TTFTEXT_LITE			
        return uvScaling; 
#else
        return Vector3.one;
#endif
        } 
	}
	

//	public bool EmbedCharset { 
//		get { return DynamicTextRuntimeFontProviderMethod==TTFText.DynamicTextRuntimeFontProviderMethodEnum.EmbeddedAndNetworkFonts; } 
//	} 
	// use experimental code for cross platform support ?


#if !TTFTEXT_LITE		
	
    public string SpecialChars {
        get { return additionalChar; }    
    }
    
	
	public string runtimeFontFallback ="Arial (Regular);Helvetica (Regular);Times (Regular)";
	
	[SerializeField] public string additionalChar = "";
	
	
	
#endif

	
    public Mesh mesh=null;
	public Vector3 advance=Vector3.zero;
	
	// For Editor state
	[HideInInspector]
	public bool showSystemFonts = true;
	[HideInInspector]
	public bool updateObjectName = true;
	

	// Updated and used in TTFText.BuildTextMesh function to keep track of previous token positions
	// in case we don't want to overrides them
	
	public struct TrInfo {
		public Vector3 localPosition;
		public Quaternion localRotation;
		public Vector3 localScale;
	
		public TrInfo(Transform tr) {
			localPosition = tr.localPosition;
			localRotation = tr.localRotation;
			localScale = tr.localScale;
		}
	}
	
	
	public bool SaveTokenPos=false;
	public List<TrInfo> TokenPos = new List<TrInfo>();

	
	
	
	public bool orientationReversed=false;
	public bool OrientationReversed  { get {return orientationReversed;} }


	
	void Update () {		
			RebuildMesh();
	}
	
		

	public void RebuildMesh() {
		gameObject.AddComponent<TTFText>();
		TTFText tt=gameObject.GetComponent<TTFText>();
	    // reflect all possible parameters :
		
	    PropertyInfo[] rproperties = typeof(TTFTextMesh).GetProperties();
		PropertyInfo[] wproperties = typeof(TTFText).GetProperties();
    	foreach (PropertyInfo pi in rproperties)
        	{
				try {
				PropertyInfo pi2=null;
				foreach(PropertyInfo pi3 in wproperties) {
					if (pi.Name==pi3.Name) {
						pi2=pi3;
					}
				}
				if (pi2!=null) {
					if ((pi.CanRead)&&(pi2.CanWrite)) {
						//Debug.Log(pi.Name);
            			pi2.SetValue(tt, pi.GetValue(this, null), null);
					}
				}
        			
			}
			catch (System.Exception e) {
				Debug.LogError(e);
			}
		}
		
		if ((Application.isEditor)&&(!Application.isPlaying)) {
			DestroyImmediate(this);		
		}
		else {
			Destroy(this);		
		}
	}
}
