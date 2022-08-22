using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponEnchantments.Effects {
    public interface IUseTimer {
    public TimerDuration { set; get; };
    public void SetTimer();
    public void CheckTimer();
		public void TimerEnd();
    public void ActivateEffect();
	}
}
