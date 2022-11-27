using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects.CustomEffects;
using WeaponEnchantments.ModIntegration;

namespace WeaponEnchantments.Items.Enchantments
{
    public abstract class KiEnchantment : Enchantment
    {
        public override bool Max1 => true;
        public override void GetMyStats()
        {


            AllowedList = new Dictionary<EItemType, float>()
            {
                { EItemType.Armor, 1f }
            };
        }
        public override string CustomTooltip => DBZMODPORTIntegration.DBTName.Lang(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
        public override string Artist => "Vyklade";
        public override string ArtModifiedBy => null;
        public override string Designer => "Vyklade";
    }

    public class KiEnchantmentBasic : KiEnchantment
    {
        public override SellCondition SellCondition => SellCondition.HardMode;
        public override List<WeightedPair> NpcDropTypes => new()
        {
            new(NPCID.Lihzahrd),
            new(NPCID.LihzahrdCrawler)
        };
        public override void GetMyStats()
        {
            base.GetMyStats();

            Effects = new()
            {
                new MaxKi(new EStatModifier(EnchantmentStat.MaxKi, @base: 100f))
            };
        }

    }
    public class KiEnchantmentCommon : KiEnchantment
    {
        public override void GetMyStats()
        {
            base.GetMyStats();

            Effects = new()
            {
                new MaxKi(new EStatModifier(EnchantmentStat.MaxKi, @base: 150f)),
                new KiRegen(1f)
            };

        }

    }
    public class KiEnchantmentRare : KiEnchantment
    {
        public override void GetMyStats()
        {
            base.GetMyStats();

            Effects = new()
            {
                new MaxKi(new EStatModifier(EnchantmentStat.MaxKi, @base: 200f)),
                new KiRegen(1f)
            };

        }
 
    }
    public class KiEnchantmentEpic : KiEnchantment
    {
        public override void GetMyStats()
        {
            base.GetMyStats();

            Effects = new()
            {
                new MaxKi(new EStatModifier(EnchantmentStat.MaxKi, @base: 250f)),
                new KiRegen(2f)
            };

        }
    }
    public class KiEnchantmentLegendary : KiEnchantment
    {
        public override void GetMyStats()
        {
            base.GetMyStats();

            Effects = new()
            {
                new MaxKi(new EStatModifier(EnchantmentStat.MaxKi, @base: 300f)),
                new KiRegen(2f)
            };

        }
    }
}
