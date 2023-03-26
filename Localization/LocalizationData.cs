using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Effects.CustomEffects;
using WeaponEnchantments.Items;
using WeaponEnchantments.Items.Enchantments;
using WeaponEnchantments.Items.Utility;
using WeaponEnchantments.ModIntegration;
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

		public static Dictionary<CultureName, string> LocalizationComments = new() {
			{ CultureName.German, "Contributors: @Shiro ᵘʷᵘ#6942, @Fischstäbchen#2603  (All others Google Translated.  Needs review)" },
			{ CultureName.English, "" },
			{ CultureName.Spanish , "Contributors: @DaviReM#8740, @JoeDolca, @Haturok#8191, @Kokopai#2506  (All others Google Translated.  Needs review)" },
			{ CultureName.French , "Contributors: @Soluna#1422, @Olixx12#5354  (All others Google Translated.  Needs review)" },
			{ CultureName.Italian , "Contributors: @Tefra_K" },
			{ CultureName.Polish , "(Google Translated.  No contribuions yet)" },
			{ CultureName.Portuguese , "Contributors: @Ninguém#8017, @pedro_123444#8294" },
			{ CultureName.Russian , "Contributed by @4sent4" },
			{ CultureName.Chinese , "1090549930 Kiritan - Github, @2578359679#1491, and @An unilolusiality" }
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
									{ typeof(ItemCooldown).Name, "(Item CD equal to {0} use speed)" },
									{ typeof(AmmoCost).Name, "{0} (Also Saves Bait When Fishing)" },
									{ typeof(AttackSpeed).Name, "{0} (Affects minion fire rate if they shoot projectiles.  Affects how fast fish will bite the fishing line.)" },
									{ typeof(BonusCoins).Name, "{0} (Hitting an enemy will increase the number of coins it will drop on death\nbased on damage dealt, enemy max health, enemy base value, and luck.)" },
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
									{ typeof(PrideOfTheWeak).Name, "{0} Increases damage dealt by weak weapons.  (100% effective at 0 infusion power.  0% effective at 500 infusion power.)" },
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
								{ CalamityIntegration.calamityName, "(Calamity Mod Enchantment)" },
								{ DBZMODPORTIntegration.DBTName, "(Dragon Ball Terraria Enchantment)"}
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
							}) },
							{ L_ID2.ArmorSlotNames.ToString(), new(values: new() {
								//Filled Automatically
							}) },
							{ L_ID2.DamageClassNames.ToString(), new(values: new() {
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
								{ DialogueID.HateCrowded.ToString(), "I can't deal with this here traffic! Get me out or kick them out, make it quick!" },
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
						}) },
						{ L_ID1.Config.ToString(), new(children: new() {
							{ nameof(ServerConfig), new(dict: new() {
								{ L_ID3.Label.ToString(), "Server Config" }
							}) },
							{ nameof(ServerConfig.presetData), new(dict: new() {
								{ L_ID3.Label.ToString(), "Presets and Multipliers" }
							}) },
							{ nameof(ServerConfig.individualStrengthsEnabled), new(dict: new() {
								{ L_ID3.Label.ToString(), "Individual Strengths Enabled" },
								{ L_ID3.Tooltip.ToString(), "Enabling this will cause the Indvidual strength values selected below to overite all other settings." }
							}) },
							{ nameof(ServerConfig.individualStrengths), new(dict: new() {
								{ L_ID3.Label.ToString(), "Individual Strengths" },
								{ L_ID3.Tooltip.ToString(), "Modify individual enchantment strengths by value\n(NOT PERCENTAGE!)\n(Overrides all other options)" }
							}) },
							{ nameof(ServerConfig.AlwaysOverrideDamageType), new(dict: new() {
								{ L_ID3.Label.ToString(), "Damage type converting enchantments always override." },
								{ L_ID3.Tooltip.ToString(), "Some mods like Stars Above change weapon damage types.  If this option is enabled, Enchantments that change the damage type will always change the weapon's damage type.\n" +
													"If not selected, the damage type will only be changed if the weapon is currently it's original damage type." }
							}) },
							{ nameof(ServerConfig.AffectOnVanillaLifeStealLimmit), new(dict: new() {
								{ L_ID3.Label.ToString(), "Life Steal Enchantment limiting (Affect on Vanilla Life Steal Limit) (%)" },
								{ L_ID3.Tooltip.ToString(), "Use a value above 100% to limmt lifesteal more, less than 100% to limit less.  0 to have not limit.\n" +
													"Vanilla Terraria uses a lifesteal limiting system: In the below example, the values used are in normal mode(Expert/Master mode values in parenthesis)\n" +
													"It has a pool of 80(70) that is saved for you to gain lifestea from.  Gaining life through lifesteal reduces this pool.\n" +
													"The pool is restored by 36(30) points per second.  If the pool value is negative, you cannot gain life from lifesteal.\n" +
													"This config value changes how much the life you heal from lifesteal enchantments affects this limit.\n" +
													"Example: 200%  You gain 200 life from lifesteal.  200 * 200% = 400.  80(70) pool - 400 healed = -320(-330) pool.\n" +
													"It will take 320/36(330/30) seconds -> 8.9(11) seconds for the pool to be positive again so you can gain life from lifesteal again.\n" +
													"Note: the mechanic does not have a cap on how much you can gain at once.  It will just take longer to recover the more you gain." }
							}) },
							{ nameof(ServerConfig.AttackSpeedEnchantmentAutoReuseSetpoint), new(dict: new() {
								{ L_ID3.Label.ToString(), "Speed Enchantment Auto Reuse Enabled (%)" },
								{ L_ID3.Tooltip.ToString(), "The strength that a Speed Enchantment will start giving the Auto Reuse stat.\n" +
													"Set to 0 for all Speed enchantments to give auto reuse.  Set to 10000 to to prevent any gaining auto reuse (unless you strength multiplier is huge)" }
							}) },
							{ nameof(ServerConfig.AutoReuseDisabledOnMagicMissile), new(dict: new() {
								{ L_ID3.Label.ToString(), "Auto Reuse Disabled on Magic Missile type weapons" },
								{ L_ID3.Tooltip.ToString(), "Auto Reuse on weapons like Magic Missile allow you to continuously shoot the projectiles to stack up damage infinitely." }
							}) },
							{ nameof(ServerConfig.BuffDuration), new(dict: new() {
								{ L_ID3.Label.ToString(), "Buff cooldown duration (seconds)" },
								{ L_ID3.Tooltip.ToString(), "Affects buff cooldown and duration." }
							}) },
							{ nameof(ServerConfig.AmaterasuSelfGrowthPerTick), new(dict: new() {
								{ L_ID3.Label.ToString(), "Amaterasu Self Growth Per Tick" },
								{ L_ID3.Tooltip.ToString(), "Affects how quickly Amaterasu damage will go up naturally (Not when being hit again with a World Ablaze weapon.)" }
							}) },
							{ nameof(ServerConfig.ReduceRecipesToMinimum), new(dict: new() {
								{ L_ID3.Label.ToString(), "Reduce recipes to minimum." },
								{ L_ID3.Tooltip.ToString(), "Removes all recipes that jump between tiers to reduce clutter when viewing recipes.\n" +
													"Also makes all essence recipes 4 to 1 instead of scaling with enchanting table tier." }
							}) },
							{ nameof(ServerConfig.ConfigCapacityCostMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Enchantment Capacity Cost Multiplier(%)" },
								{ L_ID3.Tooltip.ToString(), "Affects how much the enchantments cost to apply to an item.  Base values are 1/2/3/4/5 for utility, 2/4/6/8/10 for normal and 3/6/9/12/15 for unique." }
							}) },
							{ nameof(ServerConfig.RemoveEnchantmentRestrictions), new(dict: new() {
								{ L_ID3.Label.ToString(), "Remove enchantment restrictions (Use at your own risk!)" },
								{ L_ID3.Tooltip.ToString(), "Removes things like Unique, Max 1 and weapon or item type specific enchantments." }
							}) },
							{ nameof(ServerConfig.BossEssenceMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Boss Essence Multiplier(%)" },
								{ L_ID3.Tooltip.ToString(), "Modify the ammount of essence recieved from bosses." }
							}) },
							{ nameof(ServerConfig.EssenceMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Non-Boss Essence Multiplier(%)" },
								{ L_ID3.Tooltip.ToString(), "Modify the ammount of essence recieved from non-boss enemies." }
							}) },
							{ nameof(ServerConfig.BossExperienceMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Boss Experience Multiplier(%)" },
								{ L_ID3.Tooltip.ToString(), "Modify the ammount of experience recieved from bosses." }
							}) },
							{ nameof(ServerConfig.ExperienceMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Non-Boss Experience Multiplier(%)" },
								{ L_ID3.Tooltip.ToString(), "Modify the ammount of experience recieved from non-boss enemies." }
							}) },
							{ nameof(ServerConfig.GatheringExperienceMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Gathering Experience Multiplier(%)" },
								{ L_ID3.Tooltip.ToString(), "Modify the ammount of experience recieved from Mining/chopping/fishing" }
							}) },
							{ nameof(ServerConfig.EssenceGrabRange), new(dict: new() {
								{ L_ID3.Label.ToString(), "Essence Grab Range Multiplier" },
								{ L_ID3.Tooltip.ToString(), "Affects how far the essence can be away from the player when it starts moving towards the player." }
							}) },
							{ nameof(ServerConfig.BossEnchantmentDropChance), new(dict: new() {
								{ L_ID3.Label.ToString(), "Boss Enchantment Drop Rate(%)" },
								{ L_ID3.Tooltip.ToString(), "Adjust the drop rate of enchantments from bosses.\n(Default is 50%)" }
							}) },
							{ nameof(ServerConfig.EnchantmentDropChance), new(dict: new() {
								{ L_ID3.Label.ToString(), "Non-Boss Enchantment Drop Rate(%)" },
								{ L_ID3.Tooltip.ToString(), "Adjust the drop rate of enchantments from non -boss enemies.\n(Default is 100%)" }
							}) },
							{ nameof(ServerConfig.ChestSpawnChance), new(dict: new() {
								{ L_ID3.Label.ToString(), "Chest Enchantment Spawn Chance(%)" },
								{ L_ID3.Tooltip.ToString(), "Adjust the chance of finding enchantments in chests.  Can be over 100%.  Does not affect Biome chests.(They are always 100%)" }
							}) },
							{ nameof(ServerConfig.CrateDropChance), new(dict: new() {
								{ L_ID3.Label.ToString(), "Crate Enchantment Drop Chance Multiplier(%)" },
								{ L_ID3.Tooltip.ToString(), "Adjust the chance of finding enchantments in fishing crates." }
							}) },
							{ nameof(ServerConfig.PreventPowerBoosterFromPreHardMode), new(dict: new() {
								{ L_ID3.Label.ToString(), "Prevent pre-hard mode bosses from dropping power boosters." },
								{ L_ID3.Tooltip.ToString(), "Does not include wall of flesh." }
							}) },
							{ nameof(ServerConfig.AllowHighTierOres), new(dict: new() {
								{ L_ID3.Label.ToString(), "Recieve ores up to Chlorophyte from Offering items." },
								{ L_ID3.Tooltip.ToString(), "Disabling this option only allows you to recieve Iron, Silver, Gold (Or their equivelents based on world gen.).\n" +
													"(Only Works in hard mode.  Chlorophyte only after killing a mechanical boss.)" }
							}) },
							{ nameof(ServerConfig.EnchantmentSlotsOnWeapons), new(dict: new() {
								{ L_ID3.Label.ToString(), "Enchantment Slots On Weapons" },
								{ L_ID3.Tooltip.ToString(), "1st slot is a normal slot.\n" +
													"2nd slot is the utility slot.\n" +
													"3rd-5th are normal slots." }
							}) },
							{ nameof(ServerConfig.EnchantmentSlotsOnArmor), new(dict: new() {
								{ L_ID3.Label.ToString(), "Enchantment Slots On Armor" },
								{ L_ID3.Tooltip.ToString(), "1st slot is a normal slot.\n" +
													"2nd slot is the utility slot.\n" +
													"3rd-5th are normal slots." }
							}) },
							{ nameof(ServerConfig.EnchantmentSlotsOnAccessories), new(dict: new() {
								{ L_ID3.Label.ToString(), "Enchantment Slots On Accissories" },
								{ L_ID3.Tooltip.ToString(), "1st slot is a normal slot.\n" +
													"2nd slot is the utility slot.\n" +
													"3rd-5th are normal slots." }
							}) },
							{ nameof(ServerConfig.EnchantmentSlotsOnFishingPoles), new(dict: new() {
								{ L_ID3.Label.ToString(), "Enchantment Slots On Fishing Poles" },
								{ L_ID3.Tooltip.ToString(), "1st slot is a normal slot.\n" +
													"2nd slot is the utility slot.\n" +
													"3rd-5th are normal slots." }
							}) },
							{ nameof(ServerConfig.EnchantmentSlotsOnTools), new(dict: new() {
								{ L_ID3.Label.ToString(), "Enchantment Slots On Tools" },
								{ L_ID3.Tooltip.ToString(), "1st slot is a normal slot.\n" +
													"2nd slot is the utility slot.\n" +
													"3rd-5th are normal slots.\n" +
													"The Clentaminator is the only tool so far." }
							}) },
							{ nameof(ServerConfig.ReduceOfferEfficiencyByTableTier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Reduce Offer Efficiency By Table Tier" },
								{ L_ID3.Tooltip.ToString(), "When offering items, you recieve essence equivelent to the experience on the item.\n" +
													"Enabling this will cause the wood table to be 60% efficient.\n" +
													"Each table gains 10% efficiency.  100% with Ultimate table." }
							}) },
							{ nameof(ServerConfig.ReduceOfferEfficiencyByBaseInfusionPower), new(dict: new() {
								{ L_ID3.Label.ToString(), "Reduce Offer Efficiency By Base Infusion Power" },
								{ L_ID3.Tooltip.ToString(), "When offering items, you recieve essence equivelent to the experience on the item.\n" +
													"Enabling this will cause weapons to be 100% efficient at Infusion power of 0 to 80% efficient at infusion power of 1100 (and above)." }
							}) },
							{ nameof(ServerConfig.ArmorPenetration), new(dict: new() {
								{ L_ID3.Label.ToString(), "Convert excess armor penetration to bonus damage" },
								{ L_ID3.Tooltip.ToString(), "Example: Enemy has 4 defense, Your weapon has 10 armor penetration.\n" +
													"10 - 4 = 6 excess armor penetration (not doing anything)\nGain 3 bonus damage (6/2 = 3)" }
							}) },
							{ nameof(ServerConfig.DisableMinionCrits), new(dict: new() {
								{ L_ID3.Label.ToString(), "Disable Minion Critical hits" },
								{ L_ID3.Tooltip.ToString(), "In vanilla, minions arent affected by weapon critical chance.\n" +
													"Weapon Enchantments gives minions a critical hit chance based on weapon crit chance.\n" +
													"This option disables the crits(vanilla mechanics)" }
							}) },
							{ nameof(ServerConfig.CritPerLevelDisabled), new(dict: new() {
								{ L_ID3.Label.ToString(), "Disable Weapon critical strike chance per level" },
								{ L_ID3.Tooltip.ToString(), "Weapons gain critical strike chance equal to thier level * Global Enchantment Strength Multiplier." }
							}) },
							{ nameof(ServerConfig.DamagePerLevelInstead), new(dict: new() {
								{ L_ID3.Label.ToString(), "Damage instead of critical chance per level" },
								{ L_ID3.Tooltip.ToString(), "Weapons gain damage per level instead of critical strike chance equal to their level * Global Enchantment Strength Multiplier" }
							}) },
							{ nameof(ServerConfig.DamageReductionPerLevelDisabled), new(dict: new() {
								{ L_ID3.Label.ToString(), "Disable armor and accessory damage reduction per level" },
								{ L_ID3.Tooltip.ToString(), "Armor and accessories gain damage reduction equal to thier level * the appropriate setpoint below for the world difficulty." }
							}) },
							{ nameof(ServerConfig.CalculateDamageReductionBeforeDefense), new(dict: new() {
								{ L_ID3.Label.ToString(), "Calculate Damage Reduction before player defense" },
								{ L_ID3.Tooltip.ToString(), "By default, damage reduction is applied after player defense.  Select this to apply before.\nBefore will cause you to take much less damage." }
							}) },
							{ nameof(ServerConfig.ArmorDamageReductions), new(dict: new() {
								{ L_ID3.Label.ToString(), "Armor and accessory Damage Reductions" }
							}) },
							{ nameof(ServerConfig.AllowCriticalChancePast100), new(dict: new() {
								{ L_ID3.Label.ToString(), "Critical hit chance effective over 100% chance" },
								{ L_ID3.Tooltip.ToString(), "Vanilla terraria caps critical hit chance at 100%.  By default, Weapon Enchantments calculates extra crits after 100%.\n" +
													"120% critical chance is 100% to double the damage then 20% chance to crit to increase the damge.  See the next config option for more info." }
							}) },
							{ nameof(ServerConfig.MultiplicativeCriticalHits), new(dict: new() {
								{ L_ID3.Label.ToString(), "Multiplicative critical hits past the first." },
								{ L_ID3.Tooltip.ToString(), "Weapon Enchantments makes use of critical strike chance past 100% to allow you to crit again.\n" +
													"By default, this is an additive bonus: 1st crit 200% damage, 2nd 300% damage, 3rd 400% damage.....\n" +
													"Enabling this makes them multiplicative instead: 1st crit 200% damage, 2nd crit 400% damage, 3rd crit 400% damage... " }
							}) },
							{ nameof(ServerConfig.InfusionDamageMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Infusion Damage Multiplier (Divides by 1000, 1 -> 0.001)" },
								{ L_ID3.Tooltip.ToString(), "Changes the damage multiplier from infusion.  DamageMultiplier = InfusionDamageMultiplier^((InfusionPower - BaseInfusionPower) / 100)\n" +
													"Example: Iron Broadsword, Damage = 10, BaseInfusionPower = 31  infused with a Meowmere, Infusion Power 1100.\n" +
													"Iron Broadsword damage = 10 * 1.3^((1100 - 31) / 100) = 10 * 1.3^10.69 = 10 * 16.52 = 165 damage.\n" +
													"Setting this multiplier to 1000 will prevent you from infusing weapons as well as provide no damage bonus to already infused weapons." }
							}) },
							{ nameof(ServerConfig.DisableArmorInfusion), new(dict: new() {
								{ L_ID3.Tooltip.ToString(), "This will prevent you from infusing armor items and will ignore infused set bonues." }
							}) },
							{ nameof(ServerConfig.MinionLifeStealMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Minion Life Steal Multiplier (%)" },
								{ L_ID3.Tooltip.ToString(), "Allows you to reduce the ammount of healing recieved by minions with the Lifesteal Enchantment." }
							}) },
							{ nameof(ServerConfig.DCUStart), new(dict: new() {
								{ L_ID3.Label.ToString(), "Start with a Drill Containment Unit" },
								{ L_ID3.Tooltip.ToString(), "All players will get a Drill Containment Unit when they first spawn.\nThis is just for fun when you feel like a faster playthrough." }
							}) },
							{ nameof(ServerConfig.DisableResearch), new(dict: new() {
								{ L_ID3.Label.ToString(), "Disable Ability to research Weapon Enchantment items" }
							}) },
							{ nameof(ClientConfig), new(dict: new() {
								{ L_ID3.Label.ToString(), "ClientConfig" }
							}) },
							{ nameof(ClientConfig.teleportEssence), new(dict: new() {
								{ L_ID3.Label.ToString(), "Automatically send essence to UI" },
								{ L_ID3.Tooltip.ToString(), "Automatically send essence from your inventory to the UI essence slots.\n(Disables while the UI is open.)" }
							}) },
							{ nameof(ClientConfig.OfferAll), new(dict: new() {
								{ L_ID3.Label.ToString(), "Offer all of the same item." },
								{ L_ID3.Tooltip.ToString(), "Search your inventory for all items of the same type that was offered and offer them too if they have 0 experience and no power booster installed." }
							}) },
							{ nameof(ClientConfig.AllowShiftClickMoveFavoritedItems), new(dict: new() {
								{ L_ID3.Label.ToString(), "Allow shift click to move favorited items into the enchanting table." }
							}) },
							{ nameof(ClientConfig.AlwaysDisplayInfusionPower), new(dict: new() {
								{ L_ID3.Label.ToString(), "Always display Infusion Power" },
								{ L_ID3.Tooltip.ToString(), "Enable to display item's Infusion Power always instead of just when the enchanting table is open." }
							}) },
							{ nameof(ClientConfig.PercentOfferEssence), new(dict: new() {
								{ L_ID3.Label.ToString(), "Percentage of offered Item value converted to essence." }
							}) },
							{ nameof(ClientConfig.AllowCraftingIntoLowerTier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Allow crafting enchantments into lower tier enchantments." }
							}) },
							{ nameof(ClientConfig.AllowInfusingToLowerPower), new(dict: new() {
								{ L_ID3.Label.ToString(), "Allow Infusing items to lower infusion Powers" },
								{ L_ID3.Tooltip.ToString(), "Warning: This will allow you to consume a weak weapon to downgrade a strong weapon." }
							}) },
							{ nameof(ClientConfig.UsePointsAsTooltip), new(dict: new() {
								{ L_ID3.Label.ToString(), "\"\\\"Points\\\" instead of \\\"Enchantment Capacity\\\"\"" },
								{ L_ID3.Tooltip.ToString(), "Tooltips will show Points Available instead of Enchantment Capacity Available" }
							}) },
							{ nameof(ClientConfig.UseAlternateEnchantmentEssenceTextures), new(dict: new() {
								{ L_ID3.Label.ToString(), "Use Alternate Enchantment Essence Textures" },
								{ L_ID3.Tooltip.ToString(), "The default colors are color blind friendly.  The alternate textures have minor differences, but were voted to be kept." }
							}) },
							{ nameof(ClientConfig.DisplayApproximateWeaponDamageTooltip), new(dict: new() {
								{ L_ID3.Label.ToString(), "Display approximate weapon damage in the tooltip" },
								{ L_ID3.Tooltip.ToString(), "Damage enchantments are calculated after enemy armor reduces damage instead of directly changing the item's damage.\n" +
													"This displays the damage against a 0 armor enemy." }
							}) },
							{ nameof(ClientConfig.DisableAllErrorMessagesInChat), new(dict: new() {
								{ L_ID3.Label.ToString(), "Disable All Error Messages In Chat" },
								{ L_ID3.Tooltip.ToString(), "Prevents messages showing up in your chat that ask you to: \n" +
													"\"Please report this to andro951(Weapon Enchantments) allong with a description of what you were doing at the time.\"" }
							}) },
							{ nameof(ClientConfig.OnlyShowErrorMessagesInChatOnce), new(dict: new() {
								{ L_ID3.Label.ToString(), "OnlyShowErrorMessagesInChatOnce" },
								{ L_ID3.Tooltip.ToString(), "Messages will continue to show up in your chat, but only once during a game session.\n" +
													"(The error message must be the exact same as a previous message to be prevented.)" }
							}) },
							{ nameof(ClientConfig.PrintEnchantmentTooltips), new(dict: new() {
								{ L_ID3.Label.ToString(), "Log a List of Enchantment Tooltips" },
								{ L_ID3.Tooltip.ToString(), "The list is printed to the client.log when you enter a world.\nThe client.log default location is C:\\Steam\\SteamApps\\common\\tModLoader\\tModLoader-Logs" }
							}) },
							{ nameof(ClientConfig.PrintEnchantmentDrops), new(dict: new() {
								{ L_ID3.Label.ToString(), "Log a List of Enchantment Drop sources" },
								{ L_ID3.Tooltip.ToString(), "The list is printed to the client.log when you enter a world.\nThe client.log default location is C:\\Steam\\SteamApps\\common\\tModLoader\\tModLoader-Logs" }
							}) },
							{ nameof(ClientConfig.PrintLocalizationLists), new(dict: new() {
								{ L_ID3.Label.ToString(), "Log all translation lists" },
								{ L_ID3.Tooltip.ToString(), "The lists are printed to the client.log when you enter a world.\nThe client.log default location is C:\\Steam\\SteamApps\\common\\tModLoader\\tModLoader-Logs" }
							}) },
							{ nameof(ClientConfig.PrintWikiInfo), new(dict: new() {
								{ L_ID3.Label.ToString(), "Log all wiki info" },
								{ L_ID3.Tooltip.ToString(), "The info is printed to the client.log when you enter a world.\nThe client.log default location is C:\\Steam\\SteamApps\\common\\tModLoader\\tModLoader-Logs" }
							}) },
							{ nameof(ClientConfig.PrintWeaponInfusionPowers), new(dict: new() {
								{ L_ID3.Label.ToString(), "Log all weapon infusion powers" },
								{ L_ID3.Tooltip.ToString(), "The info is printed to the client.log when you enter a world.\nThe client.log default location is C:\\Steam\\SteamApps\\common\\tModLoader\\tModLoader-Logs" }
							}) },
							{ nameof(ClientConfig.EnableSwappingWeapons), new(dict: new() {
								{ L_ID3.Label.ToString(), "Enable swapping weapons with num keys (Weapons sorted by infusion power)" },
								{ L_ID3.Tooltip.ToString(), "Use num1 and num3 to swap between all weapons.  Use num4 and num6 to swap between only modded weapons.\n" +
													"Will not replace enchanted or modified weapons." }
							}) },
							{ nameof(ClientConfig.LogDummyDPS), new(dict: new() {
								{ L_ID3.Label.ToString(), "Enable Target Dummy Dps calculation and logging" },
								{ L_ID3.Tooltip.ToString(), "Tracks damage to targets from all sources and tracks them.  Press num0 to start then again to stop.\n" +
													$"Press num8 to print all stored dps values to the client.log\\nThe client.log default location is C:\\Steam\\SteamApps\\common\\tModLoader\\tModLoader-Logs\n" +
													$"Starting a new test by pressing num0 resets the previous dps data for the held item to allow re-doing a test." }
							}) },
							{ nameof(Pair.itemDefinition), new(dict: new() {
								{ L_ID3.Label.ToString(), "Enchantment" },
								{ L_ID3.Tooltip.ToString(), "Only Select Enchantment Items.\nLikely to cause an error if selecting any other item." }
							}) },
							{ nameof(Pair.Strength), new(dict: new() {
								{ L_ID3.Label.ToString(), "Strength (1000 = 1, 10 = 1%)" },
								{ L_ID3.Tooltip.ToString(), "Take care when adjusting this value.\nStrength is the exact value used.\nExample 40% Damage enchantment is 0.4\n10 Defense is 10" }
							}) },
							{ nameof(ArmorDamageReduction.ArmorDamageReductionPerLevel), new(dict: new() {
								{ L_ID3.Label.ToString(), "Armor DR Per Level (100000 = 1%)" },
								{ L_ID3.Tooltip.ToString(), "250000 (2.5%) is the maximum which would be 100% damage reduction at level 40." }
							}) },
							{ nameof(ArmorDamageReduction.AccessoryDamageReductionPerLevel), new(dict: new() {
								{ L_ID3.Label.ToString(), "Accessory DR Per Level (100000 = 1%)" },
								{ L_ID3.Tooltip.ToString(), "250000 (2.5%) is the maximum which would be 100% damage reduction at level 40." }
							}) },
							{ nameof(PresetData.AutomaticallyMatchPreseTtoWorldDifficulty), new(dict: new() {
								{ L_ID3.Label.ToString(), "Automatically Match Preset to World Difficulty" }
							}) },
							{ nameof(PresetData.Preset), new(dict: new() {
								{ L_ID3.Tooltip.ToString(), "Journey, Normal, Expert, Master, Automatic, Custom \n(Custom can't be selected here.  It is set automatically when adjusting the Global Strength Multiplier.)" }
							}) },
							{ nameof(PresetData.GlobalEnchantmentStrengthMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Global Enchantment Strength Multiplier (%)" },
								{ L_ID3.Tooltip.ToString(), "Adjusts all enchantment strengths based on recomended enchantment changes.\n" +
													"Uses the same calculations as the presets but allows you to pick a different number.\n" +
													"preset values are; Journey: 250, Normal: 100, Expert: 50, Master: 25 (Overides Ppreset)" }
							}) },
							{ nameof(PresetData.BasicEnchantmentStrengthMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Basic" },
								{ L_ID3.Tooltip.ToString(), "Affects the strength of all Basic Enchantments.  Overides all multipliers except individual enchantment strength multipliers.  Set to -1 for this multiplier to be ignored." }
							}) },
							{ nameof(PresetData.CommonEnchantmentStrengthMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Common" },
								{ L_ID3.Tooltip.ToString(), "Affects the strength of all Common Enchantments.  Overides all multipliers except individual enchantment strength multipliers.  Set to -1 for this multiplier to be ignored." }
							}) },
							{ nameof(PresetData.RareEnchantmentStrengthMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Rare" },
								{ L_ID3.Tooltip.ToString(), "Affects the strength of all Rare Enchantments.  Overides all multipliers except individual enchantment strength multipliers.  Set to -1 for this multiplier to be ignored." }
							}) },
							{ nameof(PresetData.EpicEnchantmentStrengthMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Epic" },
								{ L_ID3.Tooltip.ToString(), "Affects the strength of all Epic Enchantments.  Overides all multipliers except individual enchantment strength multipliers.  Set to -1 for this multiplier to be ignored." }
							}) },
							{ nameof(PresetData.LegendaryEnchantmentStrengthMultiplier), new(dict: new() {
								{ L_ID3.Label.ToString(), "Legendary" },
								{ L_ID3.Tooltip.ToString(), "Affects the strength of all Legendary Enchantments.  Overides all multipliers except individual enchantment strength multipliers.  Set to -1 for this multiplier to be ignored." }
							}) }
							}, dict: new() {
								{ "ServerConfig", "Server Config" },
								{ "IndividualEnchantmentStrengths", "Individual Enchantment Strengths" },
								{ "EnchantmentSettings", "Enchantment Settings" },
								{ "EssenceandExperience", "Essence and Experience" },
								{ "EnchantmentDropRates(%)", "Enchantment Drop Rates(%)" },
								{ "OtherDropRates", "Other Drop Rates" },
								{ "EnchantingTableOptions", "Enchanting Table Options" },
								{ "GeneralGameChanges", "General Game Changes" },
								{ "RandomExtraStuff", "Random Extra Stuff" },
								{ "DisplaySettings", "Display Settings" },
								{ "ErrorMessages", "Error Messages" },
								{ "LoggingInformation", "Logging Information" },
								{ "ModTestingTools", "Mod Testing Tools" },
								{ "Presets", "Presets" },
								{ "Multipliers", "Multipliers" },
								{ "RarityEnchantmentStrengthMultipliers", "Rarity Enchantment Strength Multipliers" },
								{ "Enchantment", "Enchantment" },
								{ "NoneSelected", "None Selected" },
								{ "Normal", "Normal" },
								{ "Expert", "Expert" },
								{ "Master", "Master" },
								{ "Journey", "Journey" },
								{ "Automatic", "Automatic" },
								{ "Custom", "Custom" },
								{ "ArmorDRValues", "Armor {0}% ({1}% at 40)" },
								{ "AccessoryDRValues", "Accessory {0}% ({1}% at 40)" }/*,
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" }*/
						}) }
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
						string name = effectType.Name;
						if (!dict.ContainsKey(name) && !dict.ContainsKey(name + "1"))
							dict.Add(name, name.AddSpaces());
					}

					foreach (string enchantmentTypeName in ModContent.GetContent<Enchantment>().Where(e => e.EnchantmentTier == 0).Select(e => e.EnchantmentTypeName)) {
						if (!dict.ContainsKey(enchantmentTypeName) && !dict.ContainsKey(enchantmentTypeName + "1"))
							dict.Add(enchantmentTypeName, enchantmentTypeName.AddSpaces());
					}

					string itemTypeKey = L_ID2.ItemType.ToString();
					foreach (string eItemType in Enum.GetNames(typeof(EItemType))) {
						allData[tooltipKey].Children[itemTypeKey].Values.Add(eItemType);
					}

					string armorSlotKey = L_ID2.ArmorSlotNames.ToString();
					foreach (string armorSlotName in Enum.GetNames(typeof(ArmorSlotSpecificID))) {
						allData[tooltipKey].Children[armorSlotKey].Values.Add(armorSlotName);
					}

					string damageClassKey = L_ID2.DamageClassNames.ToString();
					List<string> ignoredNames = new() { DamageClassID.Default.ToString(), DamageClassID.MeleeNoSpeed.ToString(), DamageClassID.MagicSummonHybrid.ToString() };
					foreach(string damageClassName in Enum.GetNames(typeof(DamageClassID)).Where(n => !ignoredNames.Contains(n))) {
						allData[tooltipKey].Children[damageClassKey].Values.Add(damageClassName);
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

		private static List<string> changedData;
		public static List<string> ChangedData {
			get {
				if (changedData == null)
					changedData = new();

				return changedData;
			}

			set => changedData = value;
		}

		private static Dictionary<string, string> renamedFullKeys;
		public static Dictionary<string, string> RenamedFullKeys {
			get {
				if (renamedFullKeys == null)
					renamedFullKeys = new();

				return renamedFullKeys;
			}

			set => renamedFullKeys = value;
		}

		public static Dictionary<string, string> RenamedKeys = new() {
			//{ typeof(ItemCooldown).Name, "AllForOne" },
			{ DialogueID.HateCrowded.ToString(), "HateCrouded" }
		};

		public static Dictionary<CultureName, List<string>> SameAsEnglish = new() {
			{ CultureName.German, new() {
					"Mobility Control Enchantment Epic",
					"Penny Pinching Enchantment Basic",
					"Rogue Class Swap Enchantment Epic",
					"Amaterasu",
					"Agatha",
					"Akko",
					"Binx",
					"Blobbelda",
					"Brentilda",
					"Freyja",
					"Gruntilda",
					"Jasminka",
					"Kyubey",
					"Melusine",
					"Mingella",
					"Morgana",
					"Sabrina",
					"Sarah",
					"Ursula",
					"Winifred",
					"Infusion",
					"Ki",
					"Ichor",
					"Ki Regen",
					"Max Ki",
					"Solar Dash",
					"Sonar",
					"Ninja Tabi Dash",
					"Solar Dash",
					"Crystal Ninja Dash",
				}
			},
			{
				CultureName.Spanish,
				new() {
					"Amaterasu",
					"Akko",
					"Binx",
					"Blobbelda",
					"Brentilda",
					"Gruntilda",
					"Ki",
					"Kyubey",
					"Medusa",
					"Mingella",
					"No",
					"Sabrina",
					"Salem",
					"Winifred",
					"Sonar"
				}
			},
			{
				CultureName.French,
				new() {
					"Berserkers Rage Enchantment Epic",
					"Npc Contact Angler Enchantment Epic",
					"World Ablaze Enchantment Epic",
					"Amaterasu",
					"Akko",
					"Binx",
					"Blobbelda",
					"Brentilda",
					"Freyja",
					"Gruntilda",
					"Jasminka",
					"Kyubey",
					"Sabrina",
					"Salem",
					"Winifred",
					"adamantite",
					"chlorophyte",
					"cobalt",
					"crimtane",
					"mythril",
					"palladium",
					"Infusion",
					"Ki",
					"Ichor",
					"Ki Regen",
					"Max Ki",
					"Max Minions",
					"Sonar",
					"Ninja Tabi Dash",
				}
			},
			{
				CultureName.Italian,
				new() {
					"Amaterasu",
					"Akko",
					"Binx",
					"Blobbelda",
					"Gruntilda",
					"Jasminka",
					"Ki",
					"Ki Regen",
					"Kyubey",
					"Max Ki",
					"Medusa",
					"Mingella",
					"Morgana",
					"Sabrina",
					"Salem",
					"Winifred",
					"No",
					"xp",
					"Sonar"
				}
			},
			{
				CultureName.Polish,
				new() {
					"Amaterasu",
					"Akko",
					"Binx",
					"Blobbelda",
					"Brentilda",
					"Freyja",
					"Kyubey",
					"Mingella",
					"Morgana",
					"Sabrina",
					"Salem",
					"Winifred",
					"orichalcum",
					"xp",
					"Ki",
					"Ki Regen",
					"Sonar",
					"Ninja Tabi Dash",
				}
			},
			{
				CultureName.Portuguese,
				new() {
					"Amaterasu",
					"Akko",
					"Binx",
					"Blobbelda",
					"Brentilda",
					"Freyja",
					"Gruntilda",
					"Kyubey",
					"Mary",
					"Medusa",
					"Morgana",
					"Winifred",
					"Item",
					"Ki",
					"Max Ki",
					"Sonar"
				}
			},
			{
				CultureName.Russian,
				new() {

				}
			},
			{
				CultureName.Chinese,
				new() {
					"Ki"
				}
			},
		};
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
