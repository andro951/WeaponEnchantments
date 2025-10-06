using androLib;
using androLib.Common.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Common.Globals
{
	public class WEGlobalItem : GlobalItem
	{
		public override void GrabRange(Item item, Player player, ref int grabRange) {
			if (player?.TryGetModPlayer(out WEPlayer wePlayer) == true) {
				if (wePlayer.CheckEnchantmentStats(EnchantmentStat.PickupRange, out float add))
					grabRange = (int)Math.Round((float)grabRange + add);
			}
		}
		private static Vector2 GetTargetVelocity(Vector2 targetCenter, Player player) {
			if (!player.TryGetModPlayer(out StoragePlayer storagePlayer))
				return player.velocity;

			if (targetCenter != storagePlayer.CenterBeforeMoveUpdate)
				return player.velocity;

			return Vector2.Zero;
		}
		private static float GetVelocityLength(Item item, Player player, float mult) {
			float itemVelocityLength = item.velocity.Length();
			float playerVelocityLength = player.velocity.Length();
			float result = Math.Max(itemVelocityLength, playerVelocityLength * 1.1f) * (mult - 1f);

			return result;
		}
		public override bool GrabStyle(Item item, Player player) {
			if (player?.TryGetModPlayer(out WEPlayer wePlayer) == true && item.playerIndexTheItemIsReservedFor == player.whoAmI) {
				if (wePlayer.CheckEnchantmentStats(EnchantmentStat.ItemAttractionAndPickupSpeed, out float mult, 1f)) {
					mult -= 1f;
					float itemVelocityLength = item.velocity.Length() * mult;
					Vector2 itemCenter = item.Center;
					Vector2 vectorToPlayer = player.Center - itemCenter;
					float distanceToPlayer = vectorToPlayer.Length();
					if (distanceToPlayer <= itemVelocityLength) {
						item.position += vectorToPlayer;
					}
					else {
						float ms = 30f;
						Vector2 target = player.position + player.velocity * ms;
						Vector2 vectorToTarget = target - itemCenter;
						float distanceToTarget = vectorToTarget.Length();
						float itemVelocityPerTime = itemVelocityLength * ms;
						if (distanceToTarget <= itemVelocityPerTime) {
							item.position += vectorToPlayer.SafeNormalize(Vector2.Zero) * itemVelocityLength;
						}
						else {
							item.position += vectorToTarget.SafeNormalize(Vector2.Zero) * itemVelocityLength;
						}
					}

					return false;

					//VectorMath.CalculateHomingVelocity(
					//	() => player.Center,
					//	(Vector2 targetCenter) => GetTargetVelocity(targetCenter, player),
					//	() => GetVelocityLength(item, player, mult),//Make sure it's base speed is at least 10% faster than the player.
					//	() => item.Center,
					//	(Vector2 velocity) => { item.position += velocity; return; },
					//	(Vector2 center) => { item.Center = center; return; }
					//);
				}

				return false;
			}

			return false;
		}
	}
}
