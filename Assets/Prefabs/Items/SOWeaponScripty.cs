using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_", menuName = "Weapons/WeaponData")]
public class SOWeaponScripty : ScriptableObject
{
    public string weaponName;
   
    public int weaponDamage;
    public float baseAttackSpeed;
    public float animtionSpeed = 1;

    public Vector2 hitboxOffset;
    public Vector2 hitboxSize;

    public GameObject weaponObj;
    public AnimatorOverrideController animatorOveride; 

}
