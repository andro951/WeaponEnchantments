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
using WeaponEnchantments.Debuffs;
using static WeaponEnchantments.Common.Configs.ConfigValues;

namespace WeaponEnchantments.Items
{
	public enum EnchantmentTypeID : int
	{
		AllForOne,
		AmmoCost,
		ArmorPenetration,
		CatastrophicRelease,
		ColdSteel,
		CriticalStrikeChance,
		Damage,
		DangerSense,
		StatDefense,
		GodSlayer,
		HellsWrath,
		Hunter,
		JunglesFury,
		LifeSteal,
		Mana,
		MaxMinions,
		Moonlight,
		MoveSpeed,
		ObsidianSkin,
		OneForAll,
		Peace,
		PhaseJump,
		Scale,
		Speed,
		Spelunker,
		Splitting,
		War,
		WorldAblaze,
	}
	//Will be removed
	public enum UtilityEnchantmentNames
	{
		AmmoCost,
		DangerSense,
		Hunter,
		Mana,
		MoveSpeed,
		ObsidianSkin,
		Peace,
		Scale,
		Spelunker,
		War
	}
	public enum DamageTypeSpecificID
	{
		Default,
		Generic,
		Melee,
		MeleeNoSpeed,
		Ranged,
		Magic,
		Summon,
		SummonMeleeSpeed,
		MagicSummonHybrid,
		Throwing
	}//Located in DamageClassLoader.cs
	public enum ArmorSlotSpecificID
	{
		Head,
		Body,
		Legs
	}
	public abstract class Enchantment : ModItem
	{
		//Static
		public static readonly string[] rarity = new string[] { "Basic", "Common", "Rare", "SuperRare", "UltraRare" };
		public static readonly string[] displayRarity = new string[] { "Basic", "Common", "Rare", "Epic", "Legendary" };
		public static readonly Color[] rarityColors = new Color[] { Color.White, Color.Green, Color.Blue, Color.Purple, Color.DarkOrange };
		public static readonly float[,] defaultEnchantmentStrengths = new float[,]
		{
			{0.03f, 0.08f, 0.16f, 0.25f, 0.40f},//0
			{0.4f, 0.8f, 1.2f, 1.6f, 2f},//1 Not used yet
			{1.2f, 1.4f, 1.6f, 1.8f, 2f },//2
			{1f, 2f, 3f, 5f, 10f},//3
			{2f, 4f, 6f, 10f, 20f},//4
			{0.005f, 0.01f, 0.015f, 0.02f, 0.025f},//5
			{2f, 3f, 5f, 8f, 10f},//6
			{0.02f, 0.04f, 0.06f, 0.08f, 0.10f},//7
			{0.5f, 0.6f, 0.75f, 0.85f, 1f},//8
			{0.6f, 0.65f, 0.7f, 0.8f, 0.9f},//9
			{0.2f, 0.4f, 0.6f, 0.8f, 1f },//10
			{0.04f, 0.08f, 0.12f, 0.16f, 0.20f},//7
		};
		public static readonly int defaultBuffDuration = 60;
		public static string listOfAllEnchantmentTooltips = "";//Only used to print the full list of enchantment tooltips in WEPlayer OnEnterWorld()  (Normally commented out there)
		public static bool printListOfEnchantmentTooltips = false;

		//Strength
		public float EnchantmentStrength { private set; get; }
		public virtual int StrengthGroup { private set; get; } = 0;

		/// <summary>
		/// Need comment about what negative scalepercent value means.  Default 1f.
		/// </summary>
		public virtual float ScalePercent { private set; get; } = 1f;
		public Dictionary<string, float> AllowedList { private set; get; } = new Dictionary<string, float>();

		//Identifiers and names
		public virtual string CustomTooltip { private set; get; } = "";
		public int EnchantmentSize { private set; get; } = -1;
		public int EnchantmentType { private set; get; } = -1;
		public string EnchantmentTypeName { private set; get; }
		public virtual string MyDisplayName { private set; get; } = "";
		public string FullToolTip { private set; get; }
		public Dictionary<string, string> AllowedListTooltips { private set; get; } = new Dictionary<string, string>();

		//Restrictions and enchantment types
		public bool Utility { private set; get; }
		public virtual bool Unique { private set; get; }
		public virtual bool Max1 { private set; get; } = false;
		public virtual int DamageClassSpecific { private set; get; } = 0;
		public virtual int ArmorSlotSpecific { private set; get; } = -1;
		public virtual int RestrictedClass { private set; get; } = -1;

		//Stats and buffs
		private bool finishedOneTimeSetup = false;
		public int NewDamageType = -1;
		public int BuffDuration => GetBuffDuration();
		public List<int> Buff { private set; get; } = new List<int>();
		public Dictionary<int, int> OnHitBuff { private set; get; } = new Dictionary<int, int>();
		public Dictionary<int, int> Debuff { private set; get; } = new Dictionary<int, int>();
		public List<EnchantmentStaticStat> StaticStats { private set; get; } = new List<EnchantmentStaticStat>();
		public List<EStat> EStats { private set; get; } = new List<EStat>();

		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');

		public override void SetStaticDefaults() {
			//Get values needed to generate tooltips
			GetDefaults(true);//Change this to have arguments to only get the needed info for setting up tooltips.

			//Journy mode item sacrifice
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			//Generate Full Tooltip
			Tooltip.SetDefault(GenerateFullTooltip(CustomTooltip));

			//DisplayName
			if (WEMod.clientConfig.UseOldRarityNames) {
				//Old rarity names, "Basic", "Common", "Rare", "SuperRare", "UltraRare"
				DisplayName.SetDefault(UtilityMethods.AddSpaces(MyDisplayName + Name.Substring(Name.IndexOf("Enchantment"))));
			}
			else {
				//Current rarity names, "Basic", "Common", "Rare", "Epic", "Legendary"
				DisplayName.SetDefault(UtilityMethods.AddSpaces(MyDisplayName + "Enchantment" + displayRarity[EnchantmentSize]));
			}

			//Only used to print the full list of enchantment tooltips in WEPlayer OnEnterWorld()
			if(printListOfEnchantmentTooltips)
				listOfAllEnchantmentTooltips += $"{Name}\n{Tooltip.GetDefault()}\n\n";
		}
		private void GetDefaults(bool tooltipSetupOnly = false) {
			//EnchantmentTypeName
			EnchantmentTypeName = Name.Substring(0, Name.IndexOf("Enchantment"));

			//Enchantment Size
			EnchantmentSize = GetEnchantmentSize(Name);

			//Check EnchantmentType
			for (int i = 0; i < Enum.GetNames(typeof(EnchantmentTypeID)).Length; i++) {
				if (EnchantmentTypeName == ((EnchantmentTypeID)i).ToString()) {
					EnchantmentType = i;
					break;
				}
			}

			//Item rarity
			Item.rare = EnchantmentSize;

			//Width and Height
			switch (EnchantmentSize) {
				case 3:
					Item.width = 44;
					Item.height = 40;
					break;
				case 4:
					Item.width = 40;
					Item.height = 40;
					break;
				default:
					Item.width = 28 + 4 * (EnchantmentSize);
					Item.height = 28 + 4 * (EnchantmentSize);
					break;
			}

			//endSize and switch will be removed when all tiers of the potion buff enchantments are added.
			int endSize;
			switch ((EnchantmentTypeID)EnchantmentType) {
				case EnchantmentTypeID.Spelunker:
				case EnchantmentTypeID.DangerSense:
				case EnchantmentTypeID.Hunter:
				case EnchantmentTypeID.ObsidianSkin:
					endSize = EnchantmentSize - 2;
					break;
				default:
					endSize = EnchantmentSize;
					break;
			}

			//Value - Essence
			for (int i = 0; i <= endSize; i++) {
				int quantity = Utility ? 5 : 10;
				int value = (int)EnchantmentEssenceBasic.values[i];
				Item.value += value * quantity;
			}

			//Value - Containment/SuperiorStaibalizers
			switch (EnchantmentSize) {
				case 3:
					Item.value += Containment.Values[2];
					break;
				case 4:
					Item.value += ContentSamples.ItemsByType[999].value;
					break;
				default:
					Item.value += Containment.Values[EnchantmentSize];
					break;
			}

			//Check Utility
			string temp = 
			if(GetType().Namespace.GetNameFolderName())
			/*for (int i = 0; i < Enum.GetNames(typeof(UtilityEnchantmentNames)).Length; i++) {
				if (EnchantmentTypeName == ((UtilityEnchantmentNames)i).ToString()) {
					Utility = true;
					break;
				}
			}*/

			//Check Unique (Vanilla Items)
			for (int i = 0; i < ItemID.Count; i++) {
				if (ContentSamples.ItemsByType[i].Name.RemoveSpaces() == EnchantmentTypeName) {
					Unique = true;
					break;
				}
			}

			//StrengthGroup (will be removed)
			switch ((EnchantmentTypeID)EnchantmentType) {
				case (EnchantmentTypeID)(-1):
					StrengthGroup = 1;// 0.4f, 0.8f, 1.2f, 1.6f, 2f Not used yet
					break;
				case EnchantmentTypeID.War:
				case EnchantmentTypeID.Peace:
					StrengthGroup = 2;// 1.2f, 1.4f, 1.6f, 1.8f, 2f
					break;
				case EnchantmentTypeID.StatDefense:
					StrengthGroup = 3;// 1, 2, 3, 5, 10
					break;
				case EnchantmentTypeID.ArmorPenetration:
					StrengthGroup = 4;// 2, 4, 6, 10, 20
					break;
				case EnchantmentTypeID.LifeSteal:
					StrengthGroup = 5;// 0.005, 0.01, 0.015, 0.02, 0.025
					break;
				case EnchantmentTypeID.AllForOne:
					StrengthGroup = 6;//2, 3, 5, 8, 10
					break;
				case EnchantmentTypeID.GodSlayer:
					StrengthGroup = 7;// 0.02, 0.04, 0.06, 0.08, 0.10
					break;
				case EnchantmentTypeID.Splitting:
				case EnchantmentTypeID.CatastrophicRelease:
					StrengthGroup = 8;//0.5, 0.6, 0.75, 0.85, 1
					break;
				case EnchantmentTypeID.ColdSteel:
				case EnchantmentTypeID.HellsWrath:
				case EnchantmentTypeID.JunglesFury:
				case EnchantmentTypeID.Moonlight:
					StrengthGroup = 9;// 0.6f, 0.65f, 0.7f, 0.8f, 0.9f
					break;
				case EnchantmentTypeID.Scale:
				case EnchantmentTypeID.OneForAll:
				case EnchantmentTypeID.WorldAblaze:
				case EnchantmentTypeID.MaxMinions:
				case EnchantmentTypeID.PhaseJump:
					StrengthGroup = 10;// 0.2f, 0.4f, 0.6f, 0.8f, 1f
					break;
				case EnchantmentTypeID.MoveSpeed:
					StrengthGroup = 11;//0.04f, 0.08f, 0.12f, 0.16f, 0.20f
					break;
				default:
					StrengthGroup = 0;//0.03, 0.08, 0.16, 0.25, 0.40
					break;
			}

			//Scale Percents (will be removed)
			switch ((EnchantmentTypeID)EnchantmentType) {
				case EnchantmentTypeID.LifeSteal:
				case EnchantmentTypeID.AllForOne:
				case EnchantmentTypeID.OneForAll:
					ScalePercent = 0.8f;
					break;
				case EnchantmentTypeID.ColdSteel:
				case EnchantmentTypeID.HellsWrath:
				case EnchantmentTypeID.JunglesFury:
				case EnchantmentTypeID.Moonlight:
					ScalePercent = 0.2f / defaultEnchantmentStrengths[StrengthGroup, rarity.Length - 1];
					break;
				case EnchantmentTypeID.MaxMinions:
				case EnchantmentTypeID.PhaseJump:
					ScalePercent = 0.6f;
					break;
				case EnchantmentTypeID.War:
				case EnchantmentTypeID.Peace:
					ScalePercent = -1f;
					break;
			}

			//Config - Individual Strength
			bool foundIndividualStrength = false;
			if (WEMod.serverConfig.individualStrengthsEnabled && WEMod.serverConfig.individualStrengths.Count > 0) {
				foreach (Pair pair in WEMod.serverConfig.individualStrengths) {
					if (pair.itemDefinition.Name == Name) {
						EnchantmentStrength = ((float)pair.Strength / 1000f);
						foundIndividualStrength = true;
					}
				}
			}

			//Config - Linear and Recomended Strength Multipliers
			if (!foundIndividualStrength) {
				float multiplier = LinearStrengthMultiplier; ;
				bool usingLinearStrengthMultiplier = multiplier != 1f;

				if(usingLinearStrengthMultiplier) {
					//Linear
					EnchantmentStrength = multiplier * defaultEnchantmentStrengths[StrengthGroup, EnchantmentSize];
				}
				else {
					//Recomended
					multiplier = RecomendedStrengthMultiplier;
					float defaultStrength = defaultEnchantmentStrengths[StrengthGroup, EnchantmentSize];
					float scale = Math.Abs(ScalePercent);

					//Apply Scale Percent
					if (ScalePercent < 0f && multiplier < 1f) {
						EnchantmentStrength = 1f + (1f - scale) * (defaultStrength - 1f) + (defaultStrength - 1f) * multiplier * scale;
					}
					else {
						EnchantmentStrength = (1f - scale) * defaultStrength + defaultStrength * multiplier * scale;
					}
				}
			}

			//Round Enchantment Strength
			EnchantmentStrength = (float)Math.Round(EnchantmentStrength, 4);

			//DamageTypeSpecific, Max1, RestrictedClass (will be removed)
			switch ((EnchantmentTypeID)EnchantmentType) {
				case EnchantmentTypeID.GodSlayer:
					DamageClassSpecific = (int)DamageTypeSpecificID.Melee;
					break;
				case EnchantmentTypeID.Splitting:
					DamageClassSpecific = (int)DamageTypeSpecificID.Ranged;
					break;
				case EnchantmentTypeID.CatastrophicRelease:
					DamageClassSpecific = (int)DamageTypeSpecificID.Magic;
					break;
				case EnchantmentTypeID.ColdSteel:
				case EnchantmentTypeID.HellsWrath:
				case EnchantmentTypeID.JunglesFury:
				case EnchantmentTypeID.Moonlight:
					RestrictedClass = (int)DamageTypeSpecificID.Summon;
					Unique = true;
					break;
				case EnchantmentTypeID.AllForOne:
					Max1 = true;
					RestrictedClass = (int)DamageTypeSpecificID.Summon;
					break;
				case EnchantmentTypeID.OneForAll:
				case EnchantmentTypeID.WorldAblaze:
					Max1 = true;
					break;
				case EnchantmentTypeID.PhaseJump:
					ArmorSlotSpecific = (int)ArmorSlotSpecificID.Legs;
					break;
			}

			//New Display Name (will be removed)
			switch ((EnchantmentTypeID)EnchantmentType) {
				case EnchantmentTypeID.Scale:
					MyDisplayName = "Size";
					break;
				case EnchantmentTypeID.StatDefense:
					MyDisplayName = "Defence";
					break;
			}

			//Default My Display Name
			if (MyDisplayName == "")
				MyDisplayName = EnchantmentTypeName;

			//Only check once
			if (finishedOneTimeSetup)
				return;

			//AllowedList (will be removed)
			switch ((EnchantmentTypeID)EnchantmentType) {
				case EnchantmentTypeID.StatDefense:
					AllowedList.Add("Weapon", 0.5f);
					AllowedList.Add("Armor", 1f);
					AllowedList.Add("Accessory", 1f);
					break;
				case EnchantmentTypeID.DangerSense:
				case EnchantmentTypeID.Hunter:
				case EnchantmentTypeID.MoveSpeed:
				case EnchantmentTypeID.ObsidianSkin:
				case EnchantmentTypeID.Peace:
				case EnchantmentTypeID.Spelunker:
				case EnchantmentTypeID.War:
					AllowedList.Add("Weapon", 1f);
					AllowedList.Add("Armor", 1f);
					AllowedList.Add("Accessory", 1f);
					break;
				case EnchantmentTypeID.AllForOne:
				case EnchantmentTypeID.OneForAll:
				case EnchantmentTypeID.LifeSteal:
				case EnchantmentTypeID.ColdSteel:
				case EnchantmentTypeID.GodSlayer:
				case EnchantmentTypeID.HellsWrath:
				case EnchantmentTypeID.JunglesFury:
				case EnchantmentTypeID.Moonlight:
				case EnchantmentTypeID.CatastrophicRelease:
				case EnchantmentTypeID.WorldAblaze:
					AllowedList.Add("Weapon", 1f);
					break;
				case EnchantmentTypeID.MaxMinions:
					AllowedList.Add("Armor", 1f);
					AllowedList.Add("Accessory", 1f);
					break;
				case EnchantmentTypeID.PhaseJump:
					AllowedList.Add("Armor", 1f);
					break;
			}

			//Default AllowedList
			if (AllowedList.Count < 1) {
				AllowedList.Add("Weapon", 1f);
				AllowedList.Add("Armor", 0.25f);
				AllowedList.Add("Accessory", 0.25f);
			}

			//Stats (will be removed)
			switch ((EnchantmentTypeID)EnchantmentType) {
				case EnchantmentTypeID.AllForOne:
					EStats.Add(new EStat(EnchantmentTypeName, 0f, EnchantmentStrength));
					EStats.Add(new EStat("Damage", 0f, EnchantmentStrength));
					EStats.Add(new EStat("NPCHitCooldown", 0f, 4f + EnchantmentStrength * 0.4f));
					AddStaticStat("useTime", 0f, 1f + EnchantmentStrength * 0.1f);
					AddStaticStat("useAnimation", 0f, 1f + EnchantmentStrength * 0.1f);
					AddStaticStat("mana", 1.5f + EnchantmentStrength * 0.15f);
					AddStaticStat("P_autoReuse", EnchantmentStrength);
					AddStaticStat("P_autoReuseGlove", EnchantmentStrength);
					break;
				case EnchantmentTypeID.AmmoCost:
					EStats.Add(new EStat(EnchantmentTypeName, 0f, 1f, 0f, -EnchantmentStrength));
					break;
				case EnchantmentTypeID.ArmorPenetration:
				case EnchantmentTypeID.CriticalStrikeChance:
				case EnchantmentTypeID.MaxMinions:
				case EnchantmentTypeID.MoveSpeed:
				case EnchantmentTypeID.StatDefense:
					CheckStaticStatByName();
					break;
				case EnchantmentTypeID.Scale:
					CheckStaticStatByName();
					AddStaticStat("whipRangeMultiplier", EnchantmentStrength);
					break;
				case EnchantmentTypeID.CatastrophicRelease:
					EStats.Add(new EStat(EnchantmentTypeName, 0f, 1f, EnchantmentStrength));
					EStats.Add(new EStat("InfinitePenetration", 0f, 1f, 13.13f));
					AddStaticStat("scale", 0f, EnchantmentStrength * 10f);
					AddStaticStat("shootSpeed", 0f, 1f - 0.8f * EnchantmentStrength);
					//AddStaticStat("useTime", 0f, 1000f);
					AddStaticStat("P_autoReuse", EnchantmentStrength);
					break;
				case EnchantmentTypeID.ColdSteel:
					EStats.Add(new EStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength));
					NewDamageType = (int)DamageTypeSpecificID.SummonMeleeSpeed;
					if (EnchantmentSize == 3) OnHitBuff.Add(BuffID.CoolWhipPlayerBuff, BuffDuration);
					if (EnchantmentSize == 4) Debuff.Add(BuffID.RainbowWhipNPCDebuff, BuffDuration);
					Debuff.Add(BuffID.Frostburn, BuffDuration);
					EStats.Add(new EStat("Damage", 0f, EnchantmentStrength));
					break;
				case EnchantmentTypeID.HellsWrath:
					EStats.Add(new EStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength));
					NewDamageType = (int)DamageTypeSpecificID.SummonMeleeSpeed;
					Debuff.Add(BuffID.FlameWhipEnemyDebuff, BuffDuration);
					if (EnchantmentSize == 4) Debuff.Add(BuffID.RainbowWhipNPCDebuff, BuffDuration);
					Debuff.Add(EnchantmentSize == 3 ? BuffID.OnFire3 : BuffID.OnFire, BuffDuration);
					EStats.Add(new EStat("Damage", 0f, EnchantmentStrength));
					break;
				case EnchantmentTypeID.JunglesFury:
					EStats.Add(new EStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength));
					NewDamageType = (int)DamageTypeSpecificID.SummonMeleeSpeed;
					OnHitBuff.Add(BuffID.SwordWhipPlayerBuff, BuffDuration);
					Debuff.Add(BuffID.SwordWhipNPCDebuff, BuffDuration);
					if (EnchantmentSize == 4) Debuff.Add(BuffID.RainbowWhipNPCDebuff, BuffDuration);
					Debuff.Add(EnchantmentSize == 3 ? BuffID.Venom : BuffID.Poisoned, BuffDuration);
					EStats.Add(new EStat("Damage", 0f, EnchantmentStrength));
					break;
				case EnchantmentTypeID.Moonlight:
					EStats.Add(new EStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength));
					NewDamageType = (int)DamageTypeSpecificID.SummonMeleeSpeed;
					OnHitBuff.Add(BuffID.ScytheWhipPlayerBuff, BuffDuration);
					if (EnchantmentSize == 3) Debuff.Add(BuffID.ScytheWhipEnemyDebuff, BuffDuration);
					if (EnchantmentSize == 4) Debuff.Add(BuffID.RainbowWhipNPCDebuff, BuffDuration);
					EStats.Add(new EStat("Damage", 0f, EnchantmentStrength));
					break;
				case EnchantmentTypeID.Damage:
					EStats.Add(new EStat(EnchantmentTypeName, EnchantmentStrength));
					break;
				case EnchantmentTypeID.DangerSense:
				case EnchantmentTypeID.Hunter:
				case EnchantmentTypeID.Spelunker:
				case EnchantmentTypeID.ObsidianSkin:
					CheckBuffByName();
					break;
				case EnchantmentTypeID.Mana:
					AddStaticStat(EnchantmentTypeName.ToFieldName(), -EnchantmentStrength);
					break;
				case EnchantmentTypeID.OneForAll:
					EStats.Add(new EStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength));
					EStats.Add(new EStat("NPCHitCooldown", 0f, 1.5f - EnchantmentStrength * 0.2f));
					AddStaticStat("useTime", 0f, 1.5f - EnchantmentStrength * 0.2f);
					AddStaticStat("useAnimation", 0f, 1.5f - EnchantmentStrength * 0.2f);
					break;
				case EnchantmentTypeID.Peace:
					EStats.Add(new EStat("spawnRate", 0f, 1f / EnchantmentStrength));
					EStats.Add(new EStat("maxSpawns", 0f, 1f / EnchantmentStrength));
					break;
				case EnchantmentTypeID.PhaseJump:
					AddStaticStat("dashType", 0f, 1f, 0f, 3f);
					break;
				case EnchantmentTypeID.War:
					EStats.Add(new EStat("spawnRate", 0f, EnchantmentStrength));
					EStats.Add(new EStat("maxSpawns", 0f, EnchantmentStrength));
					break;
				case EnchantmentTypeID.Speed:
					EStats.Add(new EStat("I_NPCHitCooldown", EnchantmentStrength));
					AddStaticStat("I_useTime", EnchantmentStrength);
					AddStaticStat("I_useAnimation", EnchantmentStrength);
					if (EnchantmentStrength >= 0.1f) AddStaticStat("autoReuse", EnchantmentStrength);
					break;
				case EnchantmentTypeID.WorldAblaze:
					if (EnchantmentSize == 4) EStats.Add(new EStat("Amaterasu", 0f, 1f, 0f, EnchantmentStrength));
					if (EnchantmentSize == 4) Debuff.Add(ModContent.BuffType<AmaterasuDebuff>(), -1);
					Debuff.Add(BuffID.OnFire, (int)((float)BuffDuration * EnchantmentStrength));
					Debuff.Add(BuffID.Oiled, (int)((float)BuffDuration * 0.8f * EnchantmentStrength));
					if (EnchantmentStrength > defaultEnchantmentStrengths[StrengthGroup, 0]) Debuff.Add(BuffID.CursedInferno, (int)((float)BuffDuration * 0.6f * EnchantmentStrength));
					if (EnchantmentStrength > defaultEnchantmentStrengths[StrengthGroup, 1]) Debuff.Add(BuffID.ShadowFlame, (int)((float)BuffDuration * 0.4f * EnchantmentStrength));
					if (EnchantmentStrength > defaultEnchantmentStrengths[StrengthGroup, 2]) Debuff.Add(BuffID.OnFire3, (int)((float)BuffDuration * 0.2f * EnchantmentStrength));
					break;
			}

			//Default Stat
			if(StaticStats.Count < 1 && EStats.Count < 1 && Buff.Count < 1 && Debuff.Count < 1 && OnHitBuff.Count < 1 && NewDamageType == -1) {
				EStats.Add(new EStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength));
			}

			//Allowed List Tooltips
			foreach (string key in AllowedList.Keys) {
				AllowedListTooltips.Add(key, GenerateShortTooltip(false, false, key));
			}

			finishedOneTimeSetup = true;
		}
		public override void SetDefaults() {
			Item.maxStack = 99;
			GetDefaults();
		}
		private void GetPercentageMult100(string s, out bool percentage, out bool multiply100, out bool plus, bool staticStat = false) {
			percentage = true;
			multiply100 = true;
			plus = staticStat;
			switch (s) {
				case "ArmorPenetration":
				case "statDefense":
				case "maxMinions":
					percentage = false;
					multiply100 = false;
					plus = true;
					break;
				case "crit":
					multiply100 = false;
					plus = true;
					break;
				case "Damage":
				case "NPCHitCooldown":
					plus = true;
					break;
			}//percentage, multiply100
		}
		public string CheckStatAlteredName(string name) {
			switch (name) {
				case "crit":
				case "statDefense":
				case "scale":
					return MyDisplayName.AddSpaces();
				case "Damage":
					return "Damage dealt(Not visible in weapon stats applied at damage calculation)";
				default:
					return name.CapitalizeFirst().AddSpaces();
			}
		}
		private bool CheckStaticStatByName(string checkName = "", bool checkBoolOnly = false) {
			if (checkName == "")
				checkName = Name;
			foreach (FieldInfo field in Item.GetType().GetFields()) {
				string fieldName = field.Name;
				if (fieldName.Length <= checkName.Length) {
					string name = UtilityMethods.ToFieldName(checkName.Substring(0, fieldName.Length));
					if (fieldName == name) {
						if (checkBoolOnly)
							return field.FieldType == typeof(bool);
						else
							switch (name) {
								case "crit":
									StaticStats.Add(new EnchantmentStaticStat(fieldName, 0f, 1f, 0f, EnchantmentStrength * 100));
									break;
								default:
									StaticStats.Add(new EnchantmentStaticStat(fieldName, EnchantmentStrength));
									break;
							}
						return true;
					}
				}
			}
			foreach (PropertyInfo property in Item.GetType().GetProperties()) {
				string name = property.Name;
				if (name.Length <= checkName.Length) {
					if (name == checkName.Substring(0, name.Length)) {
						if (checkBoolOnly)
							return property.PropertyType == typeof(bool);
						else {
							switch (name) {
								case "ArmorPenetration":
									StaticStats.Add(new EnchantmentStaticStat(name, 0f, 1f, 0f, EnchantmentStrength));
									break;
								default:
									StaticStats.Add(new EnchantmentStaticStat(name, EnchantmentStrength));
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
					string name = UtilityMethods.ToFieldName(checkName.Substring(0, fieldName.Length));
					if (fieldName == name) {
						if (checkBoolOnly)
							return field.FieldType == typeof(bool);
						else
							switch (name) {
								case "statDefense":
								case "maxMinions":
									StaticStats.Add(new EnchantmentStaticStat(fieldName, 0f, 1f, 0f, EnchantmentStrength));
									break;
								default:
									StaticStats.Add(new EnchantmentStaticStat(fieldName, EnchantmentStrength));
									break;
							}
						return true;
					}
				}
			}
			foreach (PropertyInfo property in player.GetType().GetProperties()) {
				string name = property.Name;
				if (name.Length <= checkName.Length) {
					if (name == checkName.Substring(0, name.Length)) {
						if (checkBoolOnly)
							return property.PropertyType == typeof(bool);
						else
							StaticStats.Add(new EnchantmentStaticStat(name, EnchantmentStrength));
						return true;
					}
				}
			}
			return false;
		}
		private bool AddStaticStat(string name, float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) {
			StaticStats.Add(new EnchantmentStaticStat(name, additive, multiplicative, flat, @base));
			return true;
		}
		private bool CheckBuffByName(bool debuff = false, string baseName = "") {
			if (baseName == "")
				baseName = Name;
			BuffID buffID = new();
			foreach (FieldInfo field in buffID.GetType().GetFields()) {
				string fieldName = field.Name;
				if (fieldName.Length <= baseName.Length) {
					string name = baseName.Substring(0, fieldName.Length);
					if (fieldName.ToLower() == name.ToLower()) {
						if (debuff)
							Debuff.Add((int)buffID.GetType().GetField(fieldName).GetValue(buffID), GetBuffDuration());
						else
							Buff.Add((int)buffID.GetType().GetField(fieldName).GetValue(buffID));
						//StaticStats.Add(new StaticStatStruct(fieldName, EnchantmentStrength));
						return true;
					}
				}
			}
			return false;
		}
		private string GetBuffName(int id) {
			if (id < BuffID.Count) {
				BuffID buffID = new();
				foreach (FieldInfo field in buffID.GetType().GetFields()) {
					if (field.FieldType == typeof(int) && (int)field.GetValue(buffID) == id) {
						return field.Name;
					}
				}
			}
			else {
				if (id == ModContent.BuffType<AmaterasuDebuff>())
					return "Amaterasu";
			}
			return "";
		}
		private string GenerateShortTooltip(bool forFullToolTip = false, bool firstToolTip = false, string allowedListKey = "") {
			if (EStats.Count > 0 && (EStats[0].StatName != "Damage" || Buff.Count == 0 && StaticStats.Count == 0)) {
				EStat baseNameEStat = EStats[0];
				return GetEStatToolTip(baseNameEStat, forFullToolTip, firstToolTip, allowedListKey);
			}
			else if (Buff.Count > 0) {
				return $"Grants {MyDisplayName.AddSpaces()} Buff (tier {EnchantmentSize})";
			}
			else if (StaticStats.Count > 0) {
				EnchantmentStaticStat baseNameStaticStat = StaticStats[0];
				return GetStaticStatToolTip(baseNameStaticStat, forFullToolTip, firstToolTip, allowedListKey);
			};
			return $"{MyDisplayName} {EnchantmentSize}";
		}
		private string GenerateFullTooltip(string uniqueTooltip) {
			string shortTooltip = GenerateShortTooltip(true, true);
			string toolTip = $"{shortTooltip}{(uniqueTooltip != "" ? "\n" : "")}{uniqueTooltip}";
			if (NewDamageType > -1)
				toolTip += $"\nConverts weapon damage type to {((DamageTypeSpecificID)GetDamageClass(NewDamageType)).ToString().AddSpaces()}";
			if (EStats.Count > 0) {
				foreach (EStat eStat in EStats) {
					string eStatToolTip = GetEStatToolTip(eStat, true);
					if (eStatToolTip != shortTooltip)
						toolTip += $"\n{eStatToolTip}";
				}
			}//Estats
			if (StaticStats.Count > 0) {
				foreach (EnchantmentStaticStat staticStat in StaticStats) {
					string staticStatToolTip = GetStaticStatToolTip(staticStat, true);
					if (staticStatToolTip != shortTooltip)
						toolTip += $"\n{staticStatToolTip}";
				}
			}//StaticStats
			if (OnHitBuff.Count > 0) {
				int i = 0;
				bool first = true;
				foreach (int onHitBuff in OnHitBuff.Keys) {
					string buffName = GetBuffName(onHitBuff).AddSpaces();
					if (first) {
						toolTip += $"\nOn Hit Buffs: {buffName}";
						first = false;
					}
					else if (i == OnHitBuff.Count - 1)
						toolTip += $" and {buffName}";
					else
						toolTip += $", {buffName}";
					i++;
				}
			}//OnHitBuffs
			if (Debuff.Count > 0) {
				int i = 0;
				bool first = true;
				foreach (int debuff in Debuff.Keys) {
					string buffName = GetBuffName(debuff).AddSpaces();
					if (first) {
						toolTip += $"\nOn Hit Debuffs: {buffName}";
						first = false;
					}
					else if (i == Debuff.Count - 1)
						toolTip += $" and {buffName}";
					else
						toolTip += $", {buffName}";
					i++;
				}
			}//Debuffs
			toolTip += $"\nLevel cost: { GetLevelCost()}";
			if (DamageClassSpecific > 0 || Unique || RestrictedClass > -1 || ArmorSlotSpecific > -1) {
				string limmitationToolTip = "";
				if (Unique)
					limmitationToolTip += "\n   *" + UtilityMethods.AddSpaces(Item.ModItem.Name) + " Only*";
				else if (DamageClassSpecific > 0)
					limmitationToolTip += $"\n   *{((DamageTypeSpecificID)GetDamageClass(DamageClassSpecific)).ToString().AddSpaces()} Only*";
				else if (ArmorSlotSpecific > -1)
					limmitationToolTip += $"\n   *{(ArmorSlotSpecificID)ArmorSlotSpecific} armor slot Only*";
				if (RestrictedClass > -1)
					limmitationToolTip += $"\n   *Not allowed on {((DamageTypeSpecificID)GetDamageClass(RestrictedClass)).ToString().AddSpaces()} weapons*";
				limmitationToolTip += "\n   *Unique* (Limited to 1 Unique Enchantment)";
				toolTip += limmitationToolTip;
			}//Unique, DamageClassSpecific, RestrictedClass
			if (AllowedList.Count > 0) {
				int i = 0;
				bool first = true;
				foreach (string key in AllowedList.Keys) {
					if (first) {
						toolTip += $"\n   *Allowed on {key}: {AllowedList[key] * 100}%{(AllowedList.Count == 1 ? " Only*" : "")}";
						first = false;
					}
					else if (i == AllowedList.Count - 1)
						toolTip += $" and {key}: {AllowedList[key] * 100}%{(AllowedList.Count < 3 ? " Only*" : "")}";
					else
						toolTip += $", {key}: {AllowedList[key] * 100}%";
					i++;
				}
			}//AllowedList
			if (Max1)
				toolTip += "\n   *Max of 1 per weapon*";
			toolTip += Utility ? "\n   *Utility*" : "";
			return toolTip;
		}
		public string GetEStatToolTip(EStat eStat, bool forFullToolTip = false, bool firstToolTip = false, string allowedListKey = "") {
			string toolTip = "";
			bool percentage, multiply100, plus;
			GetPercentageMult100(eStat.StatName, out percentage, out multiply100, out plus);
			string statName;
			bool invert = forFullToolTip && !firstToolTip && eStat.StatName.Substring(0, 2) == "I_";
			if (invert)
				statName = eStat.StatName.Substring(2);
			else
				statName = eStat.StatName;
			EStat enchantmentStat = new EStat(statName,
				eStat.Additive * (invert ? -1f : 1f) * (allowedListKey != "" ? AllowedList[allowedListKey] : 1f),
				invert ? 1f / (eStat.Multiplicative * (allowedListKey != "" ? AllowedList[allowedListKey] : 1f)) :
					eStat.Multiplicative * (allowedListKey != "" ? AllowedList[allowedListKey] : 1f),
				eStat.Flat * (invert ? -1f : 1f) * (allowedListKey != "" ? AllowedList[allowedListKey] : 1f),
				eStat.Base * (invert ? -1f : 1f) * (allowedListKey != "" ? AllowedList[allowedListKey] : 1f));
			if (eStat.Flat != 13.13f) {
				if (enchantmentStat.Additive != 0f || enchantmentStat.Multiplicative != 1f) {
					if (enchantmentStat.Additive != 0f)
						toolTip += (plus ? (enchantmentStat.Additive > 0f ? "+" : "") : "") + $"{enchantmentStat.Additive * (multiply100 ? 100 : 1)}{(percentage ? "%" : "")} ";
					else if (enchantmentStat.Multiplicative != 1f)
						toolTip += $"{enchantmentStat.Multiplicative}x ";
				}
				else {
					float num = enchantmentStat.Base != 0f ? enchantmentStat.Base : enchantmentStat.Flat;
					toolTip += (plus ? (num > 0f ? "+" : "") : "") + $"{num * (multiply100 ? 100 : 1)}{(percentage ? "%" : "")} ";// " + (enchantmenteStat.Base != 0f ? "base" : "");
				}
			}
			toolTip += $"{(forFullToolTip ? CheckStatAlteredName(firstToolTip ? MyDisplayName : enchantmentStat.StatName) : MyDisplayName)}";
			return toolTip;
		}
		public string GetStaticStatToolTip(EnchantmentStaticStat staticStat, bool forFullToolTip = false, bool firstToolTip = false, string allowedListKey = "") {
			string toolTip = "";
			string statName;
			bool invert = staticStat.Name.Substring(0, 2) == "I_";
			bool prevent = staticStat.Name.Substring(0, 2) == "P_";
			if (invert || prevent)
				statName = staticStat.Name.Substring(2);
			else
				statName = staticStat.Name;
			bool statIsBool = CheckStaticStatByName(statName, true);
			if (statIsBool) {
				statName = statName.CapitalizeFirst().AddSpaces();
				if (prevent)
					statName = $"Prevent {statName}";
				toolTip = statName;
			}
			else {
				EnchantmentStaticStat enchantmentStaticStat = new EnchantmentStaticStat(statName,
					staticStat.Additive * (invert ? -1f : 1f) * (allowedListKey != "" ? AllowedList[allowedListKey] : 1f),
					invert ? 1f / (1f + (staticStat.Multiplicative - 1f) * (allowedListKey != "" ? AllowedList[allowedListKey] : 1f)) :
						1f + (staticStat.Multiplicative - 1f) * (allowedListKey != "" ? AllowedList[allowedListKey] : 1f),
					staticStat.Flat * (invert ? -1f : 1f) * (allowedListKey != "" ? AllowedList[allowedListKey] : 1f),
					staticStat.Base * (invert ? -1f : 1f) * (allowedListKey != "" ? AllowedList[allowedListKey] : 1f));
				bool percentage, multiply100, plus;
				GetPercentageMult100(enchantmentStaticStat.Name, out percentage, out multiply100, out plus, true);
				if (enchantmentStaticStat.Additive != 0f || enchantmentStaticStat.Multiplicative != 1f) {
					if (enchantmentStaticStat.Additive != 0f)
						toolTip += (plus ? (enchantmentStaticStat.Additive > 0f ? "+" : "") : "") + $"{enchantmentStaticStat.Additive * (multiply100 ? 100 : 1)}{(percentage ? "%" : "")} ";
					else if (enchantmentStaticStat.Multiplicative != 1f)
						toolTip += $"{enchantmentStaticStat.Multiplicative}x ";
				}
				else {
					float num = enchantmentStaticStat.Base != 0f ? enchantmentStaticStat.Base : enchantmentStaticStat.Flat;
					toolTip += (plus ? (num > 0f ? "+" : "") : "") + $"{num * (multiply100 ? 100 : 1)}{(percentage ? "%" : "")} ";// " + (enchantmentStaticStat.Base != 0f ? "base" : "");
				}
				toolTip += $"{(forFullToolTip ? CheckStatAlteredName(firstToolTip ? MyDisplayName : enchantmentStaticStat.Name) : MyDisplayName)}";
			}
			return toolTip;
		}
		public static int GetDamageClass(int damageType) {
			switch ((DamageTypeSpecificID)damageType) {
				case DamageTypeSpecificID.Melee:
				case DamageTypeSpecificID.MeleeNoSpeed:
					return (int)DamageTypeSpecificID.Melee;
				case DamageTypeSpecificID.Ranged:
					return (int)DamageTypeSpecificID.Ranged;
				case DamageTypeSpecificID.Magic:
					return (int)DamageTypeSpecificID.Magic;
				case DamageTypeSpecificID.Summon:
				case DamageTypeSpecificID.MagicSummonHybrid:
				case DamageTypeSpecificID.SummonMeleeSpeed:
					return (int)DamageTypeSpecificID.Summon;
				case DamageTypeSpecificID.Throwing:
					return (int)DamageTypeSpecificID.Throwing;
				default:
					return (int)DamageTypeSpecificID.Generic;
			}
		}
		private int GetBuffDuration() {
			return defaultBuffDuration * (EnchantmentSize + 1);
		}
		public static int GetEnchantmentSize(string name) {
			for (int i = 0; i < rarity.Length; i++) {
				if (rarity[i] == name.Substring(name.IndexOf("Enchantment") + 11)) {
					return i;
				}
			}//Get EnchantmentSize
			return -1;
		}
		public override void AddRecipes() {
			if (EnchantmentSize > -1) {
				for (int i = EnchantmentSize; i < rarity.Length; i++) {
					Recipe recipe;
					int skipIfLessOrEqualToSize;
					switch ((EnchantmentTypeID)EnchantmentType) {
						case EnchantmentTypeID.Spelunker:
						case EnchantmentTypeID.DangerSense:
						case EnchantmentTypeID.Hunter:
						case EnchantmentTypeID.ObsidianSkin:
							skipIfLessOrEqualToSize = 4;
							break;
						case EnchantmentTypeID.Damage:
						case EnchantmentTypeID.StatDefense:
							skipIfLessOrEqualToSize = -1;
							break;
						default:
							skipIfLessOrEqualToSize = 0;
							break;
					}
					if (EnchantmentSize > skipIfLessOrEqualToSize) {
						for (int j = EnchantmentSize; j >= skipIfLessOrEqualToSize + 1; j--) {
							recipe = CreateRecipe();
							for (int k = EnchantmentSize; k >= j; k--) {
								int essenceNumber = Utility ? 5 : 10;
								recipe.AddIngredient(Mod, "EnchantmentEssence" + EnchantmentEssenceBasic.rarity[k], essenceNumber);
							}
							if (j > 0) {
								recipe.AddIngredient(Mod, EnchantmentTypeName + "Enchantment" + rarity[j - 1], 1);
							}
							if (EnchantmentSize < 3) {
								recipe.AddIngredient(Mod, Containment.sizes[EnchantmentSize] + "Containment", 1);
							}
							else if (j < 3) {
								recipe.AddIngredient(Mod, Containment.sizes[2] + "Containment", 1);
							}
							if (EnchantmentSize == 3) {
								recipe.AddRecipeGroup("WeaponEnchantments:CommonGems", 2);
							}
							if (EnchantmentSize == 4) {
								recipe.AddRecipeGroup("WeaponEnchantments:RareGems");
							}
							recipe.AddTile(Mod, WoodEnchantingTable.enchantingTableNames[i] + "EnchantingTable");
							recipe.Register();
						}
					}
				}
			}
		}
		public int GetLevelCost() {
			int multiplier = 2;
			if (Utility)
				multiplier = 1;
			if (Unique || DamageClassSpecific > 0 || Max1 || ArmorSlotSpecific > -1)
				multiplier = 3;
			return (1 + EnchantmentSize) * multiplier;
		}
	}
	public class AllForOneEnchantmentBasic : Enchantment { }public class AllForOneEnchantmentCommon : Enchantment { }public class AllForOneEnchantmentRare : Enchantment { }public class AllForOneEnchantmentSuperRare : Enchantment { }public class AllForOneEnchantmentUltraRare : Enchantment { }
	public class AmmoCostEnchantmentBasic : Enchantment { }public class AmmoCostEnchantmentCommon : Enchantment { }public class AmmoCostEnchantmentRare : Enchantment { }public class AmmoCostEnchantmentSuperRare : Enchantment { }public class AmmoCostEnchantmentUltraRare : Enchantment { }
	public class ArmorPenetrationEnchantmentBasic : Enchantment { }public class ArmorPenetrationEnchantmentCommon : Enchantment { }public class ArmorPenetrationEnchantmentRare : Enchantment { }public class ArmorPenetrationEnchantmentSuperRare : Enchantment { }public class ArmorPenetrationEnchantmentUltraRare : Enchantment { }
	public class CatastrophicReleaseEnchantmentBasic : Enchantment { }public class CatastrophicReleaseEnchantmentCommon : Enchantment { }public class CatastrophicReleaseEnchantmentRare : Enchantment { }public class CatastrophicReleaseEnchantmentSuperRare : Enchantment { }public class CatastrophicReleaseEnchantmentUltraRare : Enchantment { }
	public class ColdSteelEnchantmentBasic : Enchantment { }public class ColdSteelEnchantmentCommon : Enchantment { }public class ColdSteelEnchantmentRare : Enchantment { }public class ColdSteelEnchantmentSuperRare : Enchantment { }public class ColdSteelEnchantmentUltraRare : Enchantment { }
	public class CriticalStrikeChanceEnchantmentBasic : Enchantment { }public class CriticalStrikeChanceEnchantmentCommon : Enchantment { }public class CriticalStrikeChanceEnchantmentRare : Enchantment { }public class CriticalStrikeChanceEnchantmentSuperRare : Enchantment { }public class CriticalStrikeChanceEnchantmentUltraRare : Enchantment { }
	public class DamageEnchantmentBasic : Enchantment { }public class DamageEnchantmentCommon : Enchantment { }public class DamageEnchantmentRare : Enchantment { }public class DamageEnchantmentSuperRare : Enchantment { }public class DamageEnchantmentUltraRare : Enchantment { }
	public class DangerSenseEnchantmentUltraRare : Enchantment { }
	public class GodSlayerEnchantmentBasic : Enchantment { }public class GodSlayerEnchantmentCommon : Enchantment { }public class GodSlayerEnchantmentRare : Enchantment { }public class GodSlayerEnchantmentSuperRare : Enchantment { }public class GodSlayerEnchantmentUltraRare : Enchantment { }
	public class HellsWrathEnchantmentBasic : Enchantment { }public class HellsWrathEnchantmentCommon : Enchantment { }public class HellsWrathEnchantmentRare : Enchantment { }public class HellsWrathEnchantmentSuperRare : Enchantment { }public class HellsWrathEnchantmentUltraRare : Enchantment { }
	public class HunterEnchantmentUltraRare : Enchantment { }
	public class JunglesFuryEnchantmentBasic : Enchantment { }public class JunglesFuryEnchantmentCommon : Enchantment { }public class JunglesFuryEnchantmentRare : Enchantment { }public class JunglesFuryEnchantmentSuperRare : Enchantment { }public class JunglesFuryEnchantmentUltraRare : Enchantment { }
	public class LifeStealEnchantmentBasic : Enchantment { }public class LifeStealEnchantmentCommon : Enchantment { }public class LifeStealEnchantmentRare : Enchantment { }public class LifeStealEnchantmentSuperRare : Enchantment { }public class LifeStealEnchantmentUltraRare : Enchantment { }
	public class ManaEnchantmentBasic : Enchantment { }public class ManaEnchantmentCommon : Enchantment { }public class ManaEnchantmentRare : Enchantment { }public class ManaEnchantmentSuperRare : Enchantment { }public class ManaEnchantmentUltraRare : Enchantment { }
	public class MaxMinionsEnchantmentBasic : Enchantment { }public class MaxMinionsEnchantmentCommon : Enchantment { }public class MaxMinionsEnchantmentRare : Enchantment { }public class MaxMinionsEnchantmentSuperRare : Enchantment { }public class MaxMinionsEnchantmentUltraRare : Enchantment { }
	public class MoonlightEnchantmentBasic : Enchantment { }public class MoonlightEnchantmentCommon : Enchantment { }public class MoonlightEnchantmentRare : Enchantment { }public class MoonlightEnchantmentSuperRare : Enchantment { }public class MoonlightEnchantmentUltraRare : Enchantment { }
	public class MoveSpeedEnchantmentBasic : Enchantment { }public class MoveSpeedEnchantmentCommon : Enchantment { }public class MoveSpeedEnchantmentRare : Enchantment { }public class MoveSpeedEnchantmentSuperRare : Enchantment { }public class MoveSpeedEnchantmentUltraRare : Enchantment { }
	public class ObsidianSkinEnchantmentUltraRare : Enchantment { }
	public class OneForAllEnchantmentBasic : Enchantment { }public class OneForAllEnchantmentCommon : Enchantment { }public class OneForAllEnchantmentRare : Enchantment { }public class OneForAllEnchantmentSuperRare : Enchantment { }public class OneForAllEnchantmentUltraRare : Enchantment { }
	public class PeaceEnchantmentBasic : Enchantment { }public class PeaceEnchantmentCommon : Enchantment { }public class PeaceEnchantmentRare : Enchantment { }public class PeaceEnchantmentSuperRare : Enchantment { }public class PeaceEnchantmentUltraRare : Enchantment { }
	public class PhaseJumpEnchantmentBasic : Enchantment { }public class PhaseJumpEnchantmentCommon : Enchantment { }public class PhaseJumpEnchantmentRare : Enchantment { }public class PhaseJumpEnchantmentSuperRare : Enchantment { }public class PhaseJumpEnchantmentUltraRare : Enchantment { }
	public class ScaleEnchantmentBasic : Enchantment { }public class ScaleEnchantmentCommon : Enchantment { }public class ScaleEnchantmentRare : Enchantment { }public class ScaleEnchantmentSuperRare : Enchantment { }public class ScaleEnchantmentUltraRare : Enchantment { }
	public class SpeedEnchantmentBasic : Enchantment { }public class SpeedEnchantmentCommon : Enchantment { }public class SpeedEnchantmentRare : Enchantment { }public class SpeedEnchantmentSuperRare : Enchantment { }public class SpeedEnchantmentUltraRare : Enchantment { }
	public class SpelunkerEnchantmentUltraRare : Enchantment { }
	public class SplittingEnchantmentBasic : Enchantment { }public class SplittingEnchantmentCommon : Enchantment { }public class SplittingEnchantmentRare : Enchantment { }public class SplittingEnchantmentSuperRare : Enchantment { }public class SplittingEnchantmentUltraRare : Enchantment { }
	public class StatDefenseEnchantmentBasic : Enchantment { }public class StatDefenseEnchantmentCommon : Enchantment { }public class StatDefenseEnchantmentRare : Enchantment { }public class StatDefenseEnchantmentSuperRare : Enchantment { }public class StatDefenseEnchantmentUltraRare : Enchantment { }
	public class WarEnchantmentBasic : Enchantment { }public class WarEnchantmentCommon : Enchantment { }public class WarEnchantmentRare : Enchantment { }public class WarEnchantmentSuperRare : Enchantment { }public class WarEnchantmentUltraRare : Enchantment { }
	public class WorldAblazeEnchantmentBasic : Enchantment { }public class WorldAblazeEnchantmentCommon : Enchantment { }public class WorldAblazeEnchantmentRare : Enchantment { }public class WorldAblazeEnchantmentSuperRare : Enchantment { }public class WorldAblazeEnchantmentUltraRare : Enchantment { }
}
