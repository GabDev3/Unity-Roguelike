using UnityEngine;
 
namespace Weapons
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Spell Data")]
    public class SpellData : ScriptableObject
    {
        [Header("Base Stats")]
        public int BaseDamage = 10;
        public int Penetration = 1;
        public float ProjectileSpeed = 10f;
        public GameObject ProjectilePrefab;
    }
}