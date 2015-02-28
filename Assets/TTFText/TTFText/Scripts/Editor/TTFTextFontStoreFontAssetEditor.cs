using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

#if ! TTFTEXT_LITE
[CustomEditor(typeof(TTFTextFontStoreFontAsset))]
public class TTFTextFontStoreFontAssetEditor : Editor {
	
	public bool showFonts = true;
	public Vector2 scrollpos;
	
	public override void OnInspectorGUI ()	{		
		TTFTextFontStoreFontAsset tfs = target as TTFTextFontStoreFontAsset;
		EditorGUILayout.LabelField("This a TTFText embedded font");		
		
		EditorGUILayout.LabelField("Font Name",tfs.font.fontid);
	}
}




#endif
