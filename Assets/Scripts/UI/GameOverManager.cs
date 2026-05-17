using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    /// <summary>
    /// Game Over screen controller.
    /// </summary>
    public class GameOverManager : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject gameOverPanel;
        
        [Header("UI Elements")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI scoreText;
        
        [Header("Buttons")]
        public Button restartButton;
        public Button mainMenuButton;
        public Button quitButton;
        
        [Header("Settings")]
        public string deathTitle = "YOU DIED";
        public string victoryTitle = "VICTORY!";
        public Color deathTitleColor = Color.red;
        public Color victoryTitleColor = Color.yellow;
        
        // Cached player stats (in case player is destroyed)
        private int _cachedPlayerMoney = 0;
        private bool _hasPlayerData = false;
        
        private void Start()
        {
            // Set up button listeners
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);
            
            // Hide panel initially
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
            
            // Subscribe to GameManager events
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }
            
            // Try to cache player data early
            CachePlayerData();
        }
        
        private void Update()
        {
            // Continuously try to cache player data while playing
            if (!_hasPlayerData)
            {
                CachePlayerData();
            }
        }
        
        private void CachePlayerData()
        {
            var player = FindFirstObjectByType<Controllers.PlayerController>();
            if (player != null)
            {
                _cachedPlayerMoney = player.CurrentMoney;
                _hasPlayerData = true;
            }
        }
        
        private void OnDestroy()
        {
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }
        
        private void OnGameStateChanged(Dungeon.GameState newState)
        {
            switch (newState)
            {
                case Dungeon.GameState.GameOver:
                    ShowGameOver(false);
                    break;
                case Dungeon.GameState.Victory:
                    ShowGameOver(true);
                    break;
                case Dungeon.GameState.Playing:
                    HideGameOver();
                    break;
            }
        }
        
        public void ShowGameOver(bool isVictory)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            
            // Update title
            if (titleText != null)
            {
                titleText.text = isVictory ? victoryTitle : deathTitle;
                titleText.color = isVictory ? victoryTitleColor : deathTitleColor;
            }
            
            // Update score - try to get current player data first, fall back to cached
            if (scoreText != null)
            {
                // Try to get fresh data
                var player = FindFirstObjectByType<Controllers.PlayerController>();
                if (player != null)
                {
                    _cachedPlayerMoney = player.CurrentMoney;
                    _hasPlayerData = true;
                }
                
                // Use cached data
                if (_hasPlayerData)
                {
                    scoreText.text = $"Gold Collected: {_cachedPlayerMoney}";
                }
                else
                {
                    scoreText.text = "Gold Collected: 0";
                }
            }
            
            // Pause the game
            Time.timeScale = 0f;
            
            Debug.Log($"Game Over screen shown. Victory: {isVictory}");
        }
        
        public void HideGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }
        
        #region Button Handlers
        
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

