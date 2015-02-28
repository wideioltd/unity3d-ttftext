using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText Prefab Effect Scripts/Event/OnDisplayLetter/From Camera Position")]
public class DisplayLetter_CameraToPos : MonoBehaviour {
	TTFSubtext st;
	public float speed=5;
	
	public Vector3 viewportStartPosition=new Vector3(0.5f,0.5f,0);
	public Vector3 viewportRandomNoise=new Vector3(0,0,0);
	public AnimationCurve speedEvolution=AnimationCurve.Linear(0,1,1,1);
	public Vector3 startRotation=new Vector3(0,360,0);
	public Vector3 rotationNoise=new Vector3(0,360,0);
    
	
	bool started=false;
	float odelta=0;
	Vector3 initlocalr;
	
	// Use this for initialization
	void Start () {
		st=GetComponent<TTFSubtext>();
	}
	
	// Update is called once per frame
	void Update () {
		if (started) {
		Vector3 delta=st.LocalSoftPosition-transform.localPosition;
		float pcd=delta.magnitude/odelta;
		float m=(speedEvolution.Evaluate(pcd)*speed*Time.deltaTime);
			
		if (delta.magnitude<m) {
			transform.localPosition=st.LocalSoftPosition;
			Destroy(this);
		}
		transform.localPosition+=delta.normalized*m;
		transform.localRotation=Quaternion.Euler(initlocalr*pcd);
		}
	}
	
	public void DisplayLetter() {
		started=true;
		st=GetComponent<TTFSubtext>();
		//transform.position=Camera.main.transform.position;
		transform.position=Camera.main.ViewportToWorldPoint(viewportStartPosition)+new Vector3(Random.Range(-viewportRandomNoise.x,viewportRandomNoise.x),Random.Range(-viewportRandomNoise.y,viewportRandomNoise.y),Random.Range(-viewportRandomNoise.z,viewportRandomNoise.z));
		initlocalr=startRotation+new Vector3(Random.Range(-rotationNoise.x,rotationNoise.x),Random.Range(-rotationNoise.y,rotationNoise.y),Random.Range(-rotationNoise.z,rotationNoise.z));
		transform.localRotation=Quaternion.Euler(initlocalr);
		odelta=(st.LocalSoftPosition-transform.localPosition).magnitude+0.00001f;
	}
}
