using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class MovementSpeedEnchantment : Enchantment
	{
		protected override string TypeName => "MovementSpeed";
		protected override string NamePrefix => "Enchantments/";
		
		public override int StrengthGroup => 11;
		public override void GetMyStats() {
			Effects = new() {
				new MovementSpeed(EnchantmentStrengthData),
				new FlightSpeed(EnchantmentStrengthData),
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
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
		public override string ShortTooltip => GetShortTooltip(sign: true);
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
		
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModContent.GetInstance<EnchantmentToggle>().MovementSpeed;
		}
	}
	[Autoload(false)]
	public class MovementSpeedEnchantmentBasic : MovementSpeedEnchantment
	{
		public override SellCondition SellCondition => SellCondition.PostEyeOfCthulhu;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.EyeofCthulhu),
			new(NPCID.GiantWalkingAntlion, 10f),
			new(NPCID.WalkingAntlion, 10f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Oasis, 0.5f),
			new(CrateID.Mirage_OasisHard, 0.5f)
		};
	}
	[Autoload(false)]
	public class MovementSpeedEnchantmentCommon : MovementSpeedEnchantment { }
	[Autoload(false)]
	public class MovementSpeedEnchantmentRare : MovementSpeedEnchantment { }
	[Autoload(false)]
	public class MovementSpeedEnchantmentEpic : MovementSpeedEnchantment { }
	[Autoload(false)]
	public class MovementSpeedEnchantmentLegendary : MovementSpeedEnchantment { }
	[Autoload(false)]
	public class MovementSpeedEnchantmentCursed : MovementSpeedEnchantment { }
}