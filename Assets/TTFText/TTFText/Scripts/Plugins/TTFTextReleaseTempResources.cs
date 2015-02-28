//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Text/Internal/Automatic Release of Unused Resources")]
public class TTFTextReleaseTempResources : MonoBehaviour {
	public Object mesh=null;
	public Texture texture=null;
	public Material material=null;
	
	void Start() {
		hideFlags=HideFlags.HideInInspector|HideFlags.NotEditable;
	}
	
	void OnDestroy() {
		if (mesh!=null ) {				
			if ((Application.isEditor)&&(!Application.isPlaying)) {
				Object.DestroyImmediate(mesh);
			}
			else {
			   Object.Destroy(mesh);
			}
			mesh=null;
		 }
		if (material!=null ) {				
			if ((Application.isEditor)&&(!Application.isPlaying)) {
				Object.DestroyImmediate(material,true);
			}
			else {
			   Object.Destroy(material);
			}
			material=null;
		 }		
		if (texture!=null ) {				
			if ((Application.isEditor)&&(!Application.isPlaying)) {
				Object.DestroyImmediate(texture,true);
			}
			else {
			   Object.Destroy(texture);
			}
			texture=null;
		 }
		
	}
	
}
