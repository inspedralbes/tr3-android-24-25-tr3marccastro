using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // Puntos donde aparecen los enemigos

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 2f, 5f); // Spawnea enemigos cada 5 segundos
    }

    void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        EnemyPoolManager.Instance.GetEnemy(spawnPoint.position, Quaternion.identity);
    }
}
