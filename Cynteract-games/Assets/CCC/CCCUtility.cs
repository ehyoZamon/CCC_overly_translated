using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.CCC {
    public static class CCCUtility
    {
        public static string FormatTime(TimeSpan timeSpan)
        {
            return String.Format("{0:00}:{1:00}",timeSpan.Minutes, timeSpan.Seconds);
        }
    }

}