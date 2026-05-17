using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace UI
{
    /// <summary>
    /// Main Menu controller that handles menu navigation and game start.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Panels")]
        [Tooltip("The main menu panel")]
        public GameObject mainMenuPanel;
        
        [Tooltip("The settings panel")]
        public GameObject settingsPanel;
        
        [Tooltip("The credits panel")]
        public GameObject creditsPanel;
        
        [Header("Buttons")]
        public Button playButton;
        public Button settingsButton;
        public Button creditsButton;
        public Button quitButton;
        public Button backFromSettingsButton;
        public Button backFromCreditsButton;
        
        [Header("Settings UI")]
        public Slider masterVolumeSlider;
        public Slider musicVolumeSlider;
        public Slider sfxVolumeSlider;
        public TMP_InputField seedInputField;
        public Toggle useRandomSeedToggle;
        
        [Header("Scene Settings")]
        [Tooltip("Name of the game scene to load")]
        public string gameSceneName = "SampleScene";
        
        [Header("Animation")]
        [Tooltip("Fade duration for panel transitions")]
        public float fadeDuration = 0.3f;
        
        private CanvasGroup mainMenuCanvasGroup;
        private CanvasGroup settingsCanvasGroup;
        private CanvasGroup creditsCanvasGroup;
        
        private void Awake()
        {
            // Get or add CanvasGroups for smooth transitions
            if (mainMenuPanel != null)
                mainMenuCanvasGroup = mainMenuPanel.GetComponent<CanvasGroup>() ?? mainMenuPanel.AddComponent<CanvasGroup>();
            if (settingsPanel != null)
                settingsCanvasGroup = settingsPanel.GetComponent<CanvasGroup>() ?? settingsPanel.AddComponent<CanvasGroup>();
            if (creditsPanel != null)
                creditsCanvasGroup = creditsPanel.GetComponent<CanvasGroup>() ?? creditsPanel.AddComponent<CanvasGroup>();
        }
        
        private void Start()
        {
            // Ensure time scale is normal
            Time.timeScale = 1f;
            
            // Set up button listeners
            SetupButtonListeners();
            
            // Initialize UI state
            ShowMainMenu();
            
            // Load saved settings
            LoadSettings();
            
            // Set up settings listeners
            SetupSettingsListeners();
        }
        
        private void SetupButtonListeners()
        {
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);
            
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);
            
            if (creditsButton != null)
                creditsButton.onClick.AddListener(OnCreditsClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);
            
            if (backFromSettingsButton != null)
                backFromSettingsButton.onClick.AddListener(OnBackFromSettingsClicked);
            
            if (backFromCreditsButton != null)
                backFromCreditsButton.onClick.AddListener(OnBackFromCreditsClicked);
        }
        
        private void SetupSettingsListeners()
        {
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            
            if (useRandomSeedToggle != null)
                useRandomSeedToggle.onValueChanged.AddListener(OnRandomSeedToggleChanged);
        }
        
        #region Button Handlers
        
        public void OnPlayClicked()
        {
            Debug.Log("Starting game...");
            
            // Handle seed settings
            if (Dungeon.GameManager.Instance != null)
            {
                if (useRandomSeedToggle != null && !useRandomSeedToggle.isOn)
                {
                    // Use custom seed
                    if (seedInputField != null && int.TryParse(seedInputField.text, out int customSeed))
                    {
                        Dungeon.GameManager.Instance.StartNewGameWithSeed(customSeed);
                        return;
                    }
                }
                
                Dungeon.GameManager.Instance.StartNewGame();
            }
            else
            {
                // No GameManager, load scene directly
                SceneManager.LoadScene(gameSceneName);
            }
        }
        
        public void OnSettingsClicked()
        {
            ShowSettings();
        }
        
        public void OnCreditsClicked()
        {
            ShowCredits();
        }
        
        public void OnQuitClicked()
        {
            Debug.Log("Quitting game...");
            
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
        
        public void OnBackFromSettingsClicked()
        {
            SaveSettings();
            ShowMainMenu();
        }
        
        public void OnBackFromCreditsClicked()
        {
            ShowMainMenu();
        }
        
        #endregion
        
        #region Settings Handlers
        
        private void OnMasterVolumeChanged(float value)
        {
            AudioListener.volume = value;
            
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.masterVolume = value;
            }
        }
        
        private void OnMusicVolumeChanged(float value)
        {
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.musicVolume = value;
            }
            // You can add actual music volume control here
        }
        
        private void OnSFXVolumeChanged(float value)
        {
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.sfxVolume = value;
            }
            // You can add actual SFX volume control here
        }
        
        private void OnRandomSeedToggleChanged(bool useRandom)
        {
            if (seedInputField != null)
            {
                seedInputField.interactable = !useRandom;
            }
            
            if (Dungeon.GameManager.Instance != null)
            {
                Dungeon.GameManager.Instance.useRandomSeed = useRandom;
            }
        }
        
        #endregion
        
        #region Panel Management
        
        private void ShowMainMenu()
        {
            SetPanelActive(mainMenuPanel, true);
            SetPanelActive(settingsPanel, false);
            SetPanelActive(creditsPanel, false);
        }
        
        private void ShowSettings()
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(settingsPanel, true);
            SetPanelActive(creditsPanel, false);
        }
        
        private void ShowCredits()
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(settingsPanel, false);
            SetPanelActive(creditsPanel, true);
        }
        
        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
            {
                panel.SetActive(active);
            }
        }
        
        #endregion
        
        #region Settings Persistence
        
        private void LoadSettings()
        {
            // Load volume settings
            float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
            bool useRandomSeed = PlayerPrefs.GetInt("UseRandomSeed", 1) == 1;
            
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = masterVol;
            
            if (musicVolumeSlider != null)
                musicVolumeSlider.value = musicVol;
            
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = sfxVol;
            
            if (useRandomSeedToggle != null)
                useRandomSeedToggle.isOn = useRandomSeed;
            
            if (seedInputField != null)
                seedInputField.interactable = !useRandomSeed;
            
            // Apply volume
            AudioListener.volume = masterVol;
        }
        
        private void SaveSettings()
        {
            if (masterVolumeSlider != null)
                PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
            
            if (musicVolumeSlider != null)
                PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
            
            if (sfxVolumeSlider != null)
                PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
            
            if (useRandomSeedToggle != null)
                PlayerPrefs.SetInt("UseRandomSeed", useRandomSeedToggle.isOn ? 1 : 0);
            
            PlayerPrefs.Save();
        }
        
        #endregion
    }
}

