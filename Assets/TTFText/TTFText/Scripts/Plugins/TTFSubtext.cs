using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/Internal/TTFSubtext")]
public class TTFSubtext : MonoBehaviour {
	//[SerializeField]
	//private int _lineidx;
	[SerializeField]
	public int _seqno;
	[SerializeField]
	public string _text;
	[SerializeField]
	public Vector3 _advance;
	[SerializeField]
	public Vector3 _localsoftposition;
#if !TTFTEXT_LITE	
	[SerializeField]
	public TTFTextInternal.LineLayout _layout;
#endif
	
	public void Awake() {
		_localsoftposition=transform.localPosition;
	}
	
	
	#region DEPRECATED ACCESSOR	
	public int lineidx {
		get { Debug.LogWarning("lineidx is deprecated"); return LineNo; }
		//set { _lineidx=value; }
	}
	
	
	
	public int seqno {
		get {
#if UNITY_EDITOR			
			Debug.LogWarning("seqno is deprecated use SequenceNo instead");
#endif			
			return _seqno;}
		set {_seqno=value;}
	}
	
	public string text 	{
		get { 
#if UNITY_EDITOR			
			Debug.LogWarning("'text' is depracted use 'Text' instead");
#endif			
			return _text; }
		set {_text=value;}
	}

	public Vector3 advance;
	#endregion
	
	
	public string Text 	{
		get {return _text; }
		set {_text=value;}
	}
	
	public Vector3 Advance {
		get {return _advance; }
		set {_advance=value;}		
	}
	
	public int SequenceNo {
		get {return _seqno;}
		set {_seqno=value;}
	}
	
	public Vector3 LocalSoftPosition {
		get {return _localsoftposition;}
		set {_localsoftposition=value;}
	}
	
	
	public int LineNo;

#if !TTFTEXT_LITE	
	public TTFTextInternal.LineLayout Layout {get{return _layout;} set{_layout=value;}}
	
	public void Rebuild() {
		Vector3 advance=Vector3.zero;
		Mesh m;
		MeshFilter mf=GetComponent<MeshFilter>();
		m=mf.sharedMesh;
		TTFText tm=transform.parent.GetComponent<TTFText>();
		TTFTextInternal.Engine.BuildMesh(ref m,Layout,tm,out advance);	
		mf.sharedMesh=m;
	}
#endif
	
	
	/*
	public int WordNo;
	public int LetterNo;
	public int SequenceId;
	public float percentLine;
	
	*/
	
}
