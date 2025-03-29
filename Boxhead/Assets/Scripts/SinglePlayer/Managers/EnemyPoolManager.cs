using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance; // Singleton para acceso global

    // Prefabs para los diferentes tipos de enemigos
    public GameObject zombiePrefab;
    public GameObject fatZombiePrefab;

    // Tamaños de los pools para cada tipo de enemigo
    public int zombiePoolSize = 10;
    public int fatZombiePoolSize = 10;

    // Pool de enemigos
    private List<GameObject> zombiePool = new List<GameObject>();
    private List<GameObject> fatZombiePool = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Verificar si los prefabs están asignados
        if (zombiePrefab == null)
        {
            Debug.LogError("Zombie Prefab no asignado en el Inspector.");
            return;
        }

        if (fatZombiePrefab == null)
        {
            Debug.LogError("Fat Zombie Prefab no asignado en el Inspector.");
            return;
        }

        // Crear enemigos iniciales en el pool
        CreateEnemyPool(zombiePoolSize, fatZombiePoolSize);
    }

    // Función para crear los pools de enemigos
    private void CreateEnemyPool(int zombieCount, int fatZombieCount)
    {
        // Crear zombies
        for (int i = 0; i < zombieCount; i++)
        {
            AddToPool(Instantiate(zombiePrefab), zombiePool, "Zombie", i);
        }

        // Crear dog zombies
        for (int i = 0; i < fatZombieCount; i++)
        {
            AddToPool(Instantiate(fatZombiePrefab), fatZombiePool, "FatZombie", i);
        }
    }

    // Función para agregar enemigos al pool
    private void AddToPool(GameObject enemy, List<GameObject> pool, string name, int index)
    {
        // Asegurarse de que el enemigo esté desactivado antes de agregarlo al pool
        enemy.SetActive(false);

        // Hacer que el enemigo sea hijo de este objeto para organizarlo en la jerarquía
        enemy.transform.parent = transform;

        // Asignar un nombre único al enemigo
        enemy.name = name + "_" + index;

        // Agregar el enemigo al pool
        pool.Add(enemy);
    } 

    // Obtener un enemigo del pool por tipo (Zombie o FatZombie)
    public GameObject GetEnemy(string enemyType, Vector3 position, Quaternion rotation)
    {
        if (zombiePool.Count <= 0 && fatZombiePool.Count <= 0) return null;
        
        if (string.IsNullOrEmpty(enemyType))
        {
            Debug.LogError("enemyType no puede ser null o vacío.");
            return null;
        }

        GameObject enemy = null;

        // Seleccionar el pool correcto según el tipo de enemigo
        if (enemyType == "Zombie" && zombiePool.Count > 0)
        {
            enemy = zombiePool[0];
            zombiePool.RemoveAt(0);
        }
        else if (enemyType == "FatZombie" && fatZombiePool.Count > 0)
        {
            enemy = fatZombiePool[0];
            fatZombiePool.RemoveAt(0);
        }

        if (enemy != null)
        {
            enemy.transform.SetPositionAndRotation(position, rotation);
            enemy.transform.parent = null;
            enemy.SetActive(true);
        }

        return enemy;
    }

    // Devolver un enemigo al pool
    public void ReturnToPool(GameObject enemy, bool isZombie)
    {
        enemy.SetActive(false);
        enemy.transform.parent = transform;
        enemy.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        if (isZombie)
        {
            zombiePool.Add(enemy);
        }
        else
        {
            fatZombiePool.Add(enemy);
        }
    }
}