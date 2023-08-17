using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    protected BulletDataSO bulletDatas;
    protected Transform target;
    protected WeaponDataSO weaponDatas;
    private int _stickmanParentLevel;
    private void Awake()
    {
        _stickmanParentLevel = transform.parent.parent.parent.GetComponent<Stickman>().level;
        bulletDatas = GameManager.Instance.bulletDatas[_stickmanParentLevel];
        weaponDatas = GameManager.Instance.weaponDatas[_stickmanParentLevel];
    }
    private void OnTriggerEnter(Collider other)
    {
        IHitable<float> hitable = other.GetComponent<IHitable<float>>();
        if (hitable == null) return;
        EnemyHit(hitable, other.transform);
    }
    /// <summary>
    /// Bullet Sınıfından Kalıtılan Bir Nesne Vurulabilir Bir Nesneye Çarptığında, Çarpılan Nesnenin Özelliklerine IHitable Interface i Üzerinden Erişilmesini Sağlar
    /// When an Object Inherited from the Bullet Class Collides with a Hitable Object, Allows the Properties of the Crashed Object to be Accessed via the IHitable Interface
    /// </summary>
    /// <param name="damage"> Temas Edilen Vurulabilir Nesneyi Döndürür - Returns the Hitable Object </param>
    protected abstract void EnemyHit(IHitable<float> hitableObject, Transform hitpoint);
    protected void BulletMovement(Transform bulletTransform)
    {
        if (target == null) return;
        Vector3 targetDirection = target.position - bulletTransform.position;
        bulletTransform.GetComponent<Rigidbody>().AddForce(targetDirection.normalized * weaponDatas.bulletSpeed, ForceMode.Impulse);
    }
    public void SetTarget(Transform enemyPosition)
    {
        target = enemyPosition;
    }
}