//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   
//define TTFTEXT_LITE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

#if ! TTFTEXT_LITE
[CustomEditor(typeof(TTFTextFontStore))]
public class TTFTextFontStoreEditor : Editor {
	
	public bool showFonts = true;
	public bool showClients = true;
	public bool showControl=false;
	public bool showOptions =false;
	public bool showSystemFonts=true;
	public Vector2 scrollpos;
	
	public override void OnInspectorGUI ()	{
		
		TTFTextFontStore tfs = target as TTFTextFontStore;
		
		if (tfs==null) {
			GUI.color=Color.red;
			EditorGUILayout.LabelField("Cannot find the component");
			return;
		}
		
		tfs.dontDestroyOnLoad=EditorGUILayout.Toggle("Don't Destroy On Scene Change", tfs.dontDestroyOnLoad);	
		
		//tfs.destroyWhenUnused=EditorGUILayout.Toggle("Destroy Font Store When Unused", tfs.destroyWhenUnused);	

		
		showFonts = EditorGUILayout.Foldout(showFonts, "Embedded Fonts");
		
		if (showFonts) {
			EditorGUI.indentLevel+=2;
			int i = 0;
			
			if (tfs.embeddedFonts != null && tfs.embeddedFonts.Count != 0) {

				
				
				string d=System.IO.Path.Combine(
							System.IO.Path.Combine(Application.dataPath,"Resources"),
							System.IO.Path.Combine("TTFText","Fonts"));
				
				string fonttoberemoved=null;
				
				foreach (TTFTextFontStoreFont f in tfs.embeddedFonts) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(System.String.Format("{0} - {1} [{2}] {3}kb",i, f.fontid,f.GetRefCount(),f.BoundaryMemoryUsage()/1024));
					/*
					if (GUILayout.Button("Save As Asset")) {
						
						if (System.IO.Directory.Exists(d)) {
							System.IO.Directory.CreateDirectory(d);
							UnityEditor.AssetDatabase.Refresh();
						}
						f.BuildCharSet(f.fontid);
						UnityEditor.AssetDatabase.CreateAsset(TTFTextFontStoreFontAsset.CreateInstance<TTFTextFontStoreFontAsset>().Init(f),"Assets/Resources/TTFText/Fonts/"+f.fontid+".asset");
					}
					*/
					if (System.IO.File.Exists(d+"/" +f.fontid+".asset")) {
						if (GUILayout.Button("Delete Asset")) {
							//Debug.Log(d +"/"+f.fontid+".asset");
							//AssetDatabase.DeleteAsset(d+"/" +f.fontid+".asset");
							fonttoberemoved=f.fontid;
						}
					}
					EditorGUILayout.EndHorizontal();
					++i;

				}
				
				if (fonttoberemoved!=null) {				
							System.IO.File.Delete(d+"/" +fonttoberemoved+".asset");
							if (System.IO.File.Exists(d+"/" +fonttoberemoved+".asset.meta")) {
								System.IO.File.Delete(d+"/" +fonttoberemoved+".asset.meta");
							}	
							
							tfs.RemoveFont(fonttoberemoved);
							AssetDatabase.Refresh();
				}
			
			} else {
				
				//Color sc = GUI.color ;
				//GUI.color = Color.red;
				EditorGUILayout.LabelField("No font is currently embedded");
				//GUI.color = sc;
			}
			EditorGUI.indentLevel-=2;
		}
		

		showClients = EditorGUILayout.Foldout(showClients, "Clients");
		if (showClients) {					
			EditorGUI.indentLevel+=2;
			int i =0;
			foreach (TTFText tm in tfs.Clients) {
				EditorGUILayout.LabelField(System.String.Format("{0} - {1}",i, tm.gameObject.name));
				i++;
			}
			EditorGUI.indentLevel-=2;
		}
		

		showOptions = EditorGUILayout.Foldout(showOptions, "Shared Options");
		if (showOptions) {
			EditorGUI.indentLevel+=2;
			int pi=TTFTextFontStore.Instance.defaultInterpolationSteps;
			string das=TTFTextFontStore.Instance.defaultAdditionalCharacters;
			TTFTextFontStore.Instance.defaultInterpolationSteps=EditorGUILayout.IntField("Interpolation Steps",TTFTextFontStore.Instance.defaultInterpolationSteps);	
			if (TTFTextFontStore.Instance.defaultInterpolationSteps<1) TTFTextFontStore.Instance.defaultInterpolationSteps=1;			
			if (TTFTextFontStore.Instance.defaultInterpolationSteps>10) TTFTextFontStore.Instance.defaultInterpolationSteps=10;
			TTFTextFontStore.Instance.defaultAdditionalCharacters=EditorGUILayout.TextField("Additional Characters",TTFTextFontStore.Instance.defaultAdditionalCharacters);	
			if ((pi!=TTFTextFontStore.Instance.defaultInterpolationSteps) || (TTFTextFontStore.Instance.defaultAdditionalCharacters!=das)) {
				TTFTextFontStore.Instance.RebuildAllCharsets();	
			}
			EditorGUI.indentLevel-=2;
		}
		
		showControl = EditorGUILayout.Foldout(showControl, "Advanced FontStore Control");
		if (showControl) {
			if (GUILayout.Button(new GUIContent("Embed All Project Fonts","Embed all the fonts that are contained in the project folder in the application"))) {
				List<string> tl=new List<string>();
				foreach(var f in TTFTextFontListManager.Instance.LocalFonts) {
					tl.Add(f.Key);
				}
				foreach(string f in tl) {
					tfs.EnsureFont(f);
				}
				AssetDatabase.Refresh();
			}
			
			if (GUILayout.Button("Reset FontStore")) {
				tfs.ResetFontStore();
			}
			
			GUILayout.Label("Add specific fonts to the font store");	
			TTFTextFontListManager flm = TTFTextFontListManager.Instance;
		
			Color selectedColor = new Color (1, 1, 0, 1);		
			Color selectedColor2 = new Color (0, 1, 0, 1);		
			
			Color defcolor = GUI.color;
		
			List<string> fontIDs = new List<string>(flm.LocalFonts.Keys);

			if (showSystemFonts) {
				fontIDs.AddRange(flm.SystemFonts.Keys);
			}
			

		
		
			{
			
				scrollpos = GUILayout.BeginScrollView(scrollpos,
													  false, 
					                                  true, 
					                                  GUILayout.MinHeight(150),
					                                  GUILayout.MaxHeight(150));
		
				for (int i = 0; i < fontIDs.Count; ++i) {
				string id = fontIDs[i];
				
			
		 		//tfs.embeddedFonts			 * 
				TTFTextFontStoreFont cf=tfs.GetEmbeddedFont(id);
				if (cf!=null) {
					if (cf.refcount==0) {
						GUI.color = selectedColor;
					}
					else {
						GUI.color = selectedColor2;							
					}
				} else {
					GUI.color = defcolor;
				}
				
				
				TTFontInfo finfo = flm.GetFontInfo(id);
				
				if (finfo != null) {
					if (GUILayout.Button(finfo.Name)) {
						if ((cf==null)) {
							cf=tfs.EnsureFont(id);		
							cf.BuildCharSet(id);
						}
						else {
							if (cf.refcount==0) {
								tfs.embeddedFonts.Remove(cf);
							}
						}
						//f.incref();
					}
				}
				
				}
		
				GUILayout.EndScrollView ();
				GUI.color = defcolor;
			} 
		
					
			
		}
	}
}



#region Experimental Network Fonts
#if false
		showNetwork = EditorGUILayout.Foldout(showNetwork, "Network");
		if (showNetwork) {					
			tfs.NetworkFontEnabled=EditorGUILayout.Toggle("Enable Network Fonts", tfs.NetworkFontEnabled);	
			if (tfs.NetworkFontEnabled) {
				tfs.TTFTextServerUrl=EditorGUILayout.TextField("URL",tfs.TTFTextServerUrl);
				tfs.TTFTextServerUserKey=EditorGUILayout.TextField("User",tfs.TTFTextServerUserKey);
				tfs.TTFTextServerAuthKey=EditorGUILayout.TextField("Key",tfs.TTFTextServerAuthKey);
			}
			
			if (GUILayout.Button("Update Font List")) {
				tfs.UpdateFontList();
			}
		}
#endif
#endregion

#endif