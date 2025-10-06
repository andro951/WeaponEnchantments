using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria.ID;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using androLib.Items;
using androLib.Common.Utility;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class BerserkersRageEnchantment : Enchantment
    {
	    protected override string TypeName => "BerserkersRage";
	    protected override string NamePrefix => "Enchantments/";
	    
		public override int StrengthGroup => 21;
		public override void GetMyStats()
        {
            Effects = new() {
                new AttackSpeed(EnchantmentStrengthData),
                new MiningSpeed(EnchantmentStrengthData),
                new AutoReuse(),
                new NPCHitCooldown(EnchantmentStrengthData * -1f),
                new AmmoCost(@base: EnchantmentStrengthData * 0.3f),
				new ManaUsage(@base: EnchantmentStrengthData * -0.3f),
				new DamageAfterDefenses(multiplicative: (EnchantmentStrengthData * -0.25f + 1f).Min(0.5f))
            };

            AllowedList = new Dictionary<EItemType, float>() {
                { EItemType.Weapons, 1f },
                { EItemType.FishingPoles, 1f},
                { EItemType.Tools, 1f }
            };
        }

        public override string ShortTooltip => GetShortTooltip(sign: true);
        public override string Artist => "Zorutan";
        public override string ArtModifiedBy => "andro951";
        public override string Designer => "Jangiot";
        
        public override bool IsLoadingEnabled(Mod mod)
        {
	        return ModContent.GetInstance<EnchantmentToggle>().BerserkersRage;
        }
	}
	[Autoload(false)]
	public class BerserkersRageEnchantmentBasic : BerserkersRageEnchantment
	{
        public override SellCondition SellCondition => SellCondition.PostSkeletron;
        public override List<DropData> NpcDropTypes => new() {
            new(NPCID.SkeletronHead, chance: 0.2f)
        };
        public override List<DropData> CrateDrops => new() {
            new(CrateID.Dungeon, 0.5f),
            new(CrateID.Stockade_DungeonHard, 0.5f)
        };
	}
	[Autoload(false)]
	public class BerserkersRageEnchantmentCommon : BerserkersRageEnchantment { }
	[Autoload(false)]
	public class BerserkersRageEnchantmentRare : BerserkersRageEnchantment { }
	[Autoload(false)]
	public class BerserkersRageEnchantmentEpic : BerserkersRageEnchantment { }
	[Autoload(false)]
	public class BerserkersRageEnchantmentLegendary : BerserkersRageEnchantment { }
	[Autoload(false)]
	public class BerserkersRageEnchantmentCursed : BerserkersRageEnchantment { }
}
