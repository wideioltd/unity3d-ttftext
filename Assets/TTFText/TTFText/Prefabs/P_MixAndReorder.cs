using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText Prefab Effect Scripts/Mix And Reorder")]
public class P_MixAndReorder : MonoBehaviour {
	
	public static float dist=8;
	Vector3 speeddir;
	TTFSubtext st;
	public float aspeed=0.4f;
	public float rspeed=4;
	public float speed=5;
	public float period=12;
	
	
	// Use this for initialization
	void Start () {
		speeddir=new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f));
		st=GetComponent<TTFSubtext>();		
		transform.localPosition=st.LocalSoftPosition+new Vector3(Random.Range(-dist,dist),Random.Range(-dist,dist),Random.Range(-dist,dist));
	}
	
	// Update is called once per frame
	void Update () {			
		Vector3 delta=st.LocalSoftPosition-transform.localPosition;
		
		speeddir+=new Vector3(Random.Range(-aspeed,aspeed),Random.Range(-aspeed,aspeed),Random.Range(-aspeed,aspeed))*Time.deltaTime;
		speeddir=speeddir*0.8f;
			
		delta+=speeddir.normalized*rspeed*Mathf.Min(0,Mathf.Cos(Time.time*Mathf.PI/period));
		
		float m=(speed*Time.deltaTime);
		if (delta.magnitude>=m) {
			transform.localPosition+=delta.normalized*m;
		}
		else {
			transform.localPosition+=delta;
		}
	}
}
