using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Scripting;
using Newtonsoft.Json;

namespace Cynteract.CGlove.LinearCalibration
{
    [Serializable]
    public class LinearInterpolation
    {
        [ReadOnly]
        public int dim;
        public int resolution;
        public int pow;
        public int cornerPow;
        public bool interpolateRim;
        [ReadOnly]
        public float[] gridValues;
        [HideInInspector]
        
        public TrainingDataPack[] trainingData;
        
        [Preserve]
        [JsonConstructor]
        public LinearInterpolation()
        {

        }

        public LinearInterpolation(int size, int pow, int dim):this (size, pow, dim , 5*pow, true)
        {
        }
        public LinearInterpolation(int size, int pow, int dim, int cornerPow):this (size, pow, dim, cornerPow, true)
        {
        }
        public LinearInterpolation(int size, int pow, int dim, int cornerPow, bool interpolateRim)
        {
            this.dim = dim;
            this.resolution = size;
            this.pow = pow;
            this.cornerPow = cornerPow;
            this.interpolateRim = interpolateRim;
            GenerateGrid();
        }
        [Button(ButtonSizes.Large)]

        public void GenerateGrid()
        {
            int pow = Pow(resolution, dim);
            gridValues = new float[pow];
            trainingData = new TrainingDataPack[pow];
            for (int i = 0; i < trainingData.Length; i++)
            {
                trainingData[i] = new TrainingDataPack();
            }
        }
        public void CalculateAsync()
        {
            CalculateGrid();
        }
        public void InterpolateRim(ref BitArray valuesSet,ref VectorValue[] setValuesArray, int depth)
        {
            
            List<MultidimIndex> corners = new List<MultidimIndex>();
            List<VectorValue> setValues = new List<VectorValue>(setValuesArray);
            //Get all outer corners of the grid
            for (int i = 0; i < gridValues.Length; i++)
            {
                MultidimIndex multidimIndex = (this ^ i);
                if (multidimIndex.IsCorner(depth))
                {
                    corners.Add(multidimIndex);
                }
            }
            //Interpolate the unset corners
            for (int i = 0; i < corners.Count; i++)
            {
                int index = !corners[i];
                if (!valuesSet[index])
                {

                    gridValues[index] = Interpolate(setValuesArray, new VectorN(corners[i]),cornerPow);
                    valuesSet[index] = true;
                    setValues.Add(new VectorValue(new VectorN(corners[i]), gridValues[index]));
                }

            }
            //Interpolate Points between Corners
            for (int i = 0; i < corners.Count; i++)
            {
                for (int j = 0; j < corners.Count; j++)
                {
                    //Corners that have an edge between them
                    if(MultidimIndex.OneNorm(corners[i], corners[j]) == resolution-1)
                    {
                        List<VectorValue> localSet = new List<VectorValue>();
                        localSet.Add(new VectorValue(new VectorN(corners[i]), gridValues[!corners[i]]));
                        localSet.Add(new VectorValue(new VectorN(corners[j]), gridValues[!corners[j]]));

                        int interPolateIndex = MultidimIndex.FistDistinguishingIndex(corners[i], corners[j]);
                        //Get all gridValues between the corners, that are set
                        for (int between = 1+ depth; between < resolution-1- depth; between++)
                        {
                            MultidimIndex betweenIndexMD=corners[i].Copy();
                            betweenIndexMD[interPolateIndex] = between;
                            int betweenIndex = !betweenIndexMD;
                            if (valuesSet[betweenIndex])
                            {
                                localSet.Add(new VectorValue(new VectorN(betweenIndexMD), gridValues[betweenIndex]));
                            }
                        }
                        var localSetArray = localSet.ToArray();
                        //...Interpolate the rest between the array set of set Values
                        for (int between = 1+ depth; between < resolution-1- depth; between++)
                        {
                            MultidimIndex betweenIndexMD = corners[i].Copy();
                            betweenIndexMD[interPolateIndex] = between;
                            int betweenIndex = !betweenIndexMD;
                            if (!valuesSet[betweenIndex])
                            {
                                gridValues[betweenIndex] = Interpolate(localSetArray, new VectorN(betweenIndexMD));
                                valuesSet[betweenIndex] = true;
                                setValues.Add(new VectorValue(new VectorN(betweenIndexMD), gridValues[betweenIndex]));
                            }
                            
                        }

                    }
                }
            }
            setValuesArray = setValues.ToArray();

        }


        [Button(ButtonSizes.Large)]

        public void CalculateGrid()
        {
            BitArray valuesSet = new BitArray(gridValues.Length);
            List<VectorValue> setValues = new List<VectorValue>();
            for (int i = 0; i < gridValues.Length; i++)
            {

                gridValues[i] = InterpolateSurroundingData(this ^ i, out bool set);
                valuesSet[i] = set;
                if (set)
                {
                    setValues.Add(new VectorValue(new VectorN(this ^ i), gridValues[i]));
                }
            }

            VectorValue[] setValuesArray = setValues.ToArray();
            for (int i = 0; i <1; i++)
            {
                InterpolateRim(ref valuesSet, ref setValuesArray, i);

            }
            for (int i = 0; i < gridValues.Length; i++)
            {
                if (!valuesSet[i])
                {

                    gridValues[i] = Interpolate(setValuesArray, new VectorN(this ^ i));
                }
            }
        }

        public MultidimIndex ToMultiDim(int n)
        {

            int[] indizes = new int[dim];
            int x = n;
            for (int i = dim - 1; i >= 0; i--)
            {
                int v = Pow(resolution, i);
                indizes[i] = x / v;
                x = x % v;
            }
            return new MultidimIndex(indizes, dim, resolution);
        }
        public static MultidimIndex operator ^(LinearInterpolation linearInterpolation, int i)
        {
            return linearInterpolation.ToMultiDim(i);
        }
        public static MultidimIndex operator ^(LinearInterpolation linearInterpolation, int[] i)
        {
            return new MultidimIndex(i, linearInterpolation.dim, linearInterpolation.resolution);
        }
        public static MultidimIndex operator ^(LinearInterpolation linearInterpolation, VectorN vector)
        {
            return linearInterpolation.ToMultiDim(vector);
        }

        private MultidimIndex ToMultiDim(VectorN vector)
        {
            int[] indizes = new int[vector.Length];
            for (int i = 0; i < vector.Length; i++)
            {
                indizes[i] = (int)vector[i];
            }
            MultidimIndex multidim = new MultidimIndex(indizes, dim, resolution);
            return multidim;
        }

        private int Pow(int baseNum, int exp)
        {
            int pow = 1;

            for (int i = 0; i < exp; i++)
            {
                pow *= baseNum;
            }
            return pow;
        }
        public void AddTrainingData(float[] values, float y)
        {
            AddTrainingData(GloveToGrid(new VectorN(values)), y);
        }
        private void AddTrainingData(VectorN x, float y)
        {
            trainingData[!(this ^ x)].values.Add(new VectorValue(x, y));
        }


        public int[] GetSurrounding(MultidimIndex vector)
        {
            return GetIndizes(vector, 0, true).ToArray();
            List<int> GetIndizes(MultidimIndex mult, int depth, bool changed)
            {
                List<int> indizes = new List<int>();
                bool v = depth < dim - 1;
                MultidimIndex vec = mult.Copy();
                if (changed)
                {
                    indizes.Add(!vec);

                }
                if (v)
                {
                    indizes.AddRange(GetIndizes(vec, depth + 1, false));
                }
                vec[depth] = vec[depth] - 1;
                if (vec[depth] >= 0)
                {
                    indizes.Add(!vec);
                    if (v)
                    {
                        indizes.AddRange(GetIndizes(vec, depth + 1, false));
                    }
                }

                return indizes;
            }
        }

        public void Reset()
        {
            GenerateGrid();
        }

        public int[] GetSurrounding(VectorN vector)
        {
            return GetIndizes(vector, 0, true).ToArray();
            List<int> GetIndizes(VectorN mult, int depth, bool changed)
            {
                List<int> indizes = new List<int>();
                bool v = depth < dim - 1;
                VectorN vec = mult.Copy();
                if (changed)
                {
                    indizes.Add(!(this ^ vec));

                }
                if (v)
                {
                    indizes.AddRange(GetIndizes(vec, depth + 1, false));
                }
                vec[depth] = Mathf.Ceil(vec[depth]);

                indizes.Add(!(this ^ vec));
                if (v)
                {
                    indizes.AddRange(GetIndizes(vec, depth + 1, false));
                }
                return indizes;
            }
        }
        public int[] GetNeigbours(int index)
        {

            var multiDim = this ^ index;

            return GetIndizes(multiDim, 0).ToArray();

            List<int> GetIndizes(MultidimIndex mult, int depth)
            {
                var multidimIndex = mult.Copy();
                List<int> indizes = new List<int>();
                bool v = depth < dim - 1;


                if (v)
                {
                    indizes.AddRange(GetIndizes(multidimIndex, depth + 1));
                }

                multidimIndex[depth] -= 1;
                if (multidimIndex[depth] >= 0)
                {
                    indizes.Add(!multidimIndex);
                    if (v)
                    {
                        indizes.AddRange(GetIndizes(multidimIndex, depth + 1));
                    }
                }

                multidimIndex[depth] += 2;
                if (multidimIndex[depth] < resolution)
                {

                    indizes.Add(!multidimIndex);


                    if (v)
                    {
                        indizes.AddRange(GetIndizes(multidimIndex, depth + 1));
                    }
                }

                return indizes;
            }


        }
        public float InterpolateSurrounding(VectorN vector)
        {
            int[] surroundingIndizes = GetSurrounding(vector);
            VectorValue[] surrounding = ToVectorValues(surroundingIndizes);
            return Interpolate(surrounding, vector);
        }
        public float InterpolateSurroundingData(MultidimIndex index)
        {
            int[] surroundingIndizes = GetSurrounding(index);
            VectorValue[] surrounding = GetTrainingData(surroundingIndizes);
            return Interpolate(surrounding, new VectorN(index));
        }

        public float GetValue(VectorN v)
        {
            VectorN vector = GloveToGrid(v);
            return InterpolateSurrounding(vector);
        }

        public VectorN GloveToGrid(VectorN vectorN)
        {
            float[] values = new float[vectorN.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = Mathf.Clamp((vectorN[i] + 30) / 120*resolution, 0, resolution-1);
            }
            VectorN gridVector = new VectorN(values);
            return gridVector;
        }

        public float InterpolateSurroundingData(MultidimIndex index, out bool set)
        {
            int[] surroundingIndizes = GetSurrounding(index);
            VectorValue[] surrounding = GetTrainingData(surroundingIndizes);
            set = surrounding.Length > 0;
            return Interpolate(surrounding, new VectorN(index));
        }
        private VectorValue[] GetTrainingData(int[] surroundingIndizes)
        {
            List<VectorValue> values = new List<VectorValue>();
            foreach (var item in surroundingIndizes)
            {
                values.AddRange(trainingData[item].values);
            }
            return values.ToArray();
        }
        private float Interpolate(VectorValue[] vectors, VectorN vector, int pow)
        {
            float numerator = 0;
            float denominator = 0;
            foreach (var item in vectors)
            {
                float dist = VectorN.Distance(vector, item.x,pow);
                if (dist == 0)
                {
                    return item.y;

                }

                float w = 1 / dist;
                
                numerator += w * item.y;
                denominator += w;
            }

            if (denominator == 0)
            {
                denominator = 1;
            }
            return numerator / denominator;
        }
        private float Interpolate(VectorValue[] vectors, VectorN vector)
        {
            return Interpolate(vectors, vector, pow);
        }
        private VectorValue[] ToVectorValues(int[] surroundingIndizes)
        {
            VectorValue[] surrounding = new VectorValue[surroundingIndizes.Length];
            for (int i = 0; i < surrounding.Length; i++)
            {

                try
                {
                    int v = surroundingIndizes[i];
                    surrounding[i]
                        = new VectorValue(new VectorN(this ^ v), gridValues[v]);
                }
                catch (Exception)
                {

                    throw;
                }


            }

            return surrounding;
        }


        [Button(ButtonSizes.Large)]
        public void DrawGrid()
        {
            InterpolationDrawer.DrawGrid(this);
            
        }
    }

    [Serializable]
    public class TrainingDataPack
    {
        public List<VectorValue> values;

        public TrainingDataPack()
        {
            values = new List<VectorValue>();
        }
    }
    [Serializable]
    public struct VectorValue
    {
        public VectorN x;
        public float y;

        public VectorValue(VectorN vector, float value)
        {
            this.x = vector;
            this.y = value;
        }
    }
    [Serializable]
    public struct MultidimIndex
    {
        int[] indizes;
        int dim, size;


        public MultidimIndex(int[] indizes, int dim, int size)
        {
            this.indizes = indizes;
            this.dim = dim;
            this.size = size;
        }

        public int this[int i]
        {
            get
            {
                return indizes[i];
            }
            set
            {
                indizes[i] = value;
            }
        }

        public int Length
        {
            get
            {
                return indizes.Length;

            }
        }
        public static int OneNorm(MultidimIndex multidimIndex1, MultidimIndex multidimIndex2)
        {
            int sum = 0;
            for (int i = 0; i < multidimIndex1.dim; i++)
            {
                sum += Mathf.Abs(multidimIndex1[i] - multidimIndex2[i]);
            }
            return sum;
        }
        public static int operator ^(MultidimIndex multiDimIndex, int size)
        {
            int index = 0;
            int pow = 1;
            for (int i = 0; i < multiDimIndex.Length; i++)
            {
                index += multiDimIndex[i] * pow;
                pow *= size;
            }
            return index;
        }
        public static int operator !(MultidimIndex multiDimIndex)
        {
            return multiDimIndex ^ multiDimIndex.size;
        }

        public MultidimIndex Copy()
        {
            int[] array = new int[dim];
            Array.Copy(indizes, array, dim);
            return new MultidimIndex(array, dim, size);
        }

        public bool IsCorner(int depth)
        {
            foreach (var item in indizes)
            {
                if (item!= depth && item!=size-1- depth)
                {
                    return false;
                }
            }
            return true;
        }

        public static int FistDistinguishingIndex(MultidimIndex multidimIndex1, MultidimIndex multidimIndex2)
        {
            for (int i = 0; i < multidimIndex1.dim; i++)
            {
                if (multidimIndex1[i]!=multidimIndex2[i])
                {
                    return i;
                }
            }
            return -1;
        }

    }
}