using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Character Movement")]
    // Movement variables
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    // Mouse control variables
    public float lookSpeed = 2.0f;
    public float lookXLimit = 90.0f;


    public Material[] playerMaterials;

    // Array of player colors for random assignment
    Color[] playerColors = new Color[]
    {
        new Color(1.0f, 0.0f, 0.0f),    // Red
        new Color(0.0f, 1.0f, 0.0f),    // Green
        new Color(0.0f, 0.0f, 1.0f),    // Blue
        new Color(1.0f, 1.0f, 0.0f),    // Yellow
        new Color(1.0f, 0.0f, 1.0f),    // Magenta
        new Color(0.0f, 1.0f, 1.0f),    // Cyan
        new Color(0.5f, 0.0f, 0.5f),    // Purple
    };

    // References for the characterController
    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;


    [HideInInspector]
    public bool canMove = true;

    // Camera offset from player's position (this is not really used as the game is a first person game)
    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;

   
    public override void OnStartClient()
    {
        base.OnStartClient();
        // Set up camera for each client joined
        if (base.IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);
        }
        else
        {
            // Disable PlayerController if it's not owned by the local player, so each player only contols their own character
            gameObject.GetComponent<PlayerController>().enabled = false;
        }

        if (base.IsClient)
        {
            // Assign a random color to the player object
            if (playerMaterials != null && playerMaterials.Length > 0)
            {
                Renderer renderer = GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    int randomIndex = Random.Range(0, playerColors.Length);
                    renderer.material.color = playerColors[randomIndex];
                }
            }
        }
    }

    // Executed once when the game starts
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        // Lock cursor for better player experience
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool isRunning = false;
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // Keybord WASD contolls
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Mouse contols
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Jump Controls
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);

        // Rotate the camera based on mouse movement
        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
