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

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            if (menuPanel != null) menuPanel.SetActive(false);

            upgradeArmorBtn.onClick.AddListener(UpgradeArmor);
            upgradeDamageBtn.onClick.AddListener(UpgradeDamage);
            upgradeHealthBtn.onClick.AddListener(UpgradeHealth);
            closeBtn.onClick.AddListener(CloseMenu);
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
                armorText.text = $"Armor Level {currentNPC.currentArmorLevelIndex + 1}\nEffect: {nextArmor.value}\nCost: {nextArmor.price} coins";
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
                damageText.text = $"Damage Level {currentNPC.currentDamageLevelIndex + 1}\nEffect: {nextDam.value}\nCost: {nextDam.price} coins";
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
                healthText.text = $"Health Level {currentNPC.currentHealthLevelIndex + 1}\nEffect: {nextHp.value}\nCost: {nextHp.price} coins";
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
                currentPlayer.SetBaseArmor(nextLevel.value);
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
                if (attacker) attacker.Damage = nextLevel.value;
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
                if (hp) hp.SetMaxHealth(nextLevel.value);
                currentNPC.currentHealthLevelIndex++;
                RefreshUI();
            }
        }
    }
}

