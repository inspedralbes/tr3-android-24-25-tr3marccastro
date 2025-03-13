using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance; // Singleton para acceso global
    public GameObject enemyPrefab; // Prefab del enemigo
    public int poolSize = 10; // Tamaño del pool
    private Queue<GameObject> enemyPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Crear enemigos iniciales en el pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }

    public GameObject GetEnemy(Vector3 position, Quaternion rotation)
    {
        if (enemyPool.Count > 0)
        {
            GameObject enemy = enemyPool.Dequeue();
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            // Si el pool está vacío, crea uno nuevo (opcional)
            GameObject enemy = Instantiate(enemyPrefab, position, rotation);
            return enemy;
        }
    }

    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
}