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
    private string apiUrl = "http://localhost:3002/api/users/login";

    private void Awake()
    {
        if(UserSession.IsUserLoggedIn()) SceneManager.LoadScene("ShopMenu");
    }

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
            ShowErrorMessage("Si us plau, introduïu usuari i contrasenya.");
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
                ResponseData response = JsonUtility.FromJson<ResponseData>(result);

                if (response.message == "success")
                {
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

    private void ShowErrorMessage(string message)
    {
        errorLabel.text = message;
        errorLabel.style.display = DisplayStyle.Flex;
    }
}

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