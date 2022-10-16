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

namespace WeaponEnchantments.Effects {
    public class BuffEffect : EnchantmentEffect {
        public BuffStats BuffStats { get; protected set; }
        public string BuffName => BuffStats.BuffName;
        public BuffStyle BuffStyle;
		public override string TooltipKey => BuffStyle == BuffStyle.OnTickPlayerBuff || BuffStyle == BuffStyle.OnTickPlayerDebuff ? $"{BuffStyle.OnTickPlayerBuff}" : $"{BuffStyle.All}";
		public override IEnumerable<object> TooltipArgs => BuffStyle == BuffStyle.OnTickPlayerBuff || BuffStyle == BuffStyle.OnTickPlayerDebuff ? new object[] { DisplayName, BuffStats.Chance.Percent(), BuffStats.Duration, ConfigValues.BuffDurationTicks } : new object[] { DisplayName, BuffStats.Chance.Percent(), BuffStats.Duration };
		public override string TooltipValue => $"{BuffStats.Chance.Percent()}, {BuffStats.Duration}";
		public override int DisplayNameNum => (int)BuffStyle;
		public override IEnumerable<object> DisplayNameArgs => new object[] { BuffName };

		public BuffEffect(int buffID, BuffStyle buffStyle, uint duration = 60, float chance = 1f, DifficultyStrength buffStrength = null, bool disableImmunity = true) {
            BuffStyle = buffStyle;
            string buffName = GetBuffName(buffID);
            BuffStats = new BuffStats(buffName, (short)buffID, new Time(duration), chance, disableImmunity, buffStrength);
        }
        public BuffEffect(short buffID, BuffStyle buffStyle, uint duration = 60, float chance = 1f, DifficultyStrength buffStrength = null, bool disableImmunity = true) {
            BuffStyle = buffStyle;
            string buffName = GetBuffName(buffID);
            BuffStats = new BuffStats(buffName, buffID, new Time(duration), chance, disableImmunity, buffStrength);
        }
        public BuffEffect(int buffID, BuffStyle buffStyle, uint duration, DifficultyStrength chance, DifficultyStrength buffStrength = null, bool disableImmunity = true) {
            BuffStyle = buffStyle;
            string buffName = GetBuffName(buffID);
            if (chance == null) {
                BuffStats = new BuffStats(buffName, (short)buffID, new Time(duration), 1f, disableImmunity, buffStrength);
            }
			else {
                BuffStats = new BuffStats(buffName, (short)buffID, new Time(duration), chance, disableImmunity, buffStrength);
            }
        }
        public BuffEffect(short buffID, BuffStyle buffStyle, uint duration, DifficultyStrength chance, DifficultyStrength buffStrength = null, bool disableImmunity = true) {
            BuffStyle = buffStyle;
            string buffName = GetBuffName(buffID);
            if (chance == null) {
                BuffStats = new BuffStats(buffName, buffID, duration, 1f, disableImmunity, buffStrength);
            }
            else {
                BuffStats = new BuffStats(buffName, buffID, duration, chance, disableImmunity, buffStrength);
            }
        }
        public BuffEffect(BuffStyle buffStyle, BuffStats buffStats) {
            BuffStyle = buffStyle;
            BuffStats = buffStats;
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
                    if (name.Lang(out string result, L_ID1.Tooltip, L_ID2.VanillaBuffs))
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
    }
}
