using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 30f;
    [SerializeField] private Transform player;
    [SerializeField] private float fallYThreshold = -2f;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text messageText;

    private int totalStars;
    private int collectedStars;
    private float timeRemaining;
    private bool gameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        timeRemaining = timeLimit;

        if (messageText != null)
            messageText.text = "";

        totalStars = FindObjectsOfType<CollectibleStar>().Length;
        Debug.Log($"Total stars in scene: {totalStars}");
    }

    public void OnStarCollected()
    {
        if (gameOver) return;

        collectedStars++;
        Debug.Log($"Stars collected: {collectedStars}/{totalStars}");

        if (collectedStars >= totalStars)
        {
            WinGame();
        }
    }

    private void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            return;
        }
        
        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0f) timeRemaining = 0f;

        if (timerText != null)
            timerText.text = $"Time: {timeRemaining:0.0}";

        if (timeRemaining <= 0f)
        {
            LoseGame("Time's up!");
        }

        if (player != null && player.position.y < fallYThreshold)
        {
            LoseGame("You fell off!");
        }
    }

    private void WinGame()
    {
        gameOver = true;
        Time.timeScale = 0f;
        if (messageText != null)
            messageText.text = "You Win! Press R to restart.";
        Debug.Log("WIN");
    }

    private void LoseGame(string reason)
    {
        if (gameOver) return;

        gameOver = true;
        Time.timeScale = 0f;
        if (messageText != null)
            messageText.text = $"{reason} Press R to restart.";
        Debug.Log("LOSE: " + reason);
    }
}
