using UnityEngine;

public class playerController : MonoBehaviour
{
    public float speed = 5.0f;          // loopsnelheid
    public float mouseSensitivity = 3f; // muis draaissnelheid

    private float forwardInput;
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
        forwardInput = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");

        // Vooruit/achteruit via MovePosition (respecteert physics)
        Vector3 moveDirection = transform.forward * forwardInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Draaien via MoveRotation
        Quaternion turn = Quaternion.Euler(0f, mouseX * mouseSensitivity, 0f);
        rb.MoveRotation(rb.rotation * turn);
    }
}