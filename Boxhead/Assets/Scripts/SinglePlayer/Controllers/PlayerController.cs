using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variables públicas para la velocidad, vida y otros componentes del jugador
    public float speed = 5f;             // Velocidad de movimiento del jugador
    private float currentSpeed;          // Velocidad actual (para modificaciones futuras)
    private Rigidbody2D rb;              // Componente Rigidbody2D para el movimiento físico
    private Animator animator;           // Componente Animator para las animaciones
    public Transform firePoint;          // Punto de disparo para los proyectiles
    public LifeBar lifeBar;              // Barra de vida del jugador

    // Máxima vida del jugador
    public int maxHealth = 100;          // Salud máxima
    private float currentHealth;         // Salud actual

    // Referencia al menú de GameOver
    public GameOverMenu gameOverMenu;

    // Sonidos para disparar
    [SerializeField] private AudioClip audioShoot; // Sonido de disparo

    void Start()
    {
        // Inicializa los componentes y las variables del jugador
        rb = GetComponent<Rigidbody2D>();            // Obtiene el componente Rigidbody2D
        animator = GetComponent<Animator>();         // Obtiene el componente Animator
        currentHealth = maxHealth;                  // Asigna la salud máxima al jugador
        currentSpeed = speed;                       // Establece la velocidad del jugador

        // Establece la vida máxima del jugador en la barra de vida
        lifeBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        // Referencia al SpriteRenderer del jugador para obtener su tamaño
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        Vector2 spriteSize = renderer.bounds.size;

        // Captura las entradas de las teclas para mover al jugador
        float moveX = Input.GetAxisRaw("Horizontal");  // Movimiento en el eje X
        float moveY = Input.GetAxisRaw("Vertical");    // Movimiento en el eje Y

        // Movimiento normalizado del jugador
        Vector2 movement = new Vector2(moveX, moveY).normalized;

        // Movimiento del jugador usando Rigidbody2D
        rb.linearVelocity = movement * currentSpeed;

        // Obtén la posición del ratón en la pantalla para orientar al jugador
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = (mousePosition - transform.position).normalized;

        // Actualiza los valores de animación para reflejar la dirección de movimiento
        animator.SetFloat("Horizontal", lookDirection.x);
        animator.SetFloat("Vertical", lookDirection.y);

        // Invierte el jugador si se mueve hacia la izquierda
        float originalScaleX = Mathf.Abs(transform.localScale.x);
        if (lookDirection.x < 0)
        {
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (lookDirection.x > 0)
        {
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        }

        // Actualiza las últimas direcciones de movimiento para las animaciones de caminar
        if (moveX != 0 || moveY != 0)
        {
            animator.SetFloat("LastMoveX", moveX);
            animator.SetFloat("LastMoveY", moveY);
        }

        // Si se hace clic con el ratón, dispara un proyectil
        if (Input.GetMouseButtonDown(0))
        {
            SoundController.Instance.ExecuteSound(audioShoot); // Reproduce el sonido de disparo
            Shoot(mousePosition);  // Llama al método para disparar
        }
    }

    // Función para disparar un proyectil hacia la posición del ratón
    void Shoot(Vector3 targetPosition)
    {
        // Obtiene un proyectil del Pool de proyectiles
        GameObject bullet = PoolBulletsManager.Instance.GetFromPool(firePoint.position, firePoint.rotation);
        if (bullet != null)
        {
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();  // Obtiene el Rigidbody2D del proyectil
            if (bulletRb != null)
            {
                // Calcula la dirección del disparo y el ángulo
                Vector2 direction = (targetPosition - firePoint.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Establece el ángulo del proyectil
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

                // Aplica la velocidad al proyectil
                bulletRb.linearVelocity = direction * 10f;  // La velocidad del proyectil
            }
        }
    }

    // Función para aplicar daño al jugador
    public void TakeDamagePlayer(float damage)
    {
        currentHealth -= damage;  // Reduce la salud del jugador
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Limita la vida entre 0 y maxHealth

        // Si la salud llega a 0, el jugador muere
        if (currentHealth <= 0)
        {
            Die();  // Llama al método Die
        }

        // Actualiza la barra de vida
        lifeBar.UpdateHealth(currentHealth);
    }

    // Función que se llama cuando el jugador muere
    void Die()
    {
        // Detiene el movimiento del jugador
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;  // Detiene la velocidad del Rigidbody2D
            rb.constraints = RigidbodyConstraints2D.FreezeAll;  // Congela el Rigidbody2D para evitar movimientos
        }

        // Muestra el menú de GameOver
        gameOverMenu.ShowGameOver();
    }

    // Función para actualizar las estadísticas del jugador (vida y velocidad)
    public void UpdateStatsPlayer(int newHealth, float newSpeed)
    {
        maxHealth = newHealth;      // Asigna la nueva salud máxima
        currentHealth = newHealth;  // Asigna la nueva salud actual
        currentSpeed = newSpeed;    // Asigna la nueva velocidad

        // Actualiza la barra de vida
        lifeBar.SetMaxHealth(maxHealth);
        lifeBar.UpdateHealth(currentHealth);
    }
}
