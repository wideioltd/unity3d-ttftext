using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(TTFText))]
[AddComponentMenu("Text/TTFText Animation Bridge")]
public class TTFTextAnimationBridge : MonoBehaviour {
	TTFText tt;
	
	public float size;
	public float embold;
	public float slant;
	public float extrusionDepth;
	
	// Use this for initialization
	void Start () {
		tt=GetComponent<TTFText>();
		
		embold=tt.Embold;
		slant=tt.Slant;
		size=tt.Size;
		extrusionDepth=tt.ExtrusionDepth;
	}
	
	// Update is called once per frame
	void Update () {
		tt.Embold=embold;
		tt.Slant=slant;
		tt.Size=size;
		tt.ExtrusionDepth=extrusionDepth;
	}
}
