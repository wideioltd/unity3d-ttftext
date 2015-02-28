// ApplicationWideTTFManager.cs

// 
//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   



using UnityEngine;
using System.IO;
using System.Collections.Generic;
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
using TTF =FTSharp;
#endif

public class TTFontInfo {
	
	public string Name;
	public string Path;
	
	public TTFontInfo(string path) {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
		
		Path = path;
		using (TTF.Font f = new TTF.Font(path)) {
			
			Name = f.Name;
		}
#endif		
	}
	
}

/// <summary>
/// TTF text font list manager.
/// This is a singleton class that
/// enumerates fonts based on freetype.
/// </summary>
public class TTFTextFontListManager {
	public string lastImportSrc = System.Environment.CurrentDirectory;
	
	private static readonly TTFTextFontListManager instance_ = new TTFTextFontListManager();
	
	public static TTFTextFontListManager Instance {
		get {
			return instance_;
		}
	}
	
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR
	string[] FONT_EXTS = { ".ttf", ".ttc", ".otf", ".pfa",".pfb",".pfm", ".afm" };
	private TTFTextFontListManager() {
		UpdateAll(false);
	}
	
	
	public Dictionary<string, TTFontInfo> SystemFonts = new Dictionary<string, TTFontInfo>();
	public Dictionary<string, TTFontInfo> LocalFonts = new Dictionary<string, TTFontInfo>();


    public int Count {
        get { return SystemFonts.Count + LocalFonts.Count; }
    }

	public void UpdateAll(bool verbose) {
		UpdateSystemFonts(verbose);
		UpdateLocalFonts(verbose);
	}
	
	public void UpdateSystemFonts(bool verbose) {
		
		List<string> fontPaths = new List<string>();
		
		/*
		 	OSXEditor,
 	OSXPlayer,
 	WindowsPlayer,
 	OSXWebPlayer,
 	OSXDashboardPlayer,
 	WindowsWebPlayer,
 	WiiPlayer,
 	WindowsEditor,
 	IPhonePlayer,
 	XBOX360,
 	PS3,
 	Android,
 	NaCl,
 	LinuxPlayer,
 	FlashPlayer
 	
			*/
		if ((Application.platform==RuntimePlatform.OSXEditor)||(Application.platform==RuntimePlatform.OSXPlayer)||(Application.platform==RuntimePlatform.OSXDashboardPlayer)) {			
			fontPaths.Add("/Library/Fonts");
			fontPaths.Add("/System/Library/Fonts");
			fontPaths.Add("/System/Fonts");
		}
		if ((Application.platform==RuntimePlatform.WindowsEditor)||(Application.platform==RuntimePlatform.WindowsPlayer)) {
			DirectoryInfo winDir = Directory.GetParent(System.Environment.GetFolderPath(System.Environment.SpecialFolder.System));
			fontPaths.Add(winDir.FullName + "/Fonts");
		}
		
		SystemFonts.Clear();
		
		foreach (string dir in fontPaths) {
			try {
					foreach (string f in System.IO.Directory.GetFiles(dir)) {
						bool validext=false;
						string ext=f.Substring(f.Length-4,4).ToLower();
						foreach(string kext in FONT_EXTS) {if (kext==ext) {validext=true; break;}}
						if (validext) {
							try {
								TTFontInfo fid = new TTFontInfo(f);
								SystemFonts[fid.Name] = fid;
							} catch (System.Exception) {}
						}
				}
			} catch (System.Exception) {} // Directory not found
		}
		
		if (verbose) {
			foreach (string id in  SystemFonts.Keys) {
				Debug.Log(". Found '" + id + "' in " + SystemFonts[id].Path);
			}
			Debug.Log(SystemFonts.Count + " external TTF fonts found.");
		}
	}
	
	
	public void UpdateLocalFonts() { UpdateLocalFonts(false); }
	
	public void UpdateLocalFonts(bool verbose) {
		
		LocalFonts.Clear();
		
		
			
			foreach (string f in GetFontFilesRec(Application.dataPath)) {
				try {
					TTFontInfo fid = new TTFontInfo(f);
					LocalFonts[fid.Name] = fid;
				} catch (System.Exception ex) {
					Debug.LogWarning("Unable to read font file " + f + ":" + ex.ToString());
				}
			}
		
		if (verbose) {
			foreach (string id in  LocalFonts.Keys) {
				Debug.Log(". Found '" + id + "' in " + LocalFonts[id].Path);
			}
			Debug.Log(LocalFonts.Count + " asset TTF fonts found.");
		}
	}
	
	
	IEnumerable<string> GetFontFilesRec(string path) {
		foreach (string f in System.IO.Directory.GetFiles(path)) {
			bool validext=false;
			string ext=f.Substring(f.Length-4,4).ToLower();
			foreach(string kext in FONT_EXTS) {if (kext==ext) {validext=true; break;}}
			if (validext) {

				yield return f;
			}
		}
		
		foreach (string dir in System.IO.Directory.GetDirectories(path)) {
			foreach (string f in GetFontFilesRec(dir)) {
				yield return f;
			}
		}
	}


    // Return One valid FontId from the available fonts list
    // try search for a  local font first
    // useful to initiate fontId member for a new TTFTextMesh object
    public string GetOneId() {

        IEnumerator<string> ie = LocalFonts.Keys.GetEnumerator();
        if (ie.MoveNext()) { return ie.Current; }

        ie = SystemFonts.Keys.GetEnumerator();
        if (ie.MoveNext()) { return ie.Current; }

        return "";
    }


    // Open a font by its id, return null if the font is not found
    // When no more in use, the font should be disposed with the font.Dispose() method
    public TTF.Font OpenFont(string fontId, float size, ref bool reversed, int res ) {

        TTFontInfo fontInfo = GetFontInfo(fontId);
        if (fontInfo == null) { return null; }
		
		if (fontInfo.Path.EndsWith("otf",System.StringComparison.CurrentCultureIgnoreCase)) {
			reversed=true;
		}
		else {
			reversed=false;
		}
		
        try {

            return  new TTF.Font(fontInfo.Path, size,res);

        } catch (System.Exception ex) {
            Debug.LogError("(TTFText) Unexpected error with font '" + fontId + "':" + ex.ToString());
        }

        return null;
    }

#endif
	
	
	public TTFontInfo GetFontInfo(string fontID) {

#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR	
        if (LocalFonts.ContainsKey(fontID)) { return LocalFonts[fontID]; }
		if (SystemFonts.ContainsKey(fontID)){ return SystemFonts[fontID]; }

#endif
		Debug.LogWarning("TTF '" + fontID + "' not found.");		
		return null;
	}

    public bool HasFont(string fontID) {
        return GetFontInfo(fontID) != null;
    }
	
}

