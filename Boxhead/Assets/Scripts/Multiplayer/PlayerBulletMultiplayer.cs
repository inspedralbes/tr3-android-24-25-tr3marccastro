using Mirror;
using UnityEngine;

public class PlayerBulletMultiplayer : NetworkBehaviour
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
        Invoke(nameof(ReturnToPool), lifetime);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(ReturnToPool));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer) return; // Solo el servidor maneja colisiones

        if (collision.gameObject.CompareTag("Zombie"))
        {
            ZombieController enemy = collision.gameObject.GetComponent<ZombieController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageBullet);
            }
        }
        else if (collision.gameObject.CompareTag("DogZombie"))
        {
            DogZombieController enemy = collision.gameObject.GetComponent<DogZombieController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageBullet);
            }
        }

        ReturnToPool();
    }

    void ReturnToPool()
    {
        PoolBulletsManager.Instance.ReturnToPool(gameObject);
    }
}
