using UnityEngine;

public class Border : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bullet"))
            other.gameObject.SetActive(false);
    }
}
