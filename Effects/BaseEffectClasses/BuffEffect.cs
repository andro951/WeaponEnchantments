using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using androLib.Common.Utility;
using Microsoft.Xna.Framework.Audio;

namespace WeaponEnchantments.Effects {
    public class BuffEffect : EnchantmentEffect {
        public BuffStats BuffStats { get; protected set; }
        public string BuffName => BuffStats.BuffName;
        public BuffStyle BuffStyle;
        public float Range;
		public override string TooltipKey => BuffStyle == BuffStyle.OnTickPlayerBuff || BuffStyle == BuffStyle.OnTickPlayerDebuff ? $"{BuffStyle.OnTickPlayerBuff}" : $"{BuffStyle.All}";
		public override IEnumerable<object> TooltipArgs => BuffStyle == BuffStyle.OnTickPlayerBuff || BuffStyle == BuffStyle.OnTickPlayerDebuff ? new object[] { DisplayName, BuffStats.Chance.Percent(), BuffStats.Duration, ConfigValues.BuffDurationTicks } : new object[] { DisplayName, BuffStats.Chance.Percent(), BuffStats.Duration };
		public override string TooltipValue => $"{BuffStats.Chance.PercentString()}, {BuffStats.Duration}";
		public override int DisplayNameNum => (int)BuffStyle;
		public override IEnumerable<object> DisplayNameArgs => new object[] { BuffName };
        private const int DefaultDuration = 60;
        private const float DefaultRange = 0f;
		public BuffEffect(int buffID, BuffStyle buffStyle, uint duration = DefaultDuration, float chance = 1f, DifficultyStrength buffStrength = null, bool disableImmunity = true, float range = DefaultRange) :
			this((short)buffID, buffStyle, duration, chance, buffStrength, disableImmunity, range) {}
        public BuffEffect(short buffID, BuffStyle buffStyle, uint duration = DefaultDuration, float chance = 1f, DifficultyStrength buffStrength = null, bool disableImmunity = true, float range = DefaultRange) {
			BuffStyle = buffStyle;
			string buffName = GetBuffName(buffID);
			BuffStats = new BuffStats(buffName, buffID, new Time(duration), chance, disableImmunity, buffStrength);
			Range = range;
		}
        public BuffEffect(int buffID, BuffStyle buffStyle, uint duration, DifficultyStrength chance, DifficultyStrength buffStrength = null, bool disableImmunity = true, float range = DefaultRange) :
			this((short)buffID, buffStyle, duration, chance, buffStrength, disableImmunity, range) {}
        public BuffEffect(short buffID, BuffStyle buffStyle, uint duration, DifficultyStrength chance, DifficultyStrength buffStrength = null, bool disableImmunity = true, float range = DefaultRange) {
			BuffStyle = buffStyle;
			string buffName = GetBuffName(buffID);
			if (chance == null) {
				BuffStats = new BuffStats(buffName, buffID, new Time(duration), 1f, disableImmunity, buffStrength);
			}
			else {
				BuffStats = new BuffStats(buffName, buffID, new Time(duration), chance, disableImmunity, buffStrength);
			}

			Range = range;
		}
        public BuffEffect(BuffStyle buffStyle, BuffStats buffStats, float range = DefaultRange) {
            BuffStyle = buffStyle;
            BuffStats = buffStats;
			Range = range;
		}
		public override EnchantmentEffect Clone() {
            return new BuffEffect(BuffStyle, BuffStats.Clone());
		}
		public string GetBuffName(int id) {
            string name = null;
            if (id < BuffID.Count) {
                name = BuffID.Search.GetName(id);
                string key = $"BuffName.{name}";
                string langName = Language.GetTextValue(key);
                if (langName != key && langName != "" && !langName.Contains("Buff Name")) {
                    name = langName;
                }
				else {
                    if (name.Lang_WE(out string result, L_ID1.Tooltip, L_ID2.VanillaBuffs))
                        name = result;
				}
            }

            name ??= Language.GetTextValue(ModContent.GetModBuff(id).DisplayName.Key);
            /*
            if (ModContent.GetContent<ModBuff>().Select(b => b.Name).Contains(name))
                name = name.Lang();
            */

            //string temp = ModContent.GetModBuff(id)?.DisplayName.Key;

            return name;
        }

		public List<int> ApplyEffect(NPC npc) {
			List<int> affected = new();
			switch (this.BuffStyle) {
				case BuffStyle.OnTickPlayerDebuff:
					switch (Main.netMode) {
						case NetmodeID.SinglePlayer:
							float distance = npc.position.Distance(Main.LocalPlayer.position);
							if (distance <= Range) {
								BuffStats.TryApplyToPlayer(Main.LocalPlayer);
								affected.Add(Main.myPlayer);
							}

							break;
						case NetmodeID.Server:
							for (int i = 0; i < Main.player.Length; i++) {
								Player player = Main.player[i];
								if (player.NullOrNotActive())
									continue;

								if (npc.position.Distance(player.position) <= Range) {
									BuffStats.TryApplyToPlayer(player);
									affected.Add(i);
								}
							}

							break;
					}

					break;
			}

			return affected;
		}
	}
}
