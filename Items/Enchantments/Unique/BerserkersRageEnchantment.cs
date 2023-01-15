using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
    public abstract class BerserkersRageEnchantment : Enchantment
    {
		public override void GetMyStats()
        {
            Effects = new() {
                new AttackSpeed(EnchantmentStrengthData * 2.5f),
                new MiningSpeed(EnchantmentStrengthData * 3.75f),
                new AutoReuse(),
                new NPCHitCooldown(EnchantmentStrengthData * -2.5f),
                new AmmoCost(@base: EnchantmentStrengthData * 1.25f),
                new DamageAfterDefenses(multiplicative: EnchantmentStrengthData * -1.25f + 1f)
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.Weapons, 1f },
                { EItemType.FishingPoles, 1f}
            };
        }

        public override string ShortTooltip => GetShortTooltip(sign: true);
        public override string Artist => "Zorutan";
        public override string ArtModifiedBy => "andro951";
        public override string Designer => "Jangiot";
    }
    public class BerserkersRageEnchantmentBasic : BerserkersRageEnchantment
	{
        public override SellCondition SellCondition => SellCondition.PostSkeletron;
        public override List<WeightedPair> NpcDropTypes => new() {
            new(NPCID.SkeletronHead, 0.25f)
        };
        public override List<WeightedPair> CrateDrops => new() {
            new(CrateID.Dungeon, 0.5f),
            new(CrateID.Stockade_DungeonHard, 0.5f)
        };
    }
    public class BerserkersRageEnchantmentCommon : BerserkersRageEnchantment { }
    public class BerserkersRageEnchantmentRare : BerserkersRageEnchantment { }
    public class BerserkersRageEnchantmentEpic : BerserkersRageEnchantment { }
    public class BerserkersRageEnchantmentLegendary : BerserkersRageEnchantment { }
}
