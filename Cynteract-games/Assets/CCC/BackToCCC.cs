using Cynteract;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.CCC {
    public class BackToCCC : MonoBehaviour
    {
        public void QuitGame()
        {
            #warning score will be set to 0 -Will it or is the warning deprecated?
            GameTrainingController.instance.StopGames();
        }
    }
}