using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects {
    public class BuffEffect : EnchantmentEffect {
        public BuffStats BuffStats { get; protected set; }
        public string BuffName => BuffStats.BuffName;
        public BuffStyle BuffStyle;
        public override string Tooltip {
            get {
                string tooltip = "";
                tooltip += $"{DisplayName} ({BuffStats.Chance.Percent()}% chance to apply for {BuffStats.Duration}{(BuffStyle == BuffStyle.OnTickPlayerBuff || BuffStyle == BuffStyle.OnTickPlayerDebuff ? $" every {ConfigValues.BuffDurationTicks}" : "")})";
                return tooltip;
            }
        }
		public override string DisplayName { 
            get { 
                if (displayName == null) {
                    switch (BuffStyle) {
                        case BuffStyle.OnTickPlayerBuff:
                            displayName = $"Passively grants {BuffName} to you";
                            break;
						case BuffStyle.OnTickPlayerDebuff:
                            displayName = $"Passively inflicts {BuffName} to you";
                            break;
                        case BuffStyle.OnTickAreaTeamBuff:
                            displayName = $"Passively grants {BuffName} to nearby players";
                            break;
                        case BuffStyle.OnTickAreaTeamDebuff:
                            displayName = $"Passively inflicts {BuffName} to nearby players";
                            break;
                        case BuffStyle.OnTickEnemyBuff:
                            displayName = $"Passively grants {BuffName} to enemy";
                            break;
                        case BuffStyle.OnTickEnemyDebuff:
                            displayName = $"Passively inflicts {BuffName} to enemy";
                            break;
                        case BuffStyle.OnTickAreaEnemyBuff:
                            displayName = $"Passively grants {BuffName} to nearby enemies";
                            break;
                        case BuffStyle.OnTickAreaEnemyDebuff:
                            displayName = $"Passively inflicts {BuffName} to nearby enemies";
                            break;
                        case BuffStyle.OnHitPlayerBuff:
                            displayName = $"Grants you {BuffName} on hit";
                            break;
                        case BuffStyle.OnHitPlayerDebuff:
                            displayName = $"Inflicts {BuffName} to you on hit";
                            break;
                        case BuffStyle.OnHitEnemyBuff:
                            displayName = $"Grants {BuffName} to enemies on hit";
                            break;
                        case BuffStyle.OnHitEnemyDebuff:
                            displayName = $"Inflicts {BuffName} to enemies on hit";
                            break;
                        case BuffStyle.OnHitAreaTeamBuff:
                            displayName = $"Grants {BuffName} to nearby players on hit";
                            break;
                        case BuffStyle.OnHitAreaTeamDebuff:
                            displayName = "Inflicts {BuffName} to nearby players on hit";
                            break;
                        case BuffStyle.OnHitAreaEnemyBuff:
                            displayName = $"Grants {BuffName} to nearby enemies on hit";
                            break;
                        case BuffStyle.OnHitAreaEnemyDebuff:
                            displayName = $"Passively inflicts {BuffName} to nearby enemies on hit";
                            break;
                    }
				}

                return displayName;
            }
        }

        private string displayName;

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
		public string GetBuffName(int id) { // C# is crying
            string name = null;
            if (id < BuffID.Count) {
                BuffID buffID = new();
                name = buffID.GetType().GetFields().Where(field => field.FieldType == typeof(int) && (int)field.GetValue(buffID) == id).First().Name;//Can be replaced with BuffID.Search....
            }

            name ??= ModContent.GetModBuff(id).Name;

            return name;
        }
    }
}
