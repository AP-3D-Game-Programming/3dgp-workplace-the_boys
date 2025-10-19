using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 1, -3);

    void LateUpdate()
    {
        // Houd de camera op dezelfde offset, maar meedraaiend met de speler
        transform.position = player.position + player.rotation * offset;

        // Laat de camera naar de speler kijken
        transform.LookAt(player);
    }
}
