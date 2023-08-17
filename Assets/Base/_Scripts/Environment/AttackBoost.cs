public class AttackBoost : PickUP
{
    [UnityEngine.SerializeField] private float boostCountdown;
    [UnityEngine.Range(0, 1)]
    [UnityEngine.SerializeField] private float boostPer;
    public override void Interact()
    {
        GameManager.Instance.TakeBoost(boostCountdown, boostPer);
    }
}