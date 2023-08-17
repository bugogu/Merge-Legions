using UnityEngine;

public class MoveToBoard : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private void FixedUpdate()
    {
        if (!GameManager.Instance.isOnStarted) return;
        if (GameManager.Instance.gameOver) return;
        transform.position += Vector3.back * Time.fixedDeltaTime * moveSpeed;
    }
}
