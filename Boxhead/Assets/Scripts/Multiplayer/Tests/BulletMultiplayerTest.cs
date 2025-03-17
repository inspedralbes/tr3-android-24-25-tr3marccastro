using UnityEngine;
using Mirror;

public class BulletMultiplayerTest : NetworkBehaviour
{
    public float speed = 10f;
    public float lifespan = 3f;  // Tiempo que la bala vivirá antes de ser destruida

    private void Start()
    {
        // Destruir la bala después de un tiempo (lifetime)
        Destroy(gameObject, lifespan);
    }

    private void Update()
    {
        // Mover la bala hacia adelante
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Destruir la bala al chocar con cualquier objeto
        Destroy(gameObject);
    }
}