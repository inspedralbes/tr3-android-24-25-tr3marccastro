using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;  // Referencia a la imagen de la barra de vida
    public float maxHealth = 100f;  // Salud m�xima del jugador
    private float currentHealth;  // Salud actual del jugador

    void Start()
    {
        currentHealth = maxHealth;  // Inicializamos la salud del jugador con el valor m�ximo
    }

    // M�todo que se llama cuando el jugador recibe da�o
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;  // Asegurarnos de que la salud no sea negativa
        UpdateHealthBar();
    }

    // M�todo para curar al jugador
    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;  // Asegurarnos de que la salud no supere el m�ximo
        UpdateHealthBar();
    }

    // Actualiza la barra de vida visualmente
    private void UpdateHealthBar()
    {
        float healthPercentage = currentHealth / maxHealth;  // Calculamos el porcentaje de vida
        healthBarImage.fillAmount = healthPercentage;  // Actualizamos la imagen de la barra de vida
    }
}
