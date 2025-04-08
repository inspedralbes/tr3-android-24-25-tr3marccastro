using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    // Variables internas que controlan el estado del zombi
    private int currentHealth;  // Salud actual del zombi
    private float currentSpeed; // Velocidad actual del zombi
    private int currentDamage;  // Da�o actual del zombi

    // Referencia al jugador
    private Transform playerTransform;  // Transform del jugador para poder seguirlo

    // Componentes necesarios para el funcionamiento del zombi
    private Animator animator;         // Para controlar las animaciones del zombi
    private NavMeshAgent agent;        // Para controlar el movimiento del zombi
    private SpriteRenderer enemyRenderer; // Para controlar el sprite del zombi (flip para miradas)
    private EnemySpawner enemySpawner; // Para manejar la aparici�n del zombi
    private AudioSource audioSource;   // Para reproducir sonidos

    // Clips de sonido para la mordida y el da�o recibido
    [SerializeField] private AudioClip audioBite;  // Sonido de la mordida
    [SerializeField] private AudioClip audioHit;   // Sonido cuando recibe da�o

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
        // Cada vez que el zombi se activa, actualizamos sus estad�sticas
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

        // Reproducimos el sonido de aparici�n del zombi
        audioSource.Play();
    }

    void Update()
    {
        // Si el jugador existe, el zombi lo sigue
        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);  // Establece la posici�n del jugador como destino
            agent.speed = currentSpeed;  // Asigna la velocidad al agente
        }

        // Obtenemos la velocidad del zombi para actualizar la animaci�n
        Vector2 velocity = agent.velocity;

        animator.SetFloat("Horizontal", velocity.x);  // Animaci�n horizontal
        animator.SetFloat("Vertical", velocity.y);    // Animaci�n vertical

        // Guardamos la �ltima direcci�n del movimiento para hacer idle en la direcci�n correcta
        if (velocity.x != 0 || velocity.y != 0)
        {
            animator.SetFloat("LastMoveX", velocity.x);
            animator.SetFloat("LastMoveY", velocity.y);
        }

        // Cambiamos la direcci�n del sprite seg�n el movimiento horizontal
        if (velocity.x < 0)
        {
            enemyRenderer.flipX = false;  // Mirada a la izquierda
        }
        else if (velocity.x > 0)
        {
            enemyRenderer.flipX = true;   // Mirada a la derecha
        }
    }

    // M�todo para recibir da�o
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Reducimos la salud del zombi

        // Si la salud llega a 0 o menos, el zombi muere
        if (currentHealth <= 0)
        {
            Die();  // Llamamos al m�todo Die para que el zombi muera
        }

        // Reproducimos el sonido de da�o recibido
        SoundController.Instance.ExecuteSound(audioHit);
    }

    // M�todo para que el zombi muera y regrese al pool
    private void Die()
    {
        // Retornamos el zombi al pool de enemigos para su reutilizaci�n
        EnemyPoolManager.Instance.ReturnToPool(gameObject, true);

        // Notificamos al EnemySpawner que un enemigo ha muerto
        enemySpawner.EnemyDied();
    }

    // M�todo que maneja la colisi�n con el jugador
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))  // Si colisiona con el jugador
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();  // Obtenemos el controlador del jugador
            if (player != null)
            {
                player.TakeDamagePlayer(currentDamage);  // Hacemos da�o al jugador
            }

            // Reproducimos el sonido de mordida
            SoundController.Instance.ExecuteSound(audioBite);
        }
    }

    // M�todo para actualizar las estad�sticas del zombi desde EnemyStatsManager
    private void UpdateStats()
    {
        // Asignamos las estad�sticas del zombi
        currentHealth = EnemyStatsManager.ZombieStats.health;
        currentSpeed = EnemyStatsManager.ZombieStats.speed;
        currentDamage = EnemyStatsManager.ZombieStats.damage;

        // Cambiamos el color del sprite seg�n las configuraciones de EnemyStatsManager
        if (ColorUtility.TryParseHtmlString("#" + EnemyStatsManager.ZombieStats.color, out Color newColor))
        {
            enemyRenderer.color = newColor;  // Asignamos el nuevo color al sprite
        }
    }
}