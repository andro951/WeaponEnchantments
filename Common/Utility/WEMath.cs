using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility
{
	public static class WEMath
	{
		#region bools

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		/// <param name="remainder">0 if result is < int.MaxValue.  = result - int.MaxValue otherwise</param>
		/// <returns>True if the result is > int.MaxValue</returns>
		public static bool AddCheckOverflow(this ref int n1, int n2, out long remainder) {
			remainder = 0;
			try {
				checked {
					n1 += n2;
				}
			}
			catch {
				remainder = (long)n1 + (long)n2 - int.MaxValue;
				n1 = int.MaxValue;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at int.MaxValue.
		/// </summary>
		/// <param name="remainder">0 if result is < int.MaxValue.  = result - int.MaxValue otherwise</param>
		/// <returns>True if the result is > int.MaxValue</returns>
		public static bool MultiplyCheckOverflow(this ref int n1, int n2, out long remainder) {
			remainder = 0;
			try {
				checked {
					n1 *= n2;
				}
			}
			catch {
				remainder = (long)n1 * (long)n2 - int.MaxValue;
				n1 = int.MaxValue;
				return true;
			}

			return false;
		}

		#endregion

		#region voids

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		public static void AddCheckOverflow(this ref int n1, int n2) {
			try {
				checked {
					n1 += n2;
				}
			}
			catch {
				n1 = int.MaxValue;
			}
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at int.MaxValue.
		/// </summary>
		public static void MultiplyCheckOverflow(this ref int n1, int n2) {
			try {
				checked {
					n1 *= n2;
				}
			}
			catch {
				n1 = int.MaxValue;
			}
		}

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		public static void AddCheckOverflow(this ref int n1, float n2) {
			try {
				checked {
					n1 += (int)n2;
				}
			}
			catch {
				n1 = int.MaxValue;
			}
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at int.MaxValue.
		/// </summary>
		public static void MultiplyCheckOverflow(this ref int n1, float n2) {
			try {
				checked {
					n1 = (int)Math.Round((float)n1 * n2);
				}
			}
			catch {
				n1 = int.MaxValue;
			}
		}

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		public static void AddCheckOverflow(this ref float n1, float n2) {
			try {
				checked {
					n1 += n2;
				}
			}
			catch {
				n1 = float.MaxValue;
			}
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at float.MaxValue.
		/// </summary>
		public static void MultiplyCheckOverflow(this ref float n1, float n2) {
			try {
				checked {
					n1 *= n2;
				}
			}
			catch {
				n1 = float.MaxValue;
			}
		}

		#endregion

		#region returns

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		/// <param name="remainder">0 if result is < int.MaxValue.  = result - int.MaxValue otherwise</param>
		/// <returns>True if the result is > int.MaxValue</returns>
		public static int AddCheckOverflow(int n1, int n2, out long remainder) {
			remainder = 0;
			try {
				checked {
					n1 += n2;
				}
			}
			catch {
				remainder = (long)n1 + (long)n2 - int.MaxValue;
				n1 = int.MaxValue;
			}

			return n1;
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at int.MaxValue.
		/// </summary>
		/// <param name="remainder">0 if result is < int.MaxValue.  = result - int.MaxValue otherwise</param>
		/// <returns>True if the result is > int.MaxValue</returns>
		public static int MultiplyCheckOverflow(int n1, int n2, out long remainder) {
			remainder = 0;
			try {
				checked {
					n1 *= n2;
				}
			}
			catch {
				remainder = (long)n1 * (long)n2 - int.MaxValue;
				n1 = int.MaxValue;
			}

			return n1;
		}

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		public static int AddCheckOverflow(int n1, int n2) {
			try {
				checked {
					n1 += n2;
				}
			}
			catch {
				n1 = int.MaxValue;
			}

			return n1;
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at int.MaxValue.
		/// </summary>
		public static int MultiplyCheckOverflow(int n1, int n2) {
			try {
				checked {
					n1 *= n2;
				}
			}
			catch {
				n1 = int.MaxValue;
			}

			return n1;
		}

		/// <summary>
		/// Adds n2 to n1 and caps n1 at int.MaxValue.
		/// </summary>
		public static float AddCheckOverflow(float n1, float n2) {
			try {
				checked {
					n1 += n2;
				}
			}
			catch {
				n1 = float.MaxValue;
			}

			return n1;
		}

		/// <summary>
		/// Multiplies n1 by n2 and caps n1 at float.MaxValue.
		/// </summary>
		public static float MultiplyCheckOverflow(float n1, float n2) {
			try {
				checked {
					n1 *= n2;
				}
			}
			catch {
				n1 = float.MaxValue;
			}

			return n1;
		}

		#endregion
	}
}
