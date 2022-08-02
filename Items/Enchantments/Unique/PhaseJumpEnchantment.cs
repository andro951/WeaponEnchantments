using System.Collections.Generic;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class PhaseJumpEnchantment : Enchantment
	{
		public override bool? ShowPercentSignInTooltip => false;
		public override bool? MultiplyBy100InTooltip => false;
		public override string CustomTooltip => $"(Dash)";
		public override int StrengthGroup => 10;
		public override float ScalePercent => 0.6f;
		public override int ArmorSlotSpecific => (int)ArmorSlotSpecificID.Legs;
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Armor", 1f }
		};
		public override void GetMyStats() {
			AddStaticStat("dashType", 0f, 1f, 0f, 3f);
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
