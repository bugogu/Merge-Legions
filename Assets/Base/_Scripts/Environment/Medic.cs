public class Medic : PickUP
{
    [UnityEngine.SerializeField] private float extraHealth;
    public override void Interact()
    {
        BoardManager.TakeHealth(extraHealth + (GameManager.gameLevel + (GameManager.finishCount * GameManager.Instance.levels.Length)));
    }
}