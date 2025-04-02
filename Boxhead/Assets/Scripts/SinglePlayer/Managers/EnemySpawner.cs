using UnityEngine;
using System.Collections;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    public Transform[] spawnPoints;
    [SerializeField] private float spawnInterval = 2f;
    public Transform[] waypoints;
    public TextMeshProUGUI numRoundsText, numZombiesText;
    public int maxScreen = 10, maxScreenText;

    private int enemiesKilled = 0;
    private int totalEnemies = 0;
    public int round = 1;
    public int kills = 0;
    public float roundTimer = 0f;
    private bool isPaused = false;
    private bool isSpawning = true;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (numRoundsText != null)
            numRoundsText.text = round.ToString();
        if (numZombiesText != null)
            numZombiesText.text = maxScreen.ToString();

        maxScreenText = maxScreen;
    }

    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    void Update()
    {
        if (!isPaused)
        {
            roundTimer += Time.deltaTime;
        }
    }

    IEnumerator SpawnEnemy()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (totalEnemies >= maxScreen) continue;

            if (waypoints.Length == 0)
            {
                Debug.LogWarning("No hay waypoints asignados.");
                yield break;
            }

            Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];
            GameObject zombie = EnemyPoolManager.Instance.GetEnemy("Zombie", spawnPoint.position, Quaternion.identity);

            if (zombie) totalEnemies++;
            else Debug.LogWarning("No se pudo obtener un Zombie del pool.");

            StartCoroutine(SpawnFatZombieWithDelay());
        }
    }

    public void EnemyDied()
    {
        enemiesKilled++;
        kills++;
        maxScreenText = Mathf.Max(0, maxScreenText - 1);
        numZombiesText.text = maxScreenText.ToString();

        if (enemiesKilled >= maxScreen)
        {
            StartCoroutine(RoundPause());
        }
    }

    IEnumerator RoundPause()
    {
        isSpawning = false;
        WebSocketManager.Instance.SetRoundPause(true, round);
        yield return new WaitForSeconds(3f);
        WebSocketManager.Instance.SetRoundPause(false, round);

        enemiesKilled = 0;
        totalEnemies = 0;
        maxScreen += 2;
        maxScreenText = maxScreen;
        round++;

        numRoundsText.text = round.ToString();
        numZombiesText.text = maxScreenText.ToString();

        isSpawning = true;
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnFatZombieWithDelay()
    {
        yield return new WaitForSeconds(2f);

        if (totalEnemies >= maxScreen) yield break;

        Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];
        EnemyPoolManager.Instance.GetEnemy("FatZombie", spawnPoint.position, Quaternion.identity);
        totalEnemies++;
    }
}