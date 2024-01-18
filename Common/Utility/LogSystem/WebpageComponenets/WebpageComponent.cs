using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common.Utility.LogSystem.WebpageComponenets
{
	public abstract class WebpageComponent
	{
		public virtual FloatID AlignID { protected set; get; } = FloatID.none;
	}
}
