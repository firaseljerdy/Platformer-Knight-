using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerDamageTester : MonoBehaviour
{
    [SerializeField, Min(1)] private int damageAmount = 1;
    private PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.hKey.wasPressedThisFrame)
            player.TakeDamage(damageAmount);
    }
}
