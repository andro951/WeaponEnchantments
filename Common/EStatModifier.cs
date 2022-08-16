using System;
using WeaponEnchantments.Common.Utility;

namespace Terraria.ModLoader
{
	public struct EStatModifier {
		//public static readonly EStatModifier Default = new EStatModifier(1f, 1f, 0f, 0f);
		public byte StatType { get; private set; }

		/// <summary>
		/// Increase to the base value of the stat. Directly added to the stat before multipliers are applied.
		/// </summary>
		public float Base {
			get => _base;
			set {
				if (value != originalBase) {
					originalBase = value;
					_base = value * EfficiencyMultiplier;
					_strength = 0f;
				}
			}
		}
		private float originalBase;
		private float _base;

		/// <summary>
		/// The combination of all additive multipliers. Starts at 1
		/// </summary>
		public float Additive {
			get => _additive;
			set {
				if (value != originalAdditive) {
					originalAdditive = value;
					_additive = 1f + value * EfficiencyMultiplier;
					_strength = 0f;
				}
			}
		}
		private float originalAdditive;
		private float _additive;

		/// <summary>
		/// The combination of all multiplicative multipliers. Starts at 1. Applies 'after' all additive bonuses have been accumulated.
		/// </summary>
		public float Multiplicative {
			get => _multiplicative;
			set {
				if (value != originalMultiplicative) {
					originalMultiplicative = value;
					_multiplicative = 1f + (value - 1f) * EfficiencyMultiplier;
					_strength = 0f;
				}
			}
		}
		private float originalMultiplicative;
		private float _multiplicative;

		/// <summary>
		/// Increase to the final value of the stat. Directly added to the stat after multipliers are applied.
		/// </summary>
		public float Flat {
			get => _flat;
			set {
				if (value != originalFlat) {
					originalFlat = value;
					_flat = value * EfficiencyMultiplier;
					_strength = 0f;
				}
			}
		}
		private float originalFlat;
		private float _flat;

		/// <summary>
		/// Modifies Additive, Multiplicative, Base and Flat.<br/>
		/// 1f is subtracted from Additive and Multiplicative before being modfied and added back after.<br/>
		/// </summary>
		public float EfficiencyMultiplier {
			get => _efficiencyMultiplier;
			set {
				if (value != _efficiencyMultiplier) {
					_efficiencyMultiplier = value;
					_additive = 1f + originalAdditive * value;
					_multiplicative = 1f + (originalMultiplicative - 1f) * _efficiencyMultiplier;
					_flat = originalFlat * value;
					_base = originalBase * value;
					_strength = 0f;
				}
			}
		}
		private float _efficiencyMultiplier;

		/// <summary>
		/// The total of the EStatModifier
		/// </summary>
		public float Strength {
			get {
				if (_strength != 0f)
					return _strength;

				if (_base > 0f || _flat > 0f) {
					_strength = ApplyTo(0f);
				}
				else {
					_strength = ApplyTo(1f);
				}

				return _strength;
			}
			private set {
				_strength = value;
				tooltip = null;
			}
		}
		private float _strength;

		private float BaseTooltip => Strength - _flat;
		public string SmartTooltip {
			get {
				if(_base <= 0f && _multiplicative != 1f) {
					return SignTooltip;
				}

				/*if (_base > 0f) {
					return SignPercentMult100Tooltip;
				}
				else {
					if (_additive > 0f) {
						if (_multiplicative != 1f) {
							return SignTooltip;
						}
						else {
							return SignPercentMult100Tooltip;
						}
					}
					else if (_multiplicative != 1f) {
						return SignTooltip;
					}
				}*/

				return SignPercentMult100Tooltip;
			}
		}

		private string FlatTooltip => _flat > 0f ? _additive > 0f || _multiplicative > 0f || _base > 0f ? ", +" : "" + $"{_flat}" : "";

		private string tooltip;
		public string SignPercentMult100Tooltip => GetTootlip(true, true, true);
		public string SignPercentTooltip => GetTootlip(true, true, false);
		public string SignMult100Tooltip => GetTootlip(true, false, true);
		public string SignTooltip => GetTootlip(true, false, false);
		public string PercentMult100Tooltip => GetTootlip(false, true, true);
		public string PercentTooltip => GetTootlip(false, true, false);
		public string Mult100Tooltip => GetTootlip(false, false, true);
		public string NoneTooltip => GetTootlip(false, false, false);

		private string GetTootlip(bool percent, bool sign, bool multiply100) {

			if (tooltip == null || _strength == 0) {
				float baseTooltip = BaseTooltip;
				tooltip = "";
				if (baseTooltip > 0f) {
					if (_base > 0f) {
						if (sign)
							tooltip += "+";

						tooltip += $"{(multiply100 ? baseTooltip.Percent() : baseTooltip)}";

					}
					else {
						tooltip += $"{baseTooltip}";
						if (sign)
							tooltip += "x";
					}

					if (percent)
						tooltip += "%";
				}

				tooltip += FlatTooltip;
			}

			return tooltip;
		}
		/*
		string final = "";
        float mult = EStatModifier.Multiplicative + EStatModifier.Additive - 2;
        float flats = EStatModifier.Base * mult + EStatModifier.Flat;

        if (flats > 0f) {
            final += $"{s(flats)}{flats}";
        }

        if (mult > 0f) {
            if (final != "") final += ' ';
            final += $"{s(mult)}{mult.Percent()}%";
        }

        return final;
		*/

		public StatModifier StatModifier => new StatModifier(_additive, _multiplicative, _flat, _base);

		public EStatModifier(byte statType, float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f, float baseEfficiencyMultiplier = 1f) {
			StatType = statType; 
			originalAdditive = additive;
			originalMultiplicative = multiplicative;
			originalFlat = flat;
			originalBase = @base;
			_efficiencyMultiplier = baseEfficiencyMultiplier;
			_additive = 1f + additive * _efficiencyMultiplier;
			_multiplicative = 1f + (multiplicative - 1f) * _efficiencyMultiplier;
			_flat = flat * _efficiencyMultiplier;
			_base = @base * _efficiencyMultiplier;
			_strength = 0f;
			tooltip = null;
		}

		public override bool Equals(object obj) {
			if (obj is not EStatModifier m)
				return false;

			return this == m;
		}

		public override int GetHashCode() {
			int hashCode = 1713062080;
			hashCode = hashCode * -1521134295 + _additive.GetHashCode();
			hashCode = hashCode * -1521134295 + _multiplicative.GetHashCode();
			hashCode = hashCode * -1521134295 + _flat.GetHashCode();
			hashCode = hashCode * -1521134295 + _base.GetHashCode();
			return hashCode;
		}

		/// <summary>
		/// By using the add operator, the supplied additive modifier is combined with the existing modifiers. For example, adding 0.12f would be equivalent to a typical 12% damage boost. For 99% of effects used in the game, this approach is used.
		/// </summary>
		/// <param name="m"></param>
		/// <param name="add">The additive modifier to add, where 0.01f is equivalent to 1%</param>
		/// <returns></returns>
		public static EStatModifier operator +(EStatModifier m, float add)
			=> new EStatModifier(m._additive + add, m._multiplicative, m._flat, m._base);

		/// <summary>
		/// By using the subtract operator, the supplied subtractive modifier is combined with the existing modifiers. For example, subtracting 0.12f would be equivalent to a typical 12% damage decrease. For 99% of effects used in the game, this approach is used.
		/// </summary>
		/// <param name="m"></param>
		/// <param name="sub">The additive modifier to subtract, where 0.01f is equivalent to 1%</param>
		/// <returns></returns>
		public static EStatModifier operator -(EStatModifier m, float sub)
			=> new EStatModifier(m._additive - sub, m._multiplicative, m._flat, m._base);

		/// <summary>
		/// The multiply operator applies a multiplicative effect to the resulting multiplicative modifier. This effect is very rarely used, typical effects use the add operator.
		/// </summary>
		/// <param name="m"></param>
		/// <param name="mul">The factor by which the multiplicative modifier is scaled</param>
		/// <returns></returns>
		public static EStatModifier operator *(EStatModifier m, float mul)
			=> new EStatModifier(m._additive, m._multiplicative * mul, m._flat, m._base);

		public static EStatModifier operator /(EStatModifier m, float div)
			=> new EStatModifier(m._additive, m._multiplicative / div, m._flat, m._base);

		public static EStatModifier operator +(float add, EStatModifier m)
			=> m + add;

		public static EStatModifier operator *(float mul, EStatModifier m)
			=> m * mul;

		public static bool operator ==(EStatModifier m1, EStatModifier m2)
			=> m1.Additive == m2.Additive && m1.Multiplicative == m2.Multiplicative && m1.Flat == m2.Flat && m1.Base == m2.Base;

		public static bool operator !=(EStatModifier m1, EStatModifier m2)
			=> m1.Additive != m2.Additive || m1.Multiplicative != m2.Multiplicative || m1.Flat != m2.Flat || m1.Base != m2.Base;

		public float ApplyTo(float baseValue) =>
			(baseValue + _base) * _additive * _multiplicative + _flat;

		public void ApplyTo(ref float baseValue) {
			baseValue = (baseValue + _base) * _additive * _multiplicative + _flat;
		}
		public void ApplyTo(ref int baseValue) {
			baseValue = (int)Math.Round(((float)baseValue + _base) * _additive * _multiplicative + _flat);
		}

		public void CombineWith(EStatModifier m) {
			_additive += m.Additive - 1f;
			_multiplicative *= m.Multiplicative;
			_flat += m.Flat;
			_base += m.Base;
			_strength = 0f;
			tooltip = null;
		}

		public StatModifier CombineWith(StatModifier m) 
			=> new StatModifier(_additive + m.Additive - 1f, _multiplicative * m.Multiplicative, _flat + m.Flat, _base + m.Base);

		public StatModifier Scale(float scale)
			=> new StatModifier(1f + (_additive - 1f) * scale, 1f + (_multiplicative - 1f) * scale, _flat * scale, _base * scale);

		public EStatModifier Clone()
			=> new EStatModifier(StatType, _additive - 1f, _multiplicative, _flat, _base);
	}
}
