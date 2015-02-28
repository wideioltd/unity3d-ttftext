//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   

using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/Prefab Effect : Up and Down")]
public class UpAndDown : MonoBehaviour {
	
	int id;
	// Use this for initialization
	void Start () {
		id=GetComponent<TTFSubtext>().SequenceNo;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3(1, 1, Mathf.Abs(Mathf.Sin((id + Time.time) * Mathf.PI / 6)));
	}
}
