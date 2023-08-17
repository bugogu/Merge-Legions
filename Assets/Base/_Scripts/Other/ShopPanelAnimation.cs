using UnityEngine;
using DG.Tweening;

public class ShopPanelAnimation : MonoBehaviour
{
    [SerializeField] private float animationDuration;
    private Vector3 _initialPos;
    private RectTransform _panelRect;
    private void Awake()
    {
        _panelRect = GetComponent<RectTransform>();
        _initialPos = _panelRect.localPosition;
    }
    private void OnEnable()
    {
        _panelRect.DOLocalMoveY(8, animationDuration);
    }
    private void OnDisable()
    {
        _panelRect.localPosition = _initialPos;
    }
}
