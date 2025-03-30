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
    public WebSocketMessage statsSocket;

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
        websocket = new WebSocket(serverUrl);

        websocket.OnOpen += () =>
        {
            websocket.SendText("{\"event_backend\": \"register\", \"payload\": {\"id\": \"client_unity\", \"tipo\": \"unity\"}}");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = Encoding.UTF8.GetString(bytes);

            ProcessMessage(message);
        };

        // Inicia la connexió de manera asíncrona.
        await websocket.Connect();
    }

    private void Update()
    {
        websocket?.DispatchMessageQueue();
    }

    private void ProcessMessage(string message)
    {

        if (string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("Mensaje vacío o nulo recibido.");
            return; // No proceses si el mensaje está vacío o nulo
        }

        statsSocket = JsonUtility.FromJson<WebSocketMessage>(message);

        if (statsSocket == null || string.IsNullOrEmpty(statsSocket.event_unity))
        {
            Debug.LogWarning("Evento no reconocido o mensaje mal estructurado.");
            return;
        }

        if (statsSocket.event_unity == "stats_restart")
        {
            Debug.Log("Reinicio");
            EnemyStatsManager.Instance.ResetToDefault();
        }
        Debug.Log("Recibido mensaje con nombre: " + statsSocket.payload.name);
    }

    public void ApplyStatsIfUpdated(int currentRound)
    {
        if (!string.IsNullOrEmpty(statsSocket.payload.name))
        {
            if (statsSocket.payload.name != "Player")
            {
                Debug.Log("Aplicando estadísticas de Web Socket...");
                statsManager.UpdateStatsEnemy(statsSocket.payload.name, statsSocket.payload.health, statsSocket.payload.speed, statsSocket.payload.damage, statsSocket.payload.color, statsSocket.payload.save);
                Debug.Log("Estadísticas aplicadas.");
            }
            else
            {
                Debug.Log("Aplicando estadísticas de Web Socket al Player...");
                playerController.UpdateStatsPlayer(statsSocket.payload.health, statsSocket.payload.speed);
            }
        }
        else
        {
            // Actualizar estadísticas con valores incrementados
            statsManager.UpdateEnemyStatsForRounds(currentRound);
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
    public class WebSocketMessage
    {
        public string event_unity;
        public CharacterStatsFromWebSocket payload;
    }

    [System.Serializable]
    public class CharacterStatsFromWebSocket
    {
        public bool save;
        public string name;
        public int health;
        public float speed;
        public int damage;
        public string color;
    }
}