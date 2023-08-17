using UnityEngine;

public class Easteregg : MonoBehaviour
{
    private void Start()
    {
        if (Random.value < 0.001f)
        {
            GetComponent<TMPro.TMP_Text>().enabled = true;
        }
    }
}