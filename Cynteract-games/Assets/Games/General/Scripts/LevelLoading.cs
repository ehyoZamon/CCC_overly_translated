using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ILevelLoader
{
    void LoadLevel(int index);
    int GetNumberOfLevels();
}
public class LevelLoading 
{
    public static ILevelLoader loader;
    public static void LoadLevel(int index)
    {
        loader.LoadLevel(index);
    }
    public static int GetNumberOfLevels()
    {
        return loader.GetNumberOfLevels();
    }
}
