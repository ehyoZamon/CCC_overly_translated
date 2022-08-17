using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextField : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public Image backGroundImage;
    public RectTransform contentRect;
    public void SetText(string text)
    {
        textMesh.text = text;
    }
}
