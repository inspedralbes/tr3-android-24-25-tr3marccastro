using UnityEngine;

public class EnemyStatsManager : MonoBehaviour
{
    // Instancia est�tica de la clase EnemyStatsManager para asegurar que solo haya una instancia
    public static EnemyStatsManager Instance { get; private set; }

    // Instancias est�ticas para almacenar los stats de los enemigos Zombie y FatZombie
    public static EnemyStats ZombieStats;
    public static EnemyStats FatStats;

    // M�todo que se ejecuta cuando el objeto se inicializa
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

    // M�todo para actualizar las estad�sticas de un enemigo
    public void UpdateStatsEnemy(string enemyType, int newHealth, float newSpeed, int newDamage, string newColor, bool save)
    {
        // Si el tipo de enemigo es "Zombie", actualiza sus estad�sticas
        if (enemyType == "Zombie")
        {
            ZombieStats.health = newHealth;
            ZombieStats.speed = newSpeed;
            ZombieStats.damage = newDamage;
            ZombieStats.color = newColor;
        }
        // Si el tipo de enemigo es "FatZombie", actualiza sus estad�sticas
        else if (enemyType == "FatZombie")
        {
            FatStats.health = newHealth;
            FatStats.speed = newSpeed;
            FatStats.damage = newDamage;
            FatStats.color = newColor;
        }

        // Si la opci�n "save" es verdadera, guarda las estad�sticas actualizadas del enemigo
        if (save) EnemyStatsPrefs.SaveEnemyStats(enemyType);
    }

    // M�todo para actualizar las estad�sticas de los enemigos basadas en el n�mero de ronda
    public void UpdateEnemyStatsForRounds(int currentRound)
    {
        // Incrementa la velocidad y da�o de los zombies en funci�n del n�mero de la ronda
        ZombieStats.speed = ZombieStats.speed * Mathf.Pow(1.05f, currentRound);
        ZombieStats.damage = Mathf.RoundToInt(ZombieStats.damage * Mathf.Pow(1.1f, currentRound));

        // Incrementa la velocidad y da�o de los FatZombies en funci�n del n�mero de la ronda
        FatStats.speed = FatStats.speed * Mathf.Pow(1.05f, currentRound);
        FatStats.damage = Mathf.RoundToInt(FatStats.damage * Mathf.Pow(1.1f, currentRound));
    }

    // M�todo para cargar las estad�sticas de los enemigos desde las preferencias guardadas
    public void LoadEnemyStats()
    {
        EnemyStatsPrefs.LoadEnemyStats();
    }

    // M�todo para resetear las estad�sticas de los enemigos a sus valores por defecto
    public void ResetToDefault()
    {
        EnemyStatsPrefs.SaveDefaultEnemyStats();
        LoadEnemyStats();
    }
}

// Clase para almacenar las estad�sticas de un enemigo
[System.Serializable]
public class EnemyStats
{
    public int health;  // Vida del enemigo
    public float speed; // Velocidad del enemigo
    public int damage;  // Da�o que hace el enemigo
    public string color; // Color del enemigo (puede ser utilizado para visualizaci�n)

    // Constructor para inicializar las estad�sticas del enemigo
    public EnemyStats(int health, float speed, int damage, string color)
    {
        this.health = health;
        this.speed = speed;
        this.damage = damage;
        this.color = color;
    }
}