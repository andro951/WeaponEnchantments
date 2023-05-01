﻿using KokoLib;
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

			NPC realNPC = npc.RealNPC();
			int damageInt = damage;
			if (crit)
				damageInt *= 2;

			int life = realNPC.life;
			if (life < 0)
				damageInt += life;

			int lifeMax = realNPC.lifeMax;
			if (damageInt > lifeMax)
				damageInt = lifeMax;

			if (damageInt <= 0)
				return;

			float npcValue = realNPC.type.CSNPC().value;
			float value = (float)damageInt / (float)life * npcValue;
			if (value < damageInt)
				value = (float)damageInt;

			value *= 1f + Math.Min(wePlayer.Player.luck, 2f);

			int coins = (int)Math.Round(EStatModifier.ApplyTo(0f) * value);
			if (coins <= 0)
				coins = 1;

			Net<INetMethods>.Proxy.NetAddNPCValue(realNPC, coins);
		}

		public override IEnumerable<object> TooltipArgs => new object[] { base.Tooltip };
		public override string Tooltip => StandardTooltip;
		public override string TooltipValue => EStatModifier.PercentMult100Tooltip;
		public override EnchantmentStat statName => EnchantmentStat.None;
	}
}
