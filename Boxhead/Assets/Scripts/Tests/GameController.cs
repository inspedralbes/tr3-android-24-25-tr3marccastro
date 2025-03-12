using UnityEngine;

public class GameController : MonoBehaviour
{
    public float spawnInterval = 2f; // Intervalo entre enemigos
    public int damageAmount = 1;     // Daño que recibirá cada enemigo

    void Start()
    {
        // Llamar a SpawnEnemy() repetidamente cada cierto tiempo
        InvokeRepeating("SpawnEnemy", 1f, spawnInterval);
    }

    // Método para crear enemigos
    void SpawnEnemy()
    {
        GameObject enemy = ObjectPool.Instance.GetEnemy(); // Obtener un enemigo del pool
        enemy.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f), 0); // Posición aleatoria en la escena
    }

    // Método para hacer que un enemigo reciba daño (por ejemplo, cuando se presiona la tecla espacio)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Cuando se presiona la barra espaciadora
        {
            // Aplica daño a todos los enemigos activos (esto es solo un ejemplo)
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Buscar enemigos por etiqueta
            foreach (var enemy in enemies)
            {
                DamageEnemy(enemy);
            }
        }
    }

    // Método para aplicar daño a un enemigo
    public void DamageEnemy(GameObject enemy)
    {
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.TakeDamage(damageAmount); // Llamar al método TakeDamage()
    }
}
