using UnityEngine;

public class DogZombieController : MonoBehaviour
{
    private int currentHealth;
    private float currentSpeed;
    private int currentDamage;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private Renderer enemyRenderer;
    private EnemySpawner enemySpawner;

    private void Awake()
    {
        enemySpawner = EnemySpawner.Instance;

        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner no encontrado en la escena.");
        }

        enemyRenderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        // Cargar estadísticas cada vez que el enemigo se activa
        UpdateStats();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (playerTransform == null)
        {
            Debug.LogWarning("Player transform no encontrado.");
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            MoveDogZombie(direction);
        }
    }

    void MoveDogZombie(Vector2 direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * currentSpeed;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Perro zombie muerto!");
        EnemyPoolManager.Instance.ReturnToPool(gameObject, false);
        enemySpawner.EnemyDied();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si la bala colisiona con un enemigo, le aplica el daño
        if (collision.gameObject.CompareTag("Player"))
        {
            // Llamamos al método TakeDamage() en el enemigo
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamagePlayer(currentDamage); // Aplica el daño de la bala al enemigo
            }
        }
    }

    private void UpdateStats()
    {
        // Obtener las estadísticas más recientes del EnemyStatsManager
        currentHealth = EnemyStatsManager.DogStats.health;
        currentSpeed = EnemyStatsManager.DogStats.speed;
        currentDamage = EnemyStatsManager.DogStats.damage;

        // Convertir el color hexadecimal a un Color de Unity
        if (ColorUtility.TryParseHtmlString("#" + EnemyStatsManager.DogStats.color, out Color newColor))  // Asegúrate de agregar el `#` para que sea válido
        {
            enemyRenderer.material.color = newColor;
        }
        else
        {
            Debug.LogWarning("Color inválido: " + EnemyStatsManager.DogStats.color);
        }

        Debug.Log("DogZombie actualizado: HP=" + currentHealth + ", Velocidad=" + currentSpeed + ", Daño=" + currentDamage);
    }
}
