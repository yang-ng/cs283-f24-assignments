using UnityEngine;
using TMPro; // Required for TextMeshPro
using System.Collections.Generic; // Required for List<>
using System.Collections; // Required for Coroutine

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText; // TextMeshPro for the score display
    public TextMeshProUGUI goalText; // TextMeshPro for the goal message
    public TextMeshProUGUI endGameText; // TextMeshPro for the end-game message
    public GameObject endGameUI; // End-game UI for Play Again and Quit buttons

    [Header("Game Object References")]
    public Transform ballStartPosition; // Assign in the Inspector for the initial ball position
    public GameObject ball; // Reference to the ball
    public List<Transform> playerStartPositions; // Assign initial positions of player ghosts in the Inspector
    public List<GameObject> playerGhosts; // List of player ghosts
    public List<Transform> opponentStartPositions; // Assign initial positions of opponent ghosts in the Inspector
    public List<GameObject> opponentGhosts; // List of opponent ghosts

    private int teamAScore = 0; // Player's team score
    private int teamBScore = 0; // Opponent's team score
    private bool gameRunning = true; // Indicates if the game is running

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Check for Escape key press to quit the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void RegisterGoal(string teamName)
    {
        if (!gameRunning) return; // Ignore goal registration if the game has ended

        if (teamName == "Player")
        {
            teamAScore++;
            ShowGoalMessage("Goooooal!");
        }
        else if (teamName == "Opponent")
        {
            teamBScore++;
            ShowGoalMessage("Your opponent scored!");
        }

        // Update the score display
        UpdateScoreUI();

        // Reset the game after a short delay
        StartCoroutine(ResetGameAfterDelay());
    }

    public void EndGame()
    {
        gameRunning = false; // Stop the game

        // Display the final score and "Full Time" message
        endGameText.text = $"Full Time\nFinal Score: Player {teamAScore}:{teamBScore} Opponent";
        endGameUI.SetActive(true); // Show the end-game UI
    }

    public int GetPlayerScore()
    {
        return teamAScore;
    }

    public int GetOpponentScore()
    {
        return teamBScore;
    }

    public bool IsGameRunning()
    {
        return gameRunning;
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Player {teamAScore}:{teamBScore} Opponent";
    }

    private void ShowGoalMessage(string message)
    {
        goalText.text = message;
        goalText.gameObject.SetActive(true);

        // Start the animation
        StartCoroutine(AnimateGoalMessage());
    }

    private IEnumerator AnimateGoalMessage()
    {
        goalText.transform.localScale = Vector3.zero; // Start from small
        Vector3 targetScale = Vector3.one; // End at normal scale (1, 1, 1)
        float animationTime = 0.5f; // Duration of the animation

        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            // Smoothly interpolate the scale
            goalText.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final scale is set correctly
        goalText.transform.localScale = targetScale;

        // Keep the message visible for 2 seconds
        yield return new WaitForSeconds(2f);

        // Hide the message
        goalText.gameObject.SetActive(false);
    }

    private IEnumerator ResetGameAfterDelay()
    {
        yield return new WaitForSeconds(1.8f); // Wait for 2 seconds to show the goal message

        // Reset ball position and its velocity
        ball.transform.position = ballStartPosition.position;
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }

        // Reset player ghosts to their initial positions
        for (int i = 0; i < playerGhosts.Count; i++)
        {
            playerGhosts[i].transform.position = playerStartPositions[i].position;
            playerGhosts[i].transform.rotation = playerStartPositions[i].rotation;

            // Reset player ghost velocities if they have a Rigidbody
            Rigidbody ghostRb = playerGhosts[i].GetComponent<Rigidbody>();
            if (ghostRb != null)
            {
                ghostRb.velocity = Vector3.zero;
                ghostRb.angularVelocity = Vector3.zero;
            }
        }

        // Reset opponent ghosts to their initial positions
        for (int i = 0; i < opponentGhosts.Count; i++)
        {
            opponentGhosts[i].transform.position = opponentStartPositions[i].position;
            opponentGhosts[i].transform.rotation = opponentStartPositions[i].rotation;

            // Reset opponent ghost velocities if they have a Rigidbody
            Rigidbody ghostRb = opponentGhosts[i].GetComponent<Rigidbody>();
            if (ghostRb != null)
            {
                ghostRb.velocity = Vector3.zero;
                ghostRb.angularVelocity = Vector3.zero;
            }
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
