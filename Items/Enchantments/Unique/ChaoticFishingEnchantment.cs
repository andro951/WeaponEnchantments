using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Unique {
    public abstract class ChaoticFishingEnchantment : Enchantment {
	    protected override string TypeName => "ChaoticFishing";
	    protected override string NamePrefix => "Enchantments/";
	    
		public override int StrengthGroup => 7;
        public override void GetMyStats() {
            Effects = new() {
                new FishingEnemySpawnChance(@base: EnchantmentStrengthData)
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
	        return ModContent.GetInstance<EnchantmentToggle>().ChaoticFishing;
        }
    }
    [Autoload(false)]
	public class ChaoticFishingEnchantmentBasic : ChaoticFishingEnchantment
    {
        public override SellCondition SellCondition => SellCondition.HardMode;
        public override List<DropData> NpcDropTypes => new() {
            new(NPCID.GoblinShark),
            new(NPCID.BloodEelHead),
            new(NPCID.BloodNautilus),
            new(NPCID.Shark)
        };

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
	public class ChaoticFishingEnchantmentCommon : ChaoticFishingEnchantment { }
    [Autoload(false)]
	public class ChaoticFishingEnchantmentRare : ChaoticFishingEnchantment { }
    [Autoload(false)]
	public class ChaoticFishingEnchantmentEpic : ChaoticFishingEnchantment { }
    [Autoload(false)]
	public class ChaoticFishingEnchantmentLegendary : ChaoticFishingEnchantment { }
	[Autoload(false)]
	public class ChaoticFishingEnchantmentCursed : ChaoticFishingEnchantment { }

}
