using UnityEngine;

[System.Serializable]
public class SoundSystem
{
    /// <summary>
    /// Instantiate New Gameobject to Play the "clip", Rename it by "name", Optionally Override Volume and Destroy Delay.
    /// </summary>
    /// <param name="clip">Audio Clip</param>
    /// <param name="name">GameObject's new name</param>
    /// <param name="overridedVolume">Audio Source volume</param>
    /// <param name="destroyDelay">Delay of destroy gameObject</param>
    public static void PlayOneShot(AudioClip clip, string name = "", float overridedVolume = 1, float destroyDelay = 2)
    {
        if (clip == null) return;
        var parent = GameObject.Find("Sounds");
        if (parent == null)
        {
            parent = new GameObject("Sounds");
        }
        GameObject c = new GameObject($"OneShotClip ( {clip.name} )");
        c.transform.SetParent(parent.transform);
        if (name != "" || !string.IsNullOrEmpty(name) || name != string.Empty)
        {
            c.name = name;
        }
        AudioSource s = c.AddComponent<AudioSource>();
        s.playOnAwake = false;
        s.loop = false;
        s.clip = clip;
        if (overridedVolume >= 0)
        {
            if (overridedVolume > 1) overridedVolume = 1;
            s.volume = overridedVolume;
        }
        s.Play();
        Object.Destroy(c, destroyDelay);
    }
}