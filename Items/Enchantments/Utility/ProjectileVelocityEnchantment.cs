using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class ProjectileVelocityEnchantment : Enchantment
	{
		public override int StrengthGroup => 10;
		public override void GetMyStats() {
			Effects = new() {
				new ProjectileVelocity(additive: EnchantmentStrengthData)
			};
		}

		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Sir Bumpleton ✿";
		public override string ArtModifiedBy => null;
		public override string Designer => "Sir Bumpleton ✿";
	}
	[Autoload(false)]
	public class ProjectileVelocityEnchantmentBasic : ProjectileVelocityEnchantment
	{
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Harpy),
			new(NPCID.SnowBalla),
			new(NPCID.SnowmanGangsta)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Frozen),
			new(CrateID.Boreal_FrozenHard)
		};
	}
	[Autoload(false)]
	public class ProjectileVelocityEnchantmentCommon : ProjectileVelocityEnchantment { }
	[Autoload(false)]
	public class ProjectileVelocityEnchantmentRare : ProjectileVelocityEnchantment { }
	[Autoload(false)]
	public class ProjectileVelocityEnchantmentEpic : ProjectileVelocityEnchantment { }
	[Autoload(false)]
	public class ProjectileVelocityEnchantmentLegendary : ProjectileVelocityEnchantment { }
}
