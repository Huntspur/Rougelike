using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int health;

    private void Update()
    {
        if (health <= 0) 
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(int damage) 
    {
        health -= damage;
    }
}
