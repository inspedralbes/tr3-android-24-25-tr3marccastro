using UnityEngine;
using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

public class DogZombieController : MonoBehaviour
{
    public int health = 1;
    private int currentHealth;
    public float speed = 5;
    private float currentspeed;
    public int damage = 20;
    private int currentdamage;
    private Transform playerTransform; // Referencia al transform del jugador
    private Rigidbody2D rb;

    private void OnEnable()
    {
        Initialized(); // Restaurar vida al activarse
        UpdateStats(health, speed, damage); // Recibe nuevas estadísticas en tiempo real
    }

    public void Initialized()
    {
        currentHealth = health;
        currentspeed = speed;
        currentdamage = damage;
    }

    void Start()
    {
        // Inicializamos Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        
        // Verificamos si el Player Transform está asignado
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform == null)
        {
            Debug.LogWarning("Player transform no encontrado.");
        }
    }
    void Update()
    {
        // Asegurarnos de que playerTransform esté asignado
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
            rb.linearVelocity = direction * currentspeed; // Corregido: usar rb.velocity
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die(); // Si la salud llega a 0, el enemigo muere
        }
    }

    private void Die()
    {
        // Puedes agregar lógica adicional aquí, como animaciones o efectos
        Debug.Log("Enemigo muerto!");

        // Devolver el enemigo al pool para reutilizarlo
        PoolEnemies.Instance.ReturnToPool(gameObject, false); // Asegúrate de tener un pool para los enemigos también
    }

    public void UpdateStats(int newHealth, float newSpeed, int newDamage)
    {
        if (newHealth > 0) health = newHealth;
        if (newSpeed > 0) speed = newSpeed;
        if (newDamage > 0) damage = newDamage;

        Debug.Log("Estadísticas actualizadas de DogZombieController: Health = " + health + ", Speed = " + speed + ", Damage = " + damage);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si la bala colisiona con un enemigo, le aplica el daño
        if (collision.gameObject.CompareTag("Player"))
        {
            // Llamamos al método TakeDamage() en el enemigo
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamagePlayer(currentdamage); // Aplica el daño de la bala al enemigo
            }
        }
    }
}