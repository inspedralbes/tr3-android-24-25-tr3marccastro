using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    public float maxHealth = 100f;      // Salud máxima del jugador
    [SyncVar] public float currentHealth; // Salud actual sincronizada entre todos los clientes

    private HealthBar healthBar; // Referencia al script de la barra de vida UI

    void Start()
    {
        currentHealth = maxHealth;  // Inicializar la salud al valor máximo
        healthBar = GetComponentInChildren<HealthBar>(); // Asumimos que la barra de vida está en un hijo del jugador
    }

    // Método para aplicar daño
    public void TakeDamage(float damage)
    {
        if (!isLocalPlayer) return; // Asegurarse de que solo el jugador local reciba daño

        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Actualizar la barra de vida
        if (healthBar != null)
        {
            healthBar.TakeDamage(damage);  // Actualiza la UI de la barra de vida
        }

        // Llamamos al método Rpc para actualizar la salud en otros clientes
        RpcUpdateHealth(currentHealth);
    }

    // Método para curar al jugador
    public void Heal(float amount)
    {
        if (!isLocalPlayer) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Actualizar la barra de vida
        if (healthBar != null)
        {
            healthBar.Heal(amount);
        }

        // Llamamos al método Rpc para actualizar la salud en otros clientes
        RpcUpdateHealth(currentHealth);
    }

    // Función RPC para sincronizar la salud en los otros clientes
    [ClientRpc]
    void RpcUpdateHealth(float health)
    {
        currentHealth = health;

        if (healthBar != null)
        {
            healthBar.TakeDamage(0);  // Simplemente actualiza la barra de vida en el cliente
        }
    }
}
