using UnityEngine;

public class playerController : MonoBehaviour
{
    public float speed = 5.0f;          // loopsnelheid
    public float mouseSensitivity = 3f; // muis draaissnelheid

    private float forwardInput;
    private float strafeInput;
    private float mouseX;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Zorg dat speler niet omvalt
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        // Input ophalen
        forwardInput = Input.GetAxis("Vertical");   // W / S
        strafeInput = Input.GetAxis("Horizontal");  // A / D
        mouseX = Input.GetAxis("Mouse X");          // Muis draaien

        // Beweging richting (vooruit/achteruit + links/rechts)
        Vector3 moveDirection = (transform.forward * forwardInput + transform.right * strafeInput).normalized;
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);

        // Draaien op de Y-as met de muis
        Quaternion turn = Quaternion.Euler(0f, mouseX * mouseSensitivity, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }
}