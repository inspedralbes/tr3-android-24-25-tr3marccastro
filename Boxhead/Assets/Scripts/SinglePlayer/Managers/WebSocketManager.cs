using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class WebSocketManager : MonoBehaviour
{
    public static WebSocketManager Instance;
    private WebSocket websocket;
    public EnemyStatsManager statsManager;
    public PlayerController playerController;
    public WebSocketMessage statsSocket;

    private bool isRoundPaused = false;
    public bool isModificated = false;

    private string serverUrl = "ws://localhost:3002";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private async void Start()
    {
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
            return;
        }

        statsSocket = JsonUtility.FromJson<WebSocketMessage>(message);

        if (statsSocket == null || string.IsNullOrEmpty(statsSocket.event_unity))
        {
            return;
        }
    }

    public void ApplyStatsIfUpdated(int currentRound)
    {
        if (statsSocket.event_unity == "stats-update")
        {
            if (statsSocket.payload.name != "Player")
            {
                EnemyStatsManager.Instance.UpdateStatsEnemy(statsSocket.payload.name, statsSocket.payload.health, statsSocket.payload.speed, statsSocket.payload.damage, statsSocket.payload.color, statsSocket.payload.save);
                isModificated = true;
            }
            else
            {
                playerController.UpdateStatsPlayer(statsSocket.payload.health, statsSocket.payload.speed);
                isModificated= true;
            }
        }
        else if(statsSocket.event_unity == "stats_restart") {
            EnemyStatsManager.Instance.ResetToDefault();
        }
        else
        {
            statsManager.UpdateEnemyStatsForRounds(currentRound);
        }
    }

    public void SetRoundPause(bool pause, int currentRound)
    {
        isRoundPaused = pause;
        if (isRoundPaused)
        {
            ApplyStatsIfUpdated(currentRound);
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