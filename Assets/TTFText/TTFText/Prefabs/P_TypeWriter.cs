using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("Text/TTFText Prefab Effect Scripts/Typewriter")]
public class P_TypeWriter : MonoBehaviour {
	TTFSubtext pt;
	public float minDelay=0.2f;
	public float maxDelay=2;
	
	
	// Use this for initialization
	void Start () {
		gameObject.transform.localScale=Vector3.zero;
		pt=GetComponent<TTFSubtext>();
		if (pt.SequenceNo==0) {
			StartCoroutine("DisplayLetter");
		}
	}
	
	public IEnumerator DisplayLetter() {
		float t=Random.Range(minDelay,maxDelay);
		yield return new WaitForSeconds(t);
		transform.localScale=Vector3.one;
				
			
		
		foreach (Transform st in transform.parent) {
			TTFSubtext apt=st.GetComponent<TTFSubtext>();
			if (apt!=null) {
				// if token mode = character
				// check line no (if line no different also play)...
				// carriage return ...
				
				
				if (apt.SequenceNo==(pt.SequenceNo+1)) {		
					if (apt.LineNo!=pt.LineNo) {
						apt.SendMessage("NewLine");
					}
					apt.SendMessage("DisplayLetter");
				}
			}
		}
	}
}
