using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Platformer
{
    public class OutOfLevelKiller : MonoBehaviour
    {

        private void OnTriggerExit2D(Collider2D collision)
        {
            Debug.Log("Hallo");
            if (collision.GetComponent<PlayerMovement>())
            {
                LevelController.instance.Respawn();
            }
        }
    }
}