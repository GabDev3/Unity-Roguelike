namespace Interaction
{
    public class Armor: InteractableItem
    {
        public int Resistance = 1;
        public string ArmorName = "Cloth Armor";

        protected override void OnInteraction()
        {
            if (_targetPlayer != null)
            {
                _targetPlayer.EquipArmor(this);
            }
        }
    }
}