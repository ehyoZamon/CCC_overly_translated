#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class TimeSpanDrawer : OdinValueDrawer<TimeSpan>
{

    [Serializable]
    public struct SerializedTimeSpan
    {
        public int days, hours, minutes, seconds, milliseconds;
    }
    protected override void DrawPropertyLayout(GUIContent label)
    {
        var width = GUILayout.Width(70);
        var doubleWidth = GUILayout.Width(100);
        TimeSpan value = this.ValueEntry.SmartValue;
        SerializedTimeSpan serializedDateTime = new SerializedTimeSpan();
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(label, doubleWidth);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();


        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Days", width);
        serializedDateTime.days = EditorGUILayout.IntField(value.Days);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Hours", width);
        serializedDateTime.hours = EditorGUILayout.IntField(value.Hours);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Minutes", width);
        serializedDateTime.minutes = EditorGUILayout.IntField(value.Minutes);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Seconds", width);
        serializedDateTime.seconds = EditorGUILayout.IntField(value.Seconds);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Millis", width);
        serializedDateTime.milliseconds = EditorGUILayout.IntField(value.Milliseconds);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (ValueEntry.IsEditable)
        {
            ValueEntry.SmartValue = new TimeSpan(
                 serializedDateTime.days,
                 serializedDateTime.hours,
                 serializedDateTime.minutes,
                 serializedDateTime.seconds,
                 serializedDateTime.milliseconds);
        }

    }

}
#endif