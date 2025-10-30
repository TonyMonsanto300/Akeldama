using UnityEngine;

public class CharacterGravity : MonoBehaviour
{
    public Transform GroundPoint;
    public LayerMask SurfaceLayer;
    public float gravity = 30f;            // same sign as your original
    public float terminalVelocity = -20f;  // same as your original (negative)
    public float groundCheckRadius = 0.12f;

    private CharacterController controller;
    private float verticalVelocity = 0f;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (!controller)
            Debug.LogError("CharacterController not found on this GameObject.");
    }

    void Update()
    {
        // Ground check (use a slightly larger radius to avoid flicker)
        isGrounded = Physics.CheckSphere(GroundPoint.position, groundCheckRadius, SurfaceLayer);

        // Gentle snap keeps feet planted instead of accumulating tiny penetrations
        if (isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        // Gravity integration and clamp exactly like your original semantics
        if (!isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;   // gravity is positive, subtract to go downward
            verticalVelocity = Mathf.Max(verticalVelocity, terminalVelocity);
        }

        // Apply ONLY via CharacterController so collisions/steps/slope limits are honored
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    public void Jump(float force)
    {
        if (isGrounded)
            verticalVelocity = force;   // preserves your external jump strength
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
