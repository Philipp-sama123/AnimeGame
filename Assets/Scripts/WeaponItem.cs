using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Item")]
public class WeaponItem : Item {
    public GameObject modelPrefab;

    [Header("Damage")]
    public int baseDamage = 25;
    public int criticalDamageMultiplier = 5;

    [Header("Absorption")]
    public float physicalDamageAbsorption = 10f;

    [Header("One Handed Attack Animations")]
    public string lightAttack01;
    public string lightAttack02;
    public string lightAttack03;
    public string lightAttack04;
        
    public string heavyAttack01;
    public string heavyAttack02;
    public string heavyAttack03;
    public string heavyAttack04;

    [Header("Weapon Art -- Special Abilities")]
    public string specialAbility;

    [Header("Weapon Type")]
    public bool isMeleeWeapon;
    public bool isShieldWeapon;
}