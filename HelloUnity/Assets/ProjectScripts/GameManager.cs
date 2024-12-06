using UnityEngine;
using TMPro; // Required for TextMeshPro
using System.Collections; // Required for Coroutine

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance

    public TextMeshProUGUI scoreText; // TextMeshPro for the score display
    public TextMeshProUGUI goalText; // TextMeshPro for the goal message

    private int teamAScore = 0;
    private int teamBScore = 0;

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

    public void RegisterGoal(string teamName)
    {
        if (teamName == "Player")
        {
            teamAScore++;
            ShowGoalMessage("Goooooal!");
        }
        else if (teamName == "Opponent")
        {
            teamBScore++;
            ShowGoalMessage("Goal!");
        }

        // Update the score display
        UpdateScoreUI();
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
}