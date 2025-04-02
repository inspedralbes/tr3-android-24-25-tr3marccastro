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
        Invoke("ReturnToPool", lifetime);
    }
    private void OnDisable()
    {
        CancelInvoke("ReturnToPool");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            Vector3 position = collision.collider.ClosestPoint(transform.position);

            if (collisionEffect != null)
            {
                ParticleSystem effect = Instantiate(collisionEffect, position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
        }

        if (collision.gameObject.CompareTag("Zombie"))
        {
            ZombieController enemy = collision.gameObject.GetComponent<ZombieController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageBullet);
            }
        }
        else if (collision.gameObject.CompareTag("FatZombie"))
        {
            FatZombieController enemy = collision.gameObject.GetComponent<FatZombieController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageBullet);
            }
        }

        PoolBulletsManager.Instance.ReturnToPool(gameObject);
    }

    void ReturnToPool()
    {
        PoolBulletsManager.Instance.ReturnToPool(gameObject);
    }
}
