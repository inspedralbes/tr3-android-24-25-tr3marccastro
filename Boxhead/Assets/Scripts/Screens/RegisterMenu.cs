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

    private string apiUrl = "http://localhost:3002/api/users/register";

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        usernameField = root.Q<TextField>("usernameField");
        emailField = root.Q<TextField>("emailField");
        passwordField = root.Q<TextField>("passwordField");
        registerButton = root.Q<Button>("registerButton");
        backButton = root.Q<Button>("backButton");
        errorLabel = root.Q<Label>("errorLabel");

        registerButton.clicked += OnRegisterButtonClicked;
        backButton.clicked += () => SceneManager.LoadScene("LoginScene");
    }

    private void OnRegisterButtonClicked()
    {
        string username = usernameField.value;
        string email = emailField.value;
        string password = passwordField.value;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Tots els camps són obligatoris, si us plau");
        }
        else
        {
            StartCoroutine(RegisterRequest(username, email, password));
        }
    }

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
                ResponseRegisterData response = JsonUtility.FromJson<ResponseRegisterData>(result);

                if (response.message == "success")
                {
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
                Debug.LogError("Error en processar la resposta JSON: " + e.Message);
            }
        }
        else
        {
            ShowError("Error de connexió");
        }
    }

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