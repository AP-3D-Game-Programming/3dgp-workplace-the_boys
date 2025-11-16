using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class playerController : MonoBehaviour
{
    // Referentie naar de camera die met de speler meedraait
    public Transform cameraHolder;
    private PlayerPickup carryScript; // Referentie naar het script dat items kan oppakken

    [Header("Movement")]
    public float speed = 5f;          // Normale loopsnelheid
    public float sprintSpeed = 5.5f;  // Sprint snelheid
    private float currentSpeed;       // Huidige snelheid, afhankelijk van sprinten

    [Header("Jump")]
    public float jumpForce = 7f;      // Kracht waarmee speler omhoog springt
    public LayerMask groundLayer;     // Welke lagen tellen als "grond"
    public float groundedSkin = 0.05f;// Kleine marge voor ground check

    [Header("Look")]
    public float mouseSensitivity = 3f;   // Snelheid van muis/kijk beweging
    public float verticalLookLimit = 80f; // Maximale verticale kijkhoek
    private float xRotation = 0f;         // Houdt huidige verticale rotatie bij

    private Rigidbody rb;          // Rigidbody van de speler
    private CapsuleCollider capsule;// Capsule collider van de speler
    private bool isGrounded;       // Is speler op de grond?

    private Animator anim;         // Animator component

    void Start()
    {
        // Verkrijg benodigde componenten
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;       // Voorkom dat physics de rotatie breekt
        capsule = GetComponent<CapsuleCollider>();
        carryScript = GetComponent<PlayerPickup>();
        anim = GetComponent<Animator>();

        // Initialiseer cursor in locked state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // *** NIEUWE PAUZE-CHECK ***
        if (PauseMenu.GameIsPaused)
        {
            // Stop de Update-functie onmiddellijk
            return;
        }

        // OUDE CURSOR-LOGICA VERWIJDERD, nu geregeld door PauseMenu.cs

        // Mouse look
        xRotation = Mathf.Clamp(xRotation - Input.GetAxis("Mouse Y") * mouseSensitivity, -verticalLookLimit, verticalLookLimit);
        if (cameraHolder) cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Vector3 vel = rb.linearVelocity; vel.y = 0f; rb.linearVelocity = vel;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // animation
            if (anim != null)
            {
                anim.SetTrigger("IsJumping");
            }
        }
        HandleLook();   // Verwerk camera en muis beweging
        HandleJump();   // Verwerk springen
        HandleCursor(); // Verwerk cursor lock/unlock
    }

    void FixedUpdate()
    {
        // *** NIEUWE PAUZE-CHECK ***
        if (PauseMenu.GameIsPaused)
        {
            // Zorg ervoor dat de rigidbody volledig stopt, zelfs al is Time.timeScale = 0
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            // Stop de FixedUpdate-functie onmiddellijk
            return;
        }

        // Movement
        float forward = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");
        bool isMoving = (forward != 0 || strafe != 0);
        HandleMovement();      // Verwerk speler beweging
        UpdateGroundedState(); // Check of speler op de grond staat
        UpdateAnimator();      // Update animator parameters
    }

    // --- CAMERA LOOK ---
    void HandleLook()
    {
        // Verticale rotatie (op/af)
        xRotation = Mathf.Clamp(xRotation - Input.GetAxis("Mouse Y") * mouseSensitivity,
                                -verticalLookLimit, verticalLookLimit);
        if (cameraHolder) cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontale rotatie (links/rechts)
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity);
    }

    // --- JUMP ---
    void HandleJump()
    {
        // Alleen springen als speler op de grond staat
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Reset verticale snelheid zodat jumps consistent zijn
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;

            // Voeg kracht toe om omhoog te springen
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // --- CURSOR ---
    void HandleCursor()
    {
        // ESC toont cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Linkermuisklik vergrendelt cursor weer
        if (Input.GetMouseButtonDown(0) && Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // --- MOVEMENT ---
    void HandleMovement()
    {
        // Input ophalen
        float forward = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");

        // Check of speler beweegt
        bool isMoving = Mathf.Abs(forward) + Mathf.Abs(strafe) > 0.01f;

        // Check of speler sprint
        bool isSprinting = isMoving && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

        // Update animator parameters
        if (anim != null)
        {
            anim.SetBool("IsWalking", isMoving && !isSprinting);
            anim.SetBool("IsSprinting", isSprinting);
        }

        // Bereken huidige snelheid
        currentSpeed = (!carryScript.isCarrying && isSprinting) ? sprintSpeed : speed;

        // Bereken en voer beweging uit
        Vector3 move = (transform.forward * forward + transform.right * strafe).normalized;
        rb.MovePosition(rb.position + move * currentSpeed * Time.fixedDeltaTime);
    }

    // --- GROUND CHECK ---
    void UpdateGroundedState()
    {
        if (capsule == null) { isGrounded = false; return; }

        // Bereken startpunt en afstand van de raycast
        Vector3 origin = transform.position + Vector3.up * groundedSkin; // iets omhoog zodat capsule niet direct raakt
        float distance = (capsule.height / 2f) + groundedSkin;

        // Raycast naar beneden om te checken of speler grounded is
        isGrounded = Physics.Raycast(origin, Vector3.down, distance, groundLayer);

        // Optionele debug lijn: groen = grounded, rood = niet grounded
        Debug.DrawRay(origin, Vector3.down * distance, isGrounded ? Color.green : Color.red);
    }

    // --- ANIMATOR UPDATE ---
    void UpdateAnimator()
    {
        if (anim != null)
        {
            // Update animator bools
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetBool("IsJumping", !isGrounded); // spring animatie actief zolang speler in de lucht is
        }
    }

    // Getter om van buitenaf te checken of speler op de grond staat
    public bool IsGrounded() => isGrounded;
}