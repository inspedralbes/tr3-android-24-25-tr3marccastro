using UnityEngine;
using UnityEngine.AI;  // Asegúrate de importar el namespace de NavMesh.

public class ZombieController : MonoBehaviour
{
    private int currentHealth;
    private float currentSpeed;
    private int currentDamage;
    private Transform playerTransform;
    private NavMeshAgent agent; // Usamos NavMeshAgent en vez de Rigidbody2D
    private SpriteRenderer enemyRenderer;
    private EnemySpawner enemySpawner;

    private void Awake()
    {
        enemySpawner = EnemySpawner.Instance;
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner no encontrado en la escena.");
        }

        enemyRenderer = GetComponent<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>(); // Inicializamos el NavMeshAgent
        agent.updateRotation = false;  // No actualiza la rotación, ya que es un juego 2D
        agent.updateUpAxis = false;    // No necesitamos que se mueva en el eje Y
    }

    private void OnEnable()
    {
        UpdateStats();
    }

    void Start()
    {
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
            agent.SetDestination(playerTransform.position);  // El agente va hacia el jugador
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
        EnemyPoolManager.Instance.ReturnToPool(gameObject, true);
        enemySpawner.EnemyDied();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamagePlayer(currentDamage);
            }
        }
    }

    private void UpdateStats()
    {
        currentHealth = EnemyStatsManager.ZombieStats.health;
        currentSpeed = EnemyStatsManager.ZombieStats.speed;
        currentDamage = EnemyStatsManager.ZombieStats.damage;

        if (ColorUtility.TryParseHtmlString("#" + EnemyStatsManager.ZombieStats.color, out Color newColor))
        {
            enemyRenderer.color = newColor;
        }
        else
        {
            Debug.LogWarning("Color inválido: " + EnemyStatsManager.ZombieStats.color);
        }

        Debug.Log("Zombie actualizado: HP=" + currentHealth + ", Velocidad=" + currentSpeed + ", Daño=" + currentDamage);
    }
}
