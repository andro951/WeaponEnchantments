using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects {
    public interface IUseTimer<PlayerSetStat> {
        public Time TimerDuration { set; get; }
        //public EnchantmentStat statName { set; }
        public void SetTimer();
        public void CheckTimer();
		public void TimerEnd();
        public void ActivateEffect();
	}
}
