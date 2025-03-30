using UnityEngine;
using UnityEngine.AI;  // Asegúrate de importar el namespace de NavMesh.

public class FatZombieController : MonoBehaviour
{
    private int currentHealth;
    private float currentSpeed;
    private int currentDamage;
    private Transform playerTransform;
    private Animator animator;
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
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>(); // Inicializamos el NavMeshAgent
        agent.updateRotation = false;  // No actualiza la rotación, ya que es un juego 2D
        agent.updateUpAxis = false;    // No necesitamos que se mueva en el eje Y
    }

    private void OnEnable()
    {
        UpdateStats();
    }

    private void Start()
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
            agent.speed = currentSpeed; // Establece la velocidad del agente con el zombi
        }

        // Obtener la dirección de movimiento del agente
        Vector2 velocity = agent.velocity;

        // Actualizamos los parámetros del Blend Tree
        animator.SetFloat("Horizontal", velocity.x); // Dirección horizontal (izquierda/derecha)
        animator.SetFloat("Vertical", velocity.y); // Dirección vertical (arriba/abajo)

        // Actualizamos los parámetros LastMoveX y LastMoveY (dirección última conocida)
        if (velocity.x != 0 || velocity.y != 0)
        {
            animator.SetFloat("LastMoveX", velocity.x);  // Guarda la última dirección horizontal
            animator.SetFloat("LastMoveY", velocity.y);
        }

        // Volteamos el sprite si se mueve a la izquierda (opcional si no usas "LastMoveX")
        if (velocity.x < 0)
        {
            enemyRenderer.flipX = false;
        }
        else if (velocity.x > 0)
        {
            enemyRenderer.flipX = true;
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
        Debug.Log("Zombi gordo muerto!");
        EnemyPoolManager.Instance.ReturnToPool(gameObject, false);
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
        currentHealth = EnemyStatsManager.FatStats.health;
        currentSpeed = EnemyStatsManager.FatStats.speed;
        currentDamage = EnemyStatsManager.FatStats.damage;

        /*
        if (ColorUtility.TryParseHtmlString("#" + EnemyStatsManager.DogStats.color, out Color newColor))
        {
            enemyRenderer.material.color = newColor;
        }
        else
        {
            Debug.LogWarning("Color inválido: " + EnemyStatsManager.DogStats.color);
        }
        */

        Debug.Log("DogZombie actualizado: HP=" + currentHealth + ", Velocidad=" + currentSpeed + ", Daño=" + currentDamage);
    }
}
