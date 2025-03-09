using UnityEngine;
using Mirror;
using System.Collections;

public class ZombieSpawner : NetworkBehaviour
{
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-10, -10); // Mínimo de la zona de spawn
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(10, 10);   // Máximo de la zona de spawn
    [SerializeField] private float spawnInterval = 2f; // Tiempo entre spawns

    public override void OnStartServer()
    {
        StartCoroutine(SpawnZombiesRoutine());
    }

    [Server]
    private IEnumerator SpawnZombiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int zombiesToSpawn = Random.Range(2, 6); // Número aleatorio entre 2 y 5
            for (int i = 0; i < zombiesToSpawn; i++)
            {
                SpawnZombie();
            }
        }
    }

    [Server]
    private void SpawnZombie()
    {
        Vector2 spawnPosition = new(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        GameObject zombie = PoolEnemies.Instance.GetFromPool(spawnPosition);
        if (zombie != null)
        {
            NetworkServer.Spawn(zombie);
        }
        else
        {
            Debug.LogWarning("No hay zombies disponibles en el pool");
        }
    }
}