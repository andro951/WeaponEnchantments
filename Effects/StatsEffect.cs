using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    public abstract class StatsEffect : EnchantmentEffect {
        protected enum PlayerStat {
            Defense,            // statDefense 
            MaxHP,              // statLife2
            MaxMP,              // statMana2
            LifeRegen,          // lifeRegen (per half a second)
            ManaRegen,          // manaRegen (per half a second)
            MoveSpeed,          // moveSpeed
            MoveAccel,          // runAcceleration
            WingAccel,          
            AmmoReservation,    
        }
        
        
        public override void PostUpdateMiscEffects(WEPlayer player) {
        }
    }
}
