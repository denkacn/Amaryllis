using Amaryllis.Entities.Models;
using UnityEditor;
using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal sealed class StateDebugWindow : EditorWindow
    {
        private HasStateEntity _targetEntity;
        private bool _isTargetLocked;

        public static void Open(HasStateEntity entity)
        {
            var window = GetWindow<StateDebugWindow>();
            window.titleContent = new GUIContent("State Debug");
            window.SetTarget(entity);
            window.Show();
        }

        [MenuItem("Tools/Amaryllis/State Debug Window")]
        private static void OpenFromMenu()
        {
            Open(GetSelectedEntity());
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            if (_targetEntity == null)
            {
                SetTarget(GetSelectedEntity());
            }
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.Space(8f);
            DrawTargetInfo();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var locked = GUILayout.Toggle(_isTargetLocked, "Lock Target", EditorStyles.toolbarButton, GUILayout.Width(90f));
                if (locked != _isTargetLocked)
                {
                    _isTargetLocked = locked;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Use Selection", EditorStyles.toolbarButton, GUILayout.Width(100f)))
                {
                    SetTarget(GetSelectedEntity());
                }
            }
        }

        private void DrawTargetInfo()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField("Entity", _targetEntity, typeof(HasStateEntity), true);
                }

                if (_targetEntity == null)
                {
                    EditorGUILayout.HelpBox("Select a HasStateEntity or open this window from its inspector.", MessageType.Info);
                    return;
                }

                EditorGUILayout.LabelField("GameObject", _targetEntity.gameObject.name);
                EditorGUILayout.LabelField("Instance ID", _targetEntity.GetInstanceID().ToString());
            }
        }

        private void OnSelectionChanged()
        {
            if (_isTargetLocked)
            {
                return;
            }

            var selectedEntity = GetSelectedEntity();
            if (selectedEntity == null || selectedEntity == _targetEntity)
            {
                return;
            }

            SetTarget(selectedEntity);
        }

        private void SetTarget(HasStateEntity entity)
        {
            _targetEntity = entity;
            Repaint();
        }

        private static HasStateEntity GetSelectedEntity()
        {
            return Selection.activeGameObject == null
                ? null
                : Selection.activeGameObject.GetComponentInParent<HasStateEntity>();
        }
    }
}
