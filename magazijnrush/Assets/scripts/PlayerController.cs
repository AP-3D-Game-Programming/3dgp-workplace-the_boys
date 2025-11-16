using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class playerController : MonoBehaviour
{
    public Transform cameraHolder;
    private PlayerPickup carryScript;

    [Header("Movement")]
    public float speed = 5f;
    public float sprintSpeed = 5.5f;
    private float currentSpeed;

    [Header("Jump")]
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    public float groundedSkin = 0.05f;

    [Header("Look")]
    public float mouseSensitivity = 3f;
    public float verticalLookLimit = 80f;
    private float xRotation = 0f;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private bool isGrounded;

    // animator
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        capsule = GetComponent<CapsuleCollider>();
        carryScript = GetComponent<PlayerPickup>();

        // animator
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator component not found on the player object! Did you forget to add it?");
        }

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

        // BEREKEN OF ER BEWEGING IS
        float inputMagnitude = Mathf.Abs(forward) + Mathf.Abs(strafe); // Hoeveel er bewogen wordt (0 tot 2)
        isMoving = inputMagnitude > 0.01f; // Is de speler aan het bewegen
        bool isSprinting = isMoving && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)); // speler aan het sprinten?

        // animation
        if (anim != null)
        {
            // 1. Walk/Idle: Alleen IsWalking is niet voldoende, IsSprinting moet IsWalking overrulen
            anim.SetBool("IsWalking", isMoving && !isSprinting);

            // 2. Sprint: 
            anim.SetBool("IsSprinting", isSprinting);

            // Optioneel: Stel de snelheid in om de voetstappen te timen met de daadwerkelijke snelheid
            // anim.SetFloat("Speed", currentSpeed); 
        }

        currentSpeed = (!carryScript.isCarrying && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) ? sprintSpeed : speed;
        Vector3 move = (transform.forward * forward + transform.right * strafe).normalized;
        rb.MovePosition(rb.position + move * currentSpeed * Time.fixedDeltaTime);

        // Ground check
        Vector3 center = capsule.bounds.center;
        float radius = capsule.radius * Mathf.Max(transform.localScale.x, transform.localScale.z);
        float halfHeight = Mathf.Max(0f, capsule.height * transform.localScale.y / 2f - radius);
        Vector3 top = center + Vector3.up * halfHeight;
        Vector3 bottom = center - Vector3.up * halfHeight;
        isGrounded = Physics.CheckCapsule(top, bottom + Vector3.down * groundedSkin, radius - 0.01f, groundLayer, QueryTriggerInteraction.Ignore);
        if (anim != null)
        {
            // Vertel de Animator of de speler op de grond staat
            anim.SetBool("IsGrounded", isGrounded);
        }
    }

    public bool IsGrounded() => isGrounded;
}