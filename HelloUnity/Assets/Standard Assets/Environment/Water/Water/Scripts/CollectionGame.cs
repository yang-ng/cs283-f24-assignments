using UnityEngine;
using UnityEngine.UI;  // Required for working with UI elements

public class CollectionGame : MonoBehaviour
{
    public Text scoreText;  // Reference to the UI Text component for displaying the score
    private int score = 0;  // Variable to keep track of the score

    void Start()
    {
        // Initialize the score display when the game starts
        UpdateScoreUI();
    }

    // This method is called when another collider enters the trigger collider attached to this GameObject
    void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided has the "Collectible" tag
        if (other.gameObject.CompareTag("Collectible"))
        {
            // Hide the collectible object by deactivating it
            other.gameObject.SetActive(false);

            // Increase the score and update the UI
            score++;
            UpdateScoreUI();
        }
    }

    // Method to update the score display on the UI
    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }
}
