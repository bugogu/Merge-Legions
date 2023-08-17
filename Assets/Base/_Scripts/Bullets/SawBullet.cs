using UnityEngine;

public class SawBullet : Bullet, IBullet
{
    protected override void EnemyHit(IHitable<float> hitableObject, Transform hitpoint)
    {
        if (bulletDatas.hitVFX != null) Instantiate(bulletDatas.hitVFX, hitpoint);

        if (hitpoint.gameObject.TryGetComponent(out IBleedable bleedable))
            bleedable.StartBleeding(bulletDatas.damageValue, bulletDatas.bleedCount);
        else
            hitableObject.DamageTaken(bulletDatas.damageValue);

        if (hitableObject.MyHealth <= 0)
            hitableObject.DestroySelf();
        gameObject.SetActive(false);
    }
    public void MoveToEnemy(Transform bulletPosition)
    {
        MoveToTarget(bulletPosition);
    }
    public void SetTargetInBullet(Transform enemy)
    {
        SetEnemyPosition(enemy);
    }
    private void SetEnemyPosition(Transform enemyTransform) => SetTarget(enemyTransform);
    private void MoveToTarget(Transform bulletLocation) => BulletMovement(bulletLocation);
}