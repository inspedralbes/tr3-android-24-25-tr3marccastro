using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LobbyScene : MonoBehaviour
{
    private ListView playersList;
    private List<string> playerNames = new List<string> { "Player1", "Player2", "Player3" }; // Simulación de jugadores

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        playersList = root.Q<ListView>("playersList");
        Button startGameButton = root.Q<Button>("StartButton");
        Button backButton = root.Q<Button>("BackButton");

        // Configurar ListView
        playersList.makeItem = () => new Label(); // Crear un Label para cada ítem
        playersList.bindItem = (element, index) => 
        {
            Label label = (Label)element;
            label.text = playerNames[index];
            label.AddToClassList("player-item"); // Aplicar estilo
        };
        playersList.itemsSource = playerNames;

        // Eventos de botones
        startGameButton.clicked += StartGame;
        backButton.clicked += BackToMenu;
    }

    private void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Cambia "GameScene" por la escena del juego
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("MultiplayerScene"); // Cambia "MainMenu" por tu escena de menú
    }
}
