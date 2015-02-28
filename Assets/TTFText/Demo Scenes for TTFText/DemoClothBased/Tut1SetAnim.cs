using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/Tut1 Set Anim")]
public class Tut1SetAnim : MonoBehaviour {
#if !UNITY_FLASH	
	TTFText tm;
	TTFSubtext st;
	public AnimationCurve ac1;
	float period=0.4f;
	
	// Use this for initialization
	void Start () {
		st=GetComponent<TTFSubtext>();
		tm=transform.parent.GetComponent<TTFText>();
	}
	
	// Update is called once per frame
	void Update () {
		InteractiveCloth ic=GetComponent<InteractiveCloth>();
		float f=ac1.Evaluate((((st.SequenceNo+Time.time)/tm.Text.Length)%period)/period);
		if (((int)(Time.time/10))%tm.Text.Length==st.SequenceNo) {
			ic.pressure=Mathf.Max(0f,f);
		}
	}
#endif
}
