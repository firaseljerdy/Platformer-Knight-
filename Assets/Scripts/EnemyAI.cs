using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyAI : MonoBehaviour
{
    public enum AIState { Wander, Seek, Flee }

    [SerializeField] private float wanderSpeed = 1.2f;
    [SerializeField] private Vector2 wanderStepTime = new Vector2(1f, 3f);
    [SerializeField] private float wanderPauseChance = 0.3f;
    [SerializeField] private float probeRadius = 0.1f;
    [SerializeField] private float probeDepth = 0.25f;

    [SerializeField] private AIState state = AIState.Wander;

    [SerializeField] private Transform target;

    [SerializeField] private float probeGroundFactor = 1f;
    [SerializeField] private float probeWallFactor = 1f;
    [SerializeField] private float seekSpeed = 2f;
    [SerializeField] private float stopDistance = 0.5f;

    private Rigidbody2D rigidbody;
    private Collider2D bodyCollider;
    private Animator animator;
    private PlayerController player;
    private SpriteRenderer spriteRenderer;
    private int moveDirection;
    private int wanderDirection;
    private float wanderTimer;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidbody.freezeRotation = true;
    }

    private void Start()
    {
        if (target == null)
        {
            PlayerController found = FindFirstObjectByType<PlayerController>();
            target = found == null ? null : found.transform;
        }

        if (target != null)
        {
            player = target.GetComponent<PlayerController>();
            foreach (Collider2D targetCollider in target.GetComponentsInChildren<Collider2D>())
            {
                Physics2D.IgnoreCollision(bodyCollider, targetCollider);
            }
        }

        PickNewWanderStep();
    }

    private void Update()
    {
        animator.SetFloat("speed", Mathf.Abs(moveDirection));

        spriteRenderer.flipX = moveDirection < 0;
    }

    private void FixedUpdate()
    {
        //moveDirection = Wander();
        float speed;
        switch (state)
        {
            case AIState.Seek:
                moveDirection = Seek();
                speed = seekSpeed;
                break;
            default:
                moveDirection = Wander();
                speed = wanderSpeed;
                break;
        }
        Debug.Log(state);
        rigidbody.linearVelocity = new Vector2(speed * moveDirection, rigidbody.linearVelocityY);
    }
    private int Wander()
    {
        wanderTimer -= Time.fixedDeltaTime;
        if (wanderTimer <= 0f)
        {
            PickNewWanderStep();
        }

        if (wanderDirection != 0 && !CanMove(wanderDirection))
        {
            wanderDirection = -wanderDirection;
            if (!CanMove(wanderDirection))
            {
                wanderDirection = 0;
            }
        }

        return wanderDirection;
    }

    private int Seek()
    {
        if (!HasTarget()) return 0;

        float offset = target.position.x - transform.position.x;
        if (Mathf.Abs(offset) <= stopDistance)
        {
            return 0;
        }
        int direction = offset > 0f ? 1 : -1;
        return CanMove(direction) ? direction : 0;
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


    // THE ENEMY SENSES
    private bool CanMove(int direction)
    {
        return HasGroundAhead(direction) && !HasWallAhead(direction);
    }

    private bool HasTarget()
    {
        return target != null && (player == null || !player.IsDead);
    }

    private bool HasGroundAhead(int direction)
    {
        Bounds bounds = bodyCollider.bounds;

        Vector2 probe = new Vector2(
            bounds.center.x + direction * (bounds.extents.x + probeRadius * probeGroundFactor),
            bounds.min.y - probeDepth);

        return TouchGround(probe);
    }

    private bool HasWallAhead(int direction)
    {
        Bounds bounds = bodyCollider.bounds;
        Vector2 probe = new Vector2(
            bounds.center.x + direction * (bounds.extents.x + probeRadius * probeWallFactor),
            bounds.center.y);

        return TouchGround(probe);
    }

    private bool TouchGround(Vector2 point)
    {
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(point, probeRadius))
        {
            if (hit != bodyCollider && !hit.isTrigger && hit.CompareTag("Ground"))
            {
                return true;
            }
        }

        return false;
    }


    // draw gizmo test
    private void OnDrawGizmosSelected()
    {
        Collider2D gizmoCollider = bodyCollider != null ? bodyCollider : GetComponent<Collider2D>();
        if (gizmoCollider == null)
        {
            return;
        }

        Bounds bounds = gizmoCollider.bounds;
        Gizmos.color = Color.green;
        foreach (int direction in new[] { -1, 1 })
        {
            Gizmos.DrawWireSphere(new Vector3(
                bounds.center.x + direction * (bounds.extents.x + probeRadius * probeGroundFactor),
                bounds.min.y - probeDepth), probeRadius);

            Gizmos.DrawWireSphere(new Vector3(
                bounds.center.x + direction * (bounds.extents.x + probeRadius * probeWallFactor),
                bounds.center.y), probeRadius);
        }
    }
}
