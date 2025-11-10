using UnityEngine;

public class playerController : MonoBehaviour
{
    // NIEUW: Een reference naar het draaipunt van de camera
    public Transform cameraHolder;
    private PlayerPickup carryScript;
    public float speed = 5.0f;
    public float sprintSpeed = 5.5f;
    public float mouseSensitivity = 3f;
    public float currentSpeed;

    [Header("Jumping")]
    public float jumpForce = 30.0f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    private bool isGrounded;
    // NIEUW: Variabelen om de verticale kijkhoek te beperken
    public float verticalLookLimit = 80f;
    private float xRotation = 0f; // Houdt de huidige verticale rotatie bij

    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        carryScript = GetComponent<PlayerPickup>();
    }

    void Update() // Input wordt het best verwerkt in Update
    {
        // Muis input ophalen
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // --- Verticale Rotatie (Op en Neer Kijken) ---
        // We trekken mouseY eraf om de rotatie intu�tief te maken (muis omhoog = omhoog kijken)
        xRotation -= mouseY;
        // Beperk de rotatie zodat je niet over de kop kunt kijken
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);

        // Pas de rotatie toe op de CameraHolder
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // --- Horizontale Rotatie (Links en Rechts Draaien) ---
        // Roteer de speler zelf om de Y-as
        transform.Rotate(Vector3.up * mouseX);

        //Jumping
        if (Input.GetKeyDown(KeyCode.Space) && !carryScript.isCarrying && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void FixedUpdate()
    {
        // Input voor beweging ophalen
        float forwardInput = Input.GetAxis("Vertical");   // W / S
        float strafeInput = Input.GetAxis("Horizontal");  // A / D

        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (shiftPressed && !carryScript.isCarrying)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = speed;
        }

        // Beweging richting (vooruit/achteruit + links/rechts)
        Vector3 moveDirection = transform.forward * forwardInput + transform.right * strafeInput;
        moveDirection.Normalize(); // Zorg dat diagonale beweging niet sneller is
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);

        //Anti doublejump
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);


    }
}