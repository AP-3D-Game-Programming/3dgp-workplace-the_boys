using UnityEngine;

public class pickup : MonoBehaviour
{
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Pickupable"))
        {
            Debug.Log("✅ Je raakt een oppakbaar object: " + hit.collider.gameObject.name);
        }
    }
}
