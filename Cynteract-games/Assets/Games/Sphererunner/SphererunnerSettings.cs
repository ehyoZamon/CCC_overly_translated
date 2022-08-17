using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cynteract.Sphererunner
{
    public class SphererunnerSettings : GameSettings
    {

        public SphererunnerSettings()
        {
            namedValues = new Dictionary<string, Value>() {
                { CSettings.cameraShake, false },
                { CSettings.gameSpeed, 1f },
                { CSettings.Sphererunner.autoMove , true},
                { CSettings.vibrate , true}
            };
            settingsname = "SphererunnerSettings";
        }
    }
}