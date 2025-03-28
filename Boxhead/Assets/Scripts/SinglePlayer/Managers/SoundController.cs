using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController _instance;
    public static SoundController Instance {get {return _instance;}}
    private AudioSource audioSource;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void ExecuteSound(AudioClip sound) {
        audioSource.PlayOneShot(sound);
    }

    
}
