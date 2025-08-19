using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 0.5f;
    [Range(0, 1)]
    public float mouseSensitivity = 1;
    public Vector2 pitchMinMax = new Vector2(-40, 85);
    
    private float rotationSpeed = 0.12f;
    private float gravity = 9.8f;

    private Vector3 rotationSmoothVelocity;
    private Vector3 currentRotation;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private CharacterController characterController;

    private GameManager gameManager;
    InputAction moveAction, lookAction;

    private bool playerCaught = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCaught)
            return;

        Vector2 lookValue = lookAction.ReadValue<Vector2>();

        yaw += lookValue.x * mouseSensitivity;
        pitch -= lookValue.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSpeed);
        Camera.main.transform.eulerAngles = currentRotation;

        Vector2 moveValue = moveAction.ReadValue<Vector2>().normalized;
        Vector3 moveDirection = Camera.main.transform.forward * moveValue.y + Camera.main.transform.right * moveValue.x;
        moveDirection.y = 0.0f;
        moveDirection = moveDirection.normalized * speed;

        if (!characterController.isGrounded)
            moveDirection.y -= gravity;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 6)
        {
            if (playerCaught)
                return;
            Guard.OnPlayerCaught();
            playerCaught = true;
            gameManager.OnGameOver();
        }
    }
}
