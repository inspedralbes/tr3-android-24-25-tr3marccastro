using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolEnemies : MonoBehaviour
{
    private static PoolEnemies _instance;

    public static PoolEnemies Instance { get { return _instance; } }

    [SerializeField] private List<GameObject> pool = new List<GameObject>();
    [SerializeField] private GameObject prefabPool;
    public int maxEnemies = 20;
    [SerializeField] private float spawnInterval = 1f; // Tiempo entre aparición de enemigos
    [SerializeField] private Vector2 spawnAreaMin;
    [SerializeField] private Vector2 spawnAreaMax;

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
        StartCoroutine(SpawnEnemies()); // Comienza la corutina para generar enemigos
    }

    private void CreatePool(int maxEnemies)
    {
        if (maxEnemies <= 0) return;

        for (int i = 0; i < maxEnemies; i++)
        {
            GameObject go = Instantiate(prefabPool);
            go.transform.parent = transform; // Se asigna como hijo del pool
            go.SetActive(false);
            go.name = prefabPool.tag + "_" + i;
            pool.Add(go); // Agregar el enemigo al pool
        }
    }

    public GameObject GetFromPool(Vector2 position, Quaternion rotation)
    {
        if (pool.Count <= 0) return null;

        GameObject go = pool[0]; // Obtener el primer enemigo disponible
        pool.RemoveAt(0); // Removerlo de la lista del pool

        go.transform.position = position; // Asignar la posición al enemigo
        go.transform.rotation = rotation; // Asignar la rotación
        go.transform.parent = null; // Sacarlo del parent (para evitar problemas)
        go.SetActive(true); // Activar el enemigo

        return go; // Retornar el enemigo
    }

    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false); // Desactivar el enemigo
        enemy.transform.parent = transform; // Reasignar como hijo del pool
        enemy.transform.position = Vector3.zero; // Resetear la posición
        enemy.transform.rotation = Quaternion.identity; // Resetear la rotación
        pool.Add(enemy); // Reagregarlo al pool
    }

    // Corutina para generar enemigos cada 'spawnInterval' segundos
    private IEnumerator SpawnEnemies()
    {
        while (true) // Bucle infinito para spawn continuo
        {
            yield return new WaitForSeconds(spawnInterval); // Esperar el intervalo de tiempo

            int numberOfEnemiesToSpawn = Random.Range(1, 6); // Número aleatorio entre 1 y 5

            for (int i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                Vector2 spawnPosition = new(
                    Random.Range(spawnAreaMin.x, spawnAreaMax.x), // Posición X aleatoria
                    Random.Range(spawnAreaMin.y, spawnAreaMax.y)  // Posición Y aleatoria
                );

                // Obtiene un enemigo del pool y lo coloca en la posición generada
                GameObject enemy = GetFromPool(spawnPosition, Quaternion.identity);
            }
        }
    }
}