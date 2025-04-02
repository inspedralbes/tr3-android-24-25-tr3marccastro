using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using System.Text;

public class GameOverMenu : MonoBehaviour
{
    private VisualElement gameOverScreen;
    private Button retryButton, mainMenuButton;
    private string apiUrl = "http://localhost:3002/api/stats";
    public EnemySpawner enemySpawner;
    public WebSocketManager webSocketManager;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        gameOverScreen = root.Q<VisualElement>("gameOverScreen");
        retryButton = root.Q<Button>("retryButton");
        mainMenuButton = root.Q<Button>("mainMenuButton");

        retryButton.clicked += () => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        mainMenuButton.clicked += ReturnToMainMenu;
    }

    public void ShowGameOver()
    {
        gameOverScreen.style.display = DisplayStyle.Flex;
    }

    private void ReturnToMainMenu()
    {
        if (UserSession.GetUserEmail() != null)
        {
            StartCoroutine(Results());
        }
        else SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator Results() 
    {
        ResultsMatch resultsMatch = new()
        {
            kills = enemySpawner.kills,
            rounds = enemySpawner.round,
            totalTime = enemySpawner.roundTimer,
            wasModificated = webSocketManager.isModificated,
            email = UserSession.GetUserEmail(),
        };

        string jsonData = JsonUtility.ToJson(resultsMatch);
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
                ResponseData2 response = JsonUtility.FromJson<ResponseData2>(result);

                if (response.message == "success")
                {
                    SceneManager.LoadScene("MainMenu");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error al procesar la respuesta JSON: " + e.Message);
            }
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}

[System.Serializable]
public class ResultsMatch
{
    public int kills;
    public int rounds;
    public float totalTime;
    public bool wasModificated;
    public string email;
}

[System.Serializable]
public class ResponseData2
{
    public string message;
}