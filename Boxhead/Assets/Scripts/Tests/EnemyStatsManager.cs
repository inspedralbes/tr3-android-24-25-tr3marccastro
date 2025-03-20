using UnityEngine;

public class EnemyStatsManager : MonoBehaviour
{
    public static EnemyStatsManager Instance { get; private set; }

    public static EnemyStats ZombieStats;
    public static EnemyStats DogStats;

    private static readonly EnemyStats DefaultZombieStats = new(3, 1f, 10, "FF0000");
    private static readonly EnemyStats DefaultDogStats = new(3, 2f, 15, "FF00D2");

    void Awake()
    {
        // Implementación del Singleton
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

        // Inicializar las estadísticas
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
        ZombieStats = new EnemyStats(DefaultZombieStats.health, DefaultZombieStats.speed, DefaultZombieStats.damage, DefaultZombieStats.color);
        DogStats = new EnemyStats(DefaultDogStats.health, DefaultDogStats.speed, DefaultDogStats.damage, DefaultDogStats.color);

        EnemyStatsPersistence.SaveDefaultEnemyStats();
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
