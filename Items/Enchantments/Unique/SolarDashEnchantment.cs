using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class SolarDashEnchantment : Enchantment {
		protected override string TypeName => "SolarDash";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 1;
		public override int ArmorSlotSpecific => (int)ArmorSlotSpecificID.Legs;
		public override void GetMyStats() {
			DashID_WE dash = EnchantmentTier >= 3 ? DashID_WE.SolarDash : EnchantmentTier > 1 ? DashID_WE.NinjaTabiDash : DashID_WE.EyeOfCthulhuShieldDash;

			Effects = new() {
				new VanillaDash(dash, EnchantmentStrengthData)
			};

			if (EnchantmentTier > 0)
				Effects.Add(new MovementSpeed(additive: EnchantmentStrengthData));

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Armor, 1f }
			};
		}
		protected override List<List<EnchantmentEffect>> cursedEffectPossibilities => MyCursedEffectPossibilities;
		private List<List<EnchantmentEffect>> MyCursedEffectPossibilities {
			get {
				if (myCursedEffectPossibilities == null) {
					myCursedEffectPossibilities = new() {
						defense,
						damageReduction,
						moveControl,
						damage
					};
				}

				return myCursedEffectPossibilities;
			}
		}
		private List<List<EnchantmentEffect>> myCursedEffectPossibilities;

		public override string ShortTooltip => GetShortTooltip(showValue: false);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => "𝐍𝐢𝐱𝐲♱";
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().SolarDash;
		}
	}
	[Autoload(false)]
	public class SolarDashEnchantmentBasic : SolarDashEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostDeerclops;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Deerclops)
		};
	}
	[Autoload(false)]
	public class SolarDashEnchantmentCommon : SolarDashEnchantment { }
	[Autoload(false)]
	public class SolarDashEnchantmentRare : SolarDashEnchantment { }
	[Autoload(false)]
	public class SolarDashEnchantmentEpic : SolarDashEnchantment { }
	[Autoload(false)]
	public class SolarDashEnchantmentLegendary : SolarDashEnchantment { }
	[Autoload(false)]
	public class SolarDashEnchantmentCursed : SolarDashEnchantment { }
}
