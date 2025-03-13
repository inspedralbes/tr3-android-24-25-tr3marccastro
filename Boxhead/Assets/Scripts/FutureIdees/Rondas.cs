using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
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
    [SerializeField] private int totalEnemies = 0, maxInScreen = 0;
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
            
        for (int i = 0; i < dogZombieCount; i++) {
            AddToPool(Instantiate(prefabDogZombie), dogZombiePool, "DogZombie", i);
        }
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

        if (enemy.TryGetComponent<ZombieController>(out ZombieController zombie))
        {
            zombie.Initialized();
        }
        else if (enemy.TryGetComponent<DogZombieController>(out DogZombieController dogZombie))
        {
            dogZombie.Initialized();
        }
        else
        {
            Debug.LogWarning("El enemigo no tiene un controlador válido: " + enemy.name);
        }

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
        maxInScreen--;
    }

    // 🔴 Corutina modificada para usar los waypoints
    private IEnumerator SpawnEnemies()
    {

        while (true)
        {
            // Verificar si se ha llegado al número máximo de enemigos en esta ronda
            if (maxInScreen == 0)
            {
                Debug.Log("Fin de la ronda " + rounds);
                rounds++;
                totalEnemies = 0;
                yield return new WaitForSeconds(5f); // Pausa entre rondas
                Debug.Log("Comienza la ronda " + rounds);
            }

            yield return new WaitForSeconds(spawnInterval);

            int numberOfEnemiesToSpawn = Random.Range(1, 6);

            for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                if (waypoints.Length == 0)
                {
                    Debug.LogWarning("No hay waypoints asignados.");
                    yield break;
                }

                Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];

                GameObject enemy = null;

                if (rounds % 2 == 1 && totalEnemies <= zombieCount)  // Si la ronda es par, spawn solo Zombies
                {
                    enemy = GetZombie(spawnPoint.position, Quaternion.identity);
                    if (enemy != null)
                    {
                        totalEnemies++;  // Incrementar el contador de zombies vivos
                        maxInScreen++;
                    }
                }
                else if(totalEnemies <= dogZombieCount)  // Si la ronda es impar, spawn solo DogZombies
                {
                    enemy = GetDogZombie(spawnPoint.position, Quaternion.identity);
                    if (enemy != null)
                    {
                        totalEnemies++;  // Incrementar el contador de dogzombies vivos
                        maxInScreen++;
                    }
                }
                else break;
            }
        }
    }
}
*/