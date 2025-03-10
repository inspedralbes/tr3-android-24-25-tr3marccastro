using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

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
        Vector3 position = collision.collider.ClosestPoint(transform.position);
        PoolBullets.Instance.ReturnToPool(gameObject);
    }

    void ReturnToPool()
    {
        PoolBullets.Instance.ReturnToPool(gameObject);
    }
}
