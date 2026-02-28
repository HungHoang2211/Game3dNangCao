using UnityEngine;

/// <summary>
/// Interface for all enemy types in the game
/// Provides unified damage and health system
/// </summary>
public interface IEnemy
{
    // Combat methods
    void TakeDamage(float damage); // ← CHANGED: int → float
    void Die();

    // Health status
    float GetCurrentHP(); // ← CHANGED: int → float
    float GetMaxHP(); // ← CHANGED: int → float
    bool IsDead();

    // Transform access
    Transform GetTransform();
}