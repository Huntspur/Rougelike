using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{

    public Weapon playerWeapon;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (playerWeapon.attacking)
            {
                EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
                enemy.TakeDamage(playerWeapon.weaponData.weaponDamage);
            }
        }
    }
}
