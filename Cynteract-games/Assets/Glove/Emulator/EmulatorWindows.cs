using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WindowsInput;
public class EmulatorWindows : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EmulatorInput.GetAction(EmulatorInput.next))
        {
            InputSimulator inputSimulator = new InputSimulator();
            inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.VK_A);
        }
    }
}
