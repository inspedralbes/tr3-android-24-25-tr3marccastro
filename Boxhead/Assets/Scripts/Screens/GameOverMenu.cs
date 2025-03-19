using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using System.Text;

public class GameOverMenu : MonoBehaviour
{
    private VisualElement gameOverScreen;
    private Button retryButton;
    private Button mainMenuButton;
    private string apiUrl = "http://localhost:3002/api/stats";
    public EnemySpawner enemySpawner;

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
        StartCoroutine(Results());
    }

    private IEnumerator Results() 
    {

        // Crear el objeto JSON manualmente
        ResultsMatch resultsMatch = new()
        {
            kills = enemySpawner.kills,
            rounds = enemySpawner.round
        };

        string jsonData = JsonUtility.ToJson(resultsMatch);
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
                ResponseData2 response = JsonUtility.FromJson<ResponseData2>(result);

                if (response.message == "success")
                {
                    SceneManager.LoadScene("MainMenu");
                }
                else {
                    Debug.Log("No");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error al procesar la respuesta JSON: " + e.Message);
            }
        }
        else
        {
            Debug.Log("Error de conexión: " + request.error);
        }
    }
}

// Clase para enviar datos de login en JSON
[System.Serializable]
public class ResultsMatch
{
    public int kills;
    public int rounds;
}

// Clase para recibir respuesta del servidor
[System.Serializable]
public class ResponseData2
{
    public string message;
}