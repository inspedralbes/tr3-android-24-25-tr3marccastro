using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement; // Para cambiar de escena

public class MainMenuScreen : MonoBehaviour
{
    private void OnEnable()
    {
        // Obtener el root del documento UI
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Encontrar los botones por su nombre
        Button startButton = root.Q<Button>("StartButton");
        Button exitButton = root.Q<Button>("ExitButton");
        Button multiplayerButton = root.Q<Button>("MultiplayerButton");

        // Asignar eventos
        startButton.clicked += StartGame;
        multiplayerButton.clicked += MultiplayerGame;
        exitButton.clicked += ExitGame;
    }

    private void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Cambia "GameScene" por el nombre de tu escena
    }

    private void MultiplayerGame() {
        SceneManager.LoadScene("MultiplayerScene");
    }

    private void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Para detener en el editor
        #endif
    }
}
