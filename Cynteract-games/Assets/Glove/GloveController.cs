using Cynteract.CGlove;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

//Required for macOS
using System.Runtime.InteropServices;
using System.IO.Ports;


/// <summary>
/// Actively Connects to the gloves using the GloveManager.
/// </summary>
public class GloveController : MonoBehaviour
{
    // Start is called before the first frame update

    private void Start()
    {
        GloveManager.instance.Init();
    }

    // Update is called once per frame
    void Update()
    {
        //GloveManager.instance.GetLeft();
    }

}
