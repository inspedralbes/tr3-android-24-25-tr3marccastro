using UnityEngine;
using UnityEngine.AI;

public class FatZombieController : MonoBehaviour
{
    // Atributos del enemigo
    private int currentHealth;    // Vida actual del zombie
    private float currentSpeed;   // Velocidad de movimiento del zombie
    private int currentDamage;    // Da�o que hace el zombie al jugador
    private Transform playerTransform;  // Referencia al jugador para seguirlo
    private Animator animator;    // Controlador de animaciones del zombie
    private NavMeshAgent agent;  // Agente de movimiento del zombie
    private SpriteRenderer enemyRenderer;  // Para cambiar la imagen del zombie
    private EnemySpawner enemySpawner; // Referencia al spawner de enemigos
    private AudioSource audioSource;  // Para reproducir sonidos

    [SerializeField] private AudioClip audioBite;  // Sonido del ataque del zombie
    [SerializeField] private AudioClip audioHit;   // Sonido cuando el zombie recibe da�o

    private void Awake()
    {
        // Obtenemos referencias a componentes importantes
        enemySpawner = EnemySpawner.Instance;

        // Verificamos si el spawner de enemigos est� presente
        if (enemySpawner == null)
        {
            Debug.LogError("EnemySpawner no encontrado en la escena.");
        }

        // Inicializamos los componentes necesarios
        enemyRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        // Deshabilitamos las actualizaciones de rotaci�n y eje vertical del agente para un mejor control
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void OnEnable()
    {
        // Actualizamos las estad�sticas del zombie cuando se activa el objeto
        UpdateStats();
    }

    private void Start()
    {
        // Buscamos la referencia del jugador en la escena
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Si no encontramos al jugador, mostramos una advertencia
        if (playerTransform == null)
        {
            Debug.LogWarning("Referencia al jugador no encontrada.");
        }

        // Reproducimos el sonido de inicio (puede ser sonido de aparici�n)
        audioSource.Play();
    }

    void Update()
    {
        // Si tenemos una referencia al jugador, movemos al zombie hacia su posici�n
        if (playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);  // El zombie se dirige hacia el jugador
            agent.speed = currentSpeed;  // Establecemos la velocidad del zombie
        }

        // Obtenemos la velocidad del zombie
        Vector2 velocity = agent.velocity;

        // Actualizamos la animaci�n del zombie de acuerdo con su velocidad
        animator.SetFloat("Horizontal", velocity.x);
        animator.SetFloat("Vertical", velocity.y);

        // Si el zombie se mueve, actualizamos la direcci�n del �ltimo movimiento
        if (velocity.x != 0 || velocity.y != 0)
        {
            animator.SetFloat("LastMoveX", velocity.x);
            animator.SetFloat("LastMoveY", velocity.y);
        }

        // Gestionamos la direcci�n en la que el zombie debe orientarse (voltear en el eje X)
        if (velocity.x < 0)
        {
            enemyRenderer.flipX = false;  // El zombie mira a la izquierda
        }
        else if (velocity.x > 0)
        {
            enemyRenderer.flipX = true;  // El zombie mira a la derecha
        }
    }

    // Funci�n para aplicar da�o al zombie
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;  // Descontamos el da�o de la vida del zombie

        // Si la vida llega a 0 o menos, el zombie muere
        if (currentHealth <= 0)
        {
            Die();
        }

        // Reproducimos el sonido cuando el zombie recibe un golpe
        SoundController.Instance.ExecuteSound(audioHit);
    }

    // Funci�n para la muerte del zombie
    private void Die()
    {
        // Retornamos el zombie al pool de enemigos y actualizamos la cuenta de muertes
        EnemyPoolManager.Instance.ReturnToPool(gameObject, false);
        enemySpawner.EnemyDied();
    }

    // Cuando el zombie colisiona con otro objeto
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el zombie colisiona con el jugador, este recibe da�o
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamagePlayer(currentDamage);  // El zombie inflige da�o al jugador
            }

            // Reproducimos el sonido de la mordida del zombie
            SoundController.Instance.ExecuteSound(audioBite);
        }
    }

    // Actualizamos las estad�sticas del zombie
    private void UpdateStats()
    {
        // Obtenemos las estad�sticas del zombie (vida, velocidad, da�o)
        currentHealth = EnemyStatsManager.FatStats.health;
        currentSpeed = EnemyStatsManager.FatStats.speed;
        currentDamage = EnemyStatsManager.FatStats.damage;

        // Actualizamos el color del zombie si se puede convertir el c�digo de color HTML
        if (ColorUtility.TryParseHtmlString("#" + EnemyStatsManager.FatStats.color, out Color newColor))
        {
            enemyRenderer.material.color = newColor;  // Aplicamos el nuevo color al zombie
        }
    }
}