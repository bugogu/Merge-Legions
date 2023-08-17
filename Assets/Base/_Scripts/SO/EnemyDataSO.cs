using UnityEngine;

[CreateAssetMenu(menuName = "Create Data/Enemy")]
public class EnemyDataSO : ScriptableObject
{
    public float health;
    public float damageValue;
    public float runSpeed;
    public float minScaleMultiplier;
    public float maxScaleMultiplier;
    public short rewardValue;
    public Material hitMat;
    public GameObject deathVFX;
}