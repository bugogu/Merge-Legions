using UnityEngine;

[CreateAssetMenu(menuName = "Create Data/Weapon")]
public class WeaponDataSO : ScriptableObject
{
    public ShootType shootType;
    public enum ShootType
    {
        Single,
        Burst
    };
    public GameObject bulletPrefab;
    public int burstCount;
    public float burstFrequency;
    public float attackRadius;
    public float fireRate;
    public float bulletSpeed;
    public byte poolSize;
    public GameObject fireVFX;
    public AudioClip fireSFX;

}