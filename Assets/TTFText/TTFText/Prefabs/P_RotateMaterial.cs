using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Text/TTFText Prefab Effect Scripts/Rotate Materials")]
public class P_RotateMaterial : MonoBehaviour {
	public Material [] materials;
	public bool randomized=false;
	// Use this for initialization
	public void OnEnable () {
	  if (materials==null) {
			materials=new Material[1];
			materials[0]=new Material(Shader.Find("Diffuse"));
	  }
	  if (materials.Length==0) {
			materials=new Material[1];
			materials[0]=new Material(Shader.Find("Diffuse"));			
	  }
		
	  TTFSubtext tm=GetComponent<TTFSubtext>();
	  Material [] sm=renderer.sharedMaterials;
	  
	  for (int c=0;c<sm.Length;c++) {
		if (!randomized) {
			if (tm!=null) {
	  			sm[c]=materials[(tm.SequenceNo*sm.Length)%materials.Length];
			}
		}
		else {
			sm[c]=materials[Random.Range(0,materials.Length)];				
		}
	  }
	  renderer.sharedMaterials=sm;	
	}
	
}
