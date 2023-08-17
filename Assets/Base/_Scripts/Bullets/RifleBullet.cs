using UnityEngine;

public class RifleBullet : Bullet, IBullet
{
    protected override void EnemyHit(IHitable<float> hitableObject, Transform hitpoint)
    {
        if (bulletDatas.hitVFX != null) Instantiate(bulletDatas.hitVFX, hitpoint);
        hitableObject.DamageTaken(bulletDatas.damageValue);
        if (hitableObject.MyHealth <= 0)
            hitableObject.DestroySelf();
        gameObject.SetActive(false);
    }
    public void SetTargetInBullet(Transform enemy)
    {
        SetEnemyPosition(enemy);
    }
    public void MoveToEnemy(Transform bulletPosition)
    {
        MoveToTarget(bulletPosition);
    }
    private void SetEnemyPosition(Transform enemyTransform) => SetTarget(enemyTransform);
    private void MoveToTarget(Transform bulletLocation) => BulletMovement(bulletLocation);
}