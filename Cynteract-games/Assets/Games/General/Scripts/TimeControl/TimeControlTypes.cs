using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseTime{
    float GetBaseTime();
}
public class SettingsBaseTime : IBaseTime
{
    public float GetBaseTime()
    {
        return GameSettings.GetFloat(CSettings.gameSpeed);
    }
}
public class ConstantBaseTime : IBaseTime
{
    public float baseTime=1;
    public float GetBaseTime()
    {
        return baseTime;
    }
}
