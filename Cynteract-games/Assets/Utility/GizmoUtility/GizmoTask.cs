using System;
using UnityEngine;
#if UNITY_EDITOR
#endif
public class GizmoTask
{
    public Action action;
    public float startTime, duration;
    public bool started = false;
    public Color color;
    public GizmoTask(Action action, float duration)
    {
        this.action = action;
        this.duration = duration;
        this.color = Color.white;
    }
    public GizmoTask(Action action, float duration, Color color)
    {
        this.action = action;
        this.duration = duration;
        this.color = color;
    }
}

