using UnityEngine;
using UnityEngine.UI; // Necesario para UI

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    public Transform firePoint;
    public LifeBar lifeBar;

    // 🔴 Variables de vida
    public int maxHealth = 100;
    private float currentHealth;
    public GameOverMenu gameOverMenu;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        lifeBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveX, moveY) * moveSpeed;
        rb.linearVelocity = movement;

        // Obtener la posición del ratón en el mundo
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        // Rotar al jugador hacia el ratón
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (Input.GetMouseButtonDown(0))
        {
            Shoot(mousePosition);
        }
    }

    void Shoot(Vector3 targetPosition)
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
        }
    }

    // 🔴 Método para recibir daño y actualizar la barra
    public void TakeDamagePlayer(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        lifeBar.ActualizarVida(currentHealth);
    }

    void Die()
    {
        Debug.Log("¡Jugador muerto!");
        gameOverMenu.ShowGameOver();
    }
}
