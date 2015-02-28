using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
	
namespace TTFTextInternal {

public class FreeTypeFontEngine : IFontEngine {
	[System.Serializable]
	public class Parameters {
		public bool showSystemFonts=true;
		public int interpolation=4;
		public string fallbackFonts="Arial (Regular);Helvetica (Regular);Times (Regular)";
		public bool bitmapFontMode=false;
		public int bitmapResolution=12;
		public string shader="Unlit/Transparent";
	}
	
	public Parameters parameters=new Parameters();
	
	public bool IsBitmapFontProvider(object parameters) {
		return (parameters as Parameters).bitmapFontMode;
	}
	
	public class Font {
		public bool reversed;
		public FTSharp.Font font;		
		public Font(FTSharp.Font f) {font=f; reversed=false;}
		public Font(FTSharp.Font f,bool r) {font=f; reversed=r;}
	}
		
	public bool IsConsideredNative(	UnityEngine.RuntimePlatform p) {
		return true;
	}
		
	public bool IsConsideredEmbedded(	UnityEngine.RuntimePlatform p) {
		return false;
	}
		
	
	public bool IsCompatible(UnityEngine.RuntimePlatform p) {
		if ((p==UnityEngine.RuntimePlatform.WindowsEditor) 
			||(p==UnityEngine.RuntimePlatform.WindowsPlayer)
			||(p==UnityEngine.RuntimePlatform.OSXEditor)
			||(p==UnityEngine.RuntimePlatform.OSXPlayer)
			//||(p==UnityEngine.RuntimePlatform.OSXDashboardPlayer)
			//||(p==UnityEngine.RuntimePlatform.LinuxPlayer)
			) {			
			return true;
		}
		
		return false;
	}
	
	

	
	public List<string> GetFontList(object parameters) {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX|| UNITY_EDITOR	
		List<string> r=new List<string>();
		foreach(var f in TTFTextFontListManager.Instance.LocalFonts) {
			r.Add(f.Value.Name);
		}
		if ((parameters==null)||((parameters as Parameters).showSystemFonts)) {
		foreach(var f in TTFTextFontListManager.Instance.SystemFonts) {
			r.Add(f.Value.Name);
		}
		}
		
		return r;		
#else  
		return null;
#endif		
	}
	

	public bool IsNativeFontImported(object parameters,string fontid) {
			return true;
	}
		
	public bool ImportNativeFont(object parameters,string fontid) {
			return true;
	}
	
	public bool DiscardNativeFont(object parameters,string fontid) {
			return true;
	}
		
	public object GetFont(object parameters,string fontid) {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX|| UNITY_EDITOR			
			
		Parameters p = parameters as Parameters;
		bool reversed=false;
		//if (fontid.Length==0) {
		//	return null;
		//}
		//FTSharp.Font f=TTFTextFontListManager.Instance.OpenFont(fontid,1,ref reversed,p.bitmapResolution);			
		FTSharp.Font f=TTFTextInternal.Utilities.TryOpenFont(fontid,ref reversed,p.fallbackFonts,p.bitmapResolution);	
		return new Font(f,reversed);
#else 
		return null;
#endif 		
		
	}
	
	
	public TTFTextOutline GetGlyphOutline(object parameters,object font, char c) {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX|| UNITY_EDITOR			
		Font fnt=(Font)font;		
		FTSharp.Outline.Point adv=new FTSharp.Outline.Point();
		FTSharp.Outline ol=fnt.font.GetGlyphOutline(c);
		
		if (fnt.reversed) {
			return new TTFTextOutline(ol,adv,true);
		}
		else {
			return new TTFTextOutline(ol,adv,false);
		}
#else
		return null;
#endif		
	}
	
	public TTFTextTexturePortion ComputeGlyphBitmap(object parameters, object font ,char c) {
		Parameters p= (parameters) as Parameters;
		
			Font fnt=(Font)font;		
			if ((fnt==null)||(fnt.font==null)) {
				Debug.LogError("(TTFText) No font provided");
			}
			try {
				Texture2D d=fnt.font.RenderIntoTexture(c);
				FTSharp.Outline.Point advance;
				FTSharp.BBox bb=fnt.font.Measure(c,out advance);			
				Material m=new Material(Shader.Find(p.shader));
				m.mainTexture=d;
				return new TTFTextTexturePortion(m,0,0,1,1,bb.xMax-bb.xMin,bb.yMax-bb.yMin,bb.xMin,bb.yMin,true);		
			}
			catch (System.Exception e) {
				Debug.Log(e);
				Texture2D t = new Texture2D(1,1,TextureFormat.ARGB32,false);
				return new TTFTextTexturePortion((Texture)t,0,0,1,1,0,0,0,0,true);		
			}
	}
			
	public TTFTextTexturePortion ComputeGlyphBitmapTex(object parameters, object font ,char c) {
		//Parameters p= (parameters) as Parameters;
		
			Font fnt=(Font)font;		
			if ((fnt==null)||(fnt.font==null)) {
				Debug.LogError("(TTFText) No font provided");
			}
			try {
			Texture2D d=fnt.font.RenderIntoTexture(c);
			FTSharp.Outline.Point advance;
			FTSharp.BBox bb=fnt.font.Measure(c,out advance);
			//Debug.Log (bb.yMin);
			return new TTFTextTexturePortion(d,0,0,1,1,bb.xMax-bb.xMin,bb.yMax-bb.yMin,bb.xMin,bb.yMin,true);		
			}
			catch (System.Exception e) {
				Debug.Log(e);
				Texture2D t = new Texture2D(1,1,TextureFormat.ARGB32,false);
				return new TTFTextTexturePortion((Texture)t,0,0,1,1,0,0,0,0,true);		
			}
	}
	
	public TTFTextTexturePortion GetGlyphBitmap(object parameters, object font, char c) {		
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX|| UNITY_EDITOR					
		TTFTextInternal.TextureMapManager fontsheet=
			TTFTextInternal.TextureMapManager.GetTextureMapManager(
				this.GetType().Name,
				((Font)font).font.FamilyName,
				(cc)=>(this.ComputeGlyphBitmapTex(parameters,font,cc)));		
		Parameters p= (parameters) as Parameters;		
		fontsheet.EnablePackedMaterial(p.shader);

		
		if (fontsheet.IsRequestedInMap(c)) {
			return fontsheet.GetChar(c);
		}
		else {
			return ComputeGlyphBitmap(parameters,font,c);	
		}
#else		
		return null;
#endif		
	}
	
	
	public Vector3 GetAdvance(object parameters,object font, char c) {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX|| UNITY_EDITOR			
		FTSharp.Font fnt=((Font) font).font;
		FTSharp.Outline.Point adv;
		// TODO: EXPORT SYMBOL TO GET ONLY ADV
		fnt.GetGlyphOutline(c,out adv);
		return new Vector3(adv.X,adv.Y,0);
#else
		return Vector3.zero;
#endif		
	}
	
	public float GetHeight(object parameters, object font) {
		FTSharp.Font fnt=((Font) font).font;
		return fnt.Height;
	}
	
	public void DisposeFont(object font) {
		FTSharp.Font fnt=((Font) font).font;
		if (fnt!=null) {
			fnt.Dispose();
		}
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