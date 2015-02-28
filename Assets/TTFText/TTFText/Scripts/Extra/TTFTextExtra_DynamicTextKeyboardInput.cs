//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TTFText))]
[AddComponentMenu("Text/Extra/Keyboard Input")]
public class TTFTextExtra_DynamicTextKeyboardInput : MonoBehaviour {
	
	void Start () {}
	
	void Update () {

		TTFText dtm = GetComponent<TTFText>();
		
		string txt = dtm.Text;
		
		foreach (char c in Input.inputString) {
			
			if (c == '\b') {
				
				if (txt.Length != 0) {
					txt = txt.Substring(0, txt.Length - 1);
				}
			
			} else if (c == '\n' || c == '\r') {
				// do something interesting here ...
				txt = "";
				
			} else {
				txt += c;
			}
		}
		
		if (txt != dtm.Text) {
			dtm.Text = txt;
		}
	}
	
}
