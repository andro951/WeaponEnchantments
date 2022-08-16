/*using Microsoft.Xna.Framework;
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
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects {
    public class DebuffEffect : EnchantmentEffect, IOnHitEffect {
        public static string GetBuffName(int id) {
            if (id < BuffID.Count) {
                BuffID buffID = new();
                return buffID.GetType().GetFields().Where(field => field.FieldType == typeof(int) && (int)field.GetValue(buffID) == id).First().Name;
            }
            return ModContent.GetModBuff(id).Name;
        }

        public DebuffEffect(int debuffID, Time debuffTime, float applicationChance = 1f, bool isQuiet = false) : base(applicationChance) {
            AppliedDebuffID = debuffID;
            BaseApplicationTime = debuffTime;
            IsQuiet = isQuiet;
        }

        private int AppliedDebuffID { get; set; }
        private Time BaseApplicationTime { get; set; }
        private bool IsQuiet { get; set; }

        // Apply Equipment efficiency (Doing it here allows tooltips to update)
        private float applyChance => EnchantmentPower * EfficiencyMultiplier;
        private Time applyTime => BaseApplicationTime * EfficiencyMultiplier;

        public override sealed string DisplayName => $"On-Hit {GetBuffName(AppliedDebuffID)}";
        public override sealed string Tooltip => $"{DisplayName} ({applyChance.Percent()}% for {applyTime})";

        public void OnAfterHit(NPC npc, WEPlayer wePlayer, Item item, int damage, float knockback, bool crit, Projectile projectile = null) {
            // Can do: Apply damage type efficiency
            if (!npc.buffImmune[AppliedDebuffID] && Main.rand.NextFloat(0f, 1f) <= applyChance) {
                npc.AddBuff(AppliedDebuffID, applyTime, IsQuiet);
            }
        }
    }
}*/
