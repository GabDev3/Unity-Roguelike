using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace UI
{
    /// <summary>
    /// Automatically builds the entire Main Menu UI at runtime.
    /// Just add this component to an empty GameObject in your MainMenu scene
    /// and it will create Canvas, EventSystem, and all UI elements.
    /// </summary>
    public class MainMenuBuilder : MonoBehaviour
    {
        [Header("Colors")]
        public Color backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
        public Color panelColor = new Color(0.15f, 0.15f, 0.2f, 0.95f);
        public Color buttonNormalColor = new Color(0.2f, 0.5f, 0.8f, 1f);
        public Color buttonHighlightColor = new Color(0.3f, 0.6f, 0.9f, 1f);
        public Color buttonPressedColor = new Color(0.15f, 0.4f, 0.7f, 1f);
        public Color textColor = Color.white;
        public Color titleColor = new Color(1f, 0.85f, 0.4f, 1f);

        [Header("Settings")]
        public string gameTitle = "DUNGEON EXPLORER";
        public string creditsText = "Created with Unity\n\nUsing Edgar for Procedural Generation\n\n© 2026";
        public string gameSceneName = "SampleScene";

        [Header("Font Settings")]
        public TMP_FontAsset customFont;

        // References that will be created
        private Canvas _canvas;
        private MainMenuManager _menuManager;
        private GameObject _mainMenuPanel;
        private GameObject _settingsPanel;
        private GameObject _creditsPanel;

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

            // Create background
            CreateBackground();

            // Create all panels
            CreateMainMenuPanel();
            CreateSettingsPanel();
            CreateCreditsPanel();

            // Setup MainMenuManager
            SetupMainMenuManager();

            Debug.Log("Main Menu UI built successfully!");
        }

        private void CreateCanvas()
        {
            GameObject canvasObj = new GameObject("MainMenuCanvas");
            _canvas = canvasObj.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // Add MainMenuManager to canvas
            _menuManager = canvasObj.AddComponent<MainMenuManager>();
            _menuManager.gameSceneName = gameSceneName;
        }

        private void CreateBackground()
        {
            GameObject bgObj = CreateUIElement("Background", _canvas.transform);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = backgroundColor;
            SetFullStretch(bgObj.GetComponent<RectTransform>());
        }

        private void CreateMainMenuPanel()
        {
            // Main panel
            _mainMenuPanel = CreatePanel("MainMenuPanel", _canvas.transform);
            
            // Container for centering content
            GameObject container = CreateUIElement("Container", _mainMenuPanel.transform);
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(400, 500);
            containerRect.anchoredPosition = Vector2.zero;

            VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 20;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(20, 20, 20, 20);

            // Title
            GameObject titleObj = CreateTextElement("TitleText", container.transform, gameTitle, 64, titleColor);
            LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
            titleLayout.preferredHeight = 100;
            titleLayout.minHeight = 80;

            // Spacer
            CreateSpacer(container.transform, 40);

            // Buttons
            _menuManager.playButton = CreateButton("PlayButton", container.transform, "PLAY", 50);
            _menuManager.settingsButton = CreateButton("SettingsButton", container.transform, "SETTINGS", 50);
            _menuManager.creditsButton = CreateButton("CreditsButton", container.transform, "CREDITS", 50);
            _menuManager.quitButton = CreateButton("QuitButton", container.transform, "QUIT", 50);

            _menuManager.mainMenuPanel = _mainMenuPanel;
        }

        private void CreateSettingsPanel()
        {
            _settingsPanel = CreatePanel("SettingsPanel", _canvas.transform);
            _settingsPanel.SetActive(false);

            // Container
            GameObject container = CreateUIElement("Container", _settingsPanel.transform);
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(500, 600);
            containerRect.anchoredPosition = Vector2.zero;

            // Add background panel
            Image panelBg = container.AddComponent<Image>();
            panelBg.color = panelColor;

            VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.spacing = 25;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(40, 40, 30, 30);

            // Title
            CreateTextElement("SettingsTitle", container.transform, "SETTINGS", 48, titleColor);

            // Spacer
            CreateSpacer(container.transform, 20);

            // Master Volume
            CreateSliderWithLabel("MasterVolume", container.transform, "Master Volume", out _menuManager.masterVolumeSlider);

            // Music Volume
            CreateSliderWithLabel("MusicVolume", container.transform, "Music Volume", out _menuManager.musicVolumeSlider);

            // SFX Volume
            CreateSliderWithLabel("SFXVolume", container.transform, "SFX Volume", out _menuManager.sfxVolumeSlider);

            // Spacer
            CreateSpacer(container.transform, 10);

            // Random Seed Toggle
            CreateToggleWithLabel("RandomSeedToggle", container.transform, "Use Random Seed", out _menuManager.useRandomSeedToggle);

            // Seed Input Field
            CreateInputFieldWithLabel("SeedInputField", container.transform, "Custom Seed", "Enter seed...", out _menuManager.seedInputField);

            // Spacer
            CreateSpacer(container.transform, 20);

            // Back Button
            _menuManager.backFromSettingsButton = CreateButton("BackButton", container.transform, "BACK", 50);

            _menuManager.settingsPanel = _settingsPanel;
        }

        private void CreateCreditsPanel()
        {
            _creditsPanel = CreatePanel("CreditsPanel", _canvas.transform);
            _creditsPanel.SetActive(false);

            // Container
            GameObject container = CreateUIElement("Container", _creditsPanel.transform);
            RectTransform containerRect = container.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(500, 500);
            containerRect.anchoredPosition = Vector2.zero;

            // Add background panel
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
            CreateTextElement("CreditsTitle", container.transform, "CREDITS", 48, titleColor);

            // Spacer
            CreateSpacer(container.transform, 20);

            // Credits Text
            GameObject creditsTextObj = CreateTextElement("CreditsText", container.transform, creditsText, 24, textColor);
            LayoutElement creditsLayout = creditsTextObj.AddComponent<LayoutElement>();
            creditsLayout.preferredHeight = 250;

            // Spacer
            CreateSpacer(container.transform, 20);

            // Back Button
            _menuManager.backFromCreditsButton = CreateButton("BackButton", container.transform, "BACK", 50);

            _menuManager.creditsPanel = _creditsPanel;
        }

        private void SetupMainMenuManager()
        {
            // MainMenuManager is already attached and configured
            // It will set up listeners in its Start() method
        }

        #region UI Creation Helpers

        private GameObject CreateUIElement(string elementName, Transform parent)
        {
            GameObject obj = new GameObject(elementName);
            obj.transform.SetParent(parent, false);
            obj.AddComponent<RectTransform>();
            return obj;
        }

        private GameObject CreatePanel(string elementName, Transform parent)
        {
            GameObject panel = CreateUIElement(elementName, parent);
            SetFullStretch(panel.GetComponent<RectTransform>());
            return panel;
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

        private Button CreateButton(string elementName, Transform parent, string text, float height)
        {
            GameObject buttonObj = CreateUIElement(elementName, parent);
            
            // Add layout element
            LayoutElement layoutElement = buttonObj.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = height;
            layoutElement.minHeight = height;

            // Add Image for button background
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = buttonNormalColor;

            // Add Button component
            Button button = buttonObj.AddComponent<Button>();
            
            // Set up button colors
            ColorBlock colors = button.colors;
            colors.normalColor = buttonNormalColor;
            colors.highlightedColor = buttonHighlightColor;
            colors.pressedColor = buttonPressedColor;
            colors.selectedColor = buttonHighlightColor;
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.1f;
            button.colors = colors;

            // Add text
            GameObject textObj = CreateUIElement("Text", buttonObj.transform);
            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 28;
            tmp.color = textColor;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            if (customFont != null)
                tmp.font = customFont;

            SetFullStretch(textObj.GetComponent<RectTransform>());

            return button;
        }

        private void CreateSliderWithLabel(string elementName, Transform parent, string labelText, out Slider slider)
        {
            GameObject container = CreateUIElement(elementName + "Container", parent);
            LayoutElement containerLayout = container.AddComponent<LayoutElement>();
            containerLayout.preferredHeight = 60;

            // Label
            GameObject labelObj = CreateUIElement("Label", container.transform);
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.5f);
            labelRect.anchorMax = new Vector2(0.4f, 1f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.text = labelText;
            label.fontSize = 20;
            label.color = textColor;
            label.alignment = TextAlignmentOptions.Left;

            if (customFont != null)
                label.font = customFont;

            // Slider
            GameObject sliderObj = CreateUIElement("Slider", container.transform);
            RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.45f, 0.3f);
            sliderRect.anchorMax = new Vector2(1f, 0.7f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;

            // Background
            GameObject bgObj = CreateUIElement("Background", sliderObj.transform);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.25f, 1f);
            SetFullStretch(bgObj.GetComponent<RectTransform>());

            // Fill Area
            GameObject fillArea = CreateUIElement("Fill Area", sliderObj.transform);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(5, 0);
            fillAreaRect.offsetMax = new Vector2(-5, 0);

            GameObject fill = CreateUIElement("Fill", fillArea.transform);
            Image fillImage = fill.AddComponent<Image>();
            fillImage.color = buttonNormalColor;
            SetFullStretch(fill.GetComponent<RectTransform>());

            // Handle Slide Area
            GameObject handleArea = CreateUIElement("Handle Slide Area", sliderObj.transform);
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(10, 0);
            handleAreaRect.offsetMax = new Vector2(-10, 0);

            GameObject handle = CreateUIElement("Handle", handleArea.transform);
            Image handleImage = handle.AddComponent<Image>();
            handleImage.color = Color.white;
            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);

            // Setup Slider component
            slider = sliderObj.AddComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 1;
        }

        private void CreateToggleWithLabel(string elementName, Transform parent, string labelText, out Toggle toggle)
        {
            GameObject container = CreateUIElement(elementName + "Container", parent);
            LayoutElement containerLayout = container.AddComponent<LayoutElement>();
            containerLayout.preferredHeight = 40;

            // Label
            GameObject labelObj = CreateUIElement("Label", container.transform);
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(0.7f, 1f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.text = labelText;
            label.fontSize = 20;
            label.color = textColor;
            label.alignment = TextAlignmentOptions.Left;

            if (customFont != null)
                label.font = customFont;

            // Toggle
            GameObject toggleObj = CreateUIElement("Toggle", container.transform);
            RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0.8f, 0.1f);
            toggleRect.anchorMax = new Vector2(0.95f, 0.9f);
            toggleRect.offsetMin = Vector2.zero;
            toggleRect.offsetMax = Vector2.zero;

            // Background
            GameObject bgObj = CreateUIElement("Background", toggleObj.transform);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.25f, 1f);
            SetFullStretch(bgObj.GetComponent<RectTransform>());

            // Checkmark
            GameObject checkmarkObj = CreateUIElement("Checkmark", bgObj.transform);
            Image checkmark = checkmarkObj.AddComponent<Image>();
            checkmark.color = buttonNormalColor;
            RectTransform checkmarkRect = checkmarkObj.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.1f, 0.1f);
            checkmarkRect.anchorMax = new Vector2(0.9f, 0.9f);
            checkmarkRect.offsetMin = Vector2.zero;
            checkmarkRect.offsetMax = Vector2.zero;

            // Setup Toggle
            toggle = toggleObj.AddComponent<Toggle>();
            toggle.targetGraphic = bgImage;
            toggle.graphic = checkmark;
            toggle.isOn = true;
        }

        private void CreateInputFieldWithLabel(string elementName, Transform parent, string labelText, string placeholder, out TMP_InputField inputField)
        {
            GameObject container = CreateUIElement(elementName + "Container", parent);
            LayoutElement containerLayout = container.AddComponent<LayoutElement>();
            containerLayout.preferredHeight = 50;

            // Label
            GameObject labelObj = CreateUIElement("Label", container.transform);
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(0.4f, 1f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
            label.text = labelText;
            label.fontSize = 20;
            label.color = textColor;
            label.alignment = TextAlignmentOptions.Left;

            if (customFont != null)
                label.font = customFont;

            // Input Field
            GameObject inputObj = CreateUIElement("InputField", container.transform);
            RectTransform inputRect = inputObj.GetComponent<RectTransform>();
            inputRect.anchorMin = new Vector2(0.45f, 0.1f);
            inputRect.anchorMax = new Vector2(1f, 0.9f);
            inputRect.offsetMin = Vector2.zero;
            inputRect.offsetMax = Vector2.zero;

            // Background
            Image bgImage = inputObj.AddComponent<Image>();
            bgImage.color = new Color(0.15f, 0.15f, 0.2f, 1f);

            // Text Area
            GameObject textArea = CreateUIElement("Text Area", inputObj.transform);
            RectTransform textAreaRect = textArea.GetComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(10, 5);
            textAreaRect.offsetMax = new Vector2(-10, -5);
            textArea.AddComponent<RectMask2D>();

            // Placeholder
            GameObject placeholderObj = CreateUIElement("Placeholder", textArea.transform);
            TextMeshProUGUI placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
            placeholderText.text = placeholder;
            placeholderText.fontSize = 18;
            placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            placeholderText.fontStyle = FontStyles.Italic;
            SetFullStretch(placeholderObj.GetComponent<RectTransform>());

            if (customFont != null)
                placeholderText.font = customFont;

            // Text
            GameObject textObj = CreateUIElement("Text", textArea.transform);
            TextMeshProUGUI inputText = textObj.AddComponent<TextMeshProUGUI>();
            inputText.fontSize = 18;
            inputText.color = textColor;
            SetFullStretch(textObj.GetComponent<RectTransform>());

            if (customFont != null)
                inputText.font = customFont;

            // Setup Input Field
            inputField = inputObj.AddComponent<TMP_InputField>();
            inputField.textViewport = textAreaRect;
            inputField.textComponent = inputText;
            inputField.placeholder = placeholderText;
            inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            inputField.characterLimit = 10;
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
}

