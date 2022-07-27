using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common.Utility
{
	public class LogUtilities : ModSystem
    {
        public static bool printListOfContributors = false;

        public static bool printListOfEnchantmentTooltips = false;



        //Only used to print the full list of contributors.
        private static Dictionary<string, string> contributorLinks = new Dictionary<string, string>() {
            { "Zorutan", "https://twitter.com/ZorutanMesuta" }
		};

        public struct Contributors
        {
            public Contributors(string artist, string designer) {
                Artist = artist;
                Designer = designer;
            }
            public string Artist;
            public string Designer;
        }
        public static SortedDictionary<string, Contributors> contributorsData = new SortedDictionary<string, Contributors>();
        public static List<string> namesAddedToContributorDictionary = new List<string>();


        //Only used to print the full list of enchantment tooltips in WEPlayer OnEnterWorld()  (Normally commented out there)
        public static string listOfAllEnchantmentTooltips = "";

        //Requires an input type to have properties: Texture
        public static void UpdateContributorsList<T>(T modTypeWithTexture, string sharedName = null) {
            if (!printListOfContributors)
                return;

            //Already added
            if (sharedName != null && namesAddedToContributorDictionary.Contains(sharedName))
                return;

            Type thisObjectsType = modTypeWithTexture.GetType();
            string texture = (string)thisObjectsType.GetProperty("Texture").GetValue(modTypeWithTexture);
            string artist = (string)thisObjectsType.GetProperty("Artist").GetValue(modTypeWithTexture);
            string designer = (string)thisObjectsType.GetProperty("Designer").GetValue(modTypeWithTexture);

            if (!contributorsData.ContainsKey(texture))
                contributorsData.Add(texture, new Contributors(artist, designer));

            if (sharedName != null)
                namesAddedToContributorDictionary.Add(sharedName);
        }

		public override void OnWorldLoad() {
            if (!printListOfContributors)
                return;

            //Enchantment tooltips
            if (printListOfEnchantmentTooltips)
                listOfAllEnchantmentTooltips.Log();

            //Contributors  change to give exact file location when added to contributor.
            if (printListOfContributors) {
                //New dictionary with artist names as the key
                SortedDictionary<string, List<string>> artistCredits = new SortedDictionary<string, List<string>>();
                foreach (string key in contributorsData.Keys) {
                    string artistName = contributorsData[key].Artist;
                    if (artistName != null) {
                        if (artistCredits.ContainsKey(artistName)) {
                            artistCredits[artistName].Add(key);
                        }
                        else {
                            artistCredits.Add(artistName, new List<string>() { key });
                        }
                    }
                }

                //Create and print the GitHub Artist credits.
                string artistsMessage = "";
                foreach (string artistName in artistCredits.Keys) {
                    artistsMessage += $"\n{artistName}: ";
                    if (contributorLinks.ContainsKey(artistName))
                        artistsMessage += contributorLinks[artistName];

                    artistsMessage += "\n\n";
                    foreach (string texture in artistCredits[artistName]) {
                        artistsMessage += $"![{texture.GetFileName('/')}]({texture.RemoveNameSpace('/', false)}.png)\n";
                    }
                }
                artistsMessage.Log();

                namesAddedToContributorDictionary.Clear();
                contributorsData.Clear();
            }
        }
	}
}
