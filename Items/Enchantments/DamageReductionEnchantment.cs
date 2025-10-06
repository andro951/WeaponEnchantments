using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class DamageReductionEnchantment : Enchantment
    {
	    protected override string TypeName => "DamageReduction";
	    protected override string NamePrefix => "Enchantments/";
	    
        public override float ScalePercent => 0.6f;
        public override float CapacityCostMultiplier => CapacityCostUnique;
		public override int StrengthGroup => 20;
        public override void GetMyStats() {
            Effects = new() {
                new DamageReduction(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.Weapons, 0.5f },
				{ EItemType.Tools, 0.5f },
				{ EItemType.FishingPoles, 0.5f },
				{ EItemType.Armor, 1f },
                { EItemType.Accessories, 1f }
            };
		}
		protected override List<List<EnchantmentEffect>> cursedEffectPossibilities => defaultDefensiveCursedEffectPossibilities;

		public override string Artist => "Zorutan";
        public override string ArtModifiedBy => null;
        public override string Designer => "andro951";
        
        public override bool IsLoadingEnabled(Mod mod)
        {
	        return ModContent.GetInstance<EnchantmentToggle>().DamageReduction;
        }
    }
    [Autoload(false)]
	public class DamageReductionEnchantmentBasic : DamageReductionEnchantment
    {
        public override SellCondition SellCondition => SellCondition.PostSkeletron;
        public override List<DropData> NpcDropTypes => new() {
            new(NPCID.GraniteFlyer, chance: 0.1f),
            new(NPCID.GraniteGolem, chance : 0.1f),
            new(NPCID.GreekSkeleton, chance: 0.1f),
            new(NPCID.PossessedArmor, chance: 0.05f),
            new(NPCID.GiantTortoise, chance: 0.1f),
            new(NPCID.IceTortoise, chance: 0.2f)
        };
        public override List<DropData> CrateDrops => new() {
            new(CrateID.Stockade_DungeonHard, 0.5f)
        };
    }
    [Autoload(false)]
	public class DamageReductionEnchantmentCommon : DamageReductionEnchantment { }
    [Autoload(false)]
	public class DamageReductionEnchantmentRare : DamageReductionEnchantment { }
    [Autoload(false)]
	public class DamageReductionEnchantmentEpic : DamageReductionEnchantment { }
    [Autoload(false)]
	public class DamageReductionEnchantmentLegendary : DamageReductionEnchantment { }
	[Autoload(false)]
	public class DamageReductionEnchantmentCursed : DamageReductionEnchantment { }

}
