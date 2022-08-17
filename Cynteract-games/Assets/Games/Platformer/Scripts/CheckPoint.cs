using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cynteract.Platformer
{
    public class CheckPoint : MonoBehaviour
    {
        public GameObject partay;
        public bool triggered;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.name == "Player")
            {
                if (triggered)
                {
                    return;
                }
                triggered = true;
                Instantiate(partay, transform.position, Quaternion.identity);
                PlayerMovement.instance.ActivatePartay();
                JumpAndRun.instance.SpawnNextLevel();
            }
        }


    }
}
