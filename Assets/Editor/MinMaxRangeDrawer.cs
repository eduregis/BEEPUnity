using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class MinMaxRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MinMaxRangeAttribute range = (MinMaxRangeAttribute)attribute;

        SerializedProperty minProp = property.FindPropertyRelative("min");
        SerializedProperty maxProp = property.FindPropertyRelative("max");

        float min = minProp.floatValue;
        float max = maxProp.floatValue;

        // Label
        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label);

        // Slider
        Rect sliderRect = new Rect(position.x, position.y + 18, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.MinMaxSlider(sliderRect, GUIContent.none, ref min, ref max, range.Min, range.Max);

        // Inputs
        Rect fieldRect = new Rect(position.x, position.y + 36, position.width, EditorGUIUtility.singleLineHeight);
        float fieldWidth = (position.width - 4f) / 2f;
        Rect minRect = new Rect(fieldRect.x, fieldRect.y, fieldWidth, fieldRect.height);
        Rect maxRect = new Rect(fieldRect.x + fieldWidth + 4f, fieldRect.y, fieldWidth, fieldRect.height);

        min = EditorGUI.FloatField(minRect, min);
        max = EditorGUI.FloatField(maxRect, max);

        min = Mathf.Clamp(min, range.Min, max);
        max = Mathf.Clamp(max, min, range.Max);

        minProp.floatValue = min;
        maxProp.floatValue = max;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 3 + 6;
    }
}
