using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneUtility 
{
    public static void ReloadScene()
    {
        SceneManager.LoadScene(0);
        SceneManager.sceneLoaded += OnSceneLoaded();
    }

    private static UnityAction<Scene, LoadSceneMode> OnSceneLoaded()
    {
        throw new NotImplementedException();
    }
}
