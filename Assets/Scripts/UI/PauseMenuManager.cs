using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    /// <summary>
    /// Pause menu controller for in-game pause functionality.
    /// </summary>
    public class PauseMenuManager : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject pauseMenuPanel;
        
        [Header("Buttons")]
        public Button resumeButton;
        public Button restartButton;
        public Button mainMenuButton;
        public Button quitButton;
        
        [Header("Settings")]
        [Tooltip("Key to toggle pause menu")]
        public KeyCode pauseKey = KeyCode.Escape;
        
        private bool isPaused = false;
        
        private void Start()
        {
            // Set up button listeners
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
            
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);
            
            // Hide pause menu initially
            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(false);
            
            // Subscribe to GameManager events
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }
        }
        
        private void OnDestroy()
        {
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }
        
        private void Update()
        {
            // Handle pause input (backup in case GameManager doesn't handle it)
            if (Input.GetKeyDown(pauseKey))
            {
                TogglePause();
            }
        }
        
        public void TogglePause()
        {
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.TogglePause();
            }
            else
            {
                // Manual pause handling if no GameManager
                isPaused = !isPaused;
                
                if (isPaused)
                {
                    Time.timeScale = 0f;
                    ShowPauseMenu();
                }
                else
                {
                    Time.timeScale = 1f;
                    HidePauseMenu();
                }
            }
        }
        
        private void OnGameStateChanged(Dungeon.GameState newState)
        {
            switch (newState)
            {
                case Dungeon.GameState.Paused:
                    ShowPauseMenu();
                    break;
                case Dungeon.GameState.Playing:
                    HidePauseMenu();
                    break;
            }
        }
        
        private void ShowPauseMenu()
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }
            isPaused = true;
        }
        
        private void HidePauseMenu()
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
            isPaused = false;
        }
        
        #region Button Handlers
        
        public void OnResumeClicked()
        {
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.ResumeGame();
            }
            else
            {
                Time.timeScale = 1f;
                HidePauseMenu();
            }
        }
        
        public void OnRestartClicked()
        {
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.RestartDungeon();
            }
            else
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }
        }
        
        public void OnMainMenuClicked()
        {
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.ReturnToMainMenu();
            }
            else
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }
        
        public void OnQuitClicked()
        {
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.QuitGame();
            }
            else
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
            }
        }
        
        #endregion
    }
}

