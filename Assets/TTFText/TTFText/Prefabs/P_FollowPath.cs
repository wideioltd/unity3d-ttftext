using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[ExecuteInEditMode]
[RequireComponent(typeof(TTFSubtext))]
[AddComponentMenu("Text/TTFText Prefab Effect Scripts/Follow Path")]
public class P_FollowPath : MonoBehaviour {
	public static byte [] SerializeObject(object o)
{
    if (!o.GetType().IsSerializable)
    {
        return null;
    }

    using (MemoryStream stream = new MemoryStream())
    {
        new BinaryFormatter().Serialize(stream, o);
        return stream.ToArray();
    }
}

public static object DeserializeObject(byte [] bytes)
{

    using (MemoryStream stream = new MemoryStream(bytes))
    {
        return new BinaryFormatter().Deserialize(stream);
    }
}
	

	public Vector3 [] path;
	public bool pathVisible=false;
	public int mode;
	public System.Object parameters_c;
	public System.Object parameters {
		get { 
			  if (parameters_c!=null) {
					return parameters_c;
			  }
			  else {
				parameters_c=DeserializeObject(parameters_serialized);
				return parameters_c;
			  }
			}
		set {
				parameters_c=value;
				parameters_serialized=SerializeObject(parameters_c);
			}
	}
	
	
	public void SaveParams() {
		parameters_serialized=SerializeObject(parameters_c);
	}
	
	public byte [] parameters_serialized;
	public Vector3 up=Vector3.up;
	public AnimationCurve scalingCurve=AnimationCurve.Linear(0,1,1,1);
	public AnimationCurve YZRotationCurve=AnimationCurve.Linear(0,0,1,0);
	public bool orientAccordingPath=true;
	
	public interface FollowPathPresetMode {
		string GetModeName();
		 System.Object DefaultParameters();
		void Generate(P_FollowPath pfp);		
		void Update(P_FollowPath pfp,float t);
	}
	
	public class ModeSinusoid : FollowPathPresetMode {
		[System.Serializable]
		public class Parameters : System.Object {
			public float Period;
			public float Phase;
			public float Amplitude;
			public float Duration;
			public float Oscillation;
		}
		
		public string GetModeName() {return "Sinusoid";}
				
		public System.Object DefaultParameters () {
			Parameters pa=new Parameters();
			pa.Period=1f;
			pa.Phase=0;	
			pa.Amplitude=0.3f;
			pa.Duration=1f;
			pa.Oscillation=0f;
			return pa;
		}

		
		
		public void Generate(P_FollowPath pfp) {
			Parameters pa= pfp.parameters as Parameters;
			float xl=1;
			try {
				xl=pfp.gameObject.transform.parent.GetComponent<TTFText>().advance.magnitude;
			}
			catch {}
			for (int i=0;i<pfp.path.Length;i++) {
				pfp.path[i]=new Vector3(
					(((float)i)/pfp.path.Length)*pa.Duration*xl,
					Mathf.Cos(i*(2*Mathf.PI)/(pfp.path.Length*pa.Period)+pa.Phase*(2*Mathf.PI))*pa.Amplitude);
			}
			pfp.ComputePositions();
		}
		
		public void Update(P_FollowPath pfp,float t) {
			Parameters pa= pfp.parameters as Parameters;
			float xl=1;
			try {
				xl=pfp.gameObject.transform.parent.GetComponent<TTFText>().advance.magnitude;
			}
			catch {}
			
			for (int i=0;i<pfp.path.Length;i++) {
				pfp.path[i]=new Vector3(
					(((float)i)/pfp.path.Length)*pa.Duration*xl,
					Mathf.Cos(i*(2*Mathf.PI)/(pfp.path.Length*pa.Period)+(pa.Oscillation*t+pa.Phase)*(2*Mathf.PI))*pa.Amplitude);
			}
			pfp.ComputePositions();
		}
		
	}

	public class ModeFreehand : FollowPathPresetMode {
		[System.Serializable]
		public class Parameters  : System.Object {
		}
		
		public string GetModeName() {return "Freehand";}
				
		public   System.Object  DefaultParameters () {
			Parameters pa=new Parameters();
			return pa;
		}
		
		
		public void Generate(P_FollowPath pfp) {
		}
		
		public void Update(P_FollowPath pfp,float t) {
		}			
	}

	
	public class ModeCircular : FollowPathPresetMode {
		[System.Serializable]
		public class Parameters : System.Object {
			public float Radius=2;
			public float Phase=0;
			public float RotationSpeed=0;
			public float Loops=0.9f;
			public float SpiralFactor=0;
			public bool Direction=false;
		}
		
		public string GetModeName() {return "Circular";}
				
		public   System.Object DefaultParameters () {
			Parameters pa=new Parameters();
			pa.Radius=1;
			pa.Phase=0;	
			return pa;
		}

		
		public void Generate(P_FollowPath pfp) {
			Parameters pa= pfp.parameters as Parameters;
			float xl=1;
			try {
				xl=pfp.gameObject.transform.parent.GetComponent<TTFText>().advance.magnitude;
			}
			catch {}
					xl/=pa.Loops;
			for (int i=0;i<pfp.path.Length;i++) {
				float f=((float)i)/pfp.path.Length;
				
				pfp.path[i]=
					new Vector3(Mathf.Cos(((pa.Direction)?1f:-1f)*f*(2*Mathf.PI*pa.Loops)+pa.Phase+Time.time*pa.RotationSpeed)*(pa.Radius+(f*pa.SpiralFactor))*xl, 
						        Mathf.Sin(((pa.Direction)?1f:-1f)*f*(2*Mathf.PI*pa.Loops)+pa.Phase+Time.time*pa.RotationSpeed)*(pa.Radius+(f*pa.SpiralFactor))*xl,0);
			}
			pfp.ComputePositions();
		}
		
		
		public void Update(P_FollowPath pfp, float t) {
			Parameters pa= pfp.parameters as Parameters;
			float xl=1;
			try {
				xl=pfp.gameObject.transform.parent.GetComponent<TTFText>().advance.magnitude;
			}
			catch {}
				xl/=pa.Loops;	
			for (int i=0;i<pfp.path.Length;i++) {
				float f=((float)i)/pfp.path.Length;
				
				pfp.path[i]=
					new Vector3(Mathf.Cos(((pa.Direction)?1f:-1f)*f*(2*Mathf.PI*pa.Loops)+pa.Phase+t*pa.RotationSpeed)*(pa.Radius+(f*pa.SpiralFactor))*xl, 
						        Mathf.Sin(((pa.Direction)?1f:-1f)*f*(2*Mathf.PI*pa.Loops)+pa.Phase+t*pa.RotationSpeed)*(pa.Radius+(f*pa.SpiralFactor))*xl,0);
			}
			pfp.ComputePositions();
		}			
		
	}
	
	
	static public FollowPathPresetMode [] presetmodes = {
		new ModeFreehand(),
		new ModeSinusoid(),		
		new ModeCircular()
	};
	
	
	
	
	
	
	public Vector3 pTween(float f) {
		if (f<0) f=0;
		if (f>1) f=1;
		if (f==1) return path[path.Length-1];
		if (path.Length<=1) return path[0];
		float g=(path.Length-1)*f;
		int ci=(int)Mathf.Floor(g);		
		g=g-ci;
		return (1-g)*path[ci]+g*path[ci+1];
	}
	
	
	//get orthogonal tween
	public Vector3 oTween(float f) {
		float epsilon=0.001f;
		if (f>=(1-epsilon)) {
			return (pTween(f)-pTween(f-epsilon)).normalized;
		}
		else {
			return (pTween(f+epsilon)-pTween(f)).normalized;
		}
	}
	
	
	void OnEnable() {}	
	void OnDisable(){}
	
	public void OnDrawGizmosSelected() {
		if(pathVisible){
			if (path!=null) {
				for (int i=0;i<path.Length-1;i++) {
					Debug.DrawLine(path[i],path[i+1]);
				}	
			}
			
			pathVisible=false;
		}
	}

	
	public void ComputePositions() {
		TTFSubtext subtext=GetComponent<TTFSubtext>();
		if (!transform.parent) {
			return;
		}
		TTFText tt=transform.parent.GetComponent<TTFText>();
		if (tt==null) return;		
		if (subtext==null) return;
		if (subtext.Layout==null) {
			Debug.LogWarning("No layout info");
			return;
		}
		
		float x=subtext.Layout.CharSumAdvance(0)/tt.advance.magnitude;
			
		transform.localPosition=pTween(x);
		transform.localScale=Vector3.one*scalingCurve.Evaluate(x);
		if (orientAccordingPath) {
			transform.localRotation=Quaternion.FromToRotation(Vector3.right,oTween(x));
			
		}
		transform.localRotation=Quaternion.Euler(YZRotationCurve.Evaluate(x)*180,0,0)*transform.localRotation;
	}
	
	// Use this for initialization
	void Start () {
		//transform.localRotation=Quaternion.Euler(alpha,0,0);
		
		ComputePositions();		
		presetmodes[mode].Update(this,Time.time);		  
		//float x=subtext.SequenceNo;//subtext.LocalSoftPosition.x;
		//float alpha=Mathf.Sqrt(Mathf.Abs(af*x));
		//float r=rf*x;
		//transform.localPosition=new Vector3(Mathf.Cos(alpha)*r,Mathf.Sin(alpha)*r,transform.localPosition.z);
		//transform.localRotation=Quaternion.Euler(alpha,0,0);
	}
	
	// Update is called once per frame
	
	
	
	void Update () {		
		presetmodes[mode].Update(this,Time.time);		  
	}
}

