using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Referencia al men� de pausa visual
    private VisualElement pauseMenu;

    // Estado del juego pausado o no
    private bool isPaused = false;

    // Se ejecuta cuando el objeto se activa
    private void OnEnable()
    {
        // Obtener el elemento ra�z del UI Toolkit
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Buscar el contenedor del men� de pausa
        pauseMenu = root.Q<VisualElement>("pauseMenu");

        // Botones del men� de pausa
        Button resumeButton = root.Q<Button>("resumeButton");
        Button mainMenuButton = root.Q<Button>("mainMenuButton");

        // Asignar funciones a los botones
        resumeButton.clicked += ResumeGame;
        mainMenuButton.clicked += ReturnToMainMenu;

        // Asegurarse de que el men� est� oculto al inicio
        pauseMenu.style.display = DisplayStyle.None;
    }

    // Detectar pulsaciones de tecla en cada frame
    private void Update()
    {
        // Si se presiona Escape o P, alternar el estado de pausa
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    // Alternar entre pausar y reanudar el juego
    private void TogglePause()
    {
        isPaused = !isPaused;

        // Mostrar u ocultar el men� seg�n el estado
        pauseMenu.style.display = isPaused ? DisplayStyle.Flex : DisplayStyle.None;

        // Detener o continuar el tiempo del juego
        Time.timeScale = isPaused ? 0 : 1;
    }

    // Funci�n llamada al hacer clic en "Reanudar"
    private void ResumeGame()
    {
        TogglePause(); // Simplemente alterna el estado de pausa
    }

    // Funci�n para volver al men� principal
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1; // Asegura que el tiempo est� en 1 antes de cambiar de escena
        SceneManager.LoadScene("MainMenu"); // Cargar la escena del men� principal
    }
}
