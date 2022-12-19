using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Effects.CustomEffects;
using WeaponEnchantments.ModIntegration;

namespace WeaponEnchantments.Items.Enchantments
{
    public abstract class KiEnchantment : Enchantment
    {

        public override int StrengthGroup => 19;
        public override bool Max1 => true;
        public override void GetMyStats()
        {
            AllowedList = new Dictionary<EItemType, float>()
            {
                { EItemType.Armor, 1f }
            };
        }
        private string GetTooltip()
        {
            if (Effects.Count > 0)
            {
                var sb = new StringBuilder();
                foreach(var e in Effects)
                {
                    var ef = e as StatEffect;
                    if (ef != null)
                    {
                        sb.AppendLine(e.GetType().Name.Lang(L_ID1.Tooltip, L_ID2.EnchantmentEffects, new string[] { ef.TooltipValue }));
                    }
                }
                return sb.ToString().Trim();
            }
            else return "";
        }
        public override string ShortTooltip => GetTooltip();
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
        //public override string ShortTooltip => $"+{100} Max Ki";
        public override void GetMyStats()
        {
            base.GetMyStats();

            Effects = new()
            {
                new MaxKi(new EStatModifier(EnchantmentStat.MaxKi, flat: 100f))
            };
        }

    }
    public class KiEnchantmentCommon : KiEnchantment
    {
        //public override string ShortTooltip => $"+{150} Max Ki\n+{20} Ki Regen";
        public override void GetMyStats()
        {
            base.GetMyStats();

            Effects = new()
            {
                new MaxKi(new EStatModifier(EnchantmentStat.MaxKi, flat: 150f)),
                new KiRegen(new EStatModifier(EnchantmentStat.KiRegen, flat: 1f))
            };

        }

    }
    public class KiEnchantmentRare : KiEnchantment
    {
        //public override string ShortTooltip => $"+{200} Max Ki\n+{20} Ki Regen";
        public override void GetMyStats()
        {
            base.GetMyStats();

            Effects = new()
            {
                new MaxKi(new EStatModifier(EnchantmentStat.MaxKi, flat: 200f)),
                new KiRegen(new EStatModifier(EnchantmentStat.KiRegen, flat: 1f))
            };

        }
 
    }
    public class KiEnchantmentEpic : KiEnchantment
    {
        //public override string ShortTooltip => $"+{250} Max Ki\n+{40} Ki Regen";
        public override void GetMyStats()
        {
            base.GetMyStats();

            Effects = new()
            {
                new MaxKi(new EStatModifier(EnchantmentStat.MaxKi, flat: 250f)),
                new KiRegen(new EStatModifier(EnchantmentStat.KiRegen, flat: 2f))
            };

        }
    }
    public class KiEnchantmentLegendary : KiEnchantment
    {
        //public override string ShortTooltip => $"+{300} Max Ki\n+{40} Ki Regen";
        public override void GetMyStats()
        {
            base.GetMyStats();

            Effects = new()
            {
                new MaxKi(new EStatModifier(EnchantmentStat.MaxKi, flat: 300f)),
                new KiRegen(new EStatModifier(EnchantmentStat.KiRegen, flat: 2f))
            };

        }
    }
}
