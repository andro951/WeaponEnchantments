using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Unique {
    public abstract class NpcContactAnglerEnchantment : Enchantment {
	    protected override string TypeName => "NpcContactAngler";
	    protected override string NamePrefix => "Enchantments/";
	    
		public override int StrengthGroup => 5;
        public override void GetMyStats() {
            Effects = new() {
                new QuestFishChance(@base: EnchantmentStrengthData)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.FishingPoles, 1f }
            };
		}
		protected override List<List<EnchantmentEffect>> cursedEffectPossibilities => defaultUtilityCursedEffectPossibilities;

		public override string Artist => "andro951";
        public override string ArtModifiedBy => null;
        public override string Designer => "andro951";
        
        public override bool IsLoadingEnabled(Mod mod)
        {
	        return ModContent.GetInstance<EnchantmentToggle>().NpcContactAngler;
        }
    }
    [Autoload(false)]
	public class NpcContactAnglerEnchantmentBasic : NpcContactAnglerEnchantment
    {
        public override SellCondition SellCondition => SellCondition.AnyTimeRare;
        public override List<DropData> CrateDrops => new() {
            new(CrateID.Golden),
            new(CrateID.Titanium_GoldenHard),
			new(CrateID.Golden_LockBox),
			new(CrateID.Obsidian_LockBox)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Water),
		};
	}
    [Autoload(false)]
	public class NpcContactAnglerEnchantmentCommon : NpcContactAnglerEnchantment { }
    [Autoload(false)]
	public class NpcContactAnglerEnchantmentRare : NpcContactAnglerEnchantment { }
    [Autoload(false)]
	public class NpcContactAnglerEnchantmentEpic : NpcContactAnglerEnchantment { }
    [Autoload(false)]
	public class NpcContactAnglerEnchantmentLegendary : NpcContactAnglerEnchantment { }
	[Autoload(false)]
	public class NpcContactAnglerEnchantmentCursed : NpcContactAnglerEnchantment { }

}
