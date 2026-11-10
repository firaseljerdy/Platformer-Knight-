using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maximumHealth = 3;
    [SerializeField] Transform healthFill;
    [SerializeField] SpriteRenderer spriteRenderer;
    private int currentHealth;
    private Vector3 fullHealthScale;
    private Coroutine hitFlash;
    private Color normalColor;

    public int CurrentHealth => currentHealth;


    private void Awake()
    {
        currentHealth = maximumHealth;

        if (healthFill != null)
        {
            fullHealthScale = healthFill.localScale;
        }

        if (spriteRenderer != null)
        {
            normalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0 || damage <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        if (healthFill != null)
        {
            Vector3 scale = fullHealthScale;
            scale.x *= (float)currentHealth / maximumHealth;
            healthFill.localScale = scale;
        }

        Debug.Log("current health of enemy:"  + currentHealth);

        if (currentHealth == 0) Destroy(gameObject);

        if (spriteRenderer != null)
        {
            if (hitFlash != null)
            {
                StopCoroutine(hitFlash);
            }
            hitFlash = StartCoroutine(FlashOnHit());
        }
    }

    private IEnumerator FlashOnHit()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = normalColor;
        hitFlash = null;
    }

    [ContextMenu("Test one hit(play mode)")]
    private void TestOneHit()
    {
        if (Application.isPlaying)
        {
            TakeDamage(1);
        }
    }
}
