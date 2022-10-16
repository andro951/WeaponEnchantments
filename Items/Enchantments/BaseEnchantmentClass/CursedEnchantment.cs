using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using System.Reflection;
using Terraria.GameContent.Creative;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using static WeaponEnchantments.Common.Utility.LogModSystem;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.EnchantingRarity;
using Terraria.Localization;
using System.Linq;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Common.Globals;
using Terraria.ModLoader.IO;
using System.IO;

namespace WeaponEnchantments.Items {
	public abstract class CursedEnchantment : Enchantment {
		public override void LoadData(TagCompound tag) {
			
		}
		public override void SaveData(TagCompound tag) {
			
		}
		public override void NetReceive(BinaryReader reader) {
			
		}
		public override void NetSend(BinaryWriter writer) {
			
		}
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
		}
		public override void SetDefaults() {
			base.SetDefaults();
			Item.maxStack = 1;
		}
		public override bool CanStack(Item item2) => false;

		public override string Artist => "andro951";

		public override string Designer => "andro951";
	}
}

/*
public abstract class AllForOneEnchantment : Enchantment
	{
		public override int StrengthGroup => 6;
		public override float ScalePercent => 0.8f;
		public override bool Max1 => true;
		public override List<int> RestrictedClass => new() { (int)DamageClassID.Summon };
		public override void GetMyStats() {
			Effects = new() {
				new AllForOne(EnchantmentStrengthData * 0.4f + 4f),
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new NPCHitCooldown(multiplicative: EnchantmentStrengthData * 0.4f + 4f),
				new AttackSpeed(multiplicative: (EnchantmentStrengthData * 0.1f + 1f).Invert()),
				new ManaUsage(@base: EnchantmentStrengthData * 0.15f + 1.5f),
				new AutoReuse(prevent: true)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string ShortTooltip => $"{Math.Round(EnchantmentStrength * AllowedListMultiplier, 3)}x {GetLocalizationTypeName()}";
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class AllForOneEnchantmentBasic : AllForOneEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostSkeletron;
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.Mothron)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Gold_Locked, 1f },
			{ ChestID.Lihzahrd, 1f }
		};
		public override List<WeightedPair> CrateDrops => new() {
			new(CrateID.Golden_LockBox, 0.45f)
		};
	}
	public class AllForOneEnchantmentCommon : AllForOneEnchantment { }
	public class AllForOneEnchantmentRare : AllForOneEnchantment { }
	public class AllForOneEnchantmentEpic : AllForOneEnchantment { }
	public class AllForOneEnchantmentLegendary : AllForOneEnchantment { }
*/
