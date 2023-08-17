using UnityEngine;
using DG.Tweening;
using SupersonicWisdomSDK;

public class UIManager : MonoSing<UIManager>
{
    #region Variables
    [Header("Buttons")]
    [SerializeField] private UnityEngine.UI.Button clearAllDataButton;
    public UnityEngine.UI.Button generateStickmanButton;
    [SerializeField] private UnityEngine.UI.Button playButton, gameOverButton;
    public UnityEngine.UI.Button defenceModeButton, runnerModeButton;
    [SerializeField] private UnityEngine.UI.Button shopButton;
    [SerializeField] private UnityEngine.UI.Button generatePowerButton, incomePowerButton;
    [Space]
    [Header("Texts")]
    [SerializeField] private TMPro.TMP_Text clearDataText;
    [SerializeField] private TMPro.TMP_Text stickmanPriceText, coinText, gemText;
    [SerializeField] private TMPro.TMP_Text gameOverCoinText;
    [SerializeField] private TMPro.TMP_Text gameOverGemText;
    [SerializeField] private TMPro.TMP_Text generatePriceText, incomePriceText;
    [SerializeField] private TMPro.TMP_Text generateLevelText, incomeLevelText;
    [Space]
    [Header("Colors")]
    [SerializeField] private Color defenceButtonColor;
    [SerializeField] private Color attackButtonColor;
    [SerializeField] private Color attackBoardColor, defenceBoardColor;
    [SerializeField] private Color defenceButtonShadowColor, attackButtonShadowColor;
    [Space]
    [Header("Objects")]
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject stickmanPrefab;
    public GameObject defenceButtonOutline, runnerButtonOutline;
    [SerializeField] private GameObject gameOverPanel, level;
    [SerializeField] private GameObject clearDataPanel;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private GameObject tutorialHand;
    public RectTransform guideObject;
    [SerializeField] private GameObject defeatBossText;
    [Space]
    [Header("Others")]
    public AudioClip clickSound;
    [SerializeField] private AudioClip powerSound;
    [SerializeField] private BoardManager boardScript;
    [SerializeField] private SpriteRenderer boardSprite;
    [SerializeField] private ParticleSystem purchaseFXCoin;
    [SerializeField] private ParticleSystem purchaseFXGem;
    [SerializeField] private Sprite winSp, loseSp;
    [SerializeField] private AudioClip winSFX, loseSFX;
    [SerializeField] private Rigidbody[] levelElements;
    [SerializeField] private int scatterRandomness;
    [SerializeField] private GameObject shopPanel;
    public Vector3[] handPositions;
    [HideInInspector] public GameObject[] _stickmans;
    [HideInInspector] public GameObject[] _weapons;
    private LevelLoader loadLevel;
    [HideInInspector] public RectTransform _handTransform;
    [HideInInspector] public int _tutorialPhase;
    #endregion
    public static int tutorialActive
    {
        get => PlayerPrefs.GetInt("Tutorial", 1);
        set => PlayerPrefs.SetInt("Tutorial", value);
    }
    public static int generatePrice
    {
        get => PlayerPrefs.GetInt("GeneratePrice", 500);
        private set => PlayerPrefs.SetInt("GeneratePrice", value);
    }
    public static int incomePrice
    {
        get => PlayerPrefs.GetInt("IncomePrice", 100);
        private set => PlayerPrefs.SetInt("IncomePrice", value);
    }
    private void Start()
    {
        loadLevel = GameObject.FindObjectOfType<LevelLoader>();

        if (tutorialActive == 1)
        {
            var generated = Instantiate(tutorialHand, mainCanvas.transform);
            _handTransform = generated.GetComponent<RectTransform>();
            generated.GetComponent<RectTransform>().DOScale(Vector3.one, .5f).SetLoops(100, LoopType.Yoyo);
        }

        if (boardScript._levelsInBoard.Count > 15)
            generateStickmanButton.gameObject.SetActive(false);
        if (!(GameManager.coin >= GameManager.stickmanPrice))
            generateStickmanButton.gameObject.SetActive(false);

        if (GameManager.modeData == 0)
        {
            defenceButtonOutline.SetActive(true);
            boardSprite.color = defenceBoardColor;
        }

        if (GameManager.modeData == 1)
        {
            runnerButtonOutline.SetActive(true);
            boardSprite.color = attackBoardColor;
        }

        #region Text Control
        generatePriceText.text = generatePrice.ToString();
        incomePriceText.text = incomePrice.ToString();
        generateLevelText.text = "Level " + GameManager.generateLevel.ToString();
        incomeLevelText.text = "Level " + GameManager.incomeLevel.ToString();

        coinText.text = GameManager.coin.ToString();
        gemText.text = GameManager.gem.ToString();
        stickmanPriceText.text = GameManager.stickmanPrice.ToString();
        clearDataText.text = "game data will be <color=red>deleted</color>";
        #endregion

        if (GameManager.gem < incomePrice)
            incomePowerButton.interactable = false;
        if (GameManager.gem < generatePrice)
            generatePowerButton.interactable = false;
        if (GameManager.bossDefeatCount < 1)
        {
            generatePowerButton.interactable = false;
            defeatBossText.SetActive(true);
        }

        ButtonsController();
    }
    public void UpdateCoin(int value)
    {
        coinText.text = value.ToString();
    }
    public void UpdateGem(int value)
    {
        gemText.text = value.ToString();
    }
    private void PlayButton()
    {
        My.PlaySound(clickSound);
        if (tutorialActive == 1)
        {
            if (_tutorialPhase != 9) return;
            Destroy(_handTransform.gameObject);
            tutorialActive = 0;
            _tutorialPhase = 0;
        }
        PlayAnimation();
        SetRunAnimEnemys();
        defenceButtonOutline.SetActive(false);
        runnerButtonOutline.SetActive(false);
        GameManager.Instance.isOnStarted = true;
        if (GameManager.gameModeOption == GameManager.GameMode.Runner)
            GameManager.Instance.boardImage.SetActive(false);
        if (GameManager.gameModeOption == GameManager.GameMode.Defence)
        {
            generateStickmanButton.gameObject.SetActive(true);
            generateStickmanButton.interactable = false;
        }
        for (int i = 0; i < GameManager.Instance.passiveUI.Length; i++)
        {
            GameManager.Instance.passiveUI[i].SetActive(false);
        }
        _stickmans = GameObject.FindGameObjectsWithTag("Stickman");
        _weapons = GameObject.FindGameObjectsWithTag("Weapon");
        for (int i = 0; i < _stickmans.Length; i++)
        {
            _stickmans[i].GetComponent<Stickman>().RunAnim(true);
            _stickmans[i].transform.GetChild(3).gameObject.SetActive(false);
            if (GameManager.gameModeOption == GameManager.GameMode.Runner)
                _stickmans[i].transform.GetChild(2).gameObject.SetActive(false);
        }
        for (int i = 0; i < _weapons.Length; i++)
        {
            _weapons[i].GetComponent<Weapon>().FireControl();
        }
        BoardManager.OpenHealtBar();
        BoardManager.SetHealth();
        SupersonicWisdom.Api.NotifyLevelStarted((GameManager.finishCount * GameManager.Instance.levels.Length) + GameManager.gameLevel, null);
    }
    // Board daki Kareleri Soldan Sağa Tarayıp Boş Olan İlk Kareye Stickman Yerleştirir ve Sonrasında Board daki Karelerinin Hepsinin Dolu Olması Durumuna yada Yeterli Coin Olmamasına Göre Butonu Pasif Yapar -
    // Scans the Squares on the Board from Left to Right, Places Stickman on the First Empty Square, and Then Deactivates the Button According to the Case of All Squares on the Board or Not Enough Coins.
    private void GenerateStickman()
    {
        My.PlaySound(clickSound);
        if (GameManager.Instance.gameOver) return;
        if (tutorialActive == 1)
        {
            _tutorialPhase++;
            if (_tutorialPhase == 3)
                _handTransform.localPosition = handPositions[_tutorialPhase - 3];
        }
        CameraShake.ShakeToCamera();
        GameManager.coin -= GameManager.stickmanPrice;
        GameManager.stickmanPrice += 1;
        purchaseFXCoin?.Play();
        for (int i = 0; i < GameManager.Instance.squares.Length; i++)
        {
            if (GameManager.Instance.squares[i].transform.childCount < 1)
            {
                GameObject generated = Instantiate(stickmanPrefab, GameManager.Instance.squares[i].transform, true);
                generated.transform.localPosition = Vector3.back;
                generated.GetComponent<Stickman>().IncreaseLevel(GameManager.generateValue);
                break;
            }
        }
        if (!(GameManager.coin >= GameManager.stickmanPrice))
        {
            if (!GameManager.Instance.isOnStarted)
                generateStickmanButton.gameObject.SetActive(false);
            if (GameManager.Instance.isOnStarted)
                generateStickmanButton.interactable = false;
        }
        _stickmans = GameObject.FindGameObjectsWithTag("Stickman");
        if (!(_stickmans.Length < 16))
        {
            if (!GameManager.Instance.isOnStarted)
                generateStickmanButton.gameObject.SetActive(false);
            if (GameManager.Instance.isOnStarted)
                generateStickmanButton.interactable = false;
        }
        boardScript.SaveAllInformation();
        stickmanPriceText.text = GameManager.stickmanPrice.ToString();
    }
    /// <summary>
    /// Stickman Oluşturma Butonunu Gelen Parametreye Göre Coin Sayısı Yeterliyse Aktif Eder
    /// Activates the Stickman Generate Button if the Number of Coins is Enough According to the Incoming Parameter
    /// </summary>
    /// <param name="status"></param>
    public void GenerateButtonActive(bool status)
    {
        if (!(GameManager.coin >= GameManager.stickmanPrice)) return;
        generateStickmanButton.gameObject.SetActive(status);
    }
    /// <summary>
    /// Oyun Sonu Panelini Açar ve Panelin İçindeki Butonun Görünümünü Parametre Olarak Gelen playerWin Durumuna Göre Değiştirir ve Ses Efekti Oynatır
    /// Opens the End Game Panel and Changes the Appearance of the Button in the Panel According to the Incoming playerWin Parameter and Plays the Sound Effect
    /// </summary>
    /// <param name="playerWin"> Oyuncu Leveli Başarılı Bir Şekilde Tamamladı Mı?
    /// Did the Player Successfully Complete the Level?
    /// </param>
    /// <param name="transition"> Panelin Boyunun Büyültülme Süresi - Panel Scaleing Time
    /// </param>
    public void GameOverPanel(bool playerWin, float transition)
    {
        Sprite gameOverPanelButtonSprite;
        if (playerWin)
        {
            gameOverPanelButtonSprite = winSp;
            My.PlaySound(winSFX);
            SupersonicWisdom.Api.NotifyLevelCompleted((GameManager.finishCount * GameManager.Instance.levels.Length) + GameManager.gameLevel, null);
        }
        else
        {
            gameOverPanelButtonSprite = loseSp;
            My.PlaySound(loseSFX);
            SupersonicWisdom.Api.NotifyLevelFailed((GameManager.finishCount * GameManager.Instance.levels.Length) + GameManager.gameLevel, null);
        }
        gameOverPanel.SetActive(true);
        gameOverPanel.transform.parent.GetChild(0).gameObject.SetActive(true);
        gameOverPanel.GetComponent<RectTransform>().DOScale(Vector3.one, transition);
        gameOverButton.gameObject.GetComponent<UnityEngine.UI.Image>().sprite = gameOverPanelButtonSprite;
    }
    // Oyun Sonu Panelindeki Sıradaki Leveli Yükleyen Button Fonksiyonu -
    // Button Function to Load Next Level in Gameover Panel
    private void GameOverButton()
    {
        My.PlaySound(clickSound);
        if (GameManager.gameModeOption == GameManager.GameMode.Runner)
            GameManager.coin += (GameManager.Instance.collectedCoin + (int)(GameManager.Instance.collectedCoin * GameManager.coinMultiplier));
        GameManager.gem += ((GameManager.killedEnemys / 2) + GameManager.gameLevel);
        gameOverButton.interactable = false;
        gameOverPanel.SetActive(false);
        GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>().SaveAllInformation();

        if (GameManager.gameLevel < GameManager.Instance.levels.Length)
        {
            if (!BoardManager.isDeath)
            {
                if (GameManager.gameLevel % 5 == 0)
                    GameManager.bossDefeatCount += 1;
                GameManager.gameLevel += 1;
            }
        }
        else
        {
            GameManager.bossDefeatCount += 1;
            if (!BoardManager.isDeath)
            {
                GameManager.gameLevel = 1;
                GameManager.finishCount += 1;
            }
        }

        loadLevel.LoadNextLevel();
    }
    // Sahnedeki Leveli Gösteren Nesnelere Saçılma Efekti Uyguluyor - 
    // Applying a Scattering Effect to Objects Indicating Level in the Scene
    private void PlayAnimation()
    {
        foreach (Rigidbody letters in levelElements)
        {
            float scatterX = Random.Range(-scatterRandomness, scatterRandomness);
            float scatterY = Random.Range(0, scatterRandomness);
            float scatterZ = Random.Range(0, scatterRandomness);
            Vector3 scatterVector = new Vector3(scatterX, scatterY, scatterZ);
            letters.AddForce(scatterVector, ForceMode.Impulse);
            letters.transform.rotation = Quaternion.Euler(scatterVector);
        }
        // level Objesinin Alt Nesnesindeki Smoke VFX Objesini Aktif Eder -
        // Activates the Smoke VFX Object in the Child Object of the level Object
        level.transform.GetChild(2).gameObject.SetActive(true);
        Destroy(level, 1);
    }
    /// <summary>
    /// Levele Başlamak İçin Kullanılan Butonun Görünümünü Oyun Moduna Göre Değiştirir
    /// Changes the Appearance of the Button Used to Start Leveling According to the Game Mode
    /// </summary>
    /// <param name="playMode"></param>
    public void PlayButtonMode(string playMode)
    {
        SetVisibleRunnerButton();
        if (playMode != "Defence") return;
        SetVisibleDefenceButton();
    }
    public void SetCollectedRewardsText(int gemValue = 0, int coinValue = 0, bool bossLevel = false)
    {
        if (!bossLevel)
        {
            gameOverCoinText.text = (GameManager.Instance.collectedCoin + (int)(GameManager.Instance.collectedCoin * GameManager.coinMultiplier)).ToString();
            gameOverGemText.text = (GameManager.killedEnemys * GameManager.gameLevel).ToString();
        }
        else
        {
            gameOverCoinText.text = (coinValue).ToString();
            gameOverGemText.text = (gemValue).ToString();
        }
    }
    // Inspector Panelinden Referans Alınan Butonlara Fonksiyon Yükler -
    // Loads Functions on Buttons Referenced from the Inspector Panel
    private void ButtonsController()
    {
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(PlayButton);

        generateStickmanButton.onClick.RemoveAllListeners();
        generateStickmanButton.onClick.AddListener(GenerateStickman);

        shopButton.onClick.RemoveAllListeners();
        shopButton.onClick.AddListener(ShopButton);

        gameOverButton.onClick.RemoveAllListeners();
        gameOverButton.onClick.AddListener(GameOverButton);

        defenceModeButton.onClick.RemoveAllListeners();
        defenceModeButton.onClick.AddListener(DefenceModeButton);

        runnerModeButton.onClick.RemoveAllListeners();
        runnerModeButton.onClick.AddListener(RunnerModeButton);

        generatePowerButton.onClick.RemoveAllListeners();
        generatePowerButton.onClick.AddListener(GeneratePowerButton);

        incomePowerButton.onClick.RemoveAllListeners();
        incomePowerButton.onClick.AddListener(IncomePowerButton);

        clearAllDataButton.onClick.RemoveAllListeners();
        clearAllDataButton.onClick.AddListener(ClearAllDataButton);
    }
    // Oyun Modunu Değiştirip Kaydeder -
    // Changes and Saves Game Mode
    private void DefenceModeButton()
    {
        My.PlaySound(clickSound);
        if (_tutorialPhase == 7)
        {
            _tutorialPhase++;
            _handTransform.localPosition = handPositions[_tutorialPhase - 3];
        }
        boardSprite.color = defenceBoardColor;
        GameManager.gameModeOption = GameManager.GameMode.Defence;
        defenceButtonOutline.SetActive(true);
        runnerButtonOutline.SetActive(false);
        GameManager.Instance.SetBricks(false);
        GameManager.modeData = 0;
    }
    // Oyun Modunu Değiştirip Kaydeder -
    // Changes and Saves Game Mode
    private void RunnerModeButton()
    {
        My.PlaySound(clickSound);
        if (_tutorialPhase == 8)
        {
            _tutorialPhase++;
            _handTransform.localPosition = handPositions[_tutorialPhase - 3];
        }
        boardSprite.color = attackBoardColor;
        GameManager.gameModeOption = GameManager.GameMode.Runner;
        defenceButtonOutline.SetActive(false);
        runnerButtonOutline.SetActive(true);
        GameManager.Instance.SetBricks(true);
        GameManager.modeData = 1;
    }
    // Tüm Verileri Temizle Panelindeki Sil Butonu İçin Kullanılan Fonksiyon -
    // Function Used for the Delete Button in the Clear All Data Panel
    private void ClearAllDataButton()
    {
        My.PlaySound(clickSound);
        DataController.Instance.DeleteDataFiles();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("FirstLoad", "False");
        GameManager.gameLevel = 1;
        loadLevel.LoadNextLevel();
    }
    public void CreditPanelControl() => creditPanel.SetActive(!creditPanel.activeSelf);
    // Referans Fonksiyonunu İncele - Review Reference Function
    private void SetVisibleDefenceButton()
    {
        playButton.GetComponent<UnityEngine.UI.Image>().color = defenceButtonColor;
        playButton.GetComponent<UnityEngine.UI.Shadow>().effectColor = defenceButtonShadowColor;
        playButton.gameObject.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = "Defence";
    }
    // Referans Fonksiyonunu İncele - Review Reference Function
    private void SetVisibleRunnerButton()
    {
        playButton.GetComponent<UnityEngine.UI.Image>().color = attackButtonColor;
        playButton.GetComponent<UnityEngine.UI.Shadow>().effectColor = attackButtonShadowColor;
        playButton.gameObject.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Text>().text = "Attack";
    }
    // Bu Fonksiyon Tüm Verileri Silme Panelini Açan Buton ve bu Pnaeldeki Kapama Butonu İçin Kullanılıyor -
    // This Function Is Used For The Button To Open All Data Delete Panel And For The Close Button In This Panel
    public void ClearAllDataPanelButton() => clearDataPanel.SetActive(!clearDataPanel.activeSelf);
    private void ShopButton()
    {
        My.PlaySound(clickSound);
        if (tutorialActive == 1)
            return;
        ShopPanel();
    }
    private void ShopPanel() => shopPanel.SetActive(!shopPanel.activeSelf);
    private void GeneratePowerButton()
    {
        My.PlaySound(powerSound);
        GameManager.gem -= generatePrice;
        GameManager.generateLevel += 1;
        GameManager.bossDefeatCount -= 1;
        GameManager.generateValue += 1;
        generatePrice += 500;
        generateLevelText.text = "Level " + GameManager.generateLevel.ToString();
        generatePriceText.text = generatePrice.ToString();
        purchaseFXGem?.Play();
        if (GameManager.bossDefeatCount < 1)
        {
            generatePowerButton.interactable = false;
            defeatBossText.SetActive(true);
        }
        if (GameManager.gem < generatePrice)
            generatePowerButton.interactable = false;
        if (GameManager.gem < incomePrice)
            incomePowerButton.interactable = false;

        for (int i = 0; i < boardScript._squaresFilled.Count; i++)
        {
            if (boardScript._squaresFilled[i] == 1)
            {
                if (GameManager.Instance.squares[i].transform.GetChild(0).GetComponent<Stickman>().level < GameManager.generateValue)
                    Destroy(GameManager.Instance.squares[i].transform.GetChild(0).gameObject);
            }
        }
        boardScript.SaveAllInformation();
    }
    private void IncomePowerButton()
    {
        My.PlaySound(powerSound);
        GameManager.gem -= incomePrice;
        GameManager.coinMultiplier += .10f;
        GameManager.incomeLevel += 1;
        incomePrice += 50;
        incomeLevelText.text = "Level " + GameManager.incomeLevel.ToString();
        incomePriceText.text = incomePrice.ToString();
        purchaseFXGem?.Play();
        if (GameManager.gem < incomePrice)
            incomePowerButton.interactable = false;
        if (GameManager.gem < generatePrice)
            generatePowerButton.interactable = false;
    }
    private void SetRunAnimEnemys()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemys)
        {
            enemy.GetComponent<Enemy>().SetRunAnim(true);
        }
    }
    #region Test Inputs
#if UNITY_EDITOR
    private void Update()
    {
        IncreaseCoinAndGem();
        DeleteData();
        AnyEvent();
    }
#endif
    private void IncreaseCoinAndGem()
    {
        if (!Input.GetKeyDown(KeyCode.Q)) return;
        GameManager.coin += 123;
        GameManager.gem += 100;
        if ((GameManager.coin >= GameManager.stickmanPrice))
        {
            generateStickmanButton.gameObject.SetActive(true);
            generateStickmanButton.interactable = true;
        }
    }
    private void AnyEvent()
    {
        if (!Input.GetKeyDown(KeyCode.W)) return;
        GameManager.coin = 0;
        tutorialActive = 0;
    }
    private void DeleteData()
    {
        if (!Input.GetKeyDown(KeyCode.A)) return;
        DataController.Instance.DeleteDataFiles();
    }
    #endregion
}