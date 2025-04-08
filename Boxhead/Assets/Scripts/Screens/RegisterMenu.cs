using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using System.Text;

public class RegisterMenu : MonoBehaviour
{
    // Campos de entrada para el usuario
    private TextField usernameField;
    private TextField emailField, passwordField;

    // Botones para registrar y volver atrás
    private Button registerButton, backButton;

    // Etiqueta para mostrar errores
    private Label errorLabel;

    // URL de la API para el registro
    private string apiUrl = "http://localhost:3002/api/users/register";

    // Se ejecuta al activar el objeto (pantalla de registro)
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Obtener referencias de la UI
        usernameField = root.Q<TextField>("usernameField");
        emailField = root.Q<TextField>("emailField");
        passwordField = root.Q<TextField>("passwordField");
        registerButton = root.Q<Button>("registerButton");
        backButton = root.Q<Button>("backButton");
        errorLabel = root.Q<Label>("errorLabel");

        // Asignar eventos a los botones
        registerButton.clicked += OnRegisterButtonClicked;
        backButton.clicked += () => SceneManager.LoadScene("LoginScene");
    }

    // Acción al hacer clic en el botón de registrar
    private void OnRegisterButtonClicked()
    {
        string username = usernameField.value;
        string email = emailField.value;
        string password = passwordField.value;

        // Validar que los campos no estén vacíos
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Tots els camps són obligatoris, si us plau"); // Mensaje en catalán: Todos los campos son obligatorios
        }
        else
        {
            // Enviar solicitud al servidor
            StartCoroutine(RegisterRequest(username, email, password));
        }
    }

    // Enviar datos al servidor para registrar al usuario
    private IEnumerator RegisterRequest(string username, string email, string password)
    {
        RegisterData registerData = new()
        {
            username = username,
            email = email,
            password = password
        };

        string jsonData = JsonUtility.ToJson(registerData);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

        // Crear solicitud POST
        UnityWebRequest request = new(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(jsonBytes),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string result = request.downloadHandler.text;

            try
            {
                // Procesar respuesta
                ResponseRegisterData response = JsonUtility.FromJson<ResponseRegisterData>(result);

                if (response.message == "success")
                {
                    // Guardar correo del usuario y cargar pantalla de tienda
                    UserSession.SaveUserEmail(email);
                    SceneManager.LoadScene("ShopMenu");
                }
                else
                {
                    ShowError(response.message); // Mostrar error devuelto por el servidor
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error en processar la resposta JSON: " + e.Message);
            }
        }
        else
        {
            ShowError("Error de connexió"); // Error si falla la conexión
        }
    }

    // Mostrar mensaje de error en pantalla
    private void ShowError(string message)
    {
        errorLabel.text = message;
        errorLabel.style.display = DisplayStyle.Flex;
    }
}

// --- Clases para los datos JSON enviados y recibidos ---

[System.Serializable]
public class RegisterData
{
    public string username;
    public string email;
    public string password;
}

[System.Serializable]
public class ResponseRegisterData
{
    public string message;
    public string email;
}