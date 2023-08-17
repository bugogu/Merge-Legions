using UnityEngine;
using DG.Tweening;
public class MergeController : MonoBehaviour
{
    #region Variables
    [SerializeField] private byte maxLevel;
    [SerializeField] private GameObject stickmanPrefab;
    [SerializeField] private GameObject mergeFXPrefab;
    private GameObject _currentObject;
    private Camera _cam;
    private Ray _ray;
    private RaycastHit _hit;
    #endregion
    private void Start()
    {
        _cam = Camera.main;
    }
    private void Update()
    {
        MergeMechanic();
    }
    // Fonksiyon Gönderilen Işının Stickman Objesine Square Objesine veya Bunlar Dışında Bir Objeye Temas Durumlarında Çalışır -
    // The Function Operates When the Emitted Ray Contacts the Stickman Object, the Square Object, or an Object Other Than These
    private void MergeControl()
    {
        if (_hit.collider.TryGetComponent(out Stickman stickman)) // _currentObject Değeri Sadece Bu if Bloğunda Belirlenir - _currentObject Value Set as Fill Only In This If Block
        {
            if (_currentObject == null) // İlk _currentObject Değerini Belirleyen Block - Block Determining First Current Object Value
            {
                _currentObject = _hit.transform.gameObject;
                stickman.transform.parent.GetComponent<SpriteRenderer>().enabled = true;
                if (UIManager.tutorialActive == 1)
                    if (UIManager.Instance._tutorialPhase == 3 || UIManager.Instance._tutorialPhase == 5)
                    {
                        UIManager.Instance._tutorialPhase++;
                        UIManager.Instance._handTransform.localPosition = UIManager.Instance.handPositions[UIManager.Instance._tutorialPhase - 3];
                    }
            }
            else // Merge Mekaniğinin Çalıştığı Ana Block - The Main Block on which Merge Mechanics Work
            {
                Stickman firstClicked = _currentObject.GetComponent<Stickman>();
                Stickman secondClicked = _hit.collider.GetComponent<Stickman>();
                MergeStickman(firstClicked, secondClicked);
            }
        }
        else if (_hit.collider.CompareTag("Square")) // Işın Bir Kareye Temas Ederse - If the Ray Contacts a Square
        {
            // currentObject Değeri Boş Değilse ve Temas Edilen Square Objesinin Alt Nesnesi Yoksa currentObject Objesi Square Objesinin Konumuna Işınlanır -
            // If the Value of currentObject is Not Null and the Contacted Square Object Doesn't Have a Child Object, the currentObject Object Teleports to the Location of the Square Object
            if (_currentObject == null) return;
            if (GameManager.Instance.isOnStarted) return;
            _currentObject.transform.parent.GetComponent<SpriteRenderer>().enabled = false;
            if ((_hit.transform.childCount < 1))
            {
                if (UIManager.tutorialActive == 1)
                    if (UIManager.Instance._tutorialPhase == 6)
                    {
                        UIManager.Instance._tutorialPhase++;
                        UIManager.Instance._handTransform.localPosition = UIManager.Instance.handPositions[UIManager.Instance._tutorialPhase - 3];
                    }
                    else { return; }
                _currentObject.transform.position = _hit.transform.position;
                _currentObject.transform.parent = _hit.transform;
                _currentObject.transform.DORotate(new Vector3(0, 360), 0.35f);
                _currentObject.transform.DORotate(new Vector3(0, 180), 0.35f).SetDelay(0.45f);
                _currentObject = null;
                gameObject.GetComponent<BoardManager>().SaveAllInformation();
            }
            else { _currentObject = null; } // Bu else Bloğunun Kaldırılması Bug a Neden Olur - Removing This else Block Causes a Bug
        }
        else // Işın Stickman Seçiliyken Kare yada Stickman Dışında Bir Nesneye Temas Ederse - If the Beam Contacts a Square or Non-Stickman Object While the Stickman is Selected
        {
            if (_currentObject == null) return;
            _currentObject.transform.parent.GetComponent<SpriteRenderer>().enabled = false;
            _currentObject = null;
            return;
        }
    }
    // Oyun Başlamışsa ve Oyun Modu Defence Değilse Bu Fonksiyon Çalışmaz, Ekrana Dokunulduğunda Bir Nesneye Temas Ederse MergeControl Fonksiyonu Çalıştırılır -
    // This Function Does Not Work If The Game Has Been Started And The Game Mode Is Not Defense, If The Screen Is Touched With An Object, The MergeControl Function Is Run
    private void MergeMechanic()
    {
        if (GameManager.Instance.gameOver) return;
        if (GameManager.Instance.isOnStarted && GameManager.gameModeOption != GameManager.GameMode.Defence) return;
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mousePos = Input.mousePosition;
        _ray = _cam.ScreenPointToRay(mousePos);

        if (!Physics.Raycast(_ray, out _hit, 100)) return;
        MergeControl();
    }
    private void MergeStickman(Stickman first, Stickman second)
    {
        _currentObject = second.gameObject;
        first.transform.parent.GetComponent<SpriteRenderer>().enabled = false;
        second.transform.parent.GetComponent<SpriteRenderer>().enabled = true;

        if (!(first.level == second.level)) return;
        second.transform.parent.GetComponent<SpriteRenderer>().enabled = false;

        if (!(first.GetInstanceID() != second.GetInstanceID())) return;

        _currentObject = second.gameObject;
        first.transform.parent.GetComponent<SpriteRenderer>().enabled = false;
        second.transform.parent.GetComponent<SpriteRenderer>().enabled = true;

        if (UIManager.tutorialActive == 1)
            if (UIManager.Instance._tutorialPhase == 4)
            {
                UIManager.Instance._tutorialPhase++;
                UIManager.Instance._handTransform.localPosition = UIManager.Instance.handPositions[UIManager.Instance._tutorialPhase - 3];
            }
            else { return; }

        if (first.level == (maxLevel - 1) || second.level == (maxLevel - 1)) return; // Merge Mekaniğinin Kesin Olarak Gerçekleştiği Block - The Block Where Merge Mechanics Exactly Happened
        CameraShake.ShakeToCamera();
        GameObject mergeFX = Instantiate(mergeFXPrefab, second.transform.parent, true);
        mergeFX.transform.localPosition = Vector3.zero;
        Destroy(mergeFX, 1);
        GameObject mergedStickman = Instantiate(stickmanPrefab, second.transform.parent, true);
        mergedStickman.transform.localPosition = Vector3.back;
        mergedStickman.GetComponent<Stickman>().IncreaseLevel(first.level + 1);
        Destroy(first.gameObject);
        Destroy(second.gameObject);
        first.transform.parent.GetComponent<SpriteRenderer>().enabled = false;
        second.transform.parent.GetComponent<SpriteRenderer>().enabled = false;
        _currentObject = null;
        UIManager.Instance.GenerateButtonActive(true);
        StartCoroutine(nameof(SaveInformation));
    }
    System.Collections.IEnumerator SaveInformation()
    {
        yield return new WaitForSeconds(1f); // 1 den Düşük Bir Değer Null Reference Hatasına Neden Olur - A Value Less Than 1 Causes a Null Reference Error
        gameObject.GetComponent<BoardManager>().SaveAllInformation();
    }
}