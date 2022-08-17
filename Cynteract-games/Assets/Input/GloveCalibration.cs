using AForge.Math;
using Cynteract.CGlove;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Cynteract.CynteractInput
{
    public class GloveCalibration : MonoBehaviour, ICalibration
    {
        public static GloveCalibration instance;
        const int calibrationTime = 10;
        public CalibrationWindow calibrationWindowPrefab;
        public ResetWindow resetWindowPrefab;
        public AnimationCurve animationCurve;
        public AnimationCurve fourierCurve;
        public AnimationCurve backwardsCurve;

        IEnumerator calibration;
        public bool drawCurve;
        private int index;
        private GloveInput.Input input;
        private Action onCalibrationFinished;
        public float min, max;
        private void Awake()
        {
            instance = this;
        }
        private void Update()
        {
            if (CalibrationWindow.instance&& calibration!=null)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                {
                    StopCalibration();
                }
            }

        }
        IEnumerator Calibrate(int index, Type type)
        {
            var calibWindow = Instantiate(calibrationWindowPrefab, MainCanvas.instance.transform);
            calibWindow.bindingUI = this;

            switch (type)
            {
                case Type.Axis:
                    var axis = CInput.instance.axes[index].axis;
                    GloveInput.inputs[0].Reset(axis);
                    input = GloveInput.inputs[0].availableAxes[(int)(axis)];
                    calibWindow.SetDescription(input.description);
                    calibWindow.SetAnimation(input.animation);
                    if (drawCurve)
                    {
                        GloveInput.inputs[0].StartCalibrating(axis, this);
                    }
                    else
                    {
                        GloveInput.inputs[0].StartCalibrating(axis);
                    }
                    
                    break;
                case Type.Action:
                    var action = CInput.instance.actions[index].action;
                    input = GloveInput.inputs[0].availableActions[(int)(action)];
                    calibWindow.SetDescription(input.description);

                    calibWindow.SetAnimation(input.animation);
                    GloveInput.inputs[0].Reset(action);
                    if (drawCurve)
                    {
                        GloveInput.inputs[0].StartCalibrating(action, this);
                    }
                    else
                    {
                        GloveInput.inputs[0].StartCalibrating(action);
                    }
                    break;
                default:
                    break;
            }
            for (int i = 0; i < calibrationTime; i++)
            {
                CalibrationWindow.instance.SetCountdown(calibrationTime - i);
                yield return new WaitForSecondsRealtime(1);
            }
            StopCalibration();
        }
        public void StopCalibration()
        {
            Destroy(CalibrationWindow.instance.gameObject);
            GloveInput.inputs[0].StopCalibratingAll();
            StopCoroutine(calibration);
            calibration = null;
            if (drawCurve)
            {
                EvaluateData();
            }
            if (onCalibrationFinished!=null)
            {
                onCalibrationFinished();
            }
        }
        public void EvaluateData()
        {
            float[] values = new float[animationCurve.keys.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = animationCurve.keys[i].value;
            }
            int newLength = (int)(values.Length * 0.9f);
            float[] newValues = new float[newLength];
            Array.Copy(values, (int)(values.Length * 0.05f), newValues, 0, newLength);
            Array.Sort(newValues);
            int lowerIndex = (int)((double)newValues.Length / 10);
            int upperIndex = (int)(9 * (double)newValues.Length / 10);
            float lowerValue = newValues[lowerIndex];
            float upperValue = newValues[upperIndex];
            float midValue = (upperValue + lowerValue) / 2;
            max = upperValue;
            min = lowerValue;
            input.SetCalibration(lowerValue, upperValue, midValue);
        }
        public void StartCalibration(int index, Type type)
        {
            this.onCalibrationFinished = null;
            StartCalibrationPrivate(index, type);
        }
        private void StartCalibrationPrivate(int index, Type type)
        {
            animationCurve = new AnimationCurve();
            calibration = Calibrate(index, type);
            StartCoroutine(calibration);
        }
        public void StartCalibration(int index, Type type, Action onCalibrationFinished)
        {
            this.onCalibrationFinished = onCalibrationFinished;
            StartCalibrationPrivate(index, type);

        }
        void FourierTransformData()
        {
            List<Complex> complexValues = new List<Complex>();
            foreach (var item in animationCurve.keys)
            {
                complexValues.Add(new Complex(item.value, 0));
            }
            Complex[] data = complexValues.ToArray();
            FourierTransform.DFT(data, FourierTransform.Direction.Forward);
            fourierCurve = new AnimationCurve();
            for (int i = 0; i < data.Length; i++)
            {
                fourierCurve.AddKey(i, (float)data[i].Re);

            }
            FourierTransform.DFT(data, FourierTransform.Direction.Backward);

            backwardsCurve = new AnimationCurve();
            for (int i = 0; i < data.Length; i++)
            {
                backwardsCurve.AddKey(i, (float)data[i].Re);
            }
        }
        Complex[] BandPass(Complex[] data, double lowCutOff, double highCutOff)
        {

            return LowPass(HighPass(data, lowCutOff),1-((1-highCutOff)/(1-lowCutOff)) );
        }
        Complex[] LowPass(Complex[] data, double cuttOff)
        {
            Complex[] filteredData = new Complex[(int)(cuttOff * data.Length)];
            for (int i = 0; i < filteredData.Length; i++)
            {
                filteredData[i] = data[i];
            }
            return filteredData;
        }
        Complex[] HighPass(Complex[] data, double cuttOff)
        {
            Complex[] filteredData = new Complex[(int)((1-cuttOff) * data.Length)];
            int startIndex = (int)(cuttOff) * (data.Length);
            for (int i = 0; i < filteredData.Length; i++)
            {
                filteredData[i] = data[i+startIndex];
            }
            return filteredData;
        }
        public void Add(float current)
        {
            animationCurve.AddKey(index, current);
            index++;
        }
        [Button]
        public void ResetAll()
        {
            GloveInput.ResetRotation();
        }
        [Button]
        public void StartResetting(bool collectData, params Action[] onFinish)
        {
            StartCoroutine(ResetRoutine(collectData,onFinish));
        }
        public Task<bool> StartResettingAsync()
        {
            var task = new TaskCompletionSource<bool>();
            StartCoroutine(ResetRoutine(true,()=>task.SetResult(true)));
            return task.Task;
        }
        IEnumerator ResetRoutine(bool collectData, params Action [] onFinish)
        {

            var resetWindow = Instantiate(resetWindowPrefab, GetTransform());
            resetWindow.progressSlider.gameObject.SetActive(false);

            int queueSize = 100;
            int maxAngle = 10;
            float value = 0;
            float bufferValue = 0;
            float speed = .4f;
            float bufferSpeed = 4f;
            Queue<Quaternion> lastRotations = new Queue<Quaternion>();
            for (int i = 0; i < 3; i++)
            {
                resetWindow.countdown.text = (3 - i).ToString();
                yield return new WaitForSecondsRealtime(1);
            }
            resetWindow.countdown.gameObject.SetActive(false);
            resetWindow.progressSlider.gameObject.SetActive(true);
            while (value < 1)
            {
                var current = Glove.AnyData.wristRotation.absolute;
                Queue<Quaternion> nextLastRotations = new Queue<Quaternion>();
                while (lastRotations.Count > queueSize)
                {
                    lastRotations.Dequeue();
                }
                bool movedToMuch = false;
                while (lastRotations.Count > 0)
                {

                    var rot = lastRotations.Dequeue();
                    movedToMuch |= Quaternion.Angle(rot, current) > maxAngle;
                    nextLastRotations.Enqueue(rot);
                }
                if (movedToMuch)
                {
                    value = 0;
                    bufferValue = 0;
                    nextLastRotations = new Queue<Quaternion>();
                }
                else
                {
                    if (bufferValue > 1)
                    {

                        value += Time.unscaledDeltaTime * speed;
                    }
                    else
                    {
                        bufferValue += Time.unscaledDeltaTime * bufferSpeed;
                    }
                }
                if (value > .5f)
                {
                    ResetAll();
                }
                resetWindow.progressSlider.value = value;
                nextLastRotations.Enqueue(current);
                lastRotations = nextLastRotations;
                yield return null;
            }
            Destroy(resetWindow.gameObject);

            foreach (var item in onFinish)
            {
                try
                {
                    item();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        private static Transform GetTransform()
        {
            if (MainCanvas.instance&&MainCanvas.instance.gameObject.scene.isLoaded)
            {
            return MainCanvas.instance.transform;

            }
            else
            {
                return CCC.CynteractControlCenter.instance.popupWindowCanvas;
            }
        }
    }
}