using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LoginManager : MonoBehaviour
{
    private TextField usernameField, passwordField;
    private Label errorLabel;
    private string apiUrl = "http://localhost:3001/api/login";

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

    private IEnumerator LoginRequest()
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
}

    //private IEnumerator LoginRequest()
    //{
    //    if (string.IsNullOrEmpty(usernameField.value) || string.IsNullOrEmpty(passwordField.value))
    //    {
    //        ShowError("Por favor, ingresa usuario y contraseña.");
    //        yield break;
    //    }

    //    string jsonData = JsonUtility.ToJson(new LoginData(usernameField.value, passwordField.value));
    //    byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

    //    using UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
    //    {
    //        uploadHandler = new UploadHandlerRaw(jsonBytes),
    //        downloadHandler = new DownloadHandlerBuffer()
    //    };
    //    request.SetRequestHeader("Content-Type", "application/json");

    //    yield return request.SendWebRequest();

    //    if (request.result == UnityWebRequest.Result.Success)
    //    {
    //        string jsonResponse = request.downloadHandler.text;
    //        Debug.Log("Respuesta del servidor: " + jsonResponse);

    //        LoginResponse response = JsonUtility.FromJson<LoginResponse>(jsonResponse);

    //        if (response.status == "success")
    //        {
    //            // Si el login es exitoso, carga la escena del multiplayer
    //            SceneManager.LoadScene("MultiplayerScene");
    //        }
    //        else
    //        {
    //            // Si el login falla, muestra el error
    //            ShowError(response.message);
    //        }
    //    }
    //    else
    //    {
    //        // Si hay error de conexión, muestra el error
    //        ShowError("Error de conexión: " + request.error);
    //    }
    //}

    //private void ShowError(string message)
    //{
    //    errorLabel.text = message;
    //    errorLabel.style.display = DisplayStyle.Flex;
    //}

    //[System.Serializable]
    //private class LoginData
    //{
    //    public string username, password;
    //    public LoginData(string username, string password)
    //    {
    //        this.username = username;
    //        this.password = password;
    //    }
    //}

    //[System.Serializable]
    //private class LoginResponse
    //{
    //    public string status;
    //    public string message;
    //}
