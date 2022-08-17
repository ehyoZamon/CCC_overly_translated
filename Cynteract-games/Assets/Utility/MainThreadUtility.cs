using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadUtility :MonoBehaviour
{
    public static Coroutine loop;
    static Queue<Action> tasks;
    public static MainThreadUtility mainThreadUtility;
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        Debug.Log("MainThreadUtility active");
        var gO = new GameObject();
        gO.name = "Main Thread Utility";
        mainThreadUtility =gO.AddComponent<MainThreadUtility>();
        DontDestroyOnLoad(gO);
        tasks = new Queue<Action>();
        loop = mainThreadUtility.StartCoroutine(Loop());
    }

    private static IEnumerator Loop()
    {
        while (true)
        {
            while (tasks.Count > 0)
            {
                var nextTask = tasks.Dequeue();
                try
                {
                    nextTask();
                }
                catch (Exception e)
                {

                    Debug.LogError(e);
                }
            }
            yield return null;
        }


    }

    public static void CallInMainThread(Action p)
    {

        print($"Action {p} added");
        tasks.Enqueue(p);
    }
}
