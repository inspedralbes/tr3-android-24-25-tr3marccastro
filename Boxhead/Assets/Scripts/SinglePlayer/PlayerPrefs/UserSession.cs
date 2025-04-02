using UnityEngine;

public static class UserSession
{
    private const string EmailKey = "UserEmail";

    public static bool IsUserLoggedIn()
    {
        return PlayerPrefs.HasKey(EmailKey);
    }

    public static void SaveUserEmail(string email)
    {
        PlayerPrefs.SetString(EmailKey, email);
        PlayerPrefs.Save();
    }
    public static string GetUserEmail()
    {
        if (IsUserLoggedIn())
        {
            return PlayerPrefs.GetString(EmailKey);
        }
        else
        {
            return null;
        }
    }
    public static void DeleteEmail()
    {
        PlayerPrefs.DeleteKey(EmailKey);
    }

    [System.Serializable]
    public class SessionData
    {
        public string email;
    }
}
