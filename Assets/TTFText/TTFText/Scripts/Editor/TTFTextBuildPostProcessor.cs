#if ! UNITY_3_5 
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class TTFTextBuildPostprocessor
{
	public static string DefaultAndroidSDKRoot = @"C:\Program Files (x86)\Android\android-sdk\";
	static public bool copyLocalFontsToBuild = true;

	private static string EscapeCommandLineArguments (string[] args)
	{
		string arguments = "";
		foreach (string arg in args) {
			arguments += " \"" +
            arg.Replace ("\\", "\\\\").Replace ("\"", "\\\"") +
            "\"";
		}
		return arguments;
	}
	
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget target, string pathToBuiltProject)
	{
		string origPathToBuildProject = (string)pathToBuiltProject.Clone ();
		string assetsbindata = System.IO.Path.Combine ("assets", System.IO.Path.Combine ("bin", "Data"));
		switch (target) {
		case UnityEditor.BuildTarget.StandaloneWindows:
		case UnityEditor.BuildTarget.StandaloneWindows64:
			pathToBuiltProject = pathToBuiltProject.Substring (0, pathToBuiltProject.Length - 4) + "_Data";
			break;
		case UnityEditor.BuildTarget.Android:			
			pathToBuiltProject = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (pathToBuiltProject), assetsbindata);
			if (!System.IO.Directory.Exists (pathToBuiltProject)) {
				System.IO.Directory.CreateDirectory (pathToBuiltProject);
			}
			break;
		default:
			pathToBuiltProject = System.IO.Path.GetDirectoryName (pathToBuiltProject);
			break;
		}				
		
		
				
		//UnityEditor.AndroidSdkRoot
		//UnityEditor.BuildPipeline.BuildPlayer()
		if ((target == UnityEditor.BuildTarget.StandaloneWindows) || (target == UnityEditor.BuildTarget.StandaloneWindows64)) {
			// COPY THE DLLS
			string [] libs = {"freetype248.dll", "msvcr100.dll"};
			foreach (string libname in libs) {
				string src = "TTFText/Libraries/Win32/" + libname;
				if (! System.IO.File.Exists (src)) {

					string[] files = Directory.GetFiles (Application.dataPath, System.IO.Path.GetFileName (src), SearchOption.AllDirectories);

					if (files.Length == 0) {
						Debug.LogError ("(TTFText) '" + src + "' not found.");					
					} else {
						src = files [0];
					}
				}
			
				string dest = System.IO.Path.Combine (
				System.IO.Path.GetDirectoryName (pathToBuiltProject),
				System.IO.Path.GetFileName (src));				
				System.IO.File.Copy (src, dest, true);
			}
		}
		
		
		if ((copyLocalFontsToBuild) && (TTFTextGlobalSettings.Instance.EasyDeployement)) {
			Debug.Log ("TTF Text copying fonts in project folder...");
			
			foreach (TTFontInfo tfi in TTFTextFontListManager.Instance.LocalFonts.Values) {
				string fn = tfi.Path;
				try {
					System.IO.File.Copy (fn,
						System.IO.Path.Combine (pathToBuiltProject, System.IO.Path.GetFileName (fn)), true);
				} catch {
					Debug.LogError ("(TTF Text)Failed copy from " + fn + " to " +
						System.IO.Path.Combine (pathToBuiltProject, System.IO.Path.GetFileName (fn)));
				}
			}
		}
		
		if (target == UnityEditor.BuildTarget.Android) {
			
			if ((TTFTextGlobalSettings.Instance.EasyDeployement)) {
				string cwd = System.IO.Directory.GetCurrentDirectory ();

				string KeystoreName = System.Environment.ExpandEnvironmentVariables ("%HOMEDRIVE%%HOMEPATH%") + "\\.android\\debug.keystore";
				//cmd /c ""C:/Program Files (x86)/Android/android-sdk\tools\apkbuilder.bat" "C:\Users\Bertrand Nouvel\Desktop\TTFText-b2\Temp/StagingArea/Package_unaligned.apk" -v -z "C:\Users\Bertrand Nouvel\Desktop\TTFText-b2\Temp/StagingArea/assets.ap_" -z "C:\Users\Bertrand Nouvel\Desktop\TTFText-b2\Temp/StagingArea/bin/resources.ap_" -nf "C:\Users\Bertrand Nouvel\Desktop\TTFText-b2\Temp/StagingArea/libs" -f "C:\Users\Bertrand Nouvel\Desktop\TTFText-b2\Temp/StagingArea/bin/classes.dex" -d"
				string KeystorePassword = "android";
				string KeyAlias = "androiddebugkey";
				string KeyPassword = "android";
			
			
				if ((PlayerSettings.Android.keyaliasName.Length > 0)
				|| (PlayerSettings.Android.keyaliasPass.Length > 0)
				|| (PlayerSettings.Android.keystoreName.Length > 0)
				|| (PlayerSettings.Android.keystorePass.Length > 0)
				) {
					KeystoreName = PlayerSettings.Android.keystoreName;
					KeystorePassword = PlayerSettings.Android.keystorePass;
					KeyAlias = PlayerSettings.Android.keyaliasName;
					KeyPassword = PlayerSettings.Android.keyaliasPass;
				}

			
				try {
					System.IO.Directory.SetCurrentDirectory (System.IO.Path.GetDirectoryName (origPathToBuildProject));
					//UnityEditor.AndroidSdkRoot asr=new UnityEditor.AndroidSdkRoot();
					//UnityEditor.EditorUserBuildSettings.activeScriptCompilationDefines
					//string asr=UnityEditor.PlayerSettings.Android.sdk
					string asr = DefaultAndroidSDKRoot;
			
					//Debug.Log(asr.ToString());
					string aapt = System.IO.Path.Combine (asr, @"platform-tools\aapt.exe");			
					string zipalign = System.IO.Path.Combine (asr, @"tools\zipalign.exe");			
//				string aptbuilder=System.IO.Path.Combine(asr,@"tools\aptbuilder.bat");			

				
				
				
					System.Func<string,string,string,bool> AAPT = (act,apk,f) => {
						if (!File.Exists (aapt)) {
							Debug.Log (aapt);
						}
						System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo (aapt);
						//psi.Arguments=System.String.Join(" ",new string []{act,apk,f});
						psi.Arguments = EscapeCommandLineArguments (new string []{act,apk,f});
						//psi.UseShellExecute=false;
						//psi.CreateNoWindow=true;					
						psi.CreateNoWindow = true;
						psi.UseShellExecute = false;
						psi.RedirectStandardError = true;
						psi.RedirectStandardOutput = true;
						psi.WorkingDirectory = System.IO.Path.GetDirectoryName (origPathToBuildProject);
//					Debug.Log(psi.WorkingDirectory);
						System.Diagnostics.Process p = System.Diagnostics.Process.Start (psi);					
						p.WaitForExit ();
					
//					string o=p.StandardOutput.ReadToEnd();
						string e = p.StandardError.ReadToEnd ();
						//if (o.Length>0) {
						//	Debug.Log(o);
						//}
						if (e.Length > 0) {
							Debug.LogError (e);
						}
						return true;
					};

					/*
				System.Func<string,string,string,bool> APTBUILDER=(act,apk,f)=> {
					if (!File.Exists(aptbuilder)) {
						Debug.Log(aptbuilder);
					}					
					System.Diagnostics.ProcessStartInfo psi=new System.Diagnostics.ProcessStartInfo(aptbuilder);
					//psi.Arguments=System.String.Join(" ",new string []{act,apk,f});
					psi.Arguments=EscapeCommandLineArguments(new string []{act,apk,f});
					//psi.UseShellExecute=false;
					//psi.CreateNoWindow=true;					
					psi.CreateNoWindow=false;
					psi.UseShellExecute=true;
					psi.WorkingDirectory=System.IO.Path.GetDirectoryName(origPathToBuildProject);
					System.Diagnostics.Process p=System.Diagnostics.Process.Start(psi);					
					p.WaitForExit();
					return true;
				};
				*/
				
					System.Func<string,string,bool> JARSIGNER = (apk,f) => {
						System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo ("jarsigner");
						psi.Arguments = EscapeCommandLineArguments (new string []{"-keystore",KeystoreName,"-storepass",KeystorePassword,"-keypass",KeyPassword,apk,f});
//					psi.CreateNoWindow=false;
						//	psi.UseShellExecute=true;
						psi.CreateNoWindow = true;
						psi.UseShellExecute = false;

						psi.RedirectStandardError = true;
						psi.RedirectStandardOutput = true;
						psi.WorkingDirectory = System.IO.Path.GetDirectoryName (origPathToBuildProject);
						System.Diagnostics.Process p = System.Diagnostics.Process.Start (psi);					
						p.WaitForExit ();
						string o = p.StandardOutput.ReadToEnd ();
						string e = p.StandardError.ReadToEnd ();
						if (o.Length > 0) {
							Debug.Log (o);
						}
						if (e.Length > 0) {
							Debug.Log (e);
						}
					
						return true;
					};

				
				
				
					System.Func<string,string,bool> ZIPALIGN = (signedapk,alignedapk) => {
						if (!File.Exists (zipalign)) {
							Debug.Log (zipalign);
						}
						System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo (zipalign);
						//psi.Arguments=System.String.Join(" ",new string []{act,apk,f});
						psi.Arguments = EscapeCommandLineArguments (new string []{
						"-v",
                        "-f","4",
                		signedapk,
               			alignedapk
					});
						psi.CreateNoWindow = true;
						psi.UseShellExecute = false;
						psi.RedirectStandardError = true;
						psi.RedirectStandardOutput = true;
						psi.WorkingDirectory = System.IO.Path.GetDirectoryName (origPathToBuildProject);
						System.Diagnostics.Process p = System.Diagnostics.Process.Start (psi);					
						p.WaitForExit ();
						string o = p.StandardOutput.ReadToEnd ();
						string e = p.StandardError.ReadToEnd ();
						if (o.Length > 0) {
							Debug.Log (o);
						}
						if (e.Length > 0) {
							Debug.LogError (e);
						}
						return true;
					};
				
				
					foreach (TTFontInfo tfi in TTFTextFontListManager.Instance.LocalFonts.Values) {
						string fn = tfi.Path;
						AAPT ("add",
						//origPathToBuildProject, 
						System.IO.Path.GetFileName (origPathToBuildProject), 
						"assets/bin/Data/" + System.IO.Path.GetFileName (fn)
						);
					}
					JARSIGNER (System.IO.Path.GetFileName (origPathToBuildProject), KeyAlias);
					ZIPALIGN (System.IO.Path.GetFileName (origPathToBuildProject), "zipaligned-" + System.IO.Path.GetFileName (origPathToBuildProject));
				
				} finally {
					System.IO.Directory.SetCurrentDirectory (cwd);
				}
			}	
			
		}
	}
}

#endif