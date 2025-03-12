using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 3; // Salud inicial del enemigo
    private int currentHealth;

    private void OnEnable()
    {
        Initialize(); // Restaurar vida al activarse
    }

    // Método para inicializar el enemigo, lo usas para poner la salud a su valor inicial cuando se crea
    public void Initialize()
    {
        currentHealth = health; // Establecer la salud inicial
    }

    // Método para aplicar daño al enemigo
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die(); // Si la salud llega a 0, el enemigo muere
        }
    }

    // Método que maneja la muerte del enemigo
    private void Die()
    {
        // Puedes agregar lógica adicional aquí, como animaciones o efectos
        Debug.Log("Enemigo muerto!");

        // Devolver el enemigo al pool para reutilizarlo
        PoolEnemies.Instance.ReturnToPool(gameObject); // Asegúrate de tener un pool para los enemigos también
    }
}