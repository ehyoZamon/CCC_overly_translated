using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cynteract.Database
{
    [Serializable]
    public class UserSettings
    {
        public DateTime lastChanged;
        public bool useVibration;
        public string language;
        public bool showWelcomeScreen;
        public float globalVolume;
        [JsonConstructor]
        public UserSettings(DateTime lastChanged, bool useVibration, string language, bool showWelcomeScreen, float globalVolume)
        {
            this.lastChanged = lastChanged;
            this.useVibration = useVibration;
            this.language = language;
            this.showWelcomeScreen = showWelcomeScreen;
            this.globalVolume = globalVolume;
        }
        public UserSettings()
        {
            
        }
    }
}