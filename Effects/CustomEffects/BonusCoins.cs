using KokoLib;
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
using WeaponEnchantments.ModLib.KokoLib;
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


			int whoAmI;
			NPC sample;
			if (npc.realLife >= 0) {
				whoAmI = npc.realLife;
				sample = ContentSamples.NpcsByNetId[Main.npc[whoAmI].netID];
			}
			else {
				whoAmI = npc.whoAmI;
				sample = ContentSamples.NpcsByNetId[npc.netID];
			}

			NPC thisNPC = Main.npc[whoAmI];

			float value = (float)damageInt / (float)thisNPC.lifeMax * sample.value;
			if (value < damageInt)
				value = (float)damageInt;

			value *= 1f + wePlayer.Player.luck;

			int coins = (int)Math.Round(EStatModifier.ApplyTo(0f) * value);
			if (coins <= 0)
				coins = 1;

			Net<INetOnHitEffects>.Proxy.NetAddNPCValue(thisNPC, coins);
		}

		public override string Tooltip => $"{EStatModifier.PercentMult100Tooltip} {DisplayName} (Hitting an enemy will increase the number of coins it will drop on death based on damage dealt, enemy max health, enemy base value, and luck.)";
		public override EnchantmentStat statName => EnchantmentStat.None;
	}
}
