using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;

    private InputAction moveAction;
    private Rigidbody2D body;
    private float horizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        if (inputActions == null) 
        {
            Debug.LogError("Assign input controls to the player.");
            enabled = false;
            return;
        }

        InputActionMap playerActions = inputActions.FindActionMap("Player", true);
        moveAction = playerActions.FindAction("Move", true);
    }

    private void OnEnable()
    {
        moveAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
    }

    private void Update()
    {
        horizontalInput = moveAction.ReadValue<float>();
        //Debug.Log(horizontalInput);
    }

    private void FixedUpdate()
    {
        body.linearVelocity =
            new Vector2(horizontalInput * moveSpeed, body.linearVelocity.y);
    }
}
