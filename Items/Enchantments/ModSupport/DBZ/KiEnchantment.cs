using System.Collections.Generic;
using androLib.Common.Utility;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects.CustomEffects;
using WeaponEnchantments.ModIntegration;

namespace WeaponEnchantments.Items.Enchantments.ModSupport.DBZ
{
    public abstract class KiEnchantment : Enchantment
    {
        protected override string TypeName => "DBZKi";
        protected override string NamePrefix => "ModSupport/";

        public override int StrengthGroup => 24;
        public override bool Max1 => true;
        public override void GetMyStats()
        {
            AllowedList = new Dictionary<EItemType, float>()
            {
                { EItemType.Armor, 1f }
            };

            Effects = new()
            {
                new MaxKi(@base: EnchantmentStrengthData),
                new KiRegen(@base: EnchantmentStrengthData / 100f)
            };
        }
        public override string ShortTooltip => GetShortTooltip(sign: true, percent: false, multiply100: false);
        public override string CustomTooltip => DBZMODPORTIntegration.DBT_NAME.Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
        public override string Artist => "Vyklade";
        public override string ArtModifiedBy => null;
        public override string Designer => "Vyklade";
        
        
        public override bool IsLoadingEnabled(Mod mod)
        {
            return ModContent.GetInstance<EnchantmentToggle>().DBZKi && WEMod.dbtEnabled;
        }
    }

    [Autoload(false)]
	public class KiEnchantmentBasic : KiEnchantment
    {
        public override SellCondition SellCondition => WEMod.dbtEnabled ? SellCondition.HardMode : SellCondition.Never;
		public override List<DropData> NpcDropTypes => WEMod.dbtEnabled ? new() {
            new(NPCID.Lihzahrd, chance : 0.05f),
            new(NPCID.LihzahrdCrawler, chance : 0.05f)
        } : null;
    }
    [Autoload(false)]
	public class KiEnchantmentCommon : KiEnchantment { }
    [Autoload(false)]
	public class KiEnchantmentRare : KiEnchantment { }
    [Autoload(false)]
	public class KiEnchantmentEpic : KiEnchantment { }
    [Autoload(false)]
	public class KiEnchantmentLegendary : KiEnchantment { }
	[Autoload(false)]
	public class KiEnchantmentCursed : KiEnchantment { }
}
