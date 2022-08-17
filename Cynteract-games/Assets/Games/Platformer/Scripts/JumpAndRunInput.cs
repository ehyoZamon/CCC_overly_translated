using Cynteract.CynteractInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Platformer
{
    public class JumpAndRunInput : CInput
    {
        public static Axis move = new Axis("Bewegung", 0, GloveInput.AxesEnum.ArmX, false);
        public static Action jump = new Action("Springen", 0, GloveInput.ActionsEnum.AngleSum, false);

        public JumpAndRunInput()
        {
            SetAxes(move);
            SetActions(jump);
        }
    }
}
