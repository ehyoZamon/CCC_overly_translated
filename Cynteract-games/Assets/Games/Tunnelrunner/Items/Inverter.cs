using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Tunnelrunner
{
    public class Inverter : Collectible
    {
        public override void Effect()
        {
            Tunnelrunner.rocketControl.Invert();
        }
    }
}

