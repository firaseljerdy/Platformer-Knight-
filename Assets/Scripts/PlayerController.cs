using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Movement")]
    //[SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField, Min(1)] private int maximumJumps = 2;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;


    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private float horizontalInput;
    private bool jumpRequested;
    private int jumpsRemaining = 1;
    private bool isRunning;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (inputActions == null) 
        {
            Debug.LogError("Assign input controls to the player.");
            enabled = false;
            return;
        }

        InputActionMap playerActions = inputActions.FindActionMap("Player", true);
        moveAction = playerActions.FindAction("Move", true);
        jumpAction = playerActions.FindAction("Jump", true);
        runAction = playerActions.FindAction("Run", true);
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
        runAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        runAction?.Disable();
    }

    private void Update()
    {
        horizontalInput = moveAction.ReadValue<float>();
        bool isOnGround = isGrounded();
        isRunning = runAction.IsPressed() && Mathf.Abs(horizontalInput) > 0.05f;
        Debug.Log(isOnGround + " " + isRunning);

        if (jumpAction.WasPressedThisFrame())
        {
            jumpRequested = true;
        }

        animator.SetFloat("speed", Mathf.Abs(horizontalInput));
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isOnGround);

        // check for the sprite flip
        if (Mathf.Abs(horizontalInput) > 0.05f)
        {
            spriteRenderer.flipX = horizontalInput < 0f;
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded())
        {
            jumpsRemaining = maximumJumps;
        } else if (jumpsRemaining >= maximumJumps)
        {
            jumpsRemaining = maximumJumps - 1;
        }

        body.linearVelocity =
            new Vector2(horizontalInput * walkSpeed, body.linearVelocity.y);

        if (jumpRequested)
        {
            if (jumpsRemaining > 0)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x,
                                        jumpForce);

                jumpsRemaining--;
                
            }
            jumpRequested = false;
        }

    }

    private bool isGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        // collect every rigidbody around the player
        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Ground") && !hit.isTrigger)
            {
                return true;
            }
        }

        return false;
    }
}
