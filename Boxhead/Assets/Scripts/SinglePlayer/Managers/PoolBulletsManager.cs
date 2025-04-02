using System.Collections.Generic;
using UnityEngine;

public class PoolBulletsManager : MonoBehaviour
{

    private static PoolBulletsManager _instance;

    public static PoolBulletsManager Instance {get {return _instance;}}

    [SerializeField] private List<GameObject> pool = new List<GameObject>();
    
    [SerializeField] private GameObject prefabPool;
    public int maxBullets = 20;

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
            go.transform.parent = transform;
            go.SetActive(false);
            go.name = prefabPool.tag + "_" + i;
            pool.Add(go);
        }
    }

    public GameObject GetFromPool(Vector2 position, Quaternion rotation)
    {
        if (pool.Count <= 0) return null;

        GameObject go = pool[0];
        pool.RemoveAt(0);

        go.transform.position = position;
        go.transform.rotation = rotation;
        go.transform.parent = null;
        go.SetActive(true);

        return go;
    }

    public void ReturnToPool(GameObject bullet)
    {
        bullet.SetActive(false);
        bullet.transform.parent = transform;
        bullet.transform.position = Vector3.zero;
        bullet.transform.rotation = Quaternion.identity;
        pool.Add(bullet);
    }
}