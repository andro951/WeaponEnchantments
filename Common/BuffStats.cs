using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.Utility.WEMath;

namespace WeaponEnchantments.Common
{
	public class BuffStats
	{
		public string BuffName { get; private set; }
		public short BuffID { get; private set; }
		public int Duration {
			get => (int)_duration;
			private set => _duration = (float)value;
		}
		private float _duration;
		public float Chance {
			get => _chance;
			private set {
				if (value > 1f) {
					_chance = 1f;
					 _duration.MultiplyCheckOverflow(value);
				}
				else {
					_chance = value;
				}

				weightedDuration = MultiplyCheckOverflow(_chance, _duration);
			}
		}
		private float _chance = 1f;
		private float weightedDuration = 0f;

		public BuffStats(string buffName, short buffID, Time duration, float chance) {
			BuffName = buffName;
			BuffID = buffID;
			_duration = duration.Value;
			Chance = chance;
		}
		public BuffStats(string buffName, short buffID, int duration, float chance) {
			BuffName = buffName;
			BuffID = buffID;
			_duration = duration;
			Chance = chance;
		}
		public void CombineNoReturn(BuffStats b) {
			float bWeightedDuration = (float)b.Duration * b.Chance;
			float combinedWeightedDuration = bWeightedDuration + weightedDuration;
			float newChance = combinedWeightedDuration / _duration;
			Chance = newChance;
		}
		public BuffStats Clone() => new BuffStats(BuffName, BuffID, Duration, Chance);
		public void AddBuff(Player player) {
			if (Chance == 1f || Chance >= Main.rand.NextFloat())
				player.AddBuff(BuffID, Duration);
		}
	}
}
