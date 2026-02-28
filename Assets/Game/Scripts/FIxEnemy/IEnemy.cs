using UnityEngine;

/// <summary>
/// Interface for all enemy types in the game
/// Provides unified damage and health system
/// </summary>
public interface IEnemy
{
    // Combat methods
    void TakeDamage(int damage);
    void Die();

    // Health status
    int GetCurrentHP();
    int GetMaxHP();
    bool IsDead();

    // Transform access
    Transform GetTransform();
}