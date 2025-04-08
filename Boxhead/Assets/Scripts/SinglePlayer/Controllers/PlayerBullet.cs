using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    // Velocidad de la bala y tiempo de vida antes de regresar al pool
    public float speed = 10f;  // Velocidad a la que se mueve la bala
    public float lifetime = 2f; // Tiempo de vida de la bala antes de ser desactivada

    // Daño que realiza la bala
    private int damageBullet = 1;

    // Efecto de partículas que se mostrará en las colisiones
    public ParticleSystem collisionEffect;

    void Update()
    {
        // Desplaza la bala en la dirección "derecha" (transform.right)
        // Multiplicamos la velocidad por Time.deltaTime para que el movimiento sea consistente independientemente de los fotogramas por segundo
        transform.position += speed * Time.deltaTime * transform.right;
    }

    private void OnEnable()
    {
        // Cuando la bala se activa, se programa para regresar al pool después del tiempo de vida (lifetime)
        Invoke("ReturnToPool", lifetime);
    }

    private void OnDisable()
    {
        // Si la bala se desactiva antes de que termine su vida, se cancela la devolución al pool
        CancelInvoke("ReturnToPool");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si la bala colisiona con un objeto, obtenemos la posición más cercana al punto de colisión
        if (collision != null)
        {
            // Obtenemos la posición más cercana al punto de colisión
            Vector3 position = collision.collider.ClosestPoint(transform.position);

            // Si existe un efecto de partículas, lo creamos en la posición de la colisión
            if (collisionEffect != null)
            {
                // Instanciamos el efecto de partículas en la posición de la colisión
                ParticleSystem effect = Instantiate(collisionEffect, position, Quaternion.identity);
                effect.Play();  // Reproducimos el efecto
                // Destruimos el efecto de partículas cuando termine su duración
                Destroy(effect.gameObject, effect.main.duration);
            }
        }

        // Si la bala colisiona con un objeto que tenga la etiqueta "Zombie"
        if (collision.gameObject.CompareTag("Zombie"))
        {
            // Obtenemos el componente "ZombieController" del objeto que colisionó
            ZombieController enemy = collision.gameObject.GetComponent<ZombieController>();
            if (enemy != null)
            {
                // Si el objeto tiene un "ZombieController", le aplicamos el daño
                enemy.TakeDamage(damageBullet);
            }
        }
        // Si la bala colisiona con un objeto que tenga la etiqueta "FatZombie"
        else if (collision.gameObject.CompareTag("FatZombie"))
        {
            // Obtenemos el componente "FatZombieController" del objeto que colisionó
            FatZombieController enemy = collision.gameObject.GetComponent<FatZombieController>();
            if (enemy != null)
            {
                // Si el objeto tiene un "FatZombieController", le aplicamos el daño
                enemy.TakeDamage(damageBullet);
            }
        }

        // Independientemente del tipo de colisión, regresamos la bala al pool de balas
        PoolBulletsManager.Instance.ReturnToPool(gameObject);
    }

    // Función para regresar la bala al pool después de que haya pasado el tiempo de vida
    void ReturnToPool()
    {
        // Regresamos el objeto de la bala al pool de balas
        PoolBulletsManager.Instance.ReturnToPool(gameObject);
    }
}