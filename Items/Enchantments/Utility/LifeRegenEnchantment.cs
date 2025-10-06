using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
    public abstract class LifeRegenEnchantment : Enchantment
    {
        protected override string TypeName => "LifeRegen";
        protected override string NamePrefix => "Enchantments/";
        
        public override int StrengthGroup => 22;
        public override void GetMyStats()
        {
            Effects = new() {
                new LifeRegeneration(@base: EnchantmentStrengthData),
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

		public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
        public override string Artist => "Auseawesome";
        public override string ArtModifiedBy => null;
        public override string Designer => "Auseawesome";
        
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<EnchantmentToggle>().LifeRegen;
        }
    }
    [Autoload(false)]
	public class LifeRegenEnchantmentBasic : LifeRegenEnchantment
    {
        public override SellCondition SellCondition => SellCondition.AnyTime;
        public override List<DropData> NpcAIDrops => new() {
            new(NPCAIStyleID.Fighter, 0.05f)
        };
		public override List<DropData> ChestDrops => new() {
            new(ChestID.Chest_Normal)
        };
        public override List<DropData> CrateDrops => new() {
            new(CrateID.Wooden, 0.25f),
            new(CrateID.Pearlwood_WoodenHard, 0.25f)
        };
    }
    [Autoload(false)]
	public class LifeRegenEnchantmentCommon : LifeRegenEnchantment { }
    [Autoload(false)]
	public class LifeRegenEnchantmentRare : LifeRegenEnchantment { }
    [Autoload(false)]
	public class LifeRegenEnchantmentEpic : LifeRegenEnchantment { }
    [Autoload(false)]
	public class LifeRegenEnchantmentLegendary : LifeRegenEnchantment { }
	[Autoload(false)]
	public class LifeRegenEnchantmentCursed : LifeRegenEnchantment { }
}