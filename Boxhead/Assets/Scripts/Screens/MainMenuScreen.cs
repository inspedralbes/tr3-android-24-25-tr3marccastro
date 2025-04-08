using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuScreen : MonoBehaviour
{
    // Referencias a los botones del menú principal
    private Button startButton, shopButton, logoutButton, exitButton;

    // Método que se ejecuta cuando el objeto se activa
    private void OnEnable()
    {
        // Obtener el elemento raíz del UI Toolkit
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Obtener el email del usuario (si está logueado)
        string email = UserSession.GetUserEmail();

        // Buscar los botones en el UI por su nombre
        startButton = root.Q<Button>("StartButton");
        exitButton = root.Q<Button>("ExitButton");
        logoutButton = root.Q<Button>("LogoutButton");
        shopButton = root.Q<Button>("ShopButton");

        // Mostrar el botón de logout solo si el usuario está logueado
        logoutButton.style.display = string.IsNullOrEmpty(email) ? DisplayStyle.None : DisplayStyle.Flex;

        // Asignar funciones a los botones
        startButton.clicked += () => SceneManager.LoadScene("SinglePlayerGameScene"); // Cargar el juego
        logoutButton.clicked += () =>
        {
            UserSession.DeleteEmail();                    // Eliminar la sesión
            logoutButton.style.display = DisplayStyle.None; // Ocultar el botón de logout
        };
        shopButton.clicked += () => SceneManager.LoadScene("LoginScene"); // Ir a la tienda o login
        exitButton.clicked += ExitGame; // Salir del juego
    }

    // Método para cerrar la aplicación
    private void ExitGame()
    {
        Application.Quit(); // Cierra el juego (en versión build)

        // Si estás en el editor de Unity, también detiene el modo play
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}