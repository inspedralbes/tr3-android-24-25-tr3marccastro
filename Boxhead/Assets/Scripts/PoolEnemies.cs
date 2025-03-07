using System.Collections.Generic;
using UnityEngine;

public class PoolEnemies : MonoBehaviour
{

    private static PoolEnemies _instance;

    public static PoolEnemies Instance {get {return _instance;}}

    [SerializeField] private List<GameObject> pool = new List<GameObject>();
    
    [SerializeField] private GameObject prefabPool;
    public int maxEnemies = 20;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        CreatePool(maxBullets);
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
            pool.Add(go); // Agregar la bala al pool
        }
    }

    public GameObject GetFromPool(Vector2 position, Quaternion rotation)
    {
        if (pool.Count <= 0) return null;

        GameObject go = pool[0]; // Obtener la primera bala
        pool.RemoveAt(0); // Removerla de la lista

        go.transform.position = position;
        go.transform.rotation = rotation;
        go.transform.parent = null; // Se saca del parent (para evitar problemas)
        go.SetActive(true); // Activar la bala

        return go; // Retornar la bala
    }

    public void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        enemy.transform.parent = transform; // Se vuelve a asignar como hijo del pool
        enemy.transform.position = Vector3.zero;
        enemy.transform.rotation = Quaternion.identity;
        pool.Add(enemy); // Reagregar la bala al pool
    }
}