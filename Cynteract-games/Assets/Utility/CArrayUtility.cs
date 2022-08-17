using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Utility
{
    public class CArrayUtility : MonoBehaviour
    {
        #region Old
        public delegate float floatOperation(float input);
        internal static float[] Map(float[] array, floatOperation op)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = op(array[i]);

            }
            return array;
        }
        public static string ArrayToString(Array array)
        {
            string s = "[";
            foreach (var item in array)
            {
                s += item;
                s += ",";
            }
            s = s.Substring(0, s.Length - 1);
            s += "]";
            return s;
        }
        internal static float[] Map(float[] array, floatOperation op, int startIndex)
        {
            for (int i = startIndex; i < array.Length; i++)
            {
                array[i] = op(array[i]);

            }
            return array;
        }
        public static float[] InterpolateArray(float[] heights, int interpolationSteps)
        {
            List<float> interpolatedHeigths = new List<float>();
            for (int i = 0; i < heights.Length; i++)
            {
                interpolatedHeigths.Add(heights[i]);

                if (i < heights.Length - 1)
                {
                    for (int j = 0; j < interpolationSteps; j++)
                    {
                        interpolatedHeigths.Add(Mathf.Lerp(interpolatedHeigths[interpolatedHeigths.Count - 1], heights[i + 1], (float)(j + 1) / (interpolationSteps + 1)));
                    }

                }

            }
            float[] newHeights = interpolatedHeigths.ToArray();
            return newHeights;
        }

        internal static float[] MapAddRandom(float[] array, float v1, float v2, int startIndex, int cutBack, float top, float[] diameters)
        {
            float[] newArray = new float[array.Length];
            for (int i = 0; i < startIndex; i++)
            {
                newArray[i] = array[i];
            }
            for (int i = startIndex; i < array.Length - cutBack; i++)
            {
                float clampiClamp = Mathf.Abs(top - array[i]) - diameters[i] / 2;
                newArray[i] = array[i] + Mathf.Clamp(UnityEngine.Random.Range(v1, v2), -clampiClamp, clampiClamp);

            }
            for (int i = array.Length - cutBack; i < array.Length; i++)
            {
                newArray[i] = array[i];

            }
            return newArray;
        }
        #endregion
        public static T[] Join<T>(params T[][] arrays)
        {
            if (arrays == null)
            {
                return null;
            }
            int length = 0;

            foreach (var array in arrays)
            {
                length += array.Length;
            }
            var result = new T[length];
            int offset = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                for (int j = 0; j < arrays[i].Length; j++)
                {
                    result[j + offset] = arrays[i][j];
                }
                offset += arrays[i].Length;
            }
            return result;
        }
        public static T[][] CutIntoParts<T>(T[] array, params int[] partSizes)
        {
            T[][] result = new T[partSizes.Length][];
            int offset = 0;
            for (int i = 0; i < partSizes.Length; i++)
            {
                result[i] = new T[partSizes[i]];
                for (int j = 0; j < partSizes[i]; j++)
                {
                    result[i][j] = array[j + offset];
                }
                offset += partSizes[i];
            }
            return result;
        }
        public static T[][] CutIntoEqualParts<T>(T[] array, int partSize)
        {
            int[] sizes = new int[array.Length / partSize];
            for (int i = 0; i < sizes.Length; i++)
            {
                sizes[i] = partSize;
            }

            return CutIntoParts(array, sizes);
        }
    }
}