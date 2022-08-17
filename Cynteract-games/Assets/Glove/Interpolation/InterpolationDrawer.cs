using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.CGlove.LinearCalibration {

    public class InterpolationDrawer
    {
        public static void DrawGrid(LinearInterpolation linearInterpolation)
        {
            switch (linearInterpolation.dim)
            {
                case 2:
                    for (int i = 0; i < linearInterpolation.gridValues.Length; i++)
                    {
                        var index = linearInterpolation ^ i;
                        var neighbourI = linearInterpolation.GetNeigbours(i);
                        foreach (var item in neighbourI)
                        {
                            var neighbourIndex = linearInterpolation ^ item;
                            Debug.DrawLine(new Vector3(index[0], linearInterpolation.gridValues[i], index[1]), new Vector3(neighbourIndex[0], linearInterpolation.gridValues[item], neighbourIndex[1]), Color.green, 10f);
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < linearInterpolation.gridValues.Length; i++)
                    {
                        var index = linearInterpolation ^ i;
                        var neighbourI = linearInterpolation.GetNeigbours(i);
                        foreach (var item in neighbourI)
                        {
                            var neighbourIndex = linearInterpolation ^ item;
                            float v = linearInterpolation.gridValues[i];
                            Debug.DrawLine(new Vector3(index[0], index[1], index[2]), new Vector3(neighbourIndex[0], neighbourIndex[1], neighbourIndex[2]), new Color(v, 0, 0), 10f);
                        }
                    }
                    break;
            }
        }
    }
}