using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace TTFTextInternal {
#if ! TTFTEXT_LITE
public class FontStoreFontEngine: IFontEngine  {
	[System.Serializable] 
	public class Parameters {
		public bool showOnlyAlreadyEmbeddedFonts=false;
		public bool localMode; // local or global fonts ?
		public string additionalCharacters;
	}
	public Parameters parameters=new Parameters();
	
	public bool IsConsideredNative(	UnityEngine.RuntimePlatform p) {
		return false;
	}
		
	public bool IsConsideredEmbedded(	UnityEngine.RuntimePlatform p) {
		return true;
	}
	
	public bool IsBitmapFontProvider(object parameters) {
		return false;
	}
	
		
	public bool ImportNativeFont(object parameters,string fontid) {
			return true;
	}
		
	public bool IsNativeFontImported(object parameters,string fontid) {
			return false;
	}

	public bool DiscardNativeFont(object parameters,string fontid) {
			return false;
	}
		
		
	static TTFTextFontStore _fontstore;
	static TTFTextFontStore fontstore {
		get { if (_fontstore==null) {
				_fontstore=TTFTextFontStore.Instance;
			}
			return _fontstore;
		}
	}
	
	public bool IsCompatible(UnityEngine.RuntimePlatform p) {
			return true;
	}
	
	public List<string> GetFontList(object parameters) {		
		//TTFTextFontStore.Instance
		System.Collections.Generic.List<string> r=new System.Collections.Generic.List<string>();
		if (TTFTextFontStore.IsInstanciated) {
			foreach(TTFTextFontStoreFont f in  TTFTextFontStore.Instance.embeddedFonts) {
				r.Add(f.fontid);
			}
		}
		if (!(parameters as Parameters).showOnlyAlreadyEmbeddedFonts) {
			r.AddRange(TTFTextFontEngine.font_engines[0].GetFontList(null));
		}
		return r;
	}

	
	/*
	public object DefaultFontSelector(object parameters,params string [] strings) {
		return null;
	}
	*/
	
	public object GetFont(object parameters,string fontid) {
		return fontstore.GetFont(fontid);
	}
	
	/*
	public object GetFont(object parameters,object fontselector) {
		return fontstore.GetFont(fontselector as string);
	}
	*/
	
	public object GetFont(object parameters,string fontid, bool bold, bool italic) {
		return fontstore.GetFont(fontid);
	}
	
	public TTFTextOutline GetGlyphOutline(object parameters,object font, char c) {
		return new TTFTextOutline(((TTFTextFontStoreFont)font).GetGlyph(c));
	}
	
	
	public TTFTextTexturePortion GetGlyphBitmap(object parameters, object font, char c) {
		return null;
	}
	
	public Vector3 GetAdvance(object parameters,object font, char c) {
		return ((TTFTextFontStoreFont)font).GetAdvance(c);
	}

	public float GetHeight(object parameters, object font) {
		TTFTextFontStoreFont fnt=(TTFTextFontStoreFont) font;
		return fnt.height;
	}
	
	
	public void DisposeFont(object font) {
	}
	public void RegisterClient(TTFText t) {
		TTFTextFontStore store=TTFTextFontStore.Instance;
		store.RegisterClient(t);
	}
	public void UpdateClient(TTFText t) {
		TTFTextFontStore store=TTFTextFontStore.Instance;
		store.UpdateClient(t);
	}
	
	public void UnregisterClient(TTFText t) {
		TTFTextFontStore store=TTFTextFontStore.Instance;
		store.UnregisterClient(t);
	}
	
	public void IncRef(object parameters, string fontid) {
		TTFTextFontStoreFont fnt=TTFTextFontStore.Instance.GetEmbeddedFont(fontid);
		if (fnt==null) {
			fnt=TTFTextFontStore.Instance.EnsureFont(fontid);
			fnt.BuildCharSet(fontid);
		}
		fnt.incref();
	}	
	public void DecRef(object parameters, string fontid) {
		TTFTextFontStoreFont fnt=TTFTextFontStore.Instance.GetEmbeddedFont(fontid);
		if (fnt!=null) {
			fnt.decref();
			TTFTextFontStore.Instance.SetGarbageCollectUnusedFonts();
		}
		
	}
		
}

#endif
}
