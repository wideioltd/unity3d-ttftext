//  Unity TTF Text
//  Copyrights 2011-2012 ComputerDreams.org O. Blanc & B. Nouvel
//  All infos related to this software at http://ttftext.computerdreams.org/
//   

#if ! UNITY_3_5
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using System.IO;
using System.Collections.Generic;


// keeps track of all TTF files in asset folder
public class TTFAssetPostprocessor : AssetPostprocessor {	
	
	private static void OnPostprocessAllAssets(string[] importedAssets, 
	                                           string[] deletedAssets, 
	                                           string[] movedAssets, 
	                                           string[] movedFromPaths) {
		
		// Update Font List every time some change occurs in Asset Database
		// TODO: call update when only change affect some ttf file
		
		TTFTextLibraryInstaller.EnsureFreetype();
		TTFTextFontListManager flm = TTFTextFontListManager.Instance;
		flm.UpdateLocalFonts();
    }
	
	
}

#endif