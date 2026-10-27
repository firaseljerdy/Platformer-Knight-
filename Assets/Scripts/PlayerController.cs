using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Health")]
    [SerializeField] private int maximumHealth = 3;

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
    private InputAction attackAction;
    private InputAction defendAction;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private float horizontalInput;
    private bool jumpRequested;
    private int jumpsRemaining = 1;
    private bool isRunning;

    private float lastAttackTime;
    private bool isDefending;
    private bool isDead;
    private int attackStep;
    private int currentHealth;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maximumHealth;
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
        attackAction = playerActions.FindAction("Attack", true);
        defendAction = playerActions.FindAction("Defend", true);
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
        runAction?.Enable();
        attackAction?.Enable();
        defendAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        runAction?.Disable();
        attackAction?.Disable();
        defendAction?.Disable();
        jumpRequested = false;
    }

    private void Update()
    {
        horizontalInput = isDead ? 0f : moveAction.ReadValue<float>();
        bool isOnGround = isGrounded();
        isRunning = !isDead && runAction.IsPressed() && Mathf.Abs(horizontalInput) > 0.05f;

        bool wasDefending = isDefending;

        // check if we are defending
        isDefending = !isDead && isOnGround && defendAction.IsPressed();

        //play defend animation
        if (isDefending && !wasDefending)
        {
            animator.SetTrigger("playDefend");
        }

        if (!isDead && !isDefending && jumpAction.WasPressedThisFrame())
        {
            jumpRequested = true;
        }

        if (!isDead && !isDefending && attackAction.IsPressed())
        {
            // play attack
            PlayAttack(isOnGround);
        }

        animator.SetFloat("speed", Mathf.Abs(horizontalInput));
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isOnGround);
        animator.SetBool("isDefending", isDefending);
        animator.SetBool("isDead", isDead);

        // check for the sprite flip
        if (!isDead && Mathf.Abs(horizontalInput) > 0.05f)
        {
            spriteRenderer.flipX = horizontalInput < 0f;
        }
    }

    private void PlayAttack(bool isGrounded)
    {
        if (isGrounded && isRunning)
        {
            animator.SetTrigger("playRunAttack");
            return;
        }

        if (Time.time - lastAttackTime > 1f)
        {
            attackStep = 0;
        }

        // cycle through the attack steps
        attackStep = attackStep % 3 + 1;
        lastAttackTime = Time.time;
        animator.SetTrigger("playAttack" + attackStep);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        if (isDefending)
        {
            animator.SetTrigger("playDefend");
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth <= 0)
        {
            isDead = true;
            body.linearVelocity = Vector2.zero;
            animator.SetBool("isDead", true);
        }
        else {
            animator.SetTrigger("playHurt");
        }

        Debug.Log(currentHealth);
    }

    public void Revive()
    {
        currentHealth = maximumHealth;
        isDead = false;
        jumpsRemaining = 1;
        animator.SetBool("isDead", false);
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

        float speed = isRunning ? runSpeed : walkSpeed;
        float movement = isDead || isDefending ? 0f : horizontalInput * speed;

        body.linearVelocity =
            new Vector2(movement, body.linearVelocity.y);

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

        if (body != null && body.linearVelocity.y > 0f)
        {
            return false;
        }

        if (groundCheck == null)
        {
            return false;
        }

        // collect every rigidbody around the player
        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Ground") && !hit.isTrigger && hit.attachedRigidbody != body)
            {
                return true;
            }
        }

        return false;
    }
}
