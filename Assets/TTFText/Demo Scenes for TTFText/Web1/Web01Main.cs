using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/Main for Scene Web01")]
public class Web01Main : MonoBehaviour {
	
	string ct="J";
	TTFText tm;
	TTFText tmb;
	TTFText tmj;
	TTFText tmlc;
	TTFText tmlcdp;
	Vector3 bp;
	
	// Use this for initialization
	void Start () {
		tmb=GameObject.Find("/Text B").GetComponent<TTFText>();		
		tmj=GameObject.Find("/Text J").GetComponent<TTFText>();		
		tmlc=GameObject.Find("/Text LC").GetComponent<TTFText>();		
		tmlcdp=GameObject.Find("/Text LCDP").GetComponent<TTFText>();		
		tm=tmj;
		bp=tm.transform.position;
		tmb.transform.position=bp+Vector3.forward*100;		
		tmlc.transform.position=bp+Vector3.forward*100;
		tmlcdp.transform.position=bp+Vector3.forward*100;
	}
	
	// Update is called once per frame
	void Update () {
		float r=1.6f;
		Camera.main.transform.position=new Vector3(Mathf.Cos(Time.time*0.4f)*r,4f,Mathf.Sin(Time.time*0.4f)*r); 
		Camera.main.transform.LookAt(tm.transform,Vector3.forward);
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
		GUILayout.BeginHorizontal();
		GUI.color=( ct=="J" )?Color.magenta:Color.red;
		if (GUILayout.Button("Junction")) { 
			ct="J";
			tm.gameObject.renderer.enabled=false;
			tm.transform.position=bp+Vector3.forward*100;
			tm=GameObject.Find("/Text "+ct).GetComponent<TTFText>();
			tm.gameObject.renderer.enabled=true;
			tm.transform.position=bp;
			tm.Slant=0;tm.Slant=1;tm.Slant=0;
			
		}	
		GUI.color=( ct=="B" )?Color.magenta:Color.red;
		if (GUILayout.Button("Talie")) { 
			ct="B";
			tm.gameObject.renderer.enabled=false;
			tm.transform.position=bp+Vector3.forward*100;
			tm=GameObject.Find("/Text "+ct).GetComponent<TTFText>();
			tm.gameObject.renderer.enabled=true;
			tm.transform.position=bp;
			tm.Slant=0;tm.Slant=1;tm.Slant=0;
	        tm.ExtrusionMode=TTFText.ExtrusionModeEnum.Simple;
			tm.ExtrusionDepth=0;tm.ExtrusionDepth=1;			
		}	
		
		GUI.color=( ct=="LC" )?Color.magenta:Color.red;
		if (GUILayout.Button("Strato")) { 
			ct="LC";
			tm.gameObject.renderer.enabled=false;
			tm.transform.position=bp+Vector3.forward*100;
			tm=GameObject.Find("/Text "+ct).GetComponent<TTFText>();
			tm.gameObject.renderer.enabled=true;
			tm.transform.position=bp;
			tm.Slant=0;tm.Slant=1;tm.Slant=0;
		}	
		GUI.color=( ct=="LCDP" )?Color.magenta:Color.red;
		if (GUILayout.Button("Dots")) { 
			ct="LCDP";
			tm.gameObject.renderer.enabled=false;
			tm.transform.position=bp+Vector3.forward*100;
			tm=GameObject.Find("/Text "+ct).GetComponent<TTFText>();
			tm.gameObject.renderer.enabled=true;
			tm.transform.position=bp;
			tm.Slant=0;tm.Slant=1;tm.Slant=0;
	        tm.ExtrusionMode=TTFText.ExtrusionModeEnum.Simple;
			tm.ExtrusionDepth=0;tm.ExtrusionDepth=0.1f;
			
		}	
		
		GUILayout.EndHorizontal();

		
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
			//tm.ExtrusionSteps=;
		}
		
		/*
				f = EditorGUILayout.FloatField("Gamma", tm.Gamma);
				if (f != tm.Gamma) {
					tm.Gamma = f;
					int nbDiv=tm.NbDiv;
					tm.ExtrusionSteps = new float[nbDiv];
					float cz=0; 
					float deltaZ = 0;
		
					for (int i=1;i<(nbDiv+1);i++) {
						deltaZ=Mathf.Pow(Mathf.Sin(i*Mathf.PI/(nbDiv+1)),tm.Gamma);
						tm.ExtrusionSteps[i-1]=cz;
						cz+=deltaZ;
					}
		
					cz -= deltaZ;
					if (cz != 0) {
						for (int i=0;i<nbDiv;i++) {
							tm.ExtrusionSteps[i]/=cz;
						}
					}
				}
		*/	
		/*
		
		 if (tm.ExtrusionMode == TTFText.ExtrusionMode.Bevel) {
	
				tm.ExtrusionDepth = EditorGUILayout.FloatField("Extrusion Depth", tm.ExtrusionDepth);			
				tm.BevelForce = EditorGUILayout.FloatField("Bevel Force", tm.BevelForce);
			
				idx = EditorGUILayout.IntField("Steps", tm.NbDiv);
				if (idx != tm.NbDiv) {
					if (idx < 2) { idx = 2; }
					tm.NbDiv = idx;
				}
			
				 tm.BevelDepth = EditorGUILayout.Slider("Bevel Depth %", tm.BevelDepth, 0f, 1f);
				
				
			} 
			
		*/
		GUILayout.EndArea();
	}
}
