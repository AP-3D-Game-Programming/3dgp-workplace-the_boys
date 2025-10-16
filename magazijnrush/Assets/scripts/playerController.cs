using UnityEngine;

public class playerController : MonoBehaviour
{
    public float speed = 5.0f;          // loopsnelheid
    public float mouseSensitivity = 3f; // hoe snel de muis draait

    private float forwardInput;
    private float mouseX;

    void Update()
    {
        // Beweging vooruit/achteruit
        forwardInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);

        // Muisinput voor draaien
        mouseX = Input.GetAxis("Mouse X");

        // Draai speler rond Y-as met muis
        transform.Rotate(Vector3.up * mouseX * mouseSensitivity);
    }
}
