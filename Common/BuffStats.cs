using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Utility;
using static androLib.Common.Utility.ModMath;
using androLib.Common.Utility;
using androLib.Common.Globals;
using WeaponEnchantments.Common.Configs;

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
		private Time _originalDuration;
		public float Chance {
			get {
				if (_waitingForEnterWorld)
					SetUpChanceDifficultyStrength();

				return _chance;
			}
			private set {
				if (value > 1f) {
					_chance = 1f;
					int ticks = _duration.Ticks;
					ticks.MultiplyCheckOverflow(value);
					_duration.Ticks = ticks;
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

				return _buffStrength;
			}
			protected set => _buffStrength = value;
		}
		private float _buffStrength;
		private DifficultyStrength buffStrengths;

		public BuffStats(string buffName, short buffID, Time duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) {
			if (buffStrength != null)
				_waitingForEnterWorld = true;

			BuffName = buffName;
			BuffID = buffID;
			_duration = duration;
			_originalDuration = _duration.Clone();
			Chance = chance;
			DisableImmunity = disableImmunity;
			buffStrengths = buffStrength?.Clone();
		}
		public BuffStats(string buffName, short buffID, uint duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) {
			if (buffStrength != null)
				_waitingForEnterWorld = true;

			BuffName = buffName;
			BuffID = buffID;
			_duration = new Time((uint)duration);
			_originalDuration = _duration.Clone();
			Chance = chance;
			DisableImmunity = disableImmunity;
			buffStrengths = buffStrength?.Clone();
		}
		public BuffStats(string buffName, short buffID, Time duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) {
			_waitingForEnterWorld = true;
			BuffName = buffName;
			BuffID = buffID;
			_duration = duration;
			_originalDuration = _duration.Clone();
			_chanceDifficultyStrengths = chance;
			DisableImmunity = disableImmunity;
			buffStrengths = buffStrength?.Clone();
		}
		public BuffStats(string buffName, short buffID, uint duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) {
			_waitingForEnterWorld = true;
			BuffName = buffName;
			BuffID = buffID;
			_duration = new Time((uint)duration);
			_originalDuration = _duration.Clone();
			_chanceDifficultyStrengths = chance;
			DisableImmunity = disableImmunity;
			buffStrengths = buffStrength?.Clone();
		}
		public BuffStats(string buffName, short buffID, uint duration, float chance, bool disableImmunity, float buffStrength) {
			_waitingForEnterWorld = true;
			BuffName = buffName;
			BuffID = buffID;
			_duration = new Time((uint)duration);
			_originalDuration = _duration.Clone();
			Chance = chance;
			DisableImmunity = disableImmunity;
			BuffStrength = buffStrength;
		}
		private void SetUpChanceDifficultyStrength() {
			if (_chanceDifficultyStrengths != null) {
				_duration = _originalDuration.Clone();
				int index = Main.gameMenu ? 0 : _chanceDifficultyStrengths.AllValues.Length == 4 ? Main.GameMode : 0;
				Chance = _chanceDifficultyStrengths.AllValues[index];
			}
			
			if (buffStrengths != null) {
				int index = Main.gameMenu ? 0 : buffStrengths.AllValues.Length == 4 ? Main.GameMode : 0;
				BuffStrength = buffStrengths.AllValues[index];
			}

			if (!Main.gameMenu)
				_waitingForEnterWorld = false;
		}
		public void CombineNoReturn(BuffStats b) {
			float bWeightedDuration = (float)b.Duration * b.Chance;
			float combinedWeightedDuration = bWeightedDuration + weightedDuration;
			float newChance = combinedWeightedDuration / _duration;
			Chance = newChance;
		}
		public BuffStats Clone() => new BuffStats(BuffName, BuffID, Duration.Clone(), Chance, DisableImmunity, buffStrengths?.Clone());

		public void TryApplyToPlayer(Player player, bool addToExisting = false) {
			if (Chance >= 1f || Chance >= Main.rand.NextFloat()) {
				int buffIndex = player.FindBuffIndex(BuffID);
				int ticks = addToExisting ? Math.Min(Duration.Ticks, ConfigValues.BuffDurationTicks) : Duration.Ticks;
				if (!addToExisting || buffIndex < 0) {
					if (addToExisting)
						ticks++;

					player.AddBuff(BuffID, ticks);
				}
				else {
					player.buffTime[buffIndex] += ticks;
				}
			}
		}
		public void TryApplyBuffToNPC(NPC npc, bool addToExisting = false) {
			if (Chance >= 1f || Chance >= Main.rand.NextFloat()) {
				int buffIndex = npc.FindBuffIndex(BuffID);
				int ticks = addToExisting ? Math.Min(Duration.Ticks, ConfigValues.BuffDurationTicks) : Duration.Ticks;
				if (!addToExisting || buffIndex < 0) {
					if (addToExisting)
						ticks++;

					npc.AddBuff(BuffID, ticks);
				}
				else {
					npc.buffTime[buffIndex] += ticks;
				}
			}
		}
		public override string ToString() {
			return $"BuffName: {BuffName}, BuffID: {BuffID}, Duration: {Duration}, Duration.Ticks: {Duration.Ticks}, Chance: ";
		}
	}
}
