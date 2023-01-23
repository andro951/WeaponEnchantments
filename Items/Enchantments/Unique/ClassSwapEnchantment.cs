using System.Collections.Generic;
using Terraria.ID;
using static WeaponEnchantments.Common.EnchantingRarity;
using WeaponEnchantments.Effects;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.ModIntegration;
using System.Linq;
using static WeaponEnchantments.WEPlayer;
using WeaponEnchantments.Effects.CustomEffects;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class ClassSwapEnchantment : Enchantment
	{
		public override int StrengthGroup => 17;
		public override float ScalePercent => 0.1f;
		protected abstract DamageClass MyDamageClass { get; }
		protected virtual string ModdedDamageClass { get; } = "";
		protected virtual DamageClassID DamageClassNameOveride => DamageClassID.Default;
		public override void GetMyStats() {
			Effects = new() {
				new DamageAfterDefenses(multiplicative: EnchantmentStrengthData),
				new DamageClassSwap(MyDamageClass, damageClassNameOveride: DamageClassNameOveride)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f }
			};

			RestrictedClass.Add((int)DamageClassID.Summon);
			RestrictedClass.Add(MyDamageClass.Type);
		}

		public override string Artist => "andro951";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public abstract class MeleeClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.Melee;
	}
	public class MeleeClassSwapEnchantmentBasic : MeleeClassSwapEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.CorruptSlime),
			new(NPCID.EaterofSouls),
			new(NPCID.Crimera),
			new(NPCID.FaceMonster)
		};
	}
	public class MeleeClassSwapEnchantmentCommon : MeleeClassSwapEnchantment { }
	public class MeleeClassSwapEnchantmentRare : MeleeClassSwapEnchantment { }
	public class MeleeClassSwapEnchantmentEpic : MeleeClassSwapEnchantment { }
	public class MeleeClassSwapEnchantmentLegendary : MeleeClassSwapEnchantment { }

	public abstract class WhipClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.SummonMeleeSpeed;
		public override void GetMyStats() {
			base.GetMyStats();
			Effects.Add(new MinionAttackTarget());
		}
	}
	public class WhipClassSwapEnchantmentBasic : WhipClassSwapEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.JungleBat),
			new(NPCID.JungleSlime)
		};
	}
	public class WhipClassSwapEnchantmentCommon : WhipClassSwapEnchantment { }
	public class WhipClassSwapEnchantmentRare : WhipClassSwapEnchantment { }
	public class WhipClassSwapEnchantmentEpic : WhipClassSwapEnchantment { }
	public class WhipClassSwapEnchantmentLegendary : WhipClassSwapEnchantment { }


	public abstract class MagicClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.Magic;
	}
	public class MagicClassSwapEnchantmentBasic : MagicClassSwapEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.IceSlime),
			new(NPCID.ZombieEskimo)
		};
	}
	public class MagicClassSwapEnchantmentCommon : MagicClassSwapEnchantment { }
	public class MagicClassSwapEnchantmentRare : MagicClassSwapEnchantment { }
	public class MagicClassSwapEnchantmentEpic : MagicClassSwapEnchantment { }
	public class MagicClassSwapEnchantmentLegendary : MagicClassSwapEnchantment { }


	public abstract class RangedClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.Ranged;
	}
	public class RangedClassSwapEnchantmentBasic : RangedClassSwapEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override List<WeightedPair> NpcDropTypes => new() {
			new(NPCID.Antlion),
			new(NPCID.Vulture)
		};
	}
	public class RangedClassSwapEnchantmentCommon : RangedClassSwapEnchantment { }
	public class RangedClassSwapEnchantmentRare : RangedClassSwapEnchantment { }
	public class RangedClassSwapEnchantmentEpic : RangedClassSwapEnchantment { }
	public class RangedClassSwapEnchantmentLegendary : RangedClassSwapEnchantment { }

	
	public abstract class RogueClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => ModIntegration.CalamityValues.rogue ?? DamageClass.Throwing;
		public override string CustomTooltip => CalamityIntegration.calamityName.Lang(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
		public override string Designer => "Vyklade";
	}
	public class RogueClassSwapEnchantmentBasic : RogueClassSwapEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override List<WeightedPair> NpcDropTypes => WEMod.calamityEnabled ? new () {
			new(NPCID.KingSlime, 9f)
		} : null;
	}
	public class RogueClassSwapEnchantmentCommon : RogueClassSwapEnchantment { }
	public class RogueClassSwapEnchantmentRare : RogueClassSwapEnchantment { }
	public class RogueClassSwapEnchantmentEpic : RogueClassSwapEnchantment { }
	public class RogueClassSwapEnchantmentLegendary : RogueClassSwapEnchantment { }


	public abstract class KiClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DBZMODPORTIntegration.Enabled ? DamageClass.Generic : DamageClass.Magic;
		public override string CustomTooltip => DBZMODPORTIntegration.DBTName.Lang(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
		protected override DamageClassID DamageClassNameOveride => DamageClassID.Ki;
		public override string Designer => "Vyklade";
	}
	public class KiClassSwapEnchantmentBasic : KiClassSwapEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override List<WeightedPair> NpcDropTypes => WEMod.dbtEnabled ? new()
		{
			new(NPCID.Golem, 9f)
		} : null;
	}
	public class KiClassSwapEnchantmentCommon : KiClassSwapEnchantment { }
	public class KiClassSwapEnchantmentRare : KiClassSwapEnchantment { }
	public class KiClassSwapEnchantmentEpic : KiClassSwapEnchantment { }
	public class KiClassSwapEnchantmentLegendary : KiClassSwapEnchantment { }

}
