using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects {
    public interface IUseTimer {
        public Time TimerDuration { get; }
        public bool MultipleAllowed => false;
        public EnchantmentStat TimerStatName { get; }
        public virtual void SetTimer(WEPlayer wePlayer) {
            wePlayer.SetEffectTimer(this);
        }
        public virtual bool CheckTimer(Player player) {
            if (player == null)
                player = Main.LocalPlayer;

            return player.GetWEPlayer().CheckTimer(this);
        }
        public virtual void TimerEnd(WEPlayer wePlayer) { }
        public virtual void ActivateEffect(WEPlayer wePlayer) {
            SetTimer(wePlayer);
        }
    }
}
