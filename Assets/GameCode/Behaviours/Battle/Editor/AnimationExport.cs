using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimationExport))]

public class AnimatorInspector : Editor
{
	public Animator animator;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		var _export = target as AnimationExport;

		GUILayout.TextField("test");

		if (GUILayout.Button("Export Animation"))
			_export.Export();
	}
}
