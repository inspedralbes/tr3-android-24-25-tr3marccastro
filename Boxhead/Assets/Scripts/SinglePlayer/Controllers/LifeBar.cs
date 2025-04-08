using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    // Referencia a la imagen que se usar� para mostrar la vida de la barra
    public Image fillingBarLife;

    // M�ximo de vida para establecer la capacidad m�xima de la barra
    private float maxHealth;

    // Funci�n para establecer el m�ximo de vida y actualizar la barra de vida
    public void SetMaxHealth(float maxVida)
    {
        maxHealth = maxVida; // Guardamos el valor del m�ximo de vida
        UpdateHealth(maxVida); // Actualizamos la barra de vida con el valor m�ximo (la barra se ver� llena)
    }

    // Funci�n para actualizar la barra de vida en funci�n de la vida actual
    public void UpdateHealth(float actualHealth)
    {
        // Actualiza la proporci�n de la barra en funci�n de la vida actual y la vida m�xima
        fillingBarLife.fillAmount = actualHealth / maxHealth;
    }
}