using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Dungeon
{
    /// <summary>
    /// Singleton GameManager that persists across scenes.
    /// Handles scene transitions, game state, and global settings.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        public static GameManager Instance { get; private set; }
        
        [Header("Scene Names")]
        [Tooltip("Name of the main menu scene")]
        public string mainMenuSceneName = "MainMenu";
        
        [Tooltip("Name of the game/dungeon scene")]
        public string gameSceneName = "SampleScene";
        
        [Header("Player Class")]
        [Tooltip("Class chosen on the main menu; read by the content spawner to pick the player prefab")]
        public PlayerClass SelectedClass = PlayerClass.Melee;

        [Header("Game Settings")]
        [Tooltip("If true, uses random seed. If false, uses fixedSeed")]
        public bool useRandomSeed = true;
        
        [Tooltip("Fixed seed for reproducible dungeon generation")]
        public int fixedSeed = 12345;
        
        [Header("Audio Settings")]
        [Range(0f, 1f)]
        public float masterVolume = 1f;
        
        [Range(0f, 1f)]
        public float musicVolume = 0.7f;
        
        [Range(0f, 1f)]
        public float sfxVolume = 1f;
        
        // Current game state
        public GameState CurrentState { get; private set; } = GameState.MainMenu;
        
        // Current dungeon seed (used for this play session)
        public int CurrentSeed { get; private set; }
        
        // Events
        public event System.Action<GameState> OnGameStateChanged;
        public event System.Action OnDungeonGenerationStarted;
        public event System.Action OnDungeonGenerationCompleted;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Generate initial seed if using random
            if (useRandomSeed)
            {
                CurrentSeed = System.Environment.TickCount;
            }
            else
            {
                CurrentSeed = fixedSeed;
            }
        }
        
        /// <summary>
        /// Start a new game with a new random seed
        /// </summary>
        public void StartNewGame()
        {
            // Generate new seed
            if (useRandomSeed)
            {
                CurrentSeed = System.Environment.TickCount;
            }
            else
            {
                CurrentSeed = fixedSeed;
            }
            
            Debug.Log($"Starting new game with seed: {CurrentSeed}");
            
            ChangeState(GameState.Loading);
            StartCoroutine(LoadGameSceneAsync());
        }
        
        /// <summary>
        /// Start a new game with a specific seed
        /// </summary>
        public void StartNewGameWithSeed(int seed)
        {
            CurrentSeed = seed;
            Debug.Log($"Starting new game with custom seed: {CurrentSeed}");
            
            ChangeState(GameState.Loading);
            StartCoroutine(LoadGameSceneAsync());
        }
        
        /// <summary>
        /// Return to main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            ChangeState(GameState.Loading);
            StartCoroutine(LoadMainMenuAsync());
        }
        
        /// <summary>
        /// Quit the game
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quitting game...");
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        /// <summary>
        /// Pause the game
        /// </summary>
        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
            {
                Time.timeScale = 0f;
                ChangeState(GameState.Paused);
            }
        }
        
        /// <summary>
        /// Resume the game
        /// </summary>
        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                Time.timeScale = 1f;
                ChangeState(GameState.Playing);
            }
        }
        
        /// <summary>
        /// Toggle pause state
        /// </summary>
        public void TogglePause()
        {
            if (CurrentState == GameState.Playing)
                PauseGame();
            else if (CurrentState == GameState.Paused)
                ResumeGame();
        }
        
        /// <summary>
        /// Notify that dungeon generation has started
        /// </summary>
        public void NotifyDungeonGenerationStarted()
        {
            OnDungeonGenerationStarted?.Invoke();
        }
        
        /// <summary>
        /// Notify that dungeon generation has completed
        /// </summary>
        public void NotifyDungeonGenerationCompleted()
        {
            ChangeState(GameState.Playing);
            OnDungeonGenerationCompleted?.Invoke();
        }
        
        /// <summary>
        /// Handle player death
        /// </summary>
        public void OnPlayerDeath()
        {
            ChangeState(GameState.GameOver);
        }

        /// <summary>
        /// Handle the boss being defeated -> player wins.
        /// </summary>
        public void OnBossDefeated()
        {
            ChangeState(GameState.Victory);
        }
        
        /// <summary>
        /// Restart the current dungeon with the same seed
        /// </summary>
        public void RestartDungeon()
        {
            ChangeState(GameState.Loading);
            StartCoroutine(LoadGameSceneAsync());
        }
        
        private void ChangeState(GameState newState)
        {
            GameState previousState = CurrentState;
            CurrentState = newState;
            
            Debug.Log($"Game state changed: {previousState} -> {newState}");
            OnGameStateChanged?.Invoke(newState);
        }
        
        private IEnumerator LoadGameSceneAsync()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            // The dungeon generator will notify when generation is complete
            // For now, we stay in Loading state
        }
        
        private IEnumerator LoadMainMenuAsync()
        {
            // Ensure time scale is normal
            Time.timeScale = 1f;
            
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuSceneName);
            
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            ChangeState(GameState.MainMenu);
        }
        
        private void Update()
        {
            // Handle pause input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CurrentState == GameState.Playing || CurrentState == GameState.Paused)
                {
                    TogglePause();
                }
            }
        }
    }
    
    /// <summary>
    /// Player class chosen before starting the game.
    /// </summary>
    public enum PlayerClass
    {
        Melee,
        Ranged
    }

    /// <summary>
    /// Possible game states
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Loading,
        Playing,
        Paused,
        GameOver,
        Victory
    }
}

