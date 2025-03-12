using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolEnemies : MonoBehaviour
{
    private static PoolEnemies _instance;
    public static PoolEnemies Instance { get { return _instance; } }

    [SerializeField] private List<GameObject> pool = new List<GameObject>();
    [SerializeField] private GameObject prefabPool;
    public int maxEnemies = 2;
    [SerializeField] private float spawnInterval = 1f; // Tiempo entre spawns
    public int totalEnemies = 0;
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
        CreatePool(maxEnemies); // Crea los enemigos en el pool
        StartCoroutine(SpawnEnemies()); // Comienza la corutina de spawn
    }

    private void CreatePool(int maxEnemies)
    {
        if (maxEnemies <= 0) return;

        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject go = Instantiate(prefabPool);
            go.transform.parent = transform;
            go.SetActive(false);
            go.name = prefabPool.tag + "_" + i;
            pool.Add(go);
        }
    }

    public GameObject GetFromPool(Vector2 position, Quaternion rotation)
    {
        if (pool.Count <= 0) return null;

        GameObject enemy = pool[0];
        pool.RemoveAt(0);

        //enemy.GetComponent<EnemyStatsManager>().Initialized();
        enemy.transform.SetPositionAndRotation(position, rotation);
        enemy.transform.parent = null;
        enemy.SetActive(true);

        return enemy;
    }

    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        enemy.transform.parent = transform;
        enemy.transform.position = Vector3.zero;
        enemy.transform.rotation = Quaternion.identity;

        pool.Add(enemy);
    }

    // 🔴 Corutina modificada para usar los waypoints
    private IEnumerator SpawnEnemies()
    {
        while (true) // Bucle infinito controlado internamente
        {
            yield return new WaitForSeconds(spawnInterval); // Espera antes de spawnear nuevos enemigos

            int numberOfEnemiesToSpawn = Random.Range(1, 6);

            for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                // if (totalEnemies >= maxEnemies) break; // Evita exceder el límite de enemigos

                if (waypoints.Length == 0) 
                {
                    Debug.LogWarning("No hay waypoints asignados en el PoolEnemies.");
                    yield break;
                }

                // Seleccionar un waypoint aleatorio
                Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];
                
                // Obtener un enemigo del pool
                GameObject enemy = GetFromPool(spawnPoint.position, Quaternion.identity);
                //if (enemy != null)
                //{
                //    totalEnemies++;
                //}
            }
        }
    }
}