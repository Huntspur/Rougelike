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
        damage = dmg;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //  player health system here
            Debug.Log("Player hit for " + damage);
            Destroy(gameObject);
        }

        if (collision.CompareTag("Wall"))
            Destroy(gameObject);
    }
}