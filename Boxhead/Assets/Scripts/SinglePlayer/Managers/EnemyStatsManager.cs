using UnityEngine;

public class EnemyStatsManager : MonoBehaviour
{
    // Instancia estática de la clase EnemyStatsManager para asegurar que solo haya una instancia
    public static EnemyStatsManager Instance { get; private set; }

    // Instancias estáticas para almacenar los stats de los enemigos Zombie y FatZombie
    public static EnemyStats ZombieStats;
    public static EnemyStats FatStats;

    // Método que se ejecuta cuando el objeto se inicializa
    void Awake()
    {
        // Si no hay una instancia de EnemyStatsManager, asigna la actual
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No destruye el objeto al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Si ya existe una instancia, destruye este objeto para evitar duplicados
            return;
        }

        // Si el juego no ha sido inicializado previamente, resetea los stats por defecto
        if (!PlayerPrefs.HasKey("GameInitialized"))
        {
            ResetToDefault();
            PlayerPrefs.SetInt("GameInitialized", 1); // Marca que el juego ha sido inicializado
            PlayerPrefs.Save();
        }

        // Carga los stats de los enemigos desde las preferencias guardadas
        LoadEnemyStats();
    }

    // Método para actualizar las estadísticas de un enemigo
    public void UpdateStatsEnemy(string enemyType, int newHealth, float newSpeed, int newDamage, string newColor, bool save)
    {
        // Si el tipo de enemigo es "Zombie", actualiza sus estadísticas
        if (enemyType == "Zombie")
        {
            ZombieStats.health = newHealth;
            ZombieStats.speed = newSpeed;
            ZombieStats.damage = newDamage;
            ZombieStats.color = newColor;
        }
        // Si el tipo de enemigo es "FatZombie", actualiza sus estadísticas
        else if (enemyType == "FatZombie")
        {
            FatStats.health = newHealth;
            FatStats.speed = newSpeed;
            FatStats.damage = newDamage;
            FatStats.color = newColor;
        }

        // Si la opción "save" es verdadera, guarda las estadísticas actualizadas del enemigo
        if (save) EnemyStatsPrefs.SaveEnemyStats(enemyType);
    }

    // Método para actualizar las estadísticas de los enemigos basadas en el número de ronda
    public void UpdateEnemyStatsForRounds(int currentRound)
    {
        // Incrementa la velocidad y daño de los zombies en función del número de la ronda
        ZombieStats.speed = ZombieStats.speed * Mathf.Pow(1.05f, currentRound);
        ZombieStats.damage = Mathf.RoundToInt(ZombieStats.damage * Mathf.Pow(1.1f, currentRound));

        // Incrementa la velocidad y daño de los FatZombies en función del número de la ronda
        FatStats.speed = FatStats.speed * Mathf.Pow(1.05f, currentRound);
        FatStats.damage = Mathf.RoundToInt(FatStats.damage * Mathf.Pow(1.1f, currentRound));
    }

    // Método para cargar las estadísticas de los enemigos desde las preferencias guardadas
    public void LoadEnemyStats()
    {
        EnemyStatsPrefs.LoadEnemyStats();
    }

    // Método para resetear las estadísticas de los enemigos a sus valores por defecto
    public void ResetToDefault()
    {
        EnemyStatsPrefs.SaveDefaultEnemyStats();
        LoadEnemyStats();
    }
}

// Clase para almacenar las estadísticas de un enemigo
[System.Serializable]
public class EnemyStats
{
    public int health;  // Vida del enemigo
    public float speed; // Velocidad del enemigo
    public int damage;  // Daño que hace el enemigo
    public string color; // Color del enemigo (puede ser utilizado para visualización)

    // Constructor para inicializar las estadísticas del enemigo
    public EnemyStats(int health, float speed, int damage, string color)
    {
        this.health = health;
        this.speed = speed;
        this.damage = damage;
        this.color = color;
    }
}