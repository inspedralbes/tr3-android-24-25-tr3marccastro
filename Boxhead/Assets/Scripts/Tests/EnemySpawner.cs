using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // Puntos donde aparecen los enemigos
    [SerializeField] private float spawnInterval = 2f;
    public Transform[] waypoints;
    public int maxScreen = 10;

    private int enemiesKilled = 0;
    private int totalEnemies = 0;
    public int round = 1;
    public int kills = 0;

    void Start()
    {
        StartCoroutine(SpawnEnemy()); // Spawnea enemigos cada cierto tiempo
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            // Esperar un intervalo general de zombies
            yield return new WaitForSeconds(spawnInterval);

            if (totalEnemies >= maxScreen)
            {
                continue; // Esperar hasta que haya espacio para más enemigos
            }

            if (waypoints.Length == 0)
            {
                Debug.LogWarning("No hay waypoints asignados.");
                yield break;
            }

            Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];
            GameObject zombie = EnemyPoolManager.Instance.GetEnemy("Zombie", spawnPoint.position, Quaternion.identity);

            if (zombie) totalEnemies++;
            else Debug.LogWarning("No se pudo obtener un Zombie del pool.");

            // Continuamos el spawneo
            StartCoroutine(SpawnDogZombieWithDelay());

            if (round > 10) yield break;
        }
    }

    // Este método debe llamarse cuando un enemigo muere
    public void EnemyDied()
    {
        enemiesKilled++;
        kills++;

        if (enemiesKilled >= maxScreen)
        {
            StartCoroutine(RoundPause());
        }
    }

    // Pausa entre rondas de 10 segundos
    IEnumerator RoundPause()
    {
        // Pausar la ronda, esperar 10 segundos
        WebSocketManager.Instance.SetRoundPause(true, round);  // Pausa y aplica las nuevas estadísticas
        yield return new WaitForSeconds(3f);  // Pausa de 10 segundos
        WebSocketManager.Instance.SetRoundPause(false, round);  // Reanudar la ronda
        enemiesKilled = 0; // Reiniciar el contador
        totalEnemies = 0;
        maxScreen = Mathf.RoundToInt(maxScreen * 1.1f);
        round++;
        Debug.Log("Començando la siguiente ronda " + round);

        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnDogZombieWithDelay()
    {
        // Esperamos 3 segundos
        yield return new WaitForSeconds(3f);

        if(totalEnemies < maxScreen)
        {
            // Instanciamos el DogZombie después del retraso
            Transform spawnPoint = waypoints[Random.Range(0, waypoints.Length)];
            EnemyPoolManager.Instance.GetEnemy("DogZombie", spawnPoint.position, Quaternion.identity);

            totalEnemies++;
        }
        
    }
}
