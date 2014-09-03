using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EditorControls))]
public class LevelRootEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		EditorControls levelRoot = (EditorControls)target;
		GUILayout.BeginVertical();
		if(GUILayout.Button("Export Level"))
		{
			levelRoot.ExportLevel(levelRoot.LevelName);
		}

		if(GUILayout.Button("Load Level")) {
			levelRoot.LoadLevel(levelRoot.LevelName);
		}
		GUILayout.EndVertical();

		GUILayout.BeginVertical();

		if(GUILayout.Button ("Load Recording")) {
			levelRoot.LoadLevel ("TempRecording");
		}
		GUILayout.EndVertical();
	}
	
}