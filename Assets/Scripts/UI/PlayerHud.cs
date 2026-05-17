using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace UI
{
    public class PlayerHud : MonoBehaviour
    {
        [Header("Health Stats")] public Slider hpSlider;
        public TextMeshProUGUI hpText;

        [Header("Armor Stats")] public TextMeshProUGUI armorValueText;

        [Header("Equipment Slots")] public Image weaponSlotImage;
        public Image armorSlotImage;
        public Sprite emptySlotSprite;

        [Header("Money")] public TextMeshProUGUI moneyText;

        public TextMeshProUGUI pickItemText;

        void Awake()
        {
            pickItemText.enabled = false;
        }


        public void UpdateHealth(int currentHealth, int maxHealth)
        {
            if (hpSlider != null)
            {
                hpSlider.maxValue = maxHealth;
                hpSlider.value = currentHealth;
            }

            if (hpText != null)
            {
                hpText.text = $"{currentHealth} / {maxHealth}";
            }
        }

        public void UpdateArmorStats(int armorValue)
        {
            if (armorValueText != null)
            {
                armorValueText.text = armorValue > 0 ? $"{armorValue}" : "0";
            }
        }

        public void UpdateWeaponSlot(Sprite weaponSprite)
        {
            if (weaponSprite != null)
            {
                weaponSlotImage.sprite = weaponSprite;
                weaponSlotImage.color = Color.white;
            }
            else
            {
                weaponSlotImage.sprite = emptySlotSprite;
            }
        }

        public void UpdateArmorSlot(Sprite armorSprite)
        {
            if (armorSprite != null)
            {
                armorSlotImage.sprite = armorSprite;
                armorSlotImage.color = Color.white;
            }
            else
            {
                armorSlotImage.sprite = emptySlotSprite;
            }
        }

        public void UpdateMoneyDisplay(int currentAmount)
        {
            if (moneyText != null)
            {
                moneyText.text = currentAmount.ToString();
            }
        }

        public void TogglePickItemText(bool isVisible, string displayedText)
        {
            if (displayedText != "default")
            {
                pickItemText.text = displayedText;
            }
            else if (displayedText == "default")
            {
                pickItemText.text = "Press 'E' to pick up the item";
            }

            pickItemText.enabled = isVisible;
        }
    }
}