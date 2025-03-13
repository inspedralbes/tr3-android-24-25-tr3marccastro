using UnityEngine;

public class DogZombieControllerTest : MonoBehaviour
{
    private int currentHealth;
    private float currentSpeed;
    private int currentDamage;
    private Transform playerTransform;
    private Rigidbody2D rb;

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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(1);
        }
    }

    private void UpdateStats()
    {
        // Obtener las estadísticas más recientes del EnemyStatsManager
        currentHealth = EnemyStatsManager.DogStats.health;
        currentSpeed = EnemyStatsManager.DogStats.speed;
        currentDamage = EnemyStatsManager.DogStats.damage;

        Debug.Log("DogZombie actualizado: HP=" + currentHealth + ", Velocidad=" + currentSpeed + ", Daño=" + currentDamage);
    }
}
