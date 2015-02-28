using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/Main for Scene Web02")]
public class Web02Main : MonoBehaviour {
	
	string ct="J";
	TTFText tm;
	TTFText tmb;
	TTFText tmj;
	TTFText tmlc;
	Vector3 bp;
	bool showMeshes=false;
	float camH=10;
	
	// Use this for initialization
	void Start () {
		tmb=GameObject.Find("/Text B").GetComponent<TTFText>();		
		tmj=GameObject.Find("/Text J").GetComponent<TTFText>();		
		tmlc=GameObject.Find("/Text LC").GetComponent<TTFText>();		
		tm=tmj;
		bp=tm.transform.position;
		tmb.transform.position=bp+Vector3.forward*100;		
		tmlc.transform.position=bp+Vector3.forward*100;
		foreach (Transform t in tm.transform) { t.renderer.enabled=showMeshes;}
	}
	
	// Update is called once per frame
	void Update () {
		float r=1.6f;
		Camera.main.transform.position=new Vector3(Mathf.Cos(Time.time*0.4f)*r,Mathf.Sin(Time.time*0.4f)*r,-camH); 
		Camera.main.transform.LookAt(tm.transform,Vector3.up);
	}
	
	public void OnGUI() {
		GUI.color=Color.black;
		GUI.Label(new Rect((Screen.width-400)/2,10,400,300),
			"This is a TTF Text demo.");		
		
		GUI.color=Color.red;
		foreach (Transform t in tm.transform) { t.renderer.enabled=showMeshes;}
		
		
		GUILayout.BeginArea(new Rect(20,20,Screen.width/2.5f-40,Screen.height));	
		
		
		
		
		GUILayout.Label("Change the text");		
		tm.Text=GUILayout.TextArea(tm.Text,32);
		
		
		
		GUILayout.Label("Show Meshes");
		GUILayout.BeginHorizontal();
		GUI.color=( showMeshes )?Color.magenta:Color.red;
		if (GUILayout.Button("Yes")) { 
			showMeshes=true;
			foreach (Transform t in tm.transform) { t.renderer.enabled=showMeshes;}
		}
		GUI.color=( !showMeshes )?Color.magenta:Color.red;
		if (GUILayout.Button("No")) { 
			showMeshes=false;
			foreach (Transform t in tm.transform) { t.renderer.enabled=showMeshes;}
		}
		GUILayout.EndHorizontal();
		
		
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
			foreach (Transform t in tm.transform) { t.renderer.enabled=showMeshes;}
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
			tm.ExtrusionDepth=0;tm.ExtrusionDepth=1;			
			foreach (Transform t in tm.transform) { t.renderer.enabled=showMeshes;}
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
			foreach (Transform t in tm.transform) { t.renderer.enabled=showMeshes;}
		}	

		GUILayout.EndHorizontal();

		
		GUI.color=Color.red;
		GUILayout.Label("Embold amount");		
		tm.Embold=GUILayout.HorizontalSlider(tm.Embold,-30,30);
		GUILayout.Label("Character Spacing");		
		tm.Hspacing=GUILayout.HorizontalSlider(tm.Hspacing,1,2);
		GUILayout.Label("Simplify outline amount");		
		tm.SimplifyAmount=GUILayout.HorizontalSlider(tm.SimplifyAmount,1.5f,5f);
		GUILayout.Label("Camera Height");		
		camH=GUILayout.HorizontalSlider(camH,3,15f);
		
		
		
		GUILayout.EndArea();
	}
}
