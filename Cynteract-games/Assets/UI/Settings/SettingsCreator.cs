using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsCreator : MonoBehaviour
{
    public BoolSetter boolSetter;
    public FloatLimitedSetter floatLimitedSetter;
    // Start is called before the first frame update
    void Start()

    {
        var values = GameSettings.ValuesToArray();
        foreach (var item in values)
        {
            switch (item.value)
            {
                case GameSettings.BoolValue b:  
                    var boolSet=Instantiate(boolSetter, transform);
                    boolSet.Init(item.key);
                    break;
                case GameSettings.FloatValueLimited b:
                    var floatLimitedSet = Instantiate(floatLimitedSetter, transform);
                    floatLimitedSet.Init(item.key);
                    break;
                default:
                    break;
            }
        }
    }
    public void Save()
    {
        GameSettings.Save();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
