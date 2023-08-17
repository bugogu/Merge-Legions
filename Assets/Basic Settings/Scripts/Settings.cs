using UnityEngine;

namespace Setting
{
    public class Settings : MonoBehaviour
    {
        #region Variable
        [HideInInspector] public static int vibrateActive;
        [HideInInspector] public static int soundActive;
        #region Privates
        [Header("Buttons")]
        [Tooltip("Settings Panelini Açmak İçin Bir Buton Sürükle - Drag a Button to Open Settings Panel")]
        [SerializeField] private UnityEngine.UI.Button settingsButton;
        [Tooltip("Titreşim Ayarını Açmak Yada Kapamak İçin Bir Buton Sürükle - Drag a Button to Turn Vibration On or Off")]
        [SerializeField] private UnityEngine.UI.Button vibrateButton;
        [Tooltip("Ses Ayarını Açmak Yada Kapamak İçin Bir Buton Sürükle - Drag a Button to Turn Sound On or Off")]
        [SerializeField] private UnityEngine.UI.Button soundButton;
        [Header("Others")]
        [Tooltip("Titreşim Açık Yada Kapalı Olduğunda Efekt Uygulamak İçin Tireşim Butonunda Bulunan CanvasGroup Bileşenini Sürükle - Drag CanvasGroup Component On Vibration Button To Apply Effect When Vibration is On or Off")]
        [SerializeField] private CanvasGroup vibrateAlpha;
        [Tooltip("Ses Açık Yada Kapalı Olduğunda Efekt Uygulamak İçin Ses Butonunda Bulunan CanvasGroup Bileşenini Sürükle - Drag CanvasGroup Component On Sound Button To Apply Effect When Sound is On or Off")]
        [SerializeField] private CanvasGroup soundAlpha;
        [Tooltip("Titreşim ve Ses Butonunun Bulunduğu Bir Panel yada Obje Sürükle - Drag a Panel or Object with Vibration and Volume Button")]
        [SerializeField] private GameObject settingsPanel;
        #endregion
        #endregion
        void Start()
        {
            ButtonsControl();
            VibrateControl();
            SoundControl();
        }
        // Panel Aktifse Pasif Değilse Aktif Yapar - If the Panel is Active, it Will be Active if Not Passive.
        private void OpenSettings()
        {
            My.PlaySound(UIManager.Instance.clickSound);
            if (UIManager.tutorialActive == 1)
                return;
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            if (settingsPanel.activeSelf)
                UIManager.Instance.guideObject.localPosition = new Vector3(42.6f, -200, 0);
            else
                UIManager.Instance.guideObject.localPosition = new Vector3(42.6f, -26, 0);
        }
        // vibrateActive Değişkeninden Gelen Değere Göre vibrateActive Değer Ataması Yapar ve Kaydeder - 
        //Assigns and Saves a Value Based on the Value from the vibrateActive Variable
        private void VibrateButton()
        {
            My.PlaySound(UIManager.Instance.clickSound);
            if (vibrateActive == 1)
                vibrateActive = 0;
            else
                vibrateActive = 1;
            PlayerPrefs.SetInt("vibrateStatus", vibrateActive);
            vibrateAlpha.alpha = ((float)vibrateActive + .25f);
        }
        // soundActive Değişkeninden Gelen Değere Göre soundActive Değer Ataması Yapar ve Kaydeder - 
        //Assigns and Saves a Value Based on the Value from the soundActive Variable
        private void SoundButton()
        {
            My.PlaySound(UIManager.Instance.clickSound);
            if (soundActive == 1)
                soundActive = 0;
            else
                soundActive = 1;
            PlayerPrefs.SetInt("soundStatus", soundActive);
            soundAlpha.alpha = ((float)soundActive + .25f);
        }
        // Ses Ayarının Sahne Yenilendiğinde Kayıtlı Veriye Göre Aktif Yada Pasif Hale Getirir - 
        //Enables or Disables Sound Adjustment Based on Saved Data when Scene is Refreshed
        private void SoundControl()
        {
            soundActive = PlayerPrefs.GetInt("soundStatus", 1);
            soundAlpha.alpha = ((float)soundActive + .25f);
        }
        // Titreşim Ayarının Sahne Yenilendiğinde Kayıtlı Veriye Göre Aktif Yada Pasif Hale Getirir - 
        //Enables or Disables Vibration Adjustment Based on Saved Data when Scene is Refreshed
        private void VibrateControl()
        {
            vibrateActive = PlayerPrefs.GetInt("vibrateStatus", 1);
            vibrateAlpha.alpha = ((float)vibrateActive + .25f);
        }
        // Settings Paneli ile İlgili Tüm Butonlara Tetiklendiğinde Çalışacak Methodlar Ekler - 
        //Adds Methods to Work When Triggered to All Buttons Related to Settings Panel
        private void ButtonsControl()
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(OpenSettings);

            vibrateButton.onClick.RemoveAllListeners();
            vibrateButton.onClick.AddListener(VibrateButton);

            soundButton.onClick.RemoveAllListeners();
            soundButton.onClick.AddListener(SoundButton);
        }
    }
}