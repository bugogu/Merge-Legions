using UnityEngine;

[CreateAssetMenu(menuName = "Create Data/Bullet")]
public class BulletDataSO : ScriptableObject
{
    public TrailRenderer bulletTrail;
    public float damageValue;
    public GameObject hitVFX;
    public int bleedCount;
}