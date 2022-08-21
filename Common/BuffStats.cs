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
		private string buffName;
		public string BuffName {
			get => buffName;
			private set {
				buffName = value.AddSpaces();
			}
		}
		public short BuffID { get; private set; }
		public Time Duration {
			get => _duration;
			private set => _duration = value;
		}
		private Time _duration;
		public float Chance {
			get {
				if (_waitingForEnterWorld)
					SetUpChanceDifficultyStrength();

				return _chance;
			}
			private set {
				if (value > 1f) {
					_chance = 1f;
					 _duration.Ticks.MultiplyCheckOverflow(value);
				}
				else {
					_chance = value;
				}

				weightedDuration = MultiplyCheckOverflow(_chance, _duration);
			}
		}
		private float _chance = 1f;
		private DifficultyStrength _chanceDifficultyStrengths;
		private bool _waitingForEnterWorld;
		private float weightedDuration = 0f;

		public bool DisableImmunity { get; protected set; }
		public float BuffStrength { 
			get {
				if (_waitingForEnterWorld)
					SetUpChanceDifficultyStrength();

				return buffStrength;
			}
			protected set => buffStrength = value;
		}
		private float buffStrength;
		private DifficultyStrength buffStrengths;

		public BuffStats(string buffName, short buffID, Time duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) {
			if (buffStrength != null)
				_waitingForEnterWorld = true;

			BuffName = buffName;
			BuffID = buffID;
			_duration = duration;
			Chance = chance;
			DisableImmunity = disableImmunity;
			buffStrengths = buffStrength;
		}
		public BuffStats(string buffName, short buffID, int duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) {
			if (buffStrength != null)
				_waitingForEnterWorld = true;

			BuffName = buffName;
			BuffID = buffID;
			_duration = new Time(duration);
			Chance = chance;
			DisableImmunity = disableImmunity;
			buffStrengths = buffStrength;
		}
		public BuffStats(string buffName, short buffID, Time duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) {
			_waitingForEnterWorld = true;
			BuffName = buffName;
			BuffID = buffID;
			_duration = duration;
			_chanceDifficultyStrengths = chance;
			DisableImmunity = disableImmunity;
			buffStrengths = buffStrength;
		}
		public BuffStats(string buffName, short buffID, int duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) {
			_waitingForEnterWorld = true;
			BuffName = buffName;
			BuffID = buffID;
			_duration = new Time(duration);
			_chanceDifficultyStrengths = chance;
			DisableImmunity = disableImmunity;
			buffStrengths = buffStrength;
		}
		private void SetUpChanceDifficultyStrength() {
			if (!Main.gameMenu) {
				if (_chanceDifficultyStrengths != null) {
					int index = _chanceDifficultyStrengths.AllValues.Length == 4 ? Main.GameMode : 0;
					Chance = _chanceDifficultyStrengths.AllValues[index];
				}
				
				if (buffStrengths != null) {
					int index = buffStrengths.AllValues.Length == 4 ? Main.GameMode : 0;
					BuffStrength = buffStrengths.AllValues[index];
				}

				_waitingForEnterWorld = false;
			}
		}
		public void CombineNoReturn(BuffStats b) {
			float bWeightedDuration = (float)b.Duration * b.Chance;
			float combinedWeightedDuration = bWeightedDuration + weightedDuration;
			float newChance = combinedWeightedDuration / _duration;
			Chance = newChance;
		}
		public BuffStats Clone() => new BuffStats(BuffName, BuffID, Duration, Chance, DisableImmunity, buffStrengths);
		public void AddBuff(Player player) {
			if (Chance == 1f || Chance >= Main.rand.NextFloat())
				player.AddBuff(BuffID, Duration);
		}
	}
}
