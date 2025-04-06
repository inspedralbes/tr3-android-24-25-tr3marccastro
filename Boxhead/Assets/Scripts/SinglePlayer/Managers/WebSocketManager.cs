using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class WebSocketManager : MonoBehaviour
{
    // Instancia estática para acceder al WebSocketManager desde cualquier lugar
    public static WebSocketManager Instance;

    // Referencia al objeto WebSocket que se utilizará para la comunicación con el servidor
    private WebSocket websocket;

    // Referencias a otros componentes que gestionan las estadísticas de enemigos y del jugador
    public EnemyStatsManager statsManager;
    public PlayerController playerController;

    // Objeto para gestionar los mensajes que se reciben desde el WebSocket
    public WebSocketMessage statsSocket;

    // Variable para gestionar si la ronda está en pausa
    private bool isRoundPaused = false;

    // Indicador si las estadísticas han sido modificadas
    public bool isModificated = false;

    // URL del servidor WebSocket
    private string serverUrl = "ws://localhost:3002";

    // Método que se ejecuta cuando el objeto se instancia
    private void Awake()
    {
        // Si no existe una instancia, la asignamos, sino destruimos el objeto
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Método que se ejecuta al comenzar el juego
    private async void Start()
    {
        // Inicia la conexión WebSocket de forma asíncrona
        await ConnectWebSocket();
    }

    // Método para conectar el WebSocket al servidor
    private async Task ConnectWebSocket()
    {
        // Inicializa el WebSocket con la URL del servidor
        websocket = new WebSocket(serverUrl);

        // Evento que ocurre cuando se abre la conexión WebSocket
        websocket.OnOpen += () =>
        {
            // Envía un mensaje de registro al servidor cuando la conexión se abre
            websocket.SendText("{\"event_backend\": \"register\", \"payload\": {\"id\": \"client_unity\", \"tipo\": \"unity\"}}");
        };

        // Evento que ocurre cuando se recibe un mensaje desde el servidor
        websocket.OnMessage += (bytes) =>
        {
            // Convierte el mensaje recibido a un string
            string message = Encoding.UTF8.GetString(bytes);

            // Procesa el mensaje recibido
            ProcessMessage(message);
        };

        // Establece la conexión con el servidor WebSocket de manera asíncrona
        await websocket.Connect();
    }

    // Método que se ejecuta en cada frame para despachar el procesamiento de mensajes WebSocket
    private void Update()
    {
        // Llama a DispatchMessageQueue para procesar los mensajes recibidos
        websocket?.DispatchMessageQueue();
    }

    // Método para procesar el mensaje recibido desde el servidor
    private void ProcessMessage(string message)
    {
        // Si el mensaje es vacío o nulo, no hacer nada
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        // Deserializa el mensaje JSON a un objeto WebSocketMessage
        statsSocket = JsonUtility.FromJson<WebSocketMessage>(message);

        // Si el mensaje deserializado es nulo o el evento no está presente, no hacer nada
        if (statsSocket == null || string.IsNullOrEmpty(statsSocket.event_unity))
        {
            return;
        }
    }

    // Método que aplica las estadísticas si fueron actualizadas en el servidor
    public void ApplyStatsIfUpdated(int currentRound)
    {
        // Si el evento del mensaje es "stats-update", se actualizan las estadísticas
        if (statsSocket.event_unity == "stats-update")
        {
            // Si el nombre no es "Player", se actualizan las estadísticas de un enemigo
            if (statsSocket.payload.name != "Player")
            {
                // Actualiza las estadísticas del enemigo con los valores recibidos
                EnemyStatsManager.Instance.UpdateStatsEnemy(statsSocket.payload.name, statsSocket.payload.health, statsSocket.payload.speed, statsSocket.payload.damage, statsSocket.payload.color, statsSocket.payload.save);
                isModificated = true; // Marcamos que las estadísticas fueron modificadas
            }
            else
            {
                // Si es el jugador, se actualizan sus estadísticas
                playerController.UpdateStatsPlayer(statsSocket.payload.health, statsSocket.payload.speed);
                isModificated = true; // Marcamos que las estadísticas fueron modificadas
            }
        }
        // Si el evento es "stats_restart", se reinician las estadísticas de los enemigos
        else if (statsSocket.event_unity == "stats_restart")
        {
            EnemyStatsManager.Instance.ResetToDefault();
        }
        else
        {
            // Si no es una actualización de estadísticas, se actualizan las estadísticas para las rondas
            statsManager.UpdateEnemyStatsForRounds(currentRound);
        }
    }

    // Método para pausar o reanudar la ronda
    public void SetRoundPause(bool pause, int currentRound)
    {
        // Establece si la ronda está pausada
        isRoundPaused = pause;

        // Si la ronda está pausada, se aplican las estadísticas actualizadas
        if (isRoundPaused)
        {
            ApplyStatsIfUpdated(currentRound);
        }
    }

    // Método que se ejecuta cuando la aplicación se cierra
    private async void OnApplicationQuit()
    {
        // Si hay una conexión WebSocket abierta, se cierra de forma asíncrona
        if (websocket != null)
        {
            await websocket.Close();
        }
    }

    // Clase serializable que representa el mensaje recibido por WebSocket
    [System.Serializable]
    public class WebSocketMessage
    {
        public string event_unity; // El evento que se recibe del servidor
        public CharacterStatsFromWebSocket payload; // Datos del mensaje (estadísticas del personaje)
    }

    // Clase serializable que representa las estadísticas de un personaje recibidas por WebSocket
    [System.Serializable]
    public class CharacterStatsFromWebSocket
    {
        public bool save;   // Indica si se deben guardar las estadísticas
        public string name; // Nombre del personaje
        public int health;  // Salud del personaje
        public float speed; // Velocidad del personaje
        public int damage;  // Daño del personaje
        public string color; // Color del personaje
    }
}