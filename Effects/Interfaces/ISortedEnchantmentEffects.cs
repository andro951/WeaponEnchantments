using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects
{
	public interface ISortedEnchantmentEffects
	{
        public SortedDictionary<EnchantmentStat, EStatModifier> EnchantmentStats { set; get; }
        public SortedDictionary<EnchantmentStat, EStatModifier> VanillaStats { set; get; }
	    public SortedList<EnchantmentStat, PlayerSetEffect> PlayerSetEffects { set; get; }
        public SortedDictionary<short, BuffStats> OnTickBuffs { set; get; }

        public List<EnchantmentEffect> EnchantmentEffects { set; get; }
        public List<IPassiveEffect> PassiveEffects { set; get; }
        public List<IModifyShootStats> ModifyShootStatEffects { set; get; }
        public List<StatEffect> StatEffects { set; get; }
    }

    public interface ISortedOnHitEffects
	{
        public SortedDictionary<short, BuffStats> OnHitDebuffs { set; get; }
        public SortedDictionary<short, BuffStats> OnHitBuffs { set; get; }
        public List<IOnHitEffect> OnHitEffects { set; get; }
    }
}
