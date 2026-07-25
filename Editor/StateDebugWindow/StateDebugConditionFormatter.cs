using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Amaryllis.Editor.StateDebugWindow
{
    internal static class StateDebugConditionFormatter
    {
        public static string FormatSummary(IReadOnlyList<StateDebugConditionModel> conditions)
        {
            return conditions == null || conditions.Count == 0
                ? string.Empty
                : string.Join(", ", conditions.Select(FormatLabel));
        }

        public static string FormatLabel(StateDebugConditionModel condition)
        {
            var label = condition.Name;
            if (condition.Source is Component component && component.gameObject.name != condition.Name)
            {
                label += $" ({component.gameObject.name})";
            }

            if (TryGetSerializedBool(condition.Source, "_isCanExec", out var isCanExec))
            {
                label += $" = {isCanExec}";
            }

            return label;
        }

        private static bool TryGetSerializedBool(Object source, string propertyName, out bool value)
        {
            value = false;
            if (source == null)
            {
                return false;
            }

            var serializedObject = new SerializedObject(source);
            var property = serializedObject.FindProperty(propertyName);
            if (property == null || property.propertyType != SerializedPropertyType.Boolean)
            {
                return false;
            }

            value = property.boolValue;
            return true;
        }
    }
}
