using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int health = 3; // Salud del zombie
    public float speed = 3f; // Velocidad de movimiento del zombie
    public float damage = 10f;
    private Transform playerTransform; // Referencia al transform del jugador

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Buscar al jugador en la escena por su tag
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            // Calcular la dirección hacia el jugador
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // Mover al zombie hacia el jugador
            MoveZombie(direction);
        }
    }

    void MoveZombie(Vector2 direction)
    {
        // Mover el zombie utilizando Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            PoolEnemies.Instance.ReturnToPool(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Cuando el zombie colisiona, recibe 1 de daño
            GetDamage(1);
        }
    }
}