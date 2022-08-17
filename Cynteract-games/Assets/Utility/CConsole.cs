using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;

public enum CSubConsoleType
{
    Main,
    Glove,
    CCC
}
[Serializable]
public struct CConsoleMessage
{
    public enum Type
    {
        Message,
        Warning,
        Error
    }
    [HorizontalGroup]
    [ReadOnly]
    [GUIColor("GetColor")]
    [HideLabel]
    public string message;

    private readonly Type type;

    public CConsoleMessage(string message, Type type)
    {
        this.message = message;
        this.type = type;
    }

    public Color GetColor()
    {
        switch (type)
        {
            case Type.Message:
                return Color.white;
            case Type.Warning:
                return Color.yellow;

            case Type.Error:
                return Color.red;
            default:
                return Color.white;
        }
    }
}
public class CConsole : MonoBehaviour
{

    public static Dictionary<CSubConsoleType, CSubConsole> subConsoles;


    public static void Log(string message)
    {
        Log(message, CSubConsoleType.Main);
    }
    public static void Log(string message, CSubConsoleType type)
    {
        Add(new CConsoleMessage(message, CConsoleMessage.Type.Message), type);
    }

    public static void LogError(string message)
    {
        LogError(message, CSubConsoleType.Main);
    }
    public static void LogError(Exception e, CSubConsoleType type)
    {
        LogError(e.Message, type);
    }
    public static void LogError(string message, CSubConsoleType type)
    {
        Add(new CConsoleMessage(message, CConsoleMessage.Type.Error), type);
    }
    public static Queue<CConsoleMessage> GetMessages(CSubConsoleType type)
    {
        if (subConsoles == null)
        {
            subConsoles = new Dictionary<CSubConsoleType, CSubConsole>();
        }
        if (!subConsoles.ContainsKey(type))
        {
            subConsoles.Add(type, new CSubConsole());
        }
        return subConsoles[type].messages;
    }

    private static void Add(CConsoleMessage message, CSubConsoleType type)
    {
        if (subConsoles == null)
        {
            subConsoles = new Dictionary<CSubConsoleType, CSubConsole>();
        }
        if (!subConsoles.ContainsKey(type))
        {
            subConsoles.Add(type, new CSubConsole());
        }
        subConsoles[type].Add(message);
#if UNITY_EDITOR
#endif
    }


}
public class CSubConsole
{
    public Queue<CConsoleMessage> messages;

    public CSubConsole()
    {
        messages = new Queue<CConsoleMessage>();
    }

    public void Add(CConsoleMessage message)
    {
        messages.Enqueue(message);
    }
}