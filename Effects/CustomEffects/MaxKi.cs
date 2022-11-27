using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects.CustomEffects
{
    public class MaxKi : StatEffect, INonVanillaStat
    {
        public MaxKi(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base)
        {

        }
        public MaxKi(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentStat statName => EnchantmentStat.MaxKi;
        public override string TooltipValue => EStatModifier.Base.ToString();
        public override EnchantmentEffect Clone()
        {
            return new MaxKi(EStatModifier.Clone());
        }

        // Not used yet
        private void Apply(WEPlayer wePlayer)
        {
            if (WEMod.dbtEnabled)
            {
                var dbzmod = ModLoader.GetMod("DBZMODPORT");
                var DbtPlayerClass = dbzmod.Code.DefinedTypes.First(a => a.Name.Equals("MyPlayer"));
                var DbtPlayer = DbtPlayerClass.GetMethod("ModPlayer").Invoke(null, new object[] { wePlayer.Player });
                var MaxKi = (int)DbtPlayerClass.GetField("kiMax3").GetValue(DbtPlayer);
                DbtPlayerClass.GetField("kiMax3").SetValue(DbtPlayer, (int)EStatModifier.ApplyTo(MaxKi));

            }
        }
    }
}
