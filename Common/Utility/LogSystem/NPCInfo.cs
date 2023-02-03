using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets;
using WeaponEnchantments.Items;
using static WeaponEnchantments.Common.Utility.LogSystem.Wiki;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Common.Globals;
using Terraria.GameContent.UI;
using Microsoft.Xna.Framework;
using Terraria.ID;
using static WeaponEnchantments.Common.Utility.LogModSystem;
using WeaponEnchantments.Content.NPCs;
using Terraria.GameContent.Personalities;

namespace WeaponEnchantments.Common.Utility.LogSystem
{
	public class NPCInfo
	{
		public ModNPC ModNPC { private set; get; }
		public NPC NPC { private set; get; }
		public INPCWikiInfo WikiInfo { private set; get; }
		public NPCInfo(int modNPCType) {
			NPC = ContentSamples.NpcsByNetId[modNPCType];
			ModNPC = NPC.ModNPC;
			WikiInfo = (INPCWikiInfo)ModNPC;
		}
		public void AddStatistics(WebPage webpage, bool name = true, bool image = true, bool artists = true, bool types = true, bool aiTypes = true, bool combatStats = true) {
			List<List<string>> info = new();
			List<string> labels = new();

			if (name)
				labels.Add(GetName());

			if (image)
				labels.Add($"{NPC.netID.ToNpcPNG(displayName: false)}");

			if (artists) {
				GetArtists(out string artistString, out string artModifiedBy);

				if (artistString != null || artModifiedBy != null)
					labels.Add($"{artistString}{artModifiedBy}");
			}

			labels.Add("Statistics");

			//Type
			if (types)
				info.Add(new() { "Type", GetTypes() });

			if (aiTypes)
				info.Add(new() { "https://terraria.fandom.com/wiki/AI".ToExternalLink("AI Type"), NPCAIStyleID.Search.GetName(NPC.aiStyle) });

			if (combatStats) {
				int damage = 0;
				float knockback = 0f;
				ModNPC.TownNPCAttackStrength(ref damage, ref knockback);
				if (damage > 0)
					info.Add(new() { "Damage", $"{damage}" });

				info.Add(new() { $"Max Life", $"{NPC.RealLifeMax()}" });
				info.Add(new() { "https://terraria.fandom.com/wiki/Defense".ToExternalLink("Defense"), $"{NPC.defDefense}" });
				info.Add(new() { $"{"https://terraria.fandom.com/wiki/Knockback".ToExternalLink("KB")} Resist", $"{NPC.knockBackResist.PercentString()}" });
			}

			webpage.AddTable(info, headers: labels, maxWidth: 400, alignID: FloatID.right, collapsible: true);
		}
		public string GetName() => NPC.netID < 0 ? NPCID.Search.GetName(NPC.netID).AddSpaces(true) : NPC.FullName;
		public void GetArtists(out string artistString, out string artModifiedBy) {
			artistString = ((INPCWikiInfo)ModNPC).Artist;
			artModifiedBy = ((INPCWikiInfo)ModNPC).ArtModifiedBy;
			if (artistString == "andro951")
				artistString = null;

			if (artistString != null) {
				if (contributorLinks.ContainsKey(artistString))
					artistString = contributorLinks[artistString].ToExternalLink(artistString);

				artistString = $"  (art by {artistString}{(artModifiedBy == null ? ")" : "")}";
			}

			if (artModifiedBy != null) {
				if (contributorLinks.ContainsKey(artModifiedBy))
					artModifiedBy = contributorLinks[artModifiedBy].ToExternalLink(artModifiedBy);

				artModifiedBy = $"{(artistString == null ? "  (" : " ")}modified by {artModifiedBy})";
			}
		}
		public string GetTypes() {
			string typeText = "";
			bool first = true;
			foreach (WikiTypeID id in ((INPCWikiInfo)ModNPC).WikiNPCTypes) {
				if (first) {
					first = false;
				}
				else {
					typeText += ", ";
				}
				string linkText = id.GetLinkText(out bool external);
				typeText += external ? linkText.ToExternalLink(id.ToString().AddSpaces()) : linkText.ToLink(id.ToString().AddSpaces());
			}

			return typeText;
		}
		public void AddSpawnCondition(WebPage webpage) {
			if (WikiInfo.TownNPC) {
				webpage.AddSubHeading("Spawn Condition");
				webpage.AddParagraph($"This npc will spawn when the folowing condition is met:\n<br/>" +
					$"\t{WikiInfo.SpawnCondition}");
			}
		}
		public void AddLivingPreferences(WebPage webpage) {
			if (WikiInfo.TownNPC) {
				webpage.AddSubHeading("Living Preferences");
				List<List<string>> livingPreferences = new() {
					new List<string>() { "", "Biome", "Neighbor" },
					new() { "Loves", "", "" },
					new() { "Likes", "", "" },
					new() { "Dislikes", "", "" },
					new() { "Hates", "", "" }
				};
				foreach(KeyValuePair<IShoppingBiome, AffectionLevel> pair in WikiInfo.BiomeAffections) {
					int index = PreferenceIndex(pair.Value);
					if (livingPreferences[index][1] != "")
						livingPreferences[index][1] += " ";

					livingPreferences[index][1] += $"{pair.Key.GetPNGLink()} {pair.Key.GetLinkText().ToExternalLink(pair.Key.NameKey)}" ;
				}

				List<List<string>> npcAffections = new();
				foreach(KeyValuePair<int, AffectionLevel> pair in WikiInfo.NPCAffections) {
					int index = PreferenceIndex(pair.Value);
					if (livingPreferences[index][2] != "")
						livingPreferences[index][2] += " ";

					livingPreferences[index][2] += pair.Key.ToNpcPNG(link: true);
				}

				webpage.AddTable(
					elements: livingPreferences,
					firstRowHeaders: true
				);
			}
		}
		public int PreferenceIndex(AffectionLevel affectionLevel) {
			switch (affectionLevel) {
				case AffectionLevel.Love:
					return 1;
				case AffectionLevel.Like:
					return 2;
				case AffectionLevel.Dislike:
					return 3;
				case AffectionLevel.Hate:
					return 4;
			}

			return -1;
		}
	}
}
