using UnityEngine;
using UnityEngine.AI;

public class FatZombieController : MonoBehaviour
{
    private int currentHealth;
    private float currentSpeed;
    private int currentDamage;
    private Transform playerTransform;
    private Animator animator;
    private NavMeshAgent agent;
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
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
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
    }

    private void Die()
    {
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

        if (ColorUtility.TryParseHtmlString("#" + EnemyStatsManager.FatStats.color, out Color newColor))
        {
            enemyRenderer.material.color = newColor;
        }
    }
}
