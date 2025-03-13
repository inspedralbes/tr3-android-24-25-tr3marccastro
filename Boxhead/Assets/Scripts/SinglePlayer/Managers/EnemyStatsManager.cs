using UnityEngine;

public class EnemyStatsManager : MonoBehaviour
{
    public int health = 3;
    public float speed = 1f;
    public int damage = 10;

    public void UpdateStats(int newHealth, float newSpeed, int newDamage)
    {
        health = newHealth;
        speed = newSpeed;
        damage = newDamage;

        Debug.Log($"Estadísticas actualizadas: Health = {health}, Speed = {speed}, Damage = {damage}");
    }

    public (int health, float speed, int damage) GetStats()
    {
        return (health, speed, damage);
    }
}
