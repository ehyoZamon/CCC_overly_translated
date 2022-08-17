using UnityEngine;
using System;
namespace Cynteract.CynteractInput
{
    [Serializable]
    public class GloveInput
    {
        public enum CallMode
        {
            GloveCallback, CustomUpdate
        }
        [Serializable]
        public class InputAxis : Input
        {

            public override void Evaluate(CGlove.GloveData glove)
            {
                base.Evaluate(glove);
            }

            public InputAxis(string name, Func<CGlove.GloveData, float> func, CallMode mode = CallMode.GloveCallback) : this(name, "", func, mode)
            {
            }
            public InputAxis(string name, string description, Func<CGlove.GloveData, float> func, CallMode mode = CallMode.GloveCallback) : base(name, description, func, mode)
            {

            }
            public InputAxis(string name, string description, Func<CGlove.GloveData, float> func, string animation, CallMode mode = CallMode.GloveCallback) : base(name, description, func, animation, mode)
            {

            }
        }
        [Serializable]
        public class InputAction : Input
        {
            public Func<InputAction, bool> boolFunc;
            private bool actionValue;
            private bool lastTrueGloveThread = false;
            public InputAction(string name, Func<CGlove.GloveData, float> func, Func<InputAction, bool> boolFunc, CallMode mode = CallMode.GloveCallback) : this(name, "", func, boolFunc, mode)
            {
                this.boolFunc = boolFunc;
            }
            public InputAction(string name, string description, Func<CGlove.GloveData, float> func, Func<InputAction, bool> boolFunc, CallMode mode = CallMode.GloveCallback) : base(name, description, func, mode)
            {
                this.boolFunc = boolFunc;
            }
            public InputAction(string name, string description, Func<CGlove.GloveData, float> func, Func<InputAction, bool> boolFunc, string animation, CallMode mode = CallMode.GloveCallback) : base(name, description, func, animation, mode)
            {
                this.boolFunc = boolFunc;
            }
            public override void Evaluate(CGlove.GloveData glove)
            {
                lastTrueGloveThread = actionValue;
                base.Evaluate(glove);
                actionValue = boolFunc(this);
            }

            public bool GetAction()
            {
                return actionValue;
            }
            public bool GetValueMid()
            {
                return current > midValue;
            }
            public bool GetValueTopBottom()
            {
                if (lastTrueGloveThread)
                {
                    if (value < .3f)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (value > .7f)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            public bool GetValueDownGloveThread()
            {
                return actionValue && !lastTrueGloveThread;
            }
            public bool GetValueUpGloveThread()
            {
                return !actionValue && lastTrueGloveThread;
            }
        }

        internal static object GetInputAxis(object gloveIndex, AxesEnum axis)
        {
            throw new NotImplementedException();
        }

        [Serializable]
        public class Input
        {
            public string name, description;
            public Func<CGlove.GloveData, float> func;
            public CallMode callmode;
            public float current, upperValue, midValue, lowerValue;
            public bool calibrate = false;
            public GloveCalibration calibration;
            protected float value;
            public string animation;
            public Input(string name, string description, Func<CGlove.GloveData, float> func, CallMode mode = CallMode.GloveCallback) : this(name, description, func, "", mode)
            {
            }
            public Input(string name, string description, Func<CGlove.GloveData, float> func, string animation, CallMode mode = CallMode.GloveCallback)
            {
                this.name = name;
                this.func = func;
                this.callmode = mode;
                this.description = description;
                this.animation = animation;
            }
            public float GetValue()
            {
                return value;
            }
            public void Calibrate()
            {
                if (calibration)
                {
                    calibration.Add(current);
                }
                else
                {
                    upperValue = Mathf.Max(current, upperValue);
                    lowerValue = Mathf.Min(current, lowerValue);
                    midValue = (upperValue + lowerValue) / 2;
                }
            }
            public void SetCalibration(float lower, float upper, float mid)
            {
                lowerValue = lower;
                upperValue = upper;
                midValue = mid;
            }
            public virtual void Evaluate(CGlove.GloveData glove)
            {
                current = func(glove);
                value = Mathf.InverseLerp(lowerValue, upperValue, current);
                if (calibrate)
                {
                    Calibrate();
                }
            }
            public void Reset()
            {
                upperValue = float.MinValue;
                lowerValue = float.MaxValue;
                midValue = 0;
            }
        }



        public static void ResetRotation()
        {
            CGlove.Glove.AnyData.ResetAll();
        }

        [Serializable]
        public class SerializedData
        {
            public GloveInput[] inputs;
            public SerializedData()
            {
                inputs = GloveInput.inputs;
            }
        }

        private const string path = "GloveInputs";
        private int index;
        public enum AxesEnum
        {
            AngleSum,
            MouseX,
            MouseY,
            ArmX,
            ArmY,
            ArmZ,
            Index,
            Middle,
            Ring,
            Pinky,

            IndexTop,
            MiddleTop,
            RingTop,
            PinkyTop,

            Force,

             Splay
        }
        public enum ActionsEnum
        {
            AngleSum,
            MouseClick,
            ArmX,
            ArmY,
            ArmZ,
            Index,
            Middle,
            Ring,
            Pinky,

            IndexTop,
            MiddleTop,
            RingTop,
            PinkyTop,

            Force,
            
            Splay
        }
        public static readonly InputAxis[] availableAxesStatic =
        {
            new InputAxis("Fist Grip", "Please open and close your hand as wide as possible",x=>x.AngleSum(),"fistGrip" ),
            new InputAxis("Maus X", x=>UnityEngine.Input.mousePosition.x, CallMode.CustomUpdate),
            new InputAxis("Maus Y", x=>UnityEngine.Input.mousePosition.y, CallMode.CustomUpdate),
            new InputAxis("Arm bending","Please raise and lower your arm as far as possible", x=>x.GetWristXAngle(),"wristX"),
            new InputAxis("Arm rotation", "Please move your arm as far as possible in a horizontal direction.", x=>x.GetWristYAngle(), "wristY"),
            new InputAxis("Arm twist","Please roll your arm as far as you can.", x=>x.GetWristZAngle()),


            new InputAxis("Index finger",       "Please bend your index finger as far as possible",      x=>x.GetIndexAngle()),
            new InputAxis("Middle finger",      "Please bend your middle finger as far as possible",     x=>x.GetMiddleAngle()),
            new InputAxis("Ring finger",        "Please bend your ring finger as far as possible",       x=>x.GetRingAngle()),
            new InputAxis("Pinky",              "Please bend your little finger as far as possible",     x=>x.GetPinkyAngle()),

            new InputAxis("Index finger top",   "Please bend your index finger as far as possible",     x=>x.GetIndexTopAngle()),
            new InputAxis("Middle finger top",  "Please bend your middle finger as far as possible",    x=>x.GetMiddleTopAngle()),
            new InputAxis("Ring finger top",    "Please bend your ring finger as far as possible",      x=>x.GetRingTopAngle()),
            new InputAxis("Pinky top",          "Please bend your little finger as far as possible",    x=>x.GetPinkyTopAngle()),

            new InputAxis("Force", "Please press your fingers together as firmly as possible", x=>x.GetForceSum(),"force"),

            new InputAxis("Finger Splay","Please spread your fingers as wide as possible", x=>x.GetSplayAngleSum())

        };
        public static readonly InputAction[] availableActionsStatic =
        {
            new InputAction("Fist Grip","Please open and close your hand as wide as possible", x=>x.AngleSum(), a=>a.GetValueTopBottom(),"fistGrip"),
            new InputAction("Mouse click", x=>0,x=>UnityEngine.Input.GetKey(KeyCode.Mouse0), CallMode.CustomUpdate),
            new InputAction("Arm bending","Please raise and lower your arm as far as possible", x=>x.GetWristXAngle(),a=>a.GetValueTopBottom(),"wristX"),

            new InputAction("Arm rotation","Please move your arm as far as possible in a horizontal direction.", x=>x.GetWristYAngle(), a=>a.GetValueTopBottom(),"wristY"),
            new InputAction("Arm twist","Please roll your arm as far as you can.", x=>x.GetWristZAngle(), a=>a.GetValueTopBottom()),

            new InputAction("Index finger",     "Please bend your index finger as far as possible",     x=>x.GetIndexAngle(), a=>a.GetValueTopBottom()),
            new InputAction("Middle finger",    "Please bend your middle finger as far as possible",    x=>x.GetMiddleAngle(), a=>a.GetValueTopBottom()),
            new InputAction("Ring finger",      "Please bend your ring finger as far as possible",      x=>x.GetRingAngle(), a=>a.GetValueTopBottom()),
            new InputAction("Pinky",            "Please bend your little finger as far as possible",    x=>x.GetPinkyAngle(), a=>a.GetValueTopBottom()),

            new InputAction("Index finger top",     "Please bend your index finger as far as possible",    x=>x.GetIndexTopAngle(), a=>a.GetValueTopBottom()),
            new InputAction("Middle finger top",    "Please bend your middle finger as far as possible",   x=>x.GetMiddleTopAngle(), a=>a.GetValueTopBottom()),
            new InputAction("Ring finger top",      "Please bend your ring finger as far as possible",     x=>x.GetRingTopAngle(), a=>a.GetValueTopBottom()),
            new InputAction("Pinky top",            "Please bend your little finger as far as possible",   x=>x.GetPinkyTopAngle(), a=>a.GetValueTopBottom()),

            new InputAction("Force", "Please press your fingers together as firmly as possible", x=>x.GetForceSum(), a=>a.GetValueTopBottom(),"force"),


            new InputAction("Finger Splay","Please spread your fingers as wide as possible", x=>x.GetSplayAngleSum(), a=>a.GetValueTopBottom())

        };
        public InputAxis[] availableAxes;
        public InputAction[] availableActions;
        public static GloveInput[] inputs ={
            new GloveInput(0)
        };
        bool[] calibrateAxis;
        bool[] calibrateActions;
        public GloveInput(int index)
        {
            this.index = index;
            availableAxes = new InputAxis[availableAxesStatic.Length];
            availableActions = new InputAction[availableActionsStatic.Length];
            for (int i = 0; i < availableAxes.Length; i++)
            {
                availableAxes[i] = availableAxesStatic[i];
            }
            for (int i = 0; i < availableActions.Length; i++)
            {
                availableActions[i] = availableActionsStatic[i];
            }
            calibrateAxis = new bool[availableAxes.Length];
            calibrateActions = new bool[availableActions.Length];
        }
        public static void Load()
        {
            try
            {
                var data = JsonFileManager.Load<SerializedData>(path);

                inputs = data.inputs;
            }
            catch
            {
                Debug.LogWarning("No Glove Input file");
                inputs = new GloveInput[] { new GloveInput(0) };
                Save();
            }
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputs[i].availableActions.Length != availableActionsStatic.Length || inputs[i].availableAxes.Length != availableAxesStatic.Length)
                {
                    inputs[i] = new GloveInput(inputs[i].index);
                }
            }
        }
        public static void Save()
        {
            var data = new SerializedData();
            JsonFileManager.Save(data, path);
        }

        public void StartCalibrating(AxesEnum axis)
        {
            availableAxes[(int)axis].calibrate = true;
        }
        public void StartCalibrating(AxesEnum axis, GloveCalibration gloveCalibration)
        {
            availableAxes[(int)axis].calibrate = true;
            availableAxes[(int)axis].calibration = gloveCalibration;

        }
        public void StopCalibrating(AxesEnum axis)
        {
            availableAxes[(int)axis].calibrate = false;
            availableAxes[(int)axis].calibration = null;

        }
        public void StartCalibrating(ActionsEnum action)
        {
            availableActions[(int)action].calibrate = true;
            availableActions[(int)action].calibration = null;

        }
        public void StartCalibrating(ActionsEnum action, GloveCalibration gloveCalibration)
        {
            availableActions[(int)action].calibrate = true;
            availableActions[(int)action].calibration = gloveCalibration;
        }
        public void StopCalibrating(ActionsEnum action)
        {
            availableActions[(int)action].calibrate = false;
            availableActions[(int)action].calibration = null;

        }
        public void Reset(AxesEnum action)
        {
            availableAxes[(int)action].Reset();
        }
        public void Reset(ActionsEnum action)
        {
            availableActions[(int)action].Reset();
        }



        public static string[] GetAxisNames()
        {
            return GetNames(availableAxesStatic);
        }
        public static string[] GetActionNames()
        {
            return GetNames(availableActionsStatic);
        }

        private static string[] GetNames(Input[] inputs)
        {
            string[] names = new string[inputs.Length];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = inputs[i].name;
            }
            return names;
        }


        public void StopCalibratingAll()
        {

            foreach (var item in availableAxes)
            {
                item.calibrate = false;
            }
            foreach (var item in availableActions)
            {
                item.calibrate = false;
            }
        }
        public static float GetActionValue(int i, ActionsEnum action)
        {
            return inputs[i].availableActions[(int)action].GetValue();
        }
        public static float GetAxis(AxesEnum axes)
        {
            return GetAxis(0, axes);
        }
        public static bool GetAction(ActionsEnum action)
        {
            return GetAction(0, action);
        }
        public static InputAxis GetInputAxis(int i, AxesEnum axes)
        {
            return inputs[i].availableAxes[(int)axes];
        }
        public static InputAction GetInputAction(int i, ActionsEnum action)
        {
            return inputs[i].availableActions[(int)action];
        }
        public static float GetAxis(int i, AxesEnum axes)
        {
            return GetInputAxis(i, axes).GetValue();
        }
        public static bool GetAction(int i, ActionsEnum action)
        {
            return GetInputAction(i, action).GetAction();
        }

        public static bool GetActionDown(int i, ActionsEnum action)
        {
            return GetInputAction(i, action).GetValueDownGloveThread();
        }
        public static bool GetActionDown(ActionsEnum action)
        {
            return GetActionDown(0, action);
        }
        public static bool GetActionUp(int i, ActionsEnum action)
        {
            return GetInputAction(i, action).GetValueUpGloveThread();
        }
        public static bool GetActionUp(ActionsEnum action)
        {
            return GetActionUp(0, action);
        }
    }
}