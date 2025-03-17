using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PoolBulletsMultiplayer : NetworkBehaviour
{
    private static PoolBulletsMultiplayer _instance;
    public static PoolBulletsMultiplayer Instance { get { return _instance; } }

    [SerializeField] private List<GameObject> pool = new List<GameObject>();
    [SerializeField] private GameObject prefabPool;
    public int maxBullets = 20;

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
        if (isServer) CreatePool(maxBullets); // Solo el servidor crea el pool
    }

    private void CreatePool(int maxBullets)
    {
        for (int i = 0; i < maxBullets; i++)
        {
            GameObject bullet = Instantiate(prefabPool);
            bullet.SetActive(false);
            bullet.transform.parent = transform;
            bullet.name = prefabPool.tag + "_" + i;

            // Asegurarse de que la bala tenga NetworkIdentity
            if (!bullet.GetComponent<NetworkIdentity>())
            {
                bullet.AddComponent<NetworkIdentity>();
            }

            pool.Add(bullet);
            NetworkServer.Spawn(bullet); // Spawn en la red
        }
    }

    public GameObject GetFromPool(Vector2 position, Quaternion rotation)
    {
        if (pool.Count <= 0) return null;

        GameObject bullet = pool[0];
        pool.RemoveAt(0);

        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
        bullet.transform.parent = null;
        bullet.SetActive(true);

        return bullet;
    }

    public void ReturnToPool(GameObject bullet)
    {
        if (!isServer) return;

        bullet.SetActive(false);
        bullet.transform.parent = transform;
        pool.Add(bullet);
    }
}
