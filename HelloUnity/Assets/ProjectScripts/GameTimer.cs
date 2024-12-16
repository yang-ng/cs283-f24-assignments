using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI endGameText;
    public GameObject endGameUI; 
    public float totalGameTime = 120f; // total game time in real seconds
    private float elapsedTime = 0f;
    private bool gameRunning = true;

    void Update()
    {
        if (gameRunning)
        {
            elapsedTime += Time.deltaTime;
            float inGameTime = Mathf.Clamp((elapsedTime / totalGameTime) * 90f, 0f, 90f);

            int minutes = Mathf.FloorToInt(inGameTime);
            int seconds = Mathf.FloorToInt((inGameTime - minutes) * 60);
            timerText.text = $"{minutes:00}:{seconds:00}";

            if (inGameTime >= 90f)
            {
                EndGame();
            }
        }
    }

    void EndGame()
    {
        gameRunning = false;
        GameManager.Instance.EndGame();
        endGameText.text = $"Full Time\nPlayer {GameManager.Instance.GetPlayerScore()} - {GameManager.Instance.GetOpponentScore()} Opponent";
        endGameUI.SetActive(true);
    }

    public void PlayAgain()
    {
        // reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}