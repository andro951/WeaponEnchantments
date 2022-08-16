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

        protected BuffEffect(int buffID, Time duration, float chance) : base() {
            string buffName = GetBuffName(buffID);
            BuffStats = new BuffStats(buffName, buffID, duration, chance);
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
        protected OnTickPlayerBuffEffectGeneral(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        /*public void PostUpdateMiscEffects(WEPlayer player) {
            player.Player
            //player.Player.AddBuff(BuffStats.BuffID, 5);
        }*/
    }
    public class OnTickPlayerBuffEffect : OnTickPlayerBuffEffectGeneral {
        public OnTickPlayerBuffEffect(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        public override string DisplayName => $"Passive buff {BuffName}";
        public override string Tooltip => $"Passively grants {BuffName}";
    }
    public class OnTickPlayerDebuffEffect : OnTickPlayerBuffEffectGeneral {
        public OnTickPlayerDebuffEffect(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        public override string DisplayName => $"Passive {BuffName}";
        public override string Tooltip => $"Passively inflicts {BuffName}";
    }
    
    public abstract class OnTickAreaBuff : BuffEffect {
        protected OnTickAreaBuff(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        //public abstract void PostUpdateMiscEffects(WEPlayer player);
    }
    
    public abstract class OnTickTeamBuffEffectGeneral : OnTickAreaBuff {
        protected OnTickTeamBuffEffectGeneral(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        /*public override void PostUpdateMiscEffects(WEPlayer player) {
            //Apply to all nearby players and self.  Only call once per second? and apply for some duration
            //player.Player.AddBuff(AppliedBuffID, 5, IsQuiet);
        }*/
    }
    public class OnTickTeamBuffEffect : OnTickPlayerBuffEffectGeneral {
        public OnTickTeamBuffEffect(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        public override string DisplayName => $"Passive team buff {BuffName}";
        public override string Tooltip => $"Passively grants {BuffName} to nearby players";
    }
    public class OnTickTeamDebuffEffect : OnTickPlayerBuffEffectGeneral {
        public OnTickTeamDebuffEffect(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        public override string DisplayName => $"Passive team debuff {BuffName}";
        public override string Tooltip => $"Passively inflicts {BuffName} to nearby players";
    }
    
    public abstract class OnTickTargetBuffEffectGeneral : OnTickAreaBuff {
        protected OnTickTargetBuffEffectGeneral(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        /*public override void PostUpdateMiscEffects(WEPlayer player) {
            //Apply to all nearby enemy npcs.
            //player.Player.AddBuff(AppliedBuffID, 5, IsQuiet);
        }*/
    }
    public class OnTickTargetBuffEffect : OnTickTargetBuffEffectGeneral {
        public OnTickTargetBuffEffect(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        public override string DisplayName => $"Passive enemy buff {BuffName}";
        public override string Tooltip => $"Passively grants {BuffName} to nearby enemies";
    }
    public class OnTickTargetdebuffEffect : OnTickTargetBuffEffectGeneral {
        public OnTickTargetdebuffEffect(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        public override string DisplayName => $"Passive enemy debuff {BuffName}";
        public override string Tooltip => $"Passively inflicts {BuffName} to nearby enemies";
    }

    public abstract class OnHitPlayerBuffEffectGeneral : BuffEffect {
        protected OnHitPlayerBuffEffectGeneral(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
    }
    public class OnHitPlayerBuffEffect : OnHitPlayerBuffEffectGeneral {
        public OnHitPlayerBuffEffect(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        public override string DisplayName => $"On hit player buff {BuffName}";
        public override string Tooltip => $"Grants you {BuffName} on hit";
    }
    public class OnHitPlayerDebuffEffect : OnHitPlayerBuffEffectGeneral {
        public OnHitPlayerDebuffEffect(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        public override string DisplayName => $"On hit player debuff {BuffName}";
        public override string Tooltip => $"Inflicts {BuffName} to you on hit";
    }
    
    public abstract class OnHitTargetBuffEffectGeneral : BuffEffect {
        protected OnHitTargetBuffEffectGeneral(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
    }
    public class OnHitTargetBuffEffect : OnHitTargetBuffEffectGeneral {
        public OnHitTargetBuffEffect(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        public override string DisplayName => $"On hit enemy buff {BuffName}";
        public override string Tooltip => $"Grants {BuffName} to enemies on hit";
    }
    public class OnHitTargetDebuffEffect : OnHitTargetBuffEffectGeneral {
        public OnHitTargetDebuffEffect(int buffID, Time duration, float chance) : base(buffID, duration, chance) { }
        public override string DisplayName => $"On hit enemy debuff {BuffName}";
        public override string Tooltip => $"Inflicts {BuffName} to enemies on hit";
    }
}
