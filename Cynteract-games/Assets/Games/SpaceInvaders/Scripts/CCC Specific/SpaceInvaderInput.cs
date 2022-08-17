using Cynteract.CynteractInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.SpaceInvaders {
    public class SpaceInvaderInput :CInput
    {
        public static Action shoot = new Action("Shoot",0, GloveInput.ActionsEnum.AngleSum, false);
        public static Axis move = new Axis("Move", 0, GloveInput.AxesEnum.ArmY, false);
        public SpaceInvaderInput()
        {
            SetActions(shoot);
            SetAxes(move);
        }
    }
}