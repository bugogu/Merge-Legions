using UnityEngine;

public class RocketBullet : Bullet, IBullet
{
    protected override void EnemyHit(IHitable<float> hitableObject, Transform hitpoint)
    {
        Collider[] _enemies = Physics.OverlapSphere(transform.position, 5);
        if (bulletDatas.hitVFX != null)
        {
            GameObject spawned = Instantiate(bulletDatas.hitVFX, hitpoint);
            spawned.transform.parent = null;
            spawned.transform.localScale = Vector3.one;
        }

        foreach (Collider enemy in _enemies)
        {
            if (enemy.gameObject.TryGetComponent(out Enemy hitable))
            {
                hitable.DamageTaken(bulletDatas.damageValue);
                if (hitable.MyHealth <= 0)
                    hitable.DestroySelf();
            }
            if (enemy.gameObject.TryGetComponent(out Brick hitable2))
            {
                hitable2.DamageTaken(bulletDatas.damageValue);
                if (hitable2.MyHealth <= 0)
                    hitable2.DestroySelf();
            }
        }
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