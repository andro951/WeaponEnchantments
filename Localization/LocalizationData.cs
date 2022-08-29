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
		public static GetLanguageManager(CultureName cultureName) {
			if (cultureName != CultureName)
				languageManager = new LanguageManager();


		}
		public static bool ContainsTextVAlue(string s, CultureName cultureName) {
			if (cultureName != CultureName)
				LanguageManager.Instance.SetLanguage((int)cultureName);


			bool returnValue = s == Language.GetTextValue(s);
			LanguageManager.Instance.SetLanguage((int)CultureName.English);
			return returnValue;
		}
		public static bool ContainsText(string s, ) => s == Language.GetText(s).;

		public static SortedDictionary<string, SData> AllData => new() {
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
				}) },
				{ "NewNPC", new(new() {
					{ "1" }
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
