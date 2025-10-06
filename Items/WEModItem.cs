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
using androLib.Common.Utility;
using androLib.Items;

namespace WeaponEnchantments.Items
{
	public abstract class WEModItem : AndroModItem {
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public virtual bool CanBeStoredInEnchantmentStorage => false;
		public virtual DropRestrictionsID DropRestrictionsID => DropRestrictionsID.None;
		public abstract int CreativeItemSacrifice { get; }
		protected override Action<ModItem, string, string> AddLocalizationTooltipFunc => WeaponEnchantments.Localization.LocalizationDataStaticMethods.AddLocalizationTooltip;
		public override void SetStaticDefaults() {
			if (!WEMod.serverConfig.DisableResearch && CreativeItemSacrifice > -1) {
				CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = CreativeItemSacrifice;
			}
			else {
				CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = -1;
			}

			LogModSystem.UpdateContributorsList(this);//TODO: move to androLib
			base.SetStaticDefaults();
		}
	}
}
