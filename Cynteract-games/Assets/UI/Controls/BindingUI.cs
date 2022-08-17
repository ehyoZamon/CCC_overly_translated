using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynteract.CGlove;
using System;
using TMPro;
namespace Cynteract.CynteractInput
{
    public class BindingUI : MonoBehaviour 
    {
        public CalibrationWindow calibrationWindow;
        const int calibrationTime = 10;
        public RectTransform rectTransform;
        public TextMeshProUGUI text;
        public Type type;
        public int index;
        public TextMeshProUGUI selectedPartText;
        public HandPartSelectionMaster handPartSelectionMasterPrefab;
        IEnumerator calibration;




        public void StartCalibration()
        {
            GloveCalibration.instance.StartCalibration(index, type);
        }
        public void Invert(bool invert)
        {
            switch (type)
            {
                case Type.Axis:
                    CInput.instance.axes[index].Invert(invert);
                    break;
                case Type.Action:
                    CInput.instance.actions[index].Invert(invert);
                    break;
                default:
                    break;
            }
        }
        public void SelectHandPart()
        {
            var master=Instantiate(handPartSelectionMasterPrefab, MainCanvas.instance.transform);
            master.Init(this);
        }
        public void Select(int selection)
        {
            switch (type)
            {
                case Type.Axis:
                    CInput.instance.axes[index].axis = (GloveInput.AxesEnum)selection;
                    break;
                case Type.Action:
                    CInput.instance.actions[index].action = (GloveInput.ActionsEnum)selection;
                    break;
                default:
                    break;
            }
        }
    }
}