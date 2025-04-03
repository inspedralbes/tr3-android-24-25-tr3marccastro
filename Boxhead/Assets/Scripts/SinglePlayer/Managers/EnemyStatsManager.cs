using UnityEngine;

public class EnemyStatsManager : MonoBehaviour
{
    public static EnemyStatsManager Instance { get; private set; }

    public static EnemyStats ZombieStats;
    public static EnemyStats FatStats;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (!PlayerPrefs.HasKey("GameInitialized"))
        {
            ResetToDefault();
            PlayerPrefs.SetInt("GameInitialized", 1);
            PlayerPrefs.Save();
        }

        LoadEnemyStats();
    }

    public void UpdateStatsEnemy(string enemyType, int newHealth, float newSpeed, int newDamage, string newColor, bool save)
    {
        if (enemyType == "Zombie")
        {
            ZombieStats.health = newHealth;
            ZombieStats.speed = newSpeed;
            ZombieStats.damage = newDamage;
            ZombieStats.color = newColor;
        }
        else if (enemyType == "FatZombie")
        {
            FatStats.health = newHealth;
            FatStats.speed = newSpeed;
            FatStats.damage = newDamage;
            FatStats.color = newColor;
        }

        if (save) EnemyStatsPrefs.SaveEnemyStats(enemyType);
    }

    public void UpdateEnemyStatsForRounds(int currentRound)
    {
        ZombieStats.speed = ZombieStats.speed * Mathf.Pow(1.05f, currentRound);
        ZombieStats.damage = Mathf.RoundToInt(ZombieStats.damage * Mathf.Pow(1.1f, currentRound));

        FatStats.speed = FatStats.speed * Mathf.Pow(1.05f, currentRound);
        FatStats.damage = Mathf.RoundToInt(FatStats.damage * Mathf.Pow(1.1f, currentRound));
    }

    public void LoadEnemyStats()
    {
        EnemyStatsPrefs.LoadEnemyStats();
    }

    public void ResetToDefault()
    {
        EnemyStatsPrefs.SaveDefaultEnemyStats();
        LoadEnemyStats();
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