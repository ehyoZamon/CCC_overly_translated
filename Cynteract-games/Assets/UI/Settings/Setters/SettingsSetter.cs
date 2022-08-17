using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public abstract class SettingsSetter : MonoBehaviour
{
    public string key;
    public TextMeshProUGUI text;
    public virtual void Init(string key)
    {
        this.key = key;
        text.text = key;
    }
}
