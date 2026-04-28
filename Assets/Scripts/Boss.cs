using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour, IDamageable, IXPSource
{
    public enum BossPhase { PhaseOne, PhaseTwo }
    public BossPhase currentPhase = BossPhase.PhaseOne;

    [Header("Stats")]
    public int maxHealth = 300;
    public int currentHealth;
    public int damage = 20;
    public int xpValue = 150;
    public int XPValue => xpValue;

    [Header("Movement")]
    public float moveSpeed = 1f;
    private Rigidbody2D rb;
    private Transform player;
    private bool isKnockedBack = false;

    [Header("Detection")]
    public float detectionRange = 8f;
    private bool playerDetected = false;

    [Header("Phase One Data")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 2f;
    public float projectileSpeed = 5f;
    private float shootTimer = 0f;

    [Header("Phase Two Data")]
    public GameObject[] minionPrefabs;
    public float summonCooldown = 2f;
    public int maxMinions = 3;
    private float summonTimer = 0f;
    private int currentMinions = 0;

    public static bool IsDefeated = false;

    [Header("Knockback")]
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (GameManager.Instance != null)
        {
            maxHealth = GameManager.Instance.GetScaledHealth(maxHealth);
            damage = GameManager.Instance.GetScaledDamage(damage);
        }

        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (player == null)
        { 
            return; 
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (!playerDetected)
        {
            if (distance <= detectionRange)
            {
                playerDetected = true;

            }
            else 
            {
                return;

            }
        }

        if (currentHealth <= maxHealth / 2 && currentPhase == BossPhase.PhaseOne)
        {
            currentPhase = BossPhase.PhaseTwo;
            Debug.Log("phase 2");
            AudioManager.Instance?.Play(AudioManager.Instance.bossPhaseTwo);

        }

        if (shootTimer > 0) 
        { 
            shootTimer -= Time.deltaTime;
        }
        if (summonTimer > 0) 
        {
            summonTimer -= Time.deltaTime;
        }

        switch (currentPhase)
        {
            case BossPhase.PhaseOne:
                HandlePhaseOne(); 
                break;
            case BossPhase.PhaseTwo: 
                HandlePhaseTwo();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack || player == null || !playerDetected) 
        {
            return;
        } 
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }
    void HandlePhaseOne()
    {
        if (shootTimer <= 0)
        {
            shootTimer = shootCooldown;
            Shoot();
        }
    }

    void HandlePhaseTwo()
    {
        if (shootTimer <= 0)
        {
            shootTimer = shootCooldown * 0.6f;
            Shoot();
        }

        if (summonTimer <= 0 && currentMinions < maxMinions)
        {
            summonTimer = summonCooldown;
            SummonMinion();
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
            ep.Init(dir, projectileSpeed, damage);
        }
    }

    void SummonMinion()
    {
        if (minionPrefabs.Length == 0) 
        { 
            return; 
        }

        Vector2 spawnOffset = Random.insideUnitCircle.normalized * 2f;
        Vector2 spawnPos = (Vector2)transform.position + spawnOffset;

        int index = Random.Range(0, minionPrefabs.Length);
        var minion = Instantiate(minionPrefabs[index], spawnPos, Quaternion.identity);
        currentMinions++;

        // Track when minion dies to decrement count
        var tracker = minion.AddComponent<MinionTracker>();
        tracker.boss = this;
    }

    public void MinionDied() => currentMinions--;

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        currentHealth -= damage;

        HitStop.Instance?.Stop(0.0015f);
        CameraFollow.Instance?.Shake(.3f, 3.5f);
        AudioManager.Instance?.PlayWithVariation(AudioManager.Instance.bossHit);

        if (currentHealth <= 0)
        {
            Die();
        }
        else 
        { 
            StartCoroutine(Knockback(hitDirection)); 
        }
    }

    private IEnumerator Knockback(Vector2 hitDirection)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSecondsRealtime(knockbackDuration);
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }

    private void Die()
    {
        IsDefeated = true;
        GameManager.Instance?.AddXP(XPValue);
        AudioManager.Instance?.Play(AudioManager.Instance.bossDeath);

        Destroy(gameObject);
    }
}