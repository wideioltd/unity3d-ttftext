#if UNITY_ENGINE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace TTFTextInternal {
	
public class UnityFontEngine : IFontEngine{
	

	[System.Serializable]
	public class Parameters {
	}
	
	public List<string> GetFontList(object parameters) {
					
	}
	
	public bool ImportNativeFont(object parameters,string fontid) {
			try {
				string ed=System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath,"Fonts"),"Export");

				if (!System.IO.Directory.Exists(ed)){
					System.IO.Directory.CreateDirectory(ed);
				}
				string f0=TTFTextFontListManager.Instance.GetFontInfo(fontid).Path;
				string f1=System.IO.Path.Combine(ed,System.IO.Path.GetFileName(f0));
				if (!System.IO.File.Exists(f1)) {
					System.IO.File.Copy(f0,f1);
				}
				return true;
			}
			catch (System.Exception e){
				Debug.LogError(e.ToString());
				return false;
			}
	}
			

	public bool IsNativeFontImported(object parameters,string fontid) {			
		string ed=System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath,"Fonts"),"Export");
			string f0=TTFTextFontListManager.Instance.GetFontInfo(fontid).Path;
		string f1=System.IO.Path.Combine(ed,System.IO.Path.GetFileName(f0));
		return System.IO.File.Exists(f1);
	}
		
		
	public bool DiscardNativeFont(object parameters,string fontid) {
			if (IsNativeFontImported(parameters,fontid)) {
				string ed=System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath,"Fonts"),"Export");
				string f0=TTFTextFontListManager.Instance.GetFontInfo(fontid).Path;
				string f1=System.IO.Path.Combine(ed,System.IO.Path.GetFileName(f0));
				System.IO.File.Delete(f1);				
			}
			return true;
	}		
		
	public bool IsBitmapFontProvider(object parameters) {
		return true;
	}
	
	
	
	public Parameters parameters=new Parameters();	

	//public object DefaultFontSelector(object parameters, params string []xx) { return null;}
	
	public bool IsCompatible(UnityEngine.RuntimePlatform p) {
			return true;
			//TextMesh tm;
			//tm.
	}
	
	public object GetFontSelectorFromFontName(string s) {
		return s;
	}

	public bool IsConsideredNative(	UnityEngine.RuntimePlatform p) {
		return true;
	}
		
	public bool IsConsideredEmbedded(	UnityEngine.RuntimePlatform p) {
		return true;
	}

	
	public TTFTextTexturePortion ComputeGlyphBitmap(object parameters, object font, char c) {
	TTFTextTexturePortion p=new TTFTextTexturePortion(
			_texture,
			0,0,1,1,
			_textWidthPixels,_textHeightPixels,
			0,0,
			true
			);
		return p;
	}
	
	public TTFTextTexturePortion GetGlyphBitmap(object parameters, object font, char c) {
		Parameters p= (parameters) as Parameters;		
		if (p.useFontSheets) {
		TTFTextInternal.TextureMapManager fontsheet=
			TTFTextInternal.TextureMapManager.GetTextureMapManager(
				this.GetType().Name,
				((string)font),
				(cc)=>(this.ComputeGlyphBitmap(parameters,font,cc))
				);		
		
		//fontsheet.EnablePackedMaterial(p.shader);

		
		if (fontsheet.IsRequestedInMap(c)) {
			return fontsheet.GetChar(c);
		}
		else {
			return ComputeGlyphBitmap(parameters,font,c);	
		}
		}
		else {
			return ComputeGlyphBitmap(parameters,font,c);	
		}
	}
	
	
	
	
	public void DisposeFont(object font) {
	}
	

	
	public object GetFont(object parameters,string fontid) {
#if UNITY_ANDROID		
		return null;
#else	
		return null;	
#endif		
	}
	
	public object GetFont(object parameters,string fontid, bool bold, bool italic) {
#if UNITY_ANDROID		
		return null;
#else
		return null;
#endif		
	}

	public object GetFont(object parameters,object xxx) {
		return null;
	}
	

	
	
	public TTFTextOutline GetGlyphOutline(object parameters,object font, char c) {
			return null;
	}
	
	public Vector3 GetAdvance(object parameters,object font, char c) {
			return null;
		}

	
	
	public float GetHeight(object parameters, object font) {		
		return 1;
	}
	
	
	public void RegisterClient(TTFText t) {
	}
	public void UpdateClient(TTFText t) {
	}	
	public void UnregisterClient(TTFText t) {
	}
	
	public void IncRef(object parameters, string fontid) {
	}	
	public void DecRef(object parameters, string fontid) {
	}
	
}
}

#endif