using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Data")]
    public SOWeaponScripty weaponData;
    private GameObject currWeapon => weaponData.weaponObj;
    private int damage => weaponData.weaponDamage;
    private float baseAttackSpeed => weaponData.baseAttackSpeed;

    [Header("Attack Speed")]
    private float attackSpeedModifier = 0f;
    private float attackCooldown = 0f;
    public float EffectiveAttackSpeed => Mathf.Max(0.1f, baseAttackSpeed + attackSpeedModifier);

    [Header("References")]
    public Animator anim;
    public Transform weaponPoint;
    public BoxCollider2D hitbox;

    private GameObject equippedWeaponInstance;

    private void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
        anim = GetComponentInParent<Animator>();
        EquipWeapon();
    }

    void Update()
    {
        if (attackCooldown > 0) 
        {
            attackCooldown -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.E) && attackCooldown <= 0)
        {
            anim.SetTrigger("Attack");
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

        equippedWeaponInstance = Instantiate(currWeapon, weaponPoint.position, Quaternion.identity, weaponPoint);
    }

    protected void Attack()
    {
        attackCooldown = 1f / EffectiveAttackSpeed;

        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;

        int colliderCount = Physics2D.OverlapCollider(hitbox, filter, collidersToDamage);

        for (int i = 0; i < colliderCount; i++)
        {
            if (collidersToDamage[i] == null) continue;
            if (!collidersToDamage[i].CompareTag("Enemy")) continue;

            EnemyController enemy = collidersToDamage[i].GetComponent<EnemyController>();
            if (enemy != null) 
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    //unequip weapon

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