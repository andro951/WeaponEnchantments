using System.Collections.Generic;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class PhaseJumpEnchantment : Enchantment
	{
		public override string CustomTooltip => $"(Dash)";
		public override int StrengthGroup => 10;
		public override float ScalePercent => 0.6f;
		public override int ArmorSlotSpecific => (int)ArmorSlotSpecificID.Legs;
		public override void GetMyStats() {
			AddStaticStat("dashType", 0f, 1f, 0f, 3f);

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Armor, 1f }
			};
		}

		public override string Artist => "andro951";
		public override string Designer => "andro951";
	}
	/*public class PhaseJumpEnchantmentBasic : PhaseJumpEnchantment { }
	public class PhaseJumpEnchantmentCommon : PhaseJumpEnchantment { }
	public class PhaseJumpEnchantmentRare : PhaseJumpEnchantment { }
	public class PhaseJumpEnchantmentSuperRare : PhaseJumpEnchantment { }
	public class PhaseJumpEnchantmentUltraRare : PhaseJumpEnchantment { }*/
}
