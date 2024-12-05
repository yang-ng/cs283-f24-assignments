using UnityEngine;
using TMPro; // Required for TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance

    public TextMeshProUGUI scoreText; // TextMeshPro for the score display

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
        }
        else if (teamName == "Opponent")
        {
            teamBScore++;
        }

        // Update the score display
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Player {teamAScore}:{teamBScore} Opponent";
    }
}