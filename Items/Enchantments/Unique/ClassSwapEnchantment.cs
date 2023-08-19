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
using androLib.Common.Utility;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Items.Enchantments.Unique
{
	public abstract class ClassSwapEnchantment : Enchantment
	{
		public override int StrengthGroup => 17;
		public override float ScalePercent => 0.1f;
		protected abstract DamageClass MyDamageClass { get; }
		protected virtual string ModdedDamageClass { get; } = "";
		protected virtual DamageClassID DamageClassNameOveride => DamageClassID.Default;
		protected virtual float DropChance => 0.05f * ConfigValues.EnchantmentDropChance;
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
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.CorruptSlime, chance: DropChance),
			new(NPCID.EaterofSouls, chance: DropChance),
			new(NPCID.Crimera, chance: DropChance),
			new(NPCID.FaceMonster, chance: DropChance)
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
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.JungleBat, chance: DropChance),
			new(NPCID.JungleSlime, chance: DropChance)
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
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.IceSlime, chance: DropChance),
			new(NPCID.ZombieEskimo, chance: DropChance)
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
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Antlion, chance: DropChance),
			new(NPCID.Vulture, chance: DropChance)
		};
	}
	public class RangedClassSwapEnchantmentCommon : RangedClassSwapEnchantment { }
	public class RangedClassSwapEnchantmentRare : RangedClassSwapEnchantment { }
	public class RangedClassSwapEnchantmentEpic : RangedClassSwapEnchantment { }
	public class RangedClassSwapEnchantmentLegendary : RangedClassSwapEnchantment { }

	public abstract class ThrowingClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.Throwing;
	}
	public class ThrowingClassSwapEnchantmentBasic : ThrowingClassSwapEnchantment
	{
		public override SellCondition SellCondition => WEMod.thoriumEnabled ? SellCondition.AnyTimeRare : SellCondition.Never;
		public override List<ModDropData> ModNpcDropNames => WEMod.thoriumEnabled ? new() {
			new("ThoriumMod/TheGrandThunderBirdv2")
		} : null;
	}
	public class ThrowingClassSwapEnchantmentCommon : ThrowingClassSwapEnchantment { }
	public class ThrowingClassSwapEnchantmentRare : ThrowingClassSwapEnchantment { }
	public class ThrowingClassSwapEnchantmentEpic : ThrowingClassSwapEnchantment { }
	public class ThrowingClassSwapEnchantmentLegendary : ThrowingClassSwapEnchantment { }


	public abstract class RogueClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => CalamityValues.rogue ?? DamageClass.Throwing;
		public override string CustomTooltip => CalamityIntegration.CALAMITY_NAME.Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
		public override string Designer => "Vyklade";
	}
	public class RogueClassSwapEnchantmentBasic : RogueClassSwapEnchantment
	{
		public override SellCondition SellCondition => WEMod.calamityEnabled ? SellCondition.AnyTimeRare : SellCondition.Never;
		public override List<ModDropData> ModNpcDropNames => WEMod.calamityEnabled ? new() {
			new("CalamityMod/DesertScourgeHead")
		} : null;
	}
	public class RogueClassSwapEnchantmentCommon : RogueClassSwapEnchantment { }
	public class RogueClassSwapEnchantmentRare : RogueClassSwapEnchantment { }
	public class RogueClassSwapEnchantmentEpic : RogueClassSwapEnchantment { }
	public class RogueClassSwapEnchantmentLegendary : RogueClassSwapEnchantment { }


	public abstract class KiClassSwapEnchantment : ClassSwapEnchantment
	{
		public override void GetMyStats() {
			base.GetMyStats();
			Effects.Add(new KiDamage());
		}
		protected override DamageClass MyDamageClass => DBZMODPORTIntegration.Enabled ? DamageClass.Generic : DamageClass.Magic;
		public override string CustomTooltip => DBZMODPORTIntegration.DBT_NAME.Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
		protected override DamageClassID DamageClassNameOveride => DamageClassID.Ki;
		public override string Designer => "Vyklade";
	}
	public class KiClassSwapEnchantmentBasic : KiClassSwapEnchantment
	{
		public override SellCondition SellCondition => WEMod.dbtEnabled ? SellCondition.AnyTimeRare : SellCondition.Never;
		public override List<DropData> NpcDropTypes => WEMod.dbtEnabled ? new() {
			new(NPCID.Golem, 9f)
		} : null;
	}
	public class KiClassSwapEnchantmentCommon : KiClassSwapEnchantment { }
	public class KiClassSwapEnchantmentRare : KiClassSwapEnchantment { }
	public class KiClassSwapEnchantmentEpic : KiClassSwapEnchantment { }
	public class KiClassSwapEnchantmentLegendary : KiClassSwapEnchantment { }

	public abstract class BardClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => ThoriumValues.bard ?? DamageClass.Melee;
		public override string CustomTooltip => ThoriumIntegration.THORIUM_NAME.Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
	}
	public class BardClassSwapEnchantmentBasic : BardClassSwapEnchantment
	{
		public override SellCondition SellCondition => WEMod.thoriumEnabled ? SellCondition.AnyTimeRare : SellCondition.Never;
		public override List<ModDropData> ModNpcDropNames => WEMod.thoriumEnabled ? new() {
			new("ThoriumMod/QueenJelly")
		} : null;
	}
	public class BardClassSwapEnchantmentCommon : BardClassSwapEnchantment { }
	public class BardClassSwapEnchantmentRare : BardClassSwapEnchantment { }
	public class BardClassSwapEnchantmentEpic : BardClassSwapEnchantment { }
	public class BardClassSwapEnchantmentLegendary : BardClassSwapEnchantment { }

	public abstract class HealerClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => ThoriumValues.healerRadiation ?? DamageClass.Ranged;
		public override string CustomTooltip => ThoriumIntegration.THORIUM_NAME.Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
	}
	public class HealerClassSwapEnchantmentBasic : HealerClassSwapEnchantment
	{
		public override SellCondition SellCondition => WEMod.thoriumEnabled ? SellCondition.AnyTimeRare : SellCondition.Never;
		public override List<ModDropData> ModNpcDropNames => WEMod.thoriumEnabled ? new() {
			new("ThoriumMod/GraniteEnergyStorm"),
			new("ThoriumMod/TheBuriedWarrior")
		} : null;
	}
	public class HealerClassSwapEnchantmentCommon : HealerClassSwapEnchantment { }
	public class HealerClassSwapEnchantmentRare : HealerClassSwapEnchantment { }
	public class HealerClassSwapEnchantmentEpic : HealerClassSwapEnchantment { }
	public class HealerClassSwapEnchantmentLegendary : HealerClassSwapEnchantment { }
}
