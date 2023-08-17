using UnityEngine;

[CreateAssetMenu(menuName = "Create Data/Stickman")]
public class StickmanDataSO : ScriptableObject
{
    [Tooltip("Stickman Oluştuğunda Tek Silahlı yada Çift Silahlı Olmasını Sağlar - Allows Stickman to Be Single-Armed or Dual-Armed When Occurs")]
    public AttackType attackType;
    public enum AttackType
    {
        one_handed,
        two_handed
    };
    [Space]
    public GameObject weaponPrefab;
    public Material stickmanMaterial;
    // Stickmanin Levele Oranla Boyutunu Artırmak İçin Kullanılabilir - 
    //Can be used to increase the size of the stickman in proportion to the level
    public float scaleMultiplier;
}