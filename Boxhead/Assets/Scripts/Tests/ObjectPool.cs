using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance; // Instancia del pool
    public GameObject enemyPrefab; // Prefab del enemigo
    public int poolSize = 10; // Tamaño inicial del pool
    private Queue<GameObject> pool; // Cola para manejar los enemigos

    void Awake()
    {
        // Asegurarse de que solo haya una instancia de ObjectPool
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        pool = new Queue<GameObject>();

        // Crear enemigos al principio y desactivarlos
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab);
            obj.SetActive(false); // Desactivarlos inicialmente
            pool.Enqueue(obj); // Agregarlo a la cola del pool
        }
    }

    // Método para obtener un enemigo del pool
    public GameObject GetEnemy()
    {
        if (pool.Count <= 0) return null;
       
        GameObject enemy = pool.Dequeue(); // Obtener un enemigo del pool
        enemy.SetActive(true); // Activarlo
        enemy.GetComponent<Enemy>().Initialize(); // Restaurar sus vidas
        return enemy;
    }

    // Método para devolver un enemigo al pool
    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false); // Desactivarlo
        pool.Enqueue(enemy); // Volver a agregarlo al pool
    }
}
