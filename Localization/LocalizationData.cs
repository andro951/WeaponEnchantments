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
using WeaponEnchantments.Items.Enchantments.Utility;
using WeaponEnchantments.ModIntegration;
using WeaponEnchantments.UI;
using static Terraria.Localization.GameCulture;
using androLib.Common.Utility;
using androLib.Common.Globals;
using static androLib.Localization.AndroLocalizationData;
using androLib.Localization;
using androLib;
using Terraria;
using System.Diagnostics.Metrics;
using static WeaponEnchantments.Common.Configs.ClientConfig;
using static WeaponEnchantments.Common.Configs.ServerConfig;
using static WeaponEnchantments.Common.Configs.PresetData;
using Terraria.ID;
using WeaponEnchantments.Debuffs;

namespace WeaponEnchantments.Localization
{
	public class LocalizationData {
		public static void RegisterSDataPackage() {
			if (Main.netMode == NetmodeID.Server)
				return;

			AndroLogModSystem.RegisterModLocalizationSDataPackage(new(ModContent.GetInstance<WEMod>, () => AllData, () => ChangedData, () => RenamedKeys, () => RenamedFullKeys, () => SameAsEnglish));
		}

		private static SortedDictionary<string, SData> allData;
		public static SortedDictionary<string, SData> AllData {
			get {
				if (allData == null) {
					allData = new() {
						{ L_ID1.Items.ToString(), new(children: new() {
							//Intentionally empty.  Filled automatically
						}) },
						{ L_ID1.Buffs.ToString(), new(children: new() {
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
								{ $"{typeof(VanillaDash).Name}{(int)DashID_WE.NinjaTabiDash}", $"{DashID_WE.NinjaTabiDash}".AddSpaces() },
								{ $"{typeof(VanillaDash).Name}{(int)DashID_WE.EyeOfCthulhuShieldDash}", $"{DashID_WE.EyeOfCthulhuShieldDash}".AddSpaces() },
								{ $"{typeof(VanillaDash).Name}{(int)DashID_WE.SolarDash}", $"{DashID_WE.SolarDash}".AddSpaces() },
								{ $"{typeof(VanillaDash).Name}{(int)DashID_WE.CrystalNinjaDash}", $"{DashID_WE.CrystalNinjaDash}".AddSpaces() }
							}) },
							{ L_ID2.EnchantmentEffects.ToString(), new(
								values: new(){
									//Intentionally empty.  Filled automatically
								},
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
										"(Duration is continuously increased while the buff is active, not upon first gaining the buff.)"
									},
									{ typeof(EnemySpawnRate).Name,
										"{0}\n" +
										"(Minion Damage is reduced by your spawn rate multiplier, from enchantments, unless they are your minion attack target)\n" +
										"(minion attack target set from hitting enemies with whips or a weapon that is converted to summon damage from an enchantment)\n" +
										"(Prevents consuming boss summoning items if spawn rate multiplier, from enchantments, is > 1.6)\n" +
										"(Enemies spawned will be immune to lava/traps)"},
									{ typeof(FishingEnemySpawnChance).Name, "{0} (Reduced by 5x during the day.  Affected by Chum Caster.  Can also spawn Duke Fishron.)" },
									{ typeof(GodSlayer).Name, "{0}\n(Bonus true damage based on enemy max hp)\n(Bonus damage not affected by LifeSteal)" },
									{ typeof(LavaFishing).Name, "{0} (Allows fishing in lava and has a chance to improve catch rates in lava.  Stacks with other sources.)"},
									{ typeof(LifeSteal).Name, "{0} (remainder is saved to prevent always rounding to 0 for low damage weapons)" },
									{ typeof(MaxLifeSteal).Name, "{0} (The percentage of the vanilla lifesteal pool that can be used.)" },
									{ typeof(MinionAttackTarget).Name, "Enemies hit become the minion attack target.  Same effect as whips."},
									{ typeof(Multishot).Name, "{0} (Chance to produce an extra projectile.  Applies to each projectile created.)" },
									{ typeof(OneForAll).Name, "{0}\n(Hitting an enemy will damage all nearby enemies)\n(Only activates on the first hit from a projectile.)" },
									{ typeof(OnHitSpawnProjectile).Name, "Spawns a projectile when hitting an enemy: {0}"},
									{ typeof(PrideOfTheWeak).Name, "{0} Increases damage dealt by weak weapons.  (100% effective at 0 infusion power.  0% effective at 500 infusion power.)" },
									{ typeof(QuestFishChance).Name, "{0} (Quest fish caught will be automatically turned in and start a new quest, bypassing the 1 per day limit.)"}
								}
							) },
							{ L_ID2.EnchantmentCustomTooltips.ToString(), new(dict: new() {
								{ typeof(WorldAblazeEnchantment).Name.ToEnchantmentTypeName(),
									 "(Amaterasu debuff and below notes about it only apply at Enchantment tier 4.)\n" +
									"(None shall survive the unstoppable flames of Amaterasu)\n" +
									"(Inflict a unique fire debuff to enemies that never stops)\n" +
									"(The damage from the debuff grows over time and from dealing more damage to the target)\n" +
									"(Spreads to nearby enemies and prevents enemies from being immune to other World Ablaze debuffs.)" },
								{ typeof(TimeEnchantment).Name.ToEnchantmentTypeName(), "Time effects are randomized with 6 options, Day/Night Time Rate, Tile Update Rate, Event Update Rate" },
								{ CalamityIntegration.CALAMITY_NAME, "(Calamity Mod Enchantment)" },
								{ DBZMODPORTIntegration.DBT_NAME, "(Dragon Ball Terraria Enchantment)"},
								{ ThoriumIntegration.THORIUM_NAME, "(Throium Mod Enchantment)" }
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
								{ $"{EnchantmentGeneralTooltipsID.And}", "and" },
								{ $"{EnchantmentGeneralTooltipsID.Points}", "Points" },
								{ $"{EnchantmentGeneralTooltipsID.EnchantmentCapacity}", "Enchantment Capacity" },
								{ $"{EnchantmentGeneralTooltipsID.LevelAvailable}", "Level: {0}  {1} available: {2}" },
								{ $"{EnchantmentGeneralTooltipsID.BoosterInstalled}", "Booster Installed" },
								{ $"{EnchantmentGeneralTooltipsID.NormalBoosterAbreviation}", "N" },
								{ $"{EnchantmentGeneralTooltipsID.UltraBoosterAbreviation}", "U" },
								{ $"{EnchantmentGeneralTooltipsID.Experience}", "Experience: {0}" },
								{ $"{EnchantmentGeneralTooltipsID.ToNextLevel}", " ({0} to next level)" },
								{ $"{EnchantmentGeneralTooltipsID.MaxLevel}", "(Max Level)" },
								{ $"{EnchantmentGeneralTooltipsID.InfusedArmorID}", "Infused Armor ID: {0}   Infused Item: {1}" },
								{ $"{EnchantmentGeneralTooltipsID.SetBonusID}", "Set Bonus ID: {0}" },
								{ $"{EnchantmentGeneralTooltipsID.NewSetBonusID}", "New Set Bonus ID:" },
								{ $"{EnchantmentGeneralTooltipsID.NewInfusedItem}", "New Infused Item:" },
								{ $"{EnchantmentGeneralTooltipsID.ApproximateItemDamage}", "Item Damage ~ {0} (Against 0 armor enemy)" },
								{ $"{EnchantmentGeneralTooltipsID.OutOfAmmo}", "OUT OF AMMO" },
								{ $"{EnchantmentGeneralTooltipsID.InfusionPower}", "Infusion Power:" },
								{ $"{EnchantmentGeneralTooltipsID.InfusedItem}", "Infused Item:" },
								{ $"{EnchantmentGeneralTooltipsID.NewInfusionPower}", "New Infusion Power:" }
							}) },
							{ L_ID2.ItemType.ToString(), new(values: new() {
								//Filled Automatically
							}) },
							{ L_ID2.ArmorSlotNames.ToString(), new(values: new() {
								//Filled Automatically
							}) },
							{ L_ID2.DamageClassNames.ToString(), new(values: new() {
								//Filled Automatically
							}) },
							{ L_ID2.ItemTooltip.ToString(), new(dict: new() {
								{ $"{nameof(CursedEssence)}",
									"Energy of a curse so powerful it's taken form.  Other curses will be drawn to its power.\n" +
									"Cursed Essence will contribute to the below effects when in your inventory or Enchantment Storage.  (Cursed Essence: {0})\n" +
									"   Increases enemy spawn rate.  (Current bonus: {1}x)\n" +
									"   Increases enemy max spawns.  (Current bonus: {2}x)\n" +
									"   Increased chance of cursed enemies. (Chance a spawned enemy will be cursed: {3})\n" +
									"(Cursed Essence can be stored in a Hexproof Pouch, sold by the Witch, negating it's effects.)" }
							}) }
						}) },
						{ L_ID1.Dialogue.ToString(), new(children: new() {
							{ L_ID2.Witch.ToString(), new(dict: new() {
								{ $"{DialogueID.StandardDialogue}0", "I've learned this recipe, it's really neat, I'll keep it later for your treat!" },
								{ $"{DialogueID.StandardDialogue}1", "Why do I talk all the time, it's really hard to make these rhyme!" },
								{ $"{DialogueID.StandardDialogue}2", "I'm still here, I watch you play, but I can't think of much to say!" },
								{ $"{DialogueID.BloodMoon}", "I'm truly sorry for my prior behavior. I wasn't at my best back then, and if I came off as rude, I apologize." },
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
								{ DialogueID.FarFromHome.ToString(), "I had to venture far to find new reagents.  While my home is pleasant, I can brew a new concoction with these last ingredients." },
								{ DialogueID.DislikeCrowded.ToString(), "How am I to concentrate with all this noise!" },
								{ DialogueID.HateCrowded.ToString(), "I can't deal with this here traffic! Get me out or kick them out, make it quick!" },
								{ DialogueID.LikeBiome.ToString(), "In a secluded place like this, one could brew up double the trouble, with potions a-bubble." },
								{ DialogueID.LoveBiome.ToString(), "This place is full of critters and muck, time to make a quick buck!" },
								{ DialogueID.DislikeBiome.ToString(), "On this beach's expanse, magic stands no chance. No shadows to weave, no mysteries to cleave, a place where enchantment takes its leave." },
								{ DialogueID.HateBiome.ToString(), "Yikes! This place is so hot and bright, those above all I spite!" },
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
								{ $"{TableTextID.LootAllDescription}", "Remove all enchantments from your {0}, sending them to the \n" +
									"Enchantment Storage, and return your {0} to your inventory." },
								{ $"{TableTextID.OfferDescription}", "Offer your {0}, DESTROYING it in exchange for ores/essence.\n" +
									"Percentage of ores/essence can be adjusted in the config settings." },
								{ $"{TableTextID.StorageDescription}", "Open or close the Enchantment Storage." },
								{ $"{TableTextID.SiphonDescription}", 
									"Consume {0} experience from your {1} to remove all\n" +
									"enchantments, power boosters, infused item and\n" +
									"remaining experience without destroying your {1}.\n" +
									"Offering items returns all essence, so it is a\n" +
									"better option unless you need to keep your {1}." },
								{ $"{TableTextID.InfusionDescription}", 
									"Allows you to empower weapons or armor by sacrificing other weapons or armor.\n" +
									"Weapons: Infusing a weapon increases its damage stat by {0} for every 100 infusion power added.\n" +
									"(new damage = base damage * {1}^((new infusion power - base infusion power)/100) )\n" +
									"-Place the stronger weapon you want to sacrifice into the Item slot, then press Infusion.\n" +
									"-Next, place the weaker weapon you want to upgrade into the Item slot, then press Finalize.\n" +
									"Armor: Infusing armor transfers the set bonus from the sacrificed item, replacing the set bonus of the infused item.\n" +
									"Use case: You like the cactus armor set bonus more than your current armor.\n" +
									"Sacrifice each piece of the cactus armor onto each piece of your current armor to transfer the set bonus.\n" +
									"Each piece of armor has its own set bonus id, so you can have partial sets such as a gold helm with infused cactus helm,\n" +
									"cactus legs, cactus chest, giving the cactus armor set effect.\n" +
									"-Place the armor item you want to sacrifice into the Item slot (its set bonus will be transfered), then press Infusion.\n" +
									"-Place the armor item you want to modify into the Item slot (its set bonus will be replaced), then press Finalize.\n" +
									"(Enchantments, Experience, and Power boosters from the sacrificed item will be returned to you as if it were Offered.)" },
								{ $"{TableTextID.InfusionFinalizeDescriptionWeapon}", 
									"Finalize to sacrifice your {0} to\n" +
									"increase the damage of your {1}." },
								{ $"{TableTextID.InfusionFinalizeDescriptionArmor}", 
									"Finalize to sacrifice your {0} to replace the set bonus\n" +
									"on your {1} with the set bonus of your {0}." },
								{ $"{TableTextID.InfusionCancelDescription}", "Return your stored {0} to the Item slot." },
								{ $"{TableTextID.LevelUpDescription}", 
									"Level up your {0} from level {1} to level {2}.\n" +
									"Costs {3} experience.  Experience available: {4}" },
								{ $"{TableTextID.XPButtonDescription}", 
									"Consume one {0}\n" +
									"to add {1} experience to your {2}." },
								{ $"{TableTextID.LoadoutDescription}", 
									"Open or Close the Enchantment Loadouts menu.\n" +
									$"Enchantment loadouts don't store items.\n" +
									$"They just save the type and tier of enchantment.\n" +
									$"When creating an Enchantment Loadout, you can quickly fill the slots by\n" +
									$"shift left clicking enchantments from your storage to fill the next slot\n" +
									$"(The next slot to fill has a gold background when shift is held).\n" +
									$"You can also hold an enchantment in your mouse and click it on a slot instead.\n" +
									$"Clicking on a slot with no enchantment in your mouse will clear the slot.\n" +
									$"\n" +
									$"Add - Adds a new loadout (Max of 15).\n" +
									$"Add From Equipped Enchantments - Create a loadout by copying your equipped enchantments\n" +
									$"Loadout # - Clicking the Loadout button selects that loadout so you can see or edit it.\n" +
									$"All - All replaces all enchantments on your held item, armor and accessories with\n" +
									$"the enchantments from the loadout.  If any item isn't high enough level\n" +
									$"to support the enchantments for it's slot, the loadout will fail to load.\n" +
									$"Held Item/Armor/Accessories - These buttons to the same thing as the All button, but\n" +
									$"only load the specific enchantments for the selected category." },
								{ $"{TableTextID.LevelUpNumberDescription}", 
									"Set how many levels will be added with\n" +
									"the Level Up button to {0}." }
						}) },
						{ L_ID1.EnchantmentStorageText.ToString(), new(
							values: new() {
								//Filled Automatically
							},
							dict: new() {
								{ EnchantmentStorageTextID.NoHeldItem.ToString(), "You must be holding an enchantable item to swap enchantment loadouts." },
								{ EnchantmentStorageTextID.LoadoutSizeChanged.ToString(), "Detected less accessory slots than previously available.  Enchantments in the excess slots have been returned." },
								{ EnchantmentStorageTextID.NotHighEnoughLevel.ToString(), "Your {0} is not high enough level to apply the enchantments from this loadout." },
								{ EnchantmentStorageTextID.NoArmor.ToString(), "You must be wearing a helmet, chest or legs to swap enchantment loadouts." },
								{ EnchantmentStorageTextID.NoAccessories.ToString(), "You must be wearing at least one accessory to swap enchantment loadouts." },
								{ EnchantmentStorageTextID.NoItems.ToString(), "You must be holding an enchantable item or wearing a piece of armor or an accessory to swap enchantment loadouts." },
								{ EnchantmentStorageTextID.NotEnoughEnchantments.ToString(), "You are missing enchantments for this loadout.  {0}" }
						}) },
						{ L_ID1.GameMessages.ToString(), new(
							dict: new() {
								{ GameMessageTextID.ItemTooLowLevel.ToString(), "Your {0} level is too low to use that many enchantments." },
								{ GameMessageTextID.SlotNumDisabledByConfig.ToString(), "Slot {0} disabled by config.  Removed {1} from your {2}." },
								{ GameMessageTextID.RemovedUnloadedEnchantmentFromItem.ToString(), "Removed Unloaded Item:{0} from your {1}.  Please inform andro951(WeaponEnchantments)." },
								{ GameMessageTextID.DetectedNonEnchantmentInEnchantmentSlot.ToString(), "Detected a non-enchantment item:{0} on your {1}.  It has been returned to your inventory." },
								{ GameMessageTextID.EnchantmentNoLongerAllowed.ToString(), "{0} is no longer allowed on {1} and has been removed from your {2}." },
								{ GameMessageTextID.NoLongerUtilityEnchantment.ToString(), "{0} is no longer a utility enchantment and has been removed from your {1}." },
								{ GameMessageTextID.NoLongerAllowedOnDamageType.ToString(), "{0} is no longer allowed on {1} weapons and has removed from your {2}." },
								{ GameMessageTextID.NowLimitedToOne.ToString(), "{0} Enchantments are now limited to 1 per item.  {1} has been removed from your {2}." },
								{ GameMessageTextID.MultipleUniqueEnchantments.ToString(), "Detected multiple unique enchantments on your {0}.  {1} has been removed from your {2}." },
								{ GameMessageTextID.CongradulationsMaxLevel.ToString(), "Congratulations!  {0}'s {1} reached the maximum level, {2} ({3} xp)." },
								{ GameMessageTextID.ItemLevelUp.ToString(), "{0}'s {1} reached level {2} ({3} xp)." },
								{ GameMessageTextID.PreventedLoosingExperience.ToString(), "Prevented your {0} from loosing experience due to a calculation error." },
								{ GameMessageTextID.FishingQuestTurnedIn.ToString(), "Quest turned in.  Your next quest is {0}.  Quests finished: {1}" },
								{ GameMessageTextID.CannotGainAdditionalPower.ToString(), "Your {0}({1}) cannot gain additional power from the offered {2}({3})." },
								{ GameMessageTextID.InfusionPowerMustBeLower.ToString(), "The Infusion Power of the item being upgraded must be lower than the Infusion Power of the consumed item." },
								{ GameMessageTextID.SameSetBonusNoEffect.ToString(), "The item being upgraded has the same set bonus as the item being consumed and will have no effect." },
								{ GameMessageTextID.CantInfusionArmorDifferentTypes.ToString(), "You cannot infuse armor of different types such as a helmet and body." },
								{ GameMessageTextID.InfusionOnlyPossibleSameType.ToString(), "Infusion is only possible between items of the same type (Weapon/Armor)" },
								{ GameMessageTextID.ItemRemovedFromWeaponEnchantments.ToString(), "{0} has been removed from Weapon Enchantments." },
								{ GameMessageTextID.ItemRemovedReiceveCompensation.ToString(), "{0} has been removed from Weapon Enchantments.  You've received {1} as compensation." },
								{ GameMessageTextID.FailedReplaceWithCoins.ToString(), "Failed to replace item: {0} with coins" },
								{ GameMessageTextID.ItemRemovedRecieveCoins.ToString(), "{0} has been removed from Weapon Enchantments.  You have received Coins equal to its sell price." },
								{ GameMessageTextID.ItemRemovedRelacedWithItem.ToString(), "{0} has been removed from Weapon Enchantments.  It has been replaced with {1}" },
								{ GameMessageTextID.NewItemIsAir.ToString(), "newItem was air." },
								{ GameMessageTextID.MinSiphonXP.ToString(), "You can only Siphon an item if it has at least {0} experience." },
								{ GameMessageTextID.InfusionConsumeItemWasNull.ToString(), "wePlayer.infusionConsumeItem was null, tableItem: {0}{1}, infusionConsumeItem: {2}{3}" },
								{ GameMessageTextID.MurasamaNoInfusion.ToString(), "Murasama cannot be consumed for infusion." },
								{ GameMessageTextID.FavoritedItemsCantBeConsumedForInfusion.ToString(), "Favorited items cannot be consumed for infusion." },
								{ GameMessageTextID.ResistsYourAttemptToEmpower.ToString(), "The {0} resisted your attempt to empower it." },
								{ GameMessageTextID.TryInfuseFailed.ToString(), "TryInfuseItem failed, tableItem: {0}{1}, infusionConsumeItem: {2}{3}" },
								{ GameMessageTextID.NotEnchantableAndNotAirInfusionItem.ToString(), "tableItem: {0}{1} is not enchantable, and infusionConsumeItem: {2}{3} is not air" },
								{ GameMessageTextID.AlreadyMaxLevel.ToString(), "Your {0} is already max level." },
								{ GameMessageTextID.NotEnoughEssence.ToString(), "Not Enough Essence. You need {0} experience for level {1} you only have {2} available." },
								{ GameMessageTextID.NonEnchantableItemInTable.ToString(),   "Non-Enchantable item detected in table: {0}.\n" +
																	$"WARNING, DO NOT PRESS CONFIRM.\n" +
																	$"Please report this issue to andro951(Weapon Enchantments)" },
								{ GameMessageTextID.DailyFishingQuestReset.ToString(), "The daily fishing quest has reset.  Your next quest is {0}." },
								{ GameMessageTextID.PreventedIssueLooseExperienceTwo.ToString(), "Prevented an issue that would cause you to loose experience. (xpInt < 0) item: {0}, target: {1}, hit: {2}, melee: {3}, Main.GameMode: {4}, xpDamage: {5}, xpInt: {6}, lowDamagePerHitXPBoost: {7}, " },
								{ GameMessageTextID.FailedToCloneItem.ToString(), "In EnchantedItem, Failed to Clone(item: {0}, itemClone: {1}), cloneReforgedItem: {2}, resetGlobals: {3}." },
								{ GameMessageTextID.PreventedIssueLooseExperience.ToString(), "Prevented an issue that would cause your xp do be reduced.  (xpInt < 0) item: {0}, target: {1}, hit: {2}, melee: {3}, Main.GameMode: {4}, target.defense: {5}, xpDamage: {6}, lowDamagePerHitXPBoost: {7}" },
								{ GameMessageTextID.FailedToLocateAngler.ToString(), "Failed to locate the Angler.  You will still receive rewards" },
								{ GameMessageTextID.FailedInfuseItem.ToString(), "Failed to infuse item: {0} with consumedItem: {1}" },
								{ GameMessageTextID.LogInfusionPowerLabels.ToString(), "Mod, Weapon, Infusion Power, Value Rarity, Rarity, Original Rarity, Value, Item ID, Damage, Use Time, DPS" },
								{ GameMessageTextID.LogInfusionPowerOtherLabels.ToString(), "Rarity, Average, Min, Max" },
								{ GameMessageTextID.CouldntFindItemsInWeaponsList.ToString(), "Couldn't find Items in WeaponsList or WeaponCraftingIngredients with the names:" },
								{ GameMessageTextID.CouldntFindNPCsInIngredientsList.ToString(), "Couldn't find Npcs in NPCsThatDropWeaponsOrIngredients with the names:" },
								{ GameMessageTextID.FailedFindBossBag.ToString(), "Failed to find boss bag for boss:" },
								{ GameMessageTextID.FailedFindItemDropsForGroup.ToString(), "Failed to find item drops for loot items for group" },
								{ GameMessageTextID.NPCInProgressionGroupNoUniqueEnchantments.ToString(), "Detected an npc in a Progression group that has no unique weapons or ingredients." },
								{ GameMessageTextID.NPCInProgressionGroupNotInDropList.ToString(), "Detected an npc in a Progression group that is not in NPCsThatDropWeaponsOrIngredients." },
								{ GameMessageTextID.ItemsFromNPCsNotIncluded.ToString(), "Items from WeaponsFromNPCs not included in ItemInfusionPowers" },
								{ GameMessageTextID.ItemsFromNPCIngredientsNotIncluded.ToString(), "Items from IngredientsFromNPCs not included in ItemInfusionPowers" },
								{ GameMessageTextID.ItemsFromLootItemsNotIncluded.ToString(), "Items from WeaponsFromLootItems not included in ItemInfusionPowers" },
								{ GameMessageTextID.IngredientsFromLootItemsNotincluded.ToString(), "Ingredients from WeaponsFromLootItems not included in ItemInfusionPowers" },
								{ GameMessageTextID.WeaponInfusionPowersNotSetup.ToString(), "Weapon infusion powers not setup" },
								{ GameMessageTextID.IngredientInfusionPowersNotSetup.ToString(), "Ingredient infusion powers not setup" },
								{ GameMessageTextID.ShimmerIngredientInfusionPowersNotSetup.ToString(), "Shimmer Ingredient infusion powers not setup" },
								{ GameMessageTextID.OreInfusionPowerNotSetup.ToString(), "Ore {0} infusion power not set up. Guessed infusion power:" },
								{ GameMessageTextID.FailedReplaceOldItem.ToString(), "Failed to replace old item:" },
								{ GameMessageTextID.RemovedEnchantedItemData.ToString(), "Removed EnchantedItem data from item: {0}, count: {1}, newCount: {2}" },
								{ GameMessageTextID.FailedConvertExcessExperience.ToString(), "Failed to CheckConvertExcessExperience(item: {0}, consumedItem: {1})" },
								{ GameMessageTextID.PreventedWitchShopDuplication.ToString(), "Prevented an issue that would add a duplicate item to the Witch's shop item:" },
								{ GameMessageTextID.WitchEnchantmentRerolText.ToString(), "I guess I could try to improve your enchantments, but no refunds or complaints." },
								{ GameMessageTextID.WitchChatText.ToString(), "What more do you want?  I'm busy." },
								{ GameMessageTextID.RerollEnchantment.ToString(), "Re-roll Enchantment" },
								{ GameMessageTextID.Back.ToString(), "Back" },
								{ GameMessageTextID.WitchSpawnCondition.ToString(), "Have an enchantment in your inventory or on your equipment." },
								{ GameMessageTextID.OpenEnchantingTableFirstTime.ToString(), "You feel all of your Enchantments and Essence get pulled into the Enchanting Table.\n" +
									"Weapon Enchantments has it's own storage inside the Enchanting Table.\n" +
									"Picking up Enchantments, Essence and other Weapon Enchantments items will automatically send them to the Enchanting Table storage." },
								{ GameMessageTextID.Next.ToString(), "Next" },
								{ GameMessageTextID.Curses.ToString(), "Curses" },
								{ GameMessageTextID.ApplyCurse.ToString(), "Apply Curse" },
								{ GameMessageTextID.WitchEnchantmentCurseText.ToString(), "Curses are a powerful magic, but at a cost.  I can't help you if you change your mind.  The Dryad may be able to help with that.  Are you certain you wish to continue?" },
								{ GameMessageTextID.PlaceHereReRoll.ToString(), "Place an enchantment here to re-roll" },
								{ GameMessageTextID.PlaceHereCurse.ToString(), "Place an enchantment here to apply a curse" },
								{ GameMessageTextID.ReRoll.ToString(), "Re-roll" },
								{ GameMessageTextID.Owned.ToString(), GameMessageTextID.Owned.ToString() }, 
								{ GameMessageTextID.NegativeDef.ToString(),
									"Damage taken increased by {0} and {0}%\n" +
									"(Weapon Enchantments)" }
						}) },
						{ L_ID1.Configs.ToString(), new(children: new() {
							//Server Config
							{ nameof(ServerConfig), new(children: new() {
								{ nameof(ServerConfig.presetData), new(dict: new() {
									{ L_ID3.Label.ToString(), "Presets and Multipliers" },
									{ L_ID3.Tooltip.ToString(), "Presets and Multipliers that let you affect all enchantments at once." }
								}) },
								{ nameof(ServerConfig.individualStrengthsEnabled), new(dict: new() {
									{ L_ID3.Label.ToString(), "Individual Strengths Enabled" },
									{ L_ID3.Tooltip.ToString(), "Enabling this will cause the Individual strength values selected below to override all other settings." }
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
									{ L_ID3.Tooltip.ToString(), "Use a value above 100% to limit lifesteal more, less than 100% to limit less.  0 to have not limit.\n" +
														"Vanilla Terraria uses a lifesteal limiting system: In the below example, the values used are in normal mode(Expert/Master mode values in parenthesis)\n" +
														"It has a pool of 80(70) that is saved for you to gain lifesteal from.  Gaining life through lifesteal reduces this pool.\n" +
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
									{ L_ID3.Tooltip.ToString(), "Modify the amount of essence received from bosses." }
								}) },
								{ nameof(ServerConfig.EssenceMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Non-Boss Essence Multiplier(%)" },
									{ L_ID3.Tooltip.ToString(), "Modify the amount of essence received from non-boss enemies." }
								}) },
								{ nameof(ServerConfig.BossExperienceMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Boss Experience Multiplier(%)" },
									{ L_ID3.Tooltip.ToString(), "Modify the amount of experience received from bosses." }
								}) },
								{ nameof(ServerConfig.ExperienceMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Non-Boss Experience Multiplier(%)" },
									{ L_ID3.Tooltip.ToString(), "Modify the amount of experience received from non-boss enemies." }
								}) },
								{ nameof(ServerConfig.GatheringExperienceMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Gathering Experience Multiplier(%)" },
									{ L_ID3.Tooltip.ToString(), "Modify the amount of experience received from Mining/chopping/fishing" }
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
									{ L_ID3.Label.ToString(), "Receive ores up to Chlorophyte from Offering items." },
									{ L_ID3.Tooltip.ToString(), "Disabling this option only allows you to receive Iron, Silver, Gold (Or their equivalents based on world gen.).\n" +
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
									{ L_ID3.Label.ToString(), "Enchantment Slots On Accessories" },
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
								{ nameof(ServerConfig.PercentOfferEssence), new(dict: new() {
									{ L_ID3.Label.ToString(), "Percentage of offered Item value converted to essence." },
									{ L_ID3.Tooltip.ToString(), "100% for all essence, 0% for all ores.  The sell value of essence and ores will be equivalent." }
								}) },
								{ nameof(ServerConfig.ReduceOfferEfficiencyByTableTier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Reduce Offer Efficiency By Table Tier" },
									{ L_ID3.Tooltip.ToString(), "When offering items, you receive essence equivalent to the experience on the item.\n" +
														"Enabling this will cause the wood table to be 60% efficient.\n" +
														"Each table gains 10% efficiency.  100% with Ultimate table." }
								}) },
								{ nameof(ServerConfig.ReduceOfferEfficiencyByBaseInfusionPower), new(dict: new() {
									{ L_ID3.Label.ToString(), "Reduce Offer Efficiency By Base Infusion Power" },
									{ L_ID3.Tooltip.ToString(), "When offering items, you receive essence equivalent to the experience on the item.\n" +
														"Enabling this will cause weapons to be 100% efficient at Infusion power of 0 to 80% efficient at infusion power of 1100 (and above)." }
								}) },
								{ nameof(ServerConfig.SiphonExperiencePercentCost), new(dict: new() {
									{ L_ID3.Label.ToString(), "Siphon Experience % Cost" },
									{ L_ID3.Tooltip.ToString(), "Siphoning items only returns a portion of the experience as essence.  This option is the % that is LOST, so 1000 xp with a 20% cost will only return 800.\n" +
														"If the value of the experience cost would be greater than 4 times the value of the enchanted item, that value will be the cost instead to prevent situations like loosing 20 platinum worth of experience to save a max level wood sword.\n" +
														"Offering items returns 100% of the experience instead.  Set to 0% to have no cost, returning 100% of experience.  Set to 100% to use the original Siphon system.\n" +
														"The original Siphon system only allows you to Siphon an item that is max level, and only removes excess experience past max level. with no cost." }
								}) },
								{ nameof(ServerConfig.DisableMinionCrits), new(dict: new() {
									{ L_ID3.Label.ToString(), "Disable Minion Critical hits" },
									{ L_ID3.Tooltip.ToString(), "In vanilla, minions arent affected by weapon critical chance.\n" +
														"Weapon Enchantments gives minions a critical hit chance based on weapon crit chance.\n" +
														"This option disables the crits(vanilla mechanics)" }
								}) },
								{ nameof(ServerConfig.CritPerLevelDisabled), new(dict: new() {
									{ L_ID3.Label.ToString(), "Disable Weapon critical strike chance per level" },
									{ L_ID3.Tooltip.ToString(), "Weapons gain critical strike chance equal to their level * Global Enchantment Strength Multiplier." }
								}) },
								{ nameof(ServerConfig.DamagePerLevelInstead), new(dict: new() {
									{ L_ID3.Label.ToString(), "Damage instead of critical chance per level" },
									{ L_ID3.Tooltip.ToString(), "Weapons gain damage per level instead of critical strike chance equal to their level * Global Enchantment Strength Multiplier" }
								}) },
								{ nameof(ServerConfig.DamageReductionPerLevelDisabled), new(dict: new() {
									{ L_ID3.Label.ToString(), "Disable armor and accessory damage reduction per level" },
									{ L_ID3.Tooltip.ToString(), "Armor and accessories gain damage reduction equal to their level * the appropriate setpoint below for the world difficulty." }
								}) },
								{ nameof(ServerConfig.CalculateDamageReductionBeforeDefense), new(dict: new() {
									{ L_ID3.Label.ToString(), "Calculate Damage Reduction before player defense" },
									{ L_ID3.Tooltip.ToString(), "By default, damage reduction is applied after player defense.  Select this to apply before.\nBefore will cause you to take much less damage." }
								}) },
								{ nameof(ServerConfig.ArmorDamageReductions), new(dict: new() {
									{ L_ID3.Label.ToString(), "Armor and accessory Damage Reductions" },
									{ L_ID3.Tooltip.ToString(), "Used to modify the armor reduction gained by armor and accessories as they level." }
								}) },
								{ nameof(ServerConfig.AllowCriticalChancePast100), new(dict: new() {
									{ L_ID3.Label.ToString(), "Critical hit chance effective over 100% chance" },
									{ L_ID3.Tooltip.ToString(), "Vanilla terraria caps critical hit chance at 100%.  By default, Weapon Enchantments calculates extra crits after 100%.\n" +
														"120% critical chance is 100% to double the damage then 20% chance to crit to increase the damage.  See the next config option for more info." }
								}) },
								{ nameof(ServerConfig.MultiplicativeCriticalHits), new(dict: new() {
									{ L_ID3.Label.ToString(), "Multiplicative critical hits past the first." },
									{ L_ID3.Tooltip.ToString(), "Weapon Enchantments makes use of critical strike chance past 100% to allow you to crit again.\n" +
														"By default, this is an additive bonus: 1st crit 200% damage, 2nd 300% damage, 3rd 400% damage.....\n" +
														"Enabling this makes them multiplicative instead: 1st crit 200% damage, 2nd crit 400% damage, 3rd crit 800% damage... " }
								}) },
								{ nameof(ServerConfig.InfusionDamageMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Infusion Damage Multiplier (Divides by 1000, 1 -> 0.001)" },
									{ L_ID3.Tooltip.ToString(), "Changes the damage multiplier from infusion.  DamageMultiplier = InfusionDamageMultiplier^((InfusionPower - BaseInfusionPower) / 100)\n" +
														"Example: Iron Broadsword, Damage = 10, BaseInfusionPower = 31  infused with a Meowmere, Infusion Power 1100.\n" +
														"Iron Broadsword damage = 10 * 1.3^((1100 - 31) / 100) = 10 * 1.3^10.69 = 10 * 16.52 = 165 damage.\n" +
														"Setting this multiplier to 1000 will prevent you from infusing weapons as well as provide no damage bonus to already infused weapons." }
								}) },
								{ nameof(ServerConfig.DisableArmorInfusion), new(dict: new() {
									{ L_ID3.Label.ToString(), "Disable Armor Infusion" },
									{ L_ID3.Tooltip.ToString(), "This will prevent you from infusing armor items and will ignore infused set bonuses." }
								}) },
								{ nameof(ServerConfig.PrintWikiInfo), new(dict: new() {
									{ L_ID3.Label.ToString(), "Log all wiki info" },
									{ L_ID3.Tooltip.ToString(), "The info is printed to the client.log when you enter a world.\nThe client.log default location is C:\\Steam\\SteamApps\\common\\tModLoader\\tModLoader-Logs" }
								}) },
								{ nameof(ServerConfig.MinionLifeStealMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Minion Life Steal Multiplier (%)" },
									{ L_ID3.Tooltip.ToString(), "Allows you to reduce the amount of healing received by minions with the Lifesteal Enchantment." }
								}) },
								{ nameof(ServerConfig.NegativeDefensePenaltyMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.NegativeDefensePenaltyMultiplier).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), 
										$"If this number is greater than 0, a player having less than 0 defense will cause them to take more damage.\n" +
										$"This is a penalty for allowing curses to reduce a player's defense to less than 0.\n" +
										$"Damage taken is increased by {nameof(ServerConfig.NegativeDefensePenaltyMultiplier).AddSpaces()}/1000 * -playerDefense\n" +
										$"AND multiplied by 1 + ({nameof(ServerConfig.NegativeDefensePenaltyMultiplier).AddSpaces()}/1000 * -playerDefense / 100)" +
										$"Example: -20 defense and player takes 200 damage, {nameof(ServerConfig.NegativeDefensePenaltyMultiplier).AddSpaces()} = 1000:\n" +
										$"finalDamage = (200 -(-20) * 1000/1000) * (1 + (1000/1000 * -(-20) / 100))\n" +
										$"finalDamage = (200 + 20) * (1 + 0.2) => 220 * 1.2 => 264" }
								}) },
								{ nameof(ServerConfig.DCUStart), new(dict: new() {
									{ L_ID3.Label.ToString(), "Start with a Drill Containment Unit" },
									{ L_ID3.Tooltip.ToString(), "All players will get a Drill Containment Unit when they first spawn.\nThis is just for fun when you feel like a faster playthrough." }
								}) },
								{ nameof(ServerConfig.DisableResearch), new(dict: new() {
									{ L_ID3.Label.ToString(), "Disable Ability to research Weapon Enchantment items." },
									{ L_ID3.Tooltip.ToString(), "When enabled, all essence and enchantments will not be researchable, preventing them being duplicated in Journey mode." }
								}) },
								{ nameof(ServerConfig.AllowCursedEnemies), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.AllowCursedEnemies).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Enable to allow enemies to rarely be cursed when they spawn causing them to deal no damage, but shoot debuffs at players.  Cursed Enemies also have 5x health." }
								}) },
								{ nameof(ServerConfig.CursedEnemyLifeMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.CursedEnemyLifeMultiplier).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Modifies cursed enemies max life.  Value is divided by 100.  500 => 5x multiplier." }
								}) },
								{ nameof(ServerConfig.CursedEnemyDamageMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.CursedEnemyDamageMultiplier).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Modifies cursed enemies damage dealt.  0 will cause them to not damage or knockback players.  Value is divided by 100.  100 => 1x multiplier." }
								}) },
								{ nameof(ServerConfig.CursedEssenceDropChanceMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.CursedEssenceDropChanceMultiplier).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Affects how much cursed essence is dropped by enemies." }
								}) },
								{ nameof(ServerConfig.CursedEnemyDebuffAttackRange), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.CursedEnemyDebuffAttackRange).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Cursed Enemies will shoot debuffs at a player if they are within this distance of the player.  Value is divided by 100.  40000 => 400 range.  16 range is 1 in game tile/block." }
								}) },
								{ nameof(ServerConfig.CursedEnemyDebuffDurationMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.CursedEnemyDebuffDurationMultiplier).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Affects how long the debuffs from cursed enemies last." }
								}) },
								{ nameof(ServerConfig.CursedEnemyDebuffChanceMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.CursedEnemyDebuffChanceMultiplier).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Affects how likely the debuffs are to occur when it by cursed enemy debuff projectiles.  Most are 100% by default.  Confused is 10%, Webbed and Frozen are 5% and Petrification is 2.5%." }
								}) },
								{ nameof(ServerConfig.CuredEnemyDebuffTicksPerAttack), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.CuredEnemyDebuffTicksPerAttack).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "How many ticks between Cursed Enemies shooting a debuff projectile.  20 ticks / attack => (1 sec / 60 ticks) / (20 ticks / attack) => 3 attacks / second" }
								}) },
								{ nameof(ServerConfig.EnchantmentStrengthCurseScaling), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.EnchantmentStrengthCurseScaling).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), 
										$"This setting causes a fraction of the other enchantment strength settings to apply to the cursed effects.  Value is divided by 100.  50 => 50%\n" +
										$"If this value is 0, it will have no effect.  If it is 50, it will have half of the effect of the other enchantment strength settings.\n" +
										$"For instance, on Master world difficulty, the default enchantment strength is 25% (-75% reduction).\n" +
										$"With this setting at 50, the curse effects would be reduced by half of that amount, 62.5% (-75% * 50% => -37.5% reduction).\n" +
										$"At 100, the curse effects would scale the same as normal enchantment effects." }
								}) },
								{ nameof(ServerConfig.CurseStrengthMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.CurseStrengthMultiplier).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Affects how large the curses are on cursed enchantments." }
								}) },
								{ nameof(ServerConfig.CursedBuffSpawnRateMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.CursedBuffSpawnRateMultiplier).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), 
										$"Controls the power of the Cursed Debuff spawn rate and max spawn effects.\n" +
										$"Spawn Rate Mult = (1 + {nameof(ServerConfig.CursedBuffSpawnRateMultiplier).AddSpaces()} * 0.415)^(Log2(Cursed Essence / 100 + 2)\n" +
										$"Max Spawns Mult = 1 + (Spawn Rate Mult - 1) * 0.8" }
								}) },
								{ nameof(ServerConfig.EnchantmentEffectsOnModdedAccessorySlots), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.EnchantmentEffectsOnModdedAccessorySlots).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "If enabled, accessory slots added by mods will apply enchantment effects on items in the slot." }
								}) },
								{ nameof(ServerConfig.CursedEnchantmentsAllowedOnSummons), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ServerConfig.CursedEnchantmentsAllowedOnSummons).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), 
										"Cursed Enchantment Debuffs don't get applied to the player, so it is inherently unfair to have the extra bonus without the downside.\n" +
										"Enabling this will allow you to put Cursed Enchantments on Summon weapons." }
								}) },
							},
							dict: new() {
								{ L_ID2.DisplayName.ToString(), nameof(ServerConfig).AddSpaces() },
								{ ServerConfigKey, ServerConfigKey.AddSpaces() },
								{ IndividualEnchantmentStrengthsKey, IndividualEnchantmentStrengthsKey.AddSpaces() },
								{ EnchantmentSettingsKey, EnchantmentSettingsKey.AddSpaces() },
								{ EssenceAndExperienceKey, "Essence and Experience" },
								{ EnchantmentDropRatesKey, "Enchantment Drop Rates(%)" },
								{ OtherDropRatesKey, OtherDropRatesKey.AddSpaces() },
								{ EnchantingTableOptionsKey, EnchantingTableOptionsKey.AddSpaces() },
								{ CursedEnemiesKey, CursedEnemiesKey.AddSpaces() },
								{ GeneralGameChangesKey, GeneralGameChangesKey.AddSpaces() },
								{ RandomExtraStuffKey, RandomExtraStuffKey.AddSpaces() },
								{ PresetsKey, PresetsKey.AddSpaces() },
								{ MultipliersKey, MultipliersKey.AddSpaces() },
								{ RarityEnchantmentStrengthMultipliersKey, RarityEnchantmentStrengthMultipliersKey.AddSpaces() },
							}) },
							//Client Config
							{ nameof(ClientConfig), new(children: new() {
								{ nameof(ClientConfig.teleportEssence), new(dict: new() {
									{ L_ID3.Label.ToString(), "Automatically send essence to UI" },
									{ L_ID3.Tooltip.ToString(), "Automatically send essence from your inventory to the UI essence slots.\n(Disables while the UI is open.)" }
								}) },
								{ nameof(ClientConfig.OfferAll), new(dict: new() {
									{ L_ID3.Label.ToString(), "Offer all of the same item." },
									{ L_ID3.Tooltip.ToString(), "Search your inventory for all items of the same type that was offered and offer them too if they have 0 experience and no power booster installed.\n" +
																"Also offers all items from chests touching the enchanting table." }
								}) },
								{ nameof(ClientConfig.AllowShiftClickMoveFavoritedItems), new(dict: new() {
									{ L_ID3.Label.ToString(), "Allow shift click to move favorited items into the enchanting table." },
									{ L_ID3.Tooltip.ToString(), "If not enabled, items have to be un-favorited or manually moved by moving with the mouse." }
								}) },
								{ nameof(ClientConfig.AlwaysDisplayInfusionPower), new(dict: new() {
									{ L_ID3.Label.ToString(), "Always display Infusion Power" },
									{ L_ID3.Tooltip.ToString(), "Enable to display item's Infusion Power always instead of just when the enchanting table is open." }
								}) },
								{ nameof(ClientConfig.AllowCraftingIntoLowerTier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Allow crafting enchantments into lower tier enchantments." },
									{ L_ID3.Tooltip.ToString(), "When crafting enchantments into lower tier ones, you will receive all ingredients back including containments, essence and gems." }
								}) },
								{ nameof(ClientConfig.AllowInfusingToLowerPower), new(dict: new() {
									{ L_ID3.Label.ToString(), "Allow Infusing items to lower infusion Powers" },
									{ L_ID3.Tooltip.ToString(), "Warning: This will allow you to consume a weak weapon to downgrade a strong weapon." }
								}) },
								{ nameof(ClientConfig.UsePointsAsTooltip), new(dict: new() {
									{ L_ID3.Label.ToString(), "\"Points\" instead of \"Enchantment Capacity\"" },
									{ L_ID3.Tooltip.ToString(), "Tooltips will show Points Available instead of Enchantment Capacity Available" }
								}) },
								{ nameof(ClientConfig.DisplayDamageTooltipSeperatly), new(dict: new() {
									{ L_ID3.Label.ToString(), "Display approximate damage from enchantments in a separate tooltip." },
									{ L_ID3.Tooltip.ToString(), "Damage enchantments are calculated after enemy armor reduces damage instead of directly changing the item's damage.\n" +
														"If this is off, the tooltip damage will show the damage against zero armor targets.\n" +
														"Enable to show a separate tooltip instead of updating the normal damage value.  This displays the damage against a 0 armor enemy." }
								}) },
								{ nameof(ClientConfig.AlwaysDisplayWeaponLevelUpMessages), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ClientConfig.AlwaysDisplayWeaponLevelUpMessages).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Only displays weapon level up messages when using the enchanting table if this option is off." }
								}) },
								{ nameof(ClientConfig.AlwaysDisplayArmorLevelUpMessages), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ClientConfig.AlwaysDisplayArmorLevelUpMessages).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Only displays armor level up messages when using the enchanting table if this option is off." }
								}) },
								{ nameof(ClientConfig.AlwaysDisplayAccessoryLevelUpMessages), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ClientConfig.AlwaysDisplayAccessoryLevelUpMessages).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Only displays accessory level up messages when using the enchanting table if this option is off." }
								}) },
								{ nameof(ClientConfig.AlwaysDisplayToolLevelUpMessages), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ClientConfig.AlwaysDisplayToolLevelUpMessages).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Only displays tool level up messages when using the enchanting table if this option is off." }
								}) },
								{ nameof(ClientConfig.PrintEnchantmentTooltips), new(dict: new() {
									{ L_ID3.Label.ToString(), "Log a List of Enchantment Tooltips" },
									{ L_ID3.Tooltip.ToString(), "The list is printed to the client.log when you enter a world.\nThe client.log default location is C:\\Steam\\SteamApps\\common\\tModLoader\\tModLoader-Logs" }
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
								{ nameof(ClientConfig.CursedEnemyVisualShaking), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ClientConfig.CursedEnemyVisualShaking).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), 
										"Cursed enemies shake or vibrate to help distinguish them from normal enemies.\n" +
										"This setting controls how strong the visual shaking is of cursed enemies.  Set to 0 to disable the shaking." }
								}) },
								{ nameof(ClientConfig.CursedEnemyParticles), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ClientConfig.CursedEnemyParticles).AddSpaces() },
									{ L_ID3.Tooltip.ToString(),
										"Cursed enemies produce small particles to help distinguish them from normal enemies.\n" +
										"Turn off to prevent these particles from being created." }
								}) },
								{ nameof(ClientConfig.VisualCursedDebuff), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(ClientConfig.VisualCursedDebuff).AddSpaces() },
									{ L_ID3.Tooltip.ToString(),
										"The cursed debuff doesn't actually control the bonus spawn chance, max spawns or cursed spawn chance increase.\n" +
										"It is for show only.  If you don't want the extra buff showing, you can disable it and still have the effects." }
								}) },
							},
							dict: new() {
								{ L_ID2.DisplayName.ToString(), nameof(ClientConfig).AddSpaces() },
								{ DisplaySettingsKey, DisplaySettingsKey.AddSpaces() },
								{ ErrorMessagesKey, ErrorMessagesKey.AddSpaces() },
								{ LoggingInformationKey, LoggingInformationKey.AddSpaces() },
								{ ModTestingToolsKey, ModTestingToolsKey.AddSpaces() },
							}) },
							//ArmorDamageReduction
							{ nameof(ArmorDamageReduction), new(children: new() {
								{ nameof(ArmorDamageReduction.ArmorDamageReductionPerLevel), new(dict: new() {
									{ L_ID3.Label.ToString(), "Armor DR Per Level (100000 = 1%)" },
									{ L_ID3.Tooltip.ToString(), "250000 (2.5%) is the maximum which would be 100% damage reduction at level 40." }
								}) },
								{ nameof(ArmorDamageReduction.AccessoryDamageReductionPerLevel), new(dict: new() {
									{ L_ID3.Label.ToString(), "Accessory DR Per Level (100000 = 1%)" },
									{ L_ID3.Tooltip.ToString(), "250000 (2.5%) is the maximum which would be 100% damage reduction at level 40." }
								}) },
							},
							dict: new() {
								{ L_ID3.Tooltip.ToString(), "Allows you to modify the damage reduction gained by armor and accessories as they are leveled." }
							}) },
							//Pair
							{ nameof(Pair), new(children: new() {
								{ nameof(Pair.itemDefinition), new(dict: new() {
									{ L_ID3.Label.ToString(), "Enchantment" },
									{ L_ID3.Tooltip.ToString(), "Only Select Enchantment Items.\nLikely to cause an error if selecting any other item." }
								}) },
								{ nameof(Pair.Strength), new(dict: new() {
									{ L_ID3.Label.ToString(), "Strength (1000 = 1, 10 = 1%)" },
									{ L_ID3.Tooltip.ToString(), "Take care when adjusting this value.\nStrength is the exact value used.\nExample 40% Damage enchantment is 0.4\n10 Defense is 10" }
								}) },
							},
							dict: new() {
								{ L_ID3.Tooltip.ToString(), "Allows you to assign a specific enchantments a strength." }
							}) },
							//Preset Data
							{ nameof(PresetData), new(children: new() {
								{ nameof(PresetData.AutomaticallyMatchPresetToWorldDifficulty), new(dict: new() {
									{ L_ID3.Label.ToString(), nameof(PresetData.AutomaticallyMatchPresetToWorldDifficulty).AddSpaces() },
									{ L_ID3.Tooltip.ToString(), "Must be turned off for the other options to be edited." }
								}) },
								{ nameof(PresetData.Preset), new(dict: new() {
									{ L_ID3.Label.ToString(), "Preset" },
									{ L_ID3.Tooltip.ToString(), "Journey, Normal, Expert, Master, Automatic, Custom \n(Custom can't be selected here.  It is set automatically when adjusting the Global Strength Multiplier.)" }
								}) },
								{ nameof(PresetData.GlobalEnchantmentStrengthMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Global Enchantment Strength Multiplier (%)" },
									{ L_ID3.Tooltip.ToString(), "Adjusts all enchantment strengths based on recommended enchantment changes.\n" +
														"Uses the same calculations as the presets but allows you to pick a different number.\n" +
														"preset values are; Journey: 250, Normal: 100, Expert: 50, Master: 25 (Overrides Preset)" }
								}) },
								{ nameof(PresetData.BasicEnchantmentStrengthMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Basic" },
									{ L_ID3.Tooltip.ToString(), "Affects the strength of all Basic Enchantments.  Overrides all multipliers except individual enchantment strength multipliers.  Set to -1 for this multiplier to be ignored." }
								}) },
								{ nameof(PresetData.CommonEnchantmentStrengthMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Common" },
									{ L_ID3.Tooltip.ToString(), "Affects the strength of all Common Enchantments.  Overrides all multipliers except individual enchantment strength multipliers.  Set to -1 for this multiplier to be ignored." }
								}) },
								{ nameof(PresetData.RareEnchantmentStrengthMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Rare" },
									{ L_ID3.Tooltip.ToString(), "Affects the strength of all Rare Enchantments.  Overrides all multipliers except individual enchantment strength multipliers.  Set to -1 for this multiplier to be ignored." }
								}) },
								{ nameof(PresetData.EpicEnchantmentStrengthMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Epic" },
									{ L_ID3.Tooltip.ToString(), "Affects the strength of all Epic Enchantments.  Overrides all multipliers except individual enchantment strength multipliers.  Set to -1 for this multiplier to be ignored." }
								}) },
								{ nameof(PresetData.LegendaryEnchantmentStrengthMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Legendary" },
									{ L_ID3.Tooltip.ToString(), "Affects the strength of all Legendary Enchantments.  Overrides all multipliers except individual enchantment strength multipliers.  Set to -1 for this multiplier to be ignored." }
								}) },
								{ nameof(PresetData.CursedEnchantmentStrengthMultiplier), new(dict: new() {
									{ L_ID3.Label.ToString(), "Cursed" },
									{ L_ID3.Tooltip.ToString(), "Affects the strength of all Cursed Enchantments.  Overrides all multipliers except individual enchantment strength multipliers.  Set to -1 for this multiplier to be ignored." }
								}) }
							},
							dict: new() {
								{ L_ID3.Tooltip.ToString(), "Presets allow you to easily affect all enchantments at a time." }
							}) },
						}, dict: new() {
							{ "Enchantment", "Enchantment" },
							{ "NoneSelected", "None Selected" },
							{ "Automatic", "Automatic" },
							{ "Custom", "Custom" },
							{ ArmorDamageReduction.ArmorDRValuesKey, "Armor {0}% ({1}% at 40)" },
							{ ArmorDamageReduction.AccessoryDRValuesKey, "Accessory {0}% ({1}% at 40)" }
						}) }
					};

					Mod weMod = ModContent.GetInstance<WEMod>();
					IEnumerable<ModItem> modItems = weMod.GetContent<ModItem>();
					foreach (ModItem modItem in modItems) {
						allData[L_ID1.Items.ToString()].Children.Add(modItem.Name, new(dict: new() { { L_ID2.DisplayName.ToString(), modItem.Name.AddSpaces() } }));
					}

					foreach (ModBuff modBuff in weMod.GetContent<ModBuff>()) {
						string buffName = modBuff.Name;
						allData[L_ID1.Buffs.ToString()].Children.Add(buffName, new(dict: new() { { L_ID2.DisplayName.ToString(), buffName } }));
						if (modBuff is WEBuff weBuff && weBuff.LocalizationDescription != null) {
							allData[L_ID1.Buffs.ToString()].Children[buffName].Dict.Add(L_ID2.Description.ToString(), weBuff.LocalizationDescription);
						}
					}

					IEnumerable<Type> types = null;
					try {
						types = Assembly.GetExecutingAssembly().GetTypes();
					}
					catch (ReflectionTypeLoadException e) {
						types = e.Types.Where(t => t != null);
					}

					/*
					List<string> list = types.Where(t => t.GetType() == Type.GetType("EnchantmentEffects"))
						.Where(t => !t.IsAbstract)
						.Select(t => t.Name)
						.ToList();

					SortedDictionary<string, string> dict = pair.Value.Dict;
					foreach (string s in list) {
						if (!dict.ContainsKey(s))
							dict.Add(s, s.AddSpaces());
					}
					*/

					Type enchantmentEffectType = typeof(EnchantmentEffect);
					IEnumerable<Type> effectTypes = types.Where(t => t.InheritsFrom(enchantmentEffectType));

					string tooltipKey = L_ID1.Tooltip.ToString();
					string displayNameKey = L_ID2.EffectDisplayName.ToString();
					string enchantmentEffectsKey = L_ID2.EnchantmentEffects.ToString();
					SortedDictionary<string, string> dict = allData[tooltipKey].Children[displayNameKey].Dict;
					SortedDictionary<string, string> enchantmentEffectsDict = allData[tooltipKey].Children[enchantmentEffectsKey].Dict;
					SortedDictionary<string, SData> enchantmentEffectsChildren = allData[tooltipKey].Children[enchantmentEffectsKey].Children;
					foreach (Type effectType in effectTypes) {
						string name = effectType.Name;
						if (!dict.ContainsKey(name) && !dict.ContainsKey(name + "1"))
							dict.Add(name, name.AddSpaces());

						if (!enchantmentEffectsDict.ContainsKey(name) && !enchantmentEffectsChildren.ContainsKey(name))
							enchantmentEffectsDict.Add(name, name.AddSpaces());
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
					foreach (string damageClassName in Enum.GetNames(typeof(DamageClassID)).Where(n => !ignoredNames.Contains(n))) {
						allData[tooltipKey].Children[damageClassKey].Values.Add(damageClassName);
					}

					string tableTextKey = L_ID1.TableText.ToString();
					foreach (string tableText in Enum.GetNames(typeof(TableTextID))) {
						if (!allData[tableTextKey].Dict.ContainsKey(tableText))
							allData[tableTextKey].Values.Add(tableText);
					}

					string EnchantmentStorageTextKey = L_ID1.EnchantmentStorageText.ToString();
					foreach (string enchantmentStroageText in Enum.GetNames(typeof(EnchantmentStorageTextID))) {
						if (!allData[EnchantmentStorageTextKey].Dict.ContainsKey(enchantmentStroageText))
							allData[EnchantmentStorageTextKey].Values.Add(enchantmentStroageText);
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
			//{ DialogueID.HateCrowded.ToString(), "HateCrouded" }
		};

		public static Dictionary<CultureName, List<string>> SameAsEnglish = new() {
			{ CultureName.German, new() {
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
					"DBZKi",
					"Ichor",
					"DBZKi Regen",
					"Max DBZKi",
					"Siphon",
					"Sonar",
					"Normal",
					"Basic",
					"Max MP",
					"N",
					"U"
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
					"DBZKi",
					"Kyubey",
					"Medusa",
					"Mingella",
					"No",
					"Normal",
					"Sabrina",
					"Salem",
					"Winifred",
					"Sonar",
					"N",
					"U"
				}
			},
			{
				CultureName.French,
				new() {
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
					"DBZKi",
					"Ichor",
					"Max DBZKi",
					"Sonar",
					"Expert",
					"Rare",
					"Siphon",
					"Poison",
					"U",
					"Points",
					"N"
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
					"DBZKi",
					"Kyubey",
					"Medusa",
					"Mingella",
					"Morgana",
					"Sabrina",
					"Salem",
					"Winifred",
					"No",
					"xp",
					"Sonar",
					"N",
					"U"
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
					"DBZKi",
					"DBZKi Regen",
					"Sonar",
					"N",
					"U"
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
					"Normal",
					"Winifred",
					"Item",
					"DBZKi",
					"Sonar",
					"N",
					"U"
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
					
				}
			},
		};
	}

	public static class LocalizationDataStaticMethods
	{
		public static void AddLocalizationTooltip(this ModItem modItem, string tooltip, string name = null) {
			if ((AndroLogModSystem.printLocalization || AndroLogModSystem.printLocalizationKeysAndValues) && !LocalizationData.AllData[L_ID1.Items.ToString()].Children[modItem.Name].Dict.ContainsKey(L_ID1.Tooltip.ToString())) {
				//LocalizationData.AllData[L_ID1.Items.ToString()].Children.Add(modItem.Name, new(dict: new()));
				LocalizationData.AllData[L_ID1.Items.ToString()].Children[modItem.Name].Dict.Add(L_ID1.Tooltip.ToString(), tooltip);
				//LocalizationData.AllData[L_ID1.Items.ToString()].Children[modItem.Name].Dict.Add(L_ID2.DisplayName.ToString(), name ?? modItem.Name.AddSpaces());
			}
		}
	}
}
