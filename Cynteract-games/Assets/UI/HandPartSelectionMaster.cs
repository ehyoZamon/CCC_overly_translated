using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

namespace Cynteract.CynteractInput {
    public class HandPartSelectionMaster : MonoBehaviour
    {
        public HandPartSelection[] handPartSelections;
        Dictionary<GloveInput.AxesEnum, HandPartSelection> axes;
        Dictionary<GloveInput.ActionsEnum, HandPartSelection> actions;
        public CInput.AA aa;
        private BindingUI binding;
        public Type type;
        public CalibrationWindow calibrationWindow;
        const int calibrationTime = 10;
        public Toggle inversionToggle;

        public void Init(BindingUI bindingUI)
        {
            this.binding = bindingUI;

            type = bindingUI.type;
            switch (type)
            {
                case Type.Axis:
                    aa = CInput.instance.axes[bindingUI.index];
                    break;
                case Type.Action:
                    aa = CInput.instance.actions[bindingUI.index];
                    break;
                default:
                    break;
            }
            
            axes = new Dictionary<GloveInput.AxesEnum, HandPartSelection>();
            actions = new Dictionary<GloveInput.ActionsEnum, HandPartSelection>();
            foreach (var item in handPartSelections)
            {
                item.Init(this);
                axes.Add(item.axis, item);
                actions.Add(item.action, item);
            }
            switch (type)
            {
                case Type.Axis:
                    axes[CInput.instance.axes[bindingUI.index].axis].Select();
                    inversionToggle.isOn = CInput.instance.axes[bindingUI.index].inverted;

                    break;
                case Type.Action:
                    actions[CInput.instance.actions[bindingUI.index].action].Select();
                    inversionToggle.isOn = CInput.instance.actions[bindingUI.index].inverted;

                    break;
                default:
                    break;
            }
        }
        public void Close()
        {
            Destroy(gameObject);
            switch (type)
            {
                case Type.Axis:
                    binding.selectedPartText.text = GloveInput.availableAxesStatic[(int)CInput.instance.axes[binding.index].axis].name;

                    break;
                case Type.Action:
                    binding.selectedPartText.text = GloveInput.availableActionsStatic[(int)CInput.instance.actions[binding.index].action].name;
                    break;
                default:
                    break;
            }
        }
        public void Select(HandPartSelection selection,GloveInput.AxesEnum axis)
        {
            GetAxis().axis = axis;
            Select(selection);
        }
        public void Select(HandPartSelection selection, GloveInput.ActionsEnum action)
        {
            GetAction().action = action;
            Select(selection);
        }

        private void Select(HandPartSelection selection)
        {
            for (int i = 0; i < handPartSelections.Length; i++)
            {
                //if (selection!=handPartSelections[i])
                //{
                handPartSelections[i].Deselect();
                //}
            }
        }

        public void StartCalibration()
        {
            GloveCalibration.instance.StartCalibration(binding.index, type);
        }
        private CInput.Action GetAction()
        {
            return ((CInput.Action)aa);
        }


        public void Invert(bool invert)
        {
            switch (type)
            {
                case Type.Axis:
                    GetAxis().Invert(invert);
                    break;
                case Type.Action:
                    GetAction().Invert(invert);
                    break;
                default:
                    break;
            }
        }

        private CInput.Axis GetAxis()
        {
            return ((CInput.Axis)aa);
        }

    }
}