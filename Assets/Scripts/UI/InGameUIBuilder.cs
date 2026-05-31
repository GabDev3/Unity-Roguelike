using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace UI
{
    /// <summary>
    /// Automatically builds the in-game UI at runtime including:
    /// - Pause Menu
    /// - Game Over Screen
    /// - Loading Screen
    /// - Player HUD placeholder
    /// Just add this component to an empty GameObject in your game scene.
    /// </summary>
    public class InGameUIBuilder : MonoBehaviour
    {
        [Header("Colors")]
        public Color overlayColor = new Color(0f, 0f, 0f, 0.8f);
        public Color panelColor = new Color(0.15f, 0.15f, 0.2f, 0.95f);
        public Color buttonNormalColor = new Color(0.2f, 0.5f, 0.8f, 1f);
        public Color buttonHighlightColor = new Color(0.3f, 0.6f, 0.9f, 1f);
        public Color buttonPressedColor = new Color(0.15f, 0.4f, 0.7f, 1f);
        public Color dangerButtonColor = new Color(0.8f, 0.2f, 0.2f, 1f);
        public Color textColor = Color.white;
        public Color titleColor = new Color(1f, 0.85f, 0.4f, 1f);
        public Color victoryColor = new Color(0.4f, 1f, 0.4f, 1f);
        public Color defeatColor = new Color(1f, 0.3f, 0.3f, 1f);

        [Header("Loading Screen")]
        public string[] loadingTips = new string[]
        {
            "Explore every room for hidden treasures...",
            "Some enemies are stronger than others. Choose your battles wisely.",
            "The dungeon layout changes with each seed.",
            "Check corners for secret items!",
            "Defeat all enemies to progress.",
            "Boss rooms contain powerful rewards."
        };

        [Header("Font Settings")]
        public TMP_FontAsset customFont;

        [Header("Scene Settings")]
        public string mainMenuSceneName = "MainMenu";

        // References that will be created
        private Canvas _canvas;
        private PauseMenuManager _pauseManager;
        private GameOverManager _gameOverManager;
        private LoadingScreenManager _loadingManager;

        private void Awake()
        {
            BuildUI();
        }

        public void BuildUI()
        {
            // Create EventSystem if it doesn't exist
            if (FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<EventSystem>();
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }

            // Create Canvas
            CreateCanvas();

            // Create all UI panels
            CreateLoadingScreen();
            CreatePauseMenu();
            CreateGameOverScreen();

            // Ensure gameplay systems exist even when SampleScene is played directly
            EnsureGameSystems();

            Debug.Log("In-Game UI built successfully!");
        }

        private void EnsureGameSystems()
        {
            // A GameManager is needed for win/lose state. When launched from the
            // main menu it persists via DontDestroyOnLoad; when SampleScene is
            // played directly there is none, so create one here.
            if (Dungeon.GameManager.Instance == null && FindAnyObjectByType<Dungeon.GameManager>() == null)
            {
                var gmObj = new GameObject("GameManager");
                gmObj.AddComponent<Dungeon.GameManager>();
            }

            // Combat indicators (hit-range sword icon + attack-cooldown fill circle)
            if (FindAnyObjectByType<CombatIndicatorUI>() == null)
            {
                var indObj = new GameObject("CombatIndicatorUI");
                indObj.AddComponent<CombatIndicatorUI>();
            }
        }

        private void CreateCanvas()
        {
            GameObject canvasObj = new GameObject("InGameUICanvas");
            _canvas = canvasObj.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();
        }

        #region Loading Screen

        private void CreateLoadingScreen()
        {
            // Create manager object
            GameObject managerObj = new GameObject("LoadingScreenManager");
            managerObj.transform.SetParent(_canvas.transform, false);
            _loadingManager = managerObj.AddComponent<LoadingScreenManager>();

            // Loading Panel (covers entire screen)
            GameObject loadingPanel = CreateUIElement("LoadingPanel", _canvas.transform);
            SetFullStretch(loadingPanel.GetComponent<RectTransform>());
            
            Image panelBg = loadingPanel.AddComponent<Image>();
            panelBg.color = new Color(0.05f, 0.05f, 0.1f, 1f);

            // Container
            GameObject container = CreateUIElement("Container", loadingPanel.transform);
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(600, 300);
            containerRect.anchoredPosition = Vector2.zero;

            VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 30;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            // Loading Text
            GameObject loadingTextObj = CreateTextElement("LoadingText", container.transform, "GENERATING DUNGEON...", 42, titleColor);
            LayoutElement loadingLayout = loadingTextObj.AddComponent<LayoutElement>();
            loadingLayout.preferredHeight = 60;
            _loadingManager.loadingText = loadingTextObj.GetComponent<TextMeshProUGUI>();

            // Spinner container
            GameObject spinnerContainer = CreateUIElement("SpinnerContainer", container.transform);
            LayoutElement spinnerLayout = spinnerContainer.AddComponent<LayoutElement>();
            spinnerLayout.preferredHeight = 80;
            spinnerLayout.preferredWidth = 80;

            // Spinner (rotating image)
            GameObject spinnerObj = CreateUIElement("Spinner", spinnerContainer.transform);
            RectTransform spinnerRect = spinnerObj.GetComponent<RectTransform>();
            spinnerRect.anchorMin = new Vector2(0.5f, 0.5f);
            spinnerRect.anchorMax = new Vector2(0.5f, 0.5f);
            spinnerRect.sizeDelta = new Vector2(60, 60);
            
            Image spinnerImage = spinnerObj.AddComponent<Image>();
            spinnerImage.color = buttonNormalColor;
            // Create a simple square that rotates
            _loadingManager.loadingSpinner = spinnerImage;

            // Add rotation script
            spinnerObj.AddComponent<UISpinner>();

            // Tip Text
            GameObject tipTextObj = CreateTextElement("TipText", container.transform, "Tip: " + GetRandomTip(), 22, textColor);
            LayoutElement tipLayout = tipTextObj.AddComponent<LayoutElement>();
            tipLayout.preferredHeight = 40;
            TextMeshProUGUI tipTmp = tipTextObj.GetComponent<TextMeshProUGUI>();
            tipTmp.fontStyle = FontStyles.Italic;
            _loadingManager.tipText = tipTmp;

            // Assign panel reference
            _loadingManager.loadingPanel = loadingPanel;
            _loadingManager.loadingTips = loadingTips;

            // Start hidden
            loadingPanel.SetActive(false);
        }

        private string GetRandomTip()
        {
            if (loadingTips == null || loadingTips.Length == 0)
                return "Loading...";
            return loadingTips[Random.Range(0, loadingTips.Length)];
        }

        #endregion

        #region Pause Menu

        private void CreatePauseMenu()
        {
            // Create manager object
            GameObject managerObj = new GameObject("PauseMenuManager");
            managerObj.transform.SetParent(_canvas.transform, false);
            _pauseManager = managerObj.AddComponent<PauseMenuManager>();

            // Pause Panel (overlay)
            GameObject pausePanel = CreateUIElement("PauseMenuPanel", _canvas.transform);
            SetFullStretch(pausePanel.GetComponent<RectTransform>());
            
            Image overlayBg = pausePanel.AddComponent<Image>();
            overlayBg.color = overlayColor;

            // Center container with background
            GameObject container = CreateUIElement("Container", pausePanel.transform);
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(400, 450);
            containerRect.anchoredPosition = Vector2.zero;

            Image panelBg = container.AddComponent<Image>();
            panelBg.color = panelColor;

            VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.spacing = 20;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(40, 40, 30, 30);

            // Title
            GameObject titleObj = CreateTextElement("PauseTitle", container.transform, "PAUSED", 48, titleColor);
            LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 70;

            // Spacer
            CreateSpacer(container.transform, 20);

            // Buttons
            _pauseManager.resumeButton = CreateButton("ResumeButton", container.transform, "RESUME", 50);
            _pauseManager.restartButton = CreateButton("RestartButton", container.transform, "RESTART", 50);
            _pauseManager.mainMenuButton = CreateButton("MainMenuButton", container.transform, "MAIN MENU", 50);
            _pauseManager.quitButton = CreateButton("QuitButton", container.transform, "QUIT GAME", 50, dangerButtonColor);

            // Assign panel reference
            _pauseManager.pauseMenuPanel = pausePanel;

            // Start hidden
            pausePanel.SetActive(false);
        }

        #endregion

        #region Game Over Screen

        private void CreateGameOverScreen()
        {
            // Create manager object
            GameObject managerObj = new GameObject("GameOverManager");
            managerObj.transform.SetParent(_canvas.transform, false);
            _gameOverManager = managerObj.AddComponent<GameOverManager>();

            // Game Over Panel (overlay)
            GameObject gameOverPanel = CreateUIElement("GameOverPanel", _canvas.transform);
            SetFullStretch(gameOverPanel.GetComponent<RectTransform>());
            
            Image overlayBg = gameOverPanel.AddComponent<Image>();
            overlayBg.color = overlayColor;

            // Center container with background
            GameObject container = CreateUIElement("Container", gameOverPanel.transform);
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(500, 500);
            containerRect.anchoredPosition = Vector2.zero;

            Image panelBg = container.AddComponent<Image>();
            panelBg.color = panelColor;

            VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.spacing = 20;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(40, 40, 40, 40);

            // Title (will change based on victory/defeat)
            GameObject titleObj = CreateTextElement("TitleText", container.transform, "GAME OVER", 56, defeatColor);
            LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 80;
            _gameOverManager.titleText = titleObj.GetComponent<TextMeshProUGUI>();

            // Subtitle
            GameObject subtitleObj = CreateTextElement("SubtitleText", container.transform, "You have fallen in the dungeon", 24, textColor);
            LayoutElement subtitleLayout = subtitleObj.AddComponent<LayoutElement>();
            subtitleLayout.preferredHeight = 40;
            // Note: subtitleText not stored as GameOverManager doesn't have this property

            // Score text
            GameObject scoreObj = CreateTextElement("ScoreText", container.transform, "Rooms Explored: 0", 28, titleColor);
            LayoutElement scoreLayout = scoreObj.AddComponent<LayoutElement>();
            scoreLayout.preferredHeight = 50;
            _gameOverManager.scoreText = scoreObj.GetComponent<TextMeshProUGUI>();

            // Spacer
            CreateSpacer(container.transform, 30);

            // Buttons
            _gameOverManager.restartButton = CreateButton("RestartButton", container.transform, "TRY AGAIN", 55);
            _gameOverManager.mainMenuButton = CreateButton("MainMenuButton", container.transform, "MAIN MENU", 55);
            _gameOverManager.quitButton = CreateButton("QuitButton", container.transform, "QUIT", 55, dangerButtonColor);

            // Assign panel reference
            _gameOverManager.gameOverPanel = gameOverPanel;

            // Start hidden
            gameOverPanel.SetActive(false);
        }

        #endregion

        #region UI Creation Helpers

        private GameObject CreateUIElement(string elementName, Transform parent)
        {
            GameObject obj = new GameObject(elementName);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();
            return obj;
        }

        private GameObject CreateTextElement(string elementName, Transform parent, string text, int fontSize, Color color)
        {
            GameObject textObj = CreateUIElement(elementName, parent);
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            if (customFont != null)
                tmp.font = customFont;

            return textObj;
        }

        private Button CreateButton(string elementName, Transform parent, string text, float height, Color? customColor = null)
        {
            GameObject buttonObj = CreateUIElement(elementName, parent);
            
            LayoutElement layoutElement = buttonObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = height;
            layoutElement.minHeight = height;

            Image buttonImage = buttonObj.AddComponent<Image>();
            Color baseColor = customColor ?? buttonNormalColor;
            buttonImage.color = baseColor;

            Button button = buttonObj.AddComponent<Button>();
            
            ColorBlock colors = button.colors;
            colors.normalColor = baseColor;
            colors.highlightedColor = customColor.HasValue ? 
                new Color(baseColor.r + 0.1f, baseColor.g + 0.1f, baseColor.b + 0.1f, 1f) : 
                buttonHighlightColor;
            colors.pressedColor = customColor.HasValue ? 
                new Color(baseColor.r - 0.1f, baseColor.g - 0.1f, baseColor.b - 0.1f, 1f) : 
                buttonPressedColor;
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;

            GameObject textObj = CreateUIElement("Text", buttonObj.transform);
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 26;
            tmp.color = textColor;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            if (customFont != null)
                tmp.font = customFont;

            SetFullStretch(textObj.GetComponent<RectTransform>());

            return button;
        }

        private void CreateSpacer(Transform parent, float height)
        {
            GameObject spacer = CreateUIElement("Spacer", parent);
            LayoutElement layout = spacer.AddComponent<LayoutElement>();
            layout.preferredHeight = height;
            layout.flexibleWidth = 1;
        }

        private void SetFullStretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        #endregion
    }

    /// <summary>
    /// Simple spinner animation for loading screen
    /// </summary>
    public class UISpinner : MonoBehaviour
    {
        public float rotationSpeed = 200f;

        private void Update()
        {
            transform.Rotate(0, 0, -rotationSpeed * Time.unscaledDeltaTime);
        }
    }
}

