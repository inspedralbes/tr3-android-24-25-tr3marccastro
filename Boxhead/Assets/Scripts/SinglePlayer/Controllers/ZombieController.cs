using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    // Variables internas que controlan el estado del zombi
    private int currentHealth;  // Salud actual del zombi
    private float currentSpeed; // Velocidad actual del zombi
    private int currentDamage;  // Daño actual del zombi

    // Referencia al jugador
    private Transform playerTransform;  // Transform del jugador para poder seguirlo

    // Componentes necesarios para el funcionamiento del zombi
    private Animator animator;         // Para controlar las animaciones del zombi
    private NavMeshAgent agent;        // Para controlar el movimiento del zombi
    private SpriteRenderer enemyRenderer; // Para controlar el sprite del zombi (flip para miradas)
    private EnemySpawner enemySpawner; // Para manejar la aparición del zombi
    private AudioSource audioSource;   // Para reproducir sonidos

    // Clips de sonido para la mordida y el daño recibido
    [SerializeField] private AudioClip audioBite;  // Sonido de la mordida
    [SerializeField] private AudioClip audioHit;   // Sonido cuando recibe daño

    private void Awake()
    {
        // Obtenemos la instancia del EnemySpawner
        enemySpawner = EnemySpawner.Instance;
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner no encontrado en la escena.");
        }

        // Obtenemos los componentes necesarios de este GameObject
        enemyRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        // Configuramos el NavMeshAgent para que funcione en 2D
        agent.updateRotation = false; // No rotar el agente
        agent.updateUpAxis = false;   // No ajustar el eje 'up'
    }

    private void OnEnable()
    {
        // Cada vez que el zombi se activa, actualizamos sus estadísticas
        UpdateStats();
    }

    void Start()
    {
        // Encontramos el transform del jugador para poder seguirlo
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (playerTransform == null)
        {
            Debug.LogWarning("Transform del jugador no encontrado.");
        }

        // Reproducimos el sonido de aparición del zombi
        audioSource.Play();
    }

    void Update()
    {
        // Si el jugador existe, el zombi lo sigue
        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);  // Establece la posición del jugador como destino
            agent.speed = currentSpeed;  // Asigna la velocidad al agente
        }

        // Obtenemos la velocidad del zombi para actualizar la animación
        Vector2 velocity = agent.velocity;

        animator.SetFloat("Horizontal", velocity.x);  // Animación horizontal
        animator.SetFloat("Vertical", velocity.y);    // Animación vertical

        // Guardamos la última dirección del movimiento para hacer idle en la dirección correcta
        if (velocity.x != 0 || velocity.y != 0)
        {
            animator.SetFloat("LastMoveX", velocity.x);
            animator.SetFloat("LastMoveY", velocity.y);
        }

        // Cambiamos la dirección del sprite según el movimiento horizontal
        if (velocity.x < 0)
        {
            enemyRenderer.flipX = false;  // Mirada a la izquierda
        }
        else if (velocity.x > 0)
        {
            enemyRenderer.flipX = true;   // Mirada a la derecha
        }
    }

    // Método para recibir daño
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Reducimos la salud del zombi

        // Si la salud llega a 0 o menos, el zombi muere
        if (currentHealth <= 0)
        {
            Die();  // Llamamos al método Die para que el zombi muera
        }

        // Reproducimos el sonido de daño recibido
        SoundController.Instance.ExecuteSound(audioHit);
    }

    // Método para que el zombi muera y regrese al pool
    private void Die()
    {
        // Retornamos el zombi al pool de enemigos para su reutilización
        EnemyPoolManager.Instance.ReturnToPool(gameObject, true);

        // Notificamos al EnemySpawner que un enemigo ha muerto
        enemySpawner.EnemyDied();
    }

    // Método que maneja la colisión con el jugador
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))  // Si colisiona con el jugador
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();  // Obtenemos el controlador del jugador
            if (player != null)
            {
                player.TakeDamagePlayer(currentDamage);  // Hacemos daño al jugador
            }

            // Reproducimos el sonido de mordida
            SoundController.Instance.ExecuteSound(audioBite);
        }
    }

    // Método para actualizar las estadísticas del zombi desde EnemyStatsManager
    private void UpdateStats()
    {
        // Asignamos las estadísticas del zombi
        currentHealth = EnemyStatsManager.ZombieStats.health;
        currentSpeed = EnemyStatsManager.ZombieStats.speed;
        currentDamage = EnemyStatsManager.ZombieStats.damage;

        // Cambiamos el color del sprite según las configuraciones de EnemyStatsManager
        if (ColorUtility.TryParseHtmlString("#" + EnemyStatsManager.ZombieStats.color, out Color newColor))
        {
            enemyRenderer.color = newColor;  // Asignamos el nuevo color al sprite
        }
    }
}