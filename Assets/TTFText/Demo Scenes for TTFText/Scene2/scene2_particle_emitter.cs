using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/Text Particle Emitter")]
public class scene2_particle_emitter : MonoBehaviour {
	
	public float frequency=1;
	public float flareFrequency=1;
	public float aspeed=1.5f;
	public float minspeed=1.5f;
	public float speedinterval=3f;
	public string [] words= "a b c d e f g h i j k l m n o p q r s t u v z x y z".Split(new char[] {' '});
	public Material [] materials;
	public Flare [] flares;
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
	
	public System.Collections.Generic.List<string>  fonts;// = new System.Collections.Generic.List<string>(FontListManager.Instance.SystemFonts.Keys); 
	// Use this for initialization
	void Start () {
		fonts = new System.Collections.Generic.List<string>(TTFTextFontListManager.Instance.SystemFonts.Keys);
	}
	
	// Update is called once per frame
	void Update () {
		if (Random.Range(0,1f/frequency)<Time.deltaTime) {
			GameObject go =new GameObject();
			go.AddComponent<MeshRenderer>();
			go.AddComponent<MeshFilter>();			
			go.AddComponent<scene2_particle_autodestroy>();
			go.AddComponent<Rigidbody>();
			go.AddComponent<TTFText>();
			TTFText tm=go.GetComponent<TTFText>();
			do {
			  tm.ExtrusionMode=(TTFText.ExtrusionModeEnum)Random.Range(0,6);
			} while (tm.ExtrusionMode==TTFText.ExtrusionModeEnum.FreeHand);
			tm.ExtrusionDepth=Random.Range(0.1f,1f);
			tm.NbDiv=Random.Range(2,8);
			tm.BevelForce=Random.Range(0.2f,0.5f);
			tm.Gamma=Random.Range(0f,7f);
			tm.BevelDepth=Random.Range(0f,1f);
			tm.Text=words[Random.Range(0,words.Length)];
			tm.Size=Random.Range(0.1f,1.2f);
			go.rigidbody.velocity=Vector3.back*Random.Range(minspeed,minspeed+speedinterval);
			go.rigidbody.angularVelocity=new Vector3(Random.Range(-aspeed,aspeed),Random.Range(-aspeed,aspeed),Random.Range(-aspeed,aspeed));
			go.rigidbody.useGravity=false;
			go.transform.position=new Vector3(Random.Range(-2f,2f),Random.Range(-2f,2f),5);
			go.transform.parent=transform;
			go.renderer.material=materials[Random.Range(0,materials.Length)];
            //tm.runtimeFontPath=FontListManager.Instance.SystemFonts[fonts[Random.Range(0,fonts.Count)]].Path;
            tm.FontId = fonts[Random.Range(0,fonts.Count)];
		}

        if (Random.Range(0,1f/flareFrequency)<Time.deltaTime) {
			GameObject go =new GameObject();
			go.AddComponent<LensFlare>();
			go.AddComponent<Rigidbody>();
			go.rigidbody.velocity=Vector3.back*Random.Range(minspeed+speedinterval,minspeed+2*speedinterval);
			go.rigidbody.useGravity=false;
			go.transform.position=new Vector3(Random.Range(-2f,2f),Random.Range(-2f,2f),5);
			go.transform.parent=transform;
			LensFlare lf=go.GetComponent<LensFlare>();
			lf.color=new Color(Random.Range(0.5f,1f),Random.Range(0.5f,1f),Random.Range(0.5f,1f));
			lf.flare = flares[Random.Range(0,flares.Length)];
			lf.brightness=Random.Range(0f,1f);
		}
	}
#endif
}
