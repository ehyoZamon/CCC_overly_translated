using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Sphererunner
{
    public class SphererunnerPart : MonoBehaviour
    {
        public GameObject threeDimensionalPart, twoDimensionalPart;
        private void Start()
        {
            if (WorldGenerator.instance.twoDimensionalGraphics)
            {
                threeDimensionalPart.SetActive(false);

                twoDimensionalPart.SetActive(true);

            }
            else
            {
                threeDimensionalPart.SetActive(true);
                twoDimensionalPart.SetActive(false);

            }
        }
    }
}