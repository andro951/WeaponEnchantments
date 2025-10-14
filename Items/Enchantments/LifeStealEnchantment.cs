using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using androLib.Items;
using androLib.Common.Utility;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments {
    public abstract class LifeStealEnchantment : Enchantment
    {
	    protected override string TypeName => "LifeSteal";
	    protected override string NamePrefix => "Enchantments/";
	    
        public override float ScalePercent => 0.8f;
        public override bool Max1 => true;
        public override float CapacityCostMultiplier => CapacityCostNormal;
		public override int StrengthGroup => 25;
        public override void GetMyStats() {
            Effects = new() {
                new LifeSteal(@base: EnchantmentStrengthData),
                new MaxLifeSteal(@base: EnchantmentStrengthData.Invert() / 100f)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.Weapons, 1f }
            };
        }
		public override string Artist => "Zorutan";
        public override string ArtModifiedBy => null;
        public override string Designer => "andro951";
        public override string WikiDescription => 
            $"Life Steal from an enchantment is not exactly the same as vanilla lifesteal.  Vanilla lifesteal calculates the " +
			$"amount of healing you will receive and truncates any decimal.  Lifesteal from an enchantment stores the decimal and " +
			$"adds it to your heal amount the next time you use lifesteal.  (Note: This remainder is reset to zero if you reach " +
			$"full health)  This makes lifesteal from enchantments valuable on any weapon because you can heal some regardless of " +
			$"the damage per hit.  Lifesteal from enchantments utilizes the same lifesteal limiting as vanilla does.  This " +
			$"limit is very unlikely to be reached unless you are using a weapon at least as powerful as the Zenith.  At that point, " +
			$"you may reach the limit.  If you do exceed the limit, you will gain zero life from lifesteal util it has recovered.  " +
			$"The amount the limit value is affected can be adjusted in the config.  (Note: the config option does not affect any " +
			$"source of lifesteal besides lifesteal from enchantments.)  Life steal gained from minions is reduced by half. (I may " +
			$"ban it or reduce it more on minions at some point in the future.  Haven't decided yet.)  Vanilla lifesteal allows you " +
			$"heal from lifesteal when already at full health (wasting the life steal pool for no reason).  Enchantments do not do this.  " +
			$"Additionally, life steal from enchantments will not over heal you past full health which would also waste the pool.  " +
			$"The moon lord's Moon Leach debuff normally prevents all lifesteal.  I personally don't like mechanics that completely " +
			$"turn off effects like this, so life steal from enchantments is reduced by 50% from this debuff instead.";
        
        public override bool IsLoadingEnabled(Mod mod)
        {
	        return ModContent.GetInstance<EnchantmentToggle>().LifeSteal;
        }
    }
    [Autoload(false)]
	public class LifeStealEnchantmentBasic : LifeStealEnchantment
    {
        public override SellCondition SellCondition => SellCondition.PostEaterOfWorldsOrBrainOfCthulhu;
        public override List<DropData> NpcDropTypes => new() {
            new(NPCID.WallofFlesh, 2f),
            new(NPCID.EaterofWorldsHead, chance: 0.2f)
        };
        public override List<DropData> NpcAIDrops => new() {
            new(NPCAIStyleID.TheHungry),
            new(NPCAIStyleID.Creeper)
        };
        public override List<DropData> ChestDrops => new() {
            new(ChestID.Shadow,  chance: 0.2f),
            new(ChestID.Shadow_Locked,  chance: 0.2f)
        };
        public override List<DropData> CrateDrops => new() {
			new(CrateID.Obsidian_LockBox, chance: 0.1f),
			new(CrateID.Crimson, 0.5f),
            new(CrateID.Hematic_CrimsonHard, 0.5f)
        };
    }
    [Autoload(false)]
	public class LifeStealEnchantmentCommon : LifeStealEnchantment { }
    [Autoload(false)]
	public class LifeStealEnchantmentRare : LifeStealEnchantment { }
    [Autoload(false)]
	public class LifeStealEnchantmentEpic : LifeStealEnchantment { }
    [Autoload(false)]
	public class LifeStealEnchantmentLegendary : LifeStealEnchantment { }
	[Autoload(false)]
	public class LifeStealEnchantmentCursed : LifeStealEnchantment { }

}
