using Cynteract.CynteractInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmulatorInput : CInput
{
    public static Action next = new Action("Next", 0, GloveInput.ActionsEnum.AngleSum, false);

    public EmulatorInput()
    {
        SetActions(next);
    }
}
