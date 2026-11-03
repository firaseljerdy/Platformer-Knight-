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

    public void DealDamage()
    {
        if (player != null || player.IsDead || player.IsDefending)
        {
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayers);

        // Enemy with multiple colliders can still take one hit
        HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>();

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();
            if (enemy != null && damagedEnemies.Add(enemy))
            {
                enemy.TakeDamage(attackDamage);
            }
        }
     }
}
