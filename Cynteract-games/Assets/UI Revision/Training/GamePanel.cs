using Cynteract.CynteractInput;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Lean.Localization;
namespace Cynteract.CCC
{
    public class GamePanel : MonoBehaviour
    {
        #warning Index
        private const int GloveIndex = 0;
        public Image image;
        public CGame cGame;
        public TextMeshProUGUI titleTextMesh, descriptionTextMesh, timeTextMesh;
        public InfoBox infoBoxPrefab;
        InfoBox[] instantiatedInfoBoxes;
        public Transform infoBoxParent;
        public void Init(TrainingGame trainingGame)
        {
            if (instantiatedInfoBoxes!=null)
            {
                foreach (var item in instantiatedInfoBoxes)
                {
                    Destroy(item.gameObject);
                }
            }
            
            var cGame = trainingGame.cGame;
            image.sprite = cGame.sprite;
            this.cGame = cGame;
            titleTextMesh.text = LeanLocalization.GetTranslationText(cGame.name);
            string desc = LeanLocalization.GetTranslationText("Movements")+":";
            descriptionTextMesh.text = desc;
            timeTextMesh.text = $"{trainingGame.time.ToString(@"mm")} {LeanLocalization.GetTranslationText("Minutes")}";


            instantiatedInfoBoxes = new InfoBox[trainingGame.inputSet.axes.Length + trainingGame.inputSet.actions.Length];
            for (int i = 0; i < trainingGame.inputSet.axes.Length; i++)
            {
                instantiatedInfoBoxes[i] = Instantiate(infoBoxPrefab, infoBoxParent);
                instantiatedInfoBoxes[i].textMesh.text = LeanLocalization.GetTranslationText( GloveInput.GetInputAxis(GloveIndex, trainingGame.inputSet.axes[i].axis).name);
            }
            for (int i = 0; i < trainingGame.inputSet.actions.Length; i++)
            {
                int index = trainingGame.inputSet.axes.Length + i;
                instantiatedInfoBoxes[index] = Instantiate(infoBoxPrefab, infoBoxParent);
                instantiatedInfoBoxes[index].textMesh.text = LeanLocalization.GetTranslationText(GloveInput.GetInputAction(GloveIndex, trainingGame.inputSet.actions[i].action).name);
            }
        }

    }
}