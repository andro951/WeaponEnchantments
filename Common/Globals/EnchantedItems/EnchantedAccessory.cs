using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using static WeaponEnchantments.Common.EnchantingRarity;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using static WeaponEnchantments.Items.Enchantment;

namespace WeaponEnchantments.Common.Globals
{
	public class EnchantedAccessory : EnchantedEquipItem
	{
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => IsAccessoryItem(entity);
		public override EItemType ItemType => EItemType.Accessories;

		public override Dictionary<string, string>[] SkillPointsToNames() =>
            new Dictionary<string, string>[] {
                new Dictionary<string, string>() {
                    { "Skill", "Speed" },
                    { "Scaling", "+0.5% Movement Speed & +0.5% Control / Level" },
                    { "Milestone1", "+5% Control" },
                    { "Milestone2", "+5% Movement Speed" },
                    { "Milestone3", "Falling onto enemies allows you to stomp them, dealing damage equal to current speed" }
                },
                new Dictionary<string, string>() {
                    { "Skill", "Greed" },
                    { "Scaling", "+2.5% Coins Dropped / Level" },
                    { "Milestone1", "+25% Coins Dropped" },
                    { "Milestone2", "Each coin picked up restores mana and health" },
                    { "Milestone3", "Taking coins has a chance to spawn another stack with half the value" }
                },
                new Dictionary<string, string>() {
                    { "Skill", "Luck" },
                    { "Scaling", "+0.005 Luck / Level" },
                    { "Milestone1", "+0.05 Luck" },
                    { "Milestone2", "Your attacks may inflict random debuffs on foes" },
                    { "Milestone3", "Lucky hit: Adds a separate chance (that scales with luck) to deal a lucky hit, which deals twice the damage and can stack with crits" }
                }
            };

        public override void SkillPointsToStats()
		{
			throw new NotImplementedException();
		}
	}
}
