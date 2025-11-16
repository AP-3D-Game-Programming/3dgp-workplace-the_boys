using UnityEngine;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        capsule = GetComponent<CapsuleCollider>();
        carryScript = GetComponent<PlayerPickup>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Mouse look
        xRotation = Mathf.Clamp(xRotation - Input.GetAxis("Mouse Y") * mouseSensitivity, -verticalLookLimit, verticalLookLimit);
        if (cameraHolder) cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Vector3 vel = rb.linearVelocity; vel.y = 0f; rb.linearVelocity = vel;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Cursor
        if (Input.GetKeyDown(KeyCode.Escape)) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        if (Input.GetMouseButtonDown(0) && Cursor.visible) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
    }

    void FixedUpdate()
    {
        // Movement
        float forward = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");
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
    }

    public bool IsGrounded() => isGrounded;
}