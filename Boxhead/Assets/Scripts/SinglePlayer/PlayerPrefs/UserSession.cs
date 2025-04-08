using UnityEngine;

public static class UserSession
{
    // Clave utilizada para almacenar y recuperar el correo electr�nico del jugador
    private const string EmailKey = "UserEmail";

    // M�todo que verifica si el usuario est� logueado, comprobando si existe la clave del correo electr�nico en PlayerPrefs
    public static bool IsUserLoggedIn()
    {
        return PlayerPrefs.HasKey(EmailKey);  // Devuelve verdadero si la clave de correo electr�nico existe
    }

    // M�todo que guarda el correo electr�nico del usuario en PlayerPrefs
    public static void SaveUserEmail(string email)
    {
        PlayerPrefs.SetString(EmailKey, email);  // Guarda el correo electr�nico con la clave "UserEmail"
        PlayerPrefs.Save();  // Guarda de forma permanente los datos en PlayerPrefs
    }

    // M�todo que obtiene el correo electr�nico del usuario desde PlayerPrefs
    public static string GetUserEmail()
    {
        // Si el usuario est� logueado (existe la clave "UserEmail")
        if (IsUserLoggedIn())
        {
            return PlayerPrefs.GetString(EmailKey);  // Devuelve el correo electr�nico guardado
        }
        else
        {
            return null;  // Si no est� logueado, devuelve null
        }
    }

    // M�todo que elimina el correo electr�nico guardado en PlayerPrefs
    public static void DeleteEmail()
    {
        PlayerPrefs.DeleteKey(EmailKey);  // Elimina la clave "UserEmail" de PlayerPrefs
    }

    // Clase interna serializable que almacena los datos de sesi�n (en este caso, solo el correo electr�nico)
    [System.Serializable]
    public class SessionData
    {
        public string email;  // El correo electr�nico del usuario
    }
}

