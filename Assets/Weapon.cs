using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int attackRange;
    public int damage;
    public float baseAttackSpeed = 1f;
    private float attackSpeedModifier = 0f;
    private float attackCooldown = 0f;
    public float EffectiveAttackSpeed => Mathf.Max(0.1f, baseAttackSpeed + attackSpeedModifier);

    public GameObject currWeapon;
    public Animator anim;
    public Transform weaponPoint;
    public BoxCollider2D hitbox;

    private void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();


        anim = GetComponentInParent<Animator>();
        EquipWeapon();
    }

    void Update()
    {
        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E) && attackCooldown <= 0)
        {
            anim.SetTrigger("Attack");
            Attack();
        }
    }

    public void EquipWeapon()
    {
        if (currWeapon == null) return;

        hitbox.offset = new Vector2(0, attackRange);

        Instantiate(currWeapon, weaponPoint.position, Quaternion.identity, weaponPoint);
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
                enemy.TakeDamage(damage);
        }
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