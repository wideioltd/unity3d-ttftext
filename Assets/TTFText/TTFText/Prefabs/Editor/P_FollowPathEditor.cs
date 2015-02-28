using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(P_FollowPath))]
public class P_FollowPathEditor : Editor {
	
	public int mode=0;
	
	
	
    //iTweenPath _target;
	GUIStyle style = new GUIStyle();
	public static int count = 0;
	
	void OnEnable(){
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
	}
	
	static bool showCurves;
	
	public override void OnInspectorGUI() {		
		P_FollowPath pm = target as P_FollowPath;
		
		//draw the path?
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Path Visible");
		pm.pathVisible = EditorGUILayout.Toggle(pm.pathVisible);
		EditorGUILayout.EndHorizontal();
	
		
		//exploration segment count control:
		EditorGUILayout.BeginHorizontal();
		if (pm.path==null) {
			pm.path=new Vector3[16];
		}
		
		
		int nodeCount = Mathf.Max(2, EditorGUILayout.IntField("Node Count", pm.path.Length));
		EditorGUILayout.EndHorizontal();
	
		pm.orientAccordingPath=EditorGUILayout.Toggle("Orient According Path",pm.orientAccordingPath);
		//add node?
		if(nodeCount !=pm.path.Length){
			System.Array.Resize(ref pm.path,nodeCount);
		}
	
		
		if (pm.mode<0){
			pm.mode=0;
			pm.parameters=P_FollowPath.presetmodes[0].DefaultParameters();			
		}
		
		showCurves=EditorGUILayout.Foldout(showCurves,"Show Curves");
		if (showCurves) {
			EditorGUILayout.CurveField("Scale Factors",pm.scalingCurve);
			EditorGUILayout.CurveField("YZ Rotation",pm.YZRotationCurve);
		}
		
		Color defcolor=GUI.color;
		
		GUILayout.BeginHorizontal();
		for (int i=0;i<P_FollowPath.presetmodes.Length;i++) {
			if (pm.mode==i) {
				GUI.color=Color.yellow;
			}
			else {
				GUI.color=defcolor;
			}
			if (GUILayout.Button(P_FollowPath.presetmodes[i].GetModeName())) {
				pm.mode=i;
				pm.parameters=P_FollowPath.presetmodes[i].DefaultParameters();
			}
		}
		GUILayout.EndHorizontal();
		GUI.color=defcolor;
		
		
		if (pm.parameters!=null) {
		//if (true) {
			EditorGUI.indentLevel = 4;
			
			if (pm.mode==0) {
			//
			for (int i = 0; i < pm.path.Length; i++) {
				pm.path[i] = EditorGUILayout.Vector3Field("" + (i+1), pm.path[i]);
			}
			
			}
			else {
			//pm.parameters=P_FollowPath.presetmodes[pm.mode].EditParameters(pm,pm.parameters);		
			
			foreach(System.Reflection.FieldInfo fi in pm.parameters.GetType().GetFields()) {
                try {					
				if (fi.FieldType.Name== typeof(float).Name) {
					float f0=(float)fi.GetValue(pm.parameters);
					float f1=EditorGUILayout.FloatField(fi.Name,f0 );
					if (f1!=f0) {									
						fi.SetValue(pm.parameters,f1);
					}
				}
				if (fi.FieldType.Name== typeof(int).Name) {
					fi.SetValue(pm.parameters,(EditorGUILayout.FloatField(fi.Name,(int)fi.GetValue(pm.parameters) ) ));	
				}
				if (fi.FieldType.Name== typeof(Vector3).Name) {
					fi.SetValue(pm.parameters,(EditorGUILayout.Vector3Field(fi.Name,(Vector3)fi.GetValue(pm.parameters)) ));
				}								
				if (fi.FieldType.Name== typeof(bool).Name) {
					fi.SetValue(pm.parameters,(EditorGUILayout.Toggle(fi.Name,(bool)fi.GetValue(pm.parameters)) ));
				}																
				if (fi.FieldType.Name== typeof(AnimationCurve).Name) {
					EditorGUILayout.CurveField(fi.Name,(AnimationCurve)fi.GetValue(pm.parameters));
				}
				}
				catch(System.Exception e) {
						//Debug.LogWarning(fi.FieldType.Name);
						//Debug.LogWarning(fi.FieldType.GUID);
						Debug.Log(e);
				}
			}
			}
		
			EditorGUI.indentLevel = 0;
			//if (GUILayout.Button("Regenerate")) {
			P_FollowPath.presetmodes[pm.mode].Generate(pm);
			
		}
		else {
			//pm.parameters=P_FollowPath.presetmodes[pm.mode].DefaultParameters();
			Debug.Log(pm.parameters.ToString());
		}
		//}
		
		
		pm.pathVisible=true;
		pm.SaveParams();
		
		
		
		//update and redraw:
		//if(GUI.changed){
			EditorUtility.SetDirty(pm);		
		    P_FollowPath.presetmodes[pm.mode].Update(pm,(float)EditorApplication.timeSinceStartup);
		//}
		

		
		
	}
	
	void OnSceneGUI(){
		P_FollowPath pm = target as P_FollowPath;
		if (pm.pathVisible){			
			if(pm.path.Length > 0){
				//allow path adjustment undo:
				Undo.SetSnapshotTarget(pm,"Adjust Path");
				
				//path begin and end labels:
				Handles.Label(
					pm.path[0], 
					"'"  + "' Begin",
					style
					);
				Handles.Label(
					pm.path[pm.path.Length-1], "'" + "' End", style);
				
				//node handle display:
				for (int i = 0; i < pm.path.Length; i++) {
					pm.path[i] =
						Handles.PositionHandle(
							pm.path[i], 
							Quaternion.identity
							);
				}	
			}	
		}
		
	}
}


