using UnityEngine;
using Mirror;

public class BulletMultiplayerTest : NetworkBehaviour
{
    public float speed = 10f;
    public float lifespan = 3f;  // Tiempo que la bala vivir� antes de ser destruida
    public float damage = 10f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Destruir la bala despu�s de un tiempo (lifetime)
        Destroy(gameObject, lifespan);
    }

    private void Update()
    {
        // Mover la bala hacia adelante
        rb.velocity = transform.right * speed;
    }
    
    [Server] // Solo el servidor maneja la colisión
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bala impactó contra: " + collision.gameObject.name);

        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(damage);
            DestroyBullet();
        }
    }

    [Server] // Método seguro para destruir en red
    void DestroyBullet()
    {
        NetworkServer.Destroy(gameObject);
    }
}