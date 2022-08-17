using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cynteract.CynteractInput
{
    public class CInput : MonoBehaviour
    {

        public abstract class AA
        {
            public string name;
            public int index = 0;
            public bool inverted;
            protected AA(string name, int index, bool inverted)
            {
                this.name = name;
                this.index = index;
                this.inverted = inverted;
            }
            public void Invert(bool invert)
            {
                inverted = invert;
            }
            public abstract void Evaluate(CGlove.GloveData gloveData);

        }
        [System.Serializable]
        public class Axis : AA
        {
            public GloveInput.AxesEnum axis;

            public Axis(string name, int index, GloveInput.AxesEnum axis, bool inverted) : base(name, index, inverted)
            {
                this.axis = axis;
            }
            public override void Evaluate(CGlove.GloveData gloveData)
            {
                GloveInput.inputs[index].availableAxes[(int)axis].Evaluate(gloveData);
            }
        }

        [System.Serializable]
        public class Action : AA
        {
            public GloveInput.ActionsEnum action;


            public Action(string name, int index, GloveInput.ActionsEnum action, bool inverted) : base(name, index, inverted)
            {
                this.action = action;
            }
            public override void Evaluate(CGlove.GloveData gloveData)
            {
                GloveInput.inputs[index].availableActions[(int)action].Evaluate(gloveData);
            }
        }
        [System.Serializable]
        public class SerializedData
        {
            public Axis[] axes;
            public Action[] actions;
            public SerializedData()
            {
                axes = new Axis[instance.axes.Length];
                actions = new Action[instance.actions.Length];
                for (int i = 0; i < instance.axes.Length; i++)
                {
                    axes[i] = instance.axes[i];
                }
                for (int i = 0; i < instance.actions.Length; i++)
                {
                    actions[i] = instance.actions[i];
                }
            }
        }
        [HideInInspector]
        public  Axis[] axes;
        [HideInInspector]

        public Action[] actions;
        public static CInput instance;
        
        public virtual void Init()
        {
            //Load();
            GloveInput.Load();
            if (CGlove.Glove.Any!=null)
            {
            CGlove.Glove.Any.Subscribe(Callback);

            }
        }

        private void Callback(CGlove.Glove data)
        {
            EvaluateAll(data.data);
        }

        public CInput()
        {

            instance = this;
        }
        public static void Save()
        {

            var data = new SerializedData();
            JsonFileManager.Save(data,instance.name+ "Input");
        }

        public void EvaluateAll(CGlove.GloveData gloveData)
        {
            foreach (var item in axes)
            {
                item.Evaluate(gloveData);
            }
            foreach (var item in actions)
            {
                item.Evaluate(gloveData);
            }
        }

        public static void Load()
        {
            try
            {
                var data = JsonFileManager.Load<SerializedData>(instance.name + "Input");
                instance.SetAxes(data.axes);
                instance.SetActions(data.actions);
            }
            catch (Exception)
            {

                Debug.LogWarning("No CInput file");

            }

        }
        public void SetAxes(params Axis[] axes)
        {
            instance.axes = axes;
        }
        public void SetActions(params Action[] actions)
        {
            instance.actions = actions;
        }
        public static Axis GetAxisWithName(string axis)
        {
            return Array.Find(instance.axes, x => x.name == axis);
        }
        public static GloveInput.InputAxis GetInputAxis(string axis)
        {
            Axis axis1 = Array.Find(instance.axes, x => x.name == axis);
            return GloveInput.GetInputAxis(axis1.index,axis1.axis);
        }
        public static GloveInput.InputAction GetInputAction(string action)
        {
            Action action1 = Array.Find(instance.actions, x => x.name == action);
            return GloveInput.GetInputAction(action1.index, action1.action);
        }
        public static float GetAxis(string axis)
        {

            return GetAxis(Array.Find(instance.axes, x => x.name == axis));
        }
        public static bool GetAction(string action)
        {
            return GetAction(Array.Find(instance.actions, x => x.name == action));
        }
        public static float GetActionValue(string action)
        {
            return GetActionValue(Array.Find(instance.actions, x => x.name == action));

        }
        public static float GetAxis(Axis axis)
        {
            if (axis.inverted)
            {
                return 1-GloveInput.GetAxis(axis.index, axis.axis);
            }
            else
            {
                return GloveInput.GetAxis(axis.index, axis.axis);
            }
        }
        public static float GetActionValue(Action action)
        {
            if (action.inverted)
            {
                return 1 - GloveInput.GetActionValue(action.index, action.action);
            }
            else
            {
                return GloveInput.GetActionValue(action.index, action.action);
            }
        }
        public static bool GetAction(Action action)
        {
            if (action.inverted)
            {
                return !GloveInput.GetAction(action.index, action.action);
            }
            else
            {
                return GloveInput.GetAction(action.index, action.action);
            }
        }
        

        public static bool GetActionDown(Action action)
        {
            if (action.inverted)
            {
                return GloveInput.GetActionUp(action.index, action.action);
            }
            else
            {
                return GloveInput.GetActionDown(action.index, action.action);
            }
        }
        public static bool GetActionUp(Action action)
        {
            if (action.inverted)
            {
                return GloveInput.GetActionDown(action.index, action.action);
            }
            else
            {
                return GloveInput.GetActionUp(action.index, action.action);
            }
        }
    }


}