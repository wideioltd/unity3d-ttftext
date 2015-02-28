using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/Camera Follow Path")]
public class DemoCameraFollowPath : MonoBehaviour {
	public string targetname;
	public GameObject target;
	public float duration=10f;
	public float distance=4f;
	public Vector3 rotation=Vector3.zero;
	public Vector3 orientation=Vector3.zero;
	public bool orientedbypath=false;
	
	P_FollowPath pfp;
	float tstart;
	
	
	// Use this for initialization
	void Start () {
		tstart=Time.time;
		if (target==null) {
			target=GameObject.Find(targetname);
		}
		pfp=target.GetComponent<P_FollowPath>();
		if (pfp==null) {
			IEnumerator ie=target.transform.GetEnumerator();
			ie.MoveNext();
			target=((Transform)ie.Current).gameObject;
			pfp=target.GetComponent<P_FollowPath>();
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 t=pfp.pTween(((Time.time-tstart)%duration)/duration);
		transform.position=t+Quaternion.Euler(rotation)*(Vector3.back*distance);
		if (orientedbypath) {
		  Vector3 to=pfp.oTween(((Time.time-tstart)%duration)/duration);
		  Quaternion protation=transform.rotation;	
		  transform.LookAt(t,Quaternion.Euler(rotation)*Quaternion.Euler(orientation)*Quaternion.FromToRotation(Vector3.right,to)*Vector3.up);
		  transform.rotation=Quaternion.Slerp(protation,transform.rotation,Time.deltaTime);
		}
		else {
					  transform.LookAt(t,Quaternion.Euler(rotation)*Quaternion.Euler(orientation)*Vector3.up);
		}
	}
}
