using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuScreen : MonoBehaviour
{
    private Button startButton, shopButton, logoutButton, exitButton;
    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        string email = UserSession.GetUserEmail();

        startButton = root.Q<Button>("StartButton");
        exitButton = root.Q<Button>("ExitButton");
        logoutButton = root.Q<Button>("LogoutButton");
        shopButton = root.Q<Button>("ShopButton");

        logoutButton.style.display = string.IsNullOrEmpty(email) ? DisplayStyle.None : DisplayStyle.Flex;

        startButton.clicked += () => SceneManager.LoadScene("SinglePlayerGameScene");
        logoutButton.clicked += () => { UserSession.DeleteEmail(); logoutButton.style.display = DisplayStyle.None; };
        shopButton.clicked += () => SceneManager.LoadScene("LoginScene");
        exitButton.clicked += ExitGame;
    }

    private void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
