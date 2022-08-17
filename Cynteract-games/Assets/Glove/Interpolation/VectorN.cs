using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cynteract.CGlove.LinearCalibration
{
    [Serializable]
    public class VectorN
    {
        float[] values;

        public VectorN(params float[] values)
        {
            this.values = values;
        }
        public VectorN(MultidimIndex multidimIndex)
        {
            values = new float[multidimIndex.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = multidimIndex[i];
            }
        }
        public float this[int i]
        {
            get
            {
                return values[i];
            }
            set
            {
                values[i] = value;
            }
        }
        public int Length
        {
            get
            {
                return values.Length;
            }
        }
        public static float Distance(VectorN a, VectorN b, int pow)
        {
            float sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                float diff = a[i] - b[i];
                float powDiff = Mathf.Abs(Mathf.Pow(diff, pow));
                sum += powDiff ;
            }
            return Mathf.Pow(sum, 1/((float)pow));
        }
        public static float Distance(VectorN a, VectorN b)
        {
            return Distance(a, b, 2);
        }
        public override string ToString()
        {
            string s = "{";
            foreach (var item in values)
            {
                s += item + ",";
            }
            s.TrimEnd(',');
            return s + "}";
        }
        public VectorN Copy()
        {
            float[] values = new float[this.values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = this.values[i];

            }
            return new VectorN(values);
        }
    }
}