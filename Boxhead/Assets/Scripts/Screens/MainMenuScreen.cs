using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // Para cambiar de escena

public class MainMenuScreen : MonoBehaviour
{
    private Button startButton, shopButton, multiplayerButton, exitButton;
    private void OnEnable()
    {
        // Obtener el root del documento UI
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Encontrar los botones por su nombre
        startButton = root.Q<Button>("StartButton");
        exitButton = root.Q<Button>("ExitButton");
        multiplayerButton = root.Q<Button>("MultiplayerButton");
        shopButton = root.Q<Button>("ShopButton");

        // Asignar eventos
        startButton.clicked += () => SceneManager.LoadScene("SinglePlayerGameScene");
        multiplayerButton.clicked += () => SceneManager.LoadScene("LoginScene");
        shopButton.clicked += () => SceneManager.LoadScene("LoginScene");
        exitButton.clicked += ExitGame;
    }

    private void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Para detener en el editor
        #endif
    }
}
