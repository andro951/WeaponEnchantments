using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.EnchantmentEffects;

namespace WeaponEnchantments.Effects {
    public class LifeSteal : EnchantmentEffect {
        public LifeSteal(float power) : base(power) { }
        protected override Dictionary<DamageClass, float> EnchantmentDamageEfficiency => new Dictionary<DamageClass, float>() {
            { DamageClass.Melee, 1f },
            { DamageClass.Magic, 0.5f}
        };
        public override string DisplayName => "Life Steal";
        public override void OnAfterHit(NPC npc, WEPlayer wePlayer, Item item, ref int damage, ref float knockback, ref bool crit, Projectile projectile = null) {
            float efficiency;
            if (!EnchantmentDamageEfficiency.TryGetValue(item.DamageType, out efficiency))
                return;

            Player player = wePlayer.Player;

            float lifeSteal = EnchantmentPower;
            // TODO: Make stack with one for all
            float healTotal = damage * lifeSteal * (player.moonLeech ? 0.5f : 1f) + wePlayer.lifeStealRollover;

            //Summon damage reduction
            healTotal *= efficiency;

            int heal = (int)healTotal;

            if (player.statLife < player.statLifeMax2) {

                //Player hp less than max
                if (heal > 0 && player.lifeSteal > 0f) {
                    //Vanilla lifesteal mitigation
                    int vanillaLifeStealValue = (int)Math.Round(heal * ConfigValues.AffectOnVanillaLifeStealLimit);
                    player.lifeSteal -= vanillaLifeStealValue;

                    Projectile.NewProjectile(item.GetSource_ItemUse(item), npc.Center, new Vector2(0, 0), ProjectileID.VampireHeal, 0, 0f, player.whoAmI, player.whoAmI, heal);

                }

                //Life Steal Rollover
                wePlayer.lifeStealRollover = healTotal - heal;
            }
            else {
                //Player hp is max
                wePlayer.lifeStealRollover = 0f;
            }
        }
    }
}
