using androLib.Common.Utility;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class PeaceEnchantment : Enchantment {
		protected override string TypeName => "Peace";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 2;
		public override float ScalePercent => -1f;
		public override void GetMyStats() {
			Effects = new() {
				new EnemyMaxSpawns(multiplicative: EnchantmentStrengthData.Invert()),
				new EnemySpawnRate(multiplicative: EnchantmentStrengthData.Invert())
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(percent: false, multiply100: false, multiplicative: true);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().Peace;
		}
	}
	[Autoload(false)]
	public class PeaceEnchantmentBasic : PeaceEnchantment
	{
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Pixie, chance: 0.1f)
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
	public class PeaceEnchantmentCommon : PeaceEnchantment { }
	[Autoload(false)]
	public class PeaceEnchantmentRare : PeaceEnchantment { }
	[Autoload(false)]
	public class PeaceEnchantmentEpic : PeaceEnchantment { }
	[Autoload(false)]
	public class PeaceEnchantmentLegendary : PeaceEnchantment { }

}
