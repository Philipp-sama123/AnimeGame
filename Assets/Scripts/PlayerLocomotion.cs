using System.Collections;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour {

    private AnimatorManager _animatorManager;
    private PlayerManager _playerManager;
    private InputManager _inputManager;

    public new Rigidbody rigidbody;

    private Vector3 _moveDirection;
    private Camera _mainCamera;

    private CameraManager _cameraManager;
    [Header("Falling")]
    [SerializeField] private float minimumDistanceNeededToBeginFall = 1f;
    [SerializeField] private float groundDetectionRayStartPoint = .5f;
    [SerializeField] private float groundDirectionRayDistance = 0.025f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fallingSpeed = 25f;
    [SerializeField] private float leapingVelocity = 2.5f;
    public int inAirTimer;

    private Vector3 _normalVector;
    private Vector3 _targetPosition;

    [Header("Movement Speeds")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] public Vector3 moveDirection;

    [Header("Jump Speeds")]
    [SerializeField] private float jumpForceMultiplier = 2f;

    private void Awake()
    {
        if ( Camera.main != null ) _mainCamera = Camera.main;
        else Debug.LogWarning("[Not Assigned]: Camera");

        _animatorManager = GetComponent<AnimatorManager>();
        _playerManager = GetComponent<PlayerManager>();
        _inputManager = GetComponent<InputManager>();
        rigidbody = GetComponent<Rigidbody>();
        _cameraManager = GetComponentInChildren<CameraManager>();
    }



    public void HandleMovement()
    {
        if ( _playerManager.isGrounded == false )
            rigidbody.AddForce(rigidbody.transform.forward * _inputManager.moveAmount * 10 * fallingSpeed, ForceMode.Acceleration);

        if ( _inputManager.lockOnFlag && _inputManager.sprintFlag == false )
        {
            _animatorManager.UpdateAnimatorValues(_inputManager.horizontalInput, _inputManager.verticalInput, _inputManager.sprintFlag);
        }
        else
        {
            _animatorManager.UpdateAnimatorValues(0, _inputManager.moveAmount, _inputManager.sprintFlag);
        }
    }

    public void HandleRotation(float deltaTime)
    {
        if ( _inputManager.lockOnFlag )
        {
            if ( _inputManager.sprintFlag || _inputManager.dodgeFlag )
            {
                Vector3 targetDir = Vector3.zero;
                targetDir = _cameraManager.cameraTransform.forward * _inputManager.verticalInput;
                targetDir += _cameraManager.cameraTransform.right * _inputManager.horizontalInput;

                targetDir.Normalize();
                targetDir.y = 0;

                if ( targetDir == Vector3.zero )
                {
                    targetDir = transform.forward;
                }

                Quaternion tr = Quaternion.LookRotation(targetDir);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);

                transform.rotation = targetRotation;
            }
            else
            {
                Vector3 rotationDirection = moveDirection;
                Debug.Log(_cameraManager.currentLockOnTarget + "cameraManager.currentLockOnTarget");
                Debug.Log(_cameraManager.currentLockOnTarget.transform.position + "cameraManager.currentLockOnTarget");
                rotationDirection = _cameraManager.currentLockOnTarget.transform.position - transform.position;
                rotationDirection.y = 0;
                rotationDirection.Normalize();

                Quaternion tr = Quaternion.LookRotation(rotationDirection);
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                transform.rotation = targetRotation;
            }
        }
        else
        {
            Vector3 targetDirection;
            Transform mainCameraTransform = _mainCamera.transform;

            targetDirection = mainCameraTransform.forward * _inputManager.verticalInput;
            targetDirection += mainCameraTransform.right * _inputManager.horizontalInput;

            targetDirection.Normalize();
            targetDirection.y = 0; // no movement on y-Axis (!)

            if ( targetDirection == Vector3.zero )
                targetDirection = transform.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rs * deltaTime);

            transform.rotation = targetRotation;
        }
    }

    public void HandleJumping()
    {
        if ( _inputManager.jumpInput )
        {
            _animatorManager.animator.SetInteger("State", 2);
            _animatorManager.forceMultiplier = jumpForceMultiplier;
        }
    }


    public void HandleRollingAndSprinting()
    {
        if ( _inputManager.dodgeFlag )
        {
            Transform mainCameraTransform = _mainCamera.transform;
            moveDirection = mainCameraTransform.forward * _inputManager.verticalInput;
            moveDirection += mainCameraTransform.right * _inputManager.horizontalInput;


            if ( _playerManager.isWeaponEquipped == false )
            {
                if ( _inputManager.lockOnFlag ? _inputManager.verticalInput > 0 : _inputManager.moveAmount > 0 )
                {
                    _animatorManager.PlayTargetAnimation("Slide Forward");
                    moveDirection.y = 0;

                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = rollRotation;
                }
                else
                {
                    _animatorManager.PlayTargetAnimation("Slide Backward");
                    moveDirection.y = 0;
                }
            }
            else
            {
                if ( _inputManager.lockOnFlag ? _inputManager.verticalInput > 0 : _inputManager.moveAmount > 0 )
                {
                    _animatorManager.PlayTargetAnimation("Dodge Forward");
                    moveDirection.y = 0;

                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = rollRotation;
                }
                else
                {
                    _animatorManager.PlayTargetAnimation("Dodge Backward");
                    moveDirection.y = 0;
                }
            }
        }
    }

    public void HandleFalling(float deltaTime)
    {
        _playerManager.isGrounded = false;
        rigidbody.useGravity = true;

        RaycastHit hit;
        Vector3 origin = transform.position;
        origin.y += groundDetectionRayStartPoint;

        if ( _playerManager.isInAir )
        {
            inAirTimer++;
            rigidbody.AddForce(Vector3.forward * leapingVelocity, ForceMode.Impulse);
            rigidbody.AddForce(Vector3.down * fallingSpeed * 9.8f * inAirTimer * deltaTime, ForceMode.Acceleration);
        }

        Vector3 dir = moveDirection;
        dir.Normalize();
        origin = origin + dir * groundDirectionRayDistance;

        _targetPosition = rigidbody.position;

        Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red);
        if ( Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, groundLayer) )
        {
            _normalVector = hit.normal;
            Vector3 tp = hit.point;
            _playerManager.isGrounded = true;
            _targetPosition.y = tp.y;

            if ( _playerManager.isInAir )
            {
                Debug.Log("[Info] Landing You were in the air for " + inAirTimer);
                // _animatorManager.PlayTargetAnimation("Landing");
                inAirTimer = 0;
                _playerManager.isInAir = false;
            }
        }
        else
        {
            if ( _playerManager.isGrounded )
            {
                _playerManager.isGrounded = false;
            }

            if ( _playerManager.isInAir == false )
            {
                _animatorManager.animator.SetInteger("State", 3);
                _playerManager.isInAir = true;
            }

            // ToDo: slight air movement 
        }
        // KeepFeetOnGround();
        if ( _playerManager.isGrounded ) // ToDO: find out if its good(?) if no --> better way keep your feet on the ground 
            rigidbody.position = Vector3.Lerp(transform.position, _targetPosition, deltaTime / .2f);
    }
    // private void KeepFeetOnGround() {
    //     float raycastDistance = 0.2f; // adjust as needed
    //     RaycastHit hit;
    //     bool isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, raycastDistance);
    //    Debug.Log("isGrounded"+ isGrounded);
    //     if (isGrounded) {
    //         transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
    //     }
    // }
}