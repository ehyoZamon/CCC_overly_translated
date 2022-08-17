using Cynteract.CynteractInput;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract
{
    public enum GameDifficulty
    {
        Beginner,
        Intermediate,
        Expert
    }
    [CreateAssetMenu(fileName ="Cynteract Game")]
    public class CGame : SerializedScriptableObject
    {
        public new string name;
        public Sprite sprite;
        public string sceneToLoad;
        public InputSet[] inputSets;
        public List<GameDifficulty> difficulties;
    }
    [Serializable]
    public class InputSet
    {
        public CGameAxisInfo[] axes;
        public CGameActionInfo[] actions;
    }
    [Serializable]
    public class CGameAxisInfo
    {
        public GloveInput.AxesEnum axis;
        public bool inverted;
    }
    [Serializable]
    public class CGameActionInfo
    {
        public GloveInput.ActionsEnum action;
        public bool inverted;
    }
}