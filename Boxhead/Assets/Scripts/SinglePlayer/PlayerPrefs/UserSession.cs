using UnityEngine;

public static class UserSession
{
    // Clave utilizada para almacenar y recuperar el correo electrónico del jugador
    private const string EmailKey = "UserEmail";

    // Método que verifica si el usuario está logueado, comprobando si existe la clave del correo electrónico en PlayerPrefs
    public static bool IsUserLoggedIn()
    {
        return PlayerPrefs.HasKey(EmailKey);  // Devuelve verdadero si la clave de correo electrónico existe
    }

    // Método que guarda el correo electrónico del usuario en PlayerPrefs
    public static void SaveUserEmail(string email)
    {
        PlayerPrefs.SetString(EmailKey, email);  // Guarda el correo electrónico con la clave "UserEmail"
        PlayerPrefs.Save();  // Guarda de forma permanente los datos en PlayerPrefs
    }

    // Método que obtiene el correo electrónico del usuario desde PlayerPrefs
    public static string GetUserEmail()
    {
        // Si el usuario está logueado (existe la clave "UserEmail")
        if (IsUserLoggedIn())
        {
            return PlayerPrefs.GetString(EmailKey);  // Devuelve el correo electrónico guardado
        }
        else
        {
            return null;  // Si no está logueado, devuelve null
        }
    }

    // Método que elimina el correo electrónico guardado en PlayerPrefs
    public static void DeleteEmail()
    {
        PlayerPrefs.DeleteKey(EmailKey);  // Elimina la clave "UserEmail" de PlayerPrefs
    }

    // Clase interna serializable que almacena los datos de sesión (en este caso, solo el correo electrónico)
    [System.Serializable]
    public class SessionData
    {
        public string email;  // El correo electrónico del usuario
    }
}

