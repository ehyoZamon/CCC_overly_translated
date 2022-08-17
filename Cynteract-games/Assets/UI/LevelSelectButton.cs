using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI text;
    [ReadOnly]
    public int index;
    [ReadOnly]
    public LevelSelector owner;

    public void Select()
    {
        owner.SelectLevel(index);
    }
   
}
