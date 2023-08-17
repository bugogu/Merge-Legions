using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IHitable<float>, IBleedable
{
    #region Data Variables
    public EnemyDataSO enemyData;
    private float _health;
    private float _damageValue;
    private float _runSpeed;
    private int _rewardValue;
    private GameObject _deathVFX;
    #endregion
    [SerializeField] private float extraHealth;
    [SerializeField] private EnemyMode enemyMode;
    [SerializeField] private float bossScale;
    [SerializeField] private float bossWalkSpeed;
    [SerializeField] private int bossRewardCoin;
    [SerializeField] private int bossRewardGem;
    public enum EnemyMode
    {
        Normal,
        Boss
    }
    private UnityEngine.UI.Image _healtBar;
    private Animator _anim;
    private float _dieAnimationLength;
    private float _attackAnimationLength;
    private RuntimeAnimatorController _enemyAnimator;
    private AnimationClip _dieAnimation;
    private AnimationClip _attackAnimation;
    private AnimationClip _winAnimation;
    private float _maxHealth;
    private Coroutine _hitRoutine;
    private SkinnedMeshRenderer _smr;
    private Material _orginalMat;
    private Material _hitEffect;
    private bool _isDeath;
    public float MyHealth { get => _health; }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("BoardShield")) return;
        _isDeath = true;
        GetComponent<Collider>().enabled = false;
        _anim.SetTrigger("Attack");
        if (enemyMode == EnemyMode.Boss)
            BoardManager.TakeDamage(BoardManager._health);
        else
            BoardManager.TakeDamage(_damageValue);
        if (_deathVFX != null) Instantiate(_deathVFX, transform);
        Destroy(gameObject, _attackAnimationLength);
    }
    private void FixedUpdate()
    {
        if (!GameManager.Instance.isOnStarted) return;
        if (GameManager.gameModeOption == GameManager.GameMode.Runner) return;
        if (GameManager.Instance.gameOver) return;
        if (_isDeath) return;
        transform.position += transform.forward * _runSpeed * Time.fixedDeltaTime;
    }
    public void DamageTaken(float damageTakenValue)
    {
        HitEffect();
        damageTakenValue = GameManager.gameModeOption == GameManager.GameMode.Defence ? damageTakenValue : (damageTakenValue + (damageTakenValue * .20f));
        damageTakenValue = (damageTakenValue + (damageTakenValue * GameManager.boostValue));
        _healtBar.fillAmount -= (damageTakenValue / _maxHealth);
        _health -= damageTakenValue;
    }
    public void DestroySelf()
    {
        _isDeath = true;
        GetComponent<Collider>().enabled = false;
        _anim.SetTrigger("Die");
        _smr.material = _orginalMat;

        #region DropItem

        if (Random.value < GameManager.Instance.dropGemPercent)
        {
            GameObject dropped = Instantiate(GameManager.Instance.gemPrefab, transform, true);
            dropped.transform.position = transform.position;
        }

        if (Random.value < GameManager.Instance.dropGiftPercent)
        {
            GameObject dropped = Instantiate(GameManager.Instance.giftPrefab, transform, true);
            dropped.transform.position = transform.position;
        }

        #endregion

        #region Reward

        if (enemyMode == EnemyMode.Normal)
        {
            if (GameManager.gameModeOption == GameManager.GameMode.Defence)
                GameManager.coin += ((int)(_rewardValue + GameManager.gameLevel * (GameManager.coinMultiplier)));
            GameManager.Instance.collectedCoin += (_rewardValue + GameManager.gameLevel);
            GameManager.killedEnemys += 1;
        }
        else
        {
            GameManager.coin += (bossRewardCoin + (int)(bossRewardCoin * GameManager.coinMultiplier));
            GameManager.gem += bossRewardGem;
        }

        #endregion

        if ((GameManager.coin >= GameManager.stickmanPrice && GameManager.gameModeOption == GameManager.GameMode.Defence))
        {
            GameObject[] stickmans = GameObject.FindGameObjectsWithTag("Stickman");
            if ((stickmans.Length < 16))
            {
                UIManager.Instance.generateStickmanButton.gameObject.SetActive(true);
                UIManager.Instance.generateStickmanButton.interactable = true;
            }
        }

        UIManager.Instance.SetCollectedRewardsText(bossRewardGem, bossRewardCoin, enemyMode == EnemyMode.Boss);
        if (_deathVFX != null) Instantiate(_deathVFX, transform);
        Destroy(gameObject, _dieAnimationLength);
        Destroy(this);
    }
    private void OnEnable()
    {
        Initial();
        var scaleValue = Random.Range(enemyData.minScaleMultiplier, enemyData.maxScaleMultiplier);
        transform.localScale = enemyMode == EnemyMode.Normal ? transform.localScale * scaleValue : transform.localScale * bossScale;

        _health += ((GameManager.finishCount * GameManager.Instance.levels.Length) + GameManager.gameLevel + extraHealth);
        _damageValue += GameManager.gameLevel;
        _rewardValue += GameManager.gameLevel;

        if (enemyMode == EnemyMode.Normal)
        {
            _health *= scaleValue;
            _damageValue *= scaleValue;
        }
        _runSpeed = enemyMode == EnemyMode.Normal ? _runSpeed - (scaleValue * 4) : _runSpeed = bossWalkSpeed;


        _maxHealth = _health;
    }
    private System.Collections.IEnumerator HitRoutine()
    {
        _smr.material = _hitEffect;
        yield return new WaitForSeconds(0.005f);
        _smr.material = _orginalMat;
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
    public void SetRunAnim(bool status)
    {
        if (GameManager.gameModeOption != GameManager.GameMode.Defence) return;
        if (enemyMode == EnemyMode.Normal)
            _anim.SetBool("Run Forward", status);
        else
            _anim.SetBool("Walk Forward", status);
    }
    public void SetWinAnim()
    {
        foreach (AnimationClip clip in _enemyAnimator.animationClips)
        {
            if (clip.name == "Win")
            {
                _winAnimation = clip;
                break;
            }
        }
        if (_winAnimation != null)
            _anim.SetTrigger("Win");
        else
            return;
    }
    private void Initial()
    {
        _anim = GetComponent<Animator>();
        _enemyAnimator = _anim.runtimeAnimatorController;
        foreach (AnimationClip clip in _enemyAnimator.animationClips)
        {
            if (clip.name == "Die")
                _dieAnimation = clip;
            if (clip.name == "Attack")
                _attackAnimation = clip;
        }
        _dieAnimationLength = _dieAnimation.length;
        _attackAnimationLength = _attackAnimation.length;
        _healtBar = transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>();
        _smr = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        _orginalMat = _smr.material;
        _health = enemyData.health;
        _damageValue = enemyData.damageValue;
        _runSpeed = enemyData.runSpeed;
        _rewardValue = enemyData.rewardValue;
        _hitEffect = enemyData.hitMat;
        _deathVFX = enemyData.deathVFX;
    }
    public IEnumerator Bleeding(float bleedDamage, int bleedCount, float bleedingFrequency)
    {
        for (int i = 0; i < bleedCount; i++)
        {
            yield return new WaitForSeconds(bleedingFrequency);
            _healtBar.fillAmount -= (bleedDamage / _maxHealth);
            _health -= bleedDamage;
        }
    }
    public void StartBleeding(float bleedDamage, int bleedCount, float bleedingFrequency = .2f) => StartCoroutine(Bleeding(bleedDamage, bleedCount, bleedingFrequency));
}