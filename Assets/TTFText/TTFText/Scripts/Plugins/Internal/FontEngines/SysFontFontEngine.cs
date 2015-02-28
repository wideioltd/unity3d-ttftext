using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !TTFTEXT_LITE

namespace TTFTextInternal {
	
public class SysFontFontEngine : IFontEngine{
	SysFontTexture tex;

	[System.Serializable]
	public class Parameters {
		public string [] fallbackFonts={"Droid Sans,Helvetica", "Droid Serif", "Droid Monospace"};
		public bool useFontSheets=false;
		public string shaderName="SysFont/Unlit Transparent";
		public float red=0;
		public float green=0;
		public float blue=0;
		public float alpha=1;
		public float scale=0.1f;
		public bool fixedWidth=true;
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
		
	public bool IsBitmapFontProvider(object parameters) {
		return true;
	}
	
	
	public class FontSelector {
		public string AppleFontName;
		public string AndroidFontName;
	}
	
	public Parameters parameters=new Parameters();	

	//public object DefaultFontSelector(object parameters, params string []xx) { return null;}
	
	public bool IsCompatible(UnityEngine.RuntimePlatform p) {
			return (p==RuntimePlatform.Android)||(p==RuntimePlatform.IPhonePlayer);
	}
	
	public object GetFontSelectorFromFontName(string s) {
		FontSelector fs2=new FontSelector();
		int splt=s.IndexOf('/');
		if (splt!=-1) {			
			fs2.AndroidFontName=s.Substring(0,splt);
			fs2.AppleFontName=s.Substring(splt+1);			
		}
		else {
			fs2.AndroidFontName=s;
			fs2.AppleFontName=s;
		}
		return fs2;
	}

	public bool IsConsideredNative(	UnityEngine.RuntimePlatform p) {
		return true;
	}
		
	public bool IsConsideredEmbedded(	UnityEngine.RuntimePlatform p) {
		return false;
	}

	
	public TTFTextTexturePortion ComputeGlyphBitmap(object parameters, object font, char c) {
		SysFontTexture tex=new SysFontTexture();
		FontSelector fs=(FontSelector)GetFontSelectorFromFontName((string) font);
		tex.AndroidFontName=fs.AndroidFontName;
		tex.AppleFontName=fs.AppleFontName;
		tex.Update();
			
      Texture2D _texture = new Texture2D(1, 1, TextureFormat.Alpha8, false);
	  //Texture2D _texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);			
      _texture.hideFlags = HideFlags.HideInInspector | HideFlags.DontSave;
      _texture.filterMode = FilterMode.Point;
      _texture.wrapMode = TextureWrapMode.Clamp;
   	 int textureID = _texture.GetNativeTextureID();

    SysFont.QueueTexture(""+c, 
#if UNITY_ANDROID			
			fs.AndroidFontName
#else
#if UNITY_IPHONE			
			fs.AppleFontName
#else
			((string)font)
#endif
#endif			
			,
			12, 
			false,//_isBold,
        	false,//_isItalic, 
			SysFont.Alignment.Left,//_alignment, 
			false, 			
			2048,
      		2048, 
			textureID
	);

    int _textWidthPixels  = SysFont.GetTextWidth(textureID);
    int _textHeightPixels = SysFont.GetTextHeight(textureID);
			
			
			
    SysFont.UpdateQueuedTexture(textureID);
    Debug.Log(".");			
			
	Parameters cp =parameters as Parameters;
	Material m=null;
	if (cp!=null) {
	  Shader shader=Shader.Find(cp.shaderName);
	  if (shader==null) {shader=Shader.Find("Mobile/Diffuse");}
 	  if (shader==null) {shader=Shader.Find("Diffuse");}			
	  m=new Material(shader);
	  m.color=new Color(cp.red,cp.green,cp.blue,cp.alpha);
	  m.mainTexture=_texture;
	}
	else {
      Shader shader=Shader.Find("SysFont/Unlit Transparent");
	  if (shader==null) {shader=Shader.Find("Mobile/Diffuse");}
 	  if (shader==null) {shader=Shader.Find("Diffuse");}							
	  m=new Material(shader);
	  m.color=Color.black;
	  m.mainTexture=_texture;				
	}
			
	TTFTextTexturePortion p=new TTFTextTexturePortion(
			m,
			0,0,1,1,
			_textWidthPixels * cp.scale,_textHeightPixels *cp.scale,
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
		return fontid;
	}
	
	public object GetFont(object parameters,string fontid, bool bold, bool italic) 
	{
		return fontid;
	}

	public object GetFont(object parameters,object xxx) 
	{		
		Debug.LogError("(TTFText) GETFONT : SYSFONTENGINE : Can't get the font from a parameter object");
		return null;
	}
	

	public TTFTextOutline GetGlyphOutline(object parameters,object font, char c) {
		Debug.LogError("(TTFText) GetGlyphOutline not supported by Sysfont");		
		return null;
	}
	
	public Vector3 GetAdvance(object parameters,object font, char c) {
		//return Vector3.zero;
		FontSelector fs =font as FontSelector;
		Parameters cp = parameters  as Parameters;
		if (cp.fixedWidth) {			
			return new Vector3(1,0,0);
		}
		else {
		
	      Texture2D _texture = new Texture2D(1, 1, TextureFormat.Alpha8, false);
    	  _texture.hideFlags = HideFlags.HideInInspector | HideFlags.DontSave;
     	  _texture.filterMode = FilterMode.Point;
     	  _texture.wrapMode = TextureWrapMode.Clamp;
   		  int textureID = _texture.GetNativeTextureID();

    	   SysFont.QueueTexture(""+c, 
#if UNITY_ANDROID			
			fs.AndroidFontName
#else
#if UNITY_IPHONE			
			fs.AppleFontName
#else
			((string)font)
#endif
#endif			
			,
			12, 
			false,//_isBold,
        	false,//_isItalic, 
			SysFont.Alignment.Left,//_alignment, 
			false, 			
			2048,
      		2048, 
			textureID
	       );

    		int _textWidthPixels  = SysFont.GetTextWidth(textureID);
//    		int _textHeightPixels = SysFont.GetTextHeight(textureID);
			GameObject.Destroy(_texture);
			return new Vector3( _textWidthPixels * cp.scale,0,0);		
		}
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