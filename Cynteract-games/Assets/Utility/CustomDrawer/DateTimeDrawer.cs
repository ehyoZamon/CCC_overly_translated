#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
public class DateTimeDrawer : OdinValueDrawer<DateTime>
{
    
    [Serializable]
    public struct SerializedDateTime
    {
        public int year, month, day, hour, minute, second, millisecond;
    }
    protected override void DrawPropertyLayout(GUIContent label)
    {
        var width = GUILayout.Width(70);
        var doubleWidth = GUILayout.Width(100);
        DateTime value = this.ValueEntry.SmartValue;
        SerializedDateTime serializedDateTime = new SerializedDateTime();
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(label, doubleWidth);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();


        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Year", width);
        serializedDateTime.year = EditorGUILayout.IntField(value.Year);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Month", width);
        serializedDateTime.month = EditorGUILayout.IntField(value.Month);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Day", width);
        serializedDateTime.day = EditorGUILayout.IntField(value.Day);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Hour", width);
        serializedDateTime.hour = EditorGUILayout.IntField(value.Hour);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Minute", width);
        serializedDateTime.minute = EditorGUILayout.IntField(value.Minute);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Second", width);
        serializedDateTime.second = EditorGUILayout.IntField(value.Second);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Millis", width);
        serializedDateTime.millisecond = EditorGUILayout.IntField(value.Millisecond);
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (ValueEntry.IsEditable){
            ValueEntry.SmartValue = new DateTime(
                 serializedDateTime.year,
                 serializedDateTime.month,
                 serializedDateTime.day,
                 serializedDateTime.hour,
                 serializedDateTime.minute,
                 serializedDateTime.second,
                 serializedDateTime.millisecond);
        }

    } 

}
#endif