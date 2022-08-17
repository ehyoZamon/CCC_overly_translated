using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ShutDownPc 
{
    public static void ShutDown()
    {
        var psi = new ProcessStartInfo("shutdown", "/s /t 0");
        psi.CreateNoWindow = true;
        psi.UseShellExecute = false;
        Process.Start(psi);
    }
}
