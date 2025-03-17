using UnityEngine;
using Mirror;

public class PlayerControllerMultiplayer : NetworkBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    public Transform firePoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveX, moveY) * speed;
        rb.linearVelocity = movement;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (Input.GetMouseButtonDown(0))
        {
            CmdShoot(mousePosition);
        }
    }

    /*
    [Command]
    void CmdShoot(Vector3 targetPosition)
    {
        GameObject bullet = PoolBulletsManager.Instance.GetFromPool(firePoint.position, firePoint.rotation);
        if (bullet != null)
        {
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                Vector2 direction = (targetPosition - firePoint.position).normalized;
                bulletRb.linearVelocity = direction * 10f; // Velocidad de la bala
            }

            // SINCRONIZAR LA BALA EN LA RED
            NetworkServer.Spawn(bullet);
        }
    }
    */

    [Command] // Se ejecuta en el servidor
    void CmdShoot(Vector3 targetPosition)
    {
        GameObject bullet = PoolBulletsManager.Instance.GetFromPool(firePoint.position, firePoint.rotation);
        if (bullet != null)
        {
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Calculamos la dirección en el servidor
                Vector2 shootDirection = (targetPosition - firePoint.position).normalized;
                rb.linearVelocity = shootDirection * 10f;
            }
        }

        RpcShowShoot(targetPosition); // ✅ Se lo mostramos a todos los clientes
    }

    [ClientRpc] // Se ejecuta en todos los clientes para sincronizar la animación de disparo
    void RpcShowShoot(Vector3 targetPosition)
    {
        if (isServer) return; // El servidor ya creó la bala, no necesita hacerlo de nuevo

        GameObject bullet = PoolBulletsManager.Instance.GetFromPool(firePoint.position, firePoint.rotation);
        if (bullet != null)
        {
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 shootDirection = (targetPosition - firePoint.position).normalized;
                rb.linearVelocity = shootDirection * 10f;
            }
        }
    }
}
