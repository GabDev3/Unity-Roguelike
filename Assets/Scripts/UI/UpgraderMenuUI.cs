using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Interaction;
using Controllers;

namespace UI
{
    public class UpgraderMenuUI : MonoBehaviour
    {
        public static UpgraderMenuUI Instance;

        public GameObject menuPanel;

        [Header("Armor UI")]
        public TextMeshProUGUI armorText;
        public Button upgradeArmorBtn;

        [Header("Damage UI")]
        public TextMeshProUGUI damageText;
        public Button upgradeDamageBtn;

        [Header("Health UI")]
        public TextMeshProUGUI healthText;
        public Button upgradeHealthBtn;

        public Button closeBtn;

        private UpgraderNPC currentNPC;
        private PlayerController currentPlayer;

        [Header("Auto-Build UI Colors")]
        public Color overlayColor = new Color(0f, 0f, 0f, 0.8f);
        public Color panelColor = new Color(0.15f, 0.15f, 0.2f, 0.95f);
        public Color buttonNormalColor = new Color(0.2f, 0.5f, 0.8f, 1f);
        public Color titleColor = new Color(1f, 0.85f, 0.4f, 1f);

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (menuPanel == null)
            {
                BuildUI();
            }
            else
            {
                if (menuPanel != null) menuPanel.SetActive(false);

                upgradeArmorBtn.onClick.AddListener(UpgradeArmor);
                upgradeDamageBtn.onClick.AddListener(UpgradeDamage);
                upgradeHealthBtn.onClick.AddListener(UpgradeHealth);
                closeBtn.onClick.AddListener(CloseMenu);
            }
        }

        private void BuildUI()
        {
            // Build Canvas if missing
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("UpgraderCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 90;

                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;

                canvasObj.AddComponent<GraphicRaycaster>();
                transform.SetParent(canvasObj.transform, false);
            }

            // Create Menu Panel (fullscreen overlay)
            menuPanel = new GameObject("UpgraderMenuPanel");
            menuPanel.transform.SetParent(canvas.transform, false);
            SetFullStretch(menuPanel.AddComponent<RectTransform>());
            
            Image bg = menuPanel.AddComponent<Image>();
            bg.color = overlayColor;
            
            // Create Center Box
            GameObject centerBox = new GameObject("CenterBox");
            centerBox.transform.SetParent(menuPanel.transform, false);
            RectTransform boxRect = centerBox.AddComponent<RectTransform>();
            boxRect.anchorMin = new Vector2(0.5f, 0.5f);
            boxRect.anchorMax = new Vector2(0.5f, 0.5f);
            boxRect.sizeDelta = new Vector2(800, 600);
            boxRect.anchoredPosition = Vector2.zero;

            Image boxBg = centerBox.AddComponent<Image>();
            boxBg.color = panelColor;

            // Layout for center box
            var layout = centerBox.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(40, 40, 40, 40);
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = true;

            // Title
            CreateText("Title", centerBox.transform, "UPGRADES", 48, titleColor);

            // Container for stats
            GameObject statsContainer = new GameObject("StatsContainer");
            statsContainer.transform.SetParent(centerBox.transform, false);
            var statsRect = statsContainer.AddComponent<RectTransform>();
            var statsLayout = statsContainer.AddComponent<HorizontalLayoutGroup>();
            statsLayout.spacing = 20;
            statsLayout.childControlHeight = true;
            statsLayout.childControlWidth = true;
            statsLayout.childForceExpandHeight = true;
            statsRect.sizeDelta = new Vector2(0, 350);

            // Stat columns
            CreateStatColumn(statsContainer.transform, "Armor", out armorText, out upgradeArmorBtn);
            CreateStatColumn(statsContainer.transform, "Damage", out damageText, out upgradeDamageBtn);
            CreateStatColumn(statsContainer.transform, "Health", out healthText, out upgradeHealthBtn);

            // Close button
            GameObject closeObj = new GameObject("CloseBtn");
            closeObj.transform.SetParent(centerBox.transform, false);
            var closeRect = closeObj.AddComponent<RectTransform>();
            closeRect.sizeDelta = new Vector2(200, 60);
            var closeLayout = closeObj.AddComponent<LayoutElement>();
            closeLayout.preferredHeight = 60;
            closeLayout.preferredWidth = 200;
            
            Image closeImg = closeObj.AddComponent<Image>();
            closeImg.color = buttonNormalColor;
            closeBtn = closeObj.AddComponent<Button>();
            CreateText("CloseTxt", closeObj.transform, "CLOSE", 24, Color.white);

            // Assign listeners
            upgradeArmorBtn.onClick.AddListener(UpgradeArmor);
            upgradeDamageBtn.onClick.AddListener(UpgradeDamage);
            upgradeHealthBtn.onClick.AddListener(UpgradeHealth);
            closeBtn.onClick.AddListener(CloseMenu);

            menuPanel.SetActive(false);
        }

        private void CreateStatColumn(Transform parent, string title, out TextMeshProUGUI textRef, out Button btnRef)
        {
            GameObject col = new GameObject(title + "Col");
            col.transform.SetParent(parent, false);
            var layout = col.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 15;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlHeight = false;

            CreateText(title + "Title", col.transform, title.ToUpper(), 32, titleColor);

            GameObject txtObj = CreateText(title + "Info", col.transform, "Loading...", 24, Color.white);
            textRef = txtObj.GetComponent<TextMeshProUGUI>();

            GameObject btnObj = new GameObject(title + "Btn");
            btnObj.transform.SetParent(col.transform, false);
            var btnRect = btnObj.AddComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(180, 50);
            var btnLayout = btnObj.AddComponent<LayoutElement>();
            btnLayout.preferredHeight = 50;

            Image btnImg = btnObj.AddComponent<Image>();
            btnImg.color = buttonNormalColor;
            btnRef = btnObj.AddComponent<Button>();

            var colors = btnRef.colors;
            colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            btnRef.colors = colors;

            CreateText("BtnTxt", btnObj.transform, "UPGRADE", 24, Color.white);
        }

        private GameObject CreateText(string name, Transform parent, string content, int fontSize, Color color)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var text = obj.AddComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = TextAlignmentOptions.Center;
            return obj;
        }

        private void SetFullStretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
        }

        public void OpenMenu(UpgraderNPC npc, PlayerController player)
        {
            currentNPC = npc;
            currentPlayer = player;
            menuPanel.SetActive(true);
            RefreshUI();
            
            // optionally pause game
            Time.timeScale = 0f;
        }

        public void CloseMenu()
        {
            menuPanel.SetActive(false);
            currentNPC = null;
            currentPlayer = null;
            
            // resume game
            Time.timeScale = 1f;
        }

        private void RefreshUI()
        {
            if (currentNPC == null || currentPlayer == null) return;
            var levels = currentNPC.upgradeLevels;

            // Armor
            if (currentNPC.currentArmorLevelIndex < levels.Count)
            {
                var nextArmor = levels[currentNPC.currentArmorLevelIndex].armor;
                armorText.text = $"Armor Level {currentNPC.currentArmorLevelIndex + 1}\nEffect: +{nextArmor.value}\nCost: {nextArmor.price} coins";
                upgradeArmorBtn.interactable = currentPlayer.CurrentMoney >= nextArmor.price;
            }
            else
            {
                armorText.text = "Armor Maxed!";
                upgradeArmorBtn.interactable = false;
            }

            // Damage
            if (currentNPC.currentDamageLevelIndex < levels.Count)
            {
                var nextDam = levels[currentNPC.currentDamageLevelIndex].damage;
                damageText.text = $"Damage Level {currentNPC.currentDamageLevelIndex + 1}\nEffect: +{nextDam.value}\nCost: {nextDam.price} coins";
                upgradeDamageBtn.interactable = currentPlayer.CurrentMoney >= nextDam.price;
            }
            else
            {
                damageText.text = "Damage Maxed!";
                upgradeDamageBtn.interactable = false;
            }

            // Health
            if (currentNPC.currentHealthLevelIndex < levels.Count)
            {
                var nextHp = levels[currentNPC.currentHealthLevelIndex].health;
                healthText.text = $"Health Level {currentNPC.currentHealthLevelIndex + 1}\nEffect: +{nextHp.value}\nCost: {nextHp.price} coins";
                upgradeHealthBtn.interactable = currentPlayer.CurrentMoney >= nextHp.price;
            }
            else
            {
                healthText.text = "Health Maxed!";
                upgradeHealthBtn.interactable = false;
            }
        }

        private void UpgradeArmor()
        {
            var nextLevel = currentNPC.upgradeLevels[currentNPC.currentArmorLevelIndex].armor;
            if (currentPlayer.SpendMoney(nextLevel.price))
            {
                currentPlayer.SetBaseArmor(currentPlayer.CurrentArmorDefense + nextLevel.value);
                currentNPC.currentArmorLevelIndex++;
                RefreshUI();
            }
        }

        private void UpgradeDamage()
        {
            var nextLevel = currentNPC.upgradeLevels[currentNPC.currentDamageLevelIndex].damage;
            if (currentPlayer.SpendMoney(nextLevel.price))
            {
                var attacker = currentPlayer.GetComponent<Attacker.BaseAttacker>();
                if (attacker) attacker.Damage += nextLevel.value;
                currentNPC.currentDamageLevelIndex++;
                RefreshUI();
            }
        }

        private void UpgradeHealth()
        {
            var nextLevel = currentNPC.upgradeLevels[currentNPC.currentHealthLevelIndex].health;
            if (currentPlayer.SpendMoney(nextLevel.price))
            {
                var hp = currentPlayer.GetComponent<Health>();
                if (hp) hp.SetMaxHealth(hp.MaxHealth + nextLevel.value);
                currentNPC.currentHealthLevelIndex++;
                RefreshUI();
            }
        }
    }
}
