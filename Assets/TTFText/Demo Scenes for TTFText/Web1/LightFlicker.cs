using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/Light Color Changer")]
public class LightFlicker : MonoBehaviour {
	public float offset=0.5f;
	public float frequency=2f;
	public float percentOn=0.5f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
			light.intensity=Mathf.Max(0,Mathf.Sin(((offset+Time.time)/frequency*2*Mathf.PI))/2+percentOn)*100;
	}
}
