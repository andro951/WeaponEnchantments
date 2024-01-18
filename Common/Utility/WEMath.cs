using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeaponEnchantments.Common.Utility.UtilityMethods;

namespace WeaponEnchantments.Common.Utility
{
	public static class WEMath
	{
		#region bools
		/*
		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		/// <param name="remainder">0 if result is < int.MaxValue.  = result - int.MaxValue otherwise</param>
		/// <returns>True if the result is > int.MaxValue</returns>
		public static bool AddCheckOverflow(this ref int n1, int n2, out long remainder) {
			n1 = AddCheckOverflow(n1, n2, out remainder);
			return remainder != 0;
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at int.MaxValue.
		/// </summary>
		/// <param name="remainder">0 if result is < int.MaxValue.  = result - int.MaxValue otherwise</param>
		/// <returns>True if the result is > int.MaxValue</returns>
		public static bool MultiplyCheckOverflow(this ref int n1, int n2, out long remainder) {
			n1 = MultiplyCheckOverflow(n1, n2, out remainder);
			return remainder != 0;
		}

		#endregion

		#region voids

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		public static void AddCheckOverflow(this ref int n1, int n2) {
			int maxN2 = int.MaxValue - n1;
			if (n2 > maxN2) {
				n1 = int.MaxValue;
				return;
			}

			n1 += n2;
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at int.MaxValue.
		/// </summary>
		public static void MultiplyCheckOverflow(this ref int n1, int n2) {
			int maxN2 = int.MaxValue / n1;
			if (n2 > maxN2) {
				n1 = int.MaxValue;
				return;
			}

			n1 *= n2;
		}

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		public static void AddCheckOverflow(this ref int n1, float n2) => n1.AddCheckOverflow((int)n2);

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at int.MaxValue.
		/// </summary>
		public static void MultiplyCheckOverflow(this ref int n1, float n2) {
			float maxN2 = (float)int.MaxValue / (float)n1;
			if (n2 > maxN2) {
				n1 = int.MaxValue;
				return;
			}

			n1 = (int)Math.Round((float)n1 * n2);
		}

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		public static void AddCheckOverflow(this ref float n1, float n2) {
			float maxN2 = float.MaxValue - n1;
			if (n2 > maxN2) {
				n1 = float.MaxValue;
				return;
			}

			n1 += n2;
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at float.MaxValue.
		/// </summary>
		public static void MultiplyCheckOverflow(this ref float n1, float n2) {
			float maxN2 = float.MaxValue / n1;
			if (n2 > maxN2) {
				n1 = float.MaxValue;
				return;
			}

			n1 *= n2;
		}

		#endregion

		#region returns

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		/// <param name="remainder">0 if result is < int.MaxValue.  = result - int.MaxValue otherwise</param>
		/// <returns>True if the result is > int.MaxValue</returns>
		public static int AddCheckOverflow(int n1, int n2, out long remainder) {
			int maxN2 = int.MaxValue - n1;
			if (n2 > maxN2) {
				remainder = (long)n1 + (long)n2 - int.MaxValue;
				return int.MaxValue;
			}

			remainder = 0;
			return n1 + n2;
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at int.MaxValue.
		/// </summary>
		/// <param name="remainder">0 if result is < int.MaxValue.  = result - int.MaxValue otherwise</param>
		/// <returns>True if the result is > int.MaxValue</returns>
		public static int MultiplyCheckOverflow(int n1, int n2, out long remainder) {
			int maxN2 = int.MaxValue / n1;
			if (n2 > maxN2) {
				remainder = (long)n1 * (long)n2 - int.MaxValue;
				return int.MaxValue;
			}

			remainder = 0;
			return n1 * n2;
		}

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		public static int AddCheckOverflow(int n1, int n2) {
			n1.AddCheckOverflow(n2);
			return n1;
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at int.MaxValue.
		/// </summary>
		public static int MultiplyCheckOverflow(int n1, int n2) {
			n1.MultiplyCheckOverflow(n2);
			return n1;
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at int.MaxValue.
		/// </summary>
		public static int MultiplyCheckOverflow(int n1, float n2) {
			n1.MultiplyCheckOverflow(n2);
			return n1;
		}

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		public static float AddCheckOverflow(float n1, float n2) {
			n1.AddCheckOverflow(n2);
			return n1;
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at float.MaxValue.
		/// </summary>
		public static float MultiplyCheckOverflow(float n1, float n2) {
			n1.MultiplyCheckOverflow(n2);
			return n1;
		}

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		public static double AddCheckOverflow(this double n1, double n2) {
			double maxN2 = double.MaxValue - n1;
			if (n2 > maxN2) {
				n1 = double.MaxValue;
				return n1;
			}
			
			return n1 += n2;
		}
		public static dynamic AddCheckOverflow(dynamic n1, dynamic n2) {
			dynamic maxValue = (dynamic)n1.GetType().GetField("MaxValue").GetValue(null);
			dynamic maxN2 = maxValue - n1;
			if (n2 > maxN2) {
				n1 = maxValue;
				return n1;
			}

			return n1 += n2;
		}
		public static dynamic SumCheckOverFlow(dynamic enumerable) {
			dynamic maxValue = (dynamic)enumerable[0].GetType().GetField("MaxValue").GetValue(null);
			dynamic total = 0;
			foreach(dynamic value in enumerable) {
				dynamic max = maxValue - total;
				if (value > max) {
					total = maxValue;
					return total;
				}

				total += value;
			}

			return total;
		}
		public static int CeilingDivide(this int num, int denom) {
			int result = num / denom;
			int r = num % denom;
			if (r > 0)
				result++;

			return result;
		}
		public static int RoundDivide(this int num, int denom) {
			int result = num / denom;
			int r = num % denom;
			if (r >= denom / 2)
				result++;

			return result;
		}
		public static int Ceiling(this float f) {
			int result = (int)f;
			if (result < f)
				result++;

			return result;
		}
		public static int Abs(this int v) => v < 0 ? -v : v;
		*/
		#endregion
	}
}
