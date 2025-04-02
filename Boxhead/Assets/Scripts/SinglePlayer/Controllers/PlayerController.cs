using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private float currentSpeed;
    private Rigidbody2D rb;
    private Animator animator;
    public Transform firePoint;
    public LifeBar lifeBar;

    public int maxHealth = 100;
    private float currentHealth;
    public GameOverMenu gameOverMenu;
    [SerializeField] private AudioClip audioShoot;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        currentSpeed = speed;
        lifeBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        Vector2 spriteSize = renderer.bounds.size;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = movement * currentSpeed;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = (mousePosition - transform.position).normalized;

        animator.SetFloat("Horizontal", lookDirection.x);
        animator.SetFloat("Vertical", lookDirection.y);

        float originalScaleX = Mathf.Abs(transform.localScale.x);
        if (lookDirection.x < 0)
        {
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (lookDirection.x > 0)
        {
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        }

        if (moveX != 0 || moveY != 0)
        {
            animator.SetFloat("LastMoveX", moveX);
            animator.SetFloat("LastMoveY", moveY);
        }

        if (Input.GetMouseButtonDown(0))
        {
            SoundController.Instance.ExecuteSound(audioShoot);

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

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

                bulletRb.linearVelocity = direction * 10f;
            }
        }
    }


    public void TakeDamagePlayer(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        lifeBar.UpdateHealth(currentHealth);
    }

    void Die()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        gameOverMenu.ShowGameOver();
    }

    public void UpdateStatsPlayer(int newHealth, float newSpeed)
    {
        maxHealth = newHealth;
        currentHealth = newHealth;
        currentSpeed = newSpeed;

        lifeBar.SetMaxHealth(maxHealth);
        lifeBar.UpdateHealth(currentHealth);
    }
}
