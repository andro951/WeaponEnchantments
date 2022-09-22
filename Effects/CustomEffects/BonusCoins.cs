using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects
{
	public class BonusCoins : StatEffect, INonVanillaStat, IOnHitEffect
    {
        public BonusCoins(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public BonusCoins(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone() {
            return new BonusCoins(EStatModifier.Clone());
        }

		public void OnHitNPC(NPC npc, WEPlayer wePlayer, Item item, int damage, float knockback, bool crit, Projectile projectile = null) {
			if (npc.friendly || npc.townNPC || npc.SpawnedFromStatue || npc.type == NPCID.TargetDummy)
				return;

			int damageInt = damage;
			if (crit)
				damageInt *= 2;

			if (npc.life < 0)
				damageInt += npc.life;

			if (damageInt > npc.lifeMax)
				damageInt = npc.lifeMax;

			if (damageInt <= 0)
				return;

			float value = (float)damageInt / (float)npc.lifeMax * npc.value;
			if (value < damageInt)
				value = (float)damageInt;

			float vanillaMultiplier = GetVanillaCoinMultiplier(npc, wePlayer.Player, out float extraValue);
			value *= vanillaMultiplier;
			value += extraValue;

			int coins = (int)Math.Round(EStatModifier.ApplyTo(0f) * value);
			if (coins <= 0)
				coins = 1;

			wePlayer.Player.SpawnCoins(coins);

		}
		private static float GetVanillaCoinMultiplier(NPC npc, Player player, out float extraValue) {
			float num = 0f;
			float luck = player.luck;
			int num2 = 1;
			if (Main.rand.NextFloat() < Math.Abs(luck))
				num2 = 2;

			for (int i = 0; i < num2; i++) {
				float num3 = 1f;
				if (npc.midas)
					num3 *= 1f + (float)Main.rand.Next(10, 51) * 0.01f;

				num3 *= 1f + (float)Main.rand.Next(-20, 76) * 0.01f;
				if (Main.rand.Next(2) == 0)
					num3 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;

				if (Main.rand.Next(4) == 0)
					num3 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;

				if (Main.rand.Next(8) == 0)
					num3 *= 1f + (float)Main.rand.Next(15, 31) * 0.01f;

				if (Main.rand.Next(16) == 0)
					num3 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;

				if (Main.rand.Next(32) == 0)
					num3 *= 1f + (float)Main.rand.Next(25, 51) * 0.01f;

				if (Main.rand.Next(64) == 0)
					num3 *= 1f + (float)Main.rand.Next(50, 101) * 0.01f;

				if (Main.bloodMoon)
					num3 *= 1f + (float)Main.rand.Next(101) * 0.01f;

				if (i == 0) {
					num = num3;
				}
				else if (luck < 0f) {
					if (num3 < num)
						num = num3;
				}
				else if (num3 > num) {
					num = num3;
				}
			}

			extraValue = (float)npc.extraValue;

			return num;
		}

		public override string Tooltip => $"{EStatModifier.PercentMult100Tooltip} {DisplayName} (Hitting an enemy will give you coins based on damage dealt, enemy max health, enemy value, and luck.)";
		public override EnchantmentStat statName => EnchantmentStat.None;
	}
}
