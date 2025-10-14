using androLib.Common.Utility;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class ProjectileVelocityEnchantment : Enchantment
	{
		protected override string TypeName => "ProjectileVelocity";
		protected override string NamePrefix => "Enchantments/";
		
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
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().ProjectileVelocity;
		}
	}
	[Autoload(false)]
	public class ProjectileVelocityEnchantmentBasic : ProjectileVelocityEnchantment
	{
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Harpy, chance: 0.01f),
			new(NPCID.SnowBalla, chance: 0.05f),
			new(NPCID.SnowmanGangsta, chance: 0.05f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Frozen)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Frozen, 0.5f),
			new(CrateID.Boreal_FrozenHard, 0.5f)
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
	[Autoload(false)]
	public class ProjectileVelocityEnchantmentCursed : ProjectileVelocityEnchantment { }
}
