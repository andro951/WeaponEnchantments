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
	public class EnchantedFishingPole : EnchantedHeldItem
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
			return IsFishingRod(entity);
		}
		public override EItemType ItemType => EItemType.FishingPoles;

		public override Dictionary<string, string>[] SkillPointsToNames() =>
            new Dictionary<string, string>[] {
                new Dictionary<string, string>() {
                    { "Skill", "Alluring" },
                    { "Scaling", "+0.5 Fishing Power / Level" },
                    { "Milestone1", "+5 Fishing Power" },
                    { "Milestone2", "Potion fish are more common" },
                    { "Milestone3", "The elusive Essence Fish can now be caught" }
                },
                new Dictionary<string, string>() {
                    { "Skill", "Efficiency" },
                    { "Scaling", "+1% Chance to not consume Bait / Level" },
                    { "Milestone1", "+20% Chance not to consume bait, double if using golden bait" },
                    { "Milestone2", "+1 Bobber" },
                    { "Milestone3", "Each bobber has a 50% chance to reel in a second catch" }
                },
                new Dictionary<string, string>() {
                    { "Skill", "Luck" },
                    { "Scaling", "+2.5% Crate Chance / Level" },
                    { "Milestone1", "+25% Crate Chance" },
                    { "Milestone2", "Fishing a crate also rewards a random fish" },
                    { "Milestone3", "Biome crates are twice as more likely to be fished out" }
                }
            };

        public override void SkillPointsToStats()
		{
			throw new NotImplementedException();
		}
	}
}
