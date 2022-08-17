using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoolSetter : SettingsSetter
{
    public Toggle toggle;
    public override void Init(string key)
    {
        base.Init(key);
        toggle.isOn = GameSettings.GetBool(key);
    }
    public void SetBool(bool value)
    {
        GameSettings.SetBool(key, value);
    }
}
