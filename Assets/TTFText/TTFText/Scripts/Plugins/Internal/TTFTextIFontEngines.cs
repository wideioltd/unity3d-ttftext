#define ANDROIDNATIVE_READY			
using UnityEngine;
using System.Collections.Generic;


namespace TTFTextInternal {
/// <summary>
/// TTF text texture portion is used to indicate which portion of 
/// a fontsheet must be used when rendering a character
/// that comes from a bitmap fontsheet or  a specific bitmap
/// </summary>
public class TTFTextTexturePortion {
	public bool shouldReleaseMaterial;
	public Material material;
	public bool shouldReleaseTexture;
	public Texture texture;
	
	public float sx=0;
	public float sy=0;
	public float dx=1;
	public float dy=1;
	public float w=-1;
	public float h=-1;
	public float x=0;
	public float y=0;
		
		
	public TTFTextTexturePortion(Texture tex, float _sx, float _sy, float _dx, float _dy, float _w, float _h,float _x,float _y,bool sr) {
		//material=mat;
		shouldReleaseMaterial=false;
		material=null;
		shouldReleaseTexture=sr;
		texture=tex;
		sx=_sx;sy=_sy;
		dx=_dx;dy=_dy;
		w=_w;h=_h;
		x=_x;y=_y;
	}
		
		
	public TTFTextTexturePortion(Material m, float _sx, float _sy, float _dx, float _dy, float _w, float _h,float _x, float _y,bool sr) {
		
		shouldReleaseMaterial=sr;
		material=m;
		shouldReleaseTexture=sr;
		texture=m.mainTexture;
		sx=_sx;sy=_sy;
		dx=_dx;dy=_dy;
		w=_w;h=_h;
		x=_x;y=_y;
	}
	
}


	 /// <summary>
/// Font providers are interface to access fonts independently of the platform.
/// each font implementation of a font provider will have its own limitation.
/// 
/// The system assumes the following :
///     * obtaining recently closed font is not notably slow.
///     * each font engine have specific parameters
///     * some fonts engine may require to track the usage of fonts
///     * the engine has to specify who is in charge of managing the memory
/// 
/// </summary>
	public interface IFontEngine {
	// returns a list of font selector valid for this platform
	// this list may not be exhaustive ... for simplicity font selectors 
	// have to be string		
	List<string> GetFontList(object parameters);
	
	
	/// <summary>
	/// Check whether the engine is meant to be compatible with platform we are running on
	/// </summary>
	bool IsCompatible(UnityEngine.RuntimePlatform p);
	
	bool IsConsideredNative(	UnityEngine.RuntimePlatform p);
	bool IsConsideredEmbedded(	UnityEngine.RuntimePlatform p);		
		

	/// <summary>
	/// Make a native font accessible to another engine
	/// </summary>	
	bool IsNativeFontImported(object parameters,string fontid);
	/// <summary>
	/// Make a native font accessible to another engine
	/// </summary>	
	bool ImportNativeFont(object parameters,string fontid);
	/// <summary>
	/// Remove a native font from the set of fonts accesible by other engines
	/// </summary>	
	bool DiscardNativeFont(object parameters,string fontid);
		
		
	/// <summary>
	/// returns a font object that will allow the engine to
	/// efficiently perform rendering of the font afterwards
	/// </summary>	
	object GetFont(object parameters,string fontid);
    
	/// <summary>
	/// Disposes a font after the end of its use in rendering.
	/// </summary>
	void DisposeFont(object font);
	
	/// <summary>
	/// Determines whether this instance is a bitmap font provider for the specified parameters.
	/// </summary>
	bool IsBitmapFontProvider(object parameters);
	
	
	/// <summary>
	/// Gets the glyph outline.
	/// </summary>
	TTFTextOutline GetGlyphOutline(object parameters,object font,char c);
		
	/// <summary>
	/// Gets the glyph bitmap.
	/// </summary>	
	TTFTextTexturePortion GetGlyphBitmap(object parameters, object font, char c);
	Vector3 GetAdvance(object parameters,object font, char c);
	float GetHeight(object parameters, object font);
	
	
	
	void RegisterClient(TTFText t);
	void UpdateClient(TTFText t); // call this to inform the cache that the text/charset has been updated
	void UnregisterClient(TTFText t);
	void IncRef(object parameters, string fontid);
	void DecRef(object parameters, string fontid);
	
}


/// <summary>
/// Bitmap font provider based on Unity Sysfont.
/// </summary>

/*
public class PolyFontProvider : FontProvider {
	public class Parameters {
		
	}
	public Parameters parameters=new Parameters();	
	
	public class Font {
		public object fontselector;
		private object [] font;
		private int builtfont=-1;
			
		// ---------------------------------------
		public Font(object fs) {fontselector=fs; object [] cfs=(object []) fontselector; font =new object[cfs.Length];} 
	
		public Font(object cfont, int id) {
			builtfont=id;
			font =new object[subproviders.Length];
			font[id]=cfont;
		}
		
		public object GetFont(int i) {
			object [] fs=(object []) fontselector;
			if (font[i]==null) {
				font[i]=subproviders[i].GetFont(fs[i]);
			}
			return font[i];
		}
		
		public object GetFont() {
			if (builtfont!=-1) {
				return font[builtfont];
			}
			for (int i=0;i<subproviders.Length;i++) {
				if (GetFont(i)!=null) {
					builtfont=i;
					return font[i];
				}
			}
			return null;
		}
		
		public int FontProviderId() {return builtfont;}
	}
	
	static FontProvider [] subproviders;	
	public PolyFontProvider(FontProvider []  cp ) {
		subproviders=cp;
	}
	
	
	
	public object DefaultFontSelector( params string [] xx) {
		object [] sels = new object[subproviders.Length];
		for (int i=0;i<sels.Length;i++) {
		  sels[i]=subproviders[i].DefaultFontSelector();
		}		
		return sels;
	}
	
	public bool IsCompatible(UnityEngine.RuntimePlatform p) {
			return true;
	}
	
	public List<string> GetFontList() {
		return null;
	}
	
	public object GetFont(string fontid) {
		for (int i=0;i <subproviders.Length;i++) {
			try {
			  object f=subproviders[i].GetFont(fontid);
			  if (f!=null) {
					return new Font(f,i);
			  }
			}
			catch {}
		}
		return null;
	}
	
	public object GetFont(string fontid, bool bold, bool italic) {
		return null;
	}

	public object GetFont(object o) {
		return new Font(o );
	}

	
	public TTFTextOutline GetGlyphOutline(object font, char c) {
		object f;
		Font F=(font as Font);
		f=F.GetFont();
		return subproviders[F.FontProviderId()].GetGlyphOutline(f,c);
	}
	
	public Vector3 GetAdvance(object font, char c) {
		object f;
		Font F=(font as Font);
		f=F.GetFont();
		return subproviders[F.FontProviderId()].GetAdvance(f,c);
	}
	
}

 */






#region DECLARATION_OF_THE_FONT_PROVIDER_ARRAY

public class TTFTextFontEngine {
	
	
	static IFontEngine ftfp=new FreeTypeFontEngine() as IFontEngine;
#if !TTFTEXT_LITE	
	static IFontEngine fsfp=new FontStoreFontEngine() as IFontEngine;
	static IFontEngine afp= new AndroidFontEngine() as IFontEngine;
	static IFontEngine sfp= new SysFontFontEngine() as IFontEngine;	
	
	
//#endif
	//static FontProvider pfp= new PolyFontProvider(new FontProvider [] {ftfp,fsfp,afp}) as FontProvider;
#endif	
	static public IFontEngine [] font_engines = 
		new IFontEngine [] {
		    ftfp
#if !TTFTEXT_LITE		
		    ,fsfp
		    ,afp
			,sfp
#endif		
		};
}

#endregion
	
}