/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using WeaponEnchantments.UI;
using WeaponEnchantments.UI.WeaponEnchantmentUI;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;


namespace WeaponEnchantments
{
    internal class WEUISystem : ModSystem
    {
        internal static UserInterface weaponEnchantmentInterface;

        internal static UserInterface mouseoverUIInterface;
        internal static MouseoverUI mouseoverUI;

        private GameTime _lastUpdateUiGameTime;
		public override void OnModLoad()
		{
			if (!Main.dedServ)
			{
				weaponEnchantmentInterface = new UserInterface();

				mouseoverUI = new MouseoverUI();
				mouseoverUI.Activate();
				mouseoverUIInterface = new UserInterface();
				mouseoverUIInterface.SetState(mouseoverUI);
			}
		}
		public override void Unload()
		{
			if (!Main.dedServ)
			{
				weaponEnchantmentInterface = null;

				mouseoverUIInterface = null;
				mouseoverUI = null;
			}
		}
		internal static void  ToggleWeaponEnchantmentUI()
		{
			if (weaponEnchantmentInterface.CurrentState != null)
			{
				CloseWeaponEnchantmentUI();
			}
			else
			{
				OpenWeaponEnchantmentUI();
			}
		}
		internal static void OpenWeaponEnchantmentUI()
		{
			WeaponEnchantmentUI ui = new WeaponEnchantmentUI();
			UIState state = new UIState();
			state.Append(ui);
			weaponEnchantmentInterface.SetState(state);
		}

		internal static void CloseWeaponEnchantmentUI()
		{
			weaponEnchantmentInterface.SetState(null);
		}
		public override void PreSaveAndQuit()
		{
			//Calls Deactivate and drops the item
			if (weaponEnchantmentInterface.CurrentState != null)
			{
				//WeaponEnchantmentUI.saveItemInUI = true;
				weaponEnchantmentInterface.SetState(null);
			}
		}
		public override void UpdateUI(GameTime gameTime)
		{
			_lastUpdateUiGameTime = gameTime;
			if (weaponEnchantmentInterface?.CurrentState != null)
			{
				weaponEnchantmentInterface.Update(gameTime);
			}
			mouseoverUI.Update(gameTime);
		}
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Over"));
			if (index != -1)
			{
				layers.Insert(++index, new LegacyGameInterfaceLayer
					(
					"WeaponEnchantment: Mouse Over",
					delegate
					{
						if (_lastUpdateUiGameTime != null && mouseoverUIInterface?.CurrentState != null)
						{
							mouseoverUIInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
			index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (index != -1)
			{
				layers.Insert(index, new LegacyGameInterfaceLayer(
					"WeaponEnchantment: Enchant Weapon",
					delegate
					{
						if (_lastUpdateUiGameTime != null && weaponEnchantmentInterface?.CurrentState != null)
						{
							weaponEnchantmentInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
*/