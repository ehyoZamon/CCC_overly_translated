#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public class UpdateInEditMode
{
    
    static UpdateInEditMode()
    {
        //EditorApplication.update += Update;
    }

    static void Update()
    {
        Debug.Log("Updating");
    }
    public static void CallInMainThread(Action action)
    {
        throw new NotImplementedException();
    }
}
#endif