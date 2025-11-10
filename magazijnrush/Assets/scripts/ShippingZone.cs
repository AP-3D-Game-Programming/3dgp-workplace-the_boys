using UnityEngine;

public class ShippingZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.Instance.gameActive) return;

        // Alleen SpawnedPickup-objecten tellen
        if (other.CompareTag("SpawnedPickup"))
        {
            string itemName = other.name.Replace("(Clone)", "").Trim();

            // Check of dit item deel is van de order
            if (GameManager.Instance.TryDeliverItem(itemName))
            {
                GameManager.Instance.AddScore(2); // Belonen
                Destroy(other.gameObject);
            }
            else
            {
                GameManager.Instance.AddScore(-1); // Straffen
                Destroy(other.gameObject);
            }
        }
    }
}
