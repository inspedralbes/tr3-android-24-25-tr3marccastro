using UnityEngine;

public class EnemyStatsManager : MonoBehaviour
{
    public static EnemyStatsManager Instance { get; private set; }

    public static EnemyStats ZombieStats;
    public static EnemyStats FatStats;

    private static readonly EnemyStats DefaultZombieStats = new(3, 1f, 10);
    private static readonly EnemyStats DefaultFatStats = new(3, 2f, 15);

    void Awake()
    {
        // Implementaci�n del Singleton
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

        // Inicializar las estad�sticas
        if (!PlayerPrefs.HasKey("GameInitialized"))
        {
            Debug.Log("Hola ge");
            ResetToDefault();
            PlayerPrefs.SetInt("GameInitialized", 1);
            PlayerPrefs.Save();
        }

        LoadEnemyStats();
    }

    public void UpdateEnemyStats(string enemyType, int newHealth, float newSpeed, int newDamage, string newColor, bool save, int currentRound)
    {
        if (enemyType == "Zombie")
        {
            ZombieStats.health = newHealth;
            ZombieStats.speed = newSpeed;
            ZombieStats.damage = newDamage;
            // ZombieStats.color = newColor;
        }
        else if (enemyType == "FatZombie")
        {
            FatStats.health = newHealth;
            FatStats.speed = newSpeed;
            FatStats.damage = newDamage;
            // DogStats.color = newColor;
        }
        else {
            ZombieStats.health = Mathf.RoundToInt(ZombieStats.health * Mathf.Pow(1.1f, currentRound));
            ZombieStats.speed = ZombieStats.speed * Mathf.Pow(1.05f, currentRound);
            ZombieStats.damage = Mathf.RoundToInt(ZombieStats.damage * Mathf.Pow(1.1f, currentRound));

            FatStats.health = Mathf.RoundToInt(FatStats.health * Mathf.Pow(1.1f, currentRound));
            FatStats.speed = FatStats.speed * Mathf.Pow(1.05f, currentRound);
            FatStats.damage = Mathf.RoundToInt(FatStats.damage * Mathf.Pow(1.1f, currentRound));
        }

        if (save) SaveData(enemyType);
    }

    public void SaveData(string enemyType)
    {
        EnemyStatsPersistence.SaveEnemyStats(enemyType);
    }

    public void LoadEnemyStats()
    {
        EnemyStatsPersistence.LoadEnemyStats();
    }

    public void ResetToDefault()
    {
        ZombieStats = new EnemyStats(DefaultZombieStats.health, DefaultZombieStats.speed, DefaultZombieStats.damage);
        FatStats = new EnemyStats(DefaultFatStats.health, DefaultFatStats.speed, DefaultFatStats.damage);

        EnemyStatsPersistence.SaveDefaultEnemyStats();
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
