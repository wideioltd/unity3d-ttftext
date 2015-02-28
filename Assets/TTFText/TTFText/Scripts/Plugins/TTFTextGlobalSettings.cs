using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TTFTextGlobalSettings : ScriptableObject {
	private static TTFTextGlobalSettings _Instance=null;
	
	public  static TTFTextGlobalSettings Instance {
		get {
			if (_Instance==null) {
				_Instance=(TTFTextGlobalSettings)Resources.Load("TTFTextSettings.asset");
				if (_Instance==null) {
					_Instance=TTFTextGlobalSettings.CreateInstance<TTFTextGlobalSettings>();
				}
			}
			return _Instance;
		}
	}
	
	~TTFTextGlobalSettings() {
		//Debug.Log("Destroying TTFText global settings now, is this awkward ?");
	}
	
	
	[SerializeField]
	private bool recreateTextsWhenStylePrefabModified=true;
	public bool RecreateTextsWhenStylePrefabModified {get { return recreateTextsWhenStylePrefabModified;} set {if (recreateTextsWhenStylePrefabModified!=value) {recreateTextsWhenStylePrefabModified=value; Update();}}}	
	
	[SerializeField]
	private bool easyDeployement=true;
	public bool EasyDeployement {get {return easyDeployement;} set {if (easyDeployement!=value) {easyDeployement=value; Update();}}}
	
	[SerializeField]
	private bool showTTFTextObjects=true;
	public bool ShowTTFTextObjects {get {return showTTFTextObjects;} set {if (showTTFTextObjects!=value){showTTFTextObjects=value; Update(); UpdateScene();}}}
	
	
	public void UpdateScene() {
		GameObject fs=GameObject.Find("/TTFText Font Store");
		if (fs!=null) {
			fs.hideFlags=(showTTFTextObjects)?0:HideFlags.HideInHierarchy;
		}
	}
	
	public void Update() {		
#if UNITY_EDITOR
		TTFTextGlobalSettings instance=this;
		try {
		string d=System.IO.Path.Combine(Application.dataPath,"Resources");
		//Debug.Log(d);
		if (System.IO.Directory.Exists(d)) {
							System.IO.Directory.CreateDirectory(d);
							UnityEditor.AssetDatabase.Refresh();
		}
		if (System.IO.File.Exists(d+"/TTFTextSettings.asset")) {
			instance=TTFTextGlobalSettings.CreateInstance<TTFTextGlobalSettings>();
				
			// deep copy
			instance.easyDeployement=this.easyDeployement;
			instance.showTTFTextObjects=this.showTTFTextObjects;
				
 			UnityEditor.AssetDatabase.DeleteAsset("Assets/Resources/TTFTextSettings.asset");	
	   	    if (System.IO.File.Exists(d+"/TTFTextSettings.asset")) {	
			  System.IO.File.Delete(d+"/TTFTextSettings.asset");
			   if (System.IO.File.Exists(d+"/TTFTextSettings.asset.meta")) {
								System.IO.File.Delete(d+"/TTFTextSettings.asset.meta");
			  }							
	          UnityEditor.AssetDatabase.Refresh();
		    }
				
			_Instance=instance;
		}

		UnityEditor.AssetDatabase.CreateAsset(
			instance,
			"Assets/Resources/TTFTextSettings.asset"
			//	"Resources/TTFTextSettings.asset"
			);			
		}
		catch (System.Exception e) {
			Debug.LogError(e);
		}
#endif		
	}
}


