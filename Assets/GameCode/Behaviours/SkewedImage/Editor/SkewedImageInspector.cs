using UnityEditor;
using UnityEditor.UI;

namespace UIExtensions.Editor
{
    [CustomEditor(typeof(SkewedImage))]
    public class SkewedImageInspector : ImageEditor
    {
        private SkewedImage _skewedImage;
        private SerializedProperty _skewMethod;
        private SerializedProperty _skewVector;
        private SerializedProperty _skewAngles;

        protected override void OnEnable()
        {
            base.OnEnable();
            _skewedImage = (SkewedImage)target;
            _skewMethod = serializedObject.FindProperty("SkewMethod");
            _skewVector = serializedObject.FindProperty("SkewVector");
            _skewAngles = serializedObject.FindProperty("SkewAngles");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(_skewMethod);
            if (_skewedImage.SkewMethod == SkewMethod.Angles)
            {
                UpdateAngles();
            }
            else
            {
                UpdatePixels();
            }
        }

        private void UpdatePixels()
        {
            EditorGUILayout.PropertyField(_skewVector);

            if (_skewVector.vector2Value != _skewedImage.SkewVector)
            {
                Undo.RecordObject(_skewedImage, "Changed Skew");
                _skewedImage.SkewVector = _skewVector.vector2Value;
                _skewedImage.OnRebuildRequested();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateAngles()
        {
            EditorGUILayout.PropertyField(_skewAngles);

            if (_skewAngles.vector2Value != _skewedImage.SkewVector)
            {
                Undo.RecordObject(_skewedImage, "Changed Skew");
                _skewedImage.SkewVector = _skewAngles.vector2Value;
                _skewedImage.OnRebuildRequested();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
