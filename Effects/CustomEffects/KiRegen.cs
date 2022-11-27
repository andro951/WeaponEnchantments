using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects.CustomEffects
{
    public class KiRegen : PlayerSetEffect
    {
        public KiRegen(float minimumStrength, DifficultyStrength timerData = null, bool prevent = false) : base(minimumStrength, timerData, prevent)
        {
            
        }
        public override EnchantmentEffect Clone()
        {
            return new KiRegen(MinimumStrength, StrengthData.Clone(), !EnableStat);
        }

        public override EnchantmentStat statName => EnchantmentStat.KiRegen;
        public override string TooltipValue => Math.Ceiling((MinimumStrength * 60) / 3).ToString();
        public override void SetEffect(Player player)
        {
            if (WEMod.dbtEnabled)
            {
                var dbzmod = ModLoader.GetMod("DBZMODPORT");
                var DbtPlayerClass = dbzmod.Code.DefinedTypes.First(a => a.Name.Equals("MyPlayer"));
                var DbtPlayer = DbtPlayerClass.GetMethod("ModPlayer").Invoke(null, new object[] { player });
                var KiRegen = (int)DbtPlayerClass.GetField("kiRegen").GetValue(DbtPlayer);
                DbtPlayerClass.GetField("kiRegen").SetValue(DbtPlayer, (int)KiRegen + (int)MinimumStrength);
                
            }
        }

    }
}
