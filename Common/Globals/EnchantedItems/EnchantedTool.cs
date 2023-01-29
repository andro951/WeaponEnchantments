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

        public override (string, string, string) SkillPointsToNames() => ("Swiftness", "Reach", "Violence");

        public override void SkillPointsToStats()
        {
            throw new NotImplementedException();
        }
    }
}
