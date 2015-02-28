#if !TTFTEXT_LITE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTF = FTSharp;



public class TTFTextFontStoreFontAsset : ScriptableObject {
	[SerializeField]
	private string _name="unnamed font";
	public string fontname {get {return _name;}}
	public TTFTextFontStoreFont font=null;
	
	public TTFTextFontStoreFontAsset Init(TTFTextFontStoreFont f) {
		font=f;
		_name=f.fontid;
		return this;
	}
}
#endif