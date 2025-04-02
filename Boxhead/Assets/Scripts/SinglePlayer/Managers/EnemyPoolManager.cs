using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance;

    public GameObject zombiePrefab;
    public GameObject fatZombiePrefab;

    public int zombiePoolSize = 10;
    public int fatZombiePoolSize = 10;

    private List<GameObject> zombiePool = new List<GameObject>();
    private List<GameObject> fatZombiePool = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (zombiePrefab == null)
        {
            return;
        }

        if (fatZombiePrefab == null)
        {
            return;
        }

        CreateEnemyPool(zombiePoolSize, fatZombiePoolSize);
    }

    private void CreateEnemyPool(int zombieCount, int fatZombieCount)
    {
        for (int i = 0; i < zombieCount; i++)
        {
            AddToPool(Instantiate(zombiePrefab), zombiePool, "Zombie", i);
        }

        for (int i = 0; i < fatZombieCount; i++)
        {
            AddToPool(Instantiate(fatZombiePrefab), fatZombiePool, "FatZombie", i);
        }
    }

    private void AddToPool(GameObject enemy, List<GameObject> pool, string name, int index)
    {
        enemy.SetActive(false);

        enemy.transform.parent = transform;

        enemy.name = name + "_" + index;

        pool.Add(enemy);
    } 

    public GameObject GetEnemy(string enemyType, Vector3 position, Quaternion rotation)
    {
        if (zombiePool.Count <= 0 && fatZombiePool.Count <= 0) return null;
        
        if (string.IsNullOrEmpty(enemyType))
        {
            return null;
        }

        GameObject enemy = null;

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