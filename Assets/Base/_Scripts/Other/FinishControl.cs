using UnityEngine;

public class FinishControl : MonoBehaviour
{
    private ParticleSystem _confetti;
    void Start()
    {
        _confetti = transform.GetChild(0).GetComponent<ParticleSystem>();
    }
    // Finish Nesnesine Temas Eden Board Objesi ise Oyunu Bitirir(Runner Mod İçin Geçerlidir) - 
    // If the Board Object Contacts the Finish Object, the Game Ends(Applies to Runner Mode)
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Board")) return;
        _confetti.Play();
        GameManager.Instance.GameOverAction(1, true);
    }
}