

using Cynteract.CGlove;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cynteract.CGlove.Testing
{
    public class RotaionTester : MonoBehaviour
    {

        public Transform wristCube, wristCubeRelative, wristCubeCalibrated, wristCubeForward, upCube, yCube;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            GloveData rightData = Glove.AnyData;
            if (Input.GetKeyDown(KeyCode.R))
            {
                rightData.ResetWrist();
            }
            GloveData.RotationSet wristRotation = rightData.wristRotation;
            RotateCube(wristRotation.absolute, wristCube);
            RotateCube(wristRotation.relative, wristCubeRelative);
            RotateCube(wristRotation.calibrated, wristCubeCalibrated);
            RotateCube(wristRotation.splitValues.zRot, wristCubeForward);

            RotateCube(wristRotation.splitValues.upRot, upCube);
            RotateCube(wristRotation.splitValues.yRot, yCube);
        }



        private void OnDrawGizmos()
        {
            DrawGizmos(wristCube);
            DrawGizmos(wristCubeRelative);
            DrawGizmos(wristCubeCalibrated);
            DrawGizmos(upCube);
        }

        private void DrawGizmos(Transform wristCube)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(wristCube.position, wristCube.right);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(wristCube.position, wristCube.up);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(wristCube.position, wristCube.forward);
        }
        private void RotateCube(Quaternion rotation, Transform cube)
        {
            if (cube != null)
            {
                cube.rotation = rotation;
            }
        }
    }
}
