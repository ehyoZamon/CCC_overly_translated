#if UNITY_EDITOR

using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.GameTesting
{
    [Serializable]
    public class GameTesting
    {
        [ReadOnly]
        public static GameTestingMono instantiatedGameTesting;
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            if( GameTestingWindow.instance != null&& GameTestingWindow.instance.Active)
            {
                var gO = new GameObject();
                gO.name = "Game Testing";
                instantiatedGameTesting = gO.AddComponent<GameTestingMono>();
            }

        }

    }

}
#endif