using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothSpeed = 10f; // Hoe snel de camera beweegt
    public LayerMask collisionLayers; // Selecteer muren/objecten

    void LateUpdate()
    {
        // Gewenste positie achter de speler
        Vector3 desiredPosition = player.position + player.rotation * offset;

        // Raycast van speler naar gewenste camera positie
        RaycastHit hit;
        if (Physics.Raycast(player.position, desiredPosition - player.position, out hit, offset.magnitude, collisionLayers))
        {
            // Botsing gedetecteerd → zet camera voor het object, net ertegenaan
            transform.position = hit.point;
        }
        else
        {
            // Geen obstakels → normale camera positie
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }

        transform.LookAt(player);
    }
}
