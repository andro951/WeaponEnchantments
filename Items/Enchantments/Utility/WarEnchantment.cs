using System;
using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class WarEnchantment : Enchantment
	{
		protected override string TypeName => "War";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 2;
		public override float ScalePercent => -1f;
		public override void GetMyStats() {
			Effects = new() {
				new EnemyMaxSpawns(multiplicative: EnchantmentStrengthData),
				new EnemySpawnRate(multiplicative: EnchantmentStrengthData)
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
			return ModContent.GetInstance<EnchantmentToggle>().War;
		}
	}
	[Autoload(false)]
	public class WarEnchantmentBasic : WarEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.PirateShip, chance: 0.5f),
			new(NPCID.PirateCaptain, chance: 0.25f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Shadow),
			new(ChestID.Shadow_Locked)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Obsidian_LockBox, 0.5f)
		};
	}
	[Autoload(false)]
	public class WarEnchantmentCommon : WarEnchantment { }
	[Autoload(false)]
	public class WarEnchantmentRare : WarEnchantment { }
	[Autoload(false)]
	public class WarEnchantmentEpic : WarEnchantment { }
	[Autoload(false)]
	public class WarEnchantmentLegendary : WarEnchantment { }
	[Autoload(false)]
	public class WarEnchantmentCursed : WarEnchantment { }
}
