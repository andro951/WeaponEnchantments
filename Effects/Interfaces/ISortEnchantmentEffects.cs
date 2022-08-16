using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects
{
    public enum BaseEnum { }
	public interface ISortEnchantmentEffects
	{
        public SortedDictionary<byte, CalcStatModifier> EnchantmentStats { set; get; }
        public SortedDictionary<byte, CalcStatModifier> VanillaStats { set; get; }
        public SortedDictionary<short, int> OnHitDebuffs { set; get; }
        public SortedDictionary<short, int> OnHitBuffs { set; get; }
        public SortedDictionary<short, int> OnTickBuffs { set; get; }

        public IEnumerable<EnchantmentEffect> EnchantmentEffects { set; get; }
        public IEnumerable<IPassiveEffect> PassiveEffects { set; get; }
        public IEnumerable<StatEffect> StatEffects { set; get; }
    }
}
