using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using Cynteract.CTime;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cynteract.UI
{


    public class MainMenu : MonoBehaviour
    {
        public MenuButton[] buttons;
        [ReadOnly]
        public MenuButton[] instantiatedButtons;
        RectTransform rectTransform;
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

        }
        // Use this for initialization
        void Start()
        {
            instantiatedButtons = new MenuButton[buttons.Length];
            for (int i = 0; i < buttons.Length; i++)
            {
                instantiatedButtons[i] = Instantiate(buttons[i], transform);
            }
            foreach (var item in instantiatedButtons)
            {
                if (item.menuToOpen)
                {
                    var menu = Instantiate(item.menuToOpen, MainCanvas.instance.transform);
                    menu.gameObject.SetActive(false);
                    menu.backButton.onClick.AddListener(() => menu.gameObject.SetActive(false));
                    menu.backButton.onClick.AddListener(() => gameObject.SetActive(true));
                    item.button.onClick.AddListener(() => menu.gameObject.SetActive(true));
                    item.button.onClick.AddListener(() => gameObject.SetActive(false));
                }

            }

        }



    }

}
