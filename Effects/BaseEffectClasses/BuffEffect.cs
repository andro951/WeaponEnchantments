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
    public abstract class BuffEffect : EnchantmentEffect {
        public BuffStats BuffStats { get; protected set; }
        public string BuffName => BuffStats.BuffName;
        public override string Tooltip {
            get {
                string tooltip = "";
                tooltip += $"{DisplayName} ({BuffStats.Chance.Percent()}% chance to apply for {BuffStats.Duration})";
                return tooltip;
            }
        }

        protected BuffEffect(int buffID, uint duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) {
            string buffName = GetBuffName(buffID);
            BuffStats = new BuffStats(buffName, (short)buffID, new Time(duration), chance, disableImmunity, buffStrength);
        }
        protected BuffEffect(short buffID, uint duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) {
            string buffName = GetBuffName(buffID);
            BuffStats = new BuffStats(buffName, buffID, new Time(duration), chance, disableImmunity, buffStrength);
        }
        protected BuffEffect(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) {
            string buffName = GetBuffName(buffID);
            if (chance == null) {
                BuffStats = new BuffStats(buffName, (short)buffID, new Time(duration), 1f, disableImmunity, buffStrength);
            }
			else {
                BuffStats = new BuffStats(buffName, (short)buffID, new Time(duration), chance, disableImmunity, buffStrength);
            }
        }
        protected BuffEffect(short buffID, uint duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) {
            string buffName = GetBuffName(buffID);
            if (chance == null) {
                BuffStats = new BuffStats(buffName, (short)buffID, duration, 1f, disableImmunity, buffStrength);
            }
            else {
                BuffStats = new BuffStats(buffName, (short)buffID, duration, chance, disableImmunity, buffStrength);
            }
        }
		public static string GetBuffName(int id) { // C# is crying
            if (id < BuffID.Count) {
                BuffID buffID = new();
                return buffID.GetType().GetFields().Where(field => field.FieldType == typeof(int) && (int)field.GetValue(buffID) == id).First().Name;
            }

            return ModContent.GetModBuff(id).Name;
        }
    }
    public abstract class OnTickPlayerBuffEffectGeneral : BuffEffect {
        protected OnTickPlayerBuffEffectGeneral(int buffID, uint duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        protected OnTickPlayerBuffEffectGeneral(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        /*public void PostUpdateMiscEffects(WEPlayer player) {
            player.Player
            //player.Player.AddBuff(BuffStats.BuffID, 5);
        }*/
    }
    public class OnTickPlayerBuffEffect : OnTickPlayerBuffEffectGeneral {
        public OnTickPlayerBuffEffect(int buffID, uint duration = 60, float chance = 1f, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public OnTickPlayerBuffEffect(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public override string DisplayName => $"Passively grants {BuffName}";
    }
    public class OnTickPlayerDebuffEffect : OnTickPlayerBuffEffectGeneral {
        public OnTickPlayerDebuffEffect(int buffID, uint duration = 60, float chance = 1f, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public OnTickPlayerDebuffEffect(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public override string DisplayName => $"Passively inflicts {BuffName}";
    }
    
    public abstract class OnTickAreaBuff : BuffEffect {
        protected OnTickAreaBuff(int buffID, uint duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        protected OnTickAreaBuff(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        //public abstract void PostUpdateMiscEffects(WEPlayer player);
    }
    
    public abstract class OnTickTeamBuffEffectGeneral : OnTickAreaBuff {
        protected OnTickTeamBuffEffectGeneral(int buffID, uint duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        protected OnTickTeamBuffEffectGeneral(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        /*public override void PostUpdateMiscEffects(WEPlayer player) {
            //Apply to all nearby players and self.  Only call once per second? and apply for some duration
            //player.Player.AddBuff(AppliedBuffID, 5, IsQuiet);
        }*/
    }
    public class OnTickTeamBuffEffect : OnTickPlayerBuffEffectGeneral {
        public OnTickTeamBuffEffect(int buffID, uint duration, float chance = 1f, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public OnTickTeamBuffEffect(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public override string DisplayName => $"Passively grants {BuffName} to nearby players";
    }
    public class OnTickTeamDebuffEffect : OnTickPlayerBuffEffectGeneral {
        public OnTickTeamDebuffEffect(int buffID, uint duration, float chance = 1f, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public OnTickTeamDebuffEffect(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public override string DisplayName => $"Passively inflicts {BuffName} to nearby players";
    }
    
    public abstract class OnTickTargetBuffEffectGeneral : OnTickAreaBuff {
        protected OnTickTargetBuffEffectGeneral(int buffID, uint duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        protected OnTickTargetBuffEffectGeneral(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        /*public override void PostUpdateMiscEffects(WEPlayer player) {
            //Apply to all nearby enemy npcs.
            //player.Player.AddBuff(AppliedBuffID, 5, IsQuiet);
        }*/
    }
    public class OnTickTargetBuffEffect : OnTickTargetBuffEffectGeneral {
        public OnTickTargetBuffEffect(int buffID, uint duration, float chance = 1f, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public OnTickTargetBuffEffect(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public override string DisplayName => $"Passively grants {BuffName} to nearby enemies";
    }
    public class OnTickTargetdebuffEffect : OnTickTargetBuffEffectGeneral {
        public OnTickTargetdebuffEffect(int buffID, uint duration, float chance = 1f, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public OnTickTargetdebuffEffect(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public override string DisplayName => $"Passively inflicts {BuffName} to nearby enemies";
    }

    public abstract class OnHitPlayerBuffEffectGeneral : BuffEffect {
        protected OnHitPlayerBuffEffectGeneral(int buffID, uint duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        protected OnHitPlayerBuffEffectGeneral(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
    }
    public class OnHitPlayerBuffEffect : OnHitPlayerBuffEffectGeneral {
        public OnHitPlayerBuffEffect(int buffID, uint duration, float chance = 1f, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public OnHitPlayerBuffEffect(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public override string DisplayName => $"Grants you {BuffName} on hit";
    }
    public class OnHitPlayerDebuffEffect : OnHitPlayerBuffEffectGeneral {
        public OnHitPlayerDebuffEffect(int buffID, uint duration, float chance = 1f, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public OnHitPlayerDebuffEffect(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public override string DisplayName => $"Inflicts {BuffName} to you on hit";
    }
    
    public abstract class OnHitTargetBuffEffectGeneral : BuffEffect {
        protected OnHitTargetBuffEffectGeneral(int buffID, uint duration, float chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        protected OnHitTargetBuffEffectGeneral(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity, DifficultyStrength buffStrength) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
    }
    public class OnHitTargetBuffEffect : OnHitTargetBuffEffectGeneral {
        public OnHitTargetBuffEffect(int buffID, uint duration, float chance = 1f, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public OnHitTargetBuffEffect(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public override string DisplayName => $"Grants {BuffName} to enemies on hit";
    }
    public class OnHitTargetDebuffEffect : OnHitTargetBuffEffectGeneral {
        public OnHitTargetDebuffEffect(int buffID, uint duration, float chance = 1f, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public OnHitTargetDebuffEffect(int buffID, uint duration, DifficultyStrength chance, bool disableImmunity = true, DifficultyStrength buffStrength = null) : base(buffID, duration, chance, disableImmunity, buffStrength) { }
        public override string DisplayName => $"Inflicts {BuffName} to enemies on hit";
    }
}
