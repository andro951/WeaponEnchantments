using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common.Globals
{
	public class BobberProjectile : ProjectileWithSourceItem {
		float localAI1ResetValue = 660f;
		float localAI1Carryover = 0f;
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
			if (!base.AppliesToEntity(entity, lateInstantiation))
				return false;

			return entity.aiStyle == ProjAIStyleID.Bobber;
		}

		public override bool UpdateProjectile(Projectile projectile) {
			if (!base.UpdateProjectile(projectile))
				return false;

			if (GetSharedVanillaModifierStrength(projectile.owner, EnchantmentStat.AttackSpeed, out float speedMultiplier))
				speed = 1f - 1f / speedMultiplier;

			return true;
		}

		public override bool ShouldUpdatePosition(Projectile projectile) {
			if (!base.ShouldUpdatePosition(projectile))
				return true;

			float ai0 = projectile.ai[0];
			if (ai0 == 1f)
				return true;

			float localAI1 = projectile.localAI[1];
			if (localAI1 == 0f) {
				float speedAddValue = speed * localAI1ResetValue + localAI1Carryover;
				localAI1Carryover = speedAddValue % 1f;
				int valueToAdd = (int)speedAddValue;
				projectile.localAI[1] += valueToAdd;
			}

			return true;
		}

	}
}
