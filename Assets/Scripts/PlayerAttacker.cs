using System;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour {
    private AnimatorManager _animatorManager;
    private PlayerManager _playerManager;
    private InputManager _inputManager;
    private CameraManager _cameraManager;
    public string lightAttackAnimation ="Light Attack Default";
    public string heavyAttackAnimation="Heavy Attack Default";
    private string _lastAttack;

    private void Awake()
    {
        _animatorManager = GetComponent<AnimatorManager>();

        _playerManager = GetComponentInParent<PlayerManager>();
        _inputManager = GetComponentInParent<InputManager>();

        if ( Camera.main != null ) _cameraManager = Camera.main.GetComponentInParent<CameraManager>();
        else Debug.LogWarning("[Action Required] No main Camera in Scene!");
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        // if ( playerManager.canDoCombo )
        // {
        //     inputManager.comboFlag = true;
        //     animatorManager.animator.SetBool("IsUsingRightHand", true); // Todo: think of removing it here - but where (?) -oo-> Animator handler + appropriate fct. 
        //     HandleWeaponCombo(playerInventory.rightWeapon);
        //     inputManager.comboFlag = false;
        // }
        // else
        // {
        //     if ( playerManager.isInteracting || playerManager.canDoCombo ) return;
        //
        //     animatorManager.animator.SetBool("IsUsingRightHand", true); // Todo: think of removing it here - but where (?) -oo-> Animator handler + appropriate fct. 
        //     HandleLightAttack(playerInventory.rightWeapon);
        // }
        _animatorManager.PlayTargetAnimation(weapon.lightAttack01, true, true);
        _lastAttack = weapon.lightAttack01;
    }

    public void HandleLightAttack()
    {
        _animatorManager.PlayTargetAnimation(lightAttackAnimation, true, true);
        _lastAttack = lightAttackAnimation;
    }

    public void HandleHeavyAttack()
    {
        _animatorManager.PlayTargetAnimation(heavyAttackAnimation, true, true);
        _lastAttack = heavyAttackAnimation;
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        // if ( playerStats.currentStamina <= 0 ) return;
        //
        // weaponSlotManager.attackingWeapon = weapon;
        //
        // if ( inputManager.twoHandFlag )
        // {
        //     animatorManager.PlayTargetAnimation(weapon.thHeavyAttack01, true, true);
        //     lastAttack = weapon.thHeavyAttack01;
        // }
        // else if ( playerInventory.rightWeapon.isMeleeWeapon || playerInventory.rightWeapon.isUnarmed )
        // {
        //     animatorManager.PlayTargetAnimation(weapon.ohHeavyAttack01, true, true);
        //     lastAttack = weapon.ohHeavyAttack01;
        // }
        // else if (
        //     playerInventory.rightWeapon.isSpellCaster
        //     || playerInventory.rightWeapon.isFaithCaster
        //     || playerInventory.rightWeapon.isPyroCaster
        // )
        // {
        //     // Handle Magic Spell casting
        //     // TODO: Handle Heavy Magic Input
        //     PerformRbMagicAction(playerInventory.rightWeapon);
        // }
    }

    private void HandleAttackCombos(WeaponItem weapon)
    {
        if ( _lastAttack == weapon.lightAttack01 )
        {
            _animatorManager.PlayTargetAnimation(weapon.lightAttack02, true, true);
            _lastAttack = weapon.lightAttack02;
        }
        else if ( _lastAttack == weapon.lightAttack02 )
        {
            _animatorManager.PlayTargetAnimation(weapon.lightAttack03, true, true);
            _lastAttack = weapon.lightAttack03;
        }
        else if ( _lastAttack == weapon.lightAttack03 )
        {
            _animatorManager.PlayTargetAnimation(weapon.lightAttack04, true, true);
            _lastAttack = weapon.lightAttack04;
        }


        if ( _lastAttack == weapon.heavyAttack01 )
        {
            _animatorManager.PlayTargetAnimation(weapon.heavyAttack02, true, true);
            _lastAttack = weapon.heavyAttack02;
        }
        else if ( _lastAttack == weapon.heavyAttack02 )
        {
            _animatorManager.PlayTargetAnimation(weapon.heavyAttack03, true, true);
            _lastAttack = weapon.heavyAttack03;
        }
        else if ( _lastAttack == weapon.heavyAttack03 )
        {
            _animatorManager.PlayTargetAnimation(weapon.heavyAttack04, true, true);
            _lastAttack = weapon.heavyAttack04;
        }

    }

    private void PerformBlockingAction()
    {
        if ( _playerManager.isUsingRootMotion )
            return;
        if ( _playerManager.isBlocking )
            return;

        _animatorManager.PlayTargetAnimation("[Combat Action] Blocking Start", false, true);
        // playerEquipmentManager.OpenBlockingCollider();
        _playerManager.isBlocking = true;
    }
}