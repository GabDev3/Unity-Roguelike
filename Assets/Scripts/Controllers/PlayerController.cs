using UnityEngine;
using UI;
using Interaction;

namespace Controllers
{
    public class PlayerController : BaseController, IDamageable
    {
        [Header("Dependencies")]
        public PlayerHud hud;

        public int CurrentWeaponDamage { get; private set; } = 0;
        public int CurrentArmorDefense { get; private set; } = 0;
        public int CurrentMoney { get; private set; } = 0;

        private void Awake()
        {
            hud.UpdateMoneyDisplay(CurrentMoney);
            hud.UpdateHealth(health.CurrentHealth, health.MaxHealth);
            hud.UpdateArmorStats(CurrentArmorDefense);
        }

        public void EquipWeapon(MeleeWeapon newWeapon)
        {
            CurrentWeaponDamage = newWeapon.Damage;

            if (hud)
            {
                Sprite weaponSprite = newWeapon.GetComponent<SpriteRenderer>().sprite;
                hud.UpdateWeaponSlot(weaponSprite);
            }

            Debug.Log($"Equipped: {newWeapon.name}. Damage is now: {CurrentWeaponDamage}");

            Destroy(newWeapon.gameObject);
        }

        public void EquipArmor(Armor newArmor)
        {
            CurrentArmorDefense = newArmor.Resistance;
            Resistance = CurrentArmorDefense;
            
            if (hud)
            {
                Sprite armorSprite = newArmor.GetComponent<SpriteRenderer>().sprite;
                hud.UpdateArmorSlot(armorSprite);
                hud.UpdateArmorStats(CurrentArmorDefense);
            }
            Debug.Log($"Equipped Armor. Defense is now: {CurrentArmorDefense}");

            Destroy(newArmor.gameObject);
        }

        public void AddMoney(int amount)
        {
            CurrentMoney += amount;
            if (hud)
            {
                hud.UpdateMoneyDisplay(CurrentMoney);
            }
        }
        
        public bool SpendMoney(int amount)
        {
            if (CurrentMoney >= amount)
            {
                CurrentMoney -= amount;
                if (hud)
                {
                    hud.UpdateMoneyDisplay(CurrentMoney);
                }
                return true;
            }
            return false;
        }

        public void TakeDamage(int damage)
        {
            int finalDamage = Mathf.Max(0, damage - CurrentArmorDefense);

            if (health)
            {
                hud.UpdateHealth(health.CurrentHealth - finalDamage, health.MaxHealth);
                health.DecreaseHealth(finalDamage);
            }
            
        }

        public void TogglePickItemPrompt(bool isVisible, string displayedText="default")
        {
            hud.TogglePickItemText(isVisible, displayedText);
        }
        
        public void SetBaseArmor(int amount)
        {
            CurrentArmorDefense = amount;
            Resistance = CurrentArmorDefense;
            if (hud)
            {
                hud.UpdateArmorStats(CurrentArmorDefense);
            }
        }
    }
}