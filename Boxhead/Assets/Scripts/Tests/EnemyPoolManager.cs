using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance; // Singleton para acceso global

    // Prefabs para los diferentes tipos de enemigos
    public GameObject zombiePrefab;
    public GameObject dogZombiePrefab;

    // Tamaños de los pools para cada tipo de enemigo
    public int zombiePoolSize = 10;
    public int dogZombiePoolSize = 10;

    // Pool de enemigos
    private List<GameObject> zombiePool = new List<GameObject>();
    private List<GameObject> dogZombiePool = new List<GameObject>();

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

        if (dogZombiePrefab == null)
        {
            Debug.LogError("Dog Zombie Prefab no asignado en el Inspector.");
            return;
        }

        // Crear enemigos iniciales en el pool
        CreateEnemyPool(zombiePoolSize, dogZombiePoolSize);
    }

    // Función para crear los pools de enemigos
    private void CreateEnemyPool(int zombieCount, int dogZombieCount)
    {
        // Crear zombies
        for (int i = 0; i < zombieCount; i++)
        {
            AddToPool(Instantiate(zombiePrefab), zombiePool, "Zombie", i);
        }

        // Crear dog zombies
        for (int i = 0; i < dogZombieCount; i++)
        {
            AddToPool(Instantiate(dogZombiePrefab), dogZombiePool, "DogZombie", i);
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

    // Obtener un enemigo del pool por tipo (Zombie o DogZombie)
    public GameObject GetEnemy(string enemyType, Vector3 position, Quaternion rotation)
    {
        if (zombiePool.Count <= 0 && dogZombiePool.Count <= 0) return null;
        
        if (string.IsNullOrEmpty(enemyType))
        {
            Debug.LogError("enemyType no puede ser null o vacío.");
            return null;
        }

        /*
        if (zombiePool == null || zombiePool.Count == 0)
        {
            Debug.LogError("El pool de enemigos está vacío o no se ha inicializado.");
            return null;
        }
        */

        GameObject enemy = null;

        // Seleccionar el pool correcto según el tipo de enemigo
        if (enemyType == "Zombie" && zombiePool.Count > 0)
        {
            enemy = zombiePool[0];
            zombiePool.RemoveAt(0);
        }
        else if (enemyType == "DogZombie" && dogZombiePool.Count > 0)
        {
            enemy = dogZombiePool[0];
            dogZombiePool.RemoveAt(0);
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
        enemy.transform.position = Vector3.zero;
        enemy.transform.rotation = Quaternion.identity;

        if (isZombie)
        {
            zombiePool.Add(enemy);
        }
        else
        {
            dogZombiePool.Add(enemy);
        }
    }
}