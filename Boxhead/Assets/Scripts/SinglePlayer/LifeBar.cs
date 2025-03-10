using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    public Image rellenoBarraVida;
    private float vidaMaxima;

    public void SetMaxHealth(float maxVida)
    {
        vidaMaxima = maxVida;
        ActualizarVida(maxVida);
    }

    public void ActualizarVida(float vidaActual)
    {
        rellenoBarraVida.fillAmount = vidaActual / vidaMaxima;
    }
}