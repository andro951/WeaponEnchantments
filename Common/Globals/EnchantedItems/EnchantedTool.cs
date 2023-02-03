using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;

namespace WeaponEnchantments.Common.Globals
{
	public class EnchantedTool : EnchantedHeldItem
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
			return IsTool(entity);
		}

        public override EItemType ItemType => EItemType.Tools;

        public override Dictionary<string, string>[] SkillPointsToNames() =>
            new Dictionary<string, string>[] {
                new Dictionary<string, string>() {
                    { "Skill", "Swiftness" },
                    { "Scaling", "+1.5% Mining Speed / Level" },
                    { "Milestone1", "+15% Mining Speed" },
                    { "Milestone2", "Passive spelunker's glowstick effect" },
                    { "Milestone3", "Everything takes -1 less hit to break" }
                },
                new Dictionary<string, string>() {
                    { "Skill", "Reach" },
                    { "Scaling", "+0.125 Tile Reach / Level" },
                    { "Milestone1", "+1 Reach" },
                    { "Milestone2", "Tool size +100%, +1 Reach" },
                    { "Milestone3", "Tool mines in 3x3, +1 Reach" }
                },
                new Dictionary<string, string>() {
                    { "Skill", "Violence" },
                    { "Scaling", "+0.5% Damage & +0.5% Critical Chance / Level" },
                    { "Milestone1", "+5% Damage & +5% Critical Chance" },
                    { "Milestone2", "Mining blocks increases the damage of the next hit, up to +100% at 100 blocks" },
                    { "Milestone3", "Killing enemies drops extra resources (woods acorns and fruits for axes, ores and gems for pickaxes, both for others) based on tool power" }
                }
            };

        public override void SkillPointsToStats()
        {
            throw new NotImplementedException();
        }
    }
}
