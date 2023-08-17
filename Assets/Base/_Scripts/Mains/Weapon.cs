using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    #region Varibales
    #region From Data
    [HideInInspector] public WeaponDataSO weaponData;
    private GameObject _bullet;
    private int _burstCount;
    private float _burstFrequency;
    private float _attackRange;
    private float _attackSpeed;
    private float _bulletSpeed;
    private byte _bulletPoolLength;
    private GameObject _fireEffect;
    private AudioClip _fireSound;
    #endregion
    private Queue<GameObject> _pooledObjects;
    private Collider[] _enemies;
    private Enemy _currentHitableEnemy = null;
    private Brick _currentHitableBrick = null;
    #endregion
    private void Awake()
    {
        weaponData = GameManager.Instance.weaponDatas[transform.parent.GetComponent<Stickman>().level];
        _pooledObjects = new Queue<GameObject>();
        Assignment();
        GeneratePool();
    }
    private void Assignment()
    {
        _bullet = weaponData.bulletPrefab;
        _burstCount = weaponData.burstCount;
        _burstFrequency = weaponData.burstFrequency;
        _bulletPoolLength = weaponData.poolSize;
        _attackRange = weaponData.attackRadius;
        _attackSpeed = weaponData.fireRate;
        _bulletSpeed = weaponData.bulletSpeed;
        _fireEffect = weaponData.fireVFX;
        _fireSound = weaponData.fireSFX;
    }
    private void GeneratePool()
    {
        for (int i = 0; i < _bulletPoolLength; i++)
        {
            GameObject bullet = Instantiate(this._bullet, transform.GetChild(1).transform);
            bullet.SetActive(false);
            _pooledObjects.Enqueue(bullet);
        }
    }
    private GameObject GetPooledObject()
    {
        GameObject obj = _pooledObjects.Dequeue();
        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        obj.transform.localPosition = Vector3.zero;
        if (_bullet.name == "1-5-9-10")
            obj.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        if (_fireSound != null) My.PlaySound(_fireSound);
        if (transform.GetChild(0).childCount < 1) { if (_fireEffect != null) Instantiate(_fireEffect, transform.GetChild(0).transform); }
        else { transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>()?.Play(); }
        obj.SetActive(true);
        _pooledObjects.Enqueue(obj);
        return obj;
    }
    private void ScanArea()
    {
        if (GameManager.Instance.gameOver) return;
        transform.parent.rotation = Quaternion.Euler(transform.parent.GetComponent<Stickman>().defaultRotation);
        float distance = weaponData.attackRadius;
        _enemies = Physics.OverlapSphere(transform.position, _attackRange);
        foreach (Collider enemy in _enemies)
        {
            if (enemy.gameObject.TryGetComponent(out Enemy hitable))
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist <= distance)
                {
                    _currentHitableEnemy = hitable;
                    distance = dist;
                }
            }
            else if (enemy.gameObject.TryGetComponent(out Brick hitable2))
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist <= distance)
                {
                    _currentHitableBrick = hitable2;
                    distance = dist;
                }
            }
        }
        if (_currentHitableBrick == null && _currentHitableEnemy == null)
        {
            for (int i = 0; i < transform.GetChild(1).childCount; i++)
            {
                transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
            }
        }
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        if (GameManager.gameModeOption == GameManager.GameMode.Defence)
            if (enemys.Length == 0)
                GameManager.Instance.GameOverAction(1, true);
        if (_currentHitableBrick != null)
        {
            if (weaponData.shootType == WeaponDataSO.ShootType.Single)
                FireToBrick();
            else
                BurstToBrick();
        }
        else if (_currentHitableEnemy != null)
        {
            if (weaponData.shootType == WeaponDataSO.ShootType.Single)
                FireToEnemy();
            else
                BurstToEnemy();
        }
    }
    public void FireControl()
    {
        InvokeRepeating(nameof(ScanArea), 0, (1 / _attackSpeed));
    }
    // Alttaki 4 fonksiyon parametreler ile 2 ye indirilip temiz hale getirilicek.
    private void FireToBrick()
    {
        try
        {
            Vector3 dir = _currentHitableBrick.gameObject.transform.position - transform.parent.position;
            dir.y = 0;
            transform.parent.rotation = Quaternion.LookRotation(-dir);
            GameObject bullet = GetPooledObject();
            bullet.GetComponent<IBullet>().SetTargetInBullet(_currentHitableBrick.gameObject.transform);
            bullet.GetComponent<IBullet>().MoveToEnemy(bullet.transform);
        }
        catch (MissingReferenceException)
        {
            Debug.Log("Not Important Error");
            return;
        }
    }
    private void FireToEnemy()
    {
        try
        {
            Vector3 dir = _currentHitableEnemy.gameObject.transform.position - transform.parent.position;
            dir.y = 0;
            transform.parent.rotation = Quaternion.LookRotation(-dir);
            GameObject bullet = GetPooledObject();
            bullet.GetComponent<IBullet>().SetTargetInBullet(_currentHitableEnemy.gameObject.transform);
            bullet.GetComponent<IBullet>().MoveToEnemy(bullet.transform);
        }
        catch (MissingReferenceException)
        {
            Debug.Log("Not Important Error");
            return;
        }
    }
    private async void BurstToBrick()
    {
        for (int i = 0; i < _burstCount; i++)
        {
            await System.Threading.Tasks.Task.Delay((int)(1000 * _burstFrequency));
            FireToBrick();
        }
    }
    private async void BurstToEnemy()
    {
        for (int i = 0; i < _burstCount; i++)
        {
            await System.Threading.Tasks.Task.Delay((int)(1000 * _burstFrequency));
            FireToEnemy();
        }
    }
}