using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponEnchantments.Common
{
	public class DifficultyStrength
	{
		private float[] _values;
		public float[] AllValues => _values;
		public float Value {
			set => _values[0] = value;
			get {
				if (_values.Length > 1)
					return _values[Main.GameMode];

				return _values[0];
			}
		}

		public DifficultyStrength(float[] values) {
			_values = values;
		}

		public static DifficultyStrength operator *(DifficultyStrength es, float mult) {
			if (es == null)
				return null;

			float[] arr = es._values;
			for (int i = 0; i < arr.Length; i++) {
				arr[i] *= mult;
			}

			return new DifficultyStrength(arr);
		}
		public static DifficultyStrength operator +(DifficultyStrength es, float mult) {
			if (es == null)
				return null;

			float[] arr = es._values;
			for (int i = 0; i < arr.Length; i++) {
				arr[i] += mult;
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
	}
}
