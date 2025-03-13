using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int currentHealth;
    private float currentSpeed;
    private int currentDamage;
    private Rigidbody2D rb;

    void Awake()
    {
        // Buscar el EnemyStatsManager en la escena
        EnemyStatsManager statsManager = FindObjectOfType<EnemyStatsManager>();

        if (statsManager != null)
        {
            // Obtenemos las estadísticas del Manager
            (currentHealth, currentSpeed, currentDamage) = statsManager.GetStats();
            Debug.Log($"Hola gente:222222 Estadísticas iniciales del zombie: Health = {currentHealth}, Speed = {currentSpeed}, Damage = {currentDamage}");
        }
        else
        {
            Debug.LogError("No se encontró el EnemyStatsManager en la escena.");
        }

        // Inicializar el Rigidbody
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        // Aquí puedes agregar la lógica para mover al zombie y demás comportamiento
        // Usamos currentSpeed para mover al enemigo, por ejemplo
        MoveZombie();
    }

    void MoveZombie()
    {
        // Lógica para mover el zombie
        // Por ejemplo, usando currentSpeed para la velocidad
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, -currentSpeed); // Mueve el zombie hacia abajo como ejemplo
        }
    }

    // Método para que el enemigo reciba daño
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
        Debug.Log("El enemigo ha muerto");
        EnemyPoolManager.Instance.ReturnToPool(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si la bala colisiona con un enemigo, le aplica el daño
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Llamamos al método TakeDamage() en el enemigo
            TakeDamage(1);
        }
    }
}
