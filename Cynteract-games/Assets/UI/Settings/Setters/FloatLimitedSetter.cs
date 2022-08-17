using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FloatLimitedSetter : SettingsSetter
{
    public Slider slider;
    public TextMeshProUGUI floatText;
    public override void Init(string key)
    {
        base.Init(key);
        var val = GameSettings.GetFloatLimited(key);
        slider.minValue = val.lowerLimit;
        slider.maxValue = val.upperLimit;
        slider.value = val.value;
    }
    public void SetValue(float value)
    {
        floatText.text = string.Format("{0:0.0}", value);
        GameSettings.SetFloat(key, value);
    }

}
