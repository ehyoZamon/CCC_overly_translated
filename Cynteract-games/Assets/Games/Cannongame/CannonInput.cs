using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynteract.CynteractInput;
using UnityEngine.TextCore;

namespace Cynteract.Cannongame
{
    public class CannonInput : CInput
    {
        public static Axis aim = new Axis("Zielen", 0, GloveInput.AxesEnum.AngleSum, false);
        public static Action shoot = new Action("Schießen", 0, GloveInput.ActionsEnum.AngleSum, false);

        public CannonInput()
        {
            SetAxes(aim);
            SetActions(shoot);
        }
    }
}