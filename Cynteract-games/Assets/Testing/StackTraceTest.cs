using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StackTraceTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StackTrace st = new StackTrace(true);
        print(st.ToString());
        for (int i = 0; i < st.FrameCount; i++)
        {
            // Note that high up the call stack, there is only
            // one stack frame.
            StackFrame sf = st.GetFrame(i);
           
            UnityEngine.Debug.Log("High up the call stack, Method: "+sf.GetMethod());

            UnityEngine.Debug.Log("High up the call stack, Line Number: "+sf.GetFileLineNumber());
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
