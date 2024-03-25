using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    public Material[] playerMaterials; 

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

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);    
            
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }

        if (base.IsClient)
        {
            // Assign a random color to the player
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

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        bool isRunning = false;
        isRunning = Input.GetKey(KeyCode.LeftShift);

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}
