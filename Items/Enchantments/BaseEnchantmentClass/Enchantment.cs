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
using static WeaponEnchantments.Common.EnchantingRarity;
using Terraria.Localization;
using System.Linq;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Common.Globals;

namespace WeaponEnchantments.Items
{
	public abstract class Enchantment : WEModItem, ISoldByWitch
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
			new EnchantmentStrengths(new float[] { 0.03f, 0.08f, 0.16f, 0.25f, 0.40f }),
			new EnchantmentStrengths(new float[] { 0.00f, 0.04f, 0.04f, 0.04f, 0.24f }),
			new EnchantmentStrengths(new float[] { 1.2f, 1.4f, 1.6f, 1.8f, 2f }),
			new EnchantmentStrengths(new float[] { 1f, 2f, 3f, 5f, 10f }),
			new EnchantmentStrengths(new float[] { 2f, 4f, 6f, 10f, 20f }),
			new EnchantmentStrengths(new float[] { 0.005f, 0.01f, 0.015f, 0.02f, 0.025f }),
			new EnchantmentStrengths(new float[] { 2f, 3f, 5f, 8f, 10f }),
			new EnchantmentStrengths(new float[] { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f }),
			new EnchantmentStrengths(new float[] { 0.5f, 0.6f, 0.75f, 0.85f, 1f }),
			new EnchantmentStrengths(new float[] { 0.6f, 0.65f, 0.7f, 0.8f, 0.9f }),
			new EnchantmentStrengths(new float[] { 0.2f, 0.4f, 0.6f, 0.8f, 1f }),
			new EnchantmentStrengths(new float[] { 0.04f, 0.08f, 0.12f, 0.16f, 0.20f }),
			new EnchantmentStrengths(new float[] { 0.12f, 0.16f, 0.20f, 0.25f, 0.32f }),
			new EnchantmentStrengths(new float[] { 0.8f, 0.85f, 0.90f, 0.95f, 1f }),
			new EnchantmentStrengths(new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f }),
			new EnchantmentStrengths(new float[] { 0.8f, 0.6f, 0.4f, 0.2f, 0f }),
			new EnchantmentStrengths(new float[] { 0.05f, 0.1f, 0.15f, 0.2f, 0.25f }),
			new EnchantmentStrengths(new float[] { 0.88f, 0.91f, 0.94f, 0.97f, 1f }),
			new EnchantmentStrengths(new float[] { 0.5f, 0.75f, 1f, 1.5f, 2f }),
			new EnchantmentStrengths(new float[] { 1f, 2f, 3f, 4f, 5f }),
			new EnchantmentStrengths(new float[] { 0.06f, 0.07f, 0.08f, 0.09f, 0.1f }),
			new EnchantmentStrengths(new float[] { 0.08f, 0.2f, 0.5f, 1.2f, 2f }),
			new EnchantmentStrengths(new float[] { 0.8f, 1.6f, 2.4f, 3.2f, 4f })
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
		public float EnchantmentStrength => EnchantmentStrengthData.Value;
		public float TierPercent => ((float)EnchantmentTier + 1f) / 5f;

		/// <summary>
		/// Default 0<br/>
		/// Sets the EnchantmentStrengths for the enchantment.<br/>
		/// Example: LifeSteal is StrengthGroup 5.  Tier 1 Lifesteal's Enchantment strength is 0.01f.<br/><br/>
		/// <list>
		/// <term>0</term><description>{ 0.03f, 0.08f, 0.16f, 0.25f, 0.40f }</description><br/>
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
		/// <term>12</term><description>{ 0.14f, 0.18f, 0.22f, 0.26f, 0.30f }</description><br/>
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
		/// Allows you to manually adjust affect the cost of enchantments.
		/// Utility are 1f by default -> 1, 2, 3, 4, 5
		/// Normal are 2f by defualt -> 2, 4, 6, 8, 10
		/// Unique and Max1 are 3f by default -> 3, 6, 9, 12, 15
		/// Note: The null value I chose for this is -13.13f  That value will cause the defaults above to occur.
		/// </summary>
		public virtual float CapacityCostMultiplier { private set; get; } = -13.13f;
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
		public virtual List<WeightedPair> NpcDropTypes { protected set; get; } = null;
		public virtual List<WeightedPair> NpcAIDrops { protected set; get; } = null;
		public virtual SortedDictionary<ChestID, float> ChestDrops { protected set; get; } = null;
		public virtual List<WeightedPair> CrateDrops { protected set; get; } = null;

		#endregion

		#region Identifiers and names

		/// <summary>
		/// A value 0 - 4 representing the enchantment's tier.
		/// </summary>
		public virtual int EnchantmentTier {
			get {
				if (enchantmentTier == -1) {
					enchantmentTier = GetTierNumberFromName(Name);
				}

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

		/// <summary>
		/// Default 1<br/>
		/// Acceptable Range 1 to 5<br/><br/>
		/// <list>
		/// <term>0</term><description>All tiers are craftable.</description><br/>
		/// <term>1</term><description>Tier 1 and above are craftable.</description><br/>
		/// <term>2</term><description>Tier 2 and above are craftable.</description><br/>
		/// <term>3</term><description>Tier 3 and above are craftable.</description><br/>
		/// <term>4</term><description>Tier 4 is craftable.</description><br/>
		/// <term>5</term><description>No tiers are craftable.</description><br/>
		/// </list>
		/// </summary>
		public virtual int LowestCraftableTier { protected set; get; } = 1;

		/// <summary>
		/// Not required.  Only include additional information to explain a complex enchantment.<br/>
		/// Static Stat, buff and debuff tooltips are all automatically generated.<br/>
		/// </summary>
		public virtual string CustomTooltip { protected set; get; } = "";
		public virtual string ShortTooltip => GetShortTooltip();
		public string StoredShortTooltip {
			get {
				if (shortTooltip == null) {
					shortTooltip = ShortTooltip;
				}

				return shortTooltip;
			}
		}
		private string shortTooltip;

		//public string FullToolTip { private set; get; }
		//public Dictionary<EItemType, string> AllowedListTooltips { private set; get; } = new Dictionary<EItemType, string>();

		public virtual SellCondition SellCondition => EnchantmentTier == 0 ? SellCondition.AnyTime : SellCondition.Never;
		public override List<WikiTypeID> WikiItemTypes {
			get {
				List<WikiTypeID> types = new() { WikiTypeID.Enchantments };
				if (EnchantmentTier < tierNames.Length - 1)
					types.Add(WikiTypeID.CraftingMaterial);

				return types;
			}
		}
		public override bool DynamicTooltip => true;
		public override int CreativeItemSacrifice => 1;
		public string TierName => tierNames[EnchantmentTier];

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
		public List<int> Buff { private set; get; } = new List<int>();
		public Dictionary<int, int> OnHitBuff { private set; get; } = new Dictionary<int, int>();
		public Dictionary<short, int> Debuff { private set; get; } = new Dictionary<short, int>();
		public List<EnchantmentStaticStat> StaticStats { private set; get; } = new List<EnchantmentStaticStat>();
		public List<EStat> EStats { private set; get; } = new List<EStat>();
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

			if (NpcDropTypes != null) {
				foreach (WeightedPair pair in NpcDropTypes) {
					int npcType = pair.ID;
					WeightedPair enchantmentPair = new WeightedPair(Type, pair.Weight);
					WEGlobalNPC.npcDropTypes.AddOrCombine(npcType, enchantmentPair);
				}
			}

			if (NpcAIDrops != null) {
				foreach (WeightedPair pair in NpcAIDrops) {
					int npcAIStyle = pair.ID;
					WeightedPair enchantmentPair = new WeightedPair(Type, pair.Weight);
					WEGlobalNPC.npcAIDrops.AddOrCombine(npcAIStyle, enchantmentPair);
				}
			}

			if (ChestDrops != null) {
				foreach (KeyValuePair<ChestID, float> pair in ChestDrops) {
					ChestID chestID = pair.Key;
					WeightedPair enchantmentPair = new WeightedPair(Type, pair.Value);
					WEModSystem.chestDrops.AddOrCombine(chestID, enchantmentPair);
				}
			}

			if (CrateDrops != null) {
				foreach (WeightedPair pair in CrateDrops) {
					int crateID = pair.ID;
					WeightedPair enchantmentPair = new WeightedPair(Type, pair.Weight);
					GlobalCrates.crateDrops.AddOrCombine(crateID, enchantmentPair);
				}
			}

			base.SetStaticDefaults();
		}
		private void GetDefaults() {
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
				int quantity = Utility ? 5 : 10;
				int value = (int)EnchantmentEssence.values[i];
				Item.value += value * quantity;
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

			//Check Utility
			if (GetType().Namespace.GetFolderName() == "Utility")
				Utility = true;

			//Check Unique
			if (GetType().Namespace.GetFolderName() == "Unique")
				Unique = true;

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

			//Default Stat
			//if (StaticStats.Count < 1 && EStats.Count < 1 && Buff.Count < 1 && Debuff.Count < 1 && OnHitBuff.Count < 1) {
			//	AddEStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength);
			//}

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
						//Round Enchantment Strength
						strengths[0] = (float)Math.Round(strengths[0], 4);
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

						//Round Enchantment Strength
						strengths[i] = (float)Math.Round(strengths[i], 4);
					}
				}
			}

			TierStrengthData = new DifficultyStrength(tierStrengths);
			EnchantmentStrengthData = new DifficultyStrength(strengths);
		}
		private float GetStrengthApplyScalePercent(float multiplier) {
			float defaultStrength = defaultEnchantmentStrengths[StrengthGroup].enchantmentTierStrength[EnchantmentTier];
			float scale = Math.Abs(ScalePercent);
			float strength;
			if (ScalePercent < 0f && multiplier < 1f) {
				strength = 1f + (1f - scale) * (defaultStrength - 1f) + (defaultStrength - 1f) * multiplier * scale;
			}
			else {
				strength = (1f - scale) * defaultStrength + defaultStrength * multiplier * scale;
			}

			return strength;
		}
		protected bool CheckStaticStatByName(string checkName = "", bool checkBoolOnly = false) {
			if (checkName == "")
				checkName = EnchantmentTypeName;

			foreach (FieldInfo field in Item.GetType().GetFields()) {
				string fieldName = field.Name;
				if (fieldName.Length <= checkName.Length) {
					if (fieldName.Length < checkName.Length) {
						switch (checkName) {
							case "CriticalStrikeChance":
								//Do nothing.  Allow only a segment of the name to be matched (such as crit to criticalStrikeChance)
								break;
							default:
								//Prevent name checking if not correct length.
								continue;
						}
					}

					string name = StringManipulation.ToFieldName(checkName.Substring(0, fieldName.Length));
					if (fieldName == name) {
						if (checkBoolOnly) {
							return field.FieldType == typeof(bool);
						}
						else {
							switch (name) {
								case "crit":
									AddStaticStat(fieldName, 0f, 1f, 0f, EnchantmentStrength * 100);
									break;
								default:
									AddStaticStat(fieldName, EnchantmentStrength);
									break;
							}
						}
						return true;
					}
				}
			}
			foreach (PropertyInfo property in Item.GetType().GetProperties()) {
				string name = property.Name;
				if (name.Length <= checkName.Length) {
					if (name == checkName.Substring(0, name.Length)) {
						if (checkBoolOnly) {
							return property.PropertyType == typeof(bool);
						}
						else {
							switch (name) {
								case "ArmorPenetration":
									AddStaticStat(name, 0f, 1f, 0f, EnchantmentStrength);
									break;
								default:
									AddStaticStat(name, EnchantmentStrength);
									break;
							}
						}
						return true;
					}
				}
			}
			Player player = new();
			foreach (FieldInfo field in player.GetType().GetFields()) {
				string fieldName = field.Name;
				if (fieldName.Length <= checkName.Length) {
					string name = StringManipulation.ToFieldName(checkName.Substring(0, fieldName.Length));
					if (fieldName == name) {
						if (checkBoolOnly) {
							return field.FieldType == typeof(bool);
						}
						else {
							switch (name) {
								case "Defense":
								case "maxMinions":
									AddStaticStat(fieldName, 0f, 1f, 0f, EnchantmentStrength);
									break;
								default:
									AddStaticStat(fieldName, EnchantmentStrength);
									break;
							}
						}
						return true;
					}
				}
			}
			foreach (PropertyInfo property in player.GetType().GetProperties()) {
				string name = property.Name;
				if (name.Length <= checkName.Length) {
					if (name == checkName.Substring(0, name.Length)) {
						if (checkBoolOnly) {
							return property.PropertyType == typeof(bool);
						}
						else {
							AddStaticStat(name, EnchantmentStrength);
						}
						return true;
					}
				}
			}
			return false;
		}
		protected bool AddStaticStat(string name, float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) {
			StaticStats.Add(new EnchantmentStaticStat(name, additive, multiplicative, flat, @base));

			return true;
		}
		protected bool AddEStat(string name, float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) {
			EStats.Add(new EStat(name, additive, multiplicative, flat, @base));

			return true;
		}
		protected string GetShortTooltip(bool showValue = true, bool percent = true, bool sign = false, bool multiply100 = true, string text = null) {
			string s = "";
			if (showValue) {
				float strength = (float)Math.Round(EnchantmentStrength * AllowedListMultiplier, 3);
				if (multiply100)
					strength *= 100f;

				if (sign)
					s += strength < 0f ? "" : "+";

				s += $"{strength}";
				if (percent)
					s += "%";

				s += " ";
			}

			s += text ?? GetLocalizationTypeName();

			if (!showValue)
				s += $" {EnchantmentTier}";

			return s;
		}
		protected string GetLocalizationTypeName(string s = null, IEnumerable<object> args = null) => (s ?? EnchantmentTypeName).Lang(L_ID1.Tooltip, L_ID2.EffectDisplayName, args);
		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			var tooltipTuples = GenerateFullTooltip();
			foreach (var tooltipTuple in tooltipTuples) {
				tooltips.Add(new TooltipLine(Mod, "enchantment:base", tooltipTuple.Item1) { OverrideColor = tooltipTuple.Item2 });
			}
		}
		/*
		public IEnumerable<Tuple<string, Color>> GenerateFullTooltip() {
			List<Tuple<string, Color>> fullTooltip = new List<Tuple<string, Color>>();

			if (CustomTooltip != "")
				fullTooltip.Add(new Tuple<string, Color>(CustomTooltip, Color.White));//, Color.DarkGray));

			//fullTooltip.Add(new Tuple<string, Color>("Effects:", Color.Violet));
			fullTooltip.AddRange(GetEffectsTooltips());

			fullTooltip.Add(new Tuple<string, Color>($"Level cost: {GetCapacityCost()}", Color.LightGreen));

			//Unique
			if (Unique)
				fullTooltip.Add(new Tuple<string, Color>("   *Unique* (Limited to 1 Unique Enchantment)", Color.White));

			fullTooltip.AddRange(GetAllowedListTooltips());

			if (AllowedList.ContainsKey(EItemType.Weapons) && Unique && !Max1 && DamageClassSpecific == 0 && ArmorSlotSpecific == -1 && RestrictedClass?.Count == 1 && Utility == false) {
				//Unique (Specific Item)
				fullTooltip.Add(new Tuple<string, Color>(
					$"   *{EnchantmentTypeName.AddSpaces()} Only*",
					Color.White
				));
			}
			else if (DamageClassSpecific > 0) {
				//DamageClassSpecific
				fullTooltip.Add(new Tuple<string, Color>(
					$"   *{GetDamageClassName(DamageClassSpecific)} Only*",
					Color.White
				));
			}
			else if (ArmorSlotSpecific > -1) {
				//ArmorSlotSpecific
				fullTooltip.Add(new Tuple<string, Color>(
					$"   *{(ArmorSlotSpecificID)ArmorSlotSpecific} armor slot Only*",
					Color.White
				));
			}

			//RestrictedClass
			if (RestrictedClass.Count > 0) {
				fullTooltip.Add(new Tuple<string, Color>(
					$"   *Not allowed on {RestrictedClass.Select(c => GetDamageClassName(c)).JoinList(", ", " or ")} weapons*",
					Color.White
				));
			}

			if (Max1)
				fullTooltip.Add(new Tuple<string, Color>($"   *Max of 1 per item*", Color.White));

			if (Utility)
				fullTooltip.Add(new Tuple<string, Color>($"   *Utility*", Color.White));

			return fullTooltip;
		}
		*/
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

			if (AllowedList.ContainsKey(EItemType.Weapons) && Unique && !Max1 && DamageClassSpecific == 0 && ArmorSlotSpecific == -1 && RestrictedClass?.Count == 0 && Utility == false) {
				//Unique (Specific Item)
				fullTooltip.Add(new Tuple<string, Color>(
					$"   *{GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID.Only, GetLocalizationTypeName())}*",
					Color.White
				));
			}
			else if (DamageClassSpecific > 0) {
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
		private static string GetLocalizationForGeneralTooltip(EnchantmentGeneralTooltipsID id, object arg = null) => id.ToString().Lang(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips, new object[] { arg });
		//public IEnumerable<Tuple<string, Color>> GetEnchantmentTooltips() {
		//	List<Tuple<string, Color>> tooltips = new List<Tuple<string, Color>>();
		//	
		//	return tooltips;
		//}

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

					tooltip += $"{key.ToString().Lang(L_ID1.Tooltip, L_ID2.ItemType)}: {AllowedList[key].Percent()}%";

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

		//private string GetItemRestrictionTooltip(IEnumerable<EItemType> itemTypes) {
		//	return string.Join("\nAllowed on ", itemTypes.Select(i => $"{i} ({Math.Round(AllowedList[i]*100, 1)})"));
		//}

		/*private string GetEStatToolTip(EStat eStat, bool forFullToolTip = false, bool firstToolTip = false, EItemType allowedListKey = EItemType.None) {
			string toolTip = "";

			//percentage, multiply100, plus
			GetPercentageMult100(eStat.StatName, out bool percentage, out bool multiply100, out bool plus);

			//Stat Name
			string statName;
			string eStatFirst2 = eStat.StatName.Substring(0, 2);
			bool invert = forFullToolTip && !firstToolTip && eStatFirst2 == "I_";
			if (invert) {
				//Remove "I_" from the start of the Stat Name
				statName = eStat.StatName.Substring(2);
			}
			else {
				statName = eStat.StatName;
			}

			//Flat value of 13.13f will prevent any number from being displayed in the tooltip.
			if (eStat.Flat != 13.13f) {
				float allowedListMultiplier = allowedListKey != EItemType.None ? AllowedList[allowedListKey] : 1f;
				float invertMultiplier = invert ? -1f : 1f;
				float additive = eStat.Additive * invertMultiplier * allowedListMultiplier;
				float multiplicative = invert ? 1f / (eStat.Multiplicative * allowedListMultiplier) : eStat.Multiplicative * allowedListMultiplier;
				float flat = eStat.Flat * invertMultiplier * allowedListMultiplier;
				float @base = eStat.Base * invertMultiplier * allowedListMultiplier;
				EStat enchantmentStat = new EStat(statName, additive, multiplicative, flat, @base);

				if (enchantmentStat.Additive != 0f || enchantmentStat.Multiplicative != 1f) {
					if (enchantmentStat.Additive != 0f) {
						toolTip += (plus ? (enchantmentStat.Additive > 0f ? "+" : "") : "") + 
							$"{enchantmentStat.Additive * (multiply100 ? 100 : 1)}{(percentage ? "%" : "")} ";
					}
					else if (enchantmentStat.Multiplicative != 1f) {
						toolTip += $"{enchantmentStat.Multiplicative}x ";
					}
				}
				else {
					float num = enchantmentStat.Base != 0f ? enchantmentStat.Base : enchantmentStat.Flat;
					toolTip += (plus ? (num > 0f ? "+" : "") : "") + $"{num * (multiply100 ? 100 : 1)}{(percentage ? "%" : "")} ";
				}
			}

			toolTip += $"{(forFullToolTip ? CheckStatAlteredName(firstToolTip ? MyDisplayName : statName) : MyDisplayName)}";

			return toolTip;
		}
		private string GetStaticStatToolTip(EnchantmentStaticStat staticStat, bool forFullToolTip = false, bool firstToolTip = false, EItemType allowedListKey = EItemType.None) {
			string toolTip = "";
			string statName;
			bool invert = staticStat.Name.Substring(0, 2) == "I_";
			bool prevent = staticStat.Name.Substring(0, 2) == "P_";
			if (invert || prevent) {
				statName = staticStat.Name.Substring(2);
			}
			else {
				statName = staticStat.Name;
			}

			bool statIsBool = CheckStaticStatByName(statName, true);
			if (statIsBool) {
				statName = statName.CapitalizeFirst().AddSpaces();
				if (prevent)
					statName = $"Prevent {statName}";

				toolTip = statName;
			}
			else {
				float allowedListMultiplier = allowedListKey != EItemType.None ? AllowedList[allowedListKey] : 1f;
				float invertMultiplier = invert ? -1f : 1f;
				float additive = staticStat.Additive * invertMultiplier * allowedListMultiplier;
				float multiplicative = invert ? 1f / (staticStat.Multiplicative * allowedListMultiplier) : staticStat.Multiplicative * allowedListMultiplier;
				float flat = staticStat.Flat * invertMultiplier * allowedListMultiplier;
				float @base = staticStat.Base * invertMultiplier * allowedListMultiplier;
				EnchantmentStaticStat enchantmentStaticStat = new EnchantmentStaticStat(statName, additive, multiplicative, flat, @base);

				GetPercentageMult100(enchantmentStaticStat.Name, out bool percentage, out bool multiply100, out bool plus, true);

				if (enchantmentStaticStat.Additive != 0f || enchantmentStaticStat.Multiplicative != 1f) {
					if (enchantmentStaticStat.Additive != 0f) {
						toolTip += (plus ? (enchantmentStaticStat.Additive > 0f ? "+" : "") : "") + $"{enchantmentStaticStat.Additive * (multiply100 ? 100 : 1)}{(percentage ? "%" : "")} ";
					}
					else if (enchantmentStaticStat.Multiplicative != 1f) {
						toolTip += $"{enchantmentStaticStat.Multiplicative}x ";
					}
				}
				else {
					float num = enchantmentStaticStat.Base != 0f ? enchantmentStaticStat.Base : enchantmentStaticStat.Flat;
					toolTip += (plus ? (num > 0f ? "+" : "") : "") + $"{num * (multiply100 ? 100 : 1)}{(percentage ? "%" : "")} ";// " + (enchantmentStaticStat.Base != 0f ? "base" : "");
				}
				toolTip += $"{(forFullToolTip ? CheckStatAlteredName(firstToolTip ? MyDisplayName : enchantmentStaticStat.Name) : MyDisplayName)}";
			}

			return toolTip;
		}*/
		public static int GetDamageClass(int damageType) {

			switch (damageType) {
				case (int)DamageClassID.Melee:
				case (int)DamageClassID.MeleeNoSpeed:
					return (int)DamageClassID.Melee;
				case (int)DamageClassID.Summon:
				case (int)DamageClassID.MagicSummonHybrid:
					return (int)DamageClassID.Summon;
				default:
					if (WEMod.calamityEnabled) {
						if (damageType == ModIntegration.CalamityValues.trueMelee.Type || damageType == ModIntegration.CalamityValues.trueMeleeNoSpeed.Type)
							return (int)DamageClassID.Melee;
					}

					return damageType;
			}
		}
		public static string GetDamageClassName(int type) {
			int damageType = GetDamageClass(type);

			if (damageType <= (int)DamageClassID.Default)
				return DamageClassID.Generic.ToString().Lang(L_ID1.Tooltip, L_ID2.DamageClassNames);

			switch (damageType) {
				case (int)DamageClassID.MagicSummonHybrid:
					return DamageClassID.Summon.ToString().Lang(L_ID1.Tooltip, L_ID2.DamageClassNames);
				case (int)DamageClassID.MeleeNoSpeed:
					return DamageClassID.Melee.ToString().Lang(L_ID1.Tooltip, L_ID2.DamageClassNames);
			}

			if (damageType <= (int)DamageClassID.Throwing)
				return ((DamageClassID)damageType).ToString().Lang(L_ID1.Tooltip, L_ID2.DamageClassNames);

			if (WEMod.calamityEnabled) {
				int rogue = ModIntegration.CalamityValues.rogue.Type;
				if (damageType == rogue)
					return DamageClassID.Rogue.ToString().Lang(L_ID1.Tooltip, L_ID2.DamageClassNames);
			}

			return DamageClassID.Generic.ToString().Lang(L_ID1.Tooltip, L_ID2.DamageClassNames);
		}
		private uint GetBuffDuration() {
			return defaultBuffDuration * ((uint)EnchantmentTier + 1);
		}
		public override void AddRecipes() {
			for (int i = EnchantmentTier; i < tierNames.Length; i++) {
				if (!useAllRecipes && i != EnchantmentTier)
					continue;

				//Lowest Craftable Tier
				if (EnchantmentTier < LowestCraftableTier)
					continue;

				Recipe recipe;

				for (int j = LowestCraftableTier; j <= EnchantmentTier; j++) {
					if (!useAllRecipes && j != EnchantmentTier)
						continue;

					recipe = CreateRecipe();

					//Essence
					for (int k = j; k <= EnchantmentTier; k++) {
						int essenceNumber = Utility ? 5 : 10;
						recipe.AddIngredient(Mod, "EnchantmentEssence" + tierNames[k], essenceNumber);
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
						recipe.AddRecipeGroup("WeaponEnchantments:CommonGems", 2);
					}
					if (EnchantmentTier == 4) {
						recipe.AddRecipeGroup("WeaponEnchantments:RareGems");
					}

					//Enchanting Table
					recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");

					if (j == 0)
						EditTier0Recipies(recipe);

					EditRecipe(recipe);

					recipe.Register();
				}
			}

			if (!WEMod.clientConfig.AllowCraftingIntoLowerTier || EnchantmentValueTierReduction != 0)
				return;

			for (int i = 0; i < tierNames.Length; i++) {
				if (!useAllRecipes && i != EnchantmentTier)
					continue;

				if (EnchantmentTier == tierNames.Length)
					continue;

				Recipe recipe;
				for (int j = EnchantmentTier + 1; j < tierNames.Length; j++) {
					if (!useAllRecipes && j != EnchantmentTier + 1)
						continue;

					recipe = CreateRecipe();

					//Enchantment
					recipe.AddIngredient(Mod, EnchantmentTypeName + "Enchantment" + tierNames[j], 1);

					//Containment
					if (EnchantmentTier < 2) {
						recipe.AddIngredient(Mod, ContainmentItem.sizes[EnchantmentTier] + "Containment", 1);
					}

					//Enchanting Table
					recipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");

					//Gems
					if (EnchantmentTier == 3) {
						recipe.AddRecipeGroup("WeaponEnchantments:CommonGems", 2);
					}

					recipe.Register();
				}

				if (!useAllRecipes && EnchantmentTier != 0)
					continue;

				//Basic Essence Recipe
				Recipe containmentRecipe = CreateRecipe();

				//Basic Essence
				containmentRecipe.createItem = new Item(ModContent.ItemType<EnchantmentEssenceBasic>(), Utility ? 5 : 10);

				//This enchantment
				containmentRecipe.AddIngredient(Type);

				//Enchanting Table
				containmentRecipe.AddTile(Mod, EnchantingTableItem.enchantingTableNames[i] + "EnchantingTable");
				containmentRecipe.Register();
			}


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
		public int GetCapacityCost() {
			float multiplier;
			if (CapacityCostMultiplier != -13.13f) {
				//multiplier is being manually set by this enchantment
				multiplier = CapacityCostMultiplier;
			}
			else {
				multiplier = 2f;

				if (Utility)
					multiplier = 1f;

				if (Unique || Max1)
					multiplier = 3f;
			}

			return (int)Math.Round((1f + EnchantmentTier) * multiplier * ConfigCapacityCostMultiplier);
		}
	}
}
