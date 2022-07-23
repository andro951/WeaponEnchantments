﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class MoveSpeedEnchantment : Enchantment
	{
		public override int StrengthGroup => 11;//0.04f, 0.08f, 0.12f, 0.16f, 0.20f
		public override Dictionary<string, float> AllowedList => new Dictionary<string, float>() {
			{ "Weapon", 1f },
			{ "Armor", 1f },
			{ "Accessory", 1f }
		};
		public override void MyDefaults() {
			CheckStaticStatByName();
		}
	}
	public class MoveSpeedEnchantmentBasic : MoveSpeedEnchantment { }
	public class MoveSpeedEnchantmentCommon : MoveSpeedEnchantment { }
	public class MoveSpeedEnchantmentRare : MoveSpeedEnchantment { }
	public class MoveSpeedEnchantmentSuperRare : MoveSpeedEnchantment { }
	public class MoveSpeedEnchantmentUltraRare : MoveSpeedEnchantment { }
}