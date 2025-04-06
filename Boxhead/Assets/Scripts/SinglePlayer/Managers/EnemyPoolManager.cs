using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    // Instancia estática del EnemyPoolManager para poder acceder a ella desde otras clases
    public static EnemyPoolManager Instance;

    // Prefabs de los enemigos que se generarán (Zombie y FatZombie)
    public GameObject zombiePrefab;
    public GameObject fatZombiePrefab;

    // Tamaño máximo de los pools de enemigos
    public int zombiePoolSize = 10;
    public int fatZombiePoolSize = 10;

    // Listas donde se almacenarán los enemigos instanciados
    private List<GameObject> zombiePool = new List<GameObject>();
    private List<GameObject> fatZombiePool = new List<GameObject>();

    // Método Awake se ejecuta cuando el objeto es instanciado
    private void Awake()
    {
        // Asegura que solo haya una instancia del EnemyPoolManager
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Método Start se ejecuta al inicio de la ejecución
    private void Start()
    {
        // Si los prefabs de los enemigos son nulos, no hace nada
        if (zombiePrefab == null)
        {
            return;
        }

        if (fatZombiePrefab == null)
        {
            return;
        }

        // Crea los pools de enemigos con la cantidad especificada
        CreateEnemyPool(zombiePoolSize, fatZombiePoolSize);
    }

    // Método para crear los pools de enemigos
    private void CreateEnemyPool(int zombieCount, int fatZombieCount)
    {
        // Crea y agrega zombies al pool de zombies
        for (int i = 0; i < zombieCount; i++)
        {
            AddToPool(Instantiate(zombiePrefab), zombiePool, "Zombie", i);
        }

        // Crea y agrega fat zombies al pool de fat zombies
        for (int i = 0; i < fatZombieCount; i++)
        {
            AddToPool(Instantiate(fatZombiePrefab), fatZombiePool, "FatZombie", i);
        }
    }

    // Método auxiliar para agregar un enemigo a su respectivo pool
    private void AddToPool(GameObject enemy, List<GameObject> pool, string name, int index)
    {
        // Desactiva el enemigo para que no sea visible al principio
        enemy.SetActive(false);

        // Lo establece como hijo del objeto actual para organizarlo en la jerarquía
        enemy.transform.parent = transform;

        // Asigna un nombre único al enemigo
        enemy.name = name + "_" + index;

        // Agrega el enemigo a la lista del pool
        pool.Add(enemy);
    }

    // Método que obtiene un enemigo del pool según su tipo (Zombie o FatZombie)
    public GameObject GetEnemy(string enemyType, Vector3 position, Quaternion rotation)
    {
        // Si no hay enemigos en ambos pools, retorna null
        if (zombiePool.Count <= 0 && fatZombiePool.Count <= 0) return null;

        // Si el tipo de enemigo es nulo o vacío, retorna null
        if (string.IsNullOrEmpty(enemyType))
        {
            return null;
        }

        GameObject enemy = null;

        // Si el tipo de enemigo es "Zombie" y hay zombies en el pool, toma uno
        if (enemyType == "Zombie" && zombiePool.Count > 0)
        {
            enemy = zombiePool[0];
            zombiePool.RemoveAt(0);  // Elimina el zombie del pool
        }
        // Si el tipo de enemigo es "FatZombie" y hay fat zombies en el pool, toma uno
        else if (enemyType == "FatZombie" && fatZombiePool.Count > 0)
        {
            enemy = fatZombiePool[0];
            fatZombiePool.RemoveAt(0);  // Elimina el fat zombie del pool
        }

        // Si se ha encontrado un enemigo, lo coloca en la posición y rotación indicada, y lo activa
        if (enemy != null)
        {
            enemy.transform.SetPositionAndRotation(position, rotation);
            enemy.transform.parent = null;
            enemy.SetActive(true);
        }

        return enemy;
    }

    // Método que devuelve un enemigo al pool después de que haya muerto o ya no se necesite
    public void ReturnToPool(GameObject enemy, bool isZombie)
    {
        // Desactiva el enemigo
        enemy.SetActive(false);

        // Lo establece nuevamente como hijo del objeto actual
        enemy.transform.parent = transform;

        // Resetea su posición y rotación
        enemy.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        // Dependiendo del tipo de enemigo, lo devuelve al pool correspondiente
        if (isZombie)
        {
            zombiePool.Add(enemy);  // Devuelve al pool de zombies
        }
        else
        {
            fatZombiePool.Add(enemy);  // Devuelve al pool de fat zombies
        }
    }
}