using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Items
{
	public interface IItemWikiInfo
	{
		public virtual DropRestrictionsID DropRestrictionsID => DropRestrictionsID.None;
		public List<WikiTypeID> WikiItemTypes { get; }
		public string Artist { get; }
		public string ArtModifiedBy => null;
		public bool ConfigOnlyDrop => false;
		public string WikiDescription => null;
	}
}
