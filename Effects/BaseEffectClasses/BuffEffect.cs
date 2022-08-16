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
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects {
    public abstract class BuffEffect : EnchantmentEffect {
        private int AppliedBuffID { get; set; }
        private Time Duration;
        private string BuffName;
        
        public BuffEffect(int buffID, Time duration, float chance) : base() {
            AppliedBuffID = buffID;
            Duration = duration;
            BuffName = GetBuffName(AppliedBuffID);
        }
        
        public static string GetBuffName(int id) { // C# is crying
            if (id < BuffID.Count) {
                BuffID buffID = new();
                return buffID.GetType().GetFields().Where(field => field.FieldType == typeof(int) && (int)field.GetValue(buffID) == id).First().Name;
            }

            return ModContent.GetModBuff(id).Name;
        }
    }
    public abstract class OnTickPlayerBuffEffectGeneral {
        public void PostUpdateMiscEffects(WEPlayer player) {
            player.Player.AddBuff(AppliedBuffID, 5, IsQuiet);
        }
    }
    public class OnTickPlayerBuffEffect : OnTickPlayerBuffEffectGeneral {
        public override string DisplayName => $"Passive buff {BuffName}";
        public override string Tooltip => $"Passively grants {BuffName}";
    }
    public class OnTickPlayerDebuffEffect : OnTickPlayerBuffEffectGeneral {
        public override string DisplayName => $"Passive {BuffName}";
        public override string Tooltip => $"Passively inflicts {BuffName}";
    }
    
    public abstract class OnTickAreaBuff{
        public abstract void PostUpdateMiscEffects(WEPlayer player);
    }
    
    public abstract class OnTickTeamBuffEffectGeneral {
        public override void PostUpdateMiscEffects(WEPlayer player) {
            //Apply to all nearby players and self.  Only call once per second? and apply for some duration
            //player.Player.AddBuff(AppliedBuffID, 5, IsQuiet);
        }
    }
    public class OnTickTeamBuffEffect : OnTickPlayerBuffEffectGeneral {
        public override string DisplayName => $"Passive team buff {BuffName}";
        public override string Tooltip => $"Passively grants {BuffName} to nearby players";
    }
    public class OnTickTeamDebuffEffect : OnTickPlayerBuffEffectGeneral {
        public override string DisplayName => $"Passive team debuff {BuffName}";
        public override string Tooltip => $"Passively inflicts {BuffName} to nearby players";
    }
    
    public abstract class OnTickTargetBuffEffectGeneral {
        public override void PostUpdateMiscEffects(WEPlayer player) {
            //Apply to all nearby enemy npcs.
            //player.Player.AddBuff(AppliedBuffID, 5, IsQuiet);
        }
    }
    public class OnTickTargetBuffEffect : OnTickTargetBuffEffectGeneral {
        public override string DisplayName => $"Passive enemy buff {BuffName}";
        public override string Tooltip => $"Passively grants {BuffName} to nearby enemies";
    }
    public class OnTickTargetdebuffEffect : OnTickTargetDebuffEffectGeneral {
        public override string DisplayName => $"Passive enemy debuff {BuffName}";
        public override string Tooltip => $"Passively inflicts {BuffName} to nearby enemies";
    }
    
    public abstract class OnHitPlayerBuffEffectGeneral {
      
    }
    pubilc class OnHitPlayerBuffEffect : OnHitPlayerBuffEffectGeneral {
        public override string DisplayName => $"On hit player buff {BuffName}";
        public override string Tooltip => $"Grants you {BuffName} on hit";
    }
    pubilc class OnHitPlayerDebuffEffect : OnHitPlayerBuffEffectGeneral {
        public override string DisplayName => $"On hit player debuff {BuffName}";
        public override string Tooltip => $"Inflicts {BuffName} to you on hit";
    }
    
    public abstract class OnHitTargetBuffEffectGeneral {
      
    }
    public class OnHitTargetBuffEffect : OnHitTargetBuffEffectGeneral {
        public override string DisplayName => $"On hit enemy buff {BuffName}";
        public override string Tooltip => $"Grants {BuffName} to enemies on hit";
    }
    public class OnHitTargetDebuffEffect : OnHitTargetBuffEffectGeneral {
        public override string DisplayName => $"On hit enemy debuff {BuffName}";
        public override string Tooltip => $"Inflicts {BuffName} to enemies on hit";
    }
}
