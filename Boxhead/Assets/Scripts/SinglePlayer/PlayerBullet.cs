using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    private int damageBullet = 1;
    public ParticleSystem collisionEffect;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            Vector3 position = collision.collider.ClosestPoint(transform.position);

            // Verificamos si collisionEffect está asignado antes de instanciarlo
            if (collisionEffect != null)
            {
                ParticleSystem effect = Instantiate(collisionEffect, position, Quaternion.identity);
                effect.Play();
            }
            else
            {
                Debug.LogError("El sistema de partículas no está asignado en el Inspector.");
            }
        }

        // Lógica para aplicar el daño a los enemigos
        if (collision.gameObject.CompareTag("Zombie"))
        {
            ZombieController enemy = collision.gameObject.GetComponent<ZombieController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageBullet); // Aplica el daño de la bala al enemigo
            }
        }
        else if (collision.gameObject.CompareTag("DogZombie"))
        {
            DogZombieController enemy = collision.gameObject.GetComponent<DogZombieController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageBullet); // Aplica el daño de la bala al enemigo
            }
            else Debug.Log("No existe el componente");
        }

        // Se devuelve al pool la bala después de la colisión
        PoolBulletsManager.Instance.ReturnToPool(gameObject);
    }


    void ReturnToPool()
    {
        PoolBulletsManager.Instance.ReturnToPool(gameObject);
    }
}
