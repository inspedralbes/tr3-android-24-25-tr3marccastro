using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    private VisualElement gameOverScreen;
    private Button retryButton;
    private Button mainMenuButton;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        gameOverScreen = root.Q<VisualElement>("gameOverScreen");
        retryButton = root.Q<Button>("retryButton");
        mainMenuButton = root.Q<Button>("mainMenuButton");

        retryButton.clicked += RetryGame;
        mainMenuButton.clicked += ReturnToMainMenu;
    }

    public void ShowGameOver()
    {
        gameOverScreen.style.display = DisplayStyle.Flex;
    }

    private void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reinicia la partida
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Carga el menú principal
    }
}
