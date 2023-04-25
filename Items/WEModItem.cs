using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.Localization;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Localization;

namespace WeaponEnchantments.Items
{
	public abstract class WEModItem : ModItem
	{
		public virtual DropRestrictionsID DropRestrictionsID => DropRestrictionsID.None;
		public abstract List<WikiTypeID> WikiItemTypes { get; }
		public abstract string Artist { get; }
		public virtual string ArtModifiedBy => null;
		public abstract string Designer { get; }
		public virtual bool ConfigOnlyDrop => false;
		public virtual string WikiDescription => null;
		public virtual bool DynamicTooltip => false;
		public virtual string LocalizationTooltip { protected set; get; }
		protected string localizationTooltip;
		public abstract int CreativeItemSacrifice { get; }
		public virtual bool CanBeStoredInEnchantmentStroage => false;
		public override void SetStaticDefaults() {
			if (!WEMod.serverConfig.DisableResearch && CreativeItemSacrifice > -1)
				CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = CreativeItemSacrifice;

			if (!DynamicTooltip)
				this.AddLocalizationTooltip(LocalizationTooltip);

			LogModSystem.UpdateContributorsList(this);
		}
	}
}
