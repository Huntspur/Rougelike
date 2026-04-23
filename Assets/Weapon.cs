using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    public SOWeaponScripty weaponData;
    private GameObject currWeapon => weaponData.weaponObj;
    public int damage => weaponData.weaponDamage;
    public float playbackSpeed => weaponData.animtionSpeed;

    public float baseAttackSpeed => weaponData.baseAttackSpeed;

    [Header("Attack Speed")]
    private float attackSpeedModifier = 0f;
    private float attackCooldown = 0f;
    public float EffectiveAttackSpeed => Mathf.Max(0.1f, baseAttackSpeed + attackSpeedModifier);

    public bool attacking = false;

    [Header("References")]
    public Animator anim;
    public Transform weaponPoint;
    public BoxCollider2D hitbox;

    private GameObject equippedWeaponInstance;

    private void Start()
    {
        anim = GetComponent<Animator>();
        hitbox.enabled = false;
        EquipWeapon();
    }

    void Update()
    {
        if (attackCooldown > 0) 
        {
            attackCooldown -= Time.deltaTime;
        }


        if (Input.GetKeyDown(KeyCode.E) && attackCooldown <= 0 && weaponData !=null)
        {
           
            Attack();
        }
    }

    public void EquipWeapon()
    {
        if (currWeapon == null) return;

        if (equippedWeaponInstance != null) 
        {
            Destroy(equippedWeaponInstance);
        }

        hitbox.offset = weaponData.hitboxOffset;
        hitbox.size = weaponData.hitboxSize;

        equippedWeaponInstance = Instantiate(currWeapon, weaponPoint.position, weaponPoint.rotation, weaponPoint);
    }

    protected void Attack()
    {
        
        attackCooldown = 1f / EffectiveAttackSpeed;

        anim.speed = playbackSpeed;
        anim.SetTrigger("Attack");
    }

    public void IsAttacking() 
    {
        attacking = true;
        hitbox.enabled = true;
    }

    public void StopAttacking() 
    {
        attacking = false;
        hitbox.enabled = false;
        anim.speed = 1;
    }

    public void UnEquipWeapon() 
    {
        Instantiate(equippedWeaponInstance, weaponPoint.position, Quaternion.identity);
        weaponData = null;
        Destroy(equippedWeaponInstance);
    }

    public void SwapWeapon(SOWeaponScripty newWeaponData)
    {
        weaponData = newWeaponData;
        attackSpeedModifier = 0f; 
        EquipWeapon();
    }

    public void AddAttackSpeedModifier(float amount) => attackSpeedModifier += amount;
    public void RemoveAttackSpeedModifier(float amount) => attackSpeedModifier -= amount;


    public IEnumerator ApplyTimedAttackSpeedBuff(float amount, float duration)
    {
        AddAttackSpeedModifier(amount);
        yield return new WaitForSeconds(duration);
        RemoveAttackSpeedModifier(amount);
    }
}