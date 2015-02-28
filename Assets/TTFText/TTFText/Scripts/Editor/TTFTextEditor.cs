//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   
//define TTFTEXT_LITE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

//using System.Linq;
using TTF = FTSharp;

[CustomEditor(typeof(TTFText))]
public class TTFTextEditor : Editor
{
	static Color style_choice_color=Color.cyan;
	
	static int candidateAddOF = 0;
	
	static bool show3DObject = false;
	static bool showExtrude = false;
	static bool showTextures = false;
	static bool showLayout = false;
	static bool showAdvLayout = false;
	static bool showOutlineEffects = false;
	static bool showPlatformEngineAssoc = false;
	static bool showTTFTextSystemObjects=false;

#if !TTFTEXT_LITE	
	static bool showRuntime = false;
	static bool showRuntimeOption = false;
#endif	
	
	static Color selectedColor = new Color (1, 1, 0, 1);
	Vector2 scrollpos = Vector2.zero;
	public string[][] psfontengine_names = null;
	public int[][] psfontengine_values = null;
	public static int selectedFontEngine = 0;
	public int savetimestamp=-1;
	public string fontpat="";
	
	static string Humanize (string s)
	{
		string s2 = "";
		bool cap = true;
		foreach (char c in s) {
			if ((!cap) && (char.IsUpper (c))) {
				s2 += ' ';
			}
			if (c == '_') {
				s2 += ' ';
				cap = true;
				continue;
			}
			if (cap) {
				s2 += (char.ToUpper (c));
				cap = false;
			} else {
				s2 += c;
			}
		}
		return s2;
	}

	
	public class ButtonDelegateT
	{
		public delegate void BDT ();

		public BDT bdt;

		public ButtonDelegateT (BDT bd)
		{
			bd = bdt;
		}
	}
	
	bool Inspector (ref object parameters)
	{
		//Color dc=GUI.color;
		bool dirty = false;
		
		foreach (System.Reflection.FieldInfo fi in parameters.GetType().GetFields()) {
			try {					
				if (fi.FieldType.Name == typeof(float).Name) {
					float f0 = (float)fi.GetValue (parameters);
					float f1 = EditorGUILayout.FloatField (Humanize (fi.Name), f0);
					if (f1 != f0) {									
						dirty = true;
						fi.SetValue (parameters, f1);
					}
				}
				if (fi.FieldType.Name == typeof(int).Name) {
					int f0 = (int)fi.GetValue (parameters);
					int f1 = EditorGUILayout.IntField (Humanize (fi.Name), f0);
					if (f1 != f0) {
						dirty = true;
						fi.SetValue (parameters, f1);
					}
				}
				if (fi.FieldType.Name == typeof(string).Name) {
					string f0 = (string)fi.GetValue (parameters);
					string f1 = EditorGUILayout.TextField (Humanize (fi.Name), f0);
					if (f1 != f0) {
						dirty = true;
						fi.SetValue (parameters, f1);
					}
				}
				
				if (fi.FieldType.Name == typeof(Vector3).Name) {
					Vector3 f0 = (Vector3)fi.GetValue (parameters);							
					Vector3 f1 = EditorGUILayout.Vector3Field (Humanize (fi.Name), f0); 
					if (f1 != f0) {
						dirty = true;
						fi.SetValue (parameters, f1);
					}
				}								
				if (fi.FieldType.Name == typeof(bool).Name) {
					bool f0 = (bool)fi.GetValue (parameters);
					bool f1 = EditorGUILayout.Toggle (Humanize (fi.Name), f0);
					if (f1 != f0) {
						dirty = true;
						fi.SetValue (parameters, f1);
					}
				}																
				if (fi.FieldType.Name == typeof(ButtonDelegateT).Name) {
					ButtonDelegateT f0 = (ButtonDelegateT)fi.GetValue (parameters);
					if (GUILayout.Button (Humanize (fi.Name))) {
						f0.bdt ();
					}
				}																
				
/*				if (fi.FieldType.Name== typeof(TTFTextStyle.SerializableAnimationCurve).Name) {
					TTFTextStyle.SerializableAnimationCurve tacx=(TTFTextStyle.SerializableAnimationCurve)fi.GetValue(parameters)	;
					AnimationCurve acx=tacx.GetAnimcurve();
					UnityEngine.Keyframe [] oldkeys=acx.keys;																	
					EditorGUILayout.CurveField(fi.Name,acx);
					if (oldkeys != acx.keys) {
						tacx.SetAnimcurve(acx);
					}							
				}
				*/
			} catch (ExitGUIException eg) {
				throw (eg);
			} catch (System.Exception e) {
				Debug.Log (e);
			}
		}
		
		return dirty;		

	}
	
	static RuntimePlatform BuildTargetToRuntimePlatform (BuildTarget bt)
	{
		switch (bt) {
		case BuildTarget.Android:
			return RuntimePlatform.Android;
		case BuildTarget.FlashPlayer:
			return RuntimePlatform.FlashPlayer;
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return RuntimePlatform.WindowsPlayer;
		case BuildTarget.StandaloneOSXIntel:
			return RuntimePlatform.OSXPlayer;
		case BuildTarget.WebPlayer:
			return RuntimePlatform.WindowsWebPlayer;						
		case BuildTarget.iPhone:
			return RuntimePlatform.IPhonePlayer;			
		case BuildTarget.Wii:
			return RuntimePlatform.WiiPlayer;
		case BuildTarget.StandaloneLinux:
			return RuntimePlatform.LinuxPlayer;
		case BuildTarget.NaCl:
			return RuntimePlatform.NaCl;
			
		}
		return RuntimePlatform.WindowsEditor;
	}
	
	public void FontSelector (TTFText tm)
	{
		
		
		
		
		GUILayout.Label ("Font Selection");	
		Color defcolor = GUI.color;
		 EditorGUI.indentLevel=2;
		//if (selectedFontEngine == -1) {
		//	GUILayout.Label ("Invalid font engine selected this should not happen !");
	//		return;
	//	}
		
		/*
		 * Force Compatible ?
		 * 
		if (!TTFTextInternal.TTFTextFontEngine.font_engines[selectedFontEngine].IsCompatible(BuildTargetToRuntimePlatform(EditorUserBuildSettings.activeBuildTarget))) {
			selectedFontEngine=-1;
			for (int i=0;i<TTFTextInternal.TTFTextFontEngine.font_engines.Length;i++){
				if (TTFTextInternal.TTFTextFontEngine.font_engines[i].IsCompatible(BuildTargetToRuntimePlatform(EditorUserBuildSettings.activeBuildTarget))) {
					selectedFontEngine=i;
					break;
				}				
			}
			
		}
		*/
		
		
#if !TTFTEXT_LITE		
		 tm.InitTextStyle.useDifferentFontIdForEachFontEngine=
			EditorGUILayout.Toggle (new GUIContent("Platform specific","Use different font for each font engine"),tm.InitTextStyle.useDifferentFontIdForEachFontEngine);
		if (!tm.InitTextStyle.useDifferentFontIdForEachFontEngine) {
		  tm.InitTextStyle.EmbedFont=
			EditorGUILayout.Toggle (new GUIContent("Embed font","Force font embedding on all platform"),tm.InitTextStyle.EmbedFont);		
		}
#endif		
		
		
			if (tm.InitTextStyle.GetFontEngineParameters (selectedFontEngine) == null) {		
					selectedFontEngine = 0;
		 }
		

		
		
		
		
		
		if (tm.InitTextStyle.useDifferentFontIdForEachFontEngine) {
			EditorGUILayout.BeginHorizontal ();
			int prefered=tm.InitTextStyle.GetUsedFontEngine();
			for (int i=0; i<TTFTextInternal.TTFTextFontEngine.font_engines.Length; i++) {
				//if ( (TTFTextInternal.TTFTextFontEngine.font_engines [i].IsCompatible (BuildTargetToRuntimePlatform (EditorUserBuildSettings.activeBuildTarget)))) {
			
					if (selectedFontEngine == -1) { 
						selectedFontEngine = i;
					}
					string n = TTFTextInternal.TTFTextFontEngine.font_engines [i].GetType ().Name;
				
					if (i==prefered) {					
						GUI.tooltip="This font engine is the one being used by default";
					}
					else {
						GUI.tooltip=null; 
					}
				
				
					if (selectedFontEngine == i) {
						GUI.color = selectedColor;
					} else {
						if (tm.InitTextStyle.GetFontEngineParameters (i) == null) {
							GUI.color = Color.gray;
						} else {	
							GUI.color = defcolor;
						}
					}
					
					
					if (GUILayout.Button (n.Substring (0, n.Length - 10))) {
						selectedFontEngine = i;
					}
					GUI.color = defcolor;
				
			}
			EditorGUILayout.EndHorizontal ();
		
			
		}

		
		
		
		
		
				
		// ok the font selector is gonna be either "simple" (for one platform only), either advanced
		// we want to select font for each platform without forgetting our choices for
		// the other platforms...		
		if ((selectedFontEngine < 0)||(!tm.InitTextStyle.useDifferentFontIdForEachFontEngine))
			selectedFontEngine = 0;
		
		
		
		
		List<string> fontIDs = TTFTextInternal.TTFTextFontEngine.font_engines [selectedFontEngine].GetFontList (tm.InitTextStyle.GetFontEngineParameters (selectedFontEngine));		
		string fontid = tm.InitTextStyle.GetFontEngineFontId (selectedFontEngine);
		string prevfontid = fontid;
		if (fontid == null) {
			fontid = tm.FontId;
		}
		
		
		fontpat = EditorGUILayout.TextField ("Search : ", fontpat).ToLower();
		
		if (fontIDs != null) {
			if (fontid == null || fontid == "") {
				if (fontIDs.Count > 0) {
					fontid = fontIDs [0];
				}
			}
		
		
			{
				scrollpos = EditorGUILayout.BeginScrollView (scrollpos, false, true, GUILayout.MinHeight (150), GUILayout.MaxHeight (150));		
				for (int i = 0; i < fontIDs.Count; ++i) {
					string id = fontIDs [i];
					if ((fontpat.Length>0)&&(!id.ToLower().Contains(fontpat))) {
						continue;
					}
					if (tm.IsStyleObject) {
						if (fontid == id) {
							GUI.color = (!tm.InitTextStyle.overrideFontId) ? defcolor : style_choice_color;
						} else {
							GUI.color = defcolor;
						}					
					} else {					
						if (fontid == id) {
							GUI.color = selectedColor;
						} else {
							GUI.color = defcolor;
						}
					}
				
					if (GUILayout.Button (id)) {
						fontid = id;
					}
				
				}
		
				EditorGUILayout.EndScrollView ();
				GUI.color = defcolor;
			} 
		}
		
		
		
#if !TTFTEXT_LITE				
		EditorGUI.indentLevel += 1;

		showRuntimeOption = EditorGUILayout.Foldout (showRuntimeOption, "Font Engine Advanced Options");
		
		EditorGUI.indentLevel += 1;
		if (showRuntimeOption) {
			fontid = EditorGUILayout.TextField ("Font selector : ", fontid);
			object r = tm.InitTextStyle.GetFontEngineParameters (selectedFontEngine);
			if (r != null) {
				if (Inspector (ref r)) {
					if (tm.InitTextStyle.useDifferentFontIdForEachFontEngine) {
						tm.InitTextStyle.SetFontEngineParameters (selectedFontEngine, r);
					}
					else {
						tm.InitTextStyle.SetFontEngineParameters (-1, r);
					}
					tm.SetDirty ();
				}
			} else {
				GUI.color = Color.red;
				GUILayout.TextField ("Engine parameters not accessible... [Serialization error ? This should not occur.]");
				GUI.color = defcolor;
			}
		}
		
		
		EditorGUI.indentLevel = 0;
#endif		
		
		
		if (fontid != prevfontid) {			
			if (tm.InitTextStyle.embedFont) {
				tm.FontId=fontid;
				tm.InitTextStyle.overrideFontId = false;
			}
			else {
				tm.InitTextStyle.SetFontEngineFontId (selectedFontEngine, fontid);
				tm.InitTextStyle.overrideFontId = true;
			}
			
			tm.SetDirty ();
		}
	}
	
	public override void OnInspectorGUI ()
	{
		TTFTextLibraryInstaller.EnsureFreetype ();				
		TTFText tm = target as TTFText;
		
		Color defcolor = GUI.color;
		

		int idx;
		float f;		
		EditorGUILayout.BeginVertical ();
		
		
		
		
#if TTFTEXT_LITE
		tm.DemoMode	= EditorGUILayout.Toggle("Try Pro Features",tm.DemoMode);
#endif		
		
		if (!tm.IsStyleObject) {
			tm.updateObjectName = EditorGUILayout.Toggle ("Set Object Name", tm.updateObjectName);				
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label (new GUIContent("Text","Specify your text content here. If the Text Layout is in a mark-up compatible. Then you can use the markup to modifiy the style attributes. normal text <@style=stylename@>result<@pop@>"), GUILayout.MaxWidth (140));
			tm.Text = EditorGUILayout.TextArea (tm.Text, GUILayout.MaxWidth (240)).Replace ("\r", "");		
			EditorGUILayout.EndHorizontal ();		
			if (tm.nonFoundStyles) {
				GUI.color = Color.white;
				GUILayout.TextArea ("Some styles where not found. Click on the following button to instantiate them.");
				GUI.color=defcolor;
				if (GUILayout.Button("Create Styles")) {
					tm.autoCreateStyles=true;
					tm.SetDirty();
				}	
				else {
					tm.autoCreateStyles=false;
				}
			}
			else {
				tm.autoCreateStyles=false;
			}
		}
		
		
		
		
		EditorGUI.indentLevel += 2;
#if TTFTEXT_LITE
		if (tm.DemoMode) {
#endif 			
		if (!tm.IsStyleObject) {
			idx = EditorGUILayout.Popup (new GUIContent("One GameObject Per ","TTF Text can split the mesh into different gameobjects. Bitmap fonts and text effects will probably only work in one gameobject per character mode. "), (int)tm.TokenMode, TTFText.TokenTxt);
			if (tm.TokenMode != (TTFText.TokenModeEnum)idx) {
				tm.TokenMode = (TTFText.TokenModeEnum)idx;
			}
		}
			
#if TTFTEXT_LITE
		}
#endif 			

	
		if ((tm.IsStyleObject) || (tm.TokenMode != TTFText.TokenModeEnum.Text)) {
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideGlyphPrefab) ? style_choice_color : Color.gray;
			}				
			GameObject go = (GameObject)EditorGUILayout.ObjectField (
				new GUIContent ("Prefab", "This prefab will be used as a template for all the subobjects created."), 
				tm.GlyphPrefab,
				typeof(GameObject), false);
			
			
			
			
			if (go != tm.GlyphPrefab) {
				tm.GlyphPrefab = go;
			}

			
		  
			if (go != null) {
				EditorGUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Edit prefab parameters")) {
					Selection.activeObject = go;
					
				}
				if (GUILayout.Button ("Duplicate Prefab")) {
					GameObject ngo = (GameObject)GameObject.Instantiate (tm.GlyphPrefab);
					tm.GlyphPrefab = PrefabUtility.CreatePrefab (System.String.Format ("Assets/P_TTFText_{0}_{1}.prefab", Random.Range (0, 9999), Random.Range (0, 9999)), ngo);
					GameObject.DestroyImmediate (ngo);
					
					 
				}				
				EditorGUILayout.EndHorizontal ();
			}
		
			
			GUI.color = defcolor;
			if (go == null) {
				GUI.color = Color.white;
				if (!tm.IsStyleObject) {
					EditorGUILayout.TextArea ("Use a prefab as a template for all the subgameojects created.");
				}
				GUI.color = defcolor;
				if (GUILayout.Button (new GUIContent ("New Letter Prefab", "Create a prefab ready to be used as a template for every atom of the text."))) {
					GameObject ngo = new GameObject ();
					ngo.AddComponent<MeshFilter> ();
					ngo.AddComponent<MeshRenderer> ();
					tm.GlyphPrefab = PrefabUtility.CreatePrefab (System.String.Format ("Assets/P_TTFText_{0}_{1}.prefab", Random.Range (0, 9999), Random.Range (0, 9999)), ngo);
					GameObject.DestroyImmediate (ngo);
					
					 
				}
			} else {
				if (go.renderer == null) {
					GUI.color = Color.red;
					if (!tm.IsStyleObject) {
						GUILayout.TextArea ("The prefab does not have any renderer\n Be sure to instantiate at least a renderer and at least a meshfilter or a interactivecloth.");
					}
				}
			}
		}
		EditorGUI.indentLevel -= 2;
		
		if (tm.IsStyleObject) {
			GUI.color = (tm.InitTextStyle.overrideSize) ? style_choice_color : Color.gray;
		}
		tm.Size = EditorGUILayout.FloatField ("Font Size", tm.Size);
		GUI.color = defcolor;
		
		FontSelector (tm);		
		
		
#if TTFTEXT_LITE
		if (tm.DemoMode) {
#endif		
		
		showOutlineEffects = EditorGUILayout.Foldout (showOutlineEffects, "Outline Effects");
		EditorGUI.indentLevel += 2;
		if (showOutlineEffects) {
				
#if TTFTEXT_LITE		
		if (tm.DemoMode) {		
#endif			
#if !TTFTEXT_DELUXE		
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideSimplifyAmount) ? style_choice_color : Color.gray;
			}		
			tm.SimplifyAmount = Mathf.Max (0, EditorGUILayout.FloatField ("Simplify Outline", tm.SimplifyAmount));
#endif		
		
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideEmbold) ? style_choice_color : Color.gray;
			}		
			tm.Embold = EditorGUILayout.FloatField ("Embold", tm.Embold);
		
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideSlant) ? style_choice_color : Color.gray;
			}				
			tm.Slant = EditorGUILayout.FloatField ("Slant", tm.Slant);
	
			
			
			
			
#if TTFTEXT_LITE
	}
#endif		
				
				
				
			//if (tm.IsStyleObject) {GUI.color=(tm.InitTextStyle.overrideStackEffects)?style_choice_color:Color.gray;}	
			EditorGUILayout.BeginHorizontal ();
			candidateAddOF = EditorGUILayout.Popup (candidateAddOF, TTFTextOutline.AvailableOutlineEffectNames);
			if (GUILayout.Button ("Add Effect")) {
				TTFTextStyle.TTFTextOutlineEffectStackElement tse = new TTFTextStyle.TTFTextOutlineEffectStackElement ();
				tse.id = candidateAddOF;
				tse.parameters = TTFTextOutline.AvailableOutlineEffects [candidateAddOF].GetDefaultParameters ();
				tm.InitTextStyle.SetOutlineEffectStackElement (tm.InitTextStyle.GetOutlineEffectStackElementLength (), tse);
				tm.SetDirty ();
			}
			EditorGUILayout.EndHorizontal ();
			
			for (int i=0; i<tm.InitTextStyle.GetOutlineEffectStackElementLength(); i++) {
				TTFTextStyle.TTFTextOutlineEffectStackElement tse = tm.InitTextStyle.GetOutlineEffectStackElement (i);
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (TTFTextOutline.AvailableOutlineEffectNames [tse.id]);
				if (GUILayout.Button ("Before", GUILayout.MaxWidth (64))) {
					if (i > 0) {
						TTFTextStyle.TTFTextOutlineEffectStackElement tse2 = tm.InitTextStyle.GetOutlineEffectStackElement (i - 1);
						tm.InitTextStyle.SetOutlineEffectStackElement (i, tse2);
						tm.InitTextStyle.SetOutlineEffectStackElement (i - 1, tse);
					}
					tm.SetDirty ();
					return;
				}

				if (GUILayout.Button ("After", GUILayout.MaxWidth (64))) {
					if (i + 1 < tm.InitTextStyle.GetOutlineEffectStackElementLength ()) {
						TTFTextStyle.TTFTextOutlineEffectStackElement tse2 = tm.InitTextStyle.GetOutlineEffectStackElement (i + 1);
						tm.InitTextStyle.SetOutlineEffectStackElement (i, tse2);
						tm.InitTextStyle.SetOutlineEffectStackElement (i + 1, tse);
					}
					tm.SetDirty ();
					return;
				}
				
				if (GUILayout.Button ("Del", GUILayout.MaxWidth (64))) {
					tm.InitTextStyle.SetOutlineEffectStackElement (i, null);
					tm.SetDirty ();
					return;
				}
				
				
				EditorGUILayout.EndHorizontal ();
					
					
				if (tse.parameters == null) {
					Debug.LogWarning ("Could not find the parameters which means that serialization has failed this is to be investigated...");
					tse.parameters = TTFTextOutline.AvailableOutlineEffects [tse.id].GetDefaultParameters ();
					tm.SetDirty ();
				}
				foreach (System.Reflection.FieldInfo fi in tse.parameters.GetType().GetFields()) {
					try {					
						if (fi.FieldType.Name == typeof(float).Name) {
							float f0 = (float)fi.GetValue (tse.parameters);
							float f1 = EditorGUILayout.FloatField (fi.Name, f0);
							if (f1 != f0) {									
								fi.SetValue (tse.parameters, f1);
								tm.InitTextStyle.SetOutlineEffectStackElement (i, tse);
								tm.SetDirty ();
							}
						}
						if (fi.FieldType.Name == typeof(int).Name) {
							int f0 = (int)fi.GetValue (tse.parameters);
							int f1 = EditorGUILayout.IntField (fi.Name, f0);
							if (f1 != f0) {									
								fi.SetValue (tse.parameters, f1);
								tm.InitTextStyle.SetOutlineEffectStackElement (i, tse);								
								tm.SetDirty ();
							}
						}
						if (fi.FieldType.Name == typeof(Vector3).Name) {
							Vector3 f0 = (Vector3)fi.GetValue (tse.parameters);							
							Vector3 f1 = EditorGUILayout.Vector3Field (fi.Name, f0); 
							if (f1 != f0) {									
								fi.SetValue (tse.parameters, f1);
								tm.InitTextStyle.SetOutlineEffectStackElement (i, tse);
								tm.SetDirty ();
							}
						}								
						if (fi.FieldType.Name == typeof(bool).Name) {
							bool f0 = (bool)fi.GetValue (tse.parameters);
							bool f1 = EditorGUILayout.Toggle (fi.Name, f0);
							if (f1 != f0) {									
								fi.SetValue (tse.parameters, f1);
								tm.InitTextStyle.SetOutlineEffectStackElement (i, tse);
								tm.SetDirty ();
							}
						}																
						if (fi.FieldType.Name == typeof(TTFTextStyle.SerializableAnimationCurve).Name) {
							TTFTextStyle.SerializableAnimationCurve tacx = (TTFTextStyle.SerializableAnimationCurve)fi.GetValue (tse.parameters);
							AnimationCurve acx = tacx.GetAnimcurve ();
							UnityEngine.Keyframe [] oldkeys = acx.keys;																	
							EditorGUILayout.CurveField (fi.Name, acx);
							if (oldkeys != acx.keys) {
								tacx.SetAnimcurve (acx);
								tm.InitTextStyle.SetOutlineEffectStackElement (i, tse);								
								tm.SetDirty ();
							}							
						}
					} catch (ExitGUIException eg) {
						throw (eg);
					} catch (System.Exception e) {
						//Debug.LogWarning(fi.FieldType.Name);
						//Debug.LogWarning(fi.FieldType.GUID);
						Debug.Log (e);
					}
				}
			}
		}
		if (!tm.IsStyleObject) {
			EditorGUILayout.LabelField(System.String.Format("Complexity : {0} gameobjects,{1} vertices",tm.statistics_num_subobjects,tm.statistics_num_vertices));
		}
			
		EditorGUI.indentLevel -= 2;
#if TTFTEXT_LITE
		}
#endif		
			
		
		
		
		
		
		
		// -------------------------------------------------------------------------
		// Extrusion related options
		// -------------------------------------------------------------------------
		
#region EXTRUSION_RELATED_OPTIONS 
		GUI.color = defcolor;
		showExtrude = EditorGUILayout.Foldout (showExtrude, "Extrusion");
		EditorGUI.indentLevel += 2;
		if (showExtrude) {
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideExtrusionMode) ? style_choice_color : Color.gray;
			}				
#if TTFTEXT_LITE
			if (tm.DemoMode) {
#endif				
			
			tm.ExtrusionMode = (TTFText.ExtrusionModeEnum)EditorGUILayout.Popup ("Extrusion Mode", (int)tm.ExtrusionMode, TTFText.ExtrusionTxt);
#if TTFTEXT_LITE
			}
			else {
					string [] xat={TTFText.ExtrusionTxt[0],TTFText.ExtrusionTxt[1]};
					tm.ExtrusionMode  = (TTFText.ExtrusionModeEnum) EditorGUILayout.Popup("Extrusion Mode", (int) tm.ExtrusionMode, xat);
			}
#endif				
			
			if (tm.ExtrusionMode == TTFText.ExtrusionModeEnum.None) {
#if TTFTEXT_LITE
			if (tm.DemoMode) {
#endif				

				// No extrude
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideBackFace) ? style_choice_color : Color.gray;
				}				
				tm.BackFace = EditorGUILayout.Toggle ("Generate Backface", tm.BackFace);
#if TTFTEXT_LITE
				}
#endif				
			} else if (tm.ExtrusionMode == TTFText.ExtrusionModeEnum.Simple) {
				// Simple extrude
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideExtrusionDepth) ? style_choice_color : Color.gray;
				}				
				tm.ExtrusionDepth = EditorGUILayout.FloatField ("Depth", tm.ExtrusionDepth);
			
			} else if (tm.ExtrusionMode == TTFText.ExtrusionModeEnum.Bent) {			
				// Bent				
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideExtrusionDepth) ? style_choice_color : Color.gray;
				}				
				tm.ExtrusionDepth = EditorGUILayout.FloatField ("Depth", tm.ExtrusionDepth);
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideBevelForce) ? style_choice_color : Color.gray;
				}				
				tm.BevelForce = EditorGUILayout.FloatField ("Force", tm.BevelForce);
			
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideNbDiv) ? style_choice_color : Color.gray;
				}				
				idx = EditorGUILayout.IntField ("Steps", tm.NbDiv);
				if (idx != tm.NbDiv) {
					if (idx < 2) {
						idx = 2;
					}
					tm.NbDiv = idx;
					rebuildZs ();
				}
			
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideGamma) ? style_choice_color : Color.gray;
				}				
				f = EditorGUILayout.FloatField ("Gamma", tm.Gamma);
				if (f != tm.Gamma) {
					tm.Gamma = f;
					rebuildZs ();
				}	
			} else if (tm.ExtrusionMode == TTFText.ExtrusionModeEnum.Bevel) {
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideExtrusionDepth) ? style_choice_color : Color.gray;
				}				
				tm.ExtrusionDepth = EditorGUILayout.FloatField ("Extrusion Depth", tm.ExtrusionDepth);			
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideBevelForce) ? style_choice_color : Color.gray;
				}				
				tm.BevelForce = EditorGUILayout.FloatField ("Bevel Force", tm.BevelForce);
			
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideNbDiv) ? style_choice_color : Color.gray;
				}				
				idx = EditorGUILayout.IntField ("Steps", tm.NbDiv);
				if (idx != tm.NbDiv) {
					if (idx < 2) {
						idx = 2;
					}
					tm.NbDiv = idx;
				}
			
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideBevelDepth) ? style_choice_color : Color.gray;
				}				
				tm.BevelDepth = EditorGUILayout.Slider ("Bevel Depth %", tm.BevelDepth, 0f, 1f);
				
				
			} else if (tm.ExtrusionMode == TTFText.ExtrusionModeEnum.FreeHand) {
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideExtrusionDepth) ? style_choice_color : Color.gray;
				}				
				tm.ExtrusionDepth = EditorGUILayout.FloatField ("Extrusion Depth", tm.ExtrusionDepth);
	
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideFreeHandCurve) ? style_choice_color : Color.gray;
				}
				UnityEngine.Keyframe [] oldkeys = tm.FreeHandCurve.keys;
				AnimationCurve c = EditorGUILayout.CurveField (tm.FreeHandCurve, style_choice_color, new Rect (0, -10, 1, 20));
				if (oldkeys != tm.FreeHandCurve.keys) {
					tm.FreeHandCurve = c;
				}
				
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideBevelForce) ? style_choice_color : Color.gray;
				}				
				tm.BevelForce = EditorGUILayout.FloatField ("Coefficient", tm.BevelForce);
				
			} else if (tm.ExtrusionMode == TTFText.ExtrusionModeEnum.Pipe) {				
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideRadius) ? style_choice_color : Color.gray;
				}	
				tm.Radius = EditorGUILayout.FloatField ("Radius", tm.Radius);
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideNumPipeEdges) ? style_choice_color : Color.gray;
				}	
				tm.NumPipeEdges = EditorGUILayout.IntField ("Number of Edges", tm.NumPipeEdges);
				
			} else {
				Debug.LogWarning ("Unexpected Extrusion mode: " + tm.ExtrusionMode);
			}	
		}
		EditorGUI.indentLevel -= 2;
#endregion
		
	
		
		
		
		
		
		
		// -------------------------------------------------------------------------
		// Text Layout options
		// -------------------------------------------------------------------------
		
#region TEXT_LAYOUT_OPTIONS		
#if TTFTEXT_LITE
			if (tm.DemoMode) {
#endif				

		GUI.color = defcolor;
		showLayout = EditorGUILayout.Foldout (showLayout, "Text Layout");
		EditorGUI.indentLevel += 2;
		if (showLayout) {			
			if (!tm.IsStyleObject) {
				
				tm.LayoutMode = (TTFText.LayoutModeEnum)EditorGUILayout.Popup ("Layout", (int)tm.LayoutMode, TTFText.LayoutTxt);
			
			}
			
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideParagraphAlignement) ? style_choice_color : Color.gray;
			}	
			if (tm.LayoutMode != TTFText.LayoutModeEnum.No) {
				tm.ParagraphAlignment = (TTFText.ParagraphAlignmentEnum)EditorGUILayout.Popup ("Paragraph Alignment", (int)tm.ParagraphAlignment, TTFText.ParagraphAlignmentTxt);				
			}
			
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideWordSplitMode) ? style_choice_color : Color.gray;
			}	
			tm.WordSplitMode = (TTFText.WordSplitModeEnum)EditorGUILayout.Popup ("Word Split", (int)tm.WordSplitMode, TTFText.WordSplitModeTxt);
				
				
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideHspacing) ? style_choice_color : Color.gray;
			}	
			tm.Hspacing = EditorGUILayout.FloatField ("Character Spacing", tm.Hspacing);
			
			if (tm.LayoutMode != TTFText.LayoutModeEnum.No) {
			
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideLineWidth) ? style_choice_color : Color.gray;
				}	
				tm.LineWidth = EditorGUILayout.FloatField ("Line Width", tm.LineWidth);
#if ! TTFTEXT_DELUXE				
				if (tm.IsStyleObject) {
					GUI.color = (tm.InitTextStyle.overrideFirstLineOffset) ? style_choice_color : Color.gray;
				}				
				tm.FirstLineOffset = EditorGUILayout.FloatField ("First Line Offset", tm.FirstLineOffset);
#endif				
				{
					EditorGUILayout.BeginHorizontal ();	
					EditorGUILayout.LabelField ("Line Spacing (Mult/Add)");
					if (tm.IsStyleObject) {
						GUI.color = (tm.InitTextStyle.overrideLineSpacingMult) ? style_choice_color : Color.gray;
					}	
					tm.LineSpacingMult = EditorGUILayout.FloatField (tm.LineSpacingMult, GUILayout.MaxWidth (100));
					if (tm.IsStyleObject) {
						GUI.color = (tm.InitTextStyle.overrideLineSpacingAdd) ? style_choice_color : Color.gray;
					}	
					tm.LineSpacingAdd = EditorGUILayout.FloatField (tm.LineSpacingAdd, GUILayout.MaxWidth (100));
					EditorGUILayout.EndHorizontal ();	
					//if (GUILayout.Button("Reset")) {tm.LineSpacing = tm.fontHeight * tm.Size;}
				}
				
#if !TTFTEXT_DELUXE				
				GUI.color = defcolor;
				EditorGUI.indentLevel += 2;
				showAdvLayout = EditorGUILayout.Foldout (showAdvLayout, "Advanced Typographic Gray Control");
				if (showAdvLayout) {
					if (tm.IsStyleObject) {
						GUI.color = (tm.InitTextStyle.overrideHSpacingMode) ? style_choice_color : Color.gray;
					}	
					
					// DEPRECATED	
					//	tm.HSpacingMode = (TTFText.HSpacingModeEnum)EditorGUILayout.Popup ("Space considered", (int)tm.HSpacingMode, TTFText.HSpacingModeTxt);
					//tm.WordSpacingFactor = EditorGUILayout.FloatField("Word/Chararacter Space Blend", tm.WordSpacingFactor);
					//tm.HSpacingMultFactor = EditorGUILayout.FloatField("Multiplicate/Additive Blend", tm.HSpacingMultFactor);
					if (tm.IsStyleObject) {
						GUI.color = (tm.InitTextStyle.overrideWordSpacingFactor) ? style_choice_color : Color.gray;
					}	
					tm.WordSpacingFactor = EditorGUILayout.Slider ("Character/Word Space Blend", tm.WordSpacingFactor, 0, 1);
					if (tm.IsStyleObject) {
						GUI.color = (tm.InitTextStyle.overrideHSpacingMultFactor) ? style_choice_color : Color.gray;
					}	
					tm.HSpacingMultFactor = EditorGUILayout.Slider ("Mult/Add Blend", tm.HSpacingMultFactor, 0, 1);
				}
				EditorGUI.indentLevel -= 2;					
#endif				
			}
		}
		EditorGUI.indentLevel -= 2;
			
#if TTFTEXT_LITE
		}
#endif				
		
#endregion
		
		
		
		
		
		
		
		
		
#region TEXTURE_MAPPING_OPTIONS
		//-----------------------------------------------------------------------------------------
		// Textures Mapping
		// ----------------------------------------------------------------------------------------

#if TTFTEXT_LITE
			if (tm.DemoMode) {
#endif				

		GUI.color = defcolor;
		showTextures = EditorGUILayout.Foldout (showTextures, "Texture Mapping");
		EditorGUI.indentLevel += 2;		
		if (showTextures) {			
			//tm.splitSides;
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideSplitSides) ? style_choice_color : Color.gray;
			}	
			bool split = EditorGUILayout.Toggle ("Split Sides", tm.SplitSides);
			
			if (split != tm.SplitSides) {
				// resize meshrenderer materials array
				tm.SplitSides = split;
				
				
				if ((!tm.IsStyleObject) && (tm.TokenMode == TTFText.TokenModeEnum.Text)) {
				
					MeshRenderer mr = tm.GetComponent<MeshRenderer> ();

					if (mr != null) {
						Material[] omats = mr.sharedMaterials; // old array
						Material def = omats.Length > 0 ? omats [0] : null;
				
						Material[] nmats; // new array
				
						if (split) {
					
							nmats = new Material[3];
					
							nmats [0] = omats.Length >= 1 ? omats [0] : def;
							nmats [1] = omats.Length >= 2 ? omats [1] : def;
							nmats [2] = omats.Length >= 3 ? omats [2] : def;
					
						} else {
					
							nmats = new Material[1];
							nmats [0] = def;
						}
						mr.sharedMaterials = nmats;

					}	
						
				}
			}
			
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideUVType) ? style_choice_color : Color.gray;
			}	
			idx = EditorGUILayout.Popup ("UV Mapping", (int)tm.UvType, TTFText.UVTypeTxt);
			if (tm.UvType != (TTFText.UVTypeEnum)idx) {
				tm.UvType = (TTFText.UVTypeEnum)idx;
			}	
			
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideNormalizeUV) ? style_choice_color : Color.gray;
			}	
			if (tm.UvType == TTFText.UVTypeEnum.Box) {
				tm.NormalizeUV = EditorGUILayout.Toggle ("Normalize", tm.NormalizeUV);
			}
			
			if (tm.IsStyleObject) {
				GUI.color = (tm.InitTextStyle.overrideUVScaling) ? style_choice_color : Color.gray;
			}	
			tm.UvScaling = EditorGUILayout.Vector3Field ("UV Scaling Factor", tm.UvScaling);
		
			if (tm.TokenMode!=TTFText.TokenModeEnum.Text) {
				if (tm.IsStyleObject) {GUI.color=(tm.InitTextStyle.overrideMaterialOffset)?style_choice_color:Color.gray;}	
		    	tm.MaterialOffset = 
						EditorGUILayout.IntField(
							"Material Index Offset",
							tm.MaterialOffset);
			}
		}
		EditorGUI.indentLevel -= 2;
			
#if TTFTEXT_LITE
		}
#endif				

		
#endregion
		
		

#region S3D_OBJECT_OPTIONS
		// -------------------------------------------------------------------------
		// 3D Object related options
		// -------------------------------------------------------------------------

		if (!tm.IsStyleObject) {
			show3DObject = EditorGUILayout.Foldout (show3DObject, "3D Object");
			EditorGUI.indentLevel += 2;			
		
			if (show3DObject) {
				tm.HJust = (TTFText.HJustEnum)EditorGUILayout.Popup ("Horizontal Attachment", (int)tm.HJust, TTFText.HJustTxt);			
				tm.VJust = (TTFText.VJustEnum)EditorGUILayout.Popup ("Vertical Attachment", (int)tm.VJust, TTFText.VJustTxt);			
			}
			EditorGUI.indentLevel -= 2;
			
		}
#endregion




#if !TTFTEXT_LITE
		// -----------------------------------------------
		// Other Advanced Option
		// -----------------------------------------------
		if (!tm.IsStyleObject) {
			showRuntime = EditorGUILayout.Foldout (showRuntime, "Advanced Options");
			
			
			
			
			if (showRuntime) {			
				EditorGUI.indentLevel += 2;			
				tm.DynamicTextRuntimeTriangulationMethod = (TTFText.DynamicTextRuntimeTriangulationMethodEnum)EditorGUILayout.Popup ("Prefered Triangulation", (int)tm.DynamicTextRuntimeTriangulationMethod, TTFText.DynamicTextRuntimeTriangulationMethodTxt);			
				TTFTextGlobalSettings.Instance.EasyDeployement = EditorGUILayout.Toggle (new GUIContent("Easy Deployement","Automatically copy all local fonts to the build directory"),TTFTextGlobalSettings.Instance.EasyDeployement);
				bool pshow=showTTFTextSystemObjects;
				tm.rebuildWithCoroutine=EditorGUILayout.Toggle("Coroutine based rendering",
					tm.rebuildWithCoroutine);
				tm.AutoRebuild = EditorGUILayout.Toggle ("Automatic Rebuilds", tm.AutoRebuild);				
				TTFTextGlobalSettings.Instance.RecreateTextsWhenStylePrefabModified = EditorGUILayout.Toggle (new GUIContent("Style Prefab Edit Mode","Should the text be invalidated and recreated when the style prefab is selected in the editor"),TTFTextGlobalSettings.Instance.RecreateTextsWhenStylePrefabModified );
				showTTFTextSystemObjects=EditorGUILayout.Toggle("Show System Objects",
					showTTFTextSystemObjects);
				if (showTTFTextSystemObjects!=pshow) {
					TTFTextGlobalSettings.Instance.ShowTTFTextObjects=showTTFTextSystemObjects;					
					GameObject gs=(GameObject)GameObject.Find("/TTFText Styles");
					GameObject gf=(GameObject)GameObject.Find("/TTFText Font Store");
					if (showTTFTextSystemObjects ) {						
						if (TTFTextFontStore.Instance==null) {
							Debug.LogWarning("(TTF Text)Fail invoking TTF Text Fontstore");
						}
						if (gs!=null) {							
							gs.hideFlags=0;							
							EditorUtility.SetDirty(gs);
						}
						if (gf!=null) {
							gf.hideFlags=0;							
							EditorUtility.SetDirty(gf);
						}						
					}
					else {
						
						if (gs!=null) {
							gs.hideFlags=HideFlags.HideInHierarchy|HideFlags.NotEditable;							
							EditorUtility.SetDirty(gs);
						}
						if (gf!=null) {
							//Debug.Log("Hiding font store");
							gf.hideFlags=HideFlags.HideInHierarchy|HideFlags.NotEditable;							
							EditorUtility.SetDirty(gf);
						}												
					}
				}
				
				//GUI.color=Color.gray;
#if !TTFTEXT_DELUXE			
				if (!tm.IsStyleObject) {
					tm.SaveTokenPos = EditorGUILayout.Toggle (new GUIContent("Save Subobjects Positions","Save Subobjects Positions"), tm.SaveTokenPos);
				}
#endif			
				
				
				// =================================
				showPlatformEngineAssoc = EditorGUILayout.Foldout (showPlatformEngineAssoc, "Engine Selection");
		
				bool [] enabled_engines = new bool[TTFTextInternal.TTFTextFontEngine.font_engines.Length];	
		
				
				enabled_engines [0] = true;						
				for (int i=1; i<TTFTextInternal.TTFTextFontEngine.font_engines.Length; i++) {
					bool b = tm.InitTextStyle.GetFontEngineParameters (i) != null;
					enabled_engines [i] = b;					
					
					
					//if (b) {
					//				tm.InitTextStyle.SetFontEngineParameters(i,TTFTextInternal.TTFTextFontEngine.font_engines[i].GetType().GetNestedType("Parameters").InvokeMember("Parameters",System.Reflection.BindingFlags.CreateInstance,null,null,null));
					//				tm.InitTextStyle.SetFontEngineFontId(i,tm.InitTextStyle.GetFontEngineFontId(0));
					//					TTFTextInternal.TTFTextFontEngine.font_engines[i].RegisterClient(tm);												
					//}
				}			
				
			
				if (showPlatformEngineAssoc) {
				
					//EditorGUI.indentLevel+=2;		
					/*
		for (int i=1;
			i<TTFTextInternal.TTFTextFontEngine.font_engines.Length;
			i++) 
					{
			
			string n=TTFTextInternal.TTFTextFontEngine.font_engines[i].GetType().Name;			
			bool b=tm.InitTextStyle.GetFontEngineParameters(i)!=null;
			bool tb;
			tb=//enable||
			GUILayout.Toggle(b,"Enable "+n.Substring(0,n.Length-12)+ " Font Engine");								
			enabled_engines[i]=tb;					
						
						
						
			if ((tb!=b)
			//||(enable)
			)
			{
				if ((!tb)
				//&&(!enable)
				) {
							TTFTextInternal.TTFTextFontEngine.font_engines[i].UnregisterClient(tm);														
							tm.InitTextStyle.SetFontEngineParameters(i,null);
							tm.InitTextStyle.SetFontEngineFontId(i,null);							
				}
				else  {
							tm.InitTextStyle.SetFontEngineParameters(i,TTFTextInternal.TTFTextFontEngine.font_engines[i].GetType().GetNestedType("Parameters").InvokeMember("Parameters",System.Reflection.BindingFlags.CreateInstance,null,null,null));
							tm.InitTextStyle.SetFontEngineFontId(i,tm.InitTextStyle.GetFontEngineFontId(0));
							TTFTextInternal.TTFTextFontEngine.font_engines[i].RegisterClient(tm);							
				}
			}
			
		}			
			*/
					EditorGUI.indentLevel += 2;	
					if (psfontengine_values == null) {
				
						int L = typeof(RuntimePlatform).GetMembers (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Length;
						psfontengine_names = new  string [L][];
						psfontengine_values = new  int [L][];
				
						for (int l=0; l<L; l++) {
							int cp = 0;
							for (int ci=0; ci<TTFTextInternal.TTFTextFontEngine.font_engines.Length; ci++) {
								if (TTFTextInternal.TTFTextFontEngine.font_engines [ci].IsCompatible ((RuntimePlatform)l)) {
									cp++;
								}
							}
							psfontengine_names [l] = new string[cp];
							psfontengine_values [l] = new int[cp];
							cp = 0;
							for (int ci=0; ci<TTFTextInternal.TTFTextFontEngine.font_engines.Length; ci++) {
								if (TTFTextInternal.TTFTextFontEngine.font_engines [ci].IsCompatible ((RuntimePlatform)l)) {
									psfontengine_names [l] [cp] = TTFTextInternal.TTFTextFontEngine.font_engines [ci].GetType ().Name;		
									psfontengine_names [l] [cp] = (psfontengine_names [l] [cp]).Substring (0, (psfontengine_names [l] [cp].Length - 10));
									psfontengine_values [l] [cp] = ci;		
									cp++;
								}						
							}
						}

					}
			
			
			
					int ti = 0;
					foreach (System.Reflection.MemberInfo mi in typeof(RuntimePlatform).GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) {
						int [] ta = psfontengine_values [ti];
						int cix = System.Array.IndexOf (ta, tm.InitTextStyle.PreferredEngine((RuntimePlatform)ti));
						if (cix == -1) {
							cix = 0;
						}
					
						if (!enabled_engines [tm.InitTextStyle.PreferredEngine((RuntimePlatform)ti)]) {
							GUI.color = Color.red;
						}
						
						int nix = 0;
						nix = UnityEditor.EditorGUILayout.Popup (mi.Name/*+"/"+ta.Length*/, cix, psfontengine_names [ti]);
						
						
						if (nix != cix) {
							tm.InitTextStyle.SetPreferedEngine((RuntimePlatform)ti,ta [nix]);
							tm.SetDirty ();
						}
						GUI.color = defcolor;
						ti += 1;
					}
					EditorGUI.indentLevel -= 2;					
				}
		
				//=============================================								
				EditorGUI.indentLevel -= 2;

			
			}	
						
			//if (tm.IsStyleObject) {
			
			if (!tm.AutoRebuild) {
				if (GUILayout.Button ("Rebuild Now")) {
					tm.RebuildText ();
				}
			}
			//}

		} else {
			if (GUILayout.Button ("Rebuild Now")) {
				tm.RebuildAllRelevent ();
			}			
		}
			
		
		GUI.color = defcolor;
		
#endif // TTFLITE
		
		if (!tm.IsStyleObject) {
			EditorGUILayout.BeginHorizontal ();
			if (tm.TokenMode == TTFText.TokenModeEnum.Text) {
				if (GUILayout.Button ("Save Mesh")) {
					SaveMeshAs ();
				}
			}
			if (GUILayout.Button ("Add Font")) {
				TTFTextLibraryInstaller.ImportFont ();
			}		
			EditorGUILayout.EndHorizontal ();
		} else {
#if !TTFTEXT_LITE
			if (tm.StyleNeedSaving()) {
				GUI.color = Color.cyan;
			}
			else {
				GUI.color=defcolor;
			}
			
			
			if (GUILayout.Button("Save As An Asset")) {
				tm.SaveStyleAsAsset();
			}
			
			
			
			GUI.color=defcolor;
			if (GUILayout.Button ("Reset Style")) {
				tm.InitTextStyle.ResetStyle ();
				tm.SetDirty ();
				tm.RebuildAllRelevent ();
				tm.SetClean ();
			}
#endif			
		}
			
		
		EditorGUILayout.EndVertical ();
		
		
		if (tm.isDirty ()) {
#if TTFTEXT_LITE			
			if (tm.IsStyleObject) {
				foreach (TTFText ttm in GameObject.FindSceneObjectsOfType(typeof(TTFText))){
					if (ttm.Text.Contains(tm.gameObject.name)){
									TTFTextInternal.Engine.BuildText(ttm);
					}
				}
				
			}else {
				TTFTextInternal.Engine.BuildText(tm);
				if (tm.updateObjectName) {
					tm.gameObject.name="TTF " + tm.Text;
				}
				
			
			}
			tm.SetClean();
#endif
			EditorUtility.SetDirty (target);			
			
		}
		
		GUI.color = defcolor;
		
		
#if TTFTEXT_LITE		
		if (GUILayout.Button("Upgrade TTF Text")) {
		 Application.OpenURL("http://ttftext.computerdreams.org/download");
		  // Application.OpenURL("http://u3d.as/content/computer-dreams/ttf-text/2KN");
		  //  UnityEditorInternal.AssetStore.Open("computer-dreams/ttf-text/2KN");
		}
#endif		
	}
	
	void rebuildZs ()
	{

		try {
			//bended mode
		
			TTFText tm = target as TTFText;
			int nbDiv = tm.NbDiv;
			tm.ExtrusionSteps = new float[nbDiv];
			float cz = 0; 
			float deltaZ = 0;
		
			for (int i=1; i<(nbDiv+1); i++) {
				deltaZ = Mathf.Pow (Mathf.Sin (i * Mathf.PI / (nbDiv + 1)), tm.Gamma);
				tm.ExtrusionSteps [i - 1] = cz;
				cz += deltaZ;
			}
		
			cz -= deltaZ;
			if (cz != 0) {
				for (int i=0; i<nbDiv; i++) {
					tm.ExtrusionSteps [i] /= cz;
				}
			}
			
		} catch (System.Exception ex) {
			
			Debug.LogError ("(TTFText) Error:" + ex.ToString ());
		}

	}

	void SaveMeshAs ()
	{
		
		TTFText tm = target as TTFText;
		GameObject go = tm.gameObject;
		
		Mesh smesh = tm.mesh;
		if (smesh == null) {
			return;
		}
		
		// Copy Mesh
		Mesh mesh = new Mesh ();
		mesh.vertices = smesh.vertices;
		mesh.uv = smesh.uv;
		mesh.triangles = smesh.triangles;
		mesh.RecalculateNormals ();
		
		string path = EditorUtility.SaveFilePanelInProject ("Save Text Mesh", go.name + ".asset", "asset", "bla");
		
		if (path == "") {
			return;
		}
		
		AssetDatabase.CreateAsset (mesh, path);
	}
	
	[MenuItem("GameObject/Create Other/TTF Text")]
	static void NewTTFText ()
	{
		
		if (!TTFTextLibraryInstaller.EnsureFreetype ()) {
			Debug.LogError ("(TTFText) Native FreeType library '" + TTF.FT.FT_DLL + "' cannot be found.");
			return;
		}
		
		GameObject go = new GameObject ();
		go.AddComponent<MeshFilter> ();
		go.AddComponent<MeshRenderer> ();
		go.AddComponent<TTFText> ();
		
		TTFText tm = go.GetComponent<TTFText> ();
		tm.updateObjectName=true;
		tm.Text = "Hello";
		
		
#if TTFTEXT_LITE
		{
			TTFTextInternal.Engine.BuildText(tm);
			tm.SetClean();
		}
#endif
		
		Selection.activeObject = go;
	}
	
}
