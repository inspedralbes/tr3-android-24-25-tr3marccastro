using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolEnemies : MonoBehaviour
{
    private static PoolEnemies _instance;
    public static PoolEnemies Instance { get { return _instance; } }

    private List<GameObject> zombiePool = new List<GameObject>();
    private List<GameObject> dogZombiePool = new List<GameObject>();
    [SerializeField] private GameObject prefabZombie;
    [SerializeField] private GameObject prefabDogZombie;
    public int zombieCount = 10;
    public int dogZombieCount = 5;
    // [SerializeField] private int totalEnemies = 0;
    [SerializeField] private float spawnInterval = 1f; // Tiempo entre spawns
    public int rounds = 0;

    // 🔴 Lista de waypoints donde aparecerán los enemigos
    public Transform[] waypoints;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        CreatePool(zombieCount, dogZombieCount); // Crea los enemigos en el pool
        StartCoroutine(SpawnEnemies()); // Comienza la corutina de spawn
    }

    private void CreatePool(int maxEnemies, int dogZombieCount)
    {
        if (zombieCount <= 0) return;

        for (int i = 0; i < zombieCount; i++) {
            AddToPool(Instantiate(prefabZombie), zombiePool, "Zombie", i);
        }
        /* 
        for (int i = 0; i < dogZombieCount; i++) {
            AddToPool(Instantiate(prefabDogZombie), dogZombiePool, "DogZombie", i);
        }
        */
    }

    private void AddToPool(GameObject enemy, List<GameObject> pool, string name, int index)
    {
        enemy.SetActive(false);
        enemy.transform.parent = transform;
        enemy.SetActive(false);
        enemy.name = name + "_" + index;
        pool.Add(enemy);
    }

    public GameObject GetZombie(Vector2 position, Quaternion rotation)
    {
        return GetFromPool(zombiePool, position, rotation);
    }

    public GameObject GetDogZombie(Vector2 position, Quaternion rotation)
    {
        return GetFromPool(dogZombiePool, position, rotation);
    }

    public GameObject GetFromPool(List<GameObject> pool, Vector2 position, Quaternion rotation)
    {
        if (pool.Count <= 0) return null;

        GameObject enemy = pool[0];
        pool.RemoveAt(0);

        /*
        if (enemy.TryGetComponent<ZombieController>(out ZombieController zombie))
        {
            zombie.InitializeStats();
        }
        else if (enemy.TryGetComponent<DogZombieController>(out DogZombieController dogZombie))
        {
            dogZombie.InitializeStats();
        }
        else
        {
            Debug.LogWarning("El enemigo no tiene un controlador válido: " + enemy.name);
        }
        */

        enemy.transform.SetPositionAndRotation(position, rotation);
        enemy.transform.parent = null;
        enemy.SetActive(true);

        return enemy;
    }

    public void ReturnToPool(GameObject enemy, bool isZombie)
    {
        enemy.SetActive(false);
        enemy.transform.parent = transform;
        enemy.transform.position = Vector3.zero;
        enemy.transform.rotation = Quaternion.identity;

        if(isZombie) {
            zombiePool.Add(enemy);
        }
        else {
            dogZombiePool.Add(enemy);
        }
    }

    // 🔴 Corutina modificada para usar los waypoints
    private IEnumerator SpawnEnemies()
    {
        float dogZombieSpawnTime = 0f;  // Temporizador para el spawn de dogzombies

        while (true)
        {
            // Aumentar el tiempo para el spawn de dogzombies
            dogZombieSpawnTime += spawnInterval;

            // Esperar un intervalo general de zombies
            yield return new WaitForSeconds(spawnInterval);

            // Generar un número aleatorio de zombies (entre 1 y 6 por spawn)
            int zombiesToSpawn = Random.Range(3, 6);

            // Spawnear los zombies
            for (int i = 0; i < zombiesToSpawn; i++)
            {
                if (waypoints.Length == 0)
                {
                    Debug.LogWarning("No hay waypoints asignados.");
                    yield break;
                }

                Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];
                GameObject zombie = GetZombie(spawnPoint.position, Quaternion.identity);
            }
            /*
            // Si ha pasado el tiempo suficiente para spawn de dogzombies, generar dogzombies
            if (dogZombieSpawnTime >= 5f)  // 5 segundos para spawn de dogzombies, puedes ajustar este valor
            {
                // Generar entre 0 y la cantidad máxima de dogzombies
                int dogZombiesToSpawn = Random.Range(0, dogZombieCount + 1); // de 0 a dogZombieCount

                // Spawnear los dogzombies
                for (int i = 0; i < dogZombiesToSpawn; i++)
                {
                    if (waypoints.Length == 0)
                    {
                        Debug.LogWarning("No hay waypoints asignados.");
                        yield break;
                    }

                    Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];
                    GameObject dogZombie = GetDogZombie(spawnPoint.position, Quaternion.identity);
                }

                // Resetear el temporizador para el siguiente ciclo de dogzombies
                dogZombieSpawnTime = 0f;
            }
            */
        }
    }
}