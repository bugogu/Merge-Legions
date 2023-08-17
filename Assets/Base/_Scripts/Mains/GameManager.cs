using UnityEngine;

public class GameManager : MonoSing<GameManager>
{
    #region Variables
    public static GameMode gameMode;
    public static int killedEnemys;
    public static float boostValue = 0;
    public enum GameMode
    {
        Runner,
        Defence
    };
    [Header("Arrays")]
    [Space]
    public GameObject[] squares;
    public StickmanDataSO[] stickmanLevelDatas;
    public WeaponDataSO[] weaponDatas;
    public BulletDataSO[] bulletDatas;
    public GameObject[] passiveUI;
    public GameObject[] levels;
    [SerializeField] private MeshFilter[] numbers;
    [Header("Objects")]
    [Space]
    public GameObject boardImage;
    public GameObject gemPrefab;
    public GameObject giftPrefab;
    [HideInInspector] public int collectedCoin;
    [HideInInspector] public bool isOnStarted, gameOver;
    [SerializeField] private GameObject num0, num1;
    [SerializeField] private GameObject road;
    public GameObject popupTextPrefab;
    [Header("Other")]
    [Space]
    [Range(0, 1)]
    public float dropGemPercent;
    [Range(0, 1)]
    public float dropGiftPercent;
    public AudioClip gemCollect;
    public AudioClip deathLegion;
    public GameObject[] normalLevels;
    public GameObject[] bossLevels;
    public AudioClip[] sfx;
    private GameObject[] _bricks;
    #endregion
    #region Properties
    public static GameMode gameModeOption
    {
        get => gameMode;
        set
        {
            gameMode = value;
            if (value != GameMode.Defence)
                ModeRunnerAnimation();
            if (value != GameMode.Runner)
                ModeDefenceAnimation();
        }
    }
    public static int modeData
    {
        get => PlayerPrefs.GetInt("GameMode", 1);
        set => PlayerPrefs.SetInt("GameMode", value);
    }
    public static int coin
    {
        get => PlayerPrefs.GetInt("Coin", 69);
        set
        {
            PlayerPrefs.SetInt("Coin", value);
            UIManager.Instance.UpdateCoin(value);
        }
    }
    public static int gem
    {
        get => PlayerPrefs.GetInt("Gem", 100);
        set
        {
            PlayerPrefs.SetInt("Gem", value);
            UIManager.Instance.UpdateGem(value);
        }
    }
    public static int stickmanPrice
    {
        get => PlayerPrefs.GetInt("StickmanPrice", 22);
        set => PlayerPrefs.SetInt("StickmanPrice", value);
    }
    public static int generateValue
    {
        get => PlayerPrefs.GetInt("GenerateValue", 0);
        set => PlayerPrefs.SetInt("GenerateValue", value);
    }
    public static int gameLevel
    {
        get => PlayerPrefs.GetInt("Level", 1);
        set => PlayerPrefs.SetInt("Level", value);
    }
    public static int finishCount
    {
        get => PlayerPrefs.GetInt("FinishCount", 0);
        set => PlayerPrefs.SetInt("FinishCount", value);
    }
    public static int bossDefeatCount
    {
        get => PlayerPrefs.GetInt("DefeatedBoss", 4);
        set => PlayerPrefs.SetInt("DefeatedBoss", value);
    }
    public static float coinMultiplier
    {
        get => PlayerPrefs.GetFloat("Multiplier", 0);
        set => PlayerPrefs.SetFloat("Multiplier", value);
    }
    public static int generateLevel
    {
        get => PlayerPrefs.GetInt("GenerateLevel", 1);
        set => PlayerPrefs.SetInt("GenerateLevel", value);
    }
    public static int incomeLevel
    {
        get => PlayerPrefs.GetInt("IncomeLevel", 1);
        set => PlayerPrefs.SetInt("IncomeLevel", value);
    }
    #endregion
    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (finishCount < 1)
        {
            GameObject level = Instantiate(levels[gameLevel - 1]);
            level.transform.parent = road.transform;
        }
        else
        {
            if (gameLevel % 5 != 0)
            {
                GameObject level = Instantiate(normalLevels[Random.Range(0, normalLevels.Length)]);
                level.transform.parent = road.transform;
            }
            else
            {
                GameObject level = Instantiate(bossLevels[Random.Range(0, bossLevels.Length)]);
                level.transform.parent = road.transform;
            }
        }
        _bricks = GameObject.FindGameObjectsWithTag("Brick");
        if (modeData == 0)
            gameModeOption = GameMode.Defence;
        if (modeData == 1)
            gameModeOption = GameMode.Runner;
        SetLevelModels();
        if (gameMode != GameMode.Defence) return;
        SetBricks(false);
    }
    // Sahnedeki Harf Modelleri İle Oyuncuya Aktif Olan Leveli Sahne Yenilendiğinde Yükleyerek Gösterir -
    // Shows the Active Level to the Player with the Letter Patterns in the Scene, by Loading it when the Scene is Reloaded
    private void SetLevelModels()
    {
        var levelString = (finishCount * levels.Length + gameLevel).ToString();
        if (levelString.Length == 1)
        {
            num0.GetComponent<MeshFilter>().mesh = numbers[0].mesh;
            num1.GetComponent<MeshFilter>().mesh = numbers[gameLevel].mesh;
        }
        else
        {
            num0.GetComponent<MeshFilter>().mesh = numbers[int.Parse(levelString[0].ToString())].mesh;
            num1.GetComponent<MeshFilter>().mesh = numbers[int.Parse(levelString[1].ToString())].mesh;
        }
    }
    // Oyun Moduna Göre Yol Objesinin Textureını Ters Çevirir ve Sahnedeki Stickmanlerin Alt Nesnesinde Bulunan Bir VFX Aktif Eder -
    // Inverts the Texture of the Road Object Based on the Game Mode and Activates a VFX in the Child Object of the Stickmans in the Scene
    static void ModeDefenceAnimation()
    {
        UIManager.Instance.PlayButtonMode("Defence");
        GameObject road = GameObject.FindObjectOfType<RoadMovement>().gameObject;
        road.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(.5f, 0);
        GameObject[] stickmans = GameObject.FindGameObjectsWithTag("Stickman");
        foreach (GameObject stickman in stickmans)
        {
            if (!(stickmans.Length > 0)) return;
            stickman.transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
            stickman.transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
        }
    }
    // Oyun Moduna Göre Yol Objesinin Textureını Ters Çevirir ve Sahnedeki Stickmanlerin Alt Nesnesinde Bulunan Bir VFX Aktif Eder -
    // Inverts the Texture of the Road Object Based on the Game Mode and Activates a VFX in the Child Object of the Stickmans in the Scene
    static void ModeRunnerAnimation()
    {
        UIManager.Instance.PlayButtonMode("Runner");
        GameObject road = GameObject.FindObjectOfType<RoadMovement>().gameObject;
        road.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(0, 0);
        GameObject[] stickmans = GameObject.FindGameObjectsWithTag("Stickman");
        foreach (GameObject stickman in stickmans)
        {
            if (!(stickmans.Length > 0)) return;
            stickman.transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
            stickman.transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// Level Biteceği Zaman Çalıştırılması Gereken Fonksiyon(Sıradaki Leveli Yüklemek İçin Bir Panel Açar) -
    /// Function to be Executed When Level Ends (Opens a Panel to Load the Next Level)
    /// </summary>
    public void GameOverAction(float transition, bool playerWin)
    {
        gameOver = true;
        UIManager.Instance.GameOverPanel(playerWin, transition);
        GameObject[] stickmans = GameObject.FindGameObjectsWithTag("Stickman");
        for (int i = 0; i < stickmans.Length; i++)
        {
            stickmans[i].GetComponent<Stickman>().RunAnim(false);
            stickmans[i].GetComponent<Stickman>().RunWinDance();
            stickmans[i].transform.GetChild(4).gameObject.SetActive(false);
            if (stickmans[i].GetComponent<Stickman>()._stickmanData.attackType == StickmanDataSO.AttackType.two_handed)
                stickmans[i].transform.GetChild(5).gameObject.SetActive(false);
            if (GameManager.gameModeOption == GameManager.GameMode.Defence)
                stickmans[i].transform.GetChild(2).gameObject.SetActive(false);
        }
    }
    public void SetBricks(bool status)
    {
        foreach (GameObject brick in _bricks)
        {
            brick.SetActive(status);
        }
    }
    public void TakeBoost(float boostTime, float boostPercent)
    {
        boostValue = boostPercent;
        Invoke(nameof(CancelBoost), boostTime);
    }
    private void CancelBoost() => boostValue = 0;
}