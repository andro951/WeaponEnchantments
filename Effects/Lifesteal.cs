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

namespace WeaponEnchantments.EnchantmentEffects {
    public class LifeSteal : EnchantmentEffect {
        protected override Dictionary<DamageClass, float> EnchantmentDamageEfficiency => new Dictionary<DamageClass, float>() {
            { DamageClass.Melee, 1f },
            { DamageClass.Magic, 0.5f}
        };


        public LifeSteal(float power) : base(power) { }



    public override void OnAfterHit(NPC npc, WEPlayer wePlayer, Item item, ref int damage, ref float knockback, ref bool crit, Projectile projectile = null) {
            float efficiency;
            if (!EnchantmentDamageEfficiency.TryGetValue(item.DamageType, out efficiency))
                return;

            Player player = wePlayer.Player;

            float lifeSteal = EnchantmentPower;
            // TODO: Make stack with one for all
            float healTotal = (damage) * lifeSteal * (player.moonLeech ? 0.5f : 1f) + wePlayer.lifeStealRollover;

            //Summon damage reduction
            healTotal *= efficiency;

            int heal = (int)healTotal;

            Main.NewText($"Trying to heal {heal}");


            if (player.statLife < player.statLifeMax2) {
                Main.NewText($"I can actually heal");

                //Player hp less than max
                if (heal > 0 && player.lifeSteal > 0f) {
                    Main.NewText($"WE lifesteal is OK");
                    //Vanilla lifesteal mitigation
                    int vanillaLifeStealValue = (int)Math.Round(heal * ConfigValues.AffectOnVanillaLifeStealLimit);
                    player.lifeSteal -= vanillaLifeStealValue;

                    Projectile.NewProjectile(item.GetSource_ItemUse(item), npc.Center, new Vector2(0, 0), ProjectileID.VampireHeal, 0, 0f, player.whoAmI, player.whoAmI, heal);
                    Main.NewText($"Projectile where?");

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
