using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField, Min(0.01f)] private float attackRadius = 0.5f;
    [SerializeField, Min(1)] private int attackDamage = 1;
    [SerializeField] private LayerMask enemyLayers;

    private PlayerController player;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        UpdateAttackPoint();
    }

    private void UpdateAttackPoint()
    {
        if (attackPoint == null || spriteRenderer == null)
            return;

        Vector3 position = attackPoint.localPosition;
        position.x = Mathf.Abs(position.x) * (spriteRenderer.flipX ? -1f : 1f);
        attackPoint.localPosition = position;
    }

    public void DealDamage()
    {
        Debug.Log(player + " " + !player.IsDead + " " + !player.IsDefending);
        if (player == null || player.IsDead || player.IsDefending)
        {
            return;
        }
        Debug.Log("here");
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayers);

        // Enemy with multiple colliders can still take one hit
        HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>();

        Debug.Log(damagedEnemies);
        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null && damagedEnemies.Add(enemy))
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
