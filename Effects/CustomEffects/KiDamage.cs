using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects.CustomEffects
{
    public class KiDamage : BoolEffect, IPassiveEffect
    {
        public KiDamage(bool prevent = false) : base(prevent) { }
        public override EnchantmentEffect Clone() {
            return new KiDamage(!EnableStat);
        }

		public void PostUpdateMiscEffects(WEPlayer wePlayer) {
            if (WEMod.dbtEnabled) {
                var dbzmod = ModLoader.GetMod("DBZMODPORT");
                var DbtPlayerClass = dbzmod.Code.DefinedTypes.First(a => a.Name.Equals("MyPlayer"));
                var DbtPlayer = DbtPlayerClass.GetMethod("ModPlayer").Invoke(null, new object[] { wePlayer.Player });
                var KiDamage = (float)DbtPlayerClass.GetField("KiDamage").GetValue(DbtPlayer);
                wePlayer.Player.GetDamage(DamageClass.Default) *= KiDamage;
            }
        }

		public override EnchantmentStat statName => EnchantmentStat.KiDamage;
	}
}