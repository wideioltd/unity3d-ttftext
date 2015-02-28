using UnityEngine;
using System.Collections;

	
	
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
