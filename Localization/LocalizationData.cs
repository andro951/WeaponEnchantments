using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Items;
using WeaponEnchantments.Items.Enchantments;
using WeaponEnchantments.Items.Utility;
using static Terraria.Localization.GameCulture;

namespace WeaponEnchantments.Localization
{
	public class LocalizationData
	{
		public static SortedDictionary<string, SData> All { 
			get {
				if (all == null)
					all = AllData;

				return all;
			}
		}
		private static SortedDictionary<string, SData> all;

		static CultureName CultureName = CultureName.Unknown;
		
		public static bool ContainsTextVAlue(string s, CultureName cultureName) {
			if (cultureName != CultureName)
				LanguageManager.Instance.SetLanguage((int)cultureName);


			bool returnValue = s == Language.GetTextValue(s);
			LanguageManager.Instance.SetLanguage((int)CultureName.English);
			return returnValue;
		}
		//public static bool ContainsText(string s, ) => s == Language.GetText(s).;
		public static List<string> autoFill = new() {
			"EnchantmentEffects"
		};

		private static SortedDictionary<string, SData> allData;
		public static SortedDictionary<string, SData> AllData { 
			get {
				if (allData == null) {
					allData = new() {
						{ L_ID1.ItemTooltip.ToString(), new(dict: new() {
							//Intentionally empty.  Filled automatically
						}) },
						{ L_ID1.Tooltip.ToString(), new(children: new() {
							{ L_ID2.EffectDisplayName.ToString(), new(dict: new() {
								{ $"{typeof(AmmoCost).Name}1", "Chance To Not Consume Ammo" },
								{ $"{typeof(AmmoCost).Name}2", "Increased Ammo Cost" },
								{ $"{typeof(ManaUsage).Name}1", "Reduced Mana Usage" },
								{ $"{typeof(ManaUsage).Name}2", "Increased Mana Usage" },
								{ typeof(BuffDuration).Name, "Extends the duration of buffs by {0}" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnTickPlayerBuff}", "Passively grants {0} to you" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnTickPlayerDebuff}", "Passively inflicts {0} to you" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnTickAreaTeamBuff}", "Passively grants {0} to nearby players" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnTickAreaTeamDebuff}", "Passively inflicts {0} to nearby players" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnTickEnemyBuff}", "Passively grants {0} to enemy" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnTickEnemyDebuff}", "Passively inflicts {0} to enemy" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnTickAreaEnemyBuff}", "Passively grants {0} to nearby enemies" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnTickAreaEnemyDebuff}", "Passively inflicts {0} to nearby enemies" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnHitPlayerBuff}", "Grants you {0} on hit" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnHitPlayerDebuff}", "Inflicts {0} to you on hit" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnHitEnemyBuff}", "Grants {0} to enemies on hit" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnHitEnemyDebuff}", "Inflicts {0} to enemies on hit" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnHitAreaTeamBuff}", "Grants {0} to nearby players on hit" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnHitAreaTeamDebuff}", "Inflicts {0} to nearby players on hit" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnHitAreaEnemyBuff}", "Grants {0} to nearby enemies on hit" },
								{ $"{typeof(BuffEffect).Name}{(int)BuffStyle.OnHitAreaEnemyDebuff}", "Passively inflicts {0} to nearby enemies on hit" },
								{ typeof(DamageAfterDefenses).Name, "Damage (Applied after defenses. Not visible in weapon tooltip)" },
								{ typeof(DamageClassSwap).Name, "Convert damage type to {0}"},
								{ $"{typeof(VanillaDash).Name}{(int)DashID.NinjaTabiDash}", $"{DashID.NinjaTabiDash}".AddSpaces() },
								{ $"{typeof(VanillaDash).Name}{(int)DashID.EyeOfCthulhuShieldDash}", $"{DashID.EyeOfCthulhuShieldDash}".AddSpaces() },
								{ $"{typeof(VanillaDash).Name}{(int)DashID.SolarDash}", $"{DashID.SolarDash}".AddSpaces() },
								{ $"{typeof(VanillaDash).Name}{(int)DashID.CrystalNinjaDash}", $"{DashID.CrystalNinjaDash}".AddSpaces() }
							}) },
							{ L_ID2.EnchantmentEffects.ToString(), new(
								children: new() {
									{ typeof(BoolEffect).Name, new(dict: new() {
										{ "Enabled", "{0}: Enabled" },
										{ "Prevented", "{0}: Prevented" }
									}) },
									{ typeof(BuffEffect).Name, new(dict: new() {
										{ $"{BuffStyle.All}", "{0} ({1}% chance to apply for {2})" },
										{ $"{BuffStyle.OnTickPlayerBuff}", "{0} ({1}% chance to apply for {2} every {3})" }
									}) },
									{ typeof(PlayerSetEffect).Name, new(dict: new() {
										{ "Enabled", "{0}: Enabled" },
										{ "Prevented", "{0}: Prevented" }
									})}
								},
								dict: new() {
									{ typeof(AllForOne).Name, "(Item CD equal to {0}x use speed)" },
									{ typeof(AmmoCost).Name, "{0} (Also Saves Bait When Fishing)" },
									{ typeof(AttackSpeed).Name, "{0} (Affects minion fire rate if they shoot projectiles.  Affects how fast fish will bite the fishing line.)" },
									{ typeof(BonusCoins).Name, "{0} (Hitting an enemy will increase the number of coins it will drop on death based on damage dealt, enemy max health, enemy base value, and luck.)" },
									{ typeof(BuffDuration).Name,
										"{0}\n" +
										"(Duration is continuously increased while the buff is active, not uppon first gaining the buff.)"
									},
									{ typeof(EnemySpawnRate).Name,
										"{0}\n" +
										"(Minion Damage is reduced by your spawn rate multiplier, from enchantments, unless they are your minion attack target)\n" +
										"(minion attack target set from hitting enemies with whips or a weapon that is converted to summon damage from an enchantment)\n" +
										"(Prevents consuming boss summoning items if spawn rate multiplier, from enchantments, is > 1.6)\n" +
										"(Enemies spawned will be immune to lava/traps)"},
									{ typeof(FishingEnemySpawnChance).Name, "{0} (Reduced by 5x during the day.  Affected by Chum Caster.  Can also spawn Duke Fishron.)" },
									{ typeof(GodSlayer).Name, "{0}\n(Bonus true damage based on enemy max hp)\n(Bonus damage not affected by LifeSteal)" },
									{ typeof(LavaFishing).Name, "{0} (Allows fishing in lava and has a chance to improve catch rates in lava.  Stacks with other souces.)"},
									{ typeof(LifeSteal).Name, "{0} (remainder is saved to prevent always rounding to 0 for low damage weapons)" },
									{ typeof(MinionAttackTarget).Name, "Enemies hit become the minion attack target.  Same effect as whips."},
									{ typeof(Multishot).Name, "{0} (Chance to produce an extra projectile.  Applies to each projectile created.)" },
									{ typeof(OneForAll).Name, "{0}\n(Hitting an enemy will damage all nearby enemies)\n(Only activates on the first hit from a projectile.)" },
									{ typeof(OnHitSpawnProjectile).Name, "Spawns a projectile when hitting an enemy: {0}"},
									{ typeof(QuestFishChance).Name, "{0} (Quest fish caught will be automatically turned in and start a new quest, bypassing the 1 per day limmit.)"}
								}
							) },
							{ L_ID2.EnchantmentCustomTooltips.ToString(), new(dict: new() {
								{ typeof(WorldAblazeEnchantment).Name.ToEnchantmentTypeName(),
									 "(Amaterasu debuff and below notes about it only apply at Enchantment tier 4.)\n" +
									"(None shall survive the unstopable flames of Amaterasu)\n" +
									"(Inflict a unique fire debuff to enemies that never stops)\n" +
									"(The damage from the debuff grows over time and from dealing more damage to the target)\n" +
									"(Spreads to nearby enemies and prevents enemies from being immune to other World Ablaze debuffs.)" },
							}) },
							{ L_ID2.EnchantmentShortTooltip.ToString(), new(dict: new() {
								{ typeof(OnTickPlayerBuffEnchantment).Name.ToEnchantmentTypeName(), "Passively grants {0} for {1} every {2}" }
							})},
							{ L_ID2.EnchantmentGeneralTooltips.ToString(), new(dict: new() {
								{ $"{EnchantmentGeneralTooltipsID.LevelCost}", "Level cost: {0}" },
								{ $"{EnchantmentGeneralTooltipsID.Unique}", "Unique (Limited to 1 Unique Enchantment)" },
								{ $"{EnchantmentGeneralTooltipsID.Only}", "{0} Only" },
								{ $"{EnchantmentGeneralTooltipsID.ArmorSlotOnly}", "{0} armor slot Only" },
								{ $"{EnchantmentGeneralTooltipsID.NotAllowed}", "Not allowed on {0} weapons" },
								{ $"{EnchantmentGeneralTooltipsID.Max1}", "Max of 1 per item" },
								{ $"{EnchantmentGeneralTooltipsID.Utility}", "Utility" },
								{ $"{EnchantmentGeneralTooltipsID.Or}", "or" },
								{ $"{EnchantmentGeneralTooltipsID.OnlyAllowedOn}", "Only allowed on" },
								{ $"{EnchantmentGeneralTooltipsID.AllowedOn}", "Allowed on" },
								{ $"{EnchantmentGeneralTooltipsID.And}", "and" }
							}) },
							{ L_ID2.ItemType.ToString(), new(values: new() {
								//Filled Automatically
							}) }
						}) },
						{ L_ID1.Dialogue.ToString(), new(children: new() {
							{ L_ID2.Witch.ToString(), new(dict: new() {
								{ $"{DialogueID.StandardDialogue}0", "I've learned this recipe, it's really neat, I'll keep it later for your treat!" },
								{ $"{DialogueID.StandardDialogue}1", "Why do I talk all the time, it's really hard to make these rhyme!" },
								{ $"{DialogueID.StandardDialogue}2", "I'm still here, I watch you play, but I can't think of much to say!" },
								{ $"{DialogueID.BloodMoon}", "I'm truely sorry for my prior behavior. I wasn't at my best back then, and if I came off as rude, I apologize." },
								{ $"{DialogueID.BirthdayParty}", "WHY AM I FORCED TO DRESS PINK?! THIS LOOKS AWFUL, THIS PARTY STINKS!" },
								{ $"{DialogueID.Storm}", "Hah! Thunder is the best when it comes to creating new magical nonsense! Let the grand witching hour commence!" },
								{ $"{DialogueID.QueenBee}", "The jungle has the best ingredients for brewing. Too bad a giant buzzy pest is infesting!" },
								{ $"{BiomeID.Corrupt}", "Oooh! The grime here is lovely! I can't wait to embrace new discovery!" },
								{ $"{BiomeID.Crimson}", "Oooh! The gore here is lovely! I can't wait to embrace new discovery!" },
								{ $"{BiomeID.Graveyard}", "Some people might find the dead creepy, but this 'dead quiet' is actually quite peppy! Nyack nyack nyack!" },
								{ TownNPCTypeID.WitchDoctor.ToString(), "A fellow cauldron user! {0} has so many strange gimmicks, a true seducer!" },
								{ TownNPCTypeID.Wizard.ToString(), "While {0} does not venture outside the tried-and-true, his arcane knowledge ranks him of that of a guru!" },
								{ TownNPCTypeID.DyeTrader.ToString(), "{0} always hooks me up with the darkest blacks, and the creepiest greens! These colors give me quite the sheen and make me look so lean!" },
								{ TownNPCTypeID.Princess.ToString(), "Ugh! {0} is always on my case! I don't know what 'Ethics' or 'Morals' are, but she won't leave my place!" },
								{ TownNPCTypeID.Dryad.ToString(), "This... {0} person is so cliche! 'Save the world from evil' The evils are deep-rooted, and here to stay!" },
								{ TownNPCTypeID.Zoologist.ToString(), "{0} is a tyrannous wench that keeps leaving hair everywhere, I will make her pay in warfare!" },
							}) }
						}) },
						{ L_ID1.TownNPCMood.ToString(), new(children: new() {
							{ L_ID2.Witch.ToString(), new(dict: new() {
								{ $"{DialogueID.Content}", "Life here is bearable. No strife, no drama, it's not so horrible." },
								{ DialogueID.NoHome.ToString(), "I can't hold all my wares, these are unstable please beware!" },
								{ DialogueID.LoveSpace.ToString(), "With everyone now gone, finally my experiments can carry on!" },
								{ DialogueID.FarFromHome.ToString(), "I had to venture far to find new reagants.While my home is pleasant, I can brew a new concoction with these last ingredients." },
								{ DialogueID.DislikeCrowded.ToString(), "How am I to consentrate with all this noise!" },
								{ DialogueID.HateCrouded.ToString(), "I can't deal with this here traffic! Get me out or kick them out, make it quick!" },
								{ DialogueID.LikeBiome.ToString(), "This place is full of critters and muck, time to make a quick buck!" },
								{ DialogueID.LoveBiome.ToString(), "This place reminds me of my old lair. Very grim, very magic, what a flair!" },
								{ DialogueID.DislikeBiome.ToString(), "It's incredible how here is so tame, no evil, no creepies, it's so lame!" },
								{ DialogueID.HateBiome.ToString(), "Yikes! This place is so bright, that above all I spite!" },
								{ DialogueID.LikeNPC.ToString(), "While {NPCName} does not venture outside the tried-and-true, his arcane knowledge ranks him of that of a guru!" },
								{ DialogueID.LoveNPC.ToString(), "A fellow cauldron user! {NPCName} has so many strange gimmicks, a true seducer!" },
								{ DialogueID.DislikeNPC.ToString(), "This... {NPCName} person is so cliche! 'Save the world from evil' The evils are deep-rooted, and here to stay!" },
								{ DialogueID.HateNPC.ToString(), "{NPCName} is a tyrannous wench that keeps leaving hair everywhere, I will make her pay in warfare!" }
							}) }
						}) },
						{ L_ID1.NPCNames.ToString(), new(children: new() {
							{ L_ID2.Witch.ToString(), new(new() {
								{ "Gruntilda" },
								{ "Brentilda" },
								{ "Blobbelda" },
								{ "Mingella" },
								{ "MissGulch" },
								{ "Sabrina" },
								{ "Winifred" },
								{ "Sarah" },
								{ "Mary" },
								{ "Maleficient" },
								{ "Salem" },
								{ "Binx" },
								{ "Medusa" },
								{ "Melusine" },
								{ "Ursula" },
								{ "Jasminka" },
								{ "Agatha" },
								{ "Freyja" },
								{ "Hazel" },
								{ "Akko" },
								{ "Kyubey" },
								{ "Morgana" }
							}) }
						}) },
						{ L_ID1.Bestiary.ToString(), new(dict: new() {
							{ L_ID2.Witch.ToString(), "The Witch is an ugly and wicked lady and deals in further wicked things. She trades in dubious materials used to improve equipment with mysterious effects." }
						}) },
						{ L_ID1.Ores.ToString(), new(new() {
							{ "copper" },
							{ "tin" },
							{ "iron" },
							{ "lead" },
							{ "silver" },
							{ "tungsten" },
							{ "gold" },
							{ "platinum" },
							{ "demonite" },
							{ "crimtane" },
							{ "cobalt" },
							{ "palladium" },
							{ "mythril" },
							{ "orichalcum" },
							{ "adamantite" },
							{ "titanium" },
							{ "chlorophyte" }
						}) },
						{ L_ID1.TableText.ToString(), new(
							values: new() {
								//Filled Automatically
							},
							dict: new() {
								{ $"{TableTextID.AreYouSure}",
									"Are you sure you want to PERMENANTLY DESTROY your level {0}\n" +
									"{1} {2}\n" +
									"{3}(Based on item value/experience.  Enchantments will be returned.)"},
								{ $"{TableTextID.ExchangeEssence}", "In exchange for essence?" },
								{ $"{TableTextID.ExchangeOres}", "In exchange for ores?" },
								{ $"{TableTextID.ExchangeEssenceAndOres}", "In exchange for ores({0}) and essence({1})?" },
								{ $"{TableTextID.Item}", "Item" },
								{ $"{TableTextID.Enchantments}", "Enchantments" },
								{ $"{TableTextID.weapon0}", "Place a weapon, piece of armor or accessory here." },
								{ $"{TableTextID.general1}", "Upgrading Enchanting Table Tier unlocks more Enchantment slots." },
								{ $"{TableTextID.general2}", "Using weapon Enchantments on armor or accessories" },
								{ $"{TableTextID.general3}", "provides diminished bonuses and vice versa." },
								{ $"{TableTextID.enchantment0}", "Place Enchantments here." },
								{ $"{TableTextID.enchantment4}", "Requires {0} or Better to use this slot." },
								{ $"{TableTextID.utility0}", "Only utility Enchantments can go here." },
								{ $"{TableTextID.essence0}", "Place {0} here." },
							}
						) }
					};

					IEnumerable<Type> types = null;
					try {
						types = Assembly.GetExecutingAssembly().GetTypes();
					}
					catch (ReflectionTypeLoadException e) {
						types = e.Types.Where(t => t != null);
					}

					Type enchantmentEffectType = typeof(EnchantmentEffect);
					IEnumerable<Type> effectTypes = types.Where(t => !t.IsAbstract && t.IsAssignableTo(enchantmentEffectType) && t != enchantmentEffectType);

					string tooltipKey = L_ID1.Tooltip.ToString();
					string displayNameKey = L_ID2.EffectDisplayName.ToString();
					SortedDictionary<string, string> dict = allData[tooltipKey].Children[displayNameKey].Dict;
					foreach (Type effectType in effectTypes) {
						if (!dict.ContainsKey(effectType.Name) && !dict.ContainsKey(effectType.Name + "1"))
							dict.Add(effectType.Name, effectType.Name.AddSpaces());
					}

					foreach (string enchantmentTypeName in ModContent.GetContent<Enchantment>().Where(e => e.EnchantmentTier == 0).Select(e => e.EnchantmentTypeName)) {
						if (!dict.ContainsKey(enchantmentTypeName) && !dict.ContainsKey(enchantmentTypeName + "1"))
							dict.Add(enchantmentTypeName, enchantmentTypeName.AddSpaces());
					}

					string itemTypeKey = L_ID2.ItemType.ToString();
					foreach (string eItemType in Enum.GetNames(typeof(EItemType))) {
						allData[tooltipKey].Children[itemTypeKey].Values.Add(eItemType);
					}

					string tableTextKey = L_ID1.TableText.ToString();
					foreach (string tableText in Enum.GetNames(typeof(TableTextID))) {
						if (!allData[tableTextKey].Dict.ContainsKey(tableText))
							allData[tableTextKey].Values.Add(tableText);
					}

					/*
					string enchantmentEffectKey = L_ID2.EnchantmentEffects.ToString();
					foreach (Type type in effectTypes) {
						Type baseType = type.BaseType;
						string baseName = baseType.Name;
						bool baseTypeNeeded = baseType != enchantmentEffectType && baseType.GetProperty("LocalizationTooltips").DeclaringType == baseType;
						bool needed = type.GetProperty("LocalizationTooltips").DeclaringType == type;
						SortedDictionary<string, SData> children = allData[tooltipKey].Children[enchantmentEffectKey].Children;

						if (baseName != enchantmentEffectType.Name && (baseTypeNeeded || needed) && !children.ContainsKey(baseName))
							children.Add(baseName, new(children: new(), dict: new()));

						if (baseTypeNeeded) {
							if (children[baseName].Dict.Count == 0) {
								EnchantmentEffect effect = (EnchantmentEffect)Activator.CreateInstance(type);
								Dictionary<string, string> myLocalizationTooltip = effect.LocalizationTooltips;

								foreach (KeyValuePair<string, string> pair in myLocalizationTooltip) {
									children[baseName].Dict.Add(pair.Key, pair.Value);
								}
							}
						}
						
						if (needed) {
							string name = type.Name;
							
							if (!children[baseName].Children.ContainsKey(name)) {
								EnchantmentEffect effect = (EnchantmentEffect)Activator.CreateInstance(type);
								Dictionary<string, string> myLocalizationTooltip = effect.LocalizationTooltips;

								children[baseName].Children.Add(name, new SData(dict: new()));

								foreach (KeyValuePair<string, string> pair in myLocalizationTooltip) {
									children[baseName].Children[name].Dict.Add(pair.Key, pair.Value);
								}
							}
						}
					}
					*/
				}

				return allData;
			} 
		}

		public static List<string> changedData;
		public static List<string> ChangedData {
			get {
				if (changedData == null)
					changedData = new();

				return changedData;
			}
		}
	}
	public class SData {
		public List<string> Values;
		public SortedDictionary<string, string> Dict;
		public SortedDictionary<string, SData> Children;
		public SData(List<string> values = null, SortedDictionary<string, string> dict = null, SortedDictionary<string, SData> children = null) {
			Values = values;
			Dict = dict;
			Children = children;
		}
	}

	public static class LocalizationDataStaticMethods
	{
		public static void AddLocalizationTooltip(this ModItem modItem, string tooltip) {
			if ((LogModSystem.printLocalization || LogModSystem.printLocalizationKeysAndValues) && !LocalizationData.AllData[L_ID1.ItemTooltip.ToString()].Dict.ContainsKey(modItem.Name)) {
				LocalizationData.AllData[L_ID1.ItemTooltip.ToString()].Dict.Add(modItem.Name, tooltip);
			}
		}
	}
}
