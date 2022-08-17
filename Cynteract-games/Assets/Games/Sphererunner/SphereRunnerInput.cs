
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynteract.CGlove;
using Cynteract.CynteractInput;
namespace Cynteract.Sphererunner
{


    public class SphereRunnerInput : CInput
    {
       // public static Axis movement = new Axis("Bewegung", 0, GloveInput.AxesEnum.AngleSum, false);
        public static Axis size = new Axis("Größe", 0, GloveInput.AxesEnum.AngleSum, false);
        public SphereRunnerInput()
        {
            SetActions();
            SetAxes(size);
            //SetAxes(movement, size);
        }

    }

}
