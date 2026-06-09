using UnityEngine;

namespace Desolation.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 3.2f;
        [SerializeField] private float sprintSpeed = 5.8f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float slopeLimit = 45f;

        [Header("Stamina Settings")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float staminaDrainRate = 15f;
        [SerializeField] private float staminaRegenRate = 10f;
        
        [Header("Look Settings")]
        [SerializeField] private Transform playerCamera;
        [SerializeField] private float baseLookSpeedX = 2.0f;
        [SerializeField] private float baseLookSpeedY = 2.0f;
        [SerializeField] private float upperLookLimit = 80.0f;
        [SerializeField] private float lowerLookLimit = -80.0f;

        // Dynamic State (read-only from outside)
        public float CurrentStamina { get; private set; }
        public bool IsSprinting { get; private set; }
        public float LookSensitivityMultiplier { get; set; } = 1.0f;

        private CharacterController characterController;
        private Vector3 moveDirection;
        private Vector2 currentInput;

        private float rotationX = 0f;
        private bool isGrounded;

        // Mobile touch variables
        private Vector2 joystickInputLeft;
        private Vector2 touchInputRight;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            CurrentStamina = maxStamina;
        }

        private void Start()
        {
            // Set slope limit programmatically to prevent getting stuck in backrooms wall edges
            characterController.slopeLimit = slopeLimit;

            // Load saved input sensitivity from PlayerPrefs if available
            LookSensitivityMultiplier = PlayerPrefs.GetFloat("Settings_Sensitivity", 1.0f);
        }

        private void Update()
        {
            HandleGroundedState();
            HandleInput();
            HandleLookRotation();
            HandleStamina();
            HandleMovementVector();
        }

        private void HandleGroundedState()
        {
            isGrounded = characterController.isGrounded;
            if (isGrounded && moveDirection.y < 0)
            {
                moveDirection.y = -2f; // Keep aligned to floor surface
            }
        }

        private void HandleInput()
        {
            // Dual keyboard + dynamic touch joysticks support
            float keyboardX = Input.GetAxisRaw("Horizontal");
            float keyboardY = Input.GetAxisRaw("Vertical");

            // Combine both inputs to support universal platform play testing
            float finalX = keyboardX + joystickInputLeft.x;
            float finalY = keyboardY + joystickInputLeft.y;

            currentInput = new Vector2(finalX, finalY).normalized;

            // Sprint criteria
            bool sprintKeyPressed = Input.GetKey(KeyCode.LeftShift);
            IsSprinting = sprintKeyPressed && currentInput.y > 0.1f && CurrentStamina > 5f;
        }

        private void HandleLookRotation()
        {
            if (playerCamera == null) return;

            // Retrieve touch swiping / mouse input delta
            float mouseX = Input.GetAxis("Mouse X") * baseLookSpeedX * LookSensitivityMultiplier;
            float mouseY = Input.GetAxis("Mouse Y") * baseLookSpeedY * LookSensitivityMultiplier;

            // Add virtual joystick touch swipe if playing on mobile touchpads
            float finalTurnX = mouseX + (touchInputRight.x * baseLookSpeedX * LookSensitivityMultiplier * 0.12f);
            float finalTurnY = mouseY + (touchInputRight.y * baseLookSpeedY * LookSensitivityMultiplier * 0.12f);

            rotationX -= finalTurnY;
            rotationX = Mathf.Clamp(rotationX, lowerLookLimit, upperLookLimit);

            playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, finalTurnX, 0);

            // Clean inertia decay for touch pads
            touchInputRight = Vector2.Lerp(touchInputRight, Vector2.zero, Time.deltaTime * 6f);
        }

        private void HandleStamina()
        {
            if (IsSprinting && currentInput.magnitude > 0.1f)
            {
                CurrentStamina = Mathf.Max(0f, CurrentStamina - staminaDrainRate * Time.deltaTime);
                if (CurrentStamina <= 0f)
                {
                    IsSprinting = false;
                }
            }
            else
            {
                CurrentStamina = Mathf.Min(maxStamina, CurrentStamina + staminaRegenRate * Time.deltaTime);
            }
        }

        private void HandleMovementVector()
        {
            float speed = IsSprinting ? sprintSpeed : walkSpeed;
            
            // Build vector based on look directions to enable fluid sliding against collisions
            Vector3 movementForward = transform.forward * currentInput.y;
            Vector3 movementSideways = transform.right * currentInput.x;
            Vector3 finalVelocity = (movementForward + movementSideways) * speed;

            moveDirection.x = finalVelocity.x;
            moveDirection.z = finalVelocity.z;

            // Apply gravitational pull over time
            if (!isGrounded)
            {
                moveDirection.y += gravity * Time.deltaTime;
            }

            // Perform movement
            characterController.Move(moveDirection * Time.deltaTime);
        }

        // --- PUBLIC MOBILE CONTROLS CONTEXT BRIDGES ---
        public void SetJoystickMovement(Vector2 leftAxis)
        {
            joystickInputLeft = leftAxis;
        }

        public void SetTouchLookSwipe(Vector2 dragDelta)
        {
            touchInputRight = dragDelta;
        }

        public void ToggleSprint(bool sprintOn)
        {
            IsSprinting = sprintOn && currentInput.y > 0.1f && CurrentStamina > 5f;
        }

        public void BoostStamina(float percentAmount)
        {
            CurrentStamina = Mathf.Clamp(CurrentStamina + percentAmount, 0f, maxStamina);
        }
    }
}
