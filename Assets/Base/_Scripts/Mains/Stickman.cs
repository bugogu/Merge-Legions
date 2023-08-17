using UnityEngine;

public class Stickman : MonoBehaviour
{
    [HideInInspector] public int level = 0;
    [HideInInspector] public StickmanDataSO _stickmanData;
    [HideInInspector] public Vector3 defaultRotation;
    private Animator _anim;
    void Awake()
    {
        _anim = GetComponent<Animator>();
        transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = (level + 1).ToString();
    }
    void Start()
    {
        defaultRotation = transform.rotation.eulerAngles;
        _stickmanData = GameManager.Instance.stickmanLevelDatas[level];
        transform.localScale *= _stickmanData.scaleMultiplier;
        transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = _stickmanData.stickmanMaterial;
        switch (_stickmanData.attackType)
        {
            case StickmanDataSO.AttackType.one_handed:
                _anim.SetTrigger("OneHand");
                GameObject obj = Instantiate(_stickmanData.weaponPrefab, this.transform);
                obj.transform.localPosition = new Vector3(-1.85f, 6.50f, -4);
                if (GameManager.Instance.isOnStarted)
                    obj.GetComponent<Weapon>().FireControl();
                break;
            case StickmanDataSO.AttackType.two_handed:
                _anim.SetTrigger("TwoHand");
                GameObject obje = Instantiate(_stickmanData.weaponPrefab, this.transform);
                obje.transform.localPosition = new Vector3(-1.85f, 6.50f, -4);
                if (GameManager.Instance.isOnStarted)
                    obje.GetComponent<Weapon>().FireControl();
                GameObject objec = Instantiate(_stickmanData.weaponPrefab, this.transform);
                objec.transform.localPosition = new Vector3(1.85f, 6.50f, -4);
                if (GameManager.Instance.isOnStarted)
                    objec.GetComponent<Weapon>().FireControl();
                break;
        }
    }
    public void RunAnim(bool status)
    {
        if (!(GameManager.gameModeOption == GameManager.GameMode.Runner)) return;
        _anim.SetBool("Walk", status);
    }
    public void RunWinDance()
    {
        _anim.SetTrigger("Win");
    }
    /// <summary>
    /// Stickman Oluşturulduktan Hemen Sonra Çalıştırılmalı, Bu Sayede Level Değerine Göre Özellikleri Yükler (Özellikleri Yükleme İşlemi Oluşturulan Stickman Sınıfının Start Metodunda Çalışır) -
    /// This Function Should be Run Immediately After The Stickman is Instantiated, so it Loads The Features According to The Level Value.
    /// </summary>
    /// <param name="value"></param>
    public void IncreaseLevel(int value)
    {
        level += value;
        transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text = (level + 1).ToString();
    }
    // Stickman Oluştuğunda Oyun Moduna Göre Alt Nesnesinde VFX Oynatır - When Stickman Generated, Plays VFX on Child Object by Game Mode
    private void OnEnable()
    {
        if (GameManager.Instance.isOnStarted) return;
        if (GameManager.gameModeOption == GameManager.GameMode.Defence)
            transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
        if (GameManager.gameModeOption == GameManager.GameMode.Runner)
            transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
    }
}