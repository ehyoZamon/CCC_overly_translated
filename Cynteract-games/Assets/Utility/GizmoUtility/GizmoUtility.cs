using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
    #endif
public class GizmoUtility : MonoBehaviour
{
    public static Coroutine loop;
    static Queue<GizmoTask> tasks;
    public static GizmoUtility mainThreadUtility;
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        Debug.Log("Gizmo Utility active");
        var gO = new GameObject();
        gO.name = "Gizmo Utility";
        mainThreadUtility = gO.AddComponent<GizmoUtility>();
        DontDestroyOnLoad(gO);
        tasks = new Queue<GizmoTask>();
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var newTasks = new Queue<GizmoTask>();

        while (tasks.Count > 0)
        {
            var nextTask = tasks.Dequeue();
            if (!nextTask.started)
            {
                nextTask.startTime = Time.fixedTime;
                nextTask.started = true;
            }
            
            try
            {
                Handles.color = nextTask.color;
                nextTask.action();
            }
            catch (Exception e)
            {

                Debug.LogError(e);
            }
            if (Time.fixedTime-nextTask.startTime<nextTask.duration)
            {
                newTasks.Enqueue(nextTask);
            }

        }
        tasks = newTasks;
    }
#endif
    public static void DrawText(Vector3 position, string text)
    {
        DrawText(position, text, 0);
    }
    public static void DrawText(Vector3 position, string text, float duration)
    {
        DrawText(position, text, duration, Color.white);
    }
    public static void DrawText(Vector3 position, string text, float duration, Color color)
    {
#if UNITY_EDITOR
        tasks.Enqueue(new GizmoTask(() => {Handles.Label(position, text); }, duration, color));
#endif
    }
}
