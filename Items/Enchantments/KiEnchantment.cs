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

            Effects = new()
            {
                new MaxKi(@base: (EnchantmentStrengthData + 1) * 50f),
                new KiRegen(@base: EnchantmentStrengthData / 2)
            };
        }
        public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
        public override string CustomTooltip => DBZMODPORTIntegration.DBT_NAME.Lang(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
        public override string Artist => "Vyklade";
        public override string ArtModifiedBy => null;
        public override string Designer => "Vyklade";
    }

    public class KiEnchantmentBasic : KiEnchantment
    {
        public override SellCondition SellCondition => WEMod.dbtEnabled ? SellCondition.HardMode : SellCondition.Never;
		public override List<DropData> NpcDropTypes => WEMod.dbtEnabled ? new() {
            new(NPCID.Lihzahrd),
            new(NPCID.LihzahrdCrawler)
        } : null;
    }
    public class KiEnchantmentCommon : KiEnchantment { }
    public class KiEnchantmentRare : KiEnchantment { }
    public class KiEnchantmentEpic : KiEnchantment { }
    public class KiEnchantmentLegendary : KiEnchantment { }
}
