using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoginScene : MonoBehaviour
{
    private TextField usernameField;
    private TextField passwordField;
    private Button loginButton, backButton;
    private Label errorLabel;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        usernameField = root.Q<TextField>("usernameField");
        passwordField = root.Q<TextField>("passwordField");
        loginButton = root.Q<Button>("loginButton");
        backButton = root.Q<Button>("backButton");
        errorLabel = root.Q<Label>("errorLabel");

        loginButton.clicked += OnLoginButtonClicked;
        backButton.clicked += BackToMenu;
        errorLabel.style.display = DisplayStyle.None; // Ocultar el error al principio
    }

    private void OnLoginButtonClicked()
    {
        string username = usernameField.value;
        string password = passwordField.value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorLabel.text = "Por favor ingresa un nombre de usuario y contraseña.";
            errorLabel.style.display = DisplayStyle.Flex;
        }
        else
        {
            StartCoroutine(LoginRequest(username, password));
        }
    }

    private IEnumerator LoginRequest(string username, string password)
    {
        // Datos para enviar en la petición
        var loginData = new WWWForm();
        loginData.AddField("username", username);
        loginData.AddField("password", password);

        // Reemplaza con tu URL de la API de login
        string url = "https://tu-api.com/login";  

        using (UnityWebRequest www = UnityWebRequest.Post(url, loginData))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Respuesta de la API: Si es 200 OK, continúa
                string response = www.downloadHandler.text;

                if (response.Contains("success")) // Suponiendo que la API devuelve "success" en caso de éxito
                {
                    // Si el login es exitoso, vamos al menú multiplayer
                    SceneManager.LoadScene("MultiplayerMenuScene");
                }
                else
                {
                    // Si no está registrado o el login falla
                    errorLabel.text = "Credenciales incorrectas.";
                    errorLabel.style.display = DisplayStyle.Flex;
                }
            }
            else
            {
                errorLabel.text = "Error de conexión.";
                errorLabel.style.display = DisplayStyle.Flex;
            }
        }
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Cambia "MainMenu" por tu escena de menú
    }
}
