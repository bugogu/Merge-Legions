using UnityEngine;

public class BoardManager : MonoBehaviour
{
    #region Variables
    public const byte RANDOMNESS = 100;
    [SerializeField] private GameObject stickmanPrefab;
    [Space]
    public System.Collections.Generic.List<int> _squaresFilled;
    public System.Collections.Generic.List<int> _levelsInBoard;
    [HideInInspector] public static bool isDeath;
    private static GameObject _healthCanvas;
    private static UnityEngine.UI.Image _healthBar;
    public static int _health;
    #endregion
    private void Start()
    {
        DataController.Instance.LoadLevel();
        DataController.Instance.LoadStickman();
        _healthCanvas = transform.GetChild(transform.childCount - 1).gameObject;
        _healthBar = _healthCanvas.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>();
        isDeath = false;
        SetBoardInformation();
    }
    public static void OpenHealtBar()
    {
        _healthCanvas.SetActive(true);
    }
    /// <summary>
    /// Board daki Stickmanlerin Seviyelerinin Toplamı Çarpı 10 Kadar _healt Değişkenine Değer Atar(Bu Fonksiyon Çağırıldığında Oyun Modu Defence ise _healt %20 Daha Yüksek Olur)
    /// The sum of the Stickman's Levels on the Board Sets the Value of the _healt Variable by 10 times (When This Function is Called, _healt Will Be 20% Higher If Game Mode is Defense)
    /// </summary>
    public static void SetHealth()
    {
        GameObject[] stickmans = GameObject.FindGameObjectsWithTag("Stickman");
        int tempHealt = 0;
        for (int i = 0; i < stickmans.Length; i++)
        {
            tempHealt += stickmans[i].GetComponent<Stickman>().level + 1;
        }
        tempHealt *= 10;
        if (GameManager.gameModeOption == GameManager.GameMode.Runner)
            _health = tempHealt;
        if (GameManager.gameModeOption == GameManager.GameMode.Defence)
            _health = tempHealt + (int)(tempHealt * .20f);
    }
    /// <summary>
    /// Board Objesine Canını Azaltması Gereken Bir Nesne Değdiğinde Çalıştırılmalı
    /// It should be run when an object that should reduce its health touches the board object
    /// </summary>
    /// <param name="damageValue"></param>
    public static void TakeDamage(float damageValue)
    {
        DamagePopupText.GeneratePopup(damageValue, true, Color.red);
        _healthBar.fillAmount -= (damageValue / (float)_health);
        if (_healthBar.fillAmount <= 0)
        {
            _healthCanvas.SetActive(false);
            KillSelf();
        }
    }
    public static void TakeHealth(float healthValue)
    {
        DamagePopupText.GeneratePopup(healthValue, false, Color.green);
        if (_healthBar.fillAmount < 1)
            _healthBar.fillAmount += (healthValue / (float)_health);
    }
    // _healt Değişkeni 0 veya Altına İndiği Zaman Çalışacak Bir Fonksiyon, Sahnedeki Stickmanleri Bulup Koşma Animasyonlarını İptal Eder ve Board daki Karelere Saçılma Efekti Uygular -
    // This Function That Will Run When The _healt Variable Gets 0 or Below, Finds Stickmans in the Scene, Cancels Running Animations and Applies a Scatter Effect to Squares on the Board
    private static void KillSelf()
    {
        GameManager.Instance.gameOver = true;
        isDeath = true;
        My.PlaySound(GameManager.Instance.deathLegion);
        GameObject[] stickmans = GameObject.FindGameObjectsWithTag("Stickman");
        //düşmanlar kazandığı zaman kazanma animasyonu oynatmada bug oluşuyor null reference hatası alınıyor(Muhtemel sorun bazı düşmanlar tam lürken enemy script leri silindiği için)
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemys.Length; i++)
        {
            if (enemys[i].TryGetComponent(out Enemy enemy))
            {
                enemy.SetWinAnim();
            }
        }
        #region Scattering
        foreach (GameObject stickman in stickmans)
        {
            float scatterX = Random.Range(-RANDOMNESS, RANDOMNESS);
            float scatterY = Random.Range(-RANDOMNESS, RANDOMNESS);
            float scatterZ = Random.Range(-RANDOMNESS, RANDOMNESS);
            Vector3 scatterVector = new Vector3(scatterX, scatterY, scatterZ);
            stickman.GetComponent<Rigidbody>().AddForce(scatterVector, ForceMode.Impulse);
            stickman.transform.rotation = Quaternion.Euler(scatterVector);
        }
        #endregion
        for (int i = 0; i < stickmans.Length; i++)
        {
            stickmans[i].GetComponent<Stickman>().RunAnim(false);
        }
        CameraShake.ShakeToCamera();
        My.DoVibrate();
        UIManager.Instance.GameOverPanel(false, 1);
    }
    private void SaveBoardInformation()
    {
        SaveBoardLevel();
        SaveBoardStickman();
    }
    // _squaresFilled Listesindeki Değerlerin 1 Olanlarının Bulunduğu İndeks ile squares İndeksindeki Kareyi Eşleştirip Bu Karede Stickman Oluşturur -
    // Matches the Index of Values ​​1 in the _squaresFilled List with the Square in the Squares Index, and Creates a Stickman in This Frame
    private void SetBoardInformation()
    {
        for (int i = 0; i < _squaresFilled.Count; i++)
        {
            if (_squaresFilled[i] == 1)
            {
                GameObject generated = Instantiate(stickmanPrefab, GameManager.Instance.squares[i].transform, true);
                generated.transform.localPosition = Vector3.back;
            }
        }
        SetBoardLevel();
    }
    // Sahnedeki Stickmanleri Bulup _levelsInBoard Listesindeki Seviyeleri Sırayla Aktarır(SetBoardInformation Fonksiyonundan Sonra Çağırılmalıdır) -
    // Finds the Stickmans in the Scene and Transfers the Levels in the _levelsInBoard List in Order (Must be Called After the SetBoardInformation Function)
    private void SetBoardLevel()
    {
        GameObject[] stickmans = GameObject.FindGameObjectsWithTag("Stickman");
        for (int i = 0; i < stickmans.Length; i++)
        {
            stickmans[i].GetComponent<Stickman>().IncreaseLevel(_levelsInBoard[i]);
        }
    }
    // Board daki Stickmanlerin Seviyelerini Sıralı Şekilde Bir Listeye Aktarır -
    // Transfers the Levels of Stickmans on the Board to a List in Order
    private void SaveBoardLevel()
    {
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            try
            {
                if (transform.GetChild(i).childCount > 0)
                    _levelsInBoard.Add(transform.GetChild(i).GetChild(0).GetComponent<Stickman>().level);
                else
                    continue;
            }
            catch (System.NullReferenceException)
            {
                Debug.Log("Hızlı Şekilde Merge İşlemi Yapıldığında Bazı Objeler Null Reference Döndürüyor", gameObject);
                Debug.Log("Some Objects Return Null Reference When Merged Quickly, BoardManager 132.", gameObject);
                return;
            }
        }
    }
    // Board daki Stickmanlerin Sayısını ve Sıralarını Bir Listeye 0 ve 1 Olarak Aktarır - 
    // Transfer the Number and Order of Stickmans on the Board as 0 and 1 in a List
    private void SaveBoardStickman()
    {
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            if (transform.GetChild(i).childCount > 0)
                _squaresFilled.Add(1);
            if (!(transform.GetChild(i).childCount > 0))
                _squaresFilled.Add(0);
        }
    }
    // Board daki Karelerde Stickman lerin Olup Olmama Durumunu ve Seviyelerini Belirlemekte Kullanılan 2 Listenin İçeriğini Siler -
    // Deletes the Contents of 2 Lists Used to Determine the Presence and Levels of Stickmans in the Squares on the Board
    private void ClearDataList()
    {
        _squaresFilled.Clear();
        _levelsInBoard.Clear();
    }
    /// <summary>
    /// Bu Fonksiyon Çağırıldığı Zamanki Board Düzeni 2 Listeye Aktarılır ve Json Olarak Kaydedilir
    /// Board Layout When Called This Function Is Transferred To 2 Lists And Saved As Json
    /// </summary>
    public void SaveAllInformation()
    {
        ClearDataList();
        SaveBoardInformation();
        DataController.Instance.SaveLevel();
        DataController.Instance.SaveStickman();
    }
    private void OnEnable()
    {
        SetBoardInformation();
    }
}