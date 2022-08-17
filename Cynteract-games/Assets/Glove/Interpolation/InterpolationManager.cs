#if UNITY_EDITOR
using Cynteract.CGlove;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;


namespace Cynteract.CGlove.LinearCalibration
{

    public class InterpolationManager : SerializedMonoBehaviour
    {

        [Serializable]
        public class Hand
        {
            public FingerSelection[] fingerParts;

            public Side side;
            public  float GetAngle(Fingerpart fingerpart, GloveData.SplitValues.FloatValue value)
            {
                switch (side)
                {
                    case Side.Left:
                        return GetFingerAngle(fingerpart, value).GetYValue(Glove.Left);
                    case Side.Right:
                        return GetFingerAngle(fingerpart, value).GetYValue(Glove.Right);

                    default:
                        return 0;
                }
                
            }

            private FingerAngle GetFingerAngle(Fingerpart fingerPart, GloveData.SplitValues.FloatValue value)
            {
                foreach (var part in fingerParts)
                {
                    if (fingerPart == part.fingerpart)
                    {
                        foreach (var item in part.fingerAngles)
                        {
                            if (item.value == value)
                            {
                                return item;
                            }
                        }
                    }
                }
                return null;

            }
            public void SetDimensions()
            {

                foreach (var part in fingerParts)
                {
                    foreach (var angle in part.fingerAngles)
                    {
                        if (angle.valuesToUse==null)
                        {
                            angle.valuesToUse = new ValueToUse[0];
                        }
                        if (angle.linearInterpolation==null)
                        {
                            angle.linearInterpolation = new LinearInterpolation(0, 0, 0);
                        }


                        angle.linearInterpolation.dim = angle.valuesToUse.Length;
                        
                    }
                }

            }

            public void SetStandardValues()
            {
                SetDimensions();

                foreach (var part in fingerParts)
                {
                    foreach (var angle in part.fingerAngles)
                    {
                        angle.linearInterpolation.resolution = StandardResolution;
                        angle.linearInterpolation.pow = StandardPow;
                        angle.linearInterpolation.cornerPow = StandardCornerPow;
                        angle.linearInterpolation.interpolateRim = true;
                    }
                }
            }
            public void GenerateGrids()
            {
                foreach (var part in fingerParts)
                {
                    foreach (var angle in part.fingerAngles)
                    {
                        angle.linearInterpolation.GenerateGrid();
                    }
                }
            }


        }
        [Serializable]
        public class FingerSelection
        {
            public Fingerpart fingerpart;
            [GUIColor(1f, .9f, .9f)]
            public FingerAngle[] fingerAngles;
            [NonSerialized]
            public InterpolationWindow.Finger finger;
        }
        [Serializable]
        public class FingerAngle
        {
            public enum Datatype
            {
                Glove, Custom
            }
            [VerticalGroup("Add")]
            public Datatype datatype;
            public GloveData.SplitValues.FloatValue value;
            [GUIColor(.7f,.7f,1f)]
            public ValueToUse[] valuesToUse;

            [VerticalGroup("Add")]
            [ShowInInspector]
            [ShowIf("datatype", Datatype.Custom)]

            private float [] customValues;

            [HideLabel]
            [HorizontalGroup("Add/H")]
            public float angle;
            public LinearInterpolation linearInterpolation;

            [NonSerialized]
            public FingerSelection fingerPart;
            public VectorN GetXValues(Glove glove)
            {
                float[] values = new float[valuesToUse.Length];
                for (int i = 0; i < valuesToUse.Length; i++)
                {
                    values[i] = Glove.LeftData[valuesToUse[i].part].splitValues.Get(valuesToUse[i].value);
                }
                VectorN vector = new VectorN(values);
                return vector;
            }
            public float GetYValue(Glove glove)
            {
                return linearInterpolation.GetValue(GetXValues(glove));
            }

            [Button(ButtonSizes.Large)]
            [HorizontalGroup("Add/H")]
            public void AddValue()
            {
                float[] values = new float[valuesToUse.Length];
                float y=angle;
                switch (datatype)
                {
                    case Datatype.Glove:
                        switch (fingerPart.finger.hand.side)
                        {
                            case Side.NotSet:
                                break;
                            case Side.Left:
                                
                                for (int i = 0; i < values.Length; i++)
                                {
                                    values[i]=Glove.LeftData[fingerPart.fingerpart].splitValues.Get(value);
                                }
                                break;
                            case Side.Right:
                                for (int i = 0; i < values.Length; i++)
                                {
                                    values[i] = Glove.LeftData[fingerPart.fingerpart].splitValues.Get(value);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case Datatype.Custom:
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] =customValues[i];
                        }
                        break;
                    default:
                        break;
                }
                linearInterpolation.AddTrainingData(values, y);
            }
            [Button("90", ButtonSizes.Large)]
            [HorizontalGroup("Add/H")]
            public void Add90()
            {

            }

        }
        [Serializable]
        public class ValueToUse
        {
            public Fingerpart part;
            public GloveData.SplitValues.FloatValue value;
        }

        private const int StandardResolution = 10;
        private const int StandardPow = 1;
        public const int StandardCornerPow = 5;
        [ShowInInspector]
        public static Hand left, right;

    }
}
#endif