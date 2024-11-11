using UnityEngine;
using UnityEditor;
using Legacy.Client;
using UnityEditor.UI;

[CustomEditor(typeof(LegacyButton))]
public class LegacyButtonEditor : ButtonEditor
{
	SerializedProperty blickControl;
	SerializedProperty muteSound;

	protected override void OnEnable()
	{
		base.OnEnable();
		blickControl = serializedObject.FindProperty("blickControl");
		muteSound = serializedObject.FindProperty("muteSound");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		serializedObject.Update();
		EditorGUILayout.PropertyField(blickControl);
		EditorGUILayout.PropertyField(muteSound);
		serializedObject.ApplyModifiedProperties();
	}
}