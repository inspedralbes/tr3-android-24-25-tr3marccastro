using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    public Transform firePoint;
    public int charger = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Desactivar controles en otros jugadores
        if (!isLocalPlayer)
        {
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return; // Solo controla su propio personaje

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveX, moveY) * moveSpeed;
        rb.linearVelocity = movement;

        // Obtener la posición del ratón en el mundo
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Asegurar que esté en el plano 2D

        // Rotar al jugador hacia el ratón
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (Input.GetMouseButtonDown(0))
        {
            CmdShoot(mousePosition); // ✅ Ahora pasamos la dirección del disparo al servidor
        }
    }

    [Command] // Se ejecuta en el servidor
    void CmdShoot(Vector3 targetPosition)
    {
        GameObject bullet = PoolBullets.Instance.GetFromPool(firePoint.position, firePoint.rotation);
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

        GameObject bullet = PoolBullets.Instance.GetFromPool(firePoint.position, firePoint.rotation);
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