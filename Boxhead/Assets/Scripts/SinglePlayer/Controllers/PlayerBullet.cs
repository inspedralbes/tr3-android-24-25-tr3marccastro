using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    // Velocidad de la bala y tiempo de vida antes de regresar al pool
    public float speed = 10f;  // Velocidad a la que se mueve la bala
    public float lifetime = 2f; // Tiempo de vida de la bala antes de ser desactivada

    // Da�o que realiza la bala
    private int damageBullet = 1;

    // Efecto de part�culas que se mostrar� en las colisiones
    public ParticleSystem collisionEffect;

    void Update()
    {
        // Desplaza la bala en la direcci�n "derecha" (transform.right)
        // Multiplicamos la velocidad por Time.deltaTime para que el movimiento sea consistente independientemente de los fotogramas por segundo
        transform.position += speed * Time.deltaTime * transform.right;
    }

    private void OnEnable()
    {
        // Cuando la bala se activa, se programa para regresar al pool despu�s del tiempo de vida (lifetime)
        Invoke("ReturnToPool", lifetime);
    }

    private void OnDisable()
    {
        // Si la bala se desactiva antes de que termine su vida, se cancela la devoluci�n al pool
        CancelInvoke("ReturnToPool");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si la bala colisiona con un objeto, obtenemos la posici�n m�s cercana al punto de colisi�n
        if (collision != null)
        {
            // Obtenemos la posici�n m�s cercana al punto de colisi�n
            Vector3 position = collision.collider.ClosestPoint(transform.position);

            // Si existe un efecto de part�culas, lo creamos en la posici�n de la colisi�n
            if (collisionEffect != null)
            {
                // Instanciamos el efecto de part�culas en la posici�n de la colisi�n
                ParticleSystem effect = Instantiate(collisionEffect, position, Quaternion.identity);
                effect.Play();  // Reproducimos el efecto
                // Destruimos el efecto de part�culas cuando termine su duraci�n
                Destroy(effect.gameObject, effect.main.duration);
            }
        }

        // Si la bala colisiona con un objeto que tenga la etiqueta "Zombie"
        if (collision.gameObject.CompareTag("Zombie"))
        {
            // Obtenemos el componente "ZombieController" del objeto que colision�
            ZombieController enemy = collision.gameObject.GetComponent<ZombieController>();
            if (enemy != null)
            {
                // Si el objeto tiene un "ZombieController", le aplicamos el da�o
                enemy.TakeDamage(damageBullet);
            }
        }
        // Si la bala colisiona con un objeto que tenga la etiqueta "FatZombie"
        else if (collision.gameObject.CompareTag("FatZombie"))
        {
            // Obtenemos el componente "FatZombieController" del objeto que colision�
            FatZombieController enemy = collision.gameObject.GetComponent<FatZombieController>();
            if (enemy != null)
            {
                // Si el objeto tiene un "FatZombieController", le aplicamos el da�o
                enemy.TakeDamage(damageBullet);
            }
        }

        // Independientemente del tipo de colisi�n, regresamos la bala al pool de balas
        PoolBulletsManager.Instance.ReturnToPool(gameObject);
    }

    // Funci�n para regresar la bala al pool despu�s de que haya pasado el tiempo de vida
    void ReturnToPool()
    {
        // Regresamos el objeto de la bala al pool de balas
        PoolBulletsManager.Instance.ReturnToPool(gameObject);
    }
}