using UnityEngine;
using DG.Tweening;

public class DamagePopupText : MonoBehaviour
{
    private static GameObject popupTextObject;
    private static Transform canvasTransform;
    private void Start()
    {
        popupTextObject = GameManager.Instance.popupTextPrefab;
        canvasTransform = this.transform;
    }
    public static void GeneratePopup(float popupTextValue, bool markNegative, Color textColor)
    {
        popupTextValue = Mathf.Round(popupTextValue);
        var mark = markNegative ? "-" : "+";
        var generatedPopup = Instantiate(popupTextObject, canvasTransform);
        generatedPopup.GetComponent<TMPro.TMP_Text>().text = mark + popupTextValue.ToString();
        generatedPopup.GetComponent<TMPro.TMP_Text>().color = textColor;
        generatedPopup.transform.DOLocalMoveY(3, .5f);
        Destroy(generatedPopup, .55f);
    }
}
