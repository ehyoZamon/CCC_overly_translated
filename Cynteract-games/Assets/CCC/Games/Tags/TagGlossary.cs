using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract {
    [CreateAssetMenu(fileName = "Tag Glossary")]

    public class TagGlossary : SerializedScriptableObject
    {
        public Dictionary<GameDifficulty, GameTag> tags;
    }

}