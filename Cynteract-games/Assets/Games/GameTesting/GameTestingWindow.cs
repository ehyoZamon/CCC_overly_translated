#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Cynteract.GameTesting
{
    public class GameTestingWindow : OdinEditorWindow
    {
        public static GameTestingWindow instance;

        public bool Active
        {
            get
            {
                return instance == this;
            }
        }
        public bool useGlove;
        [ListDrawerSettings(IsReadOnly =true)]
        public CGameAxisInfo[] axes;
        [ListDrawerSettings(IsReadOnly = true)]
        public CGameActionInfo[] actions;

        [ReadOnly]
        public GameController gameController;
        [MenuItem("Tools/Game Testing")]
        private static void OpenWindow()
        {
            var window = GetWindow<GameTestingWindow>();

            // Nifty little trick to quickly position the window in the middle of the editor.
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(300, 300);
            window.Init();
        }

        [Button]
        public void Init()
        {
            gameController = FindObjectOfType<GameController>();
            axes = new CGameAxisInfo[gameController.cInput.axes.Length];
            actions = new CGameActionInfo[gameController.cInput.actions.Length];
            
        }
        protected  override void OnEnable()
        {
            instance = this;
            base.OnEnable();
        }
    }
}
#endif