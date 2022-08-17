using Cynteract.CynteractInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerGameInput : CInput
{
    public static Axis move = new Axis("Bewegen", 0, GloveInput.AxesEnum.AngleSum, false);
    public static Action jump = new Action("Springen", 0, GloveInput.ActionsEnum.AngleSum, false);

    public SoccerGameInput()
    {
        SetAxes(move);
        SetActions(jump);
    }
}
