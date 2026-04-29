using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    public float lifetime = 4f;

    public void Init(Vector2 dir, float spd, int dmg)
    {
        direction = dir;
        speed = spd;
        damage = GameManager.Instance != null ? GameManager.Instance.GetScaledDamage(dmg) : dmg;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Projectile hit: " + collision.gameObject.name + " | Tag: " + collision.tag);

        if (collision.CompareTag("Player"))
        {
            IDamageable target = collision.gameObject.GetComponent<IDamageable>();
            Debug.Log("IDamageable found: " + (target != null));

            if (target != null)
            {
                Vector2 hitDir = (collision.transform.position - transform.position).normalized;
                target.TakeDamage(damage, hitDir);
                Destroy(gameObject);
            }
        }

        if (collision.CompareTag("Wall") || collision.CompareTag("HitBox")) 
        {
            Destroy(gameObject);
        }
    }
}