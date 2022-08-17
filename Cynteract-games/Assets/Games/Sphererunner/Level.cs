using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cynteract.Sphererunner
{
    [CreateAssetMenu(menuName = "SphereRunner/Level")]
    public class Level : SerializedScriptableObject
    {
        public Transform endpart;
        public Transform startPart;
        public Transform flatPart;

        public bool loadFromFolder;
        public Lvl[] lvls;
        [System.Serializable]
        public struct Lvl
        {
            public int numberOfParts;
            public Transform[] additionalParts;

        }

        public Transform[] currentParts(int currentLevel, out Transform[] newParts)
        {
            List<Transform> parts = new List<Transform>();
            for (int i = 0; i <= currentLevel; i++)
            {
                for (int j = 0; j < lvls[i].additionalParts.Length; j++)
                {
                    parts.Add(lvls[i].additionalParts[j]);
                }
            }
            newParts = lvls[currentLevel].additionalParts;
            return parts.ToArray();

        }

        internal void AutoAssingnLevels()
        {
            int[] currentPartCount = new int[lvls.Length];
            for (int i = 0; i < currentPartCount.Length; i++)
            {
                currentPartCount[i] = lvls[i].numberOfParts;
            }

            var parts = Resources.LoadAll("Levels");
            GameObject[] sortedPaths = new GameObject[parts.Length + 1];
            sortedPaths[0] = (GameObject)Resources.Load("Levels/FlatPart");
            for (int i = 1; i < sortedPaths.Length - 1; i++)
            {
                string path = "Levels/Prefab" + (i + 1);
                sortedPaths[i] = (GameObject)Resources.Load(path);
            }
            lvls = new Lvl[sortedPaths.Length];
            for (int i = 0; i < lvls.Length - 1; i++)
            {
                lvls[i].additionalParts = new Transform[1];
                lvls[i].additionalParts[0] = (sortedPaths[i]).transform;
                if (i < currentPartCount.Length)
                {
                    lvls[i].numberOfParts = currentPartCount[i];
                }
                else if (currentPartCount.Length > 0)
                {
                    lvls[i].numberOfParts = currentPartCount[currentPartCount.Length - 1];
                }
                else
                {
                    lvls[i].numberOfParts = 4;
                }
            }
            lvls[lvls.Length - 1].numberOfParts = 4;
        }
    }
}