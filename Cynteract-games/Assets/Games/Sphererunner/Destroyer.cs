using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Sphererunner
{
    public class Destroyer : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            WorldGenerator.instance.ResetPlayer();
        }
    }
}