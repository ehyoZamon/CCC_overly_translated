using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynteract.CGlove;
using WindowsInput;
using WindowsInput.Native;
using System.Runtime.InteropServices;

public class Emulator : MonoBehaviour
{
    private const string DllFilePath = "KeyPressLib";
    [DllImport(DllFilePath, CallingConvention = CallingConvention.Cdecl)]
    private extern static int PressKey(int number);

    [DllImport(DllFilePath, CallingConvention = CallingConvention.Cdecl)]
    private extern static int HoldKey(int number);

    [DllImport(DllFilePath, CallingConvention = CallingConvention.Cdecl)]
    private extern static int ReleaseKey(int number);


    public Dictionary<KeyCode, int> KeyCodeToCppKey = new Dictionary<KeyCode, int>();
    public static Emulator instance;

    public bool justPressed;

    public Emulator()
    {
        instance = this;
        KeyCodeToCppKey.Add(KeyCode.Mouse0, 0x01);
        KeyCodeToCppKey.Add(KeyCode.LeftArrow, 0x25);
        KeyCodeToCppKey.Add(KeyCode.UpArrow, 0x26);
        KeyCodeToCppKey.Add(KeyCode.RightArrow, 0x27);
        KeyCodeToCppKey.Add(KeyCode.DownArrow, 0x28);

        KeyCodeToCppKey.Add(KeyCode.A, 0x41);



    }
    private void Update()
    {
        if (EmulatorInput.GetAction(EmulatorInput.next)) 
        {
            if (!justPressed)
            {
                justPressed = true;
                PressKey(KeyCode.RightArrow);
            }
        }
        else
        {
            justPressed = false;
        }
    }
    public void PressKey(KeyCode keyCode)
    {
        PressKey(KeyCodeToCppKey[keyCode]);
    }

    public void HoldKey(KeyCode keyCode)
    {
        HoldKey(KeyCodeToCppKey[keyCode]);
    }
    public void ReleaseKey(KeyCode keyCode)
    {
        ReleaseKey(KeyCodeToCppKey[keyCode]);
    }
}

