using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using System.Reflection;
using Terraria.GameContent.Creative;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using static WeaponEnchantments.Common.Utility.LogModSystem;
using WeaponEnchantments.Common.Utility;
using static androLib.Common.EnchantingRarity;
using Terraria.Localization;
using System.Linq;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items.Enchantments.Utility;
using androLib.Common.Utility;
using androLib.Common.Globals;
using androLib.Items;
using androLib;
using WeaponEnchantments.Content.NPCs;
using androLib.Common.Utility.LogSystem;
using androLib.Items.Interfaces;

namespace WeaponEnchantments.Items
{
	public abstract class Enchantment : WEModItem, ISoldByNPC, IHasDropRates
	{
		#region Static

		public struct EnchantmentStrengths
		{
			public EnchantmentStrengths(float[] strengths) {
				enchantmentTierStrength = strengths;
			}

			public float[] enchantmentTierStrength = new float[tierNames.Length];
		}

		public static readonly EnchantmentStrengths[] defaultEnchantmentStrengths = new EnchantmentStrengths[] {
			new EnchantmentStrengths(new float[] { 0.04f, 0.08f, 0.16f, 0.28f, 0.40f }),
			new EnchantmentStrengths(new float[] { 0.00f, 0.04f, 0.04f, 0.04f, 0.24f }),
			new EnchantmentStrengths(new float[] { 1.2f, 1.4f, 1.6f, 1.8f, 2f }),
			new EnchantmentStrengths(new float[] { 1f, 2f, 3f, 5f, 10f }),
			new EnchantmentStrengths(new float[] { 2f, 4f, 6f, 10f, 20f }),
			new EnchantmentStrengths(new float[] { 0.005f, 0.01f, 0.015f, 0.02f, 0.025f }),
			new EnchantmentStrengths(new float[] { 4f, 5f, 6.5f, 8f, 10f }),
			new EnchantmentStrengths(new float[] { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f }),
			new EnchantmentStrengths(new float[] { 0.5f, 0.6f, 0.75f, 0.85f, 1f }),
			new EnchantmentStrengths(new float[] { 0.6f, 0.65f, 0.7f, 0.8f, 0.9f }),
			new EnchantmentStrengths(new float[] { 0.2f, 0.4f, 0.6f, 0.8f, 1f }),
			new EnchantmentStrengths(new float[] { 0.04f, 0.08f, 0.12f, 0.16f, 0.20f }),
			new EnchantmentStrengths(new float[] { 0.12f, 0.16f, 0.20f, 0.26f, 0.32f }),
			new EnchantmentStrengths(new float[] { 0.8f, 0.85f, 0.90f, 0.95f, 1f }),
			new EnchantmentStrengths(new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f }),
			new EnchantmentStrengths(new float[] { 0.8f, 0.6f, 0.4f, 0.2f, 0f }),
			new EnchantmentStrengths(new float[] { 0.05f, 0.1f, 0.15f, 0.2f, 0.25f }),
			new EnchantmentStrengths(new float[] { 0.88f, 0.91f, 0.94f, 0.97f, 1f }),
			new EnchantmentStrengths(new float[] { 0.5f, 0.75f, 1f, 1.5f, 2f }),
			new EnchantmentStrengths(new float[] { 1f, 2f, 3f, 4f, 5f }),
			new EnchantmentStrengths(new float[] { 0.06f, 0.07f, 0.08f, 0.09f, 0.1f }),
			new EnchantmentStrengths(new float[] { 0.08f, 0.2f, 0.5f, 1.2f, 2f }),
			new EnchantmentStrengths(new float[] { 0.8f, 1.6f, 2.4f, 3.2f, 4f }),
			new EnchantmentStrengths(new float[] { 1.2f, 1.28f, 1.36f, 1.42f, 1.5f }),
			new EnchantmentStrengths(new float[] { 100f, 150f, 200f, 250f, 300f }),
			new EnchantmentStrengths(new float[] { 0.2f, 0.05f, 0.025f, 0.01f / 0.7f, 0.01f }),
			new EnchantmentStrengths(new float[] { 0.02f, 0.04f, 0.06f, 0.08f, 0.12f }),
			new EnchantmentStrengths(new float[] { 12f, 16f, 24f, 32f, 40f })
		};//Need to manually update the StrengthGroup <summary> when changing defaultEnchantmentStrengths

		public static readonly uint defaultBuffDuration = 60;

		#endregion

		#region Strength

		public DifficultyStrength TierStrengthData;
		protected virtual bool UsesTierStrengthData => false;
		private DifficultyStrength _enchantmentStrengthData;
		public DifficultyStrength EnchantmentStrengthData {
			get => _enchantmentStrengthData;
			set => _enchantmentStrengthData = value;
		}
		public virtual float EnchantmentStrength => EnchantmentStrengthData.Value;
		public float TierPercent => ((float)EnchantmentTier + 1f) / 5f;

		/// <summary>
		/// Default 0<br/>
		/// Sets the EnchantmentStrengths for the enchantment.<br/>
		/// Example: LifeSteal is StrengthGroup 5.  Tier 1 Lifesteal's Enchantment strength is 0.01f.<br/><br/>
		/// <list>
		/// <term>0</term><description>{ 0.04f, 0.08f, 0.16f, 0.28f, 0.40f }</description><br/>
		/// <term>1</term><description>{ 0.00f, 0.04f, 0.06f, 0.06f, 0.15f }</description><br/>
		/// <term>2</term><description>{ 1.2f, 1.4f, 1.6f, 1.8f, 2f }</description><br/>
		/// <term>3</term><description>{ 1f, 2f, 3f, 5f, 10f }</description><br/>
		/// <term>4</term><description>{ 2f, 4f, 6f, 10f, 20f }</description><br/>
		/// <term>5</term><description>{ 0.005f, 0.01f, 0.015f, 0.02f, 0.025f }</description><br/>
		/// <term>6</term><description>{ 2f, 3f, 5f, 8f, 10f }</description><br/>
		/// <term>7</term><description>{ 0.02f, 0.04f, 0.06f, 0.08f, 0.10f }</description><br/>
		/// <term>8</term><description>{ 0.5f, 0.6f, 0.75f, 0.85f, 1f }</description><br/>
		/// <term>9</term><description>{ 0.6f, 0.65f, 0.7f, 0.8f, 0.9f }</description><br/>
		/// <term>10</term><description>{ 0.2f, 0.4f, 0.6f, 0.8f, 1f }</description><br/>
		/// <term>11</term><description>{ 0.04f, 0.08f, 0.12f, 0.16f, 0.20f }</description><br/>
		/// <term>12</term><description>{ 0.12f, 0.16f, 0.20f, 0.26f, 0.32f }</description><br/>
		/// <term>13</term><description>{ 0.8f, 0.85f, 0.90f, 0.95f, 1f }</description><br/>
		/// <term>14</term><description>{ 0.1f, 0.2f, 0.3f, 0.4f, 0.5f }</description><br/>
		/// <term>15</term><description>{ 0.8f, 0.6f, 0.4f, 0.2f, 0f }</description><br/>
		/// <term>16</term><description>{ 0.05f, 0.1f, 0.15f, 0.2f, 0.25f }</description><br/>
		/// <term>17</term><description>{ 0.88f, 0.91f, 0.94f, 0.97f, 1f }</description><br/>
		/// <term>18</term><description>{ 0.5f, 0.75f, 1f, 1.5f, 2f }</description><br/>
		/// <term>19</term><description>{ 1f, 2f, 3f, 4f, 5f }</description><br/>
		/// <term>20</term><description>{ 0.06f, 0.07f, 0.08f, 0.09f, 0.1f }</description><br/>
		/// <term>21</term><description>{ 0.8f, 0.2f, 0.5f, 1.2f, 2f }</description><br/>
		/// <term>22</term><description>{ 0.8f, 1.6f, 2.4f, 3.2f, 4f }</description><br/>
		/// <term>23</term><description>{ 1.2f, 1.28f, 1.36f, 1.42f, 1.5f }</description><br/>
		/// <term>24</term><description>{ 100f, 150f, 200f, 250f, 300f }</description><br/>
		/// <term>25</term><description>{ 0.2f, 0.05f, 0.025f, 0.01f / 0.7f, 0.01f }</description><br/>
		/// <term>26</term><description>{ 0.02f, 0.04f, 0.06f, 0.08f, 0.12f }</description><br/>
		/// <term>27</term><description>{ 12f, 16f, 24f, 32f, 40f }</description><br/>
		/// </list>
		/// </summary>
		public virtual int StrengthGroup { private set; get; } = 0;

		/// <summary>
		/// <para>
		/// Default 1f (All of the strength will be modified by the Recomended Strength Multiplier)<br/>
		/// Acceptable range: -1f to 1f<br/>
		/// Not required.  (Feel free to skip this and ask andro951 about it)<br/>
		/// Allows you to affect how much an enchantment is affected by the Recomended Strength multiplier from the config.<br/>
		/// </para><br/>
		/// <para>
		/// When would I need this? (This example is the Cold Steel damage multiplier at tier 4.  It desperatly needs a ScalePercent)<br/>
		/// The ScalePercent of Cold Steel is 2/9 = 0.22222.... for this example, I'm using 0.22f to make it a bit easier to read.<br/>
		/// <br/>
		/// Example: ScalePercent value of 0.2f, EnchantmentStrength of 0.9 (90%), RecomendedStrengthMultiplier of 0.25 (25%).<br/>
		///		Actual EnchantmentStrength value = ScalePercent * EnchantmentStrength * RecomendedStrengthMultiplier + (1f - ScalePercent) * EnchantmentStrength<br/>
		///		Actual EnchantmentStrength value = 0.22f * 0.9 * 0.25 + (1f - 0.22f) * 0.9f => 0.05f + 0.7f => 0.75f<br/>
		///	</para><br/>
		///	<para>
		///	If you did not set ScalePercent:<br/>
		///	Example: ScalePercent value of 1f, EnchantmentStrength of 0.9 (90%), RecomendedStrengthMultiplier of 0.25 (25%).<br/>
		///		Actual EnchantmentStrength value = ScalePercent * EnchantmentStrength * RecomendedStrengthMultiplier + (1f - ScalePercent) * EnchantmentStrength<br/>
		///		Actual EnchantmentStrength value = 1f * 0.9 * 0.25 + (1f - 1f) * 0.9f => 0.225f + 0f * 0.9f => 0.225f<br/>
		///	</para><br/>
		///	<para>
		/// Having a damage multiplier less than 1 on the Cold Steel enchantment is meant as a small nerf to offset the benefits of the enchantment.<br/>
		/// This number going from 0.9 (Normal config preset) => 0.75 (Master config preset) is a reasonable downgrade.<br/>
		/// If no ScalePercent was used, going from 0.9 (Normal config preset) => 0.225 (Master config preset) is a massive downgrade, making the enchantment worthless.<br/>
		/// <para><br/>
		/// </para>
		/// Note: To fix an issue with War and Peace, the ScalePercent can be set to a negative value.  Normally it should be 0f to 1f.<br/>
		/// Negative values are reverted to positive when calculating the Enchantment Strengths.  The only diference is it uses a different calculation.<br/>
		/// Negative values will cause 1f to be subtracted before calculating then add the 1f back after calculating.<br/>
		/// </para>
		/// </summary>
		public virtual float ScalePercent { private set; get; } = 1f;

		/// <summary>
		/// With Enchantments strength multiplier >= 1f, this will set the multiplier from ScalePercent back to 1f to allow the value going up above 100%, but not down below.
		/// </summary>
		public virtual bool OnlyApplyScalePercentBelow100 { private set; get; } = false;

		/// <summary>
		/// Allows you to manually adjust affect the cost of enchantments.
		/// Utility are 1f by default -> 1, 2, 3, 4, 5
		/// Normal are 2f by defualt -> 2, 4, 6, 8, 10
		/// Unique and Max1 are 3f by default -> 3, 6, 9, 12, 15
		/// Note: The null value I chose for this is == CapacityCostDefault (-13.13f)  That value will cause the defaults above to occur.
		/// </summary>
		public virtual float CapacityCostMultiplier { private set; get; } = CapacityCostDefault;
		private EItemType itemTypeAppliedOn = EItemType.None;
		public EItemType ItemTypeAppliedOn {
			get => itemTypeAppliedOn;
			set {
				AllowedListMultiplier = AllowedList.ContainsKey(value) ? AllowedList[value] : 1f;

				foreach (EnchantmentEffect effect in Effects) {
					effect.AllowedListMultiplier = AllowedListMultiplier;
				}

				itemTypeAppliedOn = value;
			}
		}
		private DamageClass damageClassAppliedOn = DamageClass.Default;
		public DamageClass DamageClassAppliedOn {
			get => damageClassAppliedOn;
			set {
				foreach (EnchantmentEffect effect in Effects) {
					effect.SetDamageClassMultiplier(value);
				}

				damageClassAppliedOn = value;
			}
		}
		public virtual float AllowedListMultiplier { protected set; get; }

		/// <summary>
		/// Default is { EItemType.Weapon, 1f }, { EItemType.Armor, 0.25f }, { EItemType.Accessory, 0.25f }<br/>
		/// (100% effective on weapons, 25% effective on armor and accessories)<br/>
		/// You must include ALL of the item types the enchantment can be applied on.  The above defaults are only set if you do not set the AllowedList.<br/>
		/// Example: Having just { EItemType.Weapon, 1f } will prevent the item being used on armor and accessories.<br/>
		/// </summary>
		public Dictionary<EItemType, float> AllowedList { protected set; get; }
		public virtual List<DropData> NpcDropTypes { protected set; get; } = null;
		public virtual List<ModDropData> ModNpcDropNames { protected set; get; } = null;
		public virtual List<DropData> NpcAIDrops { protected set; get; } = null;
		public virtual List<DropData> ChestDrops { protected set; get; } = null;
		public virtual List<DropData> CrateDrops { protected set; get; } = null;

		#endregion

		#region Identifiers and names

		/// <summary>
		/// A value 0 - 4 representing the enchantment's tier.
		/// </summary>
		public virtual int EnchantmentTier {
			get {
				if (enchantmentTier == -1)
					enchantmentTier = GetTierNumberFromName(Name);

				return enchantmentTier;
			}
		}
		private int enchantmentTier = -1;

		public string EnchantmentTypeName {
			get {
				if (enchantmentTypeName == null)
					enchantmentTypeName = Name.ToEnchantmentTypeName();

				return enchantmentTypeName;
			}
		}
		private string enchantmentTypeName;

		/// <summary>
		/// DO NOT CHANGE THIS UNLESS YOU ARE POSITIVE YOU ARE SUPPOSED TO!!!<br/>
		/// This is a temporary fix for enchantments that have less than 5 tiers (0 through 4).<br/>
		/// This reduces the tier of the enchantment for the purposes of calculating the enchantment value from the essence used to craft it only.<br/>
		/// Only the potion buff enchantments such as Spelunker currently set this.<br/>
		/// Default 0<br/>
		/// Acceptable Range -4 to 0 (Potion buffs set to -2).<br/>
		/// -4 would cause the value of the enchantment to be almost zero.<br/>
		/// </summary>
		public virtual int EnchantmentValueTierReduction { protected set; get; } = 0;
		public const int EssenceMultiplierUtility = 1;
		public const int EssenceMultiplierNormal = 2;
		public virtual int EssenceMultiplier => IngredientEnchantments == null ? essenceMultiplier : GetEssenceMultiplierIncludingIngredientEnchantments();
		private int essenceMultiplier => Utility ? EssenceMultiplierUtility : EssenceMultiplierNormal;
		public const int BaseEssenceQuantity = 5;
		public int EssenceQuantity => BaseEssenceQuantity * essenceMultiplier;
		public int EssenceQuantityWithIngredientEnchantments => BaseEssenceQuantity * EssenceMultiplier;

		private int GetEssenceMultiplierIncludingIngredientEnchantments() {
			int multiplier = essenceMultiplier;
			if (IngredientEnchantments != null) {
				foreach (Item item in IngredientEnchantments.Select(i => new Item(i))) {
					if (item.ModItem is Enchantment enchantment)
						multiplier += enchantment.EssenceMultiplier;
				}
			}

			return multiplier;
		}

		/// <summary>
		/// Default 1<br/>
		/// Acceptable Range 0 to 5<br/><br/>
		/// <list>
		/// <term>0</term><description>All tiers are craftable.</description><br/>
		/// <term>1</term><description>Tier 1 and above are craftable.</description><br/>
		/// <term>2</term><description>Tier 2 and above are craftable.</description><br/>
		/// <term>3</term><description>Tier 3 and above are craftable.</description><br/>
		/// <term>4</term><description>Tier 4 is craftable.</description><br/>
		/// <term>5</term><description>No tiers are craftable.</description><br/>
		/// </list>
		/// </summary>
		public virtual int LowestCraftableTier => HasIngredientEnchantments ? 1 : 1;//Separated only in case one of them changes

		/// <summary>
		/// This list should be the list of tier 0 enchantments that are used in the creation of this enchantment.<br/>
		/// </summary>
		public virtual List<int> IngredientEnchantments => null;
		public bool HasIngredientEnchantments => IngredientEnchantments != null && IngredientEnchantments.Count > 0;

		/// <summary>
		/// Not required.  Only include additional information to explain a complex enchantment.<br/>
		/// Static Stat, buff and debuff tooltips are all automatically generated.<br/>
		/// </summary>
		public virtual string CustomTooltip { protected set; get; } = "";
		public virtual string ShortTooltip => GetShortTooltip();
		public override LocalizedText Tooltip => LocalizedText.Empty;

		//public string FullToolTip { private set; get; }
		//public Dictionary<EItemType, string> AllowedListTooltips { private set; get; } = new Dictionary<EItemType, string>();

		public Func<int> SoldByNPCNetID => ModContent.NPCType<Witch>;
		public virtual SellCondition SellCondition => EnchantmentTier == 0 ? SellCondition.AnyTime : SellCondition.Never;
		public override List<WikiTypeID> WikiItemTypes {
			get {
				List<WikiTypeID> types = new() { WikiTypeID.Enchantments };
				if (EnchantmentTier < tierNames.Length - 1)
					types.Add(WikiTypeID.CraftingMaterial);

				return types;
			}
		}
		public override bool IsEquivenantForCondensingWikiCraftingRecipes(ModItem other) {
			if (other is not Enchantment enchantment)
				return false;

			return EnchantmentTier == enchantment.EnchantmentTier;
		}
		public override Type GroupingType => typeof(Enchantment);
		public virtual Func<ModItem, string, string> GetNonStandardWikiLinkString => (ModItem modItem, string name) => modItem is Enchantment enchantment ? enchantment.TierName.ToSectionLink(name, $"{enchantment.EnchantmentTypeName.AddSpaces()} Enchantment") : null;
		public override int CreativeItemSacrifice => 1;
		public string TierName => tierNames[EnchantmentTier];
		public override bool CanBeStoredInEnchantmentStroage => true;

		#endregion

		#region Restrictions and enchantment types

		/// <summary>
		/// Default false<br/>
		/// Automatically set to true if in the Utility folder.<br/>
		/// Note: It is actually based on the namespace, but the default namespace is the folder it's in.<br/>
		/// Manually changing the namespace will prevent this from being set.<br/>
		/// </summary>
		public bool Utility { private set; get; } = false;

		/// <summary>
		/// Default false<br/>
		/// Automatically set to true if in the Unique folder.<br/>
		/// Note: It is actually based on the namespace, but the default namespace is the folder it's in.<br/>
		/// Manually changing the namespace will prevent this from being set.<br/>
		/// </summary>
		public bool Unique { private set; get; } = false;

		/// <summary>
		/// Default false<br/>
		/// True will prevent more than 1 of this enchantment from being applied to an item.<br/>
		/// </summary>
		public virtual bool Max1 { private set; get; } = false;

		/// <summary>
		/// Default 0<br/>
		/// Makes an enchantment only allowed on weapons with the specified damage type.<br/>
		/// Please use the DamageTypeSpecificID enum for this.<br/>
		/// Example: DamageClassSpecific => (int)DamageTypeSpecificID.Melee<br/><br/>
		/// <list>
		/// <term>0</term><description>Generic</description><br/>
		/// <term>1</term><description>Melee</description><br/>
		/// <term>2</term><description>MeleeNoSpeed</description><br/>
		/// <term>3</term><description>Ranged</description><br/>
		/// <term>4</term><description>Magic</description><br/>
		/// <term>5</term><description>Summon</description><br/>
		/// <term>6</term><description>SummonMeleeSpeed</description><br/>
		/// <term>7</term><description>MagicSummonHybrid</description><br/>
		/// <term>8</term><description>Throwing</description><br/>
		/// </list>
		/// </summary>
		public virtual int DamageClassSpecific { private set; get; } = 0;

		/// <summary>
		/// Default -1<br/>
		/// Makes an enchantment only allowed on armor of the specified type.<br/>
		/// Please use the ArmorSlotSpecificID enum for this.<br/>
		/// Example: ArmorSlotSpecific => (int)ArmorSlotSpecificID.Head<br/><br/>
		/// <list>
		/// <term>0</term><description>Head</description><br/>
		/// <term>1</term><description>Body</description><br/>
		/// <term>2</term><description>Legs</description><br/>
		/// </list>
		/// </summary>
		public virtual int ArmorSlotSpecific { private set; get; } = -1;

		/// <summary>
		/// Default -1<br/>
		/// Prevents an enchantment from being applied on weapons with the specified damage type.<br/>
		/// Please use the DamageTypeSpecificID enum for this.<br/>
		/// Example: RestrictedClass => (int)DamageTypeSpecificID.Melee<br/><br/>
		/// <list>
		/// <term>0</term><description>Generic</description><br/>
		/// <term>1</term><description>Melee</description><br/>
		/// <term>2</term><description>MeleeNoSpeed</description><br/>
		/// <term>3</term><description>Ranged</description><br/>
		/// <term>4</term><description>Magic</description><br/>
		/// <term>5</term><description>Summon</description><br/>
		/// <term>6</term><description>SummonMeleeSpeed</description><br/>
		/// <term>7</term><description>MagicSummonHybrid</description><br/>
		/// <term>8</term><description>Throwing</description><br/>
		/// </list>
		/// </summary>
		public virtual List<int> RestrictedClass { private set; get; } = new();

		#endregion

		#region Stats and buffs

		private bool finishedOneTimeSetup = false;
		public uint BuffDuration => GetBuffDuration();
		public List<EnchantmentEffect> Effects { protected set; get; } = new List<EnchantmentEffect>() { };

		#endregion
		public override string Texture => $"WeaponEnchantments/Items/Sprites/{Name}";

		/// <summary>
		/// Add to the stat Lists and Dictionaries here.<br/>
		/// If no changes are made here, the default stat will be: AddEStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength)<br/>
		/// <br/>
		/// Buff.Add(int)<br/>
		/// OnHitBuff.Add(int, int)<br/>
		/// Debuff.Add(int, int)<br/>
		/// AddStaticStat(string name, float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f)<br/>
		/// AddEstat(string name, float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f)<br/>
		/// <br/>
		/// Automated methods to add a stat based on the enchantment name:<br/>
		/// CheckStaticStatByName()<br/>
		/// CheckBuffByName()<br/>
		/// </summary>
		public virtual void GetMyStats() { } //Meant to be overriden in the specific Enchantment class.
		public override ModItem Clone(Item newEntity) {
			Enchantment clone = (Enchantment)base.Clone(newEntity);
			//clone.EnchantmentStrengthData = EnchantmentStrengthData.Clone();
			string name = Name;
			clone.Effects = Effects.Select(e => e.Clone()).ToList();

			return clone;
		}
		public override void SetStaticDefaults() {
			//Get values needed to generate tooltips
			GetDefaults();// true);//Change this to have arguments to only get the needed info for setting up tooltips.

			//DisplayName
			//string typeNameString = "Mods.WeaponEnchantments.EnchantmentTypeNames." + EnchantmentTypeName;
			//typeNameString.Log();
			//string displayName = Language.GetTextValue(typeNameString) + " " + Language.GetTextValue("Mods.WeaponEnchantments.Enchantment");
			/*if (WEMod.clientConfig.UseOldTierNames) {
				//Old rarity names, "Basic", "Common", "Rare", "Epic", "Legendary"
				//string rarityString = "Mods.WeaponEnchantments.TierNames." + displayTierNames[EnchantmentTier];
				//rarityString.Log();
				//DisplayName.SetDefault(displayName + " " + Language.GetTextValue(rarityString));
				DisplayName.SetDefault(StringManipulation.AddSpaces(MyDisplayName + Name.Substring(Name.IndexOf("Enchantment"))));
			}
			else {
				//Current rarity names, "Basic", "Common", "Rare", "Epic", "Legendary"
				//string rarityString = "Mods.WeaponEnchantments.DisplayTierNames." + displayTierNames[EnchantmentTier];
				//rarityString.Log();
				//DisplayName.SetDefault(displayName + " " + Language.GetTextValue(rarityString));
				DisplayName.SetDefault(StringManipulation.AddSpaces(MyDisplayName + "Enchantment" + displayTierNames[EnchantmentTier]));
			}*/

			if (printListOfContributors && (EnchantmentTier == 1 || EnchantmentTypeName == "AllForOne")) {
				//All for one is allowed to pass every sprite
				bool allForOne = EnchantmentTypeName == "AllForOne";

				UpdateContributorsList(this, allForOne ? null : EnchantmentTypeName);
			}

			if (AndroMod.wikiThis != null)
				AndroMod.wikiThis.Call(1, Type, GetWikiURL());

			base.SetStaticDefaults();
		}
		private void GetDefaults() {
			//Check Utility
			if (GetType().Namespace.GetFolderName() == "Utility")
				Utility = true;

			//Check Unique
			if (GetType().Namespace.GetFolderName() == "Unique")
				Unique = true;

			//Item rarity
			Item.rare = GetRarityFromTier(EnchantmentTier);

			//Width and Height
			switch (EnchantmentTier) {
				case 3:
					Item.width = 44;
					Item.height = 40;
					break;
				case 4:
					Item.width = 40;
					Item.height = 40;
					break;
				default:
					Item.width = 28 + 4 * (EnchantmentTier);
					Item.height = 28 + 4 * (EnchantmentTier);
					break;
			}

			//Value - Essence
			for (int i = 0; i <= EnchantmentTier + EnchantmentValueTierReduction; i++) {
				int value = (int)EnchantmentEssence.values[i];
				Item.value += value * EssenceQuantityWithIngredientEnchantments;
			}

			//Value - Containment/SuperiorStaibalizers
			switch (EnchantmentTier) {
				case 3:
					Item.value += ContainmentItem.Values[2];
					break;
				case 4:
					Item.value += ContentSamples.ItemsByType[999].value;
					break;
				default:
					Item.value += ContainmentItem.Values[EnchantmentTier];
					break;
			}

			SetEnchantmentStrength();

			//Only check once
			if (finishedOneTimeSetup)
				return;

			GetMyStats();

			if (AllowedList == null || AllowedList.Count == 0) {
				AllowedList = new Dictionary<EItemType, float>() {
					{ EItemType.Weapons, 1f },
					{ EItemType.Armor, 0.25f },
					{ EItemType.Accessories, 0.25f }
				};

				if (Utility) {
					AllowedList.Add(EItemType.FishingPoles, 1f);
					AllowedList.Add(EItemType.Tools, 1f);
				}
			}

			if (RemoveEnchantmentRestrictions) {
				foreach(EItemType eItemType in Enum.GetValues(typeof(EItemType))) {
					if (!AllowedList.Keys.Contains(eItemType))
						AllowedList.Add(eItemType, 1f);
				}
			}

			finishedOneTimeSetup = true;
		}
		public override void SetDefaults() {
			Item.maxStack = 99;
			GetDefaults();
		}
		public void SetEnchantmentStrength() {//Config - Individual Strength
			bool foundIndividualStrength = false;
			float[] strengths = new float[1];
			float[] tierStrengths = new float[1];
			float tierStrengthPercentage = ((float)EnchantmentTier + 1f) / (float)defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength.Length;
			//Config - Individual Strength Multipliers
			if (WEMod.serverConfig.individualStrengthsEnabled && WEMod.serverConfig.individualStrengths.Count > 0) {
				foreach (Pair pair in WEMod.serverConfig.individualStrengths) {
					if (pair.itemDefinition.Name == Name) {
						strengths[0] = (float)pair.Strength / 1000f;
						if (UsesTierStrengthData)
							tierStrengths[0] = tierStrengthPercentage * strengths[0] / defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[EnchantmentTier];

						foundIndividualStrength = true;
					}
				}
			}

			//Config - Global Enchantment Strength Multipliers
			if (!foundIndividualStrength) {
				bool foundRarityStrength = false;
				float rarityMultiplier = RarityEnchantmentStrengthMultipliers[EnchantmentTier];
				if (rarityMultiplier >= 0f) {
					foundRarityStrength = true;
					strengths[0] = GetStrengthApplyScalePercent(rarityMultiplier);
					if (UsesTierStrengthData)
						tierStrengths[0] = tierStrengthPercentage * rarityMultiplier;
				}

				if (!foundRarityStrength) {
					if (WEMod.serverConfig.presetData.AutomaticallyMatchPreseTtoWorldDifficulty) {
						strengths = new float[4];
						tierStrengths = new float[4];
					}

					int count = strengths.Length;
					for (int i = 0; i < count; i++) {
						//Global
						float multiplier = count == 1 ? GlobalStrengthMultiplier : PresetMultipliers[i];

						//Apply Scale Percent
						strengths[i] = GetStrengthApplyScalePercent(multiplier);

						if (UsesTierStrengthData)
							tierStrengths[i] = tierStrengthPercentage * multiplier;
					}
				}
			}

			TierStrengthData = new DifficultyStrength(tierStrengths);
			EnchantmentStrengthData = new DifficultyStrength(strengths);
		}
		private float GetStrengthApplyScalePercent(float multiplier) {
			float defaultStrength = defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[EnchantmentTier];
			float scale = Math.Abs(ScalePercent);
			if (multiplier > 1f && OnlyApplyScalePercentBelow100)
				scale = 1f;

			float strength;
			if (ScalePercent < 0f) {
				strength = 1f + (1f - scale) * (defaultStrength - 1f) + (defaultStrength - 1f) * multiplier * scale;
			}
			else {
				strength = (1f - scale) * defaultStrength + defaultStrength * multiplier * scale;
			}

			return strength;
		}
		protected string GetShortTooltip(bool showValue = true, bool percent = true, bool sign = false, bool multiply100 = true, bool multiplicative = false, string text = null) {
			string s = "";
			if (showValue) {
				float strength = EnchantmentStrength * AllowedListMultiplier;
				if (multiply100)
					strength *= 100f;

				if (sign)
					s += strength < 0f ? "" : "+";

				if (multiplicative)
					s += "x";

				s += strength.S();
				if (percent)
					s += "%";

				s += " ";
			}

			s += text ?? GetLocalizationTypeName();

			if (!showValue)
				s += $" {EnchantmentTier}";

			return s;
		}
		protected string GetLocalizationTypeName(string s = null, IEnumerable<object> args = null) => (s ?? EnchantmentTypeName).Lang_WE(L_ID1.Tooltip, L_ID2.EffectDisplayName, args);
		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			var tooltipTuples = GenerateFullTooltip();
			foreach (var tooltipTuple in tooltipTuples) {
				tooltips.Add(new TooltipLine(Mod, "enchantment:base", tooltipTuple.Item1) { OverrideColor = tooltipTuple.Item2 });
			}
		}
		public IEnumerable<Tuple<string, Color>> GenerateFullTooltip() {
			List<Tuple<string, Color>> fullTooltip = new List<Tuple<string, Color>>();

			if (CustomTooltip != "")
				fullTooltip.Add(new Tuple<string, Color>(CustomTooltip, Color.White));//, Color.DarkGray));

			//fullTooltip.Add(new Tuple<string, Color>("Effects:", Color.Violet));
			fullTooltip.AddRange(GetEffectsTooltips());

			fullTooltip.Add(new Tuple<string, Color>(GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.LevelCost, GetCapacityCost()), Color.LightGreen));

			//Unique
			if (Unique)
				fullTooltip.Add(new Tuple<string, Color>($"   *{GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.Unique)}*", Color.White));

			fullTooltip.AddRange(GetAllowedListTooltips());

			//For making enchantments only allowed on 1 specific weapon with the same name
			/*if (AllowedList.ContainsKey(EItemType.Weapons) && Unique && !Max1 && DamageClassSpecific == 0 && ArmorSlotSpecific == -1 && RestrictedClass?.Count == 0 && Utility == false) {
				//Unique (Specific Item)
				fullTooltip.Add(new Tuple<string, Color>(
					$"   *{GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.Only, GetLocalizationTypeName())}*",
					Color.White
				));
			}
			else */
			if (DamageClassSpecific > 0) {
				//DamageClassSpecific
				string damageClassName = GetDamageClassName(DamageClassSpecific);
				fullTooltip.Add(new Tuple<string, Color>(
					$"   *{GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.Only, damageClassName)}*",
					Color.White
				));
			}
			else if (ArmorSlotSpecific > -1) {
				//ArmorSlotSpecific
				fullTooltip.Add(new Tuple<string, Color>(
					$"   *{GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.ArmorSlotOnly, (ArmorSlotSpecificID)ArmorSlotSpecific)}*",
					Color.White
				));
			}

			//RestrictedClass
			if (RestrictedClass.Count > 0) {
				fullTooltip.Add(new Tuple<string, Color>(
					$"   *{GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.NotAllowed, RestrictedClass.Select(c => GetDamageClassName(c)).JoinList(", ", $" {GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.Or)} "))}*",
					Color.White
				));
			}

			if (Max1)
				fullTooltip.Add(new Tuple<string, Color>($"   *{GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.Max1)}*", Color.White));

			if (Utility)
				fullTooltip.Add(new Tuple<string, Color>($"   *{GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.Utility)}*", Color.White));

			return fullTooltip;
		}
		private static string GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID id, object arg = null) => id.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips, new object[] { arg });
		public IEnumerable<Tuple<string, Color>> GetAllowedListTooltips() {
			string tooltip = "";
			int count = AllowedList.Count;
			if (AllowedList.Count > 0) {
				int i = 0;
				bool first = true;
				foreach (EItemType key in AllowedList.Keys) {
					if (first) {
						if (count < 3) {
							tooltip += $"   *{GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.OnlyAllowedOn)} ";
						}
						else {
							tooltip += $"   *{GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.AllowedOn)} ";
						}
						first = false;
					}
					else if (i == count - 1) {
						tooltip += $" {GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.And)} ";
					}
					else {
						tooltip += ", ";
					}

					tooltip += $"{key.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.ItemType)}: {AllowedList[key].Percent()}%";

					i++;
					if (i == count) {
						tooltip += "*";
					}
				}
			}

			return new List<Tuple<string, Color>>() { new Tuple<string, Color>(tooltip, Color.White) };
		}
		public IEnumerable<Tuple<string, Color>> GetEffectsTooltips() {
			List<Tuple<string, Color>> tooltips = new List<Tuple<string, Color>>();
			foreach (var effect in Effects) {
				if (!effect.showTooltip || effect.Tooltip == "")
					continue;

				tooltips.Add(new Tuple<string, Color>(effect.Tooltip, effect.TooltipColor));
			}

			return tooltips;
		}
		public static int GetDamageClass(int damageType) {

			switch (damageType) {
				case (int)DamageClassID.Melee:
				case (int)DamageClassID.MeleeNoSpeed:
					return (int)DamageClassID.Melee;
				case (int)DamageClassID.Summon:
				case (int)DamageClassID.MagicSummonHybrid:
					return (int)DamageClassID.Summon;
				default:
					if (AndroMod.calamityEnabled) {
						if (damageType == ModIntegration.CalamityValues.trueMelee.Type || damageType == ModIntegration.CalamityValues.trueMeleeNoSpeed.Type)
							return (int)DamageClassID.Melee;
					}

					return damageType;
			}
		}
		public static string GetDamageClassName(int type) {
			int damageType = GetDamageClass(type);

			if (damageType <= (int)DamageClassID.Default)
				return DamageClassID.Generic.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.DamageClassNames);

			switch (damageType) {
				case (int)DamageClassID.MagicSummonHybrid:
					return DamageClassID.Summon.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.DamageClassNames);
				case (int)DamageClassID.MeleeNoSpeed:
					return DamageClassID.Melee.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.DamageClassNames);
			}

			if (damageType <= (int)DamageClassID.Throwing)
				return ((DamageClassID)damageType).ToString().Lang_WE(L_ID1.Tooltip, L_ID2.DamageClassNames);

			if (AndroMod.calamityEnabled) {
				int rogue = ModIntegration.CalamityValues.rogue.Type;
				if (damageType == rogue)
					return DamageClassID.Rogue.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.DamageClassNames);
			}

			return DamageClassID.Generic.ToString().Lang_WE(L_ID1.Tooltip, L_ID2.DamageClassNames);
		}
		private uint GetBuffDuration() {
			return defaultBuffDuration * ((uint)EnchantmentTier + 1);
		}
		public override void AddRecipes() {
			bool hasIngredientEnchantments = HasIngredientEnchantments;
			//Ingredient Enchantment Recipe
			if (hasIngredientEnchantments) {
				Recipe ingredientEnchantmentRecipe = CreateRecipe();
				//Essence
				ingredientEnchantmentRecipe.AddIngredient(Mod, "EnchantmentEssence" + tierNames[EnchantmentTier], EssenceQuantity);

				//Ingredient Enchantments
				foreach (int ingredientEnchantment in IngredientEnchantments) {
					Item enchantmentItem = new Item(ingredientEnchantment);
					if (enchantmentItem.ModItem is Enchantment enchantment) {
						string typeName = enchantment.EnchantmentTypeName;
						ingredientEnchantmentRecipe.AddIngredient(Mod, typeName + "Enchantment" + tierNames[EnchantmentTier], 1);
					}
				}

				//Enchanting Table
				ingredientEnchantmentRecipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[EnchantmentTier] + "EnchantingTable");

				ingredientEnchantmentRecipe.DisableDecraft();

				ingredientEnchantmentRecipe.Register();
			}

			for (int j = LowestCraftableTier; j <= EnchantmentTier; j++) {
				if (!useAllRecipes && j != EnchantmentTier)
					continue;

				Recipe recipe = CreateRecipe();

				//Essence
				for (int k = j; k <= EnchantmentTier; k++) {
					recipe.AddIngredient(Mod, "EnchantmentEssence" + tierNames[k], EssenceQuantityWithIngredientEnchantments);
				}

				//Enchantment
				if (j > 0) {
					recipe.AddIngredient(Mod, EnchantmentTypeName + "Enchantment" + tierNames[j - 1], 1);
				}

				//Containment
				if (EnchantmentTier < 3) {
					recipe.AddIngredient(Mod, ContainmentItem.sizes[EnchantmentTier] + "Containment", 1);
				}
				else if (j < 3) {
					recipe.AddIngredient(Mod, ContainmentItem.sizes[2] + "Containment", 1);
				}

				//Gems
				if (EnchantmentTier == 3) {
					recipe.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.AnyCommonGem}", 2);
				}
				if (EnchantmentTier == 4) {
					recipe.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.AnyRareGem}");
				}

				//Enchanting Table
				recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[EnchantmentTier] + "EnchantingTable");

				if (j == 0)
					EditTier0Recipies(recipe);

				EditRecipe(recipe);

				if (j != 0)
					recipe.DisableDecraft();

				recipe.Register();
			}

			if (!WEMod.clientConfig.AllowCraftingIntoLowerTier || EnchantmentValueTierReduction != 0)
				return;

			for (int j = EnchantmentTier + 1; j < tierNames.Length; j++) {
				if (!useAllRecipes && j != EnchantmentTier + 1)
					continue;

				Recipe recipe = CreateRecipe();

				//Enchantment
				recipe.AddIngredient(Mod, EnchantmentTypeName + "Enchantment" + tierNames[j], 1);

				//Containment
				if (EnchantmentTier < 2) {
					recipe.AddIngredient(Mod, ContainmentItem.sizes[EnchantmentTier] + "Containment", 1);
				}

				//Enchanting Table
				recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[EnchantmentTier] + "EnchantingTable");

				//Gems
				if (EnchantmentTier == 3)
					recipe.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.AnyCommonGem}", 2);

				recipe.DisableDecraft();

				recipe.Register();
			}

			//Basic Essence Recipe
			Recipe basicEssenceRecipe = CreateRecipe();

			//Basic Essence
			int essenceTier = hasIngredientEnchantments ? EnchantmentTier : 0;
			ModItem essenceItem = ModContent.Find<ModItem>(WEMod.ModName, "EnchantmentEssence" + tierNames[essenceTier]);
			basicEssenceRecipe.createItem = new(essenceItem.Type, EssenceQuantity);

			//This enchantment
			basicEssenceRecipe.AddIngredient(Type);

			//Ingredient Enchantments
			if (hasIngredientEnchantments) {
				//Containments from the other enchantments
				int igredientEnchantmentCount = IngredientEnchantments.Count - 1;
				int containmentTier = EnchantmentTier < 3 ? EnchantmentTier : 2;
				basicEssenceRecipe.AddIngredient(Mod, ContainmentItem.sizes[containmentTier] + "Containment", igredientEnchantmentCount);

				//Gems
				if (EnchantmentTier == 3) {
					basicEssenceRecipe.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.AnyCommonGem}", 2 * igredientEnchantmentCount);
				}
				if (EnchantmentTier == 4) {
					basicEssenceRecipe.AddRecipeGroup($"{AndroMod.ModName}:{AndroModSystem.AnyRareGem}", igredientEnchantmentCount);
				}
			}

			//Enchanting Table
			basicEssenceRecipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[0] + "EnchantingTable");

			basicEssenceRecipe.DisableDecraft();

			basicEssenceRecipe.Register();
		}

		/// <summary>
		/// Allows for editing recipies in any way.  Called for every recipe.
		/// </summary>
		/// <param name="recipe"></param>
		protected virtual void EditRecipe(Recipe recipe) { }

		/// <summary>
		/// Edits all recipies that include the requirements for creating the tier 0 enchantment.
		/// </summary>
		/// <param name="recipe"></param>
		protected virtual void EditTier0Recipies(Recipe recipe) { }

		public const float CapacityCostDefault = -13.13f;
		public const float CapacityCostNone = 0f;
		public const float CapacityCostUtility = 1f;
		public const float CapacityCostNormal = 2f;
		public const float CapacityCostUnique = 3f;
		public int GetCapacityCost() {
			float multiplier;
			if (CapacityCostMultiplier != CapacityCostDefault) {
				//multiplier is being manually set by this enchantment
				multiplier = CapacityCostMultiplier;
			}
			else {
				multiplier = CapacityCostNormal;

				if (Utility)
					multiplier = CapacityCostUtility;

				if (Unique || Max1)
					multiplier = CapacityCostUnique;
			}

			return (int)Math.Round((1f + EnchantmentTier) * multiplier * ConfigCapacityCostMultiplier);
		}
		public bool SameAs(Item other) {
			if (Item.stack != other.stack)
				return false;

			return CanStack(other);
		}
		public override bool CanStack(Item item2) {
			if (Item.type != item2.type)
				return false;

			ModItem otherModItem = item2.ModItem;
			if (otherModItem == null)
				return false;

			if (otherModItem is not Enchantment otherEnchantment)
				return false;

			if (otherEnchantment.Effects.Count != Effects.Count)
				return false;

			EnchantmentStat[] statEffects = Effects.OfType<StatEffect>().Select(statEffect => statEffect.statName).ToArray();
			EnchantmentStat[] otherStatEffects = otherEnchantment.Effects.OfType<StatEffect>().Select(statEffect => statEffect.statName).ToArray();
			if (statEffects.Length != otherStatEffects.Length)
				return false;

			foreach (EnchantmentStat statName in statEffects) {
				if (!otherStatEffects.Contains(statName))
					return false;
			}

			return true;
		}
		public string GetWikiURL() {
			string underscoreName = Name.AddSpaces(space: "_");
			for (int i = Name.Length - 1; i >= 0; i--) {
				if (underscoreName[i] == '_') {
					//Replace the last underscore with a #
					underscoreName = underscoreName.Remove(i, 1).Insert(i, "#");
					break;
				}
			}

			return WEMod.WIKI_URL + underscoreName;
		}
	}
}
