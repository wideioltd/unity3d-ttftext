using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/Main for Scene Android01")]
public class Android01Main : MonoBehaviour {
	
	string ct="J";
	TTFText tm;
	TTFText tmb;
	TTFText tmh;
//	Vector3 bp;
	
	// Use this for initialization
	void Start () {
		tmb=GameObject.Find("/Text B").GetComponent<TTFText>();		
		tmh=GameObject.Find("/TTF Hello").GetComponent<TTFText>();		
		tm=tmb;
//		bp=tm.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float r=1.6f;
		Camera.main.transform.position=new Vector3(Mathf.Cos(Time.time*0.4f)*r,Camera.main.transform.position.y+Input.acceleration.y,Mathf.Sin(Time.time*0.4f)*r); 
		Camera.main.transform.LookAt(tm.transform,Vector3.forward);
		tmh.Size=1.5f+Mathf.Sin(Time.time);
	}
	
	public void OnGUI() {
		GUI.color=Color.black;
		//GUI.Label(new Rect((Screen.width-400)/2,10,400,300),
		//	"2012/03/14: First demo of textmesh generation running on the web, some algorithms are still experimental and will be improved soon.");		
		
		GUI.color=Color.red;
		
		
		
		GUILayout.BeginArea(new Rect(20,20,Screen.width/2.5f-40,Screen.height));	
		
		
		
		
		GUILayout.Label("Change the text");		
		tm.Text=GUILayout.TextArea(tm.Text,32);
		
		
		
		GUILayout.Label("Font");
		//int c=2;
		if (GUILayout.Button("default")) { 
				tm.InitTextStyle.SetFontEngineFontId(2,"default");
				tm.SetDirty();
		}	
		if (GUILayout.Button("Droid Sans")) { 
				tm.InitTextStyle.SetFontEngineFontId(2,"Droid Sans");
				tm.SetDirty();
		}
		
		/*
		foreach (string vs in TTFTextFontProvider.font_providers[2].GetFontList(tm.InitTextStyle.GetFontProviderParameters(2))) {
			if (GUILayout.Button(vs)) { 
				tm.InitTextStyle.SetFontProviderFontId(2,vs);
				tm.SetDirty();
			}
			c++;
			if (c>8) break;
		}
		 */
		
		GUI.color=Color.red;
		GUILayout.Label("Slant amount");		
		tm.Slant=GUILayout.HorizontalSlider(tm.Slant,-1,1);
		GUILayout.Label("Character Spacing");		
		tm.Hspacing=GUILayout.HorizontalSlider(tm.Hspacing,1,2);
		GUILayout.Label("Simplify outline amount");		
		tm.SimplifyAmount=GUILayout.HorizontalSlider(tm.SimplifyAmount,1.5f,5f);
		
		
			
		GUILayout.Label("Extrusion");
		if (ct!="LCDP") {
	
		GUILayout.BeginHorizontal();
		GUI.color=( tm.ExtrusionMode==TTFText.ExtrusionModeEnum.Simple)?Color.magenta:Color.red;
		if (GUILayout.Button("Simple")) { 
				tm.ExtrusionMode=TTFText.ExtrusionModeEnum.Simple;
			tm.ExtrusionDepth=0;tm.ExtrusionDepth=1;
		}	
		GUI.color=( tm.ExtrusionMode==TTFText.ExtrusionModeEnum.Bevel)?Color.magenta:Color.red;
		if (GUILayout.Button("Bevel")) { tm.ExtrusionMode=TTFText.ExtrusionModeEnum.Bevel;
			tm.ExtrusionDepth=0;tm.ExtrusionDepth=1;
		}
		GUI.color=( tm.ExtrusionMode==TTFText.ExtrusionModeEnum.Bent)?Color.magenta:Color.red;
		if (GUILayout.Button("Bent")) { tm.ExtrusionMode=TTFText.ExtrusionModeEnum.Bent;
			tm.ExtrusionDepth=0;tm.ExtrusionDepth=1;
		}				
		GUILayout.EndHorizontal();
		}
		
		GUI.color=Color.red;
		if (tm.ExtrusionMode != TTFText.ExtrusionModeEnum.None) {
			GUILayout.Label("Depth");
			tm.ExtrusionDepth=GUILayout.HorizontalSlider(tm.ExtrusionDepth,0,3.5f);
		}

		if ((tm.ExtrusionMode == TTFText.ExtrusionModeEnum.Bent)||(tm.ExtrusionMode == TTFText.ExtrusionModeEnum.Bevel)) {
			GUILayout.Label("Intensity");
			tm.BevelForce = GUILayout.HorizontalSlider(tm.BevelForce,0,5);
			tm.BevelDepth=1;
		
		}
		
		
		GUILayout.EndArea();
	}
}
