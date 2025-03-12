using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    private int damageBullet = 1;

    void Update()
    {
        transform.position += speed * Time.deltaTime * transform.right;
    }

    private void OnEnable()
    {
        Invoke("ReturnToPool", lifetime); // Se devuelve al pool después de 3 segundos
    }

    private void OnDisable()
    {
        CancelInvoke("ReturnToPool"); // Cancelamos el temporizador si la bala se desactiva antes
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // Si la bala colisiona con un enemigo, le aplica el daño
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Llamamos al método TakeDamage() en el enemigo
            EnemyStatsManager enemy = collision.gameObject.GetComponent<EnemyStatsManager>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageBullet); // Aplica el daño de la bala al enemigo
            }
        }

        // Se devuelve al pool la bala después de la colisión (con cualquier objeto)
        PoolBullets.Instance.ReturnToPool(gameObject);
    }

    void ReturnToPool()
    {
        PoolBullets.Instance.ReturnToPool(gameObject);
    }
}
