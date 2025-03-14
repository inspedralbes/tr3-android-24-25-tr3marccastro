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
        if (collision.gameObject.CompareTag("Zombie"))
        {
            // Llamamos al método TakeDamage() en el enemigo
            ZombieController enemy = collision.gameObject.GetComponent<ZombieController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageBullet); // Aplica el daño de la bala al enemigo
            }
        }
        else if(collision.gameObject.CompareTag("DogZombie")) {
            DogZombieController enemy = collision.gameObject.GetComponent<DogZombieController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageBullet); // Aplica el daño de la bala al enemigo
            }
            else Debug.Log("No existe el componente");
        }

        // Se devuelve al pool la bala después de la colisión (con cualquier objeto)
        PoolBulletsManager.Instance.ReturnToPool(gameObject);
    }

    void ReturnToPool()
    {
        PoolBulletsManager.Instance.ReturnToPool(gameObject);
    }
}
