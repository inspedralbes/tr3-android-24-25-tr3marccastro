using UnityEngine;
using UnityEngine.UI; // Necesario para UI

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private float currentSpeed;
    private Rigidbody2D rb;
    private Animator animator; // ✅ Nuevo: Referencia al Animator
    public Transform firePoint;
    public LifeBar lifeBar;

    // 🔴 Variables de vida
    public int maxHealth = 100;
    private float currentHealth;
    public GameOverMenu gameOverMenu;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // ✅ Obtener el Animator
        currentHealth = maxHealth;
        currentSpeed = speed;
        lifeBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        // 📌 1. Obtener entrada del teclado (Movimiento)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = movement * currentSpeed;

        // 📌 2. Obtener la posición del ratón en el mundo
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = (mousePosition - transform.position).normalized;

        // 📌 3. Enviar al Animator la dirección del ratón (aunque no se mueva)
        animator.SetFloat("Horizontal", lookDirection.x);
        animator.SetFloat("Vertical", lookDirection.y);

        // 📌 4. Cambiar la escala del sprite según la dirección del ratón
        float originalScaleX = Mathf.Abs(transform.localScale.x);
        if (lookDirection.x < 0)
        {
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (lookDirection.x > 0)
        {
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        }

        // 📌 5. Recordar la última dirección en que se movió
        if (moveX != 0 || moveY != 0)
        {
            animator.SetFloat("LastMoveX", moveX);
            animator.SetFloat("LastMoveY", moveY);
        }

        // 📌 6. Disparar con clic izquierdo
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
                // ✅ Corregimos la dirección de la bala
                Vector2 direction = (targetPosition - firePoint.position).normalized;

                // ✅ Aseguramos que la rotación de la bala sea correcta
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

                // ✅ Aplicamos velocidad en la dirección correcta
                bulletRb.linearVelocity = direction * 10f; // Ajusta la velocidad según sea necesario
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
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Detiene el movimiento
            rb.constraints = RigidbodyConstraints2D.FreezeAll; // Congela posición y rotación
        }
        gameOverMenu.ShowGameOver();
    }

    public void UpdateStatsPlayer(int newHealth, float newSpeed)
    {
        maxHealth = newHealth;
        currentHealth = newHealth; // Restaurar vida al máximo con la nueva estadística
        currentSpeed = newSpeed;

        lifeBar.SetMaxHealth(maxHealth);
        lifeBar.ActualizarVida(currentHealth); // Refrescar la barra de vida en UI

        Debug.Log("📌 Estadísticas del jugador actualizadas: Vida = " + maxHealth + " | Velocidad = " + currentSpeed);
    }
}
