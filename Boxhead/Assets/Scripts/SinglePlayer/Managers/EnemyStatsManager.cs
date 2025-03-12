using UnityEngine;
using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

public class EnemyStatsManager : MonoBehaviour
{
    public int health = 3;
    public float speed = 5f;
    public int damage = 10;
    private Transform playerTransform; // Referencia al transform del jugador
    private Rigidbody2D rb;

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
            MoveZombie(direction);
        }
    }

    void MoveZombie(Vector2 direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * speed; // Corregido: usar rb.velocity
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
            GetDamage(1);
        }
    }
}