using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cynteract.Tunnelrunner
{
    [CreateAssetMenu(menuName ="Tunnelrunner/Settings/LevelSet")]
    public class LevelSet : ScriptableObject
    {

        public Lvl[] lvls;
        [System.Serializable]
        public struct Lvl
        {
            public ObjectToSpawn[] items;
            public int scoreNeededForNext;
        }
        [System.Serializable]
        public struct ObjectToSpawn
        {
            public Transform objectsToSpawn;

            public int probability;
        }

        internal int CurrentThreashold()
        {
            return lvls[Tunnelrunner.instance.levelStats.currentLevel].scoreNeededForNext;
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(LevelSet))]
    public class LevelEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            LevelSet level = (LevelSet)target;

            base.OnInspectorGUI();
            if (level.lvls!=null&&level.lvls.Length>0)
            {
                level.lvls[level.lvls.Length - 1].scoreNeededForNext = -1;

            }

        }
    }
#endif

}