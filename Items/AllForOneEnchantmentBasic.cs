using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WeaponEnchantments.Common;
using System.Reflection;

namespace WeaponEnchantments.Items
{
	public enum EnchantmentTypeID : int
	{
		AllForOne,
		AmmoCost,
		ArmorPenetration,
		CriticalStrikeChance,
		Damage,
		DangerSense,
		Defence,
		GodSlayer,
		Hunter,
		LifeSteal,
		Mana,
		OneForAll,
		Peace,
		Scale,
		Speed,
		Spelunker,
		Splitting,
		War,

		Magic,//change
		Summon,//change
	}
	public enum UtilityEnchantmentNames
	{
		AmmoCost,
		DangerSense,
		Hunter,
		LifeSteal,
		Mana,
		Peace,
		Scale,
		Spelunker,
		War
	}
	public enum DamageTypeSpecificID
	{
		None = 0,
		Generic = 1,
		Melee = 2,
		Ranged = 3,
		Magic = 4,
		Summon = 5,
		SummonMeleeSpeed = 6,
		MagicSummonHybrid = 7,
		Throwing = 8
	}
	public class AllForOneEnchantmentBasic : ModItem
	{
		bool ToggleRarityNames = false;

		public static List<int[]> IDs { private set; get; } = new List<int[]>();
		public static readonly string[] rarity = new string[5] { "Basic", "Common", "Rare", "SuperRare", "UltraRare" };
		public static readonly string[] displayRarity = new string[5] { "Basic", "Common", "Rare", "Epic", "Legendary" };
		public static readonly Color[] rarityColors = new Color[5] { Color.White, Color.Green, Color.Blue, Color.Purple, Color.Orange };
		public int EnchantmentSize { private set; get; } = -1;
		public int EnchantmentType { private set; get; } = -1;
		public string EnchantmentTypeName { private set; get; }
		public string MyDisplayName { private set; get; }
		public float EnchantmentStrength { private set; get; }
		public bool Utility { private set; get; }
		public bool Unique { private set; get; }
		public bool Max1 { private set; get; } = false;
		public int DamageClassSpecific { private set; get; }
		public bool StaticStat { private set; get; }
		private bool checkedStats = false;
		public int PotionBuff { private set; get; }	= -1;
		public bool Armor { private set; get; } = false;
		//public bool statsSet = false;
		public List<EnchantmentStaticStat> StaticStats { private set; get; } = new List<EnchantmentStaticStat>();
		public List<EStat> EStats { private set; get; } = new List<EStat>();
		public Dictionary<string, float> AllowedList { private set; get; } = new Dictionary<string, float>();
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
        public override void SetStaticDefaults()
        {
			int[] arr = new int[rarity.Length];
            for(int i = 0; i < Enum.GetNames(typeof(EnchantmentTypeID)).Length; i++)
            {
				IDs.Add(arr);
            }
			GetDefaults();
			string utilityToolTip = Utility ? "\n   *Utility*" : "";
			if (DamageClassSpecific > 0 || Unique)
            {
				string limmitationToolTip;
				switch ((EnchantmentTypeID)EnchantmentType)
				{
					case EnchantmentTypeID.GodSlayer:
						limmitationToolTip = "\n   *Melee Only*";
						break;
					case EnchantmentTypeID.Splitting:
						limmitationToolTip = "\n   *Ranged Only*";
						break;
					case EnchantmentTypeID.Magic:
						limmitationToolTip = "\n   *Magic Only*";
						break;
					case EnchantmentTypeID.Summon:
						limmitationToolTip = "\n   *Summon Only*";
						break;
					default:
						limmitationToolTip = "\n   *" + UtilityMethods.AddSpaces(Item.ModItem.Name) + " Only*";
						break;
				}//DamageTypeSpecific
				limmitationToolTip += "\n   *Unique*\n(Limmited to 1 Unique Enchantment)";
				switch ((EnchantmentTypeID)EnchantmentType)
                {
					case EnchantmentTypeID.GodSlayer:
						Tooltip.SetDefault((EnchantmentStrength * 100).ToString() + "% God Slayer Bonus\n(Bonus damage based on enemy max hp)\n(Bonus damage not affected by LifeSteal against bosses)\nLevel cost: " + GetLevelCost().ToString() + limmitationToolTip + utilityToolTip);
						break;
					default:
						Tooltip.SetDefault("+" + (EnchantmentStrength * 100).ToString() + "% " + MyDisplayName + "\nLevel cost: " + GetLevelCost().ToString() + limmitationToolTip + utilityToolTip);
						break;
				}//Unique ToolTips
			}//DamageTypeSpecific and Unique ToolTips
            else
            {
				switch ((EnchantmentTypeID)EnchantmentType)
				{
					case EnchantmentTypeID.Speed:
						Tooltip.SetDefault("+" + (EnchantmentStrength * 100).ToString() + "% " + MyDisplayName + "\n(Lowers NPC immunity time to raise dps for minions/channeled weapons)\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.Defence:
						Tooltip.SetDefault("+" + EnchantmentStrength.ToString() + " " + MyDisplayName + "\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.ArmorPenetration:
						Tooltip.SetDefault(EnchantmentStrength.ToString() + " Armor Penetration\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.Mana:
						Tooltip.SetDefault("-" + (EnchantmentStrength * 100).ToString() + "% Mana Cost\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.AmmoCost:
						Tooltip.SetDefault("-" + (EnchantmentStrength * 100).ToString() + "% Chance to consume ammo\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.LifeSteal:
						Tooltip.SetDefault((EnchantmentStrength * 100).ToString() + "% Life Steal (remainder is saved to prevent \nalways rounding to 0 for low damage weapons)\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.AllForOne:
						Tooltip.SetDefault(EnchantmentStrength + "x Damage, item CD equal to " + EnchantmentStrength * 0.8f + "x use speed\n" + EnchantmentStrength * 0.4f + "x mana cost\n(Raises NPC immunity time to lower dps for minions/channeled weapons)\n   *Weapons Only*\n   *Max of 1 per weapon*\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.OneForAll:
						Tooltip.SetDefault("Hiting an enemy will damage all nearby enemies by " + (EnchantmentStrength * 100).ToString() + "% of damage dealt, " + (30f * EnchantmentStrength).ToString() + "% reduced base attack speed\n   *Weapons Only*\n   *Max of 1 per weapon*\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.Spelunker:
						Tooltip.SetDefault("Grants the Spelunker buff\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.DangerSense:
						Tooltip.SetDefault("Grants the Danger Sense buff\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.Hunter:
						Tooltip.SetDefault("Grants the Hunter buff\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.War:
						Tooltip.SetDefault((EnchantmentStrength + 1f).ToString() + "x enemy spawn rate and max enemies\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.Peace:
						Tooltip.SetDefault((1f / (EnchantmentStrength + 1f)).ToString() + "x enemy spawn rate and max enemies\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					case EnchantmentTypeID.Splitting:
						Tooltip.SetDefault("+" + (EnchantmentStrength * 100).ToString() + "% to produce an extra projectile.\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
					default:
						Tooltip.SetDefault("+" + (EnchantmentStrength * 100).ToString() + "% " + MyDisplayName + "\nLevel cost: " + GetLevelCost().ToString() + utilityToolTip);
						break;
				}//Normal ToolTips
			}//Normal ToolTips
			if(EnchantmentSize > 2 && ToggleRarityNames)
				DisplayName.SetDefault(UtilityMethods.AddSpaces(MyDisplayName + "Enchantment" + displayRarity[EnchantmentSize]));
			else
				DisplayName.SetDefault(UtilityMethods.AddSpaces(MyDisplayName + Name.Substring(Name.IndexOf("Enchantment"))));
		}
		private void GetDefaults()
        {
			EnchantmentTypeName = Name.Substring(0, Name.IndexOf("Enchantment"));
			EnchantmentSize = GetEnchantmentSize(Name);
			for (int i = 0; i < Enum.GetNames(typeof(EnchantmentTypeID)).Length; i++)
			{
				if (EnchantmentTypeName == ((EnchantmentTypeID)i).ToString())
				{
					EnchantmentType = i;
					break;
				}
			}//Check EnchantmentType
			for (int i = 0; i < Enum.GetNames(typeof(UtilityEnchantmentNames)).Length; i++)
			{
				if (EnchantmentTypeName == ((UtilityEnchantmentNames)i).ToString())
				{
					Utility = true;
					break;
				}
			}//Check Utility
			for (int i = 0; i < ItemID.Count; i++)
			{
				if (ContentSamples.ItemsByType[i].Name == EnchantmentTypeName)
				{
					Unique = true;
					break;
				}
			}//Check Unique (Vanilla Items)
            switch (EnchantmentSize)
            {
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
			}//Width/Height
			int endSize;
			switch ((EnchantmentTypeID)EnchantmentType)
			{
				case EnchantmentTypeID.Spelunker:
				case EnchantmentTypeID.DangerSense:
				case EnchantmentTypeID.Hunter:
					endSize = EnchantmentSize - 2;
					break;
				default:
					endSize = EnchantmentSize;
					break;
			}//Base Value
			for (int i = 0; i < endSize; i++)
			{
				Item.value += (int)EnchantmentEssenceBasic.values[i] * (Utility ? 5 : 10);
			}//Essence Value
			switch (EnchantmentSize)
			{
				case 3:
					Item.value += Containment.Values[2];
					break;
				case 4:
					Item.value += Containment.Values[2] + Stabilizer.Values[1] * 4;
					break;
				default:
					Item.value += Containment.Values[EnchantmentSize];
					break;
			}//Value - Containment/SuperiorStaibalizers
			switch ((EnchantmentTypeID)EnchantmentType)
			{
				case EnchantmentTypeID.Scale:
				case EnchantmentTypeID.War:
				case EnchantmentTypeID.Peace:
				case EnchantmentTypeID.OneForAll:
					switch (EnchantmentSize)
					{
						case 0:
							EnchantmentStrength = 0.1f;
							break;
						case 1:
							EnchantmentStrength = 0.2f;
							break;
						case 2:
							EnchantmentStrength = 0.50f;
							break;
						case 3:
							EnchantmentStrength = 0.8f;
							break;
						case 4:
							EnchantmentStrength = 1f;
							break;
					}
					break;
				case EnchantmentTypeID.Defence:
					switch (EnchantmentSize)
					{
						case 0:
							EnchantmentStrength = 1f;
							break;
						case 1:
							EnchantmentStrength = 2f;
							break;
						case 2:
							EnchantmentStrength = 3f;
							break;
						case 3:
							EnchantmentStrength = 5f;
							break;
						case 4:
							EnchantmentStrength = 10f;
							break;
					}
					break;
				case EnchantmentTypeID.ArmorPenetration:
					switch (EnchantmentSize)
					{
						case 0:
							EnchantmentStrength = 2f;
							break;
						case 1:
							EnchantmentStrength = 4f;
							break;
						case 2:
							EnchantmentStrength = 8f;
							break;
						case 3:
							EnchantmentStrength = 10f;
							break;
						case 4:
							EnchantmentStrength = 20f;
							break;
					}
					break;
				case EnchantmentTypeID.LifeSteal:
					switch (EnchantmentSize)
					{
						case 0:
							EnchantmentStrength = 0.005f;
							break;
						case 1:
							EnchantmentStrength = 0.01f;
							break;
						case 2:
							EnchantmentStrength = 0.02f;
							break;
						case 3:
							EnchantmentStrength = 0.03f;
							break;
						case 4:
							EnchantmentStrength = 0.04f;
							break;
					}
					break;
				case EnchantmentTypeID.AllForOne:
					switch (EnchantmentSize)
					{
						case 0:
							EnchantmentStrength = 1f;
							break;
						case 1:
							EnchantmentStrength = 2f;
							break;
						case 2:
							EnchantmentStrength = 5f;
							break;
						case 3:
							EnchantmentStrength = 8f;
							break;
						case 4:
							EnchantmentStrength = 10f;
							break;
					}
					break;
				case EnchantmentTypeID.GodSlayer:
					switch (EnchantmentSize)
					{
						case 0:
							EnchantmentStrength = 0.02f;
							break;
						case 1:
							EnchantmentStrength = 0.04f;
							break;
						case 2:
							EnchantmentStrength = 0.06f;
							break;
						case 3:
							EnchantmentStrength = 0.08f;
							break;
						case 4:
							EnchantmentStrength = 0.10f;
							break;
					}
					break;
				case EnchantmentTypeID.Splitting:
					switch (EnchantmentSize)
					{
						case 0:
							EnchantmentStrength = 0.5f;
							break;
						case 1:
							EnchantmentStrength = 0.6f;
							break;
						case 2:
							EnchantmentStrength = 0.75f;
							break;
						case 3:
							EnchantmentStrength = 0.85f;
							break;
						case 4:
							EnchantmentStrength = 1f;
							break;
					}
					break;
				case EnchantmentTypeID.Magic:
					switch (EnchantmentSize)
					{
						case 0:
							EnchantmentStrength = 0.03f;
							break;
						case 1:
							EnchantmentStrength = 0.08f;
							break;
						case 2:
							EnchantmentStrength = 0.16f;
							break;
						case 3:
							EnchantmentStrength = 0.25f;
							break;
						case 4:
							EnchantmentStrength = 0.40f;
							break;
					}
					break;
				case EnchantmentTypeID.Summon:
					switch (EnchantmentSize)
					{
						case 0:
							EnchantmentStrength = 0.03f;
							break;
						case 1:
							EnchantmentStrength = 0.08f;
							break;
						case 2:
							EnchantmentStrength = 0.16f;
							break;
						case 3:
							EnchantmentStrength = 0.25f;
							break;
						case 4:
							EnchantmentStrength = 0.40f;
							break;
					}
					break;
				default:
					switch (EnchantmentSize)
					{
						case 0:
							EnchantmentStrength = 0.03f;
							break;
						case 1:
							EnchantmentStrength = 0.08f;
							break;
						case 2:
							EnchantmentStrength = 0.16f;
							break;
						case 3:
							EnchantmentStrength = 0.25f;
							break;
						case 4:
							EnchantmentStrength = 0.40f;
							break;
					}
					break;
			}//EnchantmentStrength
			switch ((EnchantmentTypeID)EnchantmentType)
			{
				case EnchantmentTypeID.GodSlayer:
					DamageClassSpecific = (int)DamageTypeSpecificID.Melee;
					break;
				case EnchantmentTypeID.Splitting:
					DamageClassSpecific = (int)DamageTypeSpecificID.Ranged;
					break;
				case EnchantmentTypeID.Magic:
					DamageClassSpecific = (int)DamageTypeSpecificID.Magic;
					break;
				case EnchantmentTypeID.Summon:
					DamageClassSpecific = (int)DamageTypeSpecificID.Summon;
					break;
				case EnchantmentTypeID.AllForOne:
				case EnchantmentTypeID.OneForAll:
					Max1 = true;
					break;
				default:
					DamageClassSpecific = 0;
					break;
			}//DamageTypeSpecific
			switch ((EnchantmentTypeID)EnchantmentType)
			{
				case EnchantmentTypeID.Scale:
					MyDisplayName = "Size";
					break;
				default:
					MyDisplayName = EnchantmentTypeName;
					break;
			}//New Display Name
		}
		public override void SetDefaults()
		{
			Item.maxStack = 99;
			GetDefaults();
			if(!checkedStats)
            {
				switch ((EnchantmentTypeID)EnchantmentType)
				{
					case EnchantmentTypeID.Defence:
						AllowedList.Add("Weapon", 0.5f);
						AllowedList.Add("Armor", 1f);
						AllowedList.Add("Accessory", 1f);
						break;
					case EnchantmentTypeID.Peace:
					case EnchantmentTypeID.War:
						AllowedList.Add("Weapon", 1f);
						AllowedList.Add("Armor", 1f);
						AllowedList.Add("Accessory", 1f);
						break;
					case EnchantmentTypeID.AllForOne:
					case EnchantmentTypeID.OneForAll:
						AllowedList.Add("Weapon", 1f);
						break;
					default:
						AllowedList.Add("Weapon", 1f);
						AllowedList.Add("Armor", 0.25f);
						AllowedList.Add("Accessory", 0.25f);
						break;
				}//AllowedList
				switch ((EnchantmentTypeID)EnchantmentType)
				{
					case EnchantmentTypeID.AllForOne:
						AddStaticStat("damage", 0f, EnchantmentStrength);
						AddStaticStat("useSpeed", EnchantmentStrength * 0.8f);
						AddStaticStat("useAnimation", EnchantmentStrength * 0.8f);
						EStats.Add(new EStat("NPCHitCooldown", EnchantmentStrength * 0.8f));
						AddStaticStat("mana", EnchantmentStrength * 0.4f);
						StaticStat = AddStaticStat("P_autoReuse");
						break;
					case EnchantmentTypeID.AmmoCost:
					case EnchantmentTypeID.LifeSteal:
						EStats.Add(new EStat(EnchantmentTypeName, 0f, 1f, 0f, EnchantmentStrength));
						break;
					case EnchantmentTypeID.ArmorPenetration:
					case EnchantmentTypeID.CriticalStrikeChance:
					case EnchantmentTypeID.Damage:
					case EnchantmentTypeID.Scale:
						StaticStat = CheckStaticStatByName();
						break;
					case EnchantmentTypeID.DangerSense:
					case EnchantmentTypeID.Hunter:
					case EnchantmentTypeID.Spelunker:
						CheckPotionBuffByName();
						break;
					case EnchantmentTypeID.Mana:
						AddStaticStat(EnchantmentTypeName, -EnchantmentStrength);
						break;
					case EnchantmentTypeID.OneForAll:
						EStats.Add(new EStat(EnchantmentTypeName, 0f, EnchantmentStrength));
						AddStaticStat("useSpeed", EnchantmentStrength * 0.3f);
						AddStaticStat("useAnimation", EnchantmentStrength * 0.3f);
						EStats.Add(new EStat("NPCHitCooldown", EnchantmentStrength * 0.3f));
						break;
					case EnchantmentTypeID.Peace:
					case EnchantmentTypeID.War:
						EStats.Add(new EStat("spawnRate", 0f, EnchantmentStrength));
						EStats.Add(new EStat("maxSpawns", 0f, EnchantmentStrength));
						break;
					case EnchantmentTypeID.Speed:
						StaticStat = AddStaticStat("autoReuse");
						AddStaticStat("I_useSpeed", EnchantmentStrength);
						AddStaticStat("I_useAnimation", EnchantmentStrength);
						EStats.Add(new EStat("I_NPCHitCooldown", EnchantmentStrength));
						break;
					default:
						EStats.Add(new EStat(EnchantmentTypeName, EnchantmentStrength, 1f));
						break;
				}//SetStats
				StaticStat = StaticStats.Count > 0;
				checkedStats = true;
			}//Set StaticStats
		}
		private bool CheckStaticStatByName()
		{
			foreach (FieldInfo field in Item.GetType().GetFields())
			{
				string fieldName = field.Name;
				if(fieldName.Length < Name.Length)
                {
					string name = UtilityMethods.ToFieldName(Name.Substring(0, fieldName.Length));
					if (fieldName == name)
					{
                        switch (name)
                        {
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
			foreach (PropertyInfo property in Item.GetType().GetProperties())
			{
				string name = property.Name;
				if (name.Length < Name.Length)
                {
					if (name == Name.Substring(0, name.Length))
					{
						StaticStats.Add(new EnchantmentStaticStat(name, EnchantmentStrength));
						return true;
					}
				}
			}
			return false;
		}
		private bool AddStaticStat(string name, float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f)
        {
			StaticStats.Add(new EnchantmentStaticStat(name, additive, multiplicative, flat, @base));
			return true;
		}
		private bool CheckPotionBuffByName()
        {
			BuffID buffID = new();
			foreach(FieldInfo field in buffID.GetType().GetFields())
            {
				string fieldName = field.Name;
				if (fieldName.Length < Name.Length)
				{
					string name = Name.Substring(0, fieldName.Length);
					if (fieldName.ToLower() == name.ToLower())
					{
						PotionBuff = (int)buffID.GetType().GetField(fieldName).GetValue(buffID);
						//StaticStats.Add(new StaticStatStruct(fieldName, EnchantmentStrength));
						return true;
					}
				}
			}
			return false;
        }
		public static int GetEnchantmentSize(string name)
        {
			for (int i = 0; i < rarity.Length; i++)
			{
				if (rarity[i] == name.Substring(name.IndexOf("Enchantment") + 11))
				{
					return i;
				}
			}//Get EnchantmentSize
			return -1;
		}
		public override void AddRecipes()
		{
			if (EnchantmentSize > -1)
			{
				for (int i = EnchantmentSize; i < rarity.Length; i++)
				{
					Recipe recipe;
					int skipIfLessOrEqualToSize;
					switch ((EnchantmentTypeID)EnchantmentType)
					{
						case EnchantmentTypeID.Spelunker:
						case EnchantmentTypeID.DangerSense:
						case EnchantmentTypeID.Hunter:
							skipIfLessOrEqualToSize = 4;
							break;
						case EnchantmentTypeID.Damage:
						case EnchantmentTypeID.Defence:
							skipIfLessOrEqualToSize = -1;
							break;
						default:
							skipIfLessOrEqualToSize = 0;
							break;
					}
					if (EnchantmentSize > skipIfLessOrEqualToSize)
					{
						for (int j = EnchantmentSize; j >= skipIfLessOrEqualToSize + 1; j--)
						{
							recipe = CreateRecipe();
							for (int k = EnchantmentSize; k >= j; k--)
							{
								int essenceNumber = Utility ? 5 : 10;
								recipe.AddIngredient(Mod, "EnchantmentEssence" + EnchantmentEssenceBasic.rarity[k], essenceNumber);
							}
							if (j > 0)
							{
								recipe.AddIngredient(Mod, EnchantmentTypeName + "Enchantment" + rarity[j - 1], 1);
							}
							if (EnchantmentSize < 3)
							{
								recipe.AddIngredient(Mod, Containment.sizes[EnchantmentSize] + "Containment", 1);
							}
							else if (j < 3)
							{
								recipe.AddIngredient(Mod, Containment.sizes[2] + "Containment", 1);
							}
							if (EnchantmentSize == 4)
							{
								recipe.AddIngredient(ModContent.ItemType<SuperiorStabilizer>(), 4);
							}
							recipe.AddTile(Mod, WoodEnchantingTable.enchantingTableNames[i] + "EnchantingTable");
							recipe.Register();
						}
					}
				}
				IDs[EnchantmentType][EnchantmentSize] = Type;
			}
		}
		public int GetLevelCost()
        {
            switch ((EnchantmentTypeID)EnchantmentType)
            {
				case EnchantmentTypeID.AllForOne:
				case EnchantmentTypeID.OneForAll:
				case EnchantmentTypeID.Splitting:
					return (1 + EnchantmentSize) * 3;
				default:
					return Utility ? (1 + EnchantmentSize) * 1 : (1 + EnchantmentSize) * 2;
			}
        }
	}
	public class AllForOneEnchantmentCommon : AllForOneEnchantmentBasic { }public class AllForOneEnchantmentRare : AllForOneEnchantmentBasic { }public class AllForOneEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class AllForOneEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class AmmoCostEnchantmentBasic : AllForOneEnchantmentBasic { }public class AmmoCostEnchantmentCommon : AllForOneEnchantmentBasic { }public class AmmoCostEnchantmentRare : AllForOneEnchantmentBasic { }public class AmmoCostEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class AmmoCostEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class ArmorPenetrationEnchantmentBasic : AllForOneEnchantmentBasic { }public class ArmorPenetrationEnchantmentCommon : AllForOneEnchantmentBasic { }public class ArmorPenetrationEnchantmentRare : AllForOneEnchantmentBasic { }public class ArmorPenetrationEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class ArmorPenetrationEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class CriticalStrikeChanceEnchantmentBasic : AllForOneEnchantmentBasic { }public class CriticalStrikeChanceEnchantmentCommon : AllForOneEnchantmentBasic { }public class CriticalStrikeChanceEnchantmentRare : AllForOneEnchantmentBasic { }public class CriticalStrikeChanceEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class CriticalStrikeChanceEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class DamageEnchantmentBasic : AllForOneEnchantmentBasic { }public class DamageEnchantmentCommon : AllForOneEnchantmentBasic { }public class DamageEnchantmentRare : AllForOneEnchantmentBasic { }public class DamageEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class DamageEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class DangerSenseEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class DefenceEnchantmentBasic : AllForOneEnchantmentBasic { }public class DefenceEnchantmentCommon : AllForOneEnchantmentBasic { }public class DefenceEnchantmentRare : AllForOneEnchantmentBasic { }public class DefenceEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class DefenceEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class GodSlayerEnchantmentBasic : AllForOneEnchantmentBasic { }public class GodSlayerEnchantmentCommon : AllForOneEnchantmentBasic { }public class GodSlayerEnchantmentRare : AllForOneEnchantmentBasic { }public class GodSlayerEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class GodSlayerEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class HunterEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class LifeStealEnchantmentBasic : AllForOneEnchantmentBasic { }public class LifeStealEnchantmentCommon : AllForOneEnchantmentBasic { }public class LifeStealEnchantmentRare : AllForOneEnchantmentBasic { }public class LifeStealEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class LifeStealEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class ManaEnchantmentBasic : AllForOneEnchantmentBasic { }public class ManaEnchantmentCommon : AllForOneEnchantmentBasic { }public class ManaEnchantmentRare : AllForOneEnchantmentBasic { }public class ManaEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class ManaEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class OneForAllEnchantmentBasic : AllForOneEnchantmentBasic { }public class OneForAllEnchantmentCommon : AllForOneEnchantmentBasic { }public class OneForAllEnchantmentRare : AllForOneEnchantmentBasic { }public class OneForAllEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class OneForAllEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class PeaceEnchantmentBasic : AllForOneEnchantmentBasic { }public class PeaceEnchantmentCommon : AllForOneEnchantmentBasic { }public class PeaceEnchantmentRare : AllForOneEnchantmentBasic { }public class PeaceEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class PeaceEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class ScaleEnchantmentBasic : AllForOneEnchantmentBasic { }public class ScaleEnchantmentCommon : AllForOneEnchantmentBasic { }public class ScaleEnchantmentRare : AllForOneEnchantmentBasic { }public class ScaleEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class ScaleEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class SpeedEnchantmentBasic : AllForOneEnchantmentBasic { }public class SpeedEnchantmentCommon : AllForOneEnchantmentBasic { }public class SpeedEnchantmentRare : AllForOneEnchantmentBasic { }public class SpeedEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class SpeedEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class SpelunkerEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class SplittingEnchantmentBasic : AllForOneEnchantmentBasic { }public class SplittingEnchantmentCommon : AllForOneEnchantmentBasic { }public class SplittingEnchantmentRare : AllForOneEnchantmentBasic { }public class SplittingEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class SplittingEnchantmentUltraRare : AllForOneEnchantmentBasic { }
	public class WarEnchantmentBasic : AllForOneEnchantmentBasic { }public class WarEnchantmentCommon : AllForOneEnchantmentBasic { }public class WarEnchantmentRare : AllForOneEnchantmentBasic { }public class WarEnchantmentSuperRare : AllForOneEnchantmentBasic { }public class WarEnchantmentUltraRare : AllForOneEnchantmentBasic { }
}
