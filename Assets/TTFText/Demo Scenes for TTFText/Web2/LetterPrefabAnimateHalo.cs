using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/Prefab effect : Flare Following Outline")]
public class LetterPrefabAnimateHalo : MonoBehaviour {
	public float period=1f;
	TTFText ttm;
	TTFSubtext tts;
	TTFTextOutline.Boundary b;
	TTFTextOutline.BoundaryUniformTraverser but;
	Transform flare,flare2,flare3;
	Vector3 sz2;
	
	// Use this for initialization
	void Start () {
		ttm=transform.parent.GetComponent<TTFText>();
		tts=GetComponent<TTFSubtext>();
		
		TTFTextOutline o=TTFTextInternal.Engine.MakeOutline(tts.Text,
			ttm.Hspacing,ttm.Embold,ttm);
		foreach(TTFTextOutline.Boundary bn in o.boundaries) {
			b=bn;
			break;
		}
		sz2=o.GetSize()/2;
		if (b!=null) {
			but=b.GetUniformTraverser();
			flare=transform.FindChild("Flare");
			flare2=transform.FindChild("Flare2");
			flare3=transform.FindChild("Flare3");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (but!=null) {
		Vector3 v= but.GetPositionAt(Time.time/period)-sz2;
		flare.localPosition=sz2+v;
		flare2.localPosition=sz2+(v*0.7f);
		flare3.localPosition=sz2+(v*1.2f);
		}
	}
}
