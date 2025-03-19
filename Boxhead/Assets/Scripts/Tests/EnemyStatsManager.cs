using UnityEngine;

public class EnemyStatsManager : MonoBehaviour
{
    public static EnemyStats ZombieStats = new(3, 1f, 10, "FF0000");
    public static EnemyStats DogStats = new(3, 2f, 15, "FF00D2");

    /*
    void Awake()
    {
        UpdateEnemyStats("Zombie", 3, 1f, 10, "FF0000");
        UpdateEnemyStats("DogZombie", 3, 1f, 10, "FF00D2");
    }
    */

    public void UpdateEnemyStats(string enemyType, int newHealth, float newSpeed, int newDamage, string newColor, int currentRound)
    {
        if (enemyType == "Zombie")
        {
            ZombieStats.health = newHealth;
            ZombieStats.speed = newSpeed;
            ZombieStats.damage = newDamage;
            ZombieStats.color = newColor;
        }
        else if (enemyType == "DogZombie")
        {
            DogStats.health = newHealth;
            DogStats.speed = newSpeed;
            DogStats.damage = newDamage;
            DogStats.color = newColor;
        }
        else {
            ZombieStats.health = Mathf.RoundToInt(ZombieStats.health * Mathf.Pow(1.1f, currentRound));
            ZombieStats.speed = ZombieStats.speed * Mathf.Pow(1.05f, currentRound);
            ZombieStats.damage = Mathf.RoundToInt(ZombieStats.damage * Mathf.Pow(1.1f, currentRound));

            DogStats.health = Mathf.RoundToInt(DogStats.health * Mathf.Pow(1.1f, currentRound));
            DogStats.speed = DogStats.speed * Mathf.Pow(1.05f, currentRound);
            DogStats.damage = Mathf.RoundToInt(DogStats.damage * Mathf.Pow(1.1f, currentRound));
        }
    }
}

[System.Serializable]
public class EnemyStats
{
    public int health;
    public float speed;
    public int damage;
    public string color;

    public EnemyStats(int health, float speed, int damage, string color)
    {
        this.health = health;
        this.speed = speed;
        this.damage = damage;
        this.color = color;
    }
}
