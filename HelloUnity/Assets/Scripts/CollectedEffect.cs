using UnityEngine;

public class CollectedEffect : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip collectSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on the collectible object");
        }
        else if (collectSound != null)
        {
            audioSource.clip = collectSound;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}
