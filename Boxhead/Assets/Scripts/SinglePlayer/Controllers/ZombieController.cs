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
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
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
            agent.SetDestination(playerTransform.position);
            agent.speed = currentSpeed;
        }

        Vector2 velocity = agent.velocity;

        animator.SetFloat("Horizontal", velocity.x);
        animator.SetFloat("Vertical", velocity.y);

        if (velocity.x != 0 || velocity.y != 0)
        {
            animator.SetFloat("LastMoveX", velocity.x);
            animator.SetFloat("LastMoveY", velocity.y);
        }

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

        if (ColorUtility.TryParseHtmlString("#" + EnemyStatsManager.ZombieStats.color, out Color newColor))
        {
            enemyRenderer.color = newColor;
        }
    }
}