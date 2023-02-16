using UnityEngine;

public class InputManager : MonoBehaviour {
    private ThirdPersonControls _playerControls;
    private PlayerManager _playerManager;
    private CameraManager _cameraManager;
    private PlayerAttacker _playerAttacker;

    [Header("Movement")]
    public Vector2 movementInput;
    public float horizontalInput;
    public float verticalInput;
    public float moveAmount;

    [Header("Camera")]
    public Vector2 cameraInput;
    public float cameraInputX;
    public float cameraInputY;

    /** Lock On **/
    public bool lockOnInput;
    public bool lockOnFlag;
    public bool rightStickLeftInput;
    public bool rightStickRightInput;

    [Header("Jump and Dodge")]
    public bool dodgeAndSprintInput;
    public bool jumpInput;
    public bool sprintFlag;
    public bool dodgeFlag;
    public float rollInputTimer;

    public bool lightAttackInput;
    public bool heavyAttackInput;

    private void Awake()
    {
        _playerManager = GetComponent<PlayerManager>();
        _playerAttacker = GetComponent<PlayerAttacker>();
        _cameraManager = FindObjectOfType<CameraManager>();
    }

    private void OnEnable()
    {
        if ( _playerControls == null )
        {
            _playerControls = new ThirdPersonControls();

            _playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            _playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            // while you hold it --> true!
            _playerControls.PlayerActions.Sprint.performed += i => dodgeAndSprintInput = true;
            _playerControls.PlayerActions.Sprint.canceled += i => dodgeAndSprintInput = false;
            // when you press the button --> True
            _playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            _playerControls.PlayerActions.Jump.canceled += i => jumpInput = false;

            _playerControls.PlayerActions.LightAttack.performed += i => lightAttackInput = true;
            _playerControls.PlayerActions.LightAttack.canceled += i => lightAttackInput = false;

            _playerControls.PlayerActions.HeavyAttack.performed += i => heavyAttackInput = true;
            _playerControls.PlayerActions.HeavyAttack.canceled += i => heavyAttackInput = false;

            _playerControls.PlayerActions.LockOn.performed += i => lockOnInput = true;


            _playerControls.PlayerActions.LockOnTargetRight.performed += i => rightStickRightInput = true;
            _playerControls.PlayerActions.LockOnTargetLeft.performed += i => rightStickLeftInput = true;
        }
        _playerControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    public void HandleAllInputs(float deltaTime)
    {
        HandleMovementInput();
        HandleRollAndSprintInput(deltaTime);
        HandleAttackInput();
        HandleLockOnInput();
    }

    private void HandleAttackInput()
    {
        if ( lightAttackInput )
            _playerAttacker.HandleLightAttack();
        else if ( heavyAttackInput )
            _playerAttacker.HandleHeavyAttack();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputY = cameraInput.y;
        cameraInputX = cameraInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

    }

    private void HandleRollAndSprintInput(float deltaTime)
    {
        if ( dodgeAndSprintInput )
        {
            rollInputTimer += deltaTime;
            // if ( playerStats.currentStamina <= 0 )
            // {
            // dodgeAndSprintInput = false;
            // sprintFlag = false;
            // }

            if ( moveAmount > 0.5f /*&& playerStats.currentStamina > 0*/ )
            {
                sprintFlag = true;
            }
        }
        else
        {
            sprintFlag = false;
            if ( rollInputTimer > 0 && rollInputTimer < 0.5f )
            {
                dodgeFlag = true;
            }
            rollInputTimer = 0;
        }

        // if ( dodgeAndSprintInput && verticalInput > 0.5f )
        // {
        //     _playerManager.isSprinting = true;
        // }
        // else
        // {
        //     _playerManager.isSprinting = false;
        // }
    }

    private void HandleLockOnInput()
    {
        if ( lockOnInput && lockOnFlag == false )
        {
            lockOnInput = false;
            _cameraManager.HandleLockOn();

            if ( _cameraManager.nearestLockOnTarget != null )
            {
                _cameraManager.currentLockOnTarget = _cameraManager.nearestLockOnTarget;
                lockOnFlag = true;
            }
        }
        else if ( lockOnInput && lockOnFlag )
        {
            lockOnInput = false;
            lockOnFlag = false;
            _cameraManager.ClearLockOnTargets();
        }

        if ( lockOnFlag && rightStickLeftInput )
        {
            rightStickLeftInput = false;
            _cameraManager.HandleLockOn();

            if ( _cameraManager.leftLockTarget != null )
            {
                _cameraManager.currentLockOnTarget = _cameraManager.leftLockTarget;
            }
        }

        if ( lockOnFlag && rightStickRightInput )
        {
            rightStickRightInput = false;
            _cameraManager.HandleLockOn();

            if ( _cameraManager.rightLockTarget != null )
            {
                _cameraManager.currentLockOnTarget = _cameraManager.rightLockTarget;
            }
        }
        _cameraManager.SetCameraHeight();
    }

}