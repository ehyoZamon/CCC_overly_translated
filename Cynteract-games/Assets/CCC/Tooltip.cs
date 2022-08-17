using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Cynteract.CCC
{
    public class Tooltip : MonoBehaviour
    {
        public TextMeshProUGUI textMesh;
        public RectTransform rectTransform;
        public void SetText(string text)
        {
            textMesh.text = text;
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
    }
}