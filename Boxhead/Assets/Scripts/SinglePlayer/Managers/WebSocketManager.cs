using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class WebSocketManager : MonoBehaviour
{
    public static WebSocketManager Instance;
    private WebSocket websocket;
    // public ZombieController zombieController;
    public EnemyStatsManager statsManager;
    public PlayerController playerController;
    public CharacterStatsFromWebSocket statsSocket;

    private bool isRoundPaused = false;

    // Dirección del servidor WebSocket
    private string serverUrl = "ws://localhost:3002"; // Cambia a la URL de tu servidor WebSocket

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private async void Start()
    {
        // Inicialitza la connexió al WebSocket.
        await ConnectWebSocket();
    }

    private async Task ConnectWebSocket()
    {
        // Crea una nova instància de WebSocket amb la URL
        websocket = new WebSocket(serverUrl);
        // Callback: Quan la connexió s’obre.
        websocket.OnOpen += () =>
        {
            // Debug.Log("WebSocket connectat!");
        };
        // Callback: Quan es rep un error.
        websocket.OnError += (e) =>
        {
            // Debug.LogError("WebSocket error: " + e);
        };
        // Callback: Quan la connexió es tanca.
        websocket.OnClose += (e) =>
        {
            // Debug.Log("WebSocket tancat!");
        };
        // Callback: Quan es rep un missatge.
        websocket.OnMessage += (bytes) =>
        {
        // Converteix els bytes a una cadena.
            string message = Encoding.UTF8.GetString(bytes);
            // Debug.Log("Missatge rebut: " + message);
            ProcessMessage(message);
        };

        // Inicia la connexió de manera asíncrona.
        await websocket.Connect();
    }

    private void Update()
    {
        if (websocket != null)
        {
            websocket.DispatchMessageQueue();
        }
    }

    private void ProcessMessage(string message) {

        if (string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("Mensaje vacío o nulo recibido.");
            return; // No proceses si el mensaje está vacío o nulo
        }

        statsSocket = JsonUtility.FromJson<CharacterStatsFromWebSocket>(message);

        if (statsSocket == null)
        {
            Debug.LogWarning("Deserialización fallida. statsSocket es null.");
            return; // Si no se deserializa correctamente, no procedas.
        }

        Debug.Log("Recibido mensaje con nombre: " + statsSocket.name);
    }

    public void ApplyStatsIfUpdated(int currentRound)
    {
        if(!string.IsNullOrEmpty(statsSocket.name)) 
        {
            if (statsSocket.name != "Player")
            {
                Debug.Log("Aplicando estadísticas de Web Socket...");
                statsManager.UpdateEnemyStats(statsSocket.name, statsSocket.health, statsSocket.speed, statsSocket.damage, statsSocket.color, currentRound);
                Debug.Log("Estadísticas aplicadas.");
            }
            else {
                Debug.Log("Aplicando estadísticas de Web Socket al Player...");
                playerController.UpdateStatsPlayer(statsSocket.health, statsSocket.speed);
            }
        }
        else
        {
            // Actualizar estadísticas con valores incrementados
            statsManager.UpdateEnemyStats(statsSocket.name, statsSocket.health, statsSocket.speed, statsSocket.damage, statsSocket.color, currentRound);
        }
    }

    // Esto debe ejecutarse durante la pausa entre rondas (10s de descanso)
    public void SetRoundPause(bool pause, int currentRound)
    {
        Debug.Log(pause);
        isRoundPaused = pause;
        if (isRoundPaused)
        {
            ApplyStatsIfUpdated(currentRound); // Aplicamos las nuevas stats durante la pausa
        }
        else
        {
            if (statsSocket == null) Debug.LogWarning("statsSocket es null, no se pueden aplicar las estadísticas.");
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            // enemyStatsManager.UpdateStats(1, 1);
            await websocket.Close();
        }
    }

    [System.Serializable]
    public class CharacterStatsFromWebSocket
    {
        public string name;
        public int health;
        public float speed;
        public int damage;
        public string color;
    }
}