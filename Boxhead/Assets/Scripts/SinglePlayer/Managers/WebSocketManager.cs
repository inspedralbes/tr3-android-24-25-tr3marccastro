using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class WebSocketManager : MonoBehaviour
{
    // Instancia est�tica para acceder al WebSocketManager desde cualquier lugar
    public static WebSocketManager Instance;

    // Referencia al objeto WebSocket que se utilizar� para la comunicaci�n con el servidor
    private WebSocket websocket;

    // Referencias a otros componentes que gestionan las estad�sticas de enemigos y del jugador
    public EnemyStatsManager statsManager;
    public PlayerController playerController;

    // Objeto para gestionar los mensajes que se reciben desde el WebSocket
    public WebSocketMessage statsSocket;

    // Variable para gestionar si la ronda est� en pausa
    private bool isRoundPaused = false;

    // Indicador si las estad�sticas han sido modificadas
    public bool isModificated = false;

    // URL del servidor WebSocket
    private string serverUrl = "ws://localhost:3002";

    // M�todo que se ejecuta cuando el objeto se instancia
    private void Awake()
    {
        // Si no existe una instancia, la asignamos, sino destruimos el objeto
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // M�todo que se ejecuta al comenzar el juego
    private async void Start()
    {
        // Inicia la conexi�n WebSocket de forma as�ncrona
        await ConnectWebSocket();
    }

    // M�todo para conectar el WebSocket al servidor
    private async Task ConnectWebSocket()
    {
        // Inicializa el WebSocket con la URL del servidor
        websocket = new WebSocket(serverUrl);

        // Evento que ocurre cuando se abre la conexi�n WebSocket
        websocket.OnOpen += () =>
        {
            // Env�a un mensaje de registro al servidor cuando la conexi�n se abre
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

        // Establece la conexi�n con el servidor WebSocket de manera as�ncrona
        await websocket.Connect();
    }

    // M�todo que se ejecuta en cada frame para despachar el procesamiento de mensajes WebSocket
    private void Update()
    {
        // Llama a DispatchMessageQueue para procesar los mensajes recibidos
        websocket?.DispatchMessageQueue();
    }

    // M�todo para procesar el mensaje recibido desde el servidor
    private void ProcessMessage(string message)
    {
        // Si el mensaje es vac�o o nulo, no hacer nada
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        // Deserializa el mensaje JSON a un objeto WebSocketMessage
        statsSocket = JsonUtility.FromJson<WebSocketMessage>(message);

        // Si el mensaje deserializado es nulo o el evento no est� presente, no hacer nada
        if (statsSocket == null || string.IsNullOrEmpty(statsSocket.event_unity))
        {
            return;
        }
    }

    // M�todo que aplica las estad�sticas si fueron actualizadas en el servidor
    public void ApplyStatsIfUpdated(int currentRound)
    {
        // Si el evento del mensaje es "stats-update", se actualizan las estad�sticas
        if (statsSocket.event_unity == "stats-update")
        {
            // Si el nombre no es "Player", se actualizan las estad�sticas de un enemigo
            if (statsSocket.payload.name != "Player")
            {
                // Actualiza las estad�sticas del enemigo con los valores recibidos
                EnemyStatsManager.Instance.UpdateStatsEnemy(statsSocket.payload.name, statsSocket.payload.health, statsSocket.payload.speed, statsSocket.payload.damage, statsSocket.payload.color, statsSocket.payload.save);
                isModificated = true; // Marcamos que las estad�sticas fueron modificadas
            }
            else
            {
                // Si es el jugador, se actualizan sus estad�sticas
                playerController.UpdateStatsPlayer(statsSocket.payload.health, statsSocket.payload.speed);
                isModificated = true; // Marcamos que las estad�sticas fueron modificadas
            }
        }
        // Si el evento es "stats_restart", se reinician las estad�sticas de los enemigos
        else if (statsSocket.event_unity == "stats_restart")
        {
            EnemyStatsManager.Instance.ResetToDefault();
        }
        else
        {
            // Si no es una actualizaci�n de estad�sticas, se actualizan las estad�sticas para las rondas
            statsManager.UpdateEnemyStatsForRounds(currentRound);
        }
    }

    // M�todo para pausar o reanudar la ronda
    public void SetRoundPause(bool pause, int currentRound)
    {
        // Establece si la ronda est� pausada
        isRoundPaused = pause;

        // Si la ronda est� pausada, se aplican las estad�sticas actualizadas
        if (isRoundPaused)
        {
            ApplyStatsIfUpdated(currentRound);
        }
    }

    // M�todo que se ejecuta cuando la aplicaci�n se cierra
    private async void OnApplicationQuit()
    {
        // Si hay una conexi�n WebSocket abierta, se cierra de forma as�ncrona
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
        public CharacterStatsFromWebSocket payload; // Datos del mensaje (estad�sticas del personaje)
    }

    // Clase serializable que representa las estad�sticas de un personaje recibidas por WebSocket
    [System.Serializable]
    public class CharacterStatsFromWebSocket
    {
        public bool save;   // Indica si se deben guardar las estad�sticas
        public string name; // Nombre del personaje
        public int health;  // Salud del personaje
        public float speed; // Velocidad del personaje
        public int damage;  // Da�o del personaje
        public string color; // Color del personaje
    }
}