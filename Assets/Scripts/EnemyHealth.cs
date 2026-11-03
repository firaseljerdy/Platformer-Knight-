using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maximumHealth = 3;
    private int currentHealth;

    public int CurrentHealth => currentHealth;


    private void Awake()
    {
        currentHealth = maximumHealth;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0 || damage <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        Debug.Log("current health of enemy:"  + currentHealth);

        if (currentHealth == 0) Destroy(gameObject);
       
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
