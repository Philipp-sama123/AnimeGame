using UnityEngine;

public class AnimatorManager : MonoBehaviour {
    public Animator animator;
    private PlayerManager _playerManager;
    private PlayerLocomotion _playerLocomotion;
    public float forceMultiplier = 1f;

    #region Cached Animator Variables

    public int IsInAir { get; } = Animator.StringToHash("IsInAir");
    private int Vertical { get; } = Animator.StringToHash("Vertical");
    private int Horizontal { get; } = Animator.StringToHash("Horizontal");
    public int CanDoCombo { get; } = Animator.StringToHash("CanDoCombo");

    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _playerManager = GetComponent<PlayerManager>();
        _playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        // ToDO:  raw or snapping? 
        animator.SetFloat(Horizontal, isSprinting ? horizontalMovement * 2 : horizontalMovement, 0.1f, Time.deltaTime);
        animator.SetFloat(Vertical, isSprinting ? verticalMovement * 2 : verticalMovement, 0.1f, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnimation)
    {
        // ToDO: what is this actually for? 
        animator.CrossFade(targetAnimation, 0.1f);
    }

    private void OnAnimatorMove()
    {
        float delta = Time.deltaTime;

        Vector3 deltaPosition = animator.deltaPosition;
        Quaternion deltaRotation = animator.deltaRotation;

        Vector3 deltaVelocity = deltaPosition / delta;

        _playerLocomotion.rigidbody.drag = 0; //ToDo: find Out what this is doing here? 

        deltaVelocity *= forceMultiplier;
        _playerLocomotion.rigidbody.velocity = deltaVelocity;
        _playerLocomotion.rigidbody.rotation *= deltaRotation; // multiply quaternions to add shit

    }

    #region Animator Events

    // public void EnableRotation()
    // {
    //     animator.SetBool("CanRotate", true);
    // }
    //
    // public void StopRotation()
    // {
    //     animator.SetBool("CanRotate", false);
    // }

    public void EnableCombo()
    {
        animator.SetBool("CanDoCombo", true);
    }

    public void DisableCombo()
    {
        animator.SetBool("CanDoCombo", false);
    }

    #endregion
}