using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Common
{
	public class DifficultyStrength {
		private float[] _values;
		public float[] AllValues => _values;
		public float Value {
			set => _values[0] = value;
			get {
				if (_values.Length == 4)
					return _values[Main.GameMode];

				return _values[0];
			}
		}

		public DifficultyStrength(float[] values) {
			_values = values;
		}

		/// <summary>
		/// Don't use this one for enchantments.  It will set all values equal to the single value, preventing scaling with world difficulty.
		/// </summary>
		/// <param name="value"></param>
		public DifficultyStrength(float value) {
			_values = new float[4];
			for(int i = 0; i < _values.Length; i++) {
				_values[i] = value;
			}
		}

		public static DifficultyStrength Default => new(new float[] {1f, 1f, 1f, 1f});

		public static DifficultyStrength operator *(DifficultyStrength ds, float mult) {
			if (ds == null)
				return null;

			float[] arr = (float[])ds._values.Clone();
			for (int i = 0; i < arr.Length; i++) {
				arr[i] *= mult;
			}

			return new DifficultyStrength(arr);
		}
		public static DifficultyStrength operator +(DifficultyStrength ds, float mult) {
			if (ds == null)
				return null;

			float[] arr = (float[])ds._values.Clone();
			for (int i = 0; i < arr.Length; i++) {
				arr[i] += mult;
			}

			return new DifficultyStrength(arr);
		}
		public static DifficultyStrength operator -(DifficultyStrength ds, float mult) {
			if (ds == null)
				return null;

			float[] arr = (float[])ds._values.Clone();
			for (int i = 0; i < arr.Length; i++) {
				arr[i] -= mult;
			}

			return new DifficultyStrength(arr);
		}
		public static DifficultyStrength operator /(DifficultyStrength ds, float mult) {
			if (ds == null)
				return null;

			float[] arr = (float[])ds._values.Clone();
			for (int i = 0; i < arr.Length; i++) {
				arr[i] /= mult;
			}

			return new DifficultyStrength(arr);
		}
		public static DifficultyStrength operator /(DifficultyStrength ds, int mult) {
			if (ds == null)
				return null;

			float[] arr = (float[])ds._values.Clone();
			for (int i = 0; i < arr.Length; i++) {
				float value = arr[i] / mult;
				arr[i] = (float)(int)value;
			}

			return new DifficultyStrength(arr);
		}
		public static DifficultyStrength operator ^(DifficultyStrength ds, int mult) {
			if (ds == null)
				return null;

			float[] arr = (float[])ds._values.Clone();
			for (int i = 0; i < arr.Length; i++) {
				arr[i] = (float)Math.Pow(arr[i], mult);
			}

			return new DifficultyStrength(arr);
		}
		public DifficultyStrength Invert() {
			float[] arr = (float[])_values.Clone();
			for (int i = 0; i < arr.Length; i++) {
				arr[i] = 1f / arr[i];
			}

			return new DifficultyStrength(arr);
		}
		public DifficultyStrength Min(float min) {
			float[] arr = (float[])_values.Clone();
			for (int i = 0; i < arr.Length; i++) {
				if (arr[i] < min)
					arr[i] = min;
			}

			return new DifficultyStrength(arr);
		}
		public DifficultyStrength Max(float max) {
			float[] arr = (float[])_values.Clone();
			for (int i = 0; i < arr.Length; i++) {
				if (arr[i] > max)
					arr[i] = max;
			}

			return new DifficultyStrength(arr);
		}
		public override string ToString() {
			string s = "";
			int length = _values.Length;
			if (length > 0) {
				s = $"{_values[0]}";
				for(int i = 1; i < length; i++) {
					s += $", {_values[i]}";
				}
			}

			return s;
		}
		public DifficultyStrength Clone() => new DifficultyStrength(_values);
	}
}
