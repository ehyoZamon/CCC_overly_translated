using Cynteract.CynteractInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCrosserInput : CInput
{
    public static Axis moveLeftRight = new Axis("Links-Rechts-Bewegung", 0, GloveInput.AxesEnum.ArmX, false);
    public static Action stepOver = new Action("Vorwärtsbewegung", 0, GloveInput.ActionsEnum.AngleSum, false);

    public RoadCrosserInput()
    {
        SetAxes(moveLeftRight);
        SetActions(stepOver);
    }
}
