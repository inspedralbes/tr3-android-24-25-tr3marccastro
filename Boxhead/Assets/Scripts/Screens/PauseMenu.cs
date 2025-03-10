using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private VisualElement pauseMenu;
    private bool isPaused = false;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        pauseMenu = root.Q<VisualElement>("pauseMenu");

        Button resumeButton = root.Q<Button>("resumeButton");
        Button mainMenuButton = root.Q<Button>("mainMenuButton");

        resumeButton.clicked += ResumeGame;
        mainMenuButton.clicked += ReturnToMainMenu;

        pauseMenu.style.display = DisplayStyle.None; // Ocultar al inicio
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.style.display = isPaused ? DisplayStyle.Flex : DisplayStyle.None;
        Time.timeScale = isPaused ? 0 : 1; // Pausar o reanudar el tiempo
    }

    private void ResumeGame()
    {
        TogglePause();
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1; // Resetear el tiempo antes de cambiar de escena
        SceneManager.LoadScene("MainMenu");
    }
}