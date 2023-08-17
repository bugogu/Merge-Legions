public class Gift : PickUP
{
    [UnityEngine.SerializeField] private int extraGem;
    [UnityEngine.SerializeField] private UnityEngine.Color popupColor;
    public override void Interact()
    {
        DamagePopupText.GeneratePopup((float)extraGem, false, popupColor);
        GameManager.gem += extraGem;
    }
}
