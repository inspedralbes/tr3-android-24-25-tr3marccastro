using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using System.Text;

public class LoginManager : MonoBehaviour
{
    // Campos UI
    private TextField usernameField, passwordField;
    private Label errorLabel;

    // URL del endpoint de login
    private string apiUrl = "http://localhost:3002/api/users/login";

    // Se ejecuta antes del OnEnable, útil para verificar sesión persistente
    private void Awake()
    {
        // Si el usuario ya está logueado, lo redirige directamente a la tienda
        if (UserSession.IsUserLoggedIn())
            SceneManager.LoadScene("ShopMenu");
    }

    // Se ejecuta cuando el objeto se activa
    private void OnEnable()
    {
        // Obtener la raíz del UI
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Asociar campos del formulario
        usernameField = root.Q<TextField>("usernameField");
        passwordField = root.Q<TextField>("passwordField");
        errorLabel = root.Q<Label>("errorLabel");
        errorLabel.style.display = DisplayStyle.None; // Ocultar mensajes de error por defecto

        // Asignar acciones a los botones
        root.Q<Button>("loginButton").clicked += () => StartCoroutine(LoginRequest());
        root.Q<Button>("registerButton").clicked += () => SceneManager.LoadScene("RegisterMenu");
        root.Q<Button>("backButton").clicked += () => SceneManager.LoadScene("MainMenu");
    }

    // Coroutine para enviar la solicitud de login al servidor
    private IEnumerator LoginRequest()
    {
        // Validar campos vacíos
        if (string.IsNullOrEmpty(usernameField.value) || string.IsNullOrEmpty(passwordField.value))
        {
            ShowErrorMessage("Si us plau, introduïu usuari i contrasenya.");
            yield break;
        }

        // Crear el cuerpo de la solicitud
        LoginData loginData = new()
        {
            username = usernameField.value,
            password = passwordField.value
        };

        string jsonData = JsonUtility.ToJson(loginData);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

        // Preparar la solicitud HTTP POST
        UnityWebRequest request = new(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(jsonBytes),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        // Esperar respuesta del servidor
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string result = request.downloadHandler.text;

            try
            {
                // Convertir la respuesta en objeto
                ResponseData response = JsonUtility.FromJson<ResponseData>(result);

                if (response.message == "success")
                {
                    // Guardar el email del usuario en sesión y redirigir a la tienda
                    UserSession.SaveUserEmail(response.email);
                    SceneManager.LoadScene("ShopMenu");
                }
                else
                {
                    ShowErrorMessage(response.message);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error en processar la resposta JSON: " + e.Message);
            }
        }
        else
        {
            ShowErrorMessage("Error de connexió.");
        }
    }

    // Mostrar un mensaje de error en la UI
    private void ShowErrorMessage(string message)
    {
        errorLabel.text = message;
        errorLabel.style.display = DisplayStyle.Flex;
    }
}

// Clases auxiliares para estructurar los datos del login
[System.Serializable]
public class LoginData
{
    public string username;
    public string password;
}

[System.Serializable]
public class ResponseData
{
    public string message;
    public string email;
}
