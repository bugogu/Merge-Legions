using UnityEngine;

public class SniperBullet : Bullet, IBullet
{
    int triggerCount = 0;
    protected override void EnemyHit(IHitable<float> hitableObject, Transform hitpoint)
    {
        if (bulletDatas.hitVFX != null) Instantiate(bulletDatas.hitVFX, hitpoint);
        hitableObject.DamageTaken(bulletDatas.damageValue);
        if (hitableObject.MyHealth <= 0)
            hitableObject.DestroySelf();
        triggerCount++;
        if (triggerCount % 5 == 0)
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