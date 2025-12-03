using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 30f;
    [SerializeField] private Transform player;          // drag your Robot root here
    [SerializeField] private float fallYThreshold = -2f;

    [Header("UI")]
    [SerializeField] private TMP_Text timerText;        // TimerText
    [SerializeField] private TMP_Text messageText;      // MessageText
    [SerializeField] private GameObject startScreenPanel; // StartScreenPanel
    [SerializeField] private GameObject pauseMenuPanel;   // PauseMenuPanel

    [Header("Cameras")]
    [SerializeField] private GameObject playerFollowCamera; // PlayerFollowCamera (Cinemachine virtual cam)

    private int totalStars;
    private int collectedStars;
    private float timeRemaining;
    private bool gameOver = false;
    private bool gameStarted = false;
    private bool isPaused = false;

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

        // --- START SCREEN SETUP ---
        Time.timeScale = 0f;
        gameStarted = false;
        isPaused = false;

        if (startScreenPanel != null)
            startScreenPanel.SetActive(true);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // disable player until game actually starts
        if (player != null)
            player.gameObject.SetActive(false);

        // camera can stay active to show the scene
        if (playerFollowCamera != null)
            playerFollowCamera.SetActive(true);

        // show cursor for start screen
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // called by stars when collected
    public void OnStarCollected()
    {
        if (gameOver || !gameStarted) return;

        collectedStars++;
        Debug.Log($"Stars collected: {collectedStars}/{totalStars}");

        if (collectedStars >= totalStars)
        {
            WinGame();
        }
    }

    private void Update()
    {
        // --- START SCREEN STATE ---
        if (!gameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }
            return;
        }

        // --- GAME OVER STATE ---
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            return;
        }

        // --- PAUSED STATE ---
        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ResumeGame();
            }
            return;
        }

        // check for pause toggle while playing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
            return;
        }

        // --- PLAYING STATE ---

        // timer
        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0f) timeRemaining = 0f;

        if (timerText != null)
            timerText.text = $"Time: {timeRemaining:0.0}";

        if (timeRemaining <= 0f)
        {
            LoseGame("Time's up!");
        }

        // fell off the stage?
        if (player != null && player.position.y < fallYThreshold)
        {
            LoseGame("You fell off!");
        }
    }

    // ================== STATE HELPERS ==================

    private void StartGame()
    {
        gameStarted = true;
        isPaused = false;
        Time.timeScale = 1f;

        if (startScreenPanel != null)
            startScreenPanel.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (player != null)
            player.gameObject.SetActive(true);

        if (playerFollowCamera != null)
            playerFollowCamera.SetActive(true);

        // hide/lock cursor during gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (playerFollowCamera != null)
            playerFollowCamera.SetActive(false); // stop Cinemachine while paused

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (playerFollowCamera != null)
            playerFollowCamera.SetActive(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void WinGame()
    {
        gameOver = true;
        Time.timeScale = 0f;
        if (messageText != null)
            messageText.text = "You Win! Press R to restart.";
        Debug.Log("WIN");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void LoseGame(string reason)
    {
        if (gameOver) return;

        gameOver = true;
        Time.timeScale = 0f;
        if (messageText != null)
            messageText.text = $"{reason} Press R to restart.";
        Debug.Log("LOSE: " + reason);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // ============== UI CALLBACKS ==============

    // Called by the VolumeSlider (On Value Changed)
    public void SetVolume(float value)
    {
        // value between 0 and 1
        AudioListener.volume = value;
    }
}
