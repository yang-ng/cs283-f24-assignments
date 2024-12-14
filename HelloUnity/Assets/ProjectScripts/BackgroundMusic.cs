using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        // Ensure there's only one BackgroundMusicManager in the scene
        if (FindObjectsOfType<BackgroundMusic>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Prevent this object from being destroyed between scenes
        DontDestroyOnLoad(gameObject);

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Start playing the music
        PlayMusic();
    }

    void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
