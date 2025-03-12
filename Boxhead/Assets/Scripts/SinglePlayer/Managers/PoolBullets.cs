using System.Collections.Generic;
using UnityEngine;

public class PoolBullets : MonoBehaviour
{

    private static PoolBullets _instance;

    public static PoolBullets Instance {get {return _instance;}}

    [SerializeField] private List<GameObject> pool = new List<GameObject>(); // Lista del pool
    
    [SerializeField] private GameObject prefabPool; // Prefab de la bala
    public int maxBullets = 20;   // Máximo de balas en el pool

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

    private void CreatePool(int maxBullets)
    {
        if (maxBullets <= 0) return;

        for (int i = 0; i < maxBullets; i++)
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

    public void ReturnToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        bullet.transform.parent = transform; // Se vuelve a asignar como hijo del pool
        bullet.transform.position = Vector3.zero;
        bullet.transform.rotation = Quaternion.identity;
        pool.Add(bullet); // Reagregar la bala al pool
    }
}