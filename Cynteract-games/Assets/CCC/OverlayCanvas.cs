using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayCanvas : MonoBehaviour
{

    public static OverlayCanvas instance;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
