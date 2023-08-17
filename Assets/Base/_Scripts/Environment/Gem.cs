public class Gem : PickUP
{
    [UnityEngine.SerializeField] private int extraGem;
    [UnityEngine.SerializeField] private UnityEngine.Color popupColor;
    public override void Interact()
    {
        My.PlaySound(GameManager.Instance.gemCollect);
        DamagePopupText.GeneratePopup((float)extraGem, false, popupColor);
        GameManager.gem += extraGem;
    }
}