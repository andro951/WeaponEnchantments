

namespace WeaponEnchantments.Common
{
    public class EnchantmentStaticStat
    {
        public string Name { get; private set; }
        public float Additive { get; private set; }
        public float Multiplicative { get; private set; }
        public float Flat { get; private set; }
        public float Base { get; private set; }
        public bool PreventBoolStat { get; private set; }
        public EnchantmentStaticStat(string name, float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) {
            Name = name;
            Additive = additive;
            Multiplicative = multiplicative;
            Flat = flat;
            Base = @base;
        }
    }
}
