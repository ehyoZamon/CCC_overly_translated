using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogFileCreator 
{
    private const string path = @"C:\Users\manue\AppData\LocalLow\DefaultCompany\Cynteract Games\";

    public static void Log(string message)
    {
        try
        {
            var textFile = File.ReadAllText(path + "log.txt");
            File.WriteAllText(path + "log.txt", textFile + message);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            throw;
        }


    }
}
