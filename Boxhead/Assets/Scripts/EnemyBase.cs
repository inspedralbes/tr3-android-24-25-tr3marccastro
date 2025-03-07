using UnityEngine;
using Mirror;

public class EnemyBase : NetworkBehaviour
{
    [SerializeField] private int _health = 3; // Salud del zombie
    [SerializeField] private float moveSpeed = 3f; // Velocidad de movimiento del zombie
    private Transform playerTransform; // Referencia al transform del jugador

    void Start()
    {
        if (isServer)
        {
            // Buscar al jugador en la escena por su tag
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (playerTransform != null && isServer)
        {
            // Calcular la dirección hacia el jugador
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // Mover al zombie hacia el jugador
            MoveZombie(direction);
        }
    }

    // Movimiento del zombie
    void MoveZombie(Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
    }

    // Función de daño, ejecutada en el servidor
    [Server]
    public void GetDamage(int damage)
    {
        _health -= damage;

        // Si la salud llega a 0 o menos, el zombie muere
        if (_health <= 0)
        {
            // Volver al pool en lugar de destruir el objeto
            PoolEnemies.Instance.ReturnToPool(gameObject);
        }
    }

    // Colisiones, solo ejecutadas por el servidor
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isServer)
        {
            // Cuando el zombie colisiona, recibe 1 de daño
            GetDamage(1);
        }
    }
}