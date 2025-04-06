using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    // Referencia a la imagen que se usará para mostrar la vida de la barra
    public Image fillingBarLife;

    // Máximo de vida para establecer la capacidad máxima de la barra
    private float maxHealth;

    // Función para establecer el máximo de vida y actualizar la barra de vida
    public void SetMaxHealth(float maxVida)
    {
        maxHealth = maxVida; // Guardamos el valor del máximo de vida
        UpdateHealth(maxVida); // Actualizamos la barra de vida con el valor máximo (la barra se verá llena)
    }

    // Función para actualizar la barra de vida en función de la vida actual
    public void UpdateHealth(float actualHealth)
    {
        // Actualiza la proporción de la barra en función de la vida actual y la vida máxima
        fillingBarLife.fillAmount = actualHealth / maxHealth;
    }
}