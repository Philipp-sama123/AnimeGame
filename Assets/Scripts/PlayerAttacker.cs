using System;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour {
    private AnimatorManager _animatorManager;
    private PlayerManager _playerManager;
    private InputManager _inputManager;
    private CameraManager _cameraManager;
    public string lightAttackAnimation = "Light Attack Default";
    public string heavyAttackAnimation = "Heavy Attack Default";
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
        //  ToDo: --> set canDoCombo by Animations (!) 
        // if ( playerManager.canDoCombo ) 
        // {
        //     inputHandler.comboFlag = true;
        //     playerAnimatorManager.animator.SetBool("IsUsingRightHand", true); // Todo: think of removing it here - but where (?) -oo-> Animator handler + appropriate fct. 
        //     HandleWeaponCombo(playerInventory.rightWeapon);
        //     inputHandler.comboFlag = false;
        // }
        // else
        // {
        //     if ( playerManager.isInteracting || playerManager.canDoCombo ) return;
        //
        //     playerAnimatorManager.animator.SetBool("IsUsingRightHand", true); // Todo: think of removing it here - but where (?) -oo-> Animator handler + appropriate fct. 
        //     HandleLightAttack(playerInventory.rightWeapon);
        // }
    }

    public void HandleLightAttack()
    {
        // if ( _lastAttack == lightAttackAnimation )
        // {
        //     _animatorManager.PlayTargetAnimation("Light Attack Combo 1", true, false);
        //     _lastAttack = "Light Attack Combo 1";
        // }
        // else
        // {
        _animatorManager.PlayTargetAnimation(lightAttackAnimation);
        _lastAttack = lightAttackAnimation;
        // }
    }

    public void HandleHeavyAttack()
    {
        _animatorManager.PlayTargetAnimation(heavyAttackAnimation);
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
            _animatorManager.PlayTargetAnimation(weapon.lightAttack02);
            _lastAttack = weapon.lightAttack02;
        }
        else if ( _lastAttack == weapon.lightAttack02 )
        {
            _animatorManager.PlayTargetAnimation(weapon.lightAttack03);
            _lastAttack = weapon.lightAttack03;
        }
        else if ( _lastAttack == weapon.lightAttack03 )
        {
            _animatorManager.PlayTargetAnimation(weapon.lightAttack04);
            _lastAttack = weapon.lightAttack04;
        }


        if ( _lastAttack == weapon.heavyAttack01 )
        {
            _animatorManager.PlayTargetAnimation(weapon.heavyAttack02);
            _lastAttack = weapon.heavyAttack02;
        }
        else if ( _lastAttack == weapon.heavyAttack02 )
        {
            _animatorManager.PlayTargetAnimation(weapon.heavyAttack03);
            _lastAttack = weapon.heavyAttack03;
        }
        else if ( _lastAttack == weapon.heavyAttack03 )
        {
            _animatorManager.PlayTargetAnimation(weapon.heavyAttack04);
            _lastAttack = weapon.heavyAttack04;
        }

    }

    private void PerformBlockingAction()
    {
        _animatorManager.PlayTargetAnimation("[Combat Action] Blocking Start");
        // playerEquipmentManager.OpenBlockingCollider();
        _playerManager.isBlocking = true;
    }
}