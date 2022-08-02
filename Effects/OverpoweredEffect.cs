using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WeaponEnchantments.Items.Enchantments.EnchantmentEffects {
    public class OverpoweredEffect : EnchantmentEffect {
        public OverpoweredEffect(float enchantmentPower) : base(enchantmentPower) { }

        public override void PostUpdateMiscEffects(WEPlayer player) {
            player.Player.GetDamage(DamageClass.Generic) += enchantmentPower;
        }
    }
}
