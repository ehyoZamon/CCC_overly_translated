using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cynteract.CGlove
{
    /// <summary>
    /// Contains sensory Data of the Glove
    /// </summary>
    [Serializable]
    public class GloveData
    {
        [NonSerialized]
        Glove glove;
        [Serializable]
        public class RotationSet
        {
            public enum RotationType { 
                Absolute,
                Reset,
                Relative,
                Calibrated
            }
            /// <summary>
            /// The absolute Rotation
            /// </summary>
            public Quaternion absolute;

            /// <summary>
            /// The reset Rotation
            /// </summary>
            public Quaternion reset;

            /// <summary>
            ///Rotation relative to the wrist (for the wrist, the reset wrist): 
            /// </summary>
            public Quaternion relative;
            /// <summary>
            ///Relative rotation calibrated by currenty unimplemented function.
            /// </summary>
            public Quaternion calibrated;
            /// <summary>
            ///Relative rotation split into <see cref="splitValues"/>.
            /// </summary>
            public SplitValues splitValues;
            /// <summary>
            /// Sets all Rotations to <see cref="Quaternion.identity"/>.
            /// </summary>
            public RotationSet()
            {
                absolute = Quaternion.identity;
                relative = Quaternion.identity;
                calibrated = Quaternion.identity;
            }
            /// <summary>
            /// Sets absolute Rotation to value. Sets other Rotations to <see cref="Quaternion.identity"/>.
            /// </summary>
            public RotationSet(Quaternion value)
            {
                absolute = value;
                relative = Quaternion.identity;
                calibrated = Quaternion.identity;
            }
            /// <summary>
            /// Sets absolute, relative and calibrated Rotation
            /// </summary>
            public RotationSet(Quaternion absolute, Quaternion reset, Quaternion relative, Quaternion calibrated)
            {
                this.absolute = absolute;
                this.reset = reset;
                this.relative = relative;
                this.calibrated = calibrated;
            }
            /// <summary>
            /// Returns RotationSet with relative Rotation set to Rotation relative to the absolute Rotation
            /// </summary>
            public RotationSet CalcRelativeTo(Quaternion absolute)
            {
                relative = Quaternion.Inverse(absolute) * this.reset;
                return this;
            }
            /// <summary>
            /// Returns RotationSet with relative Rotation set to Rotation relative to the absolute Rotation of absolute/>
            /// </summary>
            public RotationSet CalcRelativeTo(RotationSet absolute)
            {
                CalcRelativeTo(absolute.reset);
                return this;

            }
            public RotationSet Copy()
            {
                return new RotationSet(absolute, reset, relative, calibrated)
                {
                    splitValues = splitValues
                };
            }
            public Quaternion GetRotation (RotationType type)
            {
                switch (type)
                {
                    case RotationType.Absolute:
                        return absolute;
                    case RotationType.Reset:
                        return reset;
                    case RotationType.Relative:
                        return relative;
                    case RotationType.Calibrated:
                        return calibrated;
                    default:
                        throw new Exception("Type not Found");
                }
                
            }
        }


        [Serializable]
        public struct SplitValues
        {
            public enum FloatValue
            {
                X, Y, Z
            }
            public Quaternion xRot, yRot, xyRot, zRot, upRot, supination;
            public float xAngle, yAngle, zAngle, upAngle;
            public float[] ToValues()
            {
                return new float[] { xAngle, yAngle, zAngle };
            }

            public float Get(FloatValue value)
            {
                switch (value)
                {
                    case FloatValue.X:
                        return xAngle;
                    case FloatValue.Y:
                        return yAngle;
                    case FloatValue.Z:
                        return zAngle;
                    default:
                        return 0;
                }
            }
        }
        [SerializeField]
        private RotationSet[] rotations;
        [SerializeField]
        private float[] forces;
        /// <summary>
        /// The Rotation of the wrist.
        /// </summary>
        public RotationSet wristRotation = new RotationSet();
        private Quaternion wristReset = Quaternion.identity;
        private Quaternion[] fingerResets;
        private Quaternion wristOffset= new Quaternion(-0.1f, 0.7f, 0.2f, 0.6f);
        public Action onReset;

        public GloveData(Glove glove)
        {
            this.glove = glove;
        }
        public RotationSet[] GetRotationsCopy()
        {
            RotationSet[] rotationSets = new RotationSet[rotations.Length];
            for (int i = 0; i < rotationSets.Length; i++)
            {
                rotationSets[i] = rotations[i].Copy();
            }
            return rotationSets;
        }
        /// <summary>
        /// Method called, when new Data is recieved from the Glove
        /// </summary>
        /// <param name="data">data Recieved</param>
        public void Calculate(DataReceive data)
        {
            if (data.imu==null)
            {
                return;
            }
            rotations = new RotationSet[data.imu.Length];
            for (int i = 0; i < data.imu.Length; i++)
            {
                rotations[i] = new RotationSet(ToUnityCoordinates((Quaternion)data.imu[i]));
            }
            if (fingerResets==null)
            {
                fingerResets = new Quaternion[rotations.Length];
                for (int i = 0; i < fingerResets.Length; i++)
                {
                    fingerResets[i] = Quaternion.identity;
                }
            }
            for (int i = 0; i < rotations.Length; i++)
            {
                rotations[i].reset = fingerResets[i] * rotations[i].absolute;
            }

            wristRotation = this[Fingerpart.palmCenter];
            wristRotation.reset = wristReset*wristRotation.absolute ;
            wristRotation.relative =wristRotation.reset;
            wristRotation.splitValues = SplitValue(wristRotation.reset);

            this[Fingerpart.thumbBase]= this[Fingerpart.thumbBase].CalcRelativeTo(wristRotation);
            this[Fingerpart.thumbCenter].CalcRelativeTo(this[Fingerpart.thumbBase]);
            this[Fingerpart.thumbTop].CalcRelativeTo(this[Fingerpart.thumbCenter]);

            this[Fingerpart.indexBase].CalcRelativeTo(wristRotation);
            this[Fingerpart.indexCenter].CalcRelativeTo(this[Fingerpart.indexBase]);

            this[Fingerpart.middleBase].CalcRelativeTo(wristRotation);
            this[Fingerpart.middleCenter].CalcRelativeTo(this[Fingerpart.middleBase]);

            this[Fingerpart.ringBase].CalcRelativeTo(wristRotation);
            this[Fingerpart.ringCenter].CalcRelativeTo(this[Fingerpart.ringBase]);

            this[Fingerpart.pinkyBase].CalcRelativeTo(wristRotation);
            this[Fingerpart.pinkyCenter].CalcRelativeTo(this[Fingerpart.pinkyBase]);
            foreach (var item in rotations)
            {
                item.splitValues = SplitValue(item.relative);

            }



            
            forces = new float[data.force.Length];
            for (int i = 0; i < forces.Length; i++)
            {
                forces[i] = Mathf.Clamp01(((float)data.force[i])/1023);
            }
        }
        /// <summary>
        /// Call this to set  reset Rotations.
        /// </summary>
        public void ResetAll()
        {
            ResetWrist();
            ResetFingers();
            onReset();
        }
        /// <summary>
        /// Call this to set  reset finger Rotations.
        /// </summary>
        public void ResetFingers()
        {
            if (rotations==null)
            {
                return;
            }
            if (fingerResets == null)
            {
                fingerResets = new Quaternion[rotations.Length];
            }
            for (int i = 0; i < rotations.Length; i++)
            {
                try
                {
                    fingerResets[i] = Quaternion.Inverse(rotations[i].absolute);

                }
                catch (Exception e)
                {

                    Debug.Log(e);
                }
            }
        }

        public float GetSplayAngleSum()
        {
            return this[Fingerpart.thumbCenter].splitValues.yAngle - this[Fingerpart.pinkyCenter].splitValues.yAngle;
        }



        /// <summary>
        /// Call this to set the wrist reset Rotation.
        /// Relative value of <seealso cref="wristRotation"/> is the rotation, reset by this.
        /// </summary>
        public void ResetWrist()
        {
            wristReset = Quaternion.Inverse(wristRotation.absolute);
        }
        /// <summary>
        /// Splits Rotation into <see cref="SplitValues"/>.
        /// </summary>
        /// <param name="relativeFingerQuaternion">Rotation to be split</param>
        /// <returns></returns>
        public static SplitValues SplitValue(Quaternion relativeFingerQuaternion)
        {
            var split = new SplitValues();
            Quaternion rotation = relativeFingerQuaternion;

            RotationFunctions.SwingTwist(rotation, Vector3.up, out split.yRot, out Quaternion swing);

            split.xAngle = -RotationFunctions.ProjectToPlane(rotation, Vector3.forward, Vector3.up);
            split.xRot = Quaternion.AngleAxis(split.xAngle, Vector3.right);

            split.yAngle = -Vector3.SignedAngle(Vector3.right, Vector3.ProjectOnPlane( split.yRot * Vector3.right, Vector3.up), Vector3.up);
            split.upAngle = RotationFunctions.ProjectToPlane(rotation, Vector3.forward, Vector3.right);
            split.upRot = Quaternion.AngleAxis(split.upAngle, Vector3.up);
            //RotationFunctions.SwingTwist(rotation, Vector3.forward,out  split.zRot, out Quaternion swing2);
            split.xyRot = split.yRot * split.xRot;
            split.zRot = Quaternion.Inverse(split.yRot * split.xRot) * rotation;
            split.zAngle = Vector3.SignedAngle(Vector3.right, split.zRot * Vector3.right, Vector3.forward);



            //float t =Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * split.xAngle));
            //split.supination = Quaternion.Lerp(split.zRot, Quaternion.AngleAxis(split.yAngle, Vector3.forward), t);
            //  split.zAngle = Vector3.SignedAngle(Vector3.up, split.zRot * Vector3.forward, Vector3.forward);
            return split;
        }

        /// <summary>
        /// Accessor for Glove Rotation for a specific Finger Part. Use this to get a specific Rotation.
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public RotationSet this[Fingerpart part]
        {
            get
            {
                if (glove.information.RecievedInformation)
                {

                    int index = glove.information.GetIMUIndex(part);
                    if (index!=-1)
                    {
                        return rotations[index];
                    }
                    else
                    {
                        return new RotationSet();
                    }
                }
                else
                {
                    return new RotationSet();
                }
            }
            set
            {
                if (glove.information.RecievedInformation)
                {

                    int index = glove.information.GetIMUIndex(part);
                    if (index != -1)
                    {
                        rotations[index]=value;
                    }
                }

            }
        }
        /// <summary>
        /// Returns force at fingerpart, -1 if there is no corresponding data.
        /// </summary>
        /// <returns></returns>
        public float GetForce(Fingerpart fingerpart)
        {
            try
            {
                int index = glove.information.GetForceIndex(fingerpart);
                if (index != -1)
                {
                    return forces[index];
                }
                else
                {
                    return -1;
                }
            }
            catch 
            {

                return -1;

            }

        }
        /// <summary>
        /// Turns a Glove Rotation to a Quaternion in Unity Coordinatates.
        /// </summary>
        /// <returns></returns>
        private Quaternion ToUnityCoordinates(Quaternion raw)
        {
            return new Quaternion(raw.y, -raw.z, -raw.x, raw.w);

            //return new Quaternion(raw.y, -raw.x, raw.z, -raw.w);
        }
        /// <summary>
        /// Returns the Y angle of the Wrist. This is the Movement Parellel to the ground.
        /// </summary>
        /// <returns></returns>
        public float GetWristYAngle()
        {
            return wristRotation.splitValues.yAngle;
        }
        /// <summary>
        /// Returns the X angle of the Wrist. This is the Movement Up and dowm.
        /// </summary>
        /// <returns></returns>
        public float GetWristXAngle()
        {
            return wristRotation.splitValues.xAngle;
        }
        /// <summary>
        /// Returns the Z angle of the Wrist. This is the Movement arround the axis of the lower arm.
        /// </summary>
        /// <returns></returns>
        public float GetWristZAngle()
        {
            return wristRotation.splitValues.zAngle;
        }
        /// <summary>
        /// Returns the sum of bend angles of the index finger.
        /// </summary>
        /// <returns></returns>
        public float GetThumbAngle()
        {
            return this[Fingerpart.thumbBase].splitValues.xAngle + this[Fingerpart.thumbCenter].splitValues.xAngle;
        }
        /// <summary>
        /// Returns the sum of bend angles of the index finger.
        /// </summary>
        /// <returns></returns>
        public float GetIndexAngle()
        {
            return this[Fingerpart.indexBase].splitValues.xAngle + this[Fingerpart.indexCenter].splitValues.xAngle;
        }
        /// <summary>
        /// Returns the sum of bend angles of the middle finger.
        /// </summary>
        /// <returns></returns>
        public float GetMiddleAngle()
        {
            return this[Fingerpart.middleBase].splitValues.xAngle + this[Fingerpart.middleCenter].splitValues.xAngle;
        }
        /// <summary>
        /// Returns the sum of bend angles of the ring finger.
        /// </summary>

        /// <returns></returns>
        public float GetRingAngle()
        {
            return this[Fingerpart.ringBase].splitValues.xAngle + this[Fingerpart.ringCenter].splitValues.xAngle;
        }
        /// <summary>
        /// Returns the sum of bend angles of the pinky.
        /// </summary>
        /// <returns></returns>
        public float GetPinkyAngle()
        {
            return this[Fingerpart.pinkyBase].splitValues.xAngle + this[Fingerpart.pinkyCenter].splitValues.xAngle;

        }
        /// <summary>
        /// Returns the bend angle of the top part of the index finger.
        /// </summary>

        /// <returns></returns>
        public float GetIndexTopAngle()
        {
            return this[Fingerpart.indexCenter].splitValues.xAngle;
        }
        /// <summary>
        /// Returns the bend angle of the top part of the middle finger.
        /// </summary>
        /// <returns></returns>
        public float GetMiddleTopAngle()
        {
            return  this[Fingerpart.middleCenter].splitValues.xAngle;
        }
        /// <summary>
        /// Returns the bend angle of the top part of the ring finger.
        /// </summary>

        /// <returns></returns>
        public float GetRingTopAngle()
        {
            return this[Fingerpart.ringCenter].splitValues.xAngle;
        }
        /// <summary>
        /// Returns the bend angle of the top part of the pinky finger.
        /// </summary>

        /// <returns></returns>
        public float GetPinkyTopAngle()
        {
            return this[Fingerpart.pinkyCenter].splitValues.xAngle;

        }
        public float GetBendAngle(Fingerpart fingerpart)
        {
            return this[fingerpart].splitValues.xAngle;
        }
        /// <summary>
        /// Returns the sum of bend angles
        /// </summary>
        /// <returns></returns>
        public float AngleSum()
        {
            if (glove.information.RecievedInformation)
            {
                float sum = 0;
                if (glove.information.GetUseFinger(Finger.Thumb))
                {
                    sum+=GetThumbAngle();
                }
                if (glove.information.GetUseFinger(Finger.Index))
                {
                    sum += GetIndexAngle();
                }
                if (glove.information.GetUseFinger(Finger.Middle))
                {
                    sum += GetMiddleAngle();
                }
                if (glove.information.GetUseFinger(Finger.Ring))
                {
                    sum += GetRingAngle();
                }
                if (glove.information.GetUseFinger(Finger.Pinky))
                {
                    sum += GetPinkyAngle();
                }
                return sum;
            }
            else
            {
                return 0;
            }
        }
        public float GetForceSum()
        {
            float sum = 0;
            if (glove.information.GetUseFinger(Finger.Thumb))
            {
                sum += GetForce(Fingerpart.thumbTop);
            }
            if (glove.information.GetUseFinger(Finger.Index))
            {
                sum += GetForce(Fingerpart.indexTop);
            }
            if (glove.information.GetUseFinger(Finger.Middle))
            {
                sum += GetForce(Fingerpart.middleTop);
            }
            if (glove.information.GetUseFinger(Finger.Ring))
            {
                sum += GetForce(Fingerpart.ringTop);
            }
            if (glove.information.GetUseFinger(Finger.Pinky))
            {
                sum += GetForce(Fingerpart.pinkyTop);
            }
            return sum;
        }
    }
}