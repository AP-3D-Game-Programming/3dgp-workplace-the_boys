using UnityEngine;

public class ShippingZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Alleen crates die door de GameManager zijn gespawned
        if (other.CompareTag("SpawnedPickup"))
        {
            // Score toevoegen via de GameManager
            GameManager.Instance.AddScore(1);

            // Vernietig de crate
            Destroy(other.gameObject);
        }

        // Alles met andere tags blijft onaangetast
    }
}