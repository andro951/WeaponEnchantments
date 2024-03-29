﻿using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class ReducedManaUsageEnchantment : Enchantment
	{
		public override void GetMyStats() {
			Effects = new() {
				new ManaUsage(@base: EnchantmentStrengthData * -1f)
			};
		}

		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	[Autoload(false)]
	public class ReducedManaUsageEnchantmentBasic : ReducedManaUsageEnchantment
	{
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.BrainofCthulhu)
		};
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Worm),
			new(NPCAIStyleID.Caster),
			new(NPCAIStyleID.CursedSkull)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Chest_Normal),
			new(ChestID.Frozen)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f),
			new(CrateID.Frozen, 0.5f),
			new(CrateID.Boreal_FrozenHard, 0.5f)
		};
	}
	[Autoload(false)]
	public class ReducedManaUsageEnchantmentCommon : ReducedManaUsageEnchantment { }
	[Autoload(false)]
	public class ReducedManaUsageEnchantmentRare : ReducedManaUsageEnchantment { }
	[Autoload(false)]
	public class ReducedManaUsageEnchantmentEpic : ReducedManaUsageEnchantment { }
	[Autoload(false)]
	public class ReducedManaUsageEnchantmentLegendary : ReducedManaUsageEnchantment { }

}
