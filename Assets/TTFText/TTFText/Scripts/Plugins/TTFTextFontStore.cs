using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TTF = FTSharp;

/// <summary>
/// TTF text font store.
/// 
/// This is meant to be a global storage for embedded/baked/cached fonts
/// and for network fonts.
/// 
/// Currently only embedded fonts are supported.
/// The new fontprovider scheme has changed our view of the evolution of the soft.
/// The place of the fontstore may evolve during next releases
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("Text/Internal/TTFText Font Store")]
public class TTFTextFontStore : MonoBehaviour {	
#if ! TTFTEXT_LITE	

#region Singleton
	public const string STORE_NAME = "TTFText Font Store";
	public string defaultAdditionalCharacters="";
	public int defaultInterpolationSteps=4;	
	public bool load_all_fonts_on_start=true;
	
	public static bool IsInstanciated {
		get {
			return GameObject.Find("/" + STORE_NAME) != null;
		}		
	}
	
	
	public static GameObject GOInstance {
		get {
			GameObject go = GameObject.Find("/" + STORE_NAME);
			if (go == null) {
				go = new GameObject();
				go.name = STORE_NAME;
				TTFTextFontStore store = go.AddComponent<TTFTextFontStore>();
				store.Start();
				if (!TTFTextGlobalSettings.Instance.ShowTTFTextObjects) {
					go.hideFlags=HideFlags.HideInHierarchy;
#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(go);
#endif					
				}
				else {
					go.hideFlags=0;
				}
			}
			return go;
		}
	}
	
	
	public static TTFTextFontStore Instance {
		get {
			GameObject go = GOInstance;
			TTFTextFontStore store = go.GetComponent<TTFTextFontStore>();
			if (store == null) {
				store = go.AddComponent<TTFTextFontStore>();
				store.Start();
			}
			return store;
		}
	}	
	
	public void RebuildAllCharsets() {
		foreach(TTFTextFontStoreFont e in embeddedFonts) {
			e.AddRequiredCharacters(defaultAdditionalCharacters); // FIXME: this adds character but never remove characters 
			e.BuildCharSet(e.fontid);
		}
	}
	
#endregion
	
	
	public bool dontDestroyOnLoad=false;
	public bool destroyWhenUnused=false;	
		
	[SerializeField]
	public List<TTFTextFontStoreFont> embeddedFonts = new List<TTFTextFontStoreFont>();
	//	private List<TTFTextFontStoreFont> dynamicLoadedFonts;

	[SerializeField]
	public List<string> resourceFonts = new List<string>();
	
	public List<TTFText> Clients = new List<TTFText>();
	
	
	//private List<TTFTextMesh> clients;	
	//public  List<TTFTextMesh> GetClients() {return clients;}
	
	
#region MANAGE_FONTS
	
	private int needFGC=0;
	public void SetGarbageCollectUnusedFonts() {
		needFGC++;
	}
	
#endregion MANAGE_FONTS	
	
	public bool LoadAllResourceFonts() {		
		bool added=false;
		Object [] loaded_fonts= Resources.LoadAll("TTFText/Fonts",typeof(TTFTextFontStoreFontAsset));
//		Debug.Log("Found "+loaded_fonts.Length.ToString()+" Fonts");
		foreach (Object o in loaded_fonts) {
			
			TTFTextFontStoreFontAsset asset=(TTFTextFontStoreFontAsset) o;
			if (GetEmbeddedFont(asset.fontname,false)==null) {
				embeddedFonts.Add(asset.font);
				added=true;
			}
		}
		return added;
	}
	
	public bool TryLoadAssetFont(string s) {		
		bool added=false;
		Object  loaded_font= Resources.Load("TTFText/Fonts/"+s,
			                                   typeof(TTFTextFontStoreFontAsset)
			                                  );

		if (loaded_font!=null) {			
			TTFTextFontStoreFontAsset asset=(TTFTextFontStoreFontAsset) loaded_font;
			if (GetEmbeddedFont(asset.fontname,false)==null) {
				embeddedFonts.Add(asset.font);
				added=true;
			}
		}
		return added;
	}
	
	
	public TTFTextFontStoreFont GetEmbeddedFont(string fontid,bool check_resources=true) {
		if (embeddedFonts==null) {
			Debug.LogWarning("EmbeddedFonts should not be null");
			return null;
		}
		foreach(TTFTextFontStoreFont cf in embeddedFonts) {
			if (cf.fontid==fontid) {
				return cf;
			}
		}
		if ((check_resources)&&(LoadAllResourceFonts())) {
			foreach(TTFTextFontStoreFont cf in embeddedFonts) {
				if (cf.fontid==fontid) {
					return cf;
				}
			}			
		}		
		return null;
	}

	
	
	public TTFTextFontStoreFont GetFont(string fontid) {
		return GetEmbeddedFont(fontid);
		/*
		TTFTextFontStoreFont r=null;
		r=GetEmbeddedFont(fontid);
		if (r!=null) return r;
		r=GetDynamiclyLoadedFont(fontid);
		if (r!=null) return r;		
		return r;
		 */
	}
	
	public void RemoveFont(string fontid) {
		embeddedFonts.RemoveAll(x=>(x.fontid==fontid));	
	}
	
	public TTFTextFontStoreFont EnsureFont(string fontid) {
		// ensures that a specific font is present the store	
		// this function does not update the reference counts
		
		TTFTextFontStoreFont f = GetFont(fontid);
		
		if (f==null) {
			TTFTextFontStoreFont nf=new TTFTextFontStoreFont();
			if (fontid.Length>0) {
			nf.fontid=fontid;
			nf.charset=null;
			nf.AddRequiredCharacters(this.defaultAdditionalCharacters); // addd this
			embeddedFonts.Add(nf);
			f=nf;
#if UNITY_EDITOR
				if (!Application.isPlaying) {
				string d=Application.dataPath;
				d=System.IO.Path.Combine(d,"Resources");
				if (!System.IO.Directory.Exists(d)) {
						UnityEditor.AssetDatabase.CreateFolder("Assets","Resources");
				}						
				d=System.IO.Path.Combine(d,"TTFText");
				if (!System.IO.Directory.Exists(d)) {
						UnityEditor.AssetDatabase.CreateFolder("Assets/Resources","TTFText");
				}			
				d=System.IO.Path.Combine(d,"Fonts");
				if (!System.IO.Directory.Exists(d)) {
						UnityEditor.AssetDatabase.CreateFolder("Assets/Resources/TTFText","Fonts");
				}
				f.BuildCharSet(f.fontid);
				
				UnityEditor.AssetDatabase.CreateAsset(TTFTextFontStoreFontAsset.CreateInstance<TTFTextFontStoreFontAsset>().Init(f),"Assets/Resources/TTFText/Fonts/"+f.fontid+".asset");
				Debug.Log("Font "+ f.fontid+ " has been embedded");
				}
#endif			
			}
		}
		
		return f;
	}
	
	public void RegisterClient(TTFText tm ) {
		if (! Clients.Contains(tm)) {
			Clients.Add(tm);
		}
		//EnsureFont(tm.FontId);
	}
	
	
	//
	public void UpdateClient(TTFText tm) {
		// may be used to update the necessary charmap of 
		// a font on the fly
		TTFTextFontStoreFont x= GetFont (tm.FontId);
		x.AddRequiredCharacters(x.additionalChar);
		if (x.needRebuild) {
			x.BuildCharSet(tm.FontId);
		}
		Debug.LogWarning("Not yet implemented");
	}
	
	
	
	
	public void UnregisterClient(TTFText tm ) {
		
		while (Clients.Contains(tm)) {
			Clients.Remove(tm);	
		}
		
		if (Clients.Count==0 && destroyWhenUnused) {
			if ((Application.isEditor)&&(!Application.isPlaying)) {
				GameObject.DestroyImmediate(gameObject);
			} else {
				GameObject.Destroy(gameObject);
			}
		}
	}

	
	bool started=false;
	
	
	// Use this for initialization
	public void Start () {	

		if (! started) {
		
			//Debug.Log("Font store started");
				
			GameObject ao = GameObject.Find("/" + STORE_NAME);

			if ((ao!=null)&&(ao!=gameObject)) {
				
				if ((Application.isEditor)&&(!Application.isPlaying)) {
					GameObject.DestroyImmediate(gameObject); // THERE SHOULD BE ONLY ONE TTFTextFontStore
				} else {
					GameObject.Destroy(gameObject); // THERE SHOULD BE ONLY ONE TTFTextFontStore				
				}
			
			} else {
				if (dontDestroyOnLoad) {
					GameObject.DontDestroyOnLoad(gameObject);
				}					
			}
			
			if (Clients==null) {
				Clients=new List<TTFText>();
			}
			if (embeddedFonts==null) {
				embeddedFonts=new List<TTFTextFontStoreFont>();
			}
			if (load_all_fonts_on_start) {
				LoadAllResourceFonts();
			}
				
			started=true;
		}
	}

	
	public void Update() {
		// CHECK NEED FOR GARBAGE COLLECTIONS
		if (needFGC>0) {
			for (int i=0;i<embeddedFonts.Count;) {
				if (embeddedFonts[i].GetRefCount()<=0) {
					embeddedFonts.RemoveAt(i);
				}
				else {
					i++;
				}
			}
			needFGC=0;
		}
	}

	public void ResetFontStore() {
				embeddedFonts=new List<TTFTextFontStoreFont>();
				foreach(TTFText client in Clients) {
					EnsureFont(client.FontId);
				}
				foreach(TTFText client in Clients) {
					TTFTextFontStoreFont f = GetFont(client.FontId);
					f.incref();
					if (f.charset==null) {
						f.BuildCharSet(client);
					}
				}
				
	}
	
	
	
	
	
	
	
	
#region Experimental Network Fonts
	
  // this is unfinished experimental code for futur network font support
	
#if false
	public bool NetworkFontEnabled=false;
	public string TTFTextServerUrl="http://ttftextserver.computerdreams.com:8000/";
	public string TTFTextServerUserKey="username";
	public string TTFTextServerAuthKey="password";

	private List<string> netFontList = null;

	public void UpdateFontList() {
		QueryServerForFontList(TTFTextServerUrl);
	}
	
	// QUERY SERVER FOR FONT LIST
	public void QueryServerForFontList( string url) {
		if (!url.EndsWith("/")) url=url+"/";
		GameObject go=new GameObject();
		go.transform.parent=transform;
		go.name="WWW Query";
		go.AddComponent<TTFTextFontClient>();
		TTFTextFontClient tfc=go.GetComponent<TTFTextFontClient>();
		tfc.url=url+"fontlist/list/json";
		tfc.notifiedObject=gameObject;
		tfc.signal="QueryServerForFontListReply";
		tfc.form=new WWWForm();
		tfc.form.AddField("userkey",TTFTextServerUserKey);
		tfc.form.AddField("authkey",TTFTextServerAuthKey);
	}
	
	public void QueryServerForFontListReply(string reply) {		
		JsonData fontList=JsonMapper.ToObject(reply);	
		netFontList.Clear();
		foreach(JsonData fontname in fontList) {
			Debug.Log((string)fontname);
			netFontList.Add((string)fontname);
		}
	}
	
	
	public void GetFontOutline( string url, string fontid, string characters) {
		// TODO: WE SHALL ONLY OUTPUT A QUERY IF NO SIMILAR QUERY IS RUNNING
		// AND ONLY IF THE MAX NUMBER OF SIMULTANEOUS QUERY HAS NOT BEEN REACHED
		
		if (!url.EndsWith("/")) url=url+"/";
		GameObject go=new GameObject();
		go.transform.parent=transform;
		go.name="WWW Query";
		go.AddComponent<TTFTextFontClient>();
		TTFTextFontClient tfc=go.GetComponent<TTFTextFontClient>();
		tfc.url=url+"outline/"+fontid+"/";
		tfc.form=new WWWForm();
		tfc.form.AddField("userkey",TTFTextServerUserKey);
		tfc.form.AddField("authkey",TTFTextServerAuthKey);		
		tfc.form.AddField("requestid",-1); // unused
		tfc.form.AddField("characters",characters);
		tfc.notifiedObject=gameObject;
		tfc.signal="QueryServerForFontListReply";
		
	}
	
	public void GetFontOutlineReply(string reply) {		
		JsonData fontoutlinereply=JsonMapper.ToObject(reply);	
		string fontid=(string)fontoutlinereply["fontid"];
		TTFTextFontStoreFont tfsf = GetDynamiclyLoadedFont(fontid);
		if (tfsf==null) {
			tfsf=new TTFTextFontStoreFont();
			tfsf.fontid=fontid;
		}
		
		/*
		
		byte[] outlinebytes = System.Convert.FromBase64String((string) fontoutlinereply["characters"]);
		MemoryStream ms=new MemoryStream(outlinebytes);
		BinaryReader br=new BinaryReader(ms);
		
        //tfsf.AddOutlines();
		Debug.Log("received reply");	
	*/
	}
	
	
	
	
	private IEnumerable DoSyncDownloadOutline(string fontid, string characters) {
			GetFontOutline(this.TTFTextServerUrl,fontid,characters);
			// we shall now wait .... for the download to be complete...
			TTFTextFontStoreFont f=null;
		
			for (int i=0;i<10;i++) {
				yield return new WaitForSeconds(1);
				foreach (TTFTextFontStoreFont cf in embeddedFonts) {
					if (cf.fontid==fontid) {
						f=cf;
						break;
					}
				}
				
			}
			if (f==null) {
					throw new System.Exception("TIME OUT ");
			}
			yield break;
	}
	
	
	// this version will block until all character are downloaded...
	public IEnumerable SyncGetOutlineForLetter(string fontid, string characters) {		
		TTFTextOutline []res = new TTFTextOutline[characters.Length];
		// for each character 
		
		TTFTextFontStoreFont f=GetFont(fontid);
		
		if (f==null) {
			    IEnumerable r=null;
				do {
					r=DoSyncDownloadOutline(fontid,f.additionalChar);
					if (r.GetType()==typeof(WaitForSeconds)) {
						yield return r; 
					}
				} while (r.GetType()==typeof(WaitForSeconds));
		}

		foreach(char c in characters) { 
			if (! f.HasCharacter(c)) {
				f.AddRequiredCharacters(characters);
				IEnumerable r=null;
				do {
					r=DoSyncDownloadOutline(fontid,f.additionalChar);
					if (r.GetType()==typeof(WaitForSeconds)) {
						yield return r; 
					}
				} while (r.GetType()==typeof(WaitForSeconds));
			}			
		}
		
		yield return res;
	}

#endif
#endregion	
	
#endif
}
