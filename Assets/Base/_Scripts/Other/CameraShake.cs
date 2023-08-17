using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static void ShakeToCamera() => Camera.main.DOShakePosition(.2f, 1, 5, 10);
}
