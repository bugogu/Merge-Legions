using UnityEngine;

public class RoadMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    // Play Butonuna Basıldığı Zaman Mod Runner ise Oyun Bitene Kadar Yolu Hareket Ettirir - 
    // When Play Button is Pressed If Mod Runner Moves Path Until Game Ends
    private void FixedUpdate()
    {
        if (!GameManager.Instance.isOnStarted) return;
        if (!(GameManager.gameModeOption == GameManager.GameMode.Runner)) return;
        if (GameManager.Instance.gameOver) return;
        transform.position += Vector3.back * Time.fixedDeltaTime * speed;
    }
}