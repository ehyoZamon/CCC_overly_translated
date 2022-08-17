#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;

using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using static CConsole;

public class ConsoleWindow : OdinEditorWindow
{
    public CSubConsoleType subConsole;
    public static bool changed;
    [ShowInInspector]
    
    Queue<CConsoleMessage> messages;

    public static List<ConsoleWindow> consoleWindows;
    protected override void OnGUI()
    {
        messages = CConsole.GetMessages(subConsole);
        base.OnGUI();
    }

    public void Refresh()
    {
        EditorUtility.SetDirty(this);
    }
    [MenuItem("Tools/Console")]
    private static void OpenWindow()
    {
        var window = CreateInstance<ConsoleWindow>();
        window.Show();
        if (consoleWindows==null)
        {
            consoleWindows = new List<ConsoleWindow>();
        }
        consoleWindows.Add(window);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (consoleWindows!=null)
        {

        consoleWindows.Remove(this);
        }

    }
}
#endif