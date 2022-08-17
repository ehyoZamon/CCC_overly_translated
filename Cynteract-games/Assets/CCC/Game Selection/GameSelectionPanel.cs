using Cynteract;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Lean.Localization;
using Cynteract.CynteractInput;
using Cynteract.Database;

namespace Cynteract.CCC
{
    public class GameSelectionPanel : MonoBehaviour
    {
        public Image sprite;
        public TextMeshProUGUI title;
        public Button button;
        int index;
        InfoBox[] instantiatedInfoBoxes;
        GameTag[] instantiatedGameTags;
        public Transform infoBoxParent, tagsParent;
        public InfoBox infoBoxPrefab;
        private const int GloveIndex = 0;
        public TextMeshProUGUI highscoreTextMesh, highestScoreTextMesh, rankTextMesh;
        public TagGlossary tagGlossary;
        private CGame cGame;
        
        private bool gettingHighScore=false;
        public void Init(CGame cGame, int _index)
        {
            this.cGame = cGame;
            index = _index;
            sprite.sprite = cGame.sprite;
            title.text = LeanLocalization.GetTranslationText( cGame.name);
            button.onClick.AddListener(() => StartGame(index));

            if (instantiatedInfoBoxes != null)
            {
                foreach (var item in instantiatedInfoBoxes)
                {
                    Destroy(item.gameObject);
                }
            }
            if (instantiatedGameTags != null)
            {
                foreach (var item in instantiatedGameTags)
                {
                    Destroy(item.gameObject);
                }
            }
            string desc = LeanLocalization.GetTranslationText("Movements") + ":";
            var trainingGame = new TrainingGame();
            trainingGame.cGame = cGame;
            trainingGame.inputSet = cGame.inputSets[0];

            instantiatedInfoBoxes = new InfoBox[trainingGame.inputSet.axes.Length + trainingGame.inputSet.actions.Length];
            for (int i = 0; i < trainingGame.inputSet.axes.Length; i++)
            {
                instantiatedInfoBoxes[i] = Instantiate(infoBoxPrefab, infoBoxParent);
                instantiatedInfoBoxes[i].textMesh.text = LeanLocalization.GetTranslationText(GloveInput.GetInputAxis(GloveIndex, trainingGame.inputSet.axes[i].axis).name);
            }
            for (int i = 0; i < trainingGame.inputSet.actions.Length; i++)
            {
                int index = trainingGame.inputSet.axes.Length + i;
                instantiatedInfoBoxes[index] = Instantiate(infoBoxPrefab, infoBoxParent);
                instantiatedInfoBoxes[index].textMesh.text = LeanLocalization.GetTranslationText(GloveInput.GetInputAction(GloveIndex, trainingGame.inputSet.actions[i].action).name);
            }
            highscoreTextMesh.text = $"{LeanLocalization.GetTranslationText("Your Highscore")}: {TrainingsManager.instance.GetGamesHighestScore(cGame.name)}";

            instantiatedGameTags = new GameTag[cGame.difficulties.Count];
            for (int i = 0; i < instantiatedGameTags.Length; i++)
            {
                instantiatedGameTags[i] = Instantiate(tagGlossary.tags[cGame.difficulties[i]], tagsParent);
            }
        }
        private async void Update()
        {
            if (!cGame) return;
            if (gettingHighScore) return;
            gettingHighScore = true;
            var result = await DatabaseManager.instance.GetMaxHighscore(cGame.name);
            switch (result)
            {
                case NotConnectedResponse _:
                    return;
                case GenericResponse<int> response:
                    highestScoreTextMesh.text = $"{LeanLocalization.GetTranslationText("Global Highscore")}: {response.content}";
                    break;
            }
            result = await DatabaseManager.instance.GetRank(cGame.name);
            switch (result)
            {
                case NotConnectedResponse _:
                    return;
                case GenericResponse<int> response:
                    if (response.content == -1)
                    {
                        rankTextMesh.text = $"{LeanLocalization.GetTranslationText("Your Rank")}: -";

                    }
                    else
                    {
                        rankTextMesh.text = $"{LeanLocalization.GetTranslationText("Your Rank")}: <b>#{response.content+1}</b>";
                    }
                    break;

            }
        }
        private void StartGame(int i)
        {
            StartGame(CynteractControlCenter.instance.cGames[i]);
        }

        private async void StartGame(CGame cGame)
        {
            var trainingTime =TrainingsManager.instance.GetRemainingTrainingTime();
            if (trainingTime==TimeSpan.Zero)
            {
                bool startGame = await CynteractControlCenter.PlaytimeWarningPopup();
                if (!startGame)
                {
                    return;
                }
            }

            var trainingGame = new TrainingGame();
            trainingGame.cGame = cGame;
            trainingGame.inputSet = cGame.inputSets[0];
            GameTrainingController.instance.StartGame(trainingGame);
        }
    }
}