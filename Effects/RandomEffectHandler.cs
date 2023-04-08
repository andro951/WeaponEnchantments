using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponEnchantments.Effects
{
	public static class RandomEffectHandler
	{
		public static List<EnchantmentEffect> GetRandomEffects(List<List<EnchantmentEffect>> possibleEffects, float chance = 1f, int minEffects = 1, int maxEffects = -1) {
			if (maxEffects == -1)
				maxEffects = possibleEffects.Count;

			List<EnchantmentEffect> effects = new();
			MoveOneRandomToResultEffects(ref possibleEffects, ref effects);
			if (maxEffects > 1 && possibleEffects.Count > 0) {
				if (chance >= 1f) {
					while (maxEffects < effects.Count && possibleEffects.Count > 0) {
						MoveOneRandomToResultEffects(ref possibleEffects, ref effects);
					}
				}
				else {
					do {
						for (int i = 0; i < possibleEffects.Count; i++) {
							if (Main.rand.NextFloat() <= chance) {
								MoveToResultEffects(ref possibleEffects, ref effects, i);
								i--;
							}
						}
					} while (effects.Count < minEffects);
				}
			}

			return effects;
		}
		private static void MoveOneRandomToResultEffects(ref List<List<EnchantmentEffect>> possibleEffects, ref List<EnchantmentEffect> result) {
			int effectIndex = Main.rand.Next(possibleEffects.Count);
			MoveToResultEffects(ref possibleEffects, ref result, effectIndex);
		}
		private static void MoveToResultEffects(ref List<List<EnchantmentEffect>> possibleEffects, ref List<EnchantmentEffect> result, int effectIndex) {
			AddToResultEffects(ref possibleEffects, ref result, effectIndex);
			possibleEffects.RemoveAt(effectIndex);
		}
		private static void AddToResultEffects(ref List<List<EnchantmentEffect>> possibleEffects, ref List<EnchantmentEffect> result, int effectIndex) {
			result.Add(GetRomdonEffect(possibleEffects[effectIndex]));
		}

		public static EnchantmentEffect GetRomdonEffect(List<EnchantmentEffect> possibleEffects) => possibleEffects[Main.rand.Next(possibleEffects.Count)];
	}
}
