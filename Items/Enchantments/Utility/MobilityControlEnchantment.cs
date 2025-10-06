using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Utility {
    public abstract class MobilityControlEnchantment : Enchantment {
	    protected override string TypeName => "MobilityControl";
	    protected override string NamePrefix => "Enchantments/";
	    
        public override int StrengthGroup => 12;
		public override void GetMyStats() {
            Effects = new() {
                new MaxFallSpeed(EnchantmentStrengthData),
                new MovementSlowdown(EnchantmentStrengthData),
                new MovementAcceleration(EnchantmentStrengthData),
                new FlightAcceleration(EnchantmentStrengthData),
                new JumpSpeed(EnchantmentStrengthData / 2f),
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
						moveSpeed,
						damage
					};
				}

				return myCursedEffectPossibilities;
			}
		}
		private List<List<EnchantmentEffect>> myCursedEffectPossibilities;

		public override string Artist => "Sir Bumpleton ✿";
        public override string ArtModifiedBy => null;
        public override string Designer => "Sir Bumpleton ✿";
        
        public override bool IsLoadingEnabled(Mod mod)
        {
	        return ModContent.GetInstance<EnchantmentToggle>().MobilityControl;
        }
    }
    [Autoload(false)]
	public class MobilityControlEnchantmentBasic : MobilityControlEnchantment
    {
        public override SellCondition SellCondition => SellCondition.AnyTimeRare;
        public override List<DropData> NpcDropTypes => new() {
            new(NPCID.Harpy, chance: 0.02f),
            new(NPCID.WyvernHead, chance: 0.25f)
		};
		public override List<DropData> ChestDrops => new() {
            new(ChestID.Skyware)
		};
        public override List<DropData> CrateDrops => new() {
            new(CrateID.Sky, 0.5f),
            new(CrateID.Azure_SkyHard, 0.5f)
        };
    }
    [Autoload(false)]
	public class MobilityControlEnchantmentCommon : MobilityControlEnchantment { }
    [Autoload(false)]
	public class MobilityControlEnchantmentRare : MobilityControlEnchantment { }
    [Autoload(false)]
	public class MobilityControlEnchantmentEpic : MobilityControlEnchantment { }
    [Autoload(false)]
	public class MobilityControlEnchantmentLegendary : MobilityControlEnchantment { }
	[Autoload(false)]
	public class MobilityControlEnchantmentCursed : MobilityControlEnchantment { }
}
