using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class LevelSwitcher :  ILevelLoader{
    public GameObject currentLevel;
    public GameObject[] levels;
    string path;
    int currentIndex;
    public LevelSwitcher(string path)
    {
        this.path = path;
        currentIndex = 0;
        LevelLoading.loader = this;
    }
    public void InitLevels()
    {

        var resourceLvls = Resources.LoadAll(path);
        Array.Sort(resourceLvls, (x, y) =>Mathf.Clamp( int.Parse(x.name) - int.Parse(y.name),-1, 1));
        levels = new GameObject[resourceLvls.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] =(GameObject) resourceLvls[i];
        }
    }
    public void NextLevel()
    {
        if (currentIndex+1>=levels.Length)
        {
            LoadLevel(0);
        }
        else
	    {
            LoadLevel(currentIndex + 1);
        }
    }
    public void LoadLevel(int level)
    {
        currentIndex = level;
        if (currentLevel)
        {
            UnityEngine.Object.Destroy(currentLevel);
        }

        currentLevel = UnityEngine.Object.Instantiate(levels[level]);
    }

    public int GetNumberOfLevels()
    {
        return levels.Length;
    }
}
