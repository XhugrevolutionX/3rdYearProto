#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using UnityEngine.InputSystem.Utilities;

namespace Rodak.Hexagons.HexEditor
{

    [CustomPropertyDrawer(typeof(EditableHexagon))]
    public class EditableHexagonDrawer : PropertyDrawer
    {
        public static HexAutoCorrectMode DefaultAutoCorrectMode = HexAutoCorrectMode.Staggered;

        private const string Q = "Q";
        private const string R = "R";
        private const string S = "S";

        private const float Spacing = 5f;
        private const float LabelWidth = 13f;

        private const float SmallModeSize = 300f;
        private const float MinFieldWidth = 190f;

        private static readonly ReadOnlyArray<string> PropertyNames = new(new[] { Q, R, S });

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            GUIStyle clippedStyle = new(EditorStyles.label)
            {
                clipping = TextClipping.Clip,
                wordWrap = false,
                padding = new RectOffset(0, 2, 0, 0), // less padding
                normal = { background = null }
            };

            if (position.width < SmallModeSize && position.width > MinFieldWidth)
            {
                float freeSpace = Mathf.Max(position.width - MinFieldWidth, 0);

                Rect labelPosition = new(position.x, position.y, freeSpace, position.height);


                var controlID = GUIUtility.GetControlID(FocusType.Passive);
                EditorGUI.PrefixLabel(labelPosition, controlID, GUIContent.none);

                GUIContent labelContent = new(label.text, label.tooltip ?? label.text);
                GUI.Label(labelPosition, labelContent, clippedStyle);

                position = new(position.x + freeSpace, position.y, position.width - freeSpace, position.height);
            }
            else
            {
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label, clippedStyle);
            }

            bool isReadOnly = IsReadOnly(property);
            EditorGUI.BeginDisabledGroup(isReadOnly);

            float totalFieldWidth = position.width - (Spacing * (PropertyNames.Count - 1));
            float singleFieldWidth = totalFieldWidth / PropertyNames.Count;

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = LabelWidth;

            Rect fieldRect = new(position.x, position.y, singleFieldWidth, position.height);

            for (int i = 0; i < PropertyNames.Count; i++)
            {
                if (i > 0)
                {
                    fieldRect.x += singleFieldWidth + Spacing;
                }

                string propertyName = PropertyNames[i];
                SerializedProperty subProperty = property.FindPropertyRelative(propertyName);

                EditorGUI.BeginChangeCheck();

                EditorGUI.PropertyField(fieldRect, subProperty, new GUIContent(propertyName));

                if (EditorGUI.EndChangeCheck())
                {
                    AutoCorrectValue(propertyName, property);
                }
            }

            EditorGUIUtility.labelWidth = oldLabelWidth;

            EditorGUI.EndDisabledGroup();
            EditorGUI.EndProperty();
        }

        private void AutoCorrectValue(string changedProperty, SerializedProperty property)
        {
            SerializedProperty qProperty = property.FindPropertyRelative(Q);
            SerializedProperty rProperty = property.FindPropertyRelative(R);
            SerializedProperty sProperty = property.FindPropertyRelative(S);

            HexAutoCorrectMode autoCorrectMode = GetHexAutoCorrectMode();

            (qProperty.intValue, rProperty.intValue, sProperty.intValue) = changedProperty switch
            {
                Q => HexagonAutoCorrector.AutoCorrectByQ(qProperty.intValue, rProperty.intValue, sProperty.intValue, autoCorrectMode),
                R => HexagonAutoCorrector.AutoCorrectByR(qProperty.intValue, rProperty.intValue, sProperty.intValue, autoCorrectMode),
                S => HexagonAutoCorrector.AutoCorrectByS(qProperty.intValue, rProperty.intValue, sProperty.intValue, autoCorrectMode),
                _ => throw new ArgumentException($"{nameof(changedProperty)} has unknown value: ${changedProperty}"),
            };
        }

        private HexAutoCorrectMode GetHexAutoCorrectMode()
        {
            if (fieldInfo != null)
            {
                var attr = fieldInfo.GetCustomAttribute<HexAutoCorrect>();
                if (attr != null)
                    return attr.AutoCorrectMode;
            }
            return DefaultAutoCorrectMode;
        }

        private bool IsReadOnly(SerializedProperty property)
        {
            Type targetType = property.serializedObject.targetObject.GetType();
            FieldInfo field = targetType.GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (field == null)
                return false;

            return field.GetCustomAttribute<HexReadOnly>() != null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif