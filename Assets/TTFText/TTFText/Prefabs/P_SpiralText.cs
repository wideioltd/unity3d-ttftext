using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(TTFSubtext))]
[AddComponentMenu("Text/TTFText Prefab Effect Scripts/Spiral Text (Deprecated)")]
public class P_SpiralText : MonoBehaviour {
	public float af=10;
	public float rf=0.4f;
	// Use this for initialization
	void Start () {
		Debug.LogWarning("Deprecated - use follow path instead");
		TTFSubtext subtext=GetComponent<TTFSubtext>();
		//float x=subtext.Advance.magnitude;
		float x=subtext.SequenceNo;//subtext.LocalSoftPosition.x;
		float alpha=Mathf.Sqrt(Mathf.Abs(af*x));
		float r=rf*x;
		transform.localPosition=new Vector3(Mathf.Cos(alpha)*r,Mathf.Sin(alpha)*r,transform.localPosition.z);
		transform.localRotation=Quaternion.Euler(alpha,0,0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
