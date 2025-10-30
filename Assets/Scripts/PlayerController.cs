using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GameObject avatar; // Assigned in inspector

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float pitchClamp = 80f;

    [Header("Jumping")]
    public float jumpForce = 10f;

    [Header("UI")]
    public Text healthText; // Assign in inspector

    private CharacterController avatarController;
    private CharacterGravity gravityComponent;
    private CharacterForm characterForm;
    private Animator animator;
    private NPCCombat npcCombat;

    private Transform avatarTransform;
    private Transform cameraTransform;

    private float pitch = 0f;
    private Vector3 lastMove;

    void Start()
    {
        if (avatar == null)
        {
            Debug.LogWarning("Avatar not assigned at start. Waiting for assignment.");
            UpdateHealthText();
            return;
        }

        InitializeAvatarReferences();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        UpdateHealthText();
    }

    void Update()
    {
        if (avatar == null)
        {
            UpdateHealthText();
            return;
        }

        HandleLook();
        HandleMovement();
        HandleJumpInput();
        HandleAttackInput();
        HandleAnimation();
        UpdateHealthText();
    }

    void InitializeAvatarReferences()
    {
        avatarTransform = avatar.transform;

        avatarController = avatar.GetComponent<CharacterController>();
        gravityComponent = avatar.GetComponent<CharacterGravity>();
        characterForm = avatar.GetComponent<CharacterForm>();
        animator = avatar.GetComponent<Animator>();
        npcCombat = avatar.GetComponent<NPCCombat>();

        if (!avatarController)
            Debug.LogError("CharacterController missing on Avatar.");
        if (!gravityComponent)
            Debug.LogError("CharacterGravity missing on Avatar.");
        if (!characterForm)
            Debug.LogError("CharacterForm missing on Avatar.");
        if (!npcCombat)
            Debug.LogError("NPCCombat missing on Avatar.");

        cameraTransform = transform.Find("MainCamera");
        if (cameraTransform == null)
        {
            Debug.LogError("MainCamera not found under Player.");
            return;
        }

        Transform modelTransform = avatarTransform.Find("Model");
        Transform headTransform = modelTransform?.Find("Upper Body/Head");
        if (headTransform == null)
        {
            Debug.LogError("Head not found under Model/Upper Body.");
            return;
        }

        // Parent and position the camera as usual
        cameraTransform.SetParent(modelTransform, worldPositionStays: false);
        cameraTransform.position = headTransform.position + new Vector3(0f, 0.35f, 0f);

        cameraTransform.localRotation = Quaternion.Euler(0f, 180f, 0f);
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // rotate avatar for yaw
        avatarTransform.Rotate(Vector3.up * mouseX);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);
        cameraTransform.localEulerAngles = new Vector3(pitch, 180f, 0f); // keep the 180° facing
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(moveX, 0f, moveZ).normalized;

        if (input.magnitude == 0f)
        {
            lastMove = Vector3.zero;
            return;
        }

        // ✅ Flip the forward/backward direction to match camera flip
        Vector3 move = avatarTransform.TransformDirection(new Vector3(-input.x, 0f, -input.z)) * moveSpeed;
        avatarController.Move(move * Time.deltaTime);
        lastMove = input;
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gravityComponent.IsGrounded())
            gravityComponent.Jump(jumpForce);
    }

    void HandleAttackInput()
    {
        if (npcCombat == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            npcCombat.PlayerAttack();
        }
    }

    string currentAnim = "";

    void HandleAnimation()
    {
        if (animator == null || npcCombat == null || characterForm == null) return;

        bool moving = lastMove.magnitude > 0.01f;
        bool attacking = npcCombat.IsAttacking;
        bool engaged = npcCombat.Engaged;

        string nextAnim;

        if (attacking)
            nextAnim = "Basic Attack Animation";
        else if (moving && engaged)
            nextAnim = "Combat Walk Animation";
        else if (moving)
            nextAnim = "Basic Walk Animation";
        else if (engaged)
            nextAnim = "Combat Idle Animation";
        else
            nextAnim = "Basic Idle Animation";

        if (currentAnim != nextAnim)
        {
            currentAnim = nextAnim;
            animator.CrossFadeInFixedTime(currentAnim, 0.1f);
        }
    }

    void UpdateHealthText()
    {
        if (healthText == null) return;

        float current = characterForm ? characterForm.CurrentHP : 0f;
        float max = characterForm ? characterForm.MaxHP : 0f;

        healthText.text = $"{current}/{max}";

        float percent = max > 0f ? current / max : 0f;
        if (percent > 0.5f)
            healthText.color = Color.green;
        else if (percent > 0.25f)
            healthText.color = Color.yellow;
        else
            healthText.color = Color.red;
    }
}
