using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if ! TTFTEXT_LITE
namespace TTFTextInternal {
public class AndroidFontEngine : IFontEngine {
	[System.Serializable]
	public class Parameters {
		public float InterpolationStep=1;
		public string [] fallbackFonts={"Droid Sans", "Droid Serif", "Droid Monospace"};
	}
	public Parameters parameters=new Parameters();	
	
	
	public bool IsBitmapFontProvider(object parameters) {
		return false;
	}
	
	public bool IsConsideredNative(	UnityEngine.RuntimePlatform p) {
		return true;
	}
		
	public bool IsConsideredEmbedded(	UnityEngine.RuntimePlatform p) {
		return false;
	}
		
		
		
	//public object DefaultFontSelector(object parameters, params string []xx) { return null;}
	
	public bool IsCompatible(UnityEngine.RuntimePlatform p) {
			return (p==RuntimePlatform.Android);
	}
	
	public List<string> GetFontList(object parameters) {
#if UNITY_ANDROID && !UNITY_EDITOR		
		//return TTFTextInternalAndroid.Instance.GetTypeface(fontid,false,false);
		return TTFTextInternalAndroid.Instance.EnumFonts();
#else	
	   return new List<string>(new string []{"Droid Sans","Droid Serif","Droid Monospace"});
#endif		
		
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
			
	public object GetFont(object parameters,string fontid) {
#if UNITY_ANDROID		
		return TTFTextInternalAndroid.Instance.GetTypeface(fontid,false,false);
#else	
		return null;	
#endif		
	}
	
	public object GetFont(object parameters,string fontid, bool bold, bool italic) {
#if UNITY_ANDROID		
		return TTFTextInternalAndroid.Instance.GetTypeface(fontid,bold,italic);
#else
		return null;
#endif		
	}

	public object GetFont(object parameters,object xxx) {
		// NOT YET IMPLEMENTED
		return null;
	}
	

	public TTFTextTexturePortion GetGlyphBitmap(object parameters, object font, char c) {
		return null;
	}
	
	
	
	public TTFTextOutline GetGlyphOutline(object parameters,object font, char c) {
#if UNITY_ANDROID		
		return TTFTextInternalAndroid.Instance.GetGlyph(font as TTFTextInternalAndroid.Font,c);
#else 
		return null;
#endif 		
	}
	
	public Vector3 GetAdvance(object parameters,object font, char c) {
#if UNITY_ANDROID
	/*		
		TTFTextOutline ol2=GetGlyphOutline(parameters,font,c);
		Vector3 mi,ma;
		ol2.GetMinMax(out mi,out ma);
		float mw=(ma.x-mi.x);
			
		
		if (mw>a) {a=mw;}
	*/
		float a=TTFTextInternalAndroid.Instance.GetGlyphAdvance(font as TTFTextInternalAndroid.Font,c);	
			
			
		return new Vector3(a,0,0);
#else
		return Vector3.zero;
#endif		
	}

	
	public void DisposeFont(object font) {
#if UNITY_ANDROID		
		TTFTextInternalAndroid.Font  f=font as TTFTextInternalAndroid.Font ;
		f.Dispose();
#endif		
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