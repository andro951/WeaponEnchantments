using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Common.Utility;
using androLib.Common.Globals;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class CatastrophicReleaseEnchantment : Enchantment
	{
		protected override string TypeName => "CatastrophicRelease";
		protected override string NamePrefix => "Enchantments/";
		
		public override string CustomTooltip => "(Does not work, Not finished)";
		public override int StrengthGroup => 8;
		public override int DamageClassSpecific => (int)DamageClassID.Magic;
		public override SellCondition SellCondition => SellCondition.HardMode;
		public override void GetMyStats() {
			//AddEStat(EnchantmentTypeName, 0f, 1f, EnchantmentStrength);
			//AddEStat("InfinitePenetration", 0f, 1f, 13.13f);
			//AddStaticStat("scale", 0f, EnchantmentStrength * 10f);
			//AddStaticStat("shootSpeed", 0f, 1f - 0.8f * EnchantmentStrength);
			//AddStaticStat("useTime", 0f, 1000f);
			//AddStaticStat("P_autoReuse", EnchantmentStrength);

			Effects = new() {
				new ProjectileVelocity(multiplicative: EnchantmentStrengthData * 0f + 0.2f),
				new Channel(),
				new InfinitePenetration(),
				new CatastrophicRelease(@base: EnchantmentStrengthData),
				new Size(EnchantmentStrengthData * 10f)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "andro951";
		public override string Designer => "andro951";
	}
	/*
	public class CatastrophicReleaseEnchantmentBasic : CatastrophicReleaseEnchantment {
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.TheDestroyer, 1f)
		};
	}
	public class CatastrophicReleaseEnchantmentCommon : CatastrophicReleaseEnchantment { }
	public class CatastrophicReleaseEnchantmentRare : CatastrophicReleaseEnchantment { }
	public class CatastrophicReleaseEnchantmentEpic : CatastrophicReleaseEnchantment { }
	public class CatastrophicReleaseEnchantmentLegendary : CatastrophicReleaseEnchantment { }
	public class CatastrophicReleaseEnchantmentCursed : CatastrophicReleaseEnchantment { }
	*/
}
