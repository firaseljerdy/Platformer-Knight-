using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float wanderSpeed = 1.2f;
    [SerializeField] private Vector2 wanderStepTime = new Vector2(1f, 3f);
    [SerializeField] private float wanderPauseChance = 0.3f;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private int moveDirection;
    private int wanderDirection;
    private float wanderTimer;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidbody.freezeRotation = true;
    }

    private void Start()
    {
        PickNewWanderStep();
    }

    private void Update()
    {
        animator.SetFloat("speed", Mathf.Abs(moveDirection));

        spriteRenderer.flipX = moveDirection < 0;
    }

    private void FixedUpdate()
    {
        moveDirection = Wander();
        rigidbody.linearVelocity = new Vector2(wanderSpeed * moveDirection, rigidbody.linearVelocityY);
    }
    private int Wander()
    {
        wanderTimer -= Time.fixedDeltaTime;
        if (wanderTimer <= 0f)
        {
            PickNewWanderStep();
        }

        return wanderDirection;
    }
    private void PickNewWanderStep()
    {
        wanderTimer = Random.Range(wanderStepTime.x, wanderStepTime.y);
        if (Random.value < wanderPauseChance)
        {
            wanderDirection = 0;
        }
        else
        {
            wanderDirection = Random.value < 0.5f ? -1 : 1;
        }
    }
}
