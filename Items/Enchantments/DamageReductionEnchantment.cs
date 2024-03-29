﻿using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class DamageReductionEnchantment : Enchantment {
        public override float ScalePercent => 0.6f;
        public override float CapacityCostMultiplier => 3f;
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
		public override string Artist => "Zorutan";
        public override string ArtModifiedBy => null;
        public override string Designer => "andro951";
    }
    [Autoload(false)]
	public class DamageReductionEnchantmentBasic : DamageReductionEnchantment
    {
        public override SellCondition SellCondition => SellCondition.PostSkeletron;
        public override List<DropData> NpcDropTypes => new() {
            new(NPCID.GraniteFlyer),
            new(NPCID.GraniteGolem),
            new(NPCID.GreekSkeleton),
            new(NPCID.PossessedArmor),
            new(NPCID.GiantTortoise),
            new(NPCID.IceTortoise)
        };
        public override List<DropData> CrateDrops => new() {
            new(CrateID.Dungeon, 0.5f),
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

}
