using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class SaveDictionaryTest : SerializedMonoBehaviour

{
    [Button]
    public void Save()
    {
        JsonFileManager.Save(data, "dict",JsonFileManager.Type.Newtonsoft);
    }
    [Serializable]
    public class Data
    {

        public enum TestEnum{
            Fist, other, yetAnother
        }
        [ShowInInspector]
        public DateTime dateTime;
        [ShowInInspector]
        public Dictionary<string, int> stringDict;
        [ShowInInspector]
        public Dictionary<TestEnum, int> keyValuePairs;
    }
    public Data data;
}
