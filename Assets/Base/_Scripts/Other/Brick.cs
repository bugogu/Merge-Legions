using UnityEngine;

public class Brick : MonoBehaviour, IHitable<float>
{
    [SerializeField] private float healt;
    [SerializeField] private Material _hitEffect;
    [SerializeField] private Color brickColor;
    private TMPro.TMP_Text _healtText;
    private Transform _breakableWall;
    private Coroutine _hitRoutine;
    private MeshRenderer _mr;
    private Material _orginalMat;
    public float MyHealth => healt;
    private void OnEnable()
    {
        Initial();
        SetColorToBreakableWall();
        healt += GameManager.finishCount * GameManager.gameLevel;
    }
    private void Initial()
    {
        _breakableWall = transform.GetChild(1);
        _healtText = transform.GetChild(0).GetComponent<TMPro.TMP_Text>();
        _mr = GetComponent<MeshRenderer>();
        _healtText.text = healt.ToString();
        _mr.material.color = brickColor;
        _orginalMat = _mr.material;
    }
    private void SetColorToBreakableWall()
    {
        for (int i = 0; i < _breakableWall.childCount; i++)
        {
            _breakableWall.GetChild(i).GetComponent<MeshRenderer>().material.color = brickColor;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("BoardShield")) return;
        My.DoVibrate();
        BoardManager.TakeDamage(healt);
        DestroySelf();
    }
    public void DamageTaken(float damageValue)
    {
        damageValue = GameManager.gameModeOption == GameManager.GameMode.Defence ? damageValue : (damageValue + (damageValue * .20f));
        damageValue = (damageValue + (damageValue * GameManager.boostValue));
        damageValue = Mathf.Round(damageValue);
        healt -= damageValue;
        _healtText.text = healt.ToString();
        HitEffect();
    }
    public void DestroySelf()
    {
        _mr.enabled = false;
        GetComponent<Collider>().enabled = false;
        My.PlaySound("Break");
        transform.GetChild(2).GetComponent<ParticleSystem>().Play();
        transform.GetChild(0).gameObject.SetActive(false);
        for (int i = 0; i < _breakableWall.childCount; i++)
        {
            var randomness = 50;
            var scatterX = Random.Range(-randomness, randomness);
            var scatterY = Random.Range(0, randomness);
            var scatterZ = Random.Range(0, randomness);
            Vector3 scatterVector = new Vector3(scatterX, scatterY, scatterZ);
            _breakableWall.GetChild(i).GetComponent<Rigidbody>().AddForce(scatterVector, ForceMode.Impulse);
            _breakableWall.GetChild(i).transform.rotation = Quaternion.Euler(scatterVector);
        }
        Destroy(gameObject, 1.5f);
        Destroy(this);
    }
    private System.Collections.IEnumerator HitRoutine()
    {
        _mr.material = _hitEffect;
        yield return new WaitForSeconds(0.005f);
        _mr.material = _orginalMat;
        _hitRoutine = null;
    }
    public void HitEffect()
    {
        if (_hitRoutine != null)
        {
            StopCoroutine(_hitRoutine);
        }
        _hitRoutine = StartCoroutine(HitRoutine());
    }
}