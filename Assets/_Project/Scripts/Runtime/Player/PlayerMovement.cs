using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement3D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 input;
    private float startY;

    // Read inputs in Update (frame-rate dependent)
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startY = transform.position.y;

        // Prevent tipping over; we move on XZ only
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false; // We keep Y constant for a top-down feel
    }

    private void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        input.z = Input.GetAxisRaw("Vertical");   // W/S or Up/Down
        input = input.normalized;                 // Consistent speed diagonally
    }

    // Apply physics movement in FixedUpdate (physics-step dependent)
    private void FixedUpdate()
    {
        Vector3 velocity = input * moveSpeed;
        Vector3 targetPos = rb.position + velocity * Time.fixedDeltaTime;

        
        rb.MovePosition(targetPos);
    }
}
