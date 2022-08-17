using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cynteract.CGlove;
using System;
namespace Cynteract.CynteractInput
{

    public class ControlSettings : MonoBehaviour
    {
        public float Height
        {
            get { return bindingUI.rectTransform.rect.height; }
        }

        public float Width
        {
            get
            {
                return bindingUI.rectTransform.rect.width;
            }
        }

        public BindingUI bindingUI;
        public float margin;
        private RectTransform rectTransform;
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        private void Start()
        {
            var axes = CInput.instance.axes;
            var actions = CInput.instance.actions;


            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (axes.Length + actions.Length) * (Height + margin));
            for (int i = 0; i < axes.Length; i++)
            {
                DrawButton(i, i, axes[i]);
            }
            for (int i = 0; i < actions.Length; i++)
            {
                DrawButton(i + axes.Length, i, actions[i]);
            }
        }
        public void Save()
        {
            CInput.Save();
            GloveInput.Save();
        }
        public void Load()
        {
            CInput.Load();
        }
        private void DrawButton(int i, int index, CInput.Axis axis)
        {
            BindingUI binding = CreateBinding(i);
            binding.index = index;
            binding.text.text = axis.name;
            binding.selectedPartText.text = GloveInput.availableAxesStatic[(int)CInput.instance.axes[index].axis].name;
            binding.type = Type.Axis;
        }
        private void DrawButton(int i, int index, CInput.Action action)
        {
            BindingUI binding = CreateBinding(i);
            binding.index = index;
            binding.text.text = action.name;
            binding.selectedPartText.text = GloveInput.availableActionsStatic[(int)CInput.instance.actions[index].action].name;

            binding.type = Type.Action;
        }
        private BindingUI CreateBinding(int i)
        {
            var binding = Instantiate(bindingUI);
            var bindingRect = binding.rectTransform;
            bindingRect.SetParent(transform, false);
            bindingRect.localPosition = new Vector3(bindingRect.localPosition.x, -Height / 2 - i * (Height + margin), bindingRect.localPosition.z);
            bindingRect.sizeDelta = new Vector2(Width, Height);
            return binding;
        }
        public void ResetRotation()
        {
            GloveInput.ResetRotation();
        }
    }
}