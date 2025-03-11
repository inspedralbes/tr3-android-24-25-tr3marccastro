using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class RegisterMenu : MonoBehaviour
{
    private TextField usernameField;
    private TextField emailField, passwordField;
    private Button registerButton, backButton;
    private Label errorLabel;

    private string apiUrl = "http://localhost:3000/api/register"; // URL del backend

    void OnEnable()
    {
        // Obtener el root del documento UXML
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Obtener los elementos de la UI
        usernameField = root.Q<TextField>("usernameField");
        emailField = root.Q<TextField>("emailField");
        passwordField = root.Q<TextField>("passwordField");
        registerButton = root.Q<Button>("registerButton");
        backButton = root.Q<Button>("backButton");
        errorLabel = root.Q<Label>("errorLabel");

        // Asignar el evento del botón de registro
        registerButton.clicked += OnRegisterButtonClicked;
        backButton.clicked += () => SceneManager.LoadScene("LoginScene");
    }

    // Evento del botón de registro
    private void OnRegisterButtonClicked()
    {
        // Obtener los valores de los campos
        string username = usernameField.value;
        string email = emailField.value;
        string password = passwordField.value;

        // Validar los campos
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Todos los campos son obligatorios.");
        }
        else
        {
            // Enviar la solicitud de registro
            StartCoroutine(RegisterRequest(username, email, password));
        }
    }

    // Enviar la solicitud POST al backend
    private IEnumerator RegisterRequest(string username, string email, string password)
    {
        if (string.IsNullOrEmpty(usernameField.value) || string.IsNullOrEmpty(passwordField.value))
        {
            Debug.Log("Por favor, ingresa usuario y contraseña.");
            yield break;
        }

        WWWForm form = new();
        form.AddField("username",  usernameField.value);
        form.AddField("password", passwordField.value);

        UnityWebRequest request = UnityWebRequest.Post(apiUrl, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Obtener el resultado de la respuesta
            string result = request.downloadHandler.text;

            // Depuración
            Debug.Log("Resultado recibido del servidor: " + result);

            // Comprobamos si el resultado es lo que esperamos
            if (result.Contains("success"))
            {
                Debug.Log("Operación exitosa. Cargando la escena...");
                SceneManager.LoadScene("MultiplayerScene");
            }
            else
            {
                // En caso de que el servidor no devuelva el resultado esperado
                Debug.LogWarning("Respuesta inesperada: " + result);
            }
        }
        else
        {
            // Manejo detallado del error de la conexión
            Debug.LogError("Error de conexión: " + request.error);
        }
    }

    // Mostrar el mensaje de error
    private void ShowError(string message)
    {
        Debug.Log("Hola");
        errorLabel.text = message;
        errorLabel.style.display = DisplayStyle.Flex;
    }
}
