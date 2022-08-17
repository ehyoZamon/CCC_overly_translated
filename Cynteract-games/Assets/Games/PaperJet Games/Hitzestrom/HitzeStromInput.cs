using Cynteract.CynteractInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Hitzestrom
{
    public class HitzeStromInput : CInput
    {
        public static Axis leftRight = new Axis("Bewegung", 0, GloveInput.AxesEnum.AngleSum, false);

        public HitzeStromInput()
        {
            SetAxes(leftRight);
            SetActions();
        }
    }
}

