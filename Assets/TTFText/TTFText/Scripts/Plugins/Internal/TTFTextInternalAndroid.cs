using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TTFTextInternalAndroid {

#if UNITY_ANDROID  		
  private static  TTFTextInternalAndroid _Instance = null;

  private AndroidJavaClass AndroidCSysFont;	
  private AndroidJavaObject AndroidISysFont=null;

  public AndroidJavaClass unityPlayer;
 
  public TTFTextInternalAndroid()  {
		Debug.Log("JNI Attach"+AndroidJNI.AttachCurrentThread());

		unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidCSysFont=new AndroidJavaClass("com.computerdreams.ttftext.androidsysfont.AndroidSysFont");
		if (AndroidCSysFont==null) {
			Debug.Log("Failed to get the class AndroidSysFont");
		}
		
		AndroidISysFont = AndroidCSysFont.CallStatic<AndroidJavaObject>("GetInstance");
		if (AndroidISysFont==null) {
			Debug.Log("Using constructor");
			AndroidISysFont=new AndroidJavaObject("com.computerdreams.ttftext.androidsysfont.AndroidSysFont");			
		}
		
		if (AndroidISysFont==null) {			
			Debug.Log("Failed to create the AndroidSysFont Instance");
		}
		
		Debug.Log("Android ISysfont Initialized");
  }
	
	
	
	
	
  public static  TTFTextInternalAndroid Instance
  {
    get
    {
      if (_Instance == null)
      {		
		
		_Instance=new TTFTextInternalAndroid();
      }
      return _Instance;
    }
 
  }
	
	
	
	
  public class Font {
		public AndroidJavaObject typeface;
		public Font(AndroidJavaObject jo) {typeface=jo;}
		public void Dispose() {
			typeface.Dispose();
			AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
		}
  }
	
	
	
	
  public Font GetTypeface(string tf, bool bold, bool italic) {
		AndroidJNI.PushLocalFrame(0);
		return new Font(AndroidISysFont.Call<AndroidJavaObject>("GetTypeface",tf,bold,italic));
  }
	
	
	
	
  public float GetGlyphAdvance(Font f, char c) {
		 return AndroidISysFont.Call<float>("GetGlyphWidth", f.typeface, ""+c);
  }
	
	
	
  public TTFTextOutline GetGlyph(Font f, char c) {
		AndroidJNI.PushLocalFrame(0);
		AndroidJavaObject path=AndroidISysFont.Call<AndroidJavaObject>("GetGlyph",
					f.typeface,
				    ""+c
				);
		AndroidJavaObject pm=
			AndroidISysFont.Call<AndroidJavaObject>("GetPathMeasure",
				path
			);
		
		TTFTextOutline tto=new TTFTextOutline();
		bool cont=true ; 
		while (cont) {
			List<Vector3> tb=new List<Vector3>();

			float [] res=AndroidISysFont.Call<float []>("GetBoundary",pm,0);
			int rl2=res.Length/2;
			for (int ci=0;ci<rl2;ci++) {
				tb.Add(new Vector3(res[ci*2],res[ci*2+1],0));
			}
			tto.AddBoundary(tb);
			cont=AndroidISysFont.Call<bool>("NextBoundary",pm);
		}
		
		path.Dispose();
		pm.Dispose();
		AndroidJNI.PopLocalFrame(System.IntPtr.Zero);
		return tto;
  }
	
	
	
  public List<string> EnumFonts() {
		return new List<string>();
		/*
		try {
			return new List<string>(AndroidISysFont.Call<string[]>("EnumFonts"));
		}
		catch (System.Exception e){
			Debug.LogError("Error while fetching font list"+e);
			return new List<string>();
		}
		*/
  }
 
#endif
}
