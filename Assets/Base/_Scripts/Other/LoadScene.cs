using UnityEngine;
using SupersonicWisdomSDK;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI progressText;
    [SerializeField] private UnityEngine.UI.Slider progressSlider;
    void Awake()
    {
        // Oyun İndirilip İlk Kez Açıldığında Tüm Verileri Siler(Oyun İlk Yüklendiğinde Bazı Verilerin Kayıtlı Kalmasına Karşı Önlem) -
        // The Game Deletes All Data When Downloaded and Opened for the First Time(Precaution Against Some Data Remaining When The Game Is First Loaded)
        Application.targetFrameRate = 30;
        SupersonicWisdom.Api.AddOnReadyListener(OnSupersonicWisdomReady);
        SupersonicWisdom.Api.Initialize();
        if (PlayerPrefs.GetString("FirstLoad", "True") == "False") return;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("FirstLoad", "False");
    }
    void OnSupersonicWisdomReady()
    {
        StartCoroutine(LoadingScene());
    }
    System.Collections.IEnumerator LoadingScene()
    {
        AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            progressText.text = "Loading % " + (progress * 100).ToString("F");
            progressSlider.value = progress;
            yield return null;
        }
    }
}