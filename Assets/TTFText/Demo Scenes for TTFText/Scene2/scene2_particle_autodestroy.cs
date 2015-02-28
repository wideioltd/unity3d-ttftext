using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/Particle Autodestroy")]
public class scene2_particle_autodestroy : MonoBehaviour {
	public float lifetime=60;
	public float tstart;
	// Use this for initialization
	void Start () {
		tstart=Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		//Camera.main.WorldToScreenPoint(transform.position);
		if (((Time.time-tstart)>lifetime)||(!renderer.isVisible)) {
			GameObject.Destroy(gameObject);
		}
	}
}
