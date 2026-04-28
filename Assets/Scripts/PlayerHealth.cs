using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Invincibility Frames")]
    public float iFramesDuration = 0.5f;
    private bool isInvincible = false;

    private HealthBarUI healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar = FindFirstObjectByType<HealthBarUI>();
        healthBar.SetMaxHealth(maxHealth);
    
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    private System.Collections.IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(iFramesDuration);
        isInvincible = false;
    }

    public void Heal(int healAmount) 
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    private void Die()
    {
        GameManager.Instance?.SaveHighScore();
        GameOverScreen.Show(
            GameManager.Instance.totalXP,
            GameManager.Instance.dungeonLevel,
            GameManager.Instance.highScoreXP,
            GameManager.Instance.highScoreDungeons
        );
    }
}