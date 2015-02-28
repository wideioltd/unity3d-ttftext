using UnityEngine;
using System.Collections;

#if false

 This is Experimental unfinished disabled code
	
[ExecuteInEditMode]
public class TTFTextFontClient : MonoBehaviour {
	public string url = null;
	public WWWForm form=null;
	public GameObject notifiedObject;
	public string signal;
	
	WWW www=null;
	IEnumerable Start () {
		if (form!=null) {
    		www = new WWW (url,form);    
		}
		else{
			www = new WWW (url);    
		}
    	yield return www;
		notifiedObject.SendMessage(signal,www.text,SendMessageOptions.DontRequireReceiver);
		if ((Application.isEditor)&&(!Application.isPlaying)) {
			DestroyImmediate(gameObject);
		}
		else {
			Destroy(gameObject);
		}
	}	
}
#endif