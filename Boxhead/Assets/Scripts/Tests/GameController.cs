using UnityEngine;

public class GameController : MonoBehaviour
{
    public float spawnInterval = 2f; // Intervalo entre enemigos
    public int damageAmount = 1;     // Da�o que recibir� cada enemigo

    void Start()
    {
        // Llamar a SpawnEnemy() repetidamente cada cierto tiempo
        InvokeRepeating("SpawnEnemy", 1f, spawnInterval);
    }

    /*
    // M�todo para crear enemigos
    void SpawnEnemy()
    {
        GameObject enemy = ObjectPool.Instance.GetEnemy(); // Obtener un enemigo del pool
        enemy.transform.position = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 3f), 0); // Posici�n aleatoria en la escena
    }
    */

    // M�todo para hacer que un enemigo reciba da�o (por ejemplo, cuando se presiona la tecla espacio)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Cuando se presiona la barra espaciadora
        {
            // Aplica da�o a todos los enemigos activos (esto es solo un ejemplo)
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Buscar enemigos por etiqueta
            foreach (var enemy in enemies)
            {
                DamageEnemy(enemy);
            }
        }
    }

    // M�todo para aplicar da�o a un enemigo
    public void DamageEnemy(GameObject enemy)
    {
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.TakeDamage(damageAmount); // Llamar al m�todo TakeDamage()
    }
}
