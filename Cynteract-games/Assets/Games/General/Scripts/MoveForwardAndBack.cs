using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Security.AccessControl;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
[DrawWithUnity]
public class MoveForwardAndBack : MonoBehaviour
{
    [ReadOnly]
    public Transform start, end;
    public Transform target;
    public float speed;
    [Range(0,1)]
    public float offSet;
    private float t;
    private float direction=1;

    public void CreateHandles()
    {
        Create(ref start, "Start", Vector3.zero);
        Create(ref end, "end", -Vector3.right);
    }
    public void Move(float t)
    {
        t = Mathf.Clamp01(t);
        target.position = Vector3.Lerp(start.position, end.position, t);
    }
    private void Create(ref Transform other, string name, Vector3 offset)
    {
        if (other == null)
        {
            other = new GameObject().transform;



        }

        other.parent = transform;
        other.gameObject.name = name;
        other.position = transform.position + offset;
    }
    private void Start()
    {
        t = offSet;
    }
    private void FixedUpdate()
    {
        t = Mathf.Clamp01(t + direction * speed * Time.fixedDeltaTime);
        Move(t);
        print(t);
        if (direction == 1)
        {
            if (Mathf.Approximately(t, 1))
            { 

                direction = -1;
            }
        }
        else
        {
            if (Mathf.Approximately(t,0))
            {

                direction = 1;
            }
        }
    }
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR

#endif
        if (start != null && end != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start.position, end.position);
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(MoveForwardAndBack))]
public class MoveForwardAndBackEditor : Editor
{
    MoveForwardAndBack moveForwardAndBack;
    public string editingText = "Edit";
    bool editing = false;
    float t;
    private void Awake()
    {
        moveForwardAndBack = target as MoveForwardAndBack;
    }
    Tool LastTool = Tool.None;


    void OnDisable()
    {
        Tools.current = LastTool;
    }
    private void OnSceneGUI()
    {
        moveForwardAndBack = target as MoveForwardAndBack;
        if (editing)
        {
            EditorGUI.BeginChangeCheck();
            var startPos = Handles.PositionHandle(moveForwardAndBack.start.position, moveForwardAndBack.start.rotation);
            var endPos = Handles.PositionHandle(moveForwardAndBack.end.position, moveForwardAndBack.end.rotation);
            PlaceTarget();
            Handles.color = Color.green;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(moveForwardAndBack.start, "Changed start handle position");
                Undo.RecordObject(moveForwardAndBack.end, "Changed  end handle position");
                moveForwardAndBack.start.position = startPos;
                moveForwardAndBack.end.position = endPos;
            }
        }
        else
        {
            Handles.color = Color.red;
        }
        if (moveForwardAndBack.start != null && moveForwardAndBack.end != null)
        {
            Handles.DrawLine(moveForwardAndBack.start.position, moveForwardAndBack.end.position);
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Handles.color;
            Handles.Label(moveForwardAndBack.start.position, "Start", style);
            Handles.Label(moveForwardAndBack.end.position, "End", style);
        }

    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button(editingText))
        {
            editing = !editing;
            if (editing)
            {
                editingText = "Stop Editing";
                LastTool = Tools.current;
                Tools.current = Tool.None;
            }
            else
            {
                editingText = "Edit";

                Tools.current = LastTool;
            }
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("CreateHandles"))
        {
            moveForwardAndBack.CreateHandles();
        }
        PlaceTarget();
        EditorGUILayout.Space();
    }

    private void PlaceTarget()
    {
        if (moveForwardAndBack.target != null && !Application.isPlaying)
        {
            t = GUILayout.HorizontalSlider(t, 0, 1);
            moveForwardAndBack.Move(t);
        }
    }
}
#endif