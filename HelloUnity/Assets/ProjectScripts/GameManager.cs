using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI endGameText;
    public GameObject endGameUI;

    [Header("Game Object References")]
    public Transform ballStartPosition;
    public GameObject ball;
    public List<Transform> playerStartPositions;
    public List<GameObject> playerGhosts;
    public List<Transform> opponentStartPositions;
    public List<GameObject> opponentGhosts;

    private int teamAScore = 0;
    private int teamBScore = 0;
    private bool gameRunning = true;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void RegisterGoal(string teamName)
    {
        if (!gameRunning) return;

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

        UpdateScoreUI();
        StartCoroutine(ResetGameAfterDelay());
    }

    public void EndGame()
    {
        gameRunning = false;
        endGameText.text = $"Full Time\nPlayer {teamAScore}:{teamBScore} Opponent";
        endGameUI.SetActive(true);
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
        StartCoroutine(AnimateGoalMessage());
    }

    private IEnumerator AnimateGoalMessage()
    {
        goalText.transform.localScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;
        float animationTime = 0.5f;

        float elapsedTime = 0f;
        while (elapsedTime < animationTime)
        {
            goalText.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        goalText.transform.localScale = targetScale;
        yield return new WaitForSeconds(2f); // keep the message visible for 2 second
        goalText.gameObject.SetActive(false); // hide the message
    }

    private IEnumerator ResetGameAfterDelay()
    {
        yield return new WaitForSeconds(1.8f);

        ball.transform.position = ballStartPosition.position;
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }

        for (int i = 0; i < playerGhosts.Count; i++)
        {
            playerGhosts[i].transform.position = playerStartPositions[i].position;
            playerGhosts[i].transform.rotation = playerStartPositions[i].rotation;

            Rigidbody ghostRb = playerGhosts[i].GetComponent<Rigidbody>();
            if (ghostRb != null)
            {
                ghostRb.velocity = Vector3.zero;
                ghostRb.angularVelocity = Vector3.zero;
            }
        }

        for (int i = 0; i < opponentGhosts.Count; i++)
        {
            opponentGhosts[i].transform.position = opponentStartPositions[i].position;
            opponentGhosts[i].transform.rotation = opponentStartPositions[i].rotation;

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