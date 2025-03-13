using UnityEngine;
using System.Collections;

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
        EnemyPoolManager.Instance.GetEnemy("Zombie", spawnPoint.position, Quaternion.identity);

        StartCoroutine(SpawnDogZombieWithDelay());
    }
    IEnumerator SpawnDogZombieWithDelay()
    {
        // Esperamos 3 segundos
        yield return new WaitForSeconds(3f);

        // Instanciamos el DogZombie después del retraso
        Transform spawnPoint2 = spawnPoints[Random.Range(0, spawnPoints.Length)];
        EnemyPoolManager.Instance.GetEnemy("DogZombie", spawnPoint2.position, Quaternion.identity);
    }

}
