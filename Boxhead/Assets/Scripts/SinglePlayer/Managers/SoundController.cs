using UnityEngine;

public class SoundController : MonoBehaviour
{
    // Instancia estática del SoundController para garantizar que solo haya una instancia en todo el juego
    public static SoundController _instance;

    // Propiedad estática para acceder a la instancia del SoundController desde cualquier lugar
    public static SoundController Instance { get { return _instance; } }

    // Referencia al componente AudioSource que se usa para reproducir sonidos
    private AudioSource audioSource;

    // Método que se ejecuta cuando el objeto se instancia en la escena
    private void Awake()
    {
        // Si ya existe una instancia y no es esta, destruye este objeto (asegurando una sola instancia)
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Si no existe una instancia, asigna esta instancia como la única válida
            _instance = this;

            // No destruir el objeto cuando se cambie de escena
            DontDestroyOnLoad(gameObject);
        }

        // Obtiene el componente AudioSource del objeto para poder reproducir sonidos
        audioSource = GetComponent<AudioSource>();
    }

    // Método para ejecutar un sonido específico usando el AudioSource
    public void ExecuteSound(AudioClip sound)
    {
        // Reproduce el sonido (AudioClip) pasado como parámetro una sola vez
        audioSource.PlayOneShot(sound);
    }
}