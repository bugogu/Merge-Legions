using UnityEngine;

[System.Serializable]
public class My
{
    /// <summary>
    /// Titerşim Ayarı Eğer Açıksa Tiretşim Uygular Değilse Hiçbirşey Yapmaz
    /// </summary>
    public static void DoVibrate()
    {
        if (Setting.Settings.vibrateActive == 0) return;
        Handheld.Vibrate();
    }
    /// <summary>
    /// Parametre Olarak Gönderilen Clip Dosyasını Oynatır ve Ses Dosyasının Uzunluğu Kadar Bir Süre Sonra Yok Eder - Plays Clip File Sent As Parameter And Destroys It After A Time As Long As Audio File
    /// </summary>
    /// <param name="clip"> Oynatılacak Ses Dosyası - Audio File to Play </param>
    public static void PlaySound(AudioClip clip)
    {
        if (Setting.Settings.soundActive == 0) return;
        SoundSystem.PlayOneShot(clip, clip.name, 1, clip.length);
    }
    public static void PlaySound(string clipName = "")
    {
        if (Setting.Settings.soundActive == 0) return;
        if (!(clipName.Length > 0)) return;
        foreach (AudioClip audioClip in GameManager.Instance.sfx)
        {
            if (audioClip.name == clipName)
                SoundSystem.PlayOneShot(audioClip, clipName, 1, audioClip.length);
            break;
        }
    }
}