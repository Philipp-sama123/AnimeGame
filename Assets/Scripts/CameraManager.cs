using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    private PlayerManager _playerManager;

    public Transform cameraPivotTransform; //The object the camera uses to pivot (Look up and down)
    public Transform cameraTransform; //The transform of the actual camera object in the scene

    public Transform targetTransform;
    public LayerMask collisionLayers; //The layers we want our camera to collide with
    public LayerMask environmentLayer;

    private float _defaultPosition;
    private Vector3 _cameraFollowVelocity = Vector3.zero;
    private Vector3 _cameraVectorPosition;

    public float cameraCollisionOffSet = 0.2f; //How much the camera will jump off of objects its colliding with
    public float minimumCollisionOffSet = 0.2f;
    public float cameraCollisionRadius = 2;
    public float cameraFollowSpeed = 0.1f;
    public float cameraLookSpeed = 2;
    public float cameraPivotSpeed = 2;

    public float lookAngle; //Camera looking up and down
    public float pivotAngle; //Camera looking left and right
    public float minimumPivotAngle = -45;
    public float maximumPivotAngle = 45;
    private Vector3 cameraFollowVelocity;

    [Header("Camera Height for LockOn State")]
    public float lockedPivotPosition = 2.25f;
    public float unlockedPivotPosition = 1.65f;


    [Header("Lock On Target")]
    public List<CharacterManager> availableTargets;
    public CharacterManager currentLockOnTarget;
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockTarget;
    public CharacterManager rightLockTarget;

    public float maximumLockOnDistance = 30f;
    private InputManager inputHandler;

    private void Awake()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        inputHandler = FindObjectOfType<InputManager>();
        if ( Camera.main != null ) cameraTransform = Camera.main.transform;
        _defaultPosition = cameraTransform.localPosition.z;
        // collisionLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
    }

    public void HandleAllCameraMovement(float deltaTime, float cameraInputX, float cameraInputY)
    {
        FollowTarget(deltaTime);
        RotateCamera(deltaTime, cameraInputX, cameraInputY);
    }

    private void FollowTarget(float deltaTime)
    {
        Vector3 targetPosition = Vector3.SmoothDamp
            (transform.position, _playerManager.transform.position, ref cameraFollowVelocity, deltaTime / cameraFollowSpeed);

        transform.position = targetPosition;
        // HandleCameraCollisions(deltaTime);
    }

    private void RotateCamera(float deltaTime, float cameraInputX, float cameraInputY)
    {
        lookAngle += (cameraInputX * cameraLookSpeed);
        pivotAngle -= (cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;
    }


    public void HandleLockOn()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = -Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26f);

        for ( int i = 0; i < colliders.Length; i++ )
        {
            CharacterManager character = colliders[i].GetComponent<CharacterManager>();

            if ( character != null )
            {
                Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                RaycastHit hit;

                if ( character.transform.root != targetTransform.transform.root
                     && viewableAngle > -50 && viewableAngle < 50
                     && distanceFromTarget <= maximumLockOnDistance )
                {
                    if ( Physics.Linecast(_playerManager.lockOnTransform.position, character.lockOnTransform.transform.position, out hit) )
                    {
                        Debug.DrawLine(_playerManager.lockOnTransform.position, character.lockOnTransform.transform.position);
                        if ( hit.transform.gameObject.layer == environmentLayer )
                        {
                            //cannot lock onto target
                        }
                        else
                        {
                            availableTargets.Add(character);
                        }
                    }
                }
            }
        }
        for ( int k = 0; k < availableTargets.Count; k++ )
        {
            float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[k].transform.position);

            if ( distanceFromTarget < shortestDistance )
            {
                shortestDistance = distanceFromTarget;
                nearestLockOnTarget = availableTargets[k];
            }

            if ( inputHandler.lockOnFlag )
            {
                Vector3 relativeEnemyPosition = inputHandler.transform.InverseTransformPoint(availableTargets[k].transform.position);
                var distanceFromLeftTarget = relativeEnemyPosition.x;
                var distanceFromRightTarget = relativeEnemyPosition.x;


                if ( relativeEnemyPosition.x < 0f && distanceFromLeftTarget > shortestDistanceOfLeftTarget
                                                  && availableTargets[k] != currentLockOnTarget )
                {
                    shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                    leftLockTarget = availableTargets[k];
                }
                else if ( relativeEnemyPosition.x > 0f && distanceFromRightTarget < shortestDistanceOfRightTarget
                                                       && availableTargets[k] != currentLockOnTarget )
                {
                    shortestDistanceOfRightTarget = distanceFromRightTarget;
                    rightLockTarget = availableTargets[k];
                }
            }
        }
    }

    public void SetCameraHeight()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
        Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

        if ( currentLockOnTarget != null )
        {
            cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
        }
        else
        {
            cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
        }

    }

    public void ClearLockOnTargets()
    {
        availableTargets.Clear();
        currentLockOnTarget = null;
        nearestLockOnTarget = null;
    }
}