using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using System.Text;

public class GameOverMenu : MonoBehaviour
{
    // Referencias UI
    private VisualElement gameOverScreen;
    private Button retryButton, mainMenuButton;

    // URL del endpoint donde se env�an las estad�sticas
    private string apiUrl = "http://localhost:3002/api/stats";

    // Referencias a componentes externos
    public EnemySpawner enemySpawner;
    public WebSocketManager webSocketManager;

    // Inicializaci�n al activar el men�
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        gameOverScreen = root.Q<VisualElement>("gameOverScreen");
        retryButton = root.Q<Button>("retryButton");
        mainMenuButton = root.Q<Button>("mainMenuButton");

        // Al pulsar "Retry", reinicia la escena actual
        retryButton.clicked += () => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // Al pulsar "Main Menu", llama al m�todo ReturnToMainMenu
        mainMenuButton.clicked += ReturnToMainMenu;
    }

    // Muestra el men� de Game Over
    public void ShowGameOver()
    {
        gameOverScreen.style.display = DisplayStyle.Flex;
    }

    // Vuelve al men� principal, con o sin guardar estad�sticas
    private void ReturnToMainMenu()
    {
        // Si el usuario est� logueado, guarda resultados en la base de datos
        if (UserSession.GetUserEmail() != null)
        {
            StartCoroutine(Results());
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    // Env�a los resultados de la partida al servidor
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

        // Crear solicitud POST
        UnityWebRequest request = new(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(jsonBytes),
            downloadHandler = new DownloadHandlerBuffer()
        };

        request.SetRequestHeader("Content-Type", "application/json");

        // Esperar la respuesta
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string result = request.downloadHandler.text;

            try
            {
                ResponseData2 response = JsonUtility.FromJson<ResponseData2>(result);

                // Si el servidor responde con �xito, ir al men� principal
                if (response.message == "success")
                {
                    SceneManager.LoadScene("MainMenu");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error en processar la resposta JSON: " + e.Message);
            }
        }
        else
        {
            // Si hay error de red, tambi�n va al men� principal
            SceneManager.LoadScene("MainMenu");
        }
    }
}

// Estructura de datos para enviar estad�sticas de la partida
[System.Serializable]
public class ResultsMatch
{
    public int kills;
    public int rounds;
    public float totalTime;
    public bool wasModificated;
    public string email;
}

// Estructura para procesar respuesta del servidor
[System.Serializable]
public class ResponseData2
{
    public string message;
}
