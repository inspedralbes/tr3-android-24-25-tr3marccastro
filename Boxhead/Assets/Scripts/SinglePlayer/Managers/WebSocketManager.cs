using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class WebSocketManager : MonoBehaviour
{
    private WebSocket websocket;
    private EnemyStatsManager enemyStatsManager;

    // Dirección del servidor WebSocket
    private string serverUrl = "ws://localhost:3001"; // Cambia a la URL de tu servidor WebSocket

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
            Debug.Log("WebSocket connectat!");
        };
        // Callback: Quan es rep un error.
        websocket.OnError += (e) =>
        {
            Debug.LogError("WebSocket error: " + e);
        };
        // Callback: Quan la connexió es tanca.
        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket tancat!");
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
        EnemyStats stats = JsonUtility.FromJson<EnemyStats>(message);

        Debug.Log(stats.speed);
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }

    [System.Serializable]
    public class EnemyStats
    {
        public float speed;
        public int damage;
    }
}