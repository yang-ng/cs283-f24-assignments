using UnityEngine;
using TMPro; // Required for TextMeshPro
using UnityEngine.SceneManagement; // For scene reloading

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Assign a TextMeshPro object in the Inspector
    public TextMeshProUGUI endGameText; // Text for displaying "Full Time" and the final score
    public GameObject endGameUI; // UI panel for "Play Again" and "Quit" buttons
    public float totalGameTime = 120f; // Total game time in real seconds (2 minutes)
    private float elapsedTime = 0f; // Elapsed real time
    private bool gameRunning = true; // Is the game running?

    void Update()
    {
        if (gameRunning)
        {
            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Calculate in-game time (scaled)
            float inGameTime = Mathf.Clamp((elapsedTime / totalGameTime) * 90f, 0f, 90f);

            // Format the time as MM:SS
            int minutes = Mathf.FloorToInt(inGameTime);
            int seconds = Mathf.FloorToInt((inGameTime - minutes) * 60);
            timerText.text = $"{minutes:00}:{seconds:00}";

            // Check if the game time is up
            if (inGameTime >= 90f)
            {
                EndGame();
            }
        }
    }

    void EndGame()
    {
        gameRunning = false; // Stop the timer
        Debug.Log("Game Over!");

        GameManager.Instance.EndGame();
        
        // Display "Full Time" and final score
        endGameText.text = $"Full Time\nPlayer {GameManager.Instance.GetPlayerScore()} - {GameManager.Instance.GetOpponentScore()} Opponent";
        endGameUI.SetActive(true); // Show the end-game UI
    }

    // Called when the "Play Again" button is clicked
    public void PlayAgain()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Called when the "Quit" button is clicked
    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
