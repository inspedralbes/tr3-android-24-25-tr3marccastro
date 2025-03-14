using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // Puntos donde aparecen los enemigos
    [SerializeField] private float spawnInterval = 1f;
    public Transform[] waypoints;

    void Start()
    {
        StartCoroutine(SpawnEnemy()); // Spawnea enemigos cada 5 segundos
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            // Esperar un intervalo general de zombies
            yield return new WaitForSeconds(spawnInterval);

            // Generar un n�mero aleatorio de zombies (entre 1 y 6 por spawn)
            int zombiesToSpawn = Random.Range(3, 6);

            // Spawnear los zombies
            for (int i = 0; i < zombiesToSpawn; i++)
            {
                if (waypoints.Length == 0)
                {
                    Debug.LogWarning("No hay waypoints asignados.");
                    yield break;
                }

                Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];
                EnemyPoolManager.Instance.GetEnemy("Zombie", spawnPoint.position, Quaternion.identity);
                
                /*
                if (!zombie)
                {
                    Debug.LogWarning("No se pudo obtener un Zombie del pool.");
                    continue; // Sigue intentando con los demás
                }
                */
            }

            StartCoroutine(SpawnDogZombieWithDelay());
        }
    }

    IEnumerator SpawnDogZombieWithDelay()
    {
        // Esperamos 3 segundos
        yield return new WaitForSeconds(3f);

        // Instanciamos el DogZombie despu�s del retraso
        Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];
        EnemyPoolManager.Instance.GetEnemy("DogZombie", spawnPoint.position, Quaternion.identity);
    }

}
