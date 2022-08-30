using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using WeaponEnchantments.Common.Utility;
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

		static LanguageManager languageManager;
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
			"EnchantmentEffects",
			"Debuffs"
		};

		public static SortedDictionary<string, SData> AllData => new() {
			{ L_ID1.Tooltip.ToString(), new(children: new() {
				{ L_ID2.EffectDisplayName.ToString(), new(dict: new() {
					{ "AmmoCost1", "Chance To Not Consume Ammo" },
					{ "AmmoCost2", "Increased Ammo Cost" },
					{ "DamageAfterDefenses", "Damage (Applied after defenses. Not visible in weapon tooltip)" },
					{ "DamageClassChange", "\"Convert damage type to {0}\""}
				}) },
				{ L_ID2.EnchantmentEffects.ToString(), new(children: new() { 
					{ "BoolEfect", new(dict: new() {
						{ "Enabled", "\"{0} Enabled\"" },
						{ "Enabled", "\"{0} Disabled\"" }
					}) },
					{ "BuffEffect", new(dict: new() {
						{ BuffStyle.OnTickPlayerBuff.ToString(), "\"Passively grants {0} to you\"" },
						{ BuffStyle.OnTickPlayerDebuff.ToString(), "\"Passively inflicts {0} to you\"" },
						{ BuffStyle.OnTickAreaTeamBuff.ToString(), "\"Passively grants {0} to nearby players\"" },
						{ BuffStyle.OnTickAreaTeamDebuff.ToString(), "\"Passively inflicts {0} to nearby players\"" },
						{ BuffStyle.OnTickEnemyBuff.ToString(), "\"Passively grants {0} to enemy\"" },
						{ BuffStyle.OnTickEnemyDebuff.ToString(), "\"Passively inflicts {0} to enemy\"" },
						{ BuffStyle.OnTickAreaEnemyBuff.ToString(), "\"Passively grants {0} to nearby enemies\"" },
						{ BuffStyle.OnTickAreaEnemyDebuff.ToString(), "\"Passively inflicts {0} to nearby enemies\"" },
						{ BuffStyle.OnHitPlayerBuff.ToString(), "\"Grants you {0} on hit\"" },
						{ BuffStyle.OnHitPlayerDebuff.ToString(), "\"Inflicts {0} to you on hit\"" },
						{ BuffStyle.OnHitEnemyBuff.ToString(), "\"Grants {0} to enemies on hit\"" },
						{ BuffStyle.OnHitEnemyDebuff.ToString(), "\"Inflicts {0} to enemies on hit\"" },
						{ BuffStyle.OnHitAreaTeamBuff.ToString(), "\"Grants {0} to nearby players on hit\"" },
						{ BuffStyle.OnHitAreaTeamDebuff.ToString(), "\"Inflicts {0} to nearby players on hit\"" },
						{ BuffStyle.OnHitAreaEnemyBuff.ToString(), "\"Grants {0} to nearby enemies on hit\"" },
						{ BuffStyle.OnHitAreaEnemyDebuff.ToString(), "\"Passively inflicts {0} to nearby enemies on hit\"" }
					}) },
					{ "CustomEffect", new(dict: new() {
						{ "AllForOne", "\"(Item CD equal to {0}x use speed)\"" },
						{ "AmmoCost", "\"{0} (Also Saves Bait When Fishing)\"" },
						{ "EnemySpawnRate", 
							"(Minion Damage is reduced by your spawn rate multiplier, from enchantments, unless they are your minion attack target)\n" +
							"(minion attack target set from hitting enemies with whips or a weapon that is converted to summon damage from an enchantment)\n" +
							"(Prevents consuming boss summoning items if spawn rate multiplier, from enchantments, is > 1.6)\n" +
							"(Enemies spawned will be immune to lava/traps)"},
						{ "FishingEnemySpawnChance", "\"{0} (Reduced by 5x during the day.  Affected by Chum Caster.  Can also spawn Duke Fishron)\"" },
						{ "GodSlayer", "\"{0} (Bonus true damage based on enemy max hp)\n(Bonus damage not affected by LifeSteal)\"" },
						{ "LavaFishing", "\"{0} (Allows fishing in lava and has a chance to improve catch rates in lava.  Stacks with other souces.)\""},
						{ "MinionAttackTarget", "Enemies hit become the minion attack target.  Same effect as whips."},
						{ "OnHitSpawnProjectile", "\"Spawns a projectile when hitting an enemy: {0}\""},
						{ "QuestFishChance", "\"{0} (Quest fish caught will be automatically turned in and start a new quest, bypassing the 1 per day limmit.)\""}
					}) },
					{ "PlayerSetEffect", new(children: new() {
						{ "VanillaDash", new(new() {
							{ DashID.NinjaTabiDash.ToString() },
							{ DashID.EyeOfCthulhuShieldDash.ToString() },
							{ DashID.SolarDash.ToString() },
							{ DashID.CrystalNinjaDash.ToString() }
						}) }
					})}
				}) },
				{ L_ID2.EnchantmentCustomTooltips.ToString(), new(dict: new() {
					{ "WorldAblaze", 
					 	"(Amaterasu debuff and below notes about it only apply at Enchantment tier 4.)\n" +
						"(None shall survive the unstopable flames of Amaterasu)\n" +
						"(Inflict a unique fire debuff to enemies that never stops)\n" +
						"(The damage from the debuff grows over time and from dealing more damage to the target)\n" +
						"(Spreads to nearby enemies and prevents enemies from being immune from other WorldAblaze debuffs.)" }
				}) }
			}) },
			{ L_ID1.Dialogue.ToString(), new(children: new() {
				{ L_ID2.Witch.ToString(), new(dict: new() {
					{ "StandardDialogue1", "test1" },
					{ "StandardDialogue2", "test2" },
					{ "StandardDialogue3", "test3" },
					{ "CommonDialogue", "comon" },
					{ "RareDialogue", "rarity" },
					{ "TalkALot", "talk Alot" },
					{ "PartyGirlDialogue", "ugh Party Girl...." },
					{ "BigAsMine", "\"I see your {0} is as big as mine.\"" }
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
			{ "Ores", new(new() { 
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
				{ "titanium" }
			}) }
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
}
