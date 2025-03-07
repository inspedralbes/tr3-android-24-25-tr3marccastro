using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    public Transform firePoint;
    public int charger = 0;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveX, moveY) * moveSpeed;
        rb.linearVelocity = movement;

        // Obtener la posición del ratón en el mundo
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Asegurarse de que esté en el plano 2D

        // Rotar al jugador hacia el ratón
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (Input.GetMouseButtonDown(0))
        {
            Shoot(mousePosition);
        }
    }

    void Shoot(Vector3 targetPosition) {
        GameObject bullet = PoolBullets.Instance.GetFromPool(firePoint.position, firePoint.rotation);
        if(bullet != null) {
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if(rb != null) {
                // Calcular la dirección hacia el ratón
                Vector2 direction = (targetPosition - firePoint.position).normalized;

                // Asignar la velocidad de la bala
                bulletRb.linearVelocity = direction * 10f; // La velocidad de la bala
            }
        }
    }
}