public interface IBleedable
{
    System.Collections.IEnumerator Bleeding(float bleedDamage, int bleedCount, float bleedingFrequency);
    void StartBleeding(float bleedDamage, int bleedCount, float bleedingFrequency = .2f);
}