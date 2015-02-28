using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText Prefab Effect Scripts/Event/OnDisplayLetter/Play Sound")]
public class DisplayLetter_PlaySound : MonoBehaviour {	
	TTFSubtext pt;
	public AudioClip [] clips_newline;
	public AudioClip [] clips_newatom;
	AudioSource aus;
	
	
	// Use this for initialization
	void Start () {
		aus=GetComponent<AudioSource>();
		if (aus==null) {
			gameObject.AddComponent<AudioSource>();
			aus=GetComponent<AudioSource>();
		}
	}
	
	
	
	
	public void DisplayLetter() {
		if ((clips_newatom!=null)&&(clips_newatom.Length!=0)) {
			aus.clip=clips_newatom[Random.Range(0,clips_newatom.Length)];
			aus.Play();
		}		
	}
	
	
	public void NewLine() {
		if ((clips_newline!=null)&&(clips_newline.Length!=0)) {
			aus.clip=clips_newline[Random.Range(0,clips_newline.Length)];
			aus.Play();
		}		
	}
}

