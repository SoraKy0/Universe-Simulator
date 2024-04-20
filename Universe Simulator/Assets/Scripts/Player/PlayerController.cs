using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Player Movement Variables")]
    //variables that impact the players movement
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    //Mouse Sensitivity (how fast the player looks around)
    public float mouseSensitivity = 2.0f;
    //prevents player from looking up and down past 90 degrees
    public float headAngleMax = 90.0f;

    public Material[] playerMaterials;

    //Array of player colors so when the game starts each player gets a random colour
    Color[] playerColors = new Color[]
    {
        new Color(1.0f, 0.0f, 0.0f), //Red
        new Color(0.0f, 1.0f, 0.0f), //Green
        new Color(0.0f, 0.0f, 1.0f), //Blue
        new Color(1.0f, 1.0f, 0.0f), //Yellow
        new Color(1.0f, 0.0f, 1.0f), //Magenta
        new Color(0.0f, 1.0f, 1.0f), //Cyan
        new Color(0.5f, 0.0f, 0.5f), //Purple
    };

    //References for the characterController from the inspector
    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    //Moves camera to position to where the camera does not clip into the enemys eyes or body
    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera playerCamera;

    public override void OnStartClient()
    {
        base.OnStartClient();
        //Each player gets a clone of the main camera
        if (base.IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);
        }
        else
        {
            //Disable PlayerController if it's not owned by the local player, so each player only controls their own character
            gameObject.GetComponent<PlayerController>().enabled = false;
        }

        if (base.IsClient)
        {
            //Assign a random color to the player
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

    //Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        //lock the cursor to the game so that you do not click something else by accident
        Cursor.lockState = CursorLockMode.Locked;
        //Hides the cusor when the game starts
        Cursor.visible = false;
    }

    //Update is called once per frame
    void Update()
    {
        if (PauseMenu.GameIsPaused)
        {
            //when the pause menu comes up the player movement and look is frozen
            return;
        }

        bool isRunning = false;
        isRunning = Input.GetKey(KeyCode.LeftShift);

        //Keyboard WASD controls
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        //Mouse controls
        float mouseSensOnX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float mouseSensOnY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * mouseSensOnX) + (right * mouseSensOnY);

        //when the space bar is pressed the player will jump and will only jump if the player is touching the ground colosion 
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        //gives the player gravity 
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        //moves the player 
        characterController.Move(moveDirection * Time.deltaTime);

        //Rotate the camera based on mouse movement
        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationX = Mathf.Clamp(rotationX, - headAngleMax, headAngleMax);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
        }
    }
}
