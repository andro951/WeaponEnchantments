using System.Collections.Generic;
using Terraria.ID;
using static WeaponEnchantments.Common.EnchantingRarity;
using WeaponEnchantments.Effects;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class ClassSwapEnchantment : Enchantment
	{
		public override int StrengthGroup => 17;
		public override float ScalePercent => 0.1f;
		public override SellCondition SellCondition => SellCondition.HardMode;
		protected abstract DamageClass MyDamageClass { get; }
		protected virtual string ModdedDamageClass { get; } = "";
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new DamageClassSwap(MyDamageClass)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string Artist => "andro951";
		public override string Designer => "andro951";
	}
	public abstract class MeleeClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.Melee;
	}
	public class MeleeClassSwapEnchantmentBasic : MeleeClassSwapEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.CorruptSlime),
			new(NPCID.EaterofSouls),
			new(NPCID.Crimera),
			new(NPCID.FaceMonster)
		};
	}
	public class MeleeClassSwapEnchantmentCommon : MeleeClassSwapEnchantment { }
	public class MeleeClassSwapEnchantmentRare : MeleeClassSwapEnchantment { }
	public class MeleeClassSwapEnchantmentSuperRare : MeleeClassSwapEnchantment { }
	public class MeleeClassSwapEnchantmentUltraRare : MeleeClassSwapEnchantment { }

	public abstract class WhipClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.SummonMeleeSpeed;
	}
	public class WhipClassSwapEnchantmentBasic : WhipClassSwapEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.JungleBat),
			new(NPCID.JungleSlime)
		};
	}
	public class WhipClassSwapEnchantmentCommon : WhipClassSwapEnchantment { }
	public class WhipClassSwapEnchantmentRare : WhipClassSwapEnchantment { }
	public class WhipClassSwapEnchantmentSuperRare : WhipClassSwapEnchantment { }
	public class WhipClassSwapEnchantmentUltraRare : WhipClassSwapEnchantment { }


	public abstract class MagicClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.Magic;
	}
	public class MagicClassSwapEnchantmentBasic : MagicClassSwapEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.IceSlime),
			new(NPCID.ZombieEskimo)
		};
	}
	public class MagicClassSwapEnchantmentCommon : MagicClassSwapEnchantment { }
	public class MagicClassSwapEnchantmentRare : MagicClassSwapEnchantment { }
	public class MagicClassSwapEnchantmentSuperRare : MagicClassSwapEnchantment { }
	public class MagicClassSwapEnchantmentUltraRare : MagicClassSwapEnchantment { }


	public abstract class RangedClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.Ranged;
	}
	public class RangedClassSwapEnchantmentBasic : RangedClassSwapEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.Antlion),
			new(NPCID.Vulture)
		};
	}
	public class RangedClassSwapEnchantmentCommon : RangedClassSwapEnchantment { }
	public class RangedClassSwapEnchantmentRare : RangedClassSwapEnchantment { }
	public class RangedClassSwapEnchantmentSuperRare : RangedClassSwapEnchantment { }
	public class RangedClassSwapEnchantmentUltraRare : RangedClassSwapEnchantment { }

	
	public abstract class RogueClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && calamityMod.TryFind(ModdedDamageClass, out DamageClass moddedDamageClass) ? moddedDamageClass : DamageClass.Throwing;
		protected override string ModdedDamageClass => "RogueDamageClass";
		public override string CustomTooltip => "(Calamity Mod Enchantment)";
		public override string Designer => "Vyklade";
	}
	public class RogueClassSwapEnchantmentBasic : RogueClassSwapEnchantment
	{
		public override List<WeightedPair> NpcDropTypes => WEMod.calamityEnabled ? new () {
			new(NPCID.KingSlime, 9f)
		} : null;
	}
	public class RogueClassSwapEnchantmentCommon : RogueClassSwapEnchantment { }
	public class RogueClassSwapEnchantmentRare : RogueClassSwapEnchantment { }
	public class RogueClassSwapEnchantmentSuperRare : RogueClassSwapEnchantment { }
	public class RogueClassSwapEnchantmentUltraRare : RogueClassSwapEnchantment { }
}
