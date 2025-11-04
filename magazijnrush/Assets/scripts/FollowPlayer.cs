using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Veranderd van 'player' naar 'target' voor de duidelijkheid
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 10f;
    public LayerMask collisionLayers;

    void FixedUpdate() // Of LateUpdate, wat je nu ook gebruikt
    {
        // Gebruik de positie en rotatie van de target
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // --- DIT IS DE BELANGRIJKE DEBUG-STAP ---
        Vector3 direction = desiredPosition - target.position;
        float distance = offset.magnitude;
        Debug.DrawRay(target.position, direction, Color.red); // Teken een rode lijn waar de raycast gaat
                                                              // ------------------------------------------

        // Raycast van de target's positie naar de gewenste camera positie
        RaycastHit hit;
        if (Physics.Raycast(target.position, direction, out hit, distance, collisionLayers))
        {
            // Als we iets raken, teken dan een groene lijn naar het raakpunt
            Debug.DrawLine(target.position, hit.point, Color.green);

            transform.position = hit.point;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }

        // Zorg ervoor dat de camera altijd naar de target kijkt
        transform.LookAt(target);
    }
}