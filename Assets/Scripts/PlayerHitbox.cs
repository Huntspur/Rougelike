using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public Weapon playerWeapon;
    private Transform player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.CompareTag("Enemy"))
        {
            if (playerWeapon.attacking)
            {
                Vector2 hitDir = (collision.transform.position - player.position).normalized;
                IDamageable target = collision.gameObject.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(playerWeapon.damage, hitDir);
                }
            }
        }
        
    }
}