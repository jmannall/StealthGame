using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float speed = 0.5f;
    public float rotationSpeed = 20.0f;
    private CharacterController characterController;

    private GameManager gameManager;
    InputAction moveAction, lookAction;
    InputAction jumpAction;

    private bool playerCaught = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCaught)
            return;

        Vector2 lookValue = lookAction.ReadValue<Vector2>();

        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 moveDirection = transform.forward * moveValue.y + transform.right * moveValue.x;

        if (jumpAction.IsPressed())
            moveDirection.y += 5.0f;

        Camera.main.transform.Rotate(Vector3.right, -lookValue.y * rotationSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, lookValue.x * rotationSpeed * Time.deltaTime);
        characterController.Move(moveDirection * Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (playerCaught)
            return;
        if (collision.gameObject.layer == 6)
        {
            playerCaught = true;
            gameManager.OnGameOver();
        }
    }
}
