using Amaryllis.Entities.Models;
using UnityEditor;
using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    [CustomEditor(typeof(HasStateEntity))]
    internal sealed class HasStateEntityEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(8f);

            var entity = (HasStateEntity)target;
            if (GUILayout.Button("Open State Debug"))
            {
                StateDebugWindow.Open(entity);
            }
        }
    }
}
