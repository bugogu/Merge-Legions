using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class PickUP : MonoBehaviour
{
    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("BoardShield")) return;
        Interact();
        Destroy(gameObject);
    }
    public abstract void Interact();
    private void OnEnable()
    {
        transform.parent = null;
    }
}