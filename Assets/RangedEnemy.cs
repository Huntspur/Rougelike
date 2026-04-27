using UnityEngine;
using System.Collections;

public class RangedEnemy : MonoBehaviour, IDamageable
{
    public enum EnemyState { 
        Idle, Chase, Shoot, Flee 
    }
    public EnemyState currentState = EnemyState.Idle;

    [Header("Stats")]
    public int health = 50;
    public float moveSpeed = 2.5f;
    public float detectionRange = 7f;
    public float shootRange = 5f;
    public float fleeRange = 2f;
    public float shootCooldown = 2f;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 6f;
    public int projectileDamage = 10;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    private bool isKnockedBack = false;
    private float shootTimer = 0f;
    private Transform player;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (health <= 0) { 
            Die(); 
        }

        if (shootTimer > 0) 
        {
            shootTimer -= Time.deltaTime;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle(distance);
                break;
            case EnemyState.Chase: 
                HandleChase(distance);
                break;
            case EnemyState.Shoot:
                HandleShoot(distance);
                break;
            case EnemyState.Flee:
                HandleFlee(distance);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack) 
        {
            return; 
        }

        Vector2 dir = Vector2.zero;

        if (currentState == EnemyState.Chase)
        {
            dir = ((Vector2)player.position - rb.position).normalized;
        }
        else if (currentState == EnemyState.Flee) 
        {
            dir = (rb.position - (Vector2)player.position).normalized;
        }

        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleIdle(float distance)
    {
        if (distance <= detectionRange) 
        {
            currentState = EnemyState.Chase;
        }
    }

    void HandleChase(float distance)
    {
        if (distance <= fleeRange)
        {
            currentState = EnemyState.Flee;
        }
        else if (distance <= shootRange)
        {
            currentState = EnemyState.Shoot;
        }
        else if (distance > detectionRange) 
        {
            currentState = EnemyState.Idle;
        }
    }

    void HandleShoot(float distance)
    {
        if (distance <= fleeRange)
        {
            currentState = EnemyState.Flee;
            return;
        }

        if (distance > shootRange)
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (shootTimer <= 0)
        {
            shootTimer = shootCooldown;
            Shoot();
        }
    }

    void HandleFlee(float distance)
    {
        if (distance > fleeRange + 1f)
        {
            currentState = EnemyState.Shoot;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            return;
        }

        Vector2 dir = ((Vector2)player.position - (Vector2)firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
        if (ep != null) 
        {
            ep.Init(dir, projectileSpeed, projectileDamage);
        }
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        health -= damage;
        StartCoroutine(Knockback(hitDirection));
    }

    private IEnumerator Knockback(Vector2 hitDirection)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }

    private void Die() => Destroy(gameObject);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fleeRange);
    }
}