using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage, Vector2 hitDirection);
}

public interface IXPSource
{
    int XPValue 
    { 
        get; 
    }
}