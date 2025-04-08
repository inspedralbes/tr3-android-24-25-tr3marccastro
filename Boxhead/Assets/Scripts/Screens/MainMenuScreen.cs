using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuScreen : MonoBehaviour
{
    // Referencias a los botones del men� principal
    private Button startButton, shopButton, logoutButton, exitButton;

    // M�todo que se ejecuta cuando el objeto se activa
    private void OnEnable()
    {
        // Obtener el elemento ra�z del UI Toolkit
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Obtener el email del usuario (si est� logueado)
        string email = UserSession.GetUserEmail();

        // Buscar los botones en el UI por su nombre
        startButton = root.Q<Button>("StartButton");
        exitButton = root.Q<Button>("ExitButton");
        logoutButton = root.Q<Button>("LogoutButton");
        shopButton = root.Q<Button>("ShopButton");

        // Mostrar el bot�n de logout solo si el usuario est� logueado
        logoutButton.style.display = string.IsNullOrEmpty(email) ? DisplayStyle.None : DisplayStyle.Flex;

        // Asignar funciones a los botones
        startButton.clicked += () => SceneManager.LoadScene("SinglePlayerGameScene"); // Cargar el juego
        logoutButton.clicked += () =>
        {
            UserSession.DeleteEmail();                    // Eliminar la sesi�n
            logoutButton.style.display = DisplayStyle.None; // Ocultar el bot�n de logout
        };
        shopButton.clicked += () => SceneManager.LoadScene("LoginScene"); // Ir a la tienda o login
        exitButton.clicked += ExitGame; // Salir del juego
    }

    // M�todo para cerrar la aplicaci�n
    private void ExitGame()
    {
        Application.Quit(); // Cierra el juego (en versi�n build)

        // Si est�s en el editor de Unity, tambi�n detiene el modo play
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}