using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MultiplayerScene : MonoBehaviour
{
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        Button createRoomButton = root.Q<Button>("createRoomButton");
        Button joinRoomButton = root.Q<Button>("joinRoomButton");
        Button backButton = root.Q<Button>("backButton");

        createRoomButton.clicked += CreateRoom;
        joinRoomButton.clicked += JoinRoom;
        backButton.clicked += BackToMainMenu;
    }

    private void CreateRoom()
    {
        SceneManager.LoadScene("LobbyScene"); // Cambiará a la escena del lobby
    }

    private void JoinRoom()
    {
        SceneManager.LoadScene("LobbyScene"); // En Mirror, aquí se buscaría un servidor
    }

    private void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
