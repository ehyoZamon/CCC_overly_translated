using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cynteract.UI {
    public class MenuButton : MonoBehaviour { 
        public Button button;
        public RectTransform rectTransform;
        public InstantiatedMenu menuToOpen;

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(MenuButton))]
    public class MenuButtonEditor : Editor
    {
        MenuButton menuButton;
        private void OnEnable()
        {
            menuButton = target as MenuButton;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            menuButton.button = menuButton.GetComponent<Button>();
            menuButton.rectTransform = menuButton.GetComponent<RectTransform>();

        }
    }
#endif
}

