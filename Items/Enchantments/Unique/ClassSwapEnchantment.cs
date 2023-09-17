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
using androLib;

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
	[Autoload(false)]
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
	[Autoload(false)]
	public class MeleeClassSwapEnchantmentCommon : MeleeClassSwapEnchantment { }
	[Autoload(false)]
	public class MeleeClassSwapEnchantmentRare : MeleeClassSwapEnchantment { }
	[Autoload(false)]
	public class MeleeClassSwapEnchantmentEpic : MeleeClassSwapEnchantment { }
	[Autoload(false)]
	public class MeleeClassSwapEnchantmentLegendary : MeleeClassSwapEnchantment { }

	public abstract class WhipClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.SummonMeleeSpeed;
		public override void GetMyStats() {
			base.GetMyStats();
			Effects.Add(new MinionAttackTarget());
		}
	}
	[Autoload(false)]
	public class WhipClassSwapEnchantmentBasic : WhipClassSwapEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.JungleBat, chance: DropChance),
			new(NPCID.JungleSlime, chance: DropChance)
		};
	}
	[Autoload(false)]
	public class WhipClassSwapEnchantmentCommon : WhipClassSwapEnchantment { }
	[Autoload(false)]
	public class WhipClassSwapEnchantmentRare : WhipClassSwapEnchantment { }
	[Autoload(false)]
	public class WhipClassSwapEnchantmentEpic : WhipClassSwapEnchantment { }
	[Autoload(false)]
	public class WhipClassSwapEnchantmentLegendary : WhipClassSwapEnchantment { }


	public abstract class MagicClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.Magic;
	}
	[Autoload(false)]
	public class MagicClassSwapEnchantmentBasic : MagicClassSwapEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.IceSlime, chance: DropChance),
			new(NPCID.ZombieEskimo, chance: DropChance)
		};
	}
	[Autoload(false)]
	public class MagicClassSwapEnchantmentCommon : MagicClassSwapEnchantment { }
	[Autoload(false)]
	public class MagicClassSwapEnchantmentRare : MagicClassSwapEnchantment { }
	[Autoload(false)]
	public class MagicClassSwapEnchantmentEpic : MagicClassSwapEnchantment { }
	[Autoload(false)]
	public class MagicClassSwapEnchantmentLegendary : MagicClassSwapEnchantment { }


	public abstract class RangedClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.Ranged;
	}
	[Autoload(false)]
	public class RangedClassSwapEnchantmentBasic : RangedClassSwapEnchantment
	{
		public override SellCondition SellCondition => SellCondition.AnyTimeRare;
		public override List<DropData> NpcDropTypes => new() {
			new(NPCID.Antlion, chance: DropChance),
			new(NPCID.Vulture, chance: DropChance)
		};
	}
	[Autoload(false)]
	public class RangedClassSwapEnchantmentCommon : RangedClassSwapEnchantment { }
	[Autoload(false)]
	public class RangedClassSwapEnchantmentRare : RangedClassSwapEnchantment { }
	[Autoload(false)]
	public class RangedClassSwapEnchantmentEpic : RangedClassSwapEnchantment { }
	[Autoload(false)]
	public class RangedClassSwapEnchantmentLegendary : RangedClassSwapEnchantment { }

	public abstract class ThrowingClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => DamageClass.Throwing;
	}
	[Autoload(false)]
	public class ThrowingClassSwapEnchantmentBasic : ThrowingClassSwapEnchantment
	{
		public override SellCondition SellCondition => AndroMod.thoriumEnabled ? SellCondition.AnyTimeRare : SellCondition.Never;
		public override List<ModDropData> ModNpcDropNames => AndroMod.thoriumEnabled ? new() {
			new("ThoriumMod/TheGrandThunderBirdv2")
		} : null;
	}
	[Autoload(false)]
	public class ThrowingClassSwapEnchantmentCommon : ThrowingClassSwapEnchantment { }
	[Autoload(false)]
	public class ThrowingClassSwapEnchantmentRare : ThrowingClassSwapEnchantment { }
	[Autoload(false)]
	public class ThrowingClassSwapEnchantmentEpic : ThrowingClassSwapEnchantment { }
	[Autoload(false)]
	public class ThrowingClassSwapEnchantmentLegendary : ThrowingClassSwapEnchantment { }


	public abstract class RogueClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => CalamityValues.rogue ?? DamageClass.Throwing;
		public override string CustomTooltip => CalamityIntegration.CALAMITY_NAME.Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
		public override string Designer => "Vyklade";
	}
	[Autoload(false)]
	public class RogueClassSwapEnchantmentBasic : RogueClassSwapEnchantment
	{
		public override SellCondition SellCondition => AndroMod.calamityEnabled ? SellCondition.AnyTimeRare : SellCondition.Never;
		public override List<ModDropData> ModNpcDropNames => AndroMod.calamityEnabled ? new() {
			new("CalamityMod/DesertScourgeHead")
		} : null;
	}
	[Autoload(false)]
	public class RogueClassSwapEnchantmentCommon : RogueClassSwapEnchantment { }
	[Autoload(false)]
	public class RogueClassSwapEnchantmentRare : RogueClassSwapEnchantment { }
	[Autoload(false)]
	public class RogueClassSwapEnchantmentEpic : RogueClassSwapEnchantment { }
	[Autoload(false)]
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
	[Autoload(false)]
	public class KiClassSwapEnchantmentBasic : KiClassSwapEnchantment
	{
		public override SellCondition SellCondition => WEMod.dbtEnabled ? SellCondition.AnyTimeRare : SellCondition.Never;
		public override List<DropData> NpcDropTypes => WEMod.dbtEnabled ? new() {
			new(NPCID.Golem, 9f)
		} : null;
	}
	[Autoload(false)]
	public class KiClassSwapEnchantmentCommon : KiClassSwapEnchantment { }
	[Autoload(false)]
	public class KiClassSwapEnchantmentRare : KiClassSwapEnchantment { }
	[Autoload(false)]
	public class KiClassSwapEnchantmentEpic : KiClassSwapEnchantment { }
	[Autoload(false)]
	public class KiClassSwapEnchantmentLegendary : KiClassSwapEnchantment { }

	public abstract class BardClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => ThoriumValues.bard ?? DamageClass.Melee;
		public override string CustomTooltip => ThoriumIntegration.THORIUM_NAME.Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
	}
	[Autoload(false)]
	public class BardClassSwapEnchantmentBasic : BardClassSwapEnchantment
	{
		public override SellCondition SellCondition => AndroMod.thoriumEnabled ? SellCondition.AnyTimeRare : SellCondition.Never;
		public override List<ModDropData> ModNpcDropNames => AndroMod.thoriumEnabled ? new() {
			new("ThoriumMod/QueenJelly")
		} : null;
	}
	[Autoload(false)]
	public class BardClassSwapEnchantmentCommon : BardClassSwapEnchantment { }
	[Autoload(false)]
	public class BardClassSwapEnchantmentRare : BardClassSwapEnchantment { }
	[Autoload(false)]
	public class BardClassSwapEnchantmentEpic : BardClassSwapEnchantment { }
	[Autoload(false)]
	public class BardClassSwapEnchantmentLegendary : BardClassSwapEnchantment { }

	public abstract class HealerClassSwapEnchantment : ClassSwapEnchantment
	{
		protected override DamageClass MyDamageClass => ThoriumValues.healerRadiation ?? DamageClass.Ranged;
		public override string CustomTooltip => ThoriumIntegration.THORIUM_NAME.Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentCustomTooltips);
	}
	[Autoload(false)]
	public class HealerClassSwapEnchantmentBasic : HealerClassSwapEnchantment
	{
		public override SellCondition SellCondition => AndroMod.thoriumEnabled ? SellCondition.AnyTimeRare : SellCondition.Never;
		public override List<ModDropData> ModNpcDropNames => AndroMod.thoriumEnabled ? new() {
			new("ThoriumMod/GraniteEnergyStorm"),
			new("ThoriumMod/TheBuriedWarrior")
		} : null;
	}
	[Autoload(false)]
	public class HealerClassSwapEnchantmentCommon : HealerClassSwapEnchantment { }
	[Autoload(false)]
	public class HealerClassSwapEnchantmentRare : HealerClassSwapEnchantment { }
	[Autoload(false)]
	public class HealerClassSwapEnchantmentEpic : HealerClassSwapEnchantment { }
	[Autoload(false)]
	public class HealerClassSwapEnchantmentLegendary : HealerClassSwapEnchantment { }
}
