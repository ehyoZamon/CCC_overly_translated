using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;
    public string settingsname = "DefaultSettings";





    [Serializable]
    public class Value
    {
        public static implicit operator Value(bool b)
        {
            return new BoolValue(b);
        }
        public static implicit operator Value(int b)
        {
            return new IntValue(b);
        }
        public static implicit operator Value(float b)
        {
            return new FloatValue(b);
        }
        public static implicit operator Value((float v, float l, float h) value)
        {
            return new FloatValueLimited(value.v, value.l, value.h);
        }
        public static implicit operator Value(double b)
        {
            return new DoubleValue(b);
        }
        public static implicit operator Value(long b)
        {
            return new LongValue(b);
        }
    }


    [Serializable]
    public class BoolValue : Value
    {
        public bool value;
        public static implicit operator bool(BoolValue b)
        {
            return b.value;
        }
        public BoolValue(bool value)
        {
            this.value = value;
        }
    }
    [Serializable]
    public class IntValue : Value
    {
        public int value;
        public IntValue(int value)
        {
            this.value = value;
        }
    }
    [Serializable]
    public class FloatValue : Value
    {
        public float value;
        public FloatValue(float value)
        {
            this.value = value;
        }
        public static implicit operator float(FloatValue b)
        {
            return b.value;
        }
    }
    [Serializable]
    public class FloatValueLimited : FloatValue
    {
        public float lowerLimit, upperLimit;

        public FloatValueLimited(float value, float lowerLimit, float upperLimit) : base(value)
        {
            this.lowerLimit = lowerLimit;
            this.upperLimit = upperLimit;
        }
    }
    [Serializable]
    public class DoubleValue : Value
    {
        public double value;
        public DoubleValue(double value)
        {
            this.value = value;
        }
    }
    [Serializable]
    public class LongValue : Value
    {
        public long value;
        public LongValue(long value)
        {
            this.value = value;
        }
    }
    [Serializable]
    public class NamedValue<T> where T : Value
    {
        public string name;
        public T value;
        public NamedValue(string name, Value value)
        {
            this.name = name;
            this.value = value as T;
        }
    }

    [Serializable]
    public class NamedFloat : NamedValue<FloatValue>
    {
        public NamedFloat(string name, Value value) : base(name, value)
        {
        }
    }
    [Serializable]
    public class NamedFloatLimited : NamedValue<FloatValueLimited>
    {
        public NamedFloatLimited(string name, Value value) : base(name, value)
        {
        }
    }
    [Serializable]
    public class NamedBool : NamedValue<BoolValue>
    {
        public NamedBool(string name, Value value) : base(name, value)
        {
        }
    }

    [Serializable]
    public class SerializedData
    {
        public NamedBool[] boolValues;
        public NamedFloat[] floatValues;
        public NamedFloatLimited[] floatValuesLimited;


        public void SetValues(GameSettings settings)
        {
            foreach (var item in boolValues)
            {
                TrySetKey(settings, item);
            }
            foreach (var item in floatValues)
            {
                TrySetKey(settings, item);
            }
            foreach (var item in floatValuesLimited)
            {
                TrySetKey(settings, item);
            }
        }

        private static void TrySetKey<T>(GameSettings settings, NamedValue<T> item) where T :Value
        {
            if (settings.namedValues.ContainsKey(item.name))
            {
                settings.namedValues[item.name] = item.value;
            }
        }

        public SerializedData(GameSettings settings)
        {
            List<NamedBool> namedBoolValues = new List<NamedBool>();
            List<NamedFloat> namedFloatValues = new List<NamedFloat>();
            List<NamedFloatLimited> namedFloatValuesLimited = new List<NamedFloatLimited>();
            foreach (var item in settings.namedValues)
            {
                switch (item.Value)
                {
                    case BoolValue b:
                        namedBoolValues.Add(new NamedBool(item.Key, b));
                        break;
                    case FloatValueLimited b:
                        namedFloatValuesLimited.Add(new NamedFloatLimited(item.Key, b));
                        break;
                    case FloatValue b:
                        namedFloatValues.Add(new NamedFloat(item.Key, b));
                        break;
                    default:
                        break;
                }
            }
            boolValues = namedBoolValues.ToArray();
            floatValues = namedFloatValues.ToArray();
            floatValuesLimited = namedFloatValuesLimited.ToArray();
        }
    }
    public Dictionary<string, Value> namedValues = new Dictionary<string, Value> {
            {CSettings.cameraShake, false },
            {CSettings.gameSpeed,(1,0.1f, 1) },
            {CSettings.simpleGraphics, false},
            {CSettings.vibrate, false}
        };

    public bool set = false;

    public static bool SimpleGraphics {
        get {
            return GetBool(CSettings.simpleGraphics);
        }
        set
        {
            instance.namedValues[CSettings.simpleGraphics] = value;
        }
    }

    public GameSettings()
    {
        instance = this;
    }
    private void Awake()
    {
        set = false;
        Load();
        set = true;
    }
    #region Getters
    public static bool GetBool(string key)
    {
        return (instance.namedValues[key] as BoolValue);
    }
    public static (string key, Value value) []ValuesToArray()
    {
        List<(string key, Value value)> list = new List<(string key, Value value)>();
        foreach (var item in instance.namedValues)
        {
            list.Add((item.Key, item.Value));
        }
        return list.ToArray();
    }
    public static float GetFloat(string key)
    {
        return instance.namedValues[key] as FloatValue;
    }
    public static (float value, float lowerLimit, float upperLimit) GetFloatLimited(string key)
    {
        var fvl= instance.namedValues[key] as FloatValueLimited;
        return (fvl.value, fvl.lowerLimit, fvl.upperLimit);
    }
    #endregion
    #region Setters

    public static void SetBool(string key, bool value)
    {
        Set(key, value);
    }
    public static void SetFloat(string key, float value)
    {
        (instance.namedValues[key] as FloatValue).value = value;
    }
    private static void Set(string key, Value value)
    {
        instance.namedValues[key] = value;
    }
    #endregion

    public static void Save()
    {
        var data =new  SerializedData(instance);
        JsonFileManager.Save(data, instance.settingsname);
        print("Saved");
    }
    public static void Load()
    {
        try
        {
            var data = JsonFileManager.Load<SerializedData>(instance.settingsname);
            data.SetValues(instance);
        }
        catch 
        {

        }

    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(GameSettings))]
public class SettingsEditor : Editor
{
    GameSettings settings;
    private void Awake()
    {
        settings = target as GameSettings;
    }
    /*
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        Dictionary<string, GameSettings.Value> values = new Dictionary<string, GameSettings.Value>();
        foreach (var item in GameSettings.instance.namedValues)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.Key);
            GameSettings.Value value = new GameSettings.Value();
            if (item.Value is GameSettings.BoolValue)
            {
                value = EditorGUILayout.Toggle(item.Value as GameSettings.BoolValue);
            }
            if (item.Value is GameSettings.FloatValueLimited)
            {
                GameSettings.FloatValueLimited namedFloatLimited = item.Value as GameSettings.FloatValueLimited;
                value =
                    (EditorGUILayout.Slider(namedFloatLimited.value, namedFloatLimited.lowerLimit, namedFloatLimited.upperLimit),
                    ((GameSettings.FloatValueLimited)item.Value).lowerLimit,
                    ((GameSettings.FloatValueLimited)item.Value).upperLimit);
            }
            values.Add(item.Key, value);
            GUILayout.EndHorizontal();

        }
        GameSettings.instance.namedValues = values;
        if (GUILayout.Button("Save"))
        {
            GameSettings.Save();
        }
    }*/
}
#endif