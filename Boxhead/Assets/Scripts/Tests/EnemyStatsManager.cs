using UnityEngine;

public class EnemyStatsManager : MonoBehaviour
{
    public static EnemyStats ZombieStats = new(3, 1f, 10);
    public static EnemyStats DogStats = new(5, 2f, 15);

    void Awake()
    {
        UpdateEnemyStats("Zombie", 3, 1f, 10);
        UpdateEnemyStats("DogZombie", 3, 1f, 10);
    }

    public void UpdateEnemyStats(string enemyType, int newHealth, float newSpeed, int newDamage)
    {
        if (enemyType == "Zombie")
        {
            ZombieStats.health = newHealth;
            ZombieStats.speed = newSpeed;
            ZombieStats.damage = newDamage;
        }
        else if (enemyType == "DogZombie")
        {
            DogStats.health = newHealth;
            DogStats.speed = newSpeed;
            DogStats.damage = newDamage;
        }
    }
}

[System.Serializable]
public class EnemyStats
{
    public int health;
    public float speed;
    public int damage;

    public EnemyStats(int health, float speed, int damage)
    {
        this.health = health;
        this.speed = speed;
        this.damage = damage;
    }
}
