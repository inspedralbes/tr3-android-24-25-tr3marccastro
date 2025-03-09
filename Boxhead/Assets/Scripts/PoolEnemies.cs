using System.Collections.Generic;
using UnityEngine;

public class PoolEnemies : MonoBehaviour
{
    private static PoolEnemies _instance;
    public static PoolEnemies Instance { get { return _instance; } }

    [SerializeField] private List<GameObject> pool = new List<GameObject>();
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int maxZombies = 10;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < maxZombies; i++)
        {
            GameObject zombie = Instantiate(zombiePrefab);
            zombie.transform.parent = transform;
            zombie.SetActive(false);
            zombie.name = zombiePrefab.tag + "_" + i;
            pool.Add(zombie);
        }
    }

    public GameObject GetFromPool(Vector2 position)
    {
        if (pool.Count > 0)
        {
            GameObject zombie = pool[0];
            pool.RemoveAt(0);
            zombie.transform.position = position;
            zombie.transform.parent = null;
            zombie.SetActive(true);
            return zombie;
        }
        return null;
    }

    public void ReturnToPool(GameObject zombie)
    {
        zombie.SetActive(false);
        zombie.transform.parent = transform;
        zombie.transform.position= Vector2.zero;
        pool.Add(zombie);
    }
}
