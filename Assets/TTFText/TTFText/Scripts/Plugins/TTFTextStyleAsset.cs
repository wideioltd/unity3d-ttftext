#if !TTFTEXT_LITE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTF = FTSharp;

// DEPRECATED SHOULD BE SAVED AS PREFAB !
public class TTFTextStyleAsset : ScriptableObject {
	[SerializeField]
	private string _name="unnamed style";
	public string stylename {get {return _name;}}
	public TTFTextStyle style=null;
	
	public TTFTextStyleAsset Init(TTFTextStyle s, string n) {
		style=s;
		_name=n;
		return this;
	}
}
#endif
