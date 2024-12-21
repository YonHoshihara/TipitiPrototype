using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Soundtrack", menuName = "AudioSystem/Soundtrack List")]
public class SoundtrackSO : ScriptableObject
{
    public List<SoundEntry> sounds;

    public AudioClip GetClipByName(string name)
    {
        foreach (var sound in sounds)
        {
            if (sound.name == name)
            {
                return sound.clip;
            }
        }
        return null;
    }
}

[System.Serializable]
public class SoundEntry
{
    public string name;
    public AudioClip clip;
}