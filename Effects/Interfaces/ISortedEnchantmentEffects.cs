using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects
{
    public enum BaseEnum { }
	public interface ISortedEnchantmentEffects
	{
        public SortedDictionary<byte, EStatModifier> EnchantmentStats { set; get; }
        public SortedDictionary<byte, EStatModifier> VanillaStats { set; get; }
        public SortedDictionary<short, BuffStats> OnHitDebuffs { set; get; }
        public SortedDictionary<short, BuffStats> OnHitBuffs { set; get; }
        public SortedDictionary<short, BuffStats> OnTickBuffs { set; get; }

        public IEnumerable<EnchantmentEffect> EnchantmentEffects { set; get; }
        public IEnumerable<IPassiveEffect> PassiveEffects { set; get; }
        public IEnumerable<StatEffect> StatEffects { set; get; }
    }
}
