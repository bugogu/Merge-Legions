using UnityEngine;

public class RotateY : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    private void FixedUpdate()
    {
        transform.Rotate(0, rotateSpeed * Time.fixedDeltaTime, 0, Space.World);
    }
}