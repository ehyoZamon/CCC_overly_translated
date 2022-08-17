using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class JsonFileManager 
{
    public enum EncryptionType
    {
        Aes
    }
    private const string jsonFilePath = "/Resources/";
    private const string jsonEnding = ".json";
    public enum Type
    {
        Unity,
        Newtonsoft
    }
    public static void Save<T>(T data,string name, Type type=Type.Unity , bool refreshDatabase=true)
    {
        #if UNITY_EDITOR
        string filePathJ = Application.dataPath + jsonFilePath + name + jsonEnding;
#else
        string filePathJ = GetPersistantPath(name);
#endif
        string dataAsJson = "";
        switch (type)
        {
            case Type.Unity:
                 dataAsJson = JsonUtility.ToJson(data);
                
                break;
            case Type.Newtonsoft:
                 dataAsJson = JObject.FromObject(data).ToString();
                break;
            default:
                break;
        }
        
        File.WriteAllText(filePathJ, dataAsJson);
#if UNITY_EDITOR
        if (refreshDatabase)
        {
            AssetDatabase.Refresh();
            
        }
#endif
    }
    public static T Load<T>(string name, Type type = Type.Unity) where T : class
    {
#if UNITY_EDITOR
        TextAsset file = Resources.Load<TextAsset>(name);
        string dataAsJson = file.text;
#else
        string dataAsJson=File.ReadAllText( GetPersistantPath(name) );
#endif
        switch (type)
        {
            case Type.Unity:

                T data = JsonUtility.FromJson<T>(dataAsJson);
                return data;
                
            case Type.Newtonsoft:
                JObject newtonData = JObject.Parse(dataAsJson);
                return newtonData.ToObject<T>();
                
            default:
                throw new Exception("Type " + type + " does not exist");
        }
    }
    public static void SaveEncrypted<T>( T data, string name, EncryptionType encryptionType=EncryptionType.Aes)
    {
        string filePath = GetPersistantPath(name);
        string dataAsJson = JObject.FromObject(data).ToString();
        byte[] bytes;
        switch (encryptionType)
        {
            case EncryptionType.Aes:
                bytes = AesEncryption.Encrypt(dataAsJson);
                break;
            default:
                throw new Exception("Type " + encryptionType + " does not exist");
        }
        File.WriteAllBytes(filePath, bytes);

    }

    private static string GetPersistantPath(string name)
    {
        return Application.persistentDataPath + "/ " + name + jsonEnding;
    }

    public static T LoadEncrypted<T>(string name, EncryptionType encryptionType = EncryptionType.Aes)
    {
        string filePath = GetPersistantPath(name);
        byte[] bytes;
        bytes=File.ReadAllBytes(filePath);
        string dataAsJson;
        switch (encryptionType)
        {
            case EncryptionType.Aes:
                dataAsJson = AesEncryption.DecryptToString(bytes);
                break;
            default:
                throw new Exception("Type " + encryptionType + " does not exist");
        }
        JObject newtonData = JObject.Parse(dataAsJson);
        return newtonData.ToObject<T>();
    }
    public static byte[] ToJsonBytes(JObject jObject)
    {
        string jsonString = jObject.ToString(Formatting.None);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
        return jsonBytes;
    }
    public static JObject FromJsonBytes(byte[] bytes)
    {
        return JObject.Parse( Encoding.UTF8.GetString(bytes));
    }
}
