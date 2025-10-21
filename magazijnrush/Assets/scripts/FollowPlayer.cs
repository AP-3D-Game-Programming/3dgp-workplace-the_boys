using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Veranderd van 'player' naar 'target' voor de duidelijkheid
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 10f;
    public LayerMask collisionLayers;

    void FixedUpdate()
    {
        // Gebruik de positie en rotatie van de target (de CameraHolder)
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // Raycast van de target's positie naar de gewenste camera positie
        RaycastHit hit;
        if (Physics.Raycast(target.position, desiredPosition - target.position, out hit, offset.magnitude, collisionLayers))
        {
            // Botsing gedetecteerd → zet camera voor het object, net ertegenaan
            transform.position = hit.point;
        }
        else
        {
            // Geen obstakels → normale camera positie
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }

        // Zorg ervoor dat de camera altijd naar de target kijkt
        transform.LookAt(target);
    }
}