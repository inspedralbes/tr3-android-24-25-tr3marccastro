using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using System.Text;

public class LoginManager : MonoBehaviour
{
    private TextField usernameField, passwordField;
    private Label errorLabel;
    private string apiUrl = "http://localhost:3002/api/login";

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        usernameField = root.Q<TextField>("usernameField");
        passwordField = root.Q<TextField>("passwordField");
        errorLabel = root.Q<Label>("errorLabel");
        errorLabel.style.display = DisplayStyle.None;

        root.Q<Button>("loginButton").clicked += () => StartCoroutine(LoginRequest());
        root.Q<Button>("registerButton").clicked += () => SceneManager.LoadScene("RegisterMenu");
        root.Q<Button>("backButton").clicked += () => SceneManager.LoadScene("MainMenu");
    }

    // Metodo 1
    private IEnumerator LoginRequest()
    {
        if (string.IsNullOrEmpty(usernameField.value) || string.IsNullOrEmpty(passwordField.value))
        {
            ShowErrorMessage("Por favor, ingresa usuario y contraseña.");
            yield break;
        }

        // Crear el objeto JSON manualmente
        LoginData loginData = new()
        {
            username = usernameField.value,
            password = passwordField.value
        };

        string jsonData = JsonUtility.ToJson(loginData);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
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
                ResponseData response = JsonUtility.FromJson<ResponseData>(result);

                if (response.message == "success")
                {
                    Debug.Log("Inicio de sesión exitoso. Cargando la escena...");
                    SceneManager.LoadScene("MultiplayerScene");
                }
                else
                {
                    ShowErrorMessage("Usuario o contraseña incorrectos.");
                }
            }
            catch (System.Exception e)
            {
                ShowErrorMessage("Error en la respuesta del servidor.");
                Debug.LogError("Error al procesar la respuesta JSON: " + e.Message);
            }
        }
        else
        {
            ShowErrorMessage("Error de conexión: " + request.error);
        }
    }

    private void ShowErrorMessage(string message)
    {
        errorLabel.text = message;
        errorLabel.style.display = DisplayStyle.Flex;
    }
}

// Clase para enviar datos de login en JSON
[System.Serializable]
public class LoginData
{
    public string username;
    public string password;
}

// Clase para recibir respuesta del servidor
[System.Serializable]
public class ResponseData
{
    public string message;
}