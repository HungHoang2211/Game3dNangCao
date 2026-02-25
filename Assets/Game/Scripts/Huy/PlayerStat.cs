using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "Scriptable Objects/PlayerStat")]
public class PlayerStat : ScriptableObject
{
    [Header("Stat")]
    public int damage;
    public float health;
    public float speed;
    public int level = 0;


}
