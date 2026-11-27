using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 4.2f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float jumpHeight = 1.35f;
    [SerializeField] private float gravity = -22f;
    [SerializeField] private float turnSpeed = 14f;

    private CharacterController controller;
    private float verticalSpeed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (GameManager.IsPaused || GameManager.IsGameOver)
            return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        Vector2 input = Vector2.zero;
        if (keyboard.aKey.isPressed) input.x -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;
        if (keyboard.wKey.isPressed) input.y += 1f;
        input = Vector2.ClampMagnitude(input, 1f);

        Transform cameraTransform = Camera.main != null ? Camera.main.transform : transform;
        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        Vector3 direction = right * input.x + forward * input.y;

        float speed = keyboard.leftShiftKey.isPressed ? sprintSpeed : walkSpeed;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                turnSpeed * Time.deltaTime);
        }

        if (controller.isGrounded)
        {
            if (verticalSpeed < 0f)
                verticalSpeed = -2f;

            if (keyboard.spaceKey.wasPressedThisFrame)
                verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
        }

        Vector3 velocity = direction * speed;
        velocity.y = verticalSpeed;
        controller.Move(velocity * Time.deltaTime);
    }
}
