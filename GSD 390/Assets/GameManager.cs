using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 90f;
    [SerializeField] private Transform player;
    [SerializeField] private float fallYThreshold = -2f;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Scenes")]
    [SerializeField] private int titleSceneBuildIndex = 0;
    [SerializeField] private int gameCompleteBuildIndex = 3;
    [SerializeField] private int lastPlayableLevelBuildIndex = 2;

    private int totalStars;
    private int collectedStars;
    private float timeRemaining;

    private bool gameOver = false;
    private bool isPaused = false;
    private bool hasWon = false;

    private const string WIN_TEXT =
        "You Win!\nPress R to play again\nPress T to back to title\nPress C to continue";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        timeRemaining = timeLimit;

        if (messageText != null) messageText.text = "";
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        totalStars = FindObjectsOfType<CollectibleStar>(true).Length;
        collectedStars = 0;

        LockCursorForPlay();
    }

    public void OnStarCollected()
    {
        if (gameOver) return;

        collectedStars++;
        if (collectedStars >= totalStars)
            WinGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) { GoToTitle(); return; }
        if (Input.GetKeyDown(KeyCode.R)) { RestartLevel(); return; }

        if (hasWon && Input.GetKeyDown(KeyCode.C))
        {
            ContinueToNext();
            return;
        }

        if (!gameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
            return;
        }

        if (isPaused || gameOver) return;

        // Timer
        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0f) timeRemaining = 0f;

        if (timerText != null)
            timerText.text = $"Time: {timeRemaining:0.0}";

        if (timeRemaining <= 0f)
            LoseGame("Time's up!");

        if (player != null && player.position.y < fallYThreshold)
            LoseGame("You fell off!");
    }

    private void WinGame()
    {
        gameOver = true;
        hasWon = true;
        Time.timeScale = 0f;

        if (messageText != null)
            messageText.text = WIN_TEXT;

        UnlockCursorForUI();
    }

    private void LoseGame(string reason)
    {
        if (gameOver) return;

        gameOver = true;
        hasWon = false;
        Time.timeScale = 0f;

        if (messageText != null)
            messageText.text = $"{reason}\nPress R to restart\nPress T to title";

        UnlockCursorForUI();
    }

    private void ContinueToNext()
    {
        Time.timeScale = 1f;

        int cur = SceneManager.GetActiveScene().buildIndex;

        if (cur >= lastPlayableLevelBuildIndex)
        {
            SceneManager.LoadScene(gameCompleteBuildIndex);
            return;
        }

        SceneManager.LoadScene(cur + 1);
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        UnlockCursorForUI();
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        LockCursorForPlay();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        LockCursorForPlay();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToTitle()
    {
        Time.timeScale = 1f;
        UnlockCursorForUI();
        SceneManager.LoadScene(titleSceneBuildIndex);
    }

    public void RestartFromButton() => RestartLevel();
    public void ResumeFromButton() => ResumeGame();

    public void SetVolume(float value) => AudioListener.volume = value;

    private void LockCursorForPlay()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursorForUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
