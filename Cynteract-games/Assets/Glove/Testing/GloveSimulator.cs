using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.CGlove
{
    public class GloveSimulator : MonoBehaviour
    {
        private void Update()
        {
            Glove.Left.OnDataChanged(new DataReceive()
            {
                imu = new IMUData[16],
                force = new short[5]
            }
            ) ;
        }
    }
}

