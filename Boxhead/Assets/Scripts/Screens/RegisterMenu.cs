using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using System.Text;

public class RegisterMenu : MonoBehaviour
{
    private TextField usernameField;
    private TextField emailField, passwordField;
    private Button registerButton, backButton;
    private Label errorLabel;

    private string apiUrl = "http://localhost:3002/api/users/register"; // URL del backend

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
        // Crear el objeto JSON manualmente
        RegisterData registerData = new()
        {
            username = username,
            email = email,
            password = password
        };

        string jsonData = JsonUtility.ToJson(registerData);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

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
            Debug.Log("Respuesta del servidor: " + result);

            try
            {
                ResponseRegisterData response = JsonUtility.FromJson<ResponseRegisterData>(result);

                if (response.message == "success")
                {
                    Debug.Log("Resgistro exitoso.");
                    UserSession.SaveUserEmail(email);
                    SceneManager.LoadScene("ShopMenu");
                }
                else
                {
                    ShowError(response.message);
                }
            }
            catch (System.Exception e)
            {
                ShowError("Error en la respuesta del servidor.");
                Debug.LogError("Error al procesar la respuesta JSON: " + e.Message);
            }
        }
        else
        {
            ShowError("Error de conexión: " + request.error);
        }
    }

    // Mostrar el mensaje de error
    private void ShowError(string message)
    {
        errorLabel.text = message;
        errorLabel.style.display = DisplayStyle.Flex;
    }
}

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