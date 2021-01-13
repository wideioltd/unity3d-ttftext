using UnityEngine;
using System.Collections;

public class MobileTest01 : MonoBehaviour {
	
	public GameObject textobject;
	public TTFText text;
	public float stime=0;
	
	
	delegate void SwitchModeD();
	int cmode=0;
	SwitchModeD [] switchmodes;
	public string extrachar;
	
/*	
	public string extrachar=System.Text.UTF32Encoding.UTF32.GetString(
		System.Text.UTF8Encoding.UTF8.GetBytes(
		"基本的に découvrir sur elle-même une vision inaccessible"
		)
		);
		 */
	// Use this for initialization
	void Start () {
		textobject=new GameObject();
		textobject.AddComponent<MeshFilter>();		
		textobject.AddComponent<MeshRenderer>();		
		textobject.AddComponent<TTFText>();		
		textobject.GetComponent<Renderer>().material=new Material(Shader.Find("Diffuse"));
		textobject.GetComponent<Renderer>().material.color=Color.red;	
		
		text=textobject.GetComponent<TTFText>();
		text.TokenMode=TTFText.TokenModeEnum.Character;
//#if UNITY_ANDROID
	
		text.SetPreferedEngine(RuntimePlatform.Android,2);
	// text.SetPreferedEngine(RuntimePlatform.Android,3);
//#endif		
		stime=Time.time;
		switchmodes = new SwitchModeD [] {
			Mode0,
			Mode1,
			Mode2,
			Mode3,
			Mode4,
			Mode5
#if UNITY_ANDROID || UNITY_IPHONE	
			,Mode6
#endif			
		};
		switchmodes[0]();
	}
	
	public void Mode0() {
		int fe=text.GetDefaultNativeEngine(Application.platform);
		Debug.Log("0-"+fe.ToString());
		text.SetPreferedEngine(Application.platform,fe);
		text.InitTextStyle.SetBitmapMode(fe,false);		
		text.FontId="Arial (Regular)";	
		text.Size=0.25f;
		text.LineWidth=2f;
		text.Text="0- Arial Mesh Native No Extrusion"+ extrachar;
		text.ExtrusionMode=TTFText.ExtrusionModeEnum.None;
		text.TokenMode=TTFText.TokenModeEnum.Character;
		text.rebuildWithCoroutine=true;
		//text.InitTextStyle.GetUsedFontEngine();
	}

	public void Mode1() {
		int fe=text.GetDefaultNativeEngine(Application.platform);
		Debug.Log("1-"+fe.ToString());
		text.SetPreferedEngine(Application.platform,fe);
		text.InitTextStyle.SetBitmapMode(fe,false);		
		text.FontId="Arial (Regular)";		
		text.Text="1- Arial Mesh Native Extrusion"+extrachar;
		text.Size=0.25f;
		text.ExtrusionMode=TTFText.ExtrusionModeEnum.Simple;
		text.TokenMode=TTFText.TokenModeEnum.Character;
		//text.InitTextStyle.GetUsedFontEngine();
	}

	public void Mode2() {
		int fe=text.GetDefaultNativeEngine(Application.platform);
		Debug.Log("2-"+fe.ToString());
		text.SetPreferedEngine(Application.platform,fe);		
		text.InitTextStyle.SetBitmapMode(fe,true);
		text.FontId="Arial (Regular)";		
				//text.InitTextStyle.SetPreferedEngine(text.InitTextStyle.G,0);
		text.Text="2-Arial Bitmap Native"+extrachar;
		text.ExtrusionMode=TTFText.ExtrusionModeEnum.None;
		text.TokenMode=TTFText.TokenModeEnum.Character;
		//text.InitTextStyle.GetUsedFontEngine();
	}

	public void Mode3() {
		int fe=text.GetDefaultEmbeddedEngine(Application.platform);
		text.SetPreferedEngine(Application.platform,fe);		
		Debug.Log("3-"+fe.ToString()+"-"+text.GetPreferedEngine(Application.platform).ToString());		
		text.InitTextStyle.SetBitmapMode(fe,false);
		text.Text="3- Arial Mesh Embedded No Extrusion:"+fe.ToString()+extrachar;
		text.ExtrusionMode=TTFText.ExtrusionModeEnum.None;
	}

	public void Mode4() {
		int fe=text.GetDefaultEmbeddedEngine(Application.platform);
		text.SetPreferedEngine(Application.platform,fe);		
		Debug.Log("4-"+fe.ToString()+"-"+text.GetPreferedEngine(Application.platform).ToString());		
		text.InitTextStyle.SetBitmapMode(fe,false);

		text.InitTextStyle.SetPreferedEngine(Application.platform,1);
		text.Text="4- Arial Mesh Embedded Extrusion:"+fe.ToString()+extrachar;
		text.ExtrusionMode=TTFText.ExtrusionModeEnum.Simple;
		text.TokenMode=TTFText.TokenModeEnum.Character;
		//text.InitTextStyle.GetUsedFontEngine();
	}

	
	
	public void Mode5() {
		int fe=text.GetDefaultNativeEngine(Application.platform);
		Debug.Log("5-"+fe.ToString());
		text.SetPreferedEngine(Application.platform,fe);		
		text.InitTextStyle.SetBitmapMode(fe,false);

		
		
		text.FontId="";
		text.Text="5-Native No Font Id No Extrusion";
		text.ExtrusionMode=TTFText.ExtrusionModeEnum.None;
		text.TokenMode=TTFText.TokenModeEnum.Character;
		//text.InitTextStyle.GetUsedFontEngine();
	}
	
#if UNITY_ANDROID || UNITY_IPHONE	
	public void Mode6() {
		Debug.Log("6- sysfont (Droid Sans)");
		text.SetPreferedEngine(Application.platform,3);		
		text.InitTextStyle.SetBitmapMode(3,true);

		text.FontId="Droid Sans";
		text.Text="6 Sysfont"+ extrachar;
		text.ExtrusionMode=TTFText.ExtrusionModeEnum.None;
		text.TokenMode=TTFText.TokenModeEnum.Character;
		//text.InitTextStyle.GetUsedFontEngine();
	}
#endif
	
	void OnGUI() {
		GUI.Label(new Rect(0,0,100,100),text.Text);
	}
	
	// Update is called once per frame
	void Update () {
		if ((Time.time-stime)>3f) {
			cmode++;
			switchmodes[cmode%switchmodes.Length]();
			stime=Time.time;
		}
		textobject.transform.position=new Vector3(0,Mathf.Sin(Time.time),0);
	}
}
