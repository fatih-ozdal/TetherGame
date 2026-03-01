using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float pausedMusicVolume = 0.25f;
    private float originalMusicVolume = 1f;

    private void Start()
    {
        // Hide pause menu at start
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Store original music volume if assigned
        if (musicSource != null)
        {
            originalMusicVolume = musicSource.volume;
        }
    }

    private void Update()
    {
        // ESC key to pause/resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        GameIsPaused = true;

        // Lower music volume if assigned
        if (musicSource != null)
        {
            musicSource.volume = originalMusicVolume * pausedMusicVolume;
        }
    }

    public void Resume()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        GameIsPaused = false;

        // Restore music volume if assigned
        if (musicSource != null)
        {
            musicSource.volume = originalMusicVolume;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        // Change "MainMenu" to your main menu scene name
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

        // For Unity Editor testing
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
