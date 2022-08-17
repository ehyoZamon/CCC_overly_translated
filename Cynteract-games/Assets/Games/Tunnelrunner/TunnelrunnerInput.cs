using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynteract.CynteractInput;
namespace Cynteract.Tunnelrunner
{
    public class TunnelrunnerInput : CInput
    {
        public static Action boost = new Action("Boost", 0, GloveInput.ActionsEnum.AngleSum, false);

        public TunnelrunnerInput()
        {
            SetAxes();
            SetActions(boost);
        }
    }
}