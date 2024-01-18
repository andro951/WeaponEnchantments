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
using static androLib.Common.EnchantingRarity;
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