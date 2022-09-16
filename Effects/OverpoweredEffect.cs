using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    public class OverpoweredEffect : EnchantmentEffect, IPassiveEffect {
        public OverpoweredEffect(float EnchantmentStrength) : base(EnchantmentStrength) { }
		public override EnchantmentEffect Clone() {
			return new OverpoweredEffect(EffectStrength);
		}

		public override string DisplayName { get; } = "OP Effect";

        public void PostUpdateMiscEffects(WEPlayer player) {
            player.Player.GetDamage(DamageClass.Generic) += EffectStrength;
        }
    }
}
