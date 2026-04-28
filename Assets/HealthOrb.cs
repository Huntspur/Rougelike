using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    public int healAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null) 
            {
                health.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
