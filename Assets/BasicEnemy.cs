using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour, IDamageable
{
    public enum EnemyState
    {
        Idle, Chase, Attack
    }
    public EnemyState currentState = EnemyState.Idle;

    [Header("Stats")]
    public int health;
    public float moveSpeed = 3f;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public int damage = 10;

    [Header("Knockback")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    private float attackTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
   


    private void Die()
    {
        Destroy(gameObject);
    }



    private void Update()
    {

        if (health <= 0)
        {
            Die();
        }

        if (attackTimer > 0) 
        {
            attackTimer -= Time.deltaTime;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle(distanceToPlayer);
                break;
            case EnemyState.Chase:
                HandleChase(distanceToPlayer);
                break;
            case EnemyState.Attack:
                HandleAttack(distanceToPlayer);
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack) 
        {
            return; 
        }

        if (currentState == EnemyState.Chase)
        {
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            anim.SetBool("isChasing", true);
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        }
        else 
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isChasing", false);

        }
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
        if (distance <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if(distance > detectionRange) 
        {
            currentState = EnemyState.Idle;
        }
    }
    void HandleAttack(float distance) 
    {
        if (distance > attackRange) 
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (attackTimer <= 0) 
        {
            attackTimer = attackCooldown;
            DealDamage();
        }
    }
    void DealDamage() 
    {
      
        Debug.Log("Dealin' damage");
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        health -= damage;
        anim.SetTrigger("damage");
        StartCoroutine(Knockback(hitDirection));
    }

    private IEnumerator Knockback(Vector2 hitDirection)
    {
        isKnockedBack = true;
        anim.SetTrigger("knockback");

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
