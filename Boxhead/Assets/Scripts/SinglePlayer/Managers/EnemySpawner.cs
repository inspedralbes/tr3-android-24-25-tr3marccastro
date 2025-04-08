using UnityEngine;
using System.Collections;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    // Instancia est�tica del EnemySpawner para poder acceder a ella desde otras clases
    public static EnemySpawner Instance { get; private set; }

    // Puntos de aparici�n de los enemigos
    public Transform[] spawnPoints;

    // Intervalo de tiempo entre las apariciones de los enemigos
    [SerializeField] private float spawnInterval = 2f;

    // Puntos de ruta por donde los enemigos se desplazan despu�s de aparecer
    public Transform[] waypoints;

    // UI para mostrar el n�mero de rondas y la cantidad de enemigos restantes
    public TextMeshProUGUI numRoundsText, numZombiesText;

    // Variables para controlar la cantidad de enemigos en pantalla y el texto en pantalla
    public int maxScreen = 10, maxScreenText;

    // Variables para realizar un seguimiento de los enemigos muertos, los enemigos totales y las rondas
    private int enemiesKilled = 0;
    private int totalEnemies = 0;
    public int round = 1;
    public int kills = 0;
    public float roundTimer = 0f;

    // Flags para controlar si la ronda est� pausada o si los enemigos est�n siendo generados
    private bool isPaused = false;
    private bool isSpawning = true;

    // M�todo Awake se ejecuta cuando el objeto es instanciado
    void Awake()
    {
        // Asegura que solo haya una instancia del spawner de enemigos
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);  // Destruye el objeto si ya existe una instancia
            return;
        }

        // Actualiza el texto en la UI para mostrar la ronda y la cantidad de zombies
        if (numRoundsText != null)
            numRoundsText.text = round.ToString();
        if (numZombiesText != null)
            numZombiesText.text = maxScreen.ToString();

        // Inicializa maxScreenText con el valor de maxScreen
        maxScreenText = maxScreen;
    }

    // M�todo Start se ejecuta al inicio de la ejecuci�n
    void Start()
    {
        // Comienza la rutina de spawn de enemigos
        StartCoroutine(SpawnEnemy());
    }

    // M�todo Update se ejecuta cada frame
    void Update()
    {
        // Solo actualiza el temporizador de la ronda si no est� pausado
        if (!isPaused)
        {
            roundTimer += Time.deltaTime;
        }
    }

    // Corutina que se encarga de generar enemigos en intervalos de tiempo
    IEnumerator SpawnEnemy()
    {
        // Continuar� generando enemigos mientras isSpawning sea verdadero
        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnInterval);  // Espera un intervalo antes de generar un nuevo enemigo

            // Si ya se han alcanzado el l�mite de enemigos en pantalla, no genera m�s
            if (totalEnemies >= maxScreen) continue;

            // Si no hay puntos de ruta, sale de la rutina
            if (waypoints.Length == 0)
            {
                yield break;
            }

            // Elige aleatoriamente un punto de aparici�n
            Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];

            // Obtiene un enemigo de tipo "Zombie" desde el pool de enemigos
            GameObject zombie = EnemyPoolManager.Instance.GetEnemy("Zombie", spawnPoint.position, Quaternion.identity);

            // Si el zombie se gener� correctamente, incrementa el contador de enemigos totales
            if (zombie) totalEnemies++;

            // Inicia la generaci�n de FatZombies con un retraso
            StartCoroutine(SpawnFatZombieWithDelay());
        }
    }

    // M�todo que se llama cuando un enemigo muere
    public void EnemyDied()
    {
        // Incrementa los contadores de enemigos muertos y kills
        enemiesKilled++;
        kills++;

        // Reduce la cantidad de enemigos visibles en la UI
        maxScreenText = Mathf.Max(0, maxScreenText - 1);
        numZombiesText.text = maxScreenText.ToString();

        // Si se han matado suficientes enemigos, pausa la ronda
        if (enemiesKilled >= maxScreen)
        {
            StartCoroutine(RoundPause());
        }
    }

    // Corutina que pausa la ronda por un tiempo y luego la reinicia con nuevos enemigos
    IEnumerator RoundPause()
    {
        // Pausa la generaci�n de enemigos
        isSpawning = false;

        // Informa al WebSocketManager para que se pause la ronda
        WebSocketManager.Instance.SetRoundPause(true, round);

        // Espera 3 segundos antes de continuar
        yield return new WaitForSeconds(3f);

        // Informa al WebSocketManager para que se reanude la ronda
        WebSocketManager.Instance.SetRoundPause(false, round);

        // Reinicia los contadores de enemigos muertos y totales
        enemiesKilled = 0;
        totalEnemies = 0;

        // Aumenta el l�mite de enemigos en pantalla
        maxScreen += 2;
        maxScreenText = maxScreen;

        // Incrementa el n�mero de ronda y actualiza la UI
        round++;
        numRoundsText.text = round.ToString();
        numZombiesText.text = maxScreenText.ToString();

        // Reactiva la generaci�n de enemigos
        isSpawning = true;

        // Reinicia la rutina de generaci�n de enemigos
        StartCoroutine(SpawnEnemy());
    }

    // Corutina que genera un "FatZombie" con un retraso
    IEnumerator SpawnFatZombieWithDelay()
    {
        // Espera 2 segundos antes de generar un "FatZombie"
        yield return new WaitForSeconds(2f);

        // Si ya se ha alcanzado el l�mite de enemigos en pantalla, no genera m�s
        if (totalEnemies >= maxScreen) yield break;

        // Elige aleatoriamente un punto de aparici�n
        Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];

        // Obtiene un enemigo de tipo "FatZombie" desde el pool de enemigos
        EnemyPoolManager.Instance.GetEnemy("FatZombie", spawnPoint.position, Quaternion.identity);

        // Incrementa el contador de enemigos totales
        totalEnemies++;
    }
}