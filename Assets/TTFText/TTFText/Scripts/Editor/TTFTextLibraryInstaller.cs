//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

// Keep tracks of all ttf fonts presents in asset directories and system directories
// Also ensures the proper installation of the system on different platforms
// This is manager is aimed at being used in the editor, see ApplicationWideTTFManager for the one used for dynamic texts
[InitializeOnLoad]
public class TTFTextLibraryInstaller /*: ScriptableObject*/ {
	
	// keep track of source and destination directories between two font importations
	public static string lastImportSrc = System.Environment.CurrentDirectory;
	public static string lastImportDest = Application.dataPath;
	
	
	
	
#region SYSTEM_CHECKS	
	
	public static bool OnMac {
		get {
			System.PlatformID id = System.Environment.OSVersion.Platform;
			return id == System.PlatformID.MacOSX || id == System.PlatformID.Unix;
		}
	}
	
	public static bool OnWin {
		get {
			System.PlatformID id = System.Environment.OSVersion.Platform;
			return id == System.PlatformID.WinCE || id == System.PlatformID.Win32Windows || id == System.PlatformID.Win32S || id == System.PlatformID.Win32NT;
		}
	}

	// Get rid of "value never used" warnings
	public static void Unused(System.Object o) {}
	
	
	private static bool CheckForLibsOnMac() {
		string ftpath = System.Environment.CurrentDirectory + "/" + "libfreetype248.so";
		string glutpath = System.Environment.CurrentDirectory + "/" + "libglu32.dll.so";
#if UNITY_EDITOR		
		InstallIOSPluginInProject();
		InstallAndroidPluginInProject();
#endif		
		return System.IO.File.Exists(ftpath) && System.IO.File.Exists(glutpath);
	}
	
	private static bool CheckForLibsOnWin() {
		string path = System.Environment.CurrentDirectory + "/" + "freetype248.dll";
		InstallAndroidPluginInProject();
		return System.IO.File.Exists(path);
	}
	
	// Checks if freetype dll is accessible
	// Check for file do not by try-catch dllimport
	// so that we can try checking, copying and dllimporting in the same run
	public static bool CheckForFreetypeDll() {
		if (OnWin) {
			return CheckForLibsOnWin();
		} else if (OnMac)
		{
			return CheckForLibsOnMac();
		} else {
			Debug.LogWarning("Unsuported platform: " + System.Environment.OSVersion.Platform);
			return true;
		}
	}

#endregion

	
	
	
	
	/*
	 * Not working anymore since libfreetype has been removed from 
	 * standard distribution of OSX
	 * 
	static bool InstallFreetypeOnMac() {
		
		try {
			string ftdylib="";
			string [] ftdylibs  = new string[]{ 
				"/usr/X11/lib/libfreetype.dylib", 
				"/usr/X11/lib/libfreetype.6.dylib"
			};
		
			bool found=false;
			
			foreach(string s in ftdylibs) {
				ftdylib=s;
				if (System.IO.File.Exists(s)) { 
					found=true;
					break;
				}
			}
			
			if (!found) {
				Debug.LogError("freetype not found on your system (please contact us - so that we can avoid this to rehappen).");
				return false; 
			}
			
			// Try copy dylib in Project Root Dir ...
			System.IO.File.Copy(ftdylib, System.Environment.CurrentDirectory + "/" + "libfreetype248.so", true);
			
			
			
			const string glutp =  "/usr/X11/lib/libglut.dylib";
		
			if (! System.IO.File.Exists(glutp)) { 
				Debug.LogError("'" + glutp + "' not found.");
				return false; 
			}
	
			// Try copy dylib in Project Root Dir ...
			System.IO.File.Copy(glutp, System.Environment.CurrentDirectory + "/" + "libglu32.dll.so", true);
			
		
		} catch (System.Exception ex) {
			
			
			Debug.LogError("Error: " + ex.ToString());
			return false;
		}
		
		return true;
	}
	*/
	
	
	
	static bool InstallIOSPluginInProject() {
#if !TTFTEXT_LITE		
#if UNITY_EDITOR		
		string sysfontmodule = "Plugins/iOS/UnitySysFont.mm";
		try {
			if (!System.IO.File.Exists(
				Application.dataPath+"/"+sysfontmodule)) {
				
				if (!
					System.IO.Directory.Exists(
						System.IO.Path.GetDirectoryName(
							Application.dataPath+"/"+sysfontmodule)
								)) {
					if (!System.IO.Directory.Exists(Application.dataPath+"/Plugins")) {
						System.IO.Directory.CreateDirectory(Application.dataPath+"/Plugins");
					}
					Debug.Log("Creating iOS plugin directory ");
					System.IO.Directory.CreateDirectory(
						System.IO.Path.GetDirectoryName(
							Application.dataPath+"/"+sysfontmodule)
						);
				}
			string src;
				
			string[] files = Directory.GetFiles(Application.dataPath, "UnitySysFont.mm", SearchOption.AllDirectories);
				if (files.Length == 0) {
					Debug.LogError("(TTFText) 'UnitySysFont.mm' not found.");
					return false;
				}

				src = files[0];
            	
			
			// Try copy freetype248 in Project Root Dir ...
		
			string dest = Application.dataPath+"/"+sysfontmodule;
			
            
			Debug.Log(src);
			Debug.Log(dest);
			System.IO.File.Copy(src, dest, true);
			}	
		} catch (System.Exception ex) {
			Debug.LogError("(TTF Text)Error:" + ex.ToString());
			return false;
		}
		
		
#endif
#endif
		return true;
	}
	
	static bool InstallAndroidPluginInProject() {
#if !TTFTEXT_LITE		
#if UNITY_EDITOR		
		string AndroidSFModule = "Plugins/Android/NormalSysFont.jar";
		string AndroidSFModule2 = "Plugins/Android/TTFTextAndroidSysFont.jar";
		try {
			if (!System.IO.File.Exists(
				Application.dataPath+"/"+AndroidSFModule)) {
				
				if (!
					System.IO.Directory.Exists(
						System.IO.Path.GetDirectoryName(
							Application.dataPath+"/"+AndroidSFModule)
								)) {
					if (!System.IO.Directory.Exists(Application.dataPath+"/Plugins")) {
						System.IO.Directory.CreateDirectory(Application.dataPath+"/Plugins");
					}
					Debug.Log("Creating Android plugin directory ");
					System.IO.Directory.CreateDirectory(
						System.IO.Path.GetDirectoryName(
							Application.dataPath+"/"+AndroidSFModule)
						);
				}
				
			if (true) {
			string src;				
			string[] files = Directory.GetFiles(Application.dataPath, Path.GetFileName(AndroidSFModule), SearchOption.AllDirectories);
				if (files.Length == 0) {
					Debug.LogError("(TTFText) 'NormalSysFont.jar' not found.");
					return false;
				}

				src = files[0];
            	
		
			string dest = Application.dataPath+"/"+AndroidSFModule;			            
			System.IO.File.Copy(src, dest, true);
			}
				
			if (true) {
			string src;				
			string[] files = Directory.GetFiles(Application.dataPath, Path.GetFileName(AndroidSFModule2), SearchOption.AllDirectories);
				if (files.Length == 0) {
					Debug.LogError("(TTFText) 'NormalSysFont.jar' not found.");
					return false;
				}

				src = files[0];
            	
		
			string dest = Application.dataPath+"/"+AndroidSFModule2;			            
			System.IO.File.Copy(src, dest, true);
					AssetDatabase.Refresh();
			}
				
			}	
		} catch (System.Exception ex) {
			Debug.LogError("Error:" + ex.ToString());
			return false;
		}
		
		
#endif
#endif		
		return true;
	}
	
	static bool InstallFreetypeOnMac() {
		
		string freetype248path = "TTFText/Libraries/Win32/libfreetype248.so";
		string glu32path = "TTFText/Libraries/Win32/libglu32.dll.so";
		
		
		
		try {
			
			// Try copy freetype248 in Project Root Dir ...
		
			string src = Application.dataPath + "/" + freetype248path;
			string dest = System.Environment.CurrentDirectory + "/" + "libfreetype248.so";
			
            if (! System.IO.File.Exists(src)) {

				string[] files = Directory.GetFiles(Application.dataPath, "libfreetype248.so", SearchOption.AllDirectories);

				if (files.Length == 0) {
					Debug.LogError("(TTFText) '" + src + "' not found.");
					return false;
				}

				src = files[0];
            }
			
			
			System.IO.File.Copy(src, dest, true);
			
		} catch (System.Exception ex) {
			Debug.LogError("(TTFText) Error:" + ex.ToString());
			return false;
		}
		
		try {
			
			// Try copy freetype248 in Project Root Dir ...
		
			string src = Application.dataPath + "/" + glu32path;
			string dest = System.Environment.CurrentDirectory + "/" + "libglu32.dll.so";
			
            if (! System.IO.File.Exists(src)) {

				string[] files = Directory.GetFiles(Application.dataPath, "libglu32.dll.so", SearchOption.AllDirectories);

				if (files.Length == 0) {
					Debug.LogError("'" + src + "' not found.");
					return false;
				}

				src = files[0];
            }
			
			
			System.IO.File.Copy(src, dest, true);
			
		} catch (System.Exception ex) {
			Debug.LogError("Error:" + ex.ToString());
			return false;
		}
		
		return true;
	}
	
	
	
	
	static bool InstallFreetypeOnWin() {
		
		string freetype248path = "TTFText/Libraries/Win32/freetype248.dll";
		
		try {
			
			// Try copy freetype248 in Project Root Dir ...
		
			string src = Application.dataPath + "/" + freetype248path;
			string dest = System.Environment.CurrentDirectory + "/" + "freetype248.dll";
			
            if (! System.IO.File.Exists(src)) {

				string[] files = Directory.GetFiles(Application.dataPath, "freetype248.dll", SearchOption.AllDirectories);

				if (files.Length == 0) {
					Debug.LogError("(TTFText) '" + src + "' not found.");
					return false;
				}

				src = files[0];
            }
			
			
			System.IO.File.Copy(src, dest, true);
			
		} catch (System.Exception ex) {
			Debug.LogError("(TTFText) Error:" + ex.ToString());
			return false;
		}
		return true;
	}
	
	
	static bool TryInstallFreetype() {
		
		bool ok;
		
		System.PlatformID platform = System.Environment.OSVersion.Platform;
		
		switch (platform) {
		case System.PlatformID.MacOSX:
		case System.PlatformID.Unix:
			ok = InstallFreetypeOnMac();
			break;
			
		case System.PlatformID.WinCE:
		case System.PlatformID.Win32Windows:
		case System.PlatformID.Win32S:
		case System.PlatformID.Win32NT:
			ok = InstallFreetypeOnWin();
			break;
			
		default:
			ok = false;
			break;
		}
		
		AssetDatabase.Refresh();
		return ok;
	}
	
	
	// Check that freetype dll is accessble,
	// try to install it otherwise
	public static bool EnsureFreetype() {
		
		bool ok = CheckForFreetypeDll();
		
		if (! ok) { // Freetype dll not found
			
			if (! TryInstallFreetype()) {
				Debug.LogError("(TTFText) Freetype library cannot be found. Try install it on your system.");
			}
		
			ok = CheckForFreetypeDll();
		
			if (! ok) {
				Debug.LogError("(TTFText) Unable to access freetype library." 
				               + " The library has just been added to your project, try restarting Unity for the change to take effects.");
			}
		}
		
		return ok;
	}
	
	public static void ImportFont() {
		
		string srcPath = EditorUtility.OpenFilePanel("Import TrueType Font", lastImportSrc, "ttf");
		if (srcPath == "") { return; }
		
		lastImportSrc = System.IO.Path.GetDirectoryName(srcPath);
		string ttfname = System.IO.Path.GetFileName(srcPath);
		
		string destPath = EditorUtility.SaveFilePanelInProject("Save font as Asset", ttfname, "ttf", "");
		if (destPath == null) { return; }
		
		System.IO.File.Copy(srcPath, destPath, true);
		AssetDatabase.Refresh();
	}
	
	
	
	[MenuItem("Assets/TTF Text/Force Update Font List")]
	static void updateFontList() {
		
		EnsureFreetype();
		
		TTFTextFontListManager flm = TTFTextFontListManager.Instance;
		flm.UpdateAll(true);
		
	}
}
