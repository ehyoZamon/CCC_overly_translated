using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using System;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManger : MonoBehaviour
{
    public SoundCollection sounds;
    public static SoundManger instance;
    [ReadOnly]
    public Transform virtualSoundsParent;
    private void Awake()
    {
       instance = this;
    }
    private void Start()
    {
        var go = new GameObject();
        go.transform.parent = transform;
        go.name = "Virtual Sources";
        foreach (var item in sounds.soundList)
        {
            var virtualSoundParent = new GameObject();
            virtualSoundParent.transform.parent = go.transform;
            virtualSoundParent.name = item.key;
            item.sound.virtualSoundParent = virtualSoundParent.transform;
        }
        virtualSoundsParent = go.transform;
    }
    private void Update()
    {

        foreach (var sound in sounds.soundList)
        {
            for (int i = 0; i < sound.sound.virtualSources.Count; i++)
            {
                if (!sound.sound.virtualSources[i].isPlaying)
                {
                    AudioSource audioSource = sound.sound.virtualSources[i];
                    sound.sound.virtualSources.Remove(audioSource);
                    Destroy(audioSource.gameObject);
                }
            }
        }
    }
    public void PlayRandomClip(string key)
    {
        var sound=sounds.GetSound(key);
        var clip = sound.clips[UnityEngine.Random.Range(0, sound.clips.Count)];
        sound.source.clip = clip;
        sound.source.Play();
    }
    public void PlayOneShotRandomClip(string key)
    {
        var sound = sounds.GetSound(key);
        var clip = sound.clips[UnityEngine.Random.Range(0, sound.clips.Count)];
        sound.source.PlayOneShot(clip);
    }

    public void PlayRandomClipWithVirtualSource(string key,float minDelay)
    {
        var sound = sounds.GetSound(key);
        if (Time.unscaledTime<=sound.lastVirtualSoundStartTime+ minDelay)
        {
            return;
        }
        PlaySound(sound);
    }
    private static void PlaySound(Sound sound)
    {
        var clip = sound.clips[UnityEngine.Random.Range(0, sound.clips.Count)];

        var source = Instantiate(sound.source, sound.virtualSoundParent);
        sound.lastVirtualSoundStartTime = Time.unscaledTime;
        sound.virtualSources.Add(source);
        source.clip = clip;
        source.Play();
    }

}
[System.Serializable]

public class KeySoundPair
{
    public string key;
    public Sound sound;

    public KeySoundPair(string key, Sound sound)
    {
        this.key = key;
        this.sound = sound;
    }

    public override bool Equals(object obj)
    {
        return obj is KeySoundPair other &&
               key == other.key &&
               EqualityComparer<Sound>.Default.Equals(sound, other.sound);
    }

    public override int GetHashCode()
    {
        int hashCode = 485657312;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(key);
        hashCode = hashCode * -1521134295 + EqualityComparer<Sound>.Default.GetHashCode(sound);
        return hashCode;
    }

    public void Deconstruct(out string key, out Sound sound)
    {
        key = this.key;
        sound = this.sound;
    }

    public static implicit operator (string key, Sound sound)(KeySoundPair value)
    {
        return (value.key, value.sound);
    }

    public static implicit operator KeySoundPair((string key, Sound sound) value)
    {
        return new KeySoundPair(value.key, value.sound);
    }
}

[System.Serializable]
public class Sound
{
    public AudioSource source;
    public List<AudioClip> clips;
    public Transform virtualSoundParent;
    public List<AudioSource> virtualSources;
    public float lastVirtualSoundStartTime;
    public Sound(AudioSource source)
    {
        this.source = source;
        this.source.playOnAwake = false;
        clips = new List<AudioClip>();
        virtualSources = new List<AudioSource>();
    }
}

[System.Serializable]
public class SoundCollection
{
    public SoundManger owner;
    public List<KeySoundPair > soundList;
    public void AddSound(string key)
    {
        var gameObject = new GameObject();
        gameObject.name = key;
        gameObject.transform.parent = owner.transform;
        var source=gameObject.AddComponent<AudioSource>();
        var sound = new Sound(source);
        soundList.Add((key, sound));
    }
    public Sound GetSound(string key)
    {
        return soundList.Find(x => x.key == key).sound;
    }
    public bool Remove(string key)
    {
        KeySoundPair pair = soundList.Find(x => x.key == key);
        if (pair!=null)
        {
            if (GetSound(key).source)
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(GetSound(key).source.gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(GetSound(key).source.gameObject);
                }
            }

            soundList.Remove(pair);
            return true;
        }
        return false;
    }

    public  bool ContainsKey(string key)
    {
        return soundList.Find(x => x.key == key) != null;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof (SoundManger))]
public class SoundManagerEditor : OdinEditor
{
    public SoundManger soundManger;
    protected override void OnEnable()
    {
        base.OnEnable();
        soundManger = target as SoundManger;
        //soundManger.sounds.owner = soundManger;
    }
}
public class SoundCollectionValueDrawer : OdinValueDrawer<SoundCollection>
{
    bool adding;
    string newKey;
    protected override void DrawPropertyLayout(GUIContent label)
    {
        SoundCollection value = this.ValueEntry.SmartValue;
        EditorGUI.BeginChangeCheck();
        value.owner=EditorGUILayout.ObjectField(value.owner, typeof(SoundManger), true) as SoundManger;

        if (value.soundList==null)
        {
            if (GUILayout.Button("CreateNew"))
            {
                value.soundList = new List<KeySoundPair>();
            }
            
        }
        else
        {
            EditorGUILayout.BeginVertical();
            List<string> keysToRemove = new List<string>();
            List<(string oldKey, string newKey)> keysToRename = new List<(string oldKey, string newKey)>();
            List<KeySoundPair> soundList = value.soundList;
            for (int i = 0; i < soundList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var key = EditorGUILayout.TextField(soundList[i].key);
                if (key!= soundList[i].key)
                {
                    if (soundList[i].sound.source)
                    {
                        soundList[i].sound.source.gameObject.name = key;
                    }
                }
                soundList[i].key = key;
                var sound = soundList[i].sound;
                EditorGUILayout.BeginVertical();
                for (int j = 0; j < sound.clips.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    sound.clips[i] = EditorGUILayout.ObjectField(sound.clips[i], typeof(AudioClip), true) as AudioClip;
                    if (GUILayout.Button("Remove Clip"))
                    {
                        sound.clips.Remove(sound.clips[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add Clip"))
                {
                    sound.clips.Add(null);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add New Key"))
            {
                adding = true;
            }
            if (adding)
            {
                newKey = EditorGUILayout.TextField(newKey);
                if (newKey==null)
                {
                    newKey = "";
                }
                if (value.ContainsKey(newKey))
                {
                    EditorGUILayout.LabelField("Collection already Contains Key");
                }
                else
                {
                    if (GUILayout.Button("Add"))
                    {
                        value.AddSound(newKey);
                        adding = false;
                    }
                }

            }
            EditorGUILayout.EndVertical();

        }
        if (EditorGUI.EndChangeCheck())
        {
        ValueEntry.SmartValue = value;

        }
    }
}

#endif