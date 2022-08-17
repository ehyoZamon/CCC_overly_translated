
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Cynteract.CCC
{
    public class GameSelectionWindow : CCCWindow
    {
        public Transform panelParent;
        public GameSelectionPanel selectionPanelPrefab;
        [ReadOnly]
        public GameSelectionPanel[] gameSelectionPanels;
        public Button backButton;
        protected override void OnInit()
        {
            backButton.onClick.AddListener(() => CynteractControlCenter.Home());
        }

        protected override void OnOpen()
        {
            var games = CynteractControlCenter.instance.cGames;
            if (gameSelectionPanels!=null)
            {
                for (int i = 0; i < gameSelectionPanels.Length; i++)
                {
                    if (gameSelectionPanels[i] != null)
                    { 
                        Destroy(gameSelectionPanels[i].gameObject);
                    }
                }
                gameSelectionPanels = null;
            }
            gameSelectionPanels = new GameSelectionPanel[games.Length];
            for (int i = 0; i < games.Length; i++)
            {
                gameSelectionPanels[i] = Instantiate(selectionPanelPrefab, panelParent);
                gameSelectionPanels[i].Init(games[i],i);
            }
        }



        protected override void OnClose(bool wasDestroyed)
        {

        }
    }
}