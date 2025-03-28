using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    private int currentHealth;
    private float currentSpeed;
    private int currentDamage;
    private Transform playerTransform;
    private Animator animator;
    private NavMeshAgent agent;
    private SpriteRenderer enemyRenderer;
    private EnemySpawner enemySpawner;
    private AudioSource audioSource;
    [SerializeField] private AudioClip audioBite;
    [SerializeField] private AudioClip audioHit;

    private void Awake()
    {
        enemySpawner = EnemySpawner.Instance;
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner no encontrado en la escena.");
        }

        enemyRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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

        audioSource.Play();
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

        SoundController.Instance.ExecuteSound(audioHit);
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

            SoundController.Instance.ExecuteSound(audioBite);
        }
    }

    private void UpdateStats()
    {
        currentHealth = EnemyStatsManager.ZombieStats.health;
        currentSpeed = EnemyStatsManager.ZombieStats.speed;
        currentDamage = EnemyStatsManager.ZombieStats.damage;

        /*
        if (ColorUtility.TryParseHtmlString("#" + EnemyStatsManager.ZombieStats.color, out Color newColor))
        {
            enemyRenderer.color = newColor;
        }
        else
        {
            Debug.LogWarning("Color inválido: " + EnemyStatsManager.ZombieStats.color);
        }
        */

        Debug.Log("Zombie actualizado: HP=" + currentHealth + ", Velocidad=" + currentSpeed + ", Daño=" + currentDamage);
    }
}