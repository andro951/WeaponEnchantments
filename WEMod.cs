using IL.Terraria.Localization;
using MonoMod.RuntimeDetour.HookGen;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Debuffs;
using WeaponEnchantments.Items;


namespace WeaponEnchantments
{
    public class WEMod : Mod
    {
		internal static ServerConfig config = ModContent.GetInstance<ServerConfig>();
		//internal static ClientConfig clientConfig = ModContent.GetInstance<ClientConfig>();
		internal static bool IsEnchantable(Item item)
        {
			if((IsWeaponItem(item) || IsArmorItem(item) || IsAccessoryItem(item)) && !item.consumable)
            {
				return true;
			}
            else
            {
				return false;
            }
        }
		internal static bool IsWeaponItem(Item item)
		{
			return item != null && !item.IsAir && (item.damage > 0 && item.ammo == 0 || item.type == ItemID.CoinGun) && !item.accessory;
		}
		internal static bool IsArmorItem(Item item)
		{
			return item != null && !item.IsAir && !item.vanity && (item.headSlot > -1 || item.bodySlot > -1 || item.legSlot > -1);
		}
		internal static bool IsAccessoryItem(Item item)
		{
			return item != null && !item.IsAir && item.accessory && !IsArmorItem(item);
		}
		internal static bool IsEnchantmentItem(Item item, bool utility)
        {
			if(item.ModItem is AllForOneEnchantmentBasic)
            {
                if (utility)
                {
                    if (((AllForOneEnchantmentBasic)item.ModItem).Utility)
                    {
						return true;
                    }
                    else
                    {
						return false;
                    }
                }
                else
                {
					return true;
				}
			}
            else
            {
				return false;
            }
        }
		internal static bool IsEssenceItem(Item item)
        {
			if (item.ModItem is EnchantmentEssenceBasic)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public static class PacketIDs
		{
			public const byte TransferGlobalItemFields = 0;
			public const byte Enchantment = 1;
			public const byte Infusion = 2;
			public const byte OnHitEffects = 3;
		}
		//public void SendInfusionPacket()
		public void SendEnchantmentPacket(byte enchantmentSlotNumber, byte slotNumber, short itemType, short bank = -1, byte type = 1)
		{
			if(itemType > 0)
            {
				ModPacket packet = GetPacket();
				packet.Write(type);
				packet.Write(enchantmentSlotNumber);
				packet.Write(slotNumber);
				if (bank != -1)
					packet.Write(bank);
				packet.Write(itemType);
				packet.Send();
			}
		}
		public void SendPacket(byte type, Item newItem, Item oldItem, bool weapon = true, byte armorSlot = 0)
        {
			ModPacket packet = GetPacket();
			packet.Write(type);
            switch (type)
            {
				case PacketIDs.TransferGlobalItemFields:
					packet.Write(weapon);
					packet.Write(armorSlot);
					bool writeNewItem = IsEnchantable(newItem);
					packet.Write(writeNewItem);
					if (writeNewItem)
						WriteItem(newItem, packet);
					bool writeOldItem = IsEnchantable(oldItem);
					packet.Write(writeOldItem);
					if (writeOldItem)
						WriteItem(oldItem, packet);
					break;
            }
			packet.Send();
		}
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
			byte type = reader.ReadByte();
			if(UtilityMethods.debugging) ($"\\/HandlePacket(reader, " + whoAmI + ": " + Main.player[whoAmI].name + ") type: " + type).Log();
			switch (type)
			{
				case PacketIDs.TransferGlobalItemFields:
					bool weapon = reader.ReadBoolean();
					byte armorSlot = reader.ReadByte();
					WEPlayer wePlayer = Main.player[whoAmI].GetModPlayer<WEPlayer>();
					Item newItem = weapon ? armorSlot == 0 ? wePlayer.Player.HeldItem : Main.mouseItem : wePlayer.Player.armor[armorSlot];
					Item oldItem = weapon ? wePlayer.trackedWeapon : wePlayer.equipArmor[armorSlot];
					bool readNewItem = reader.ReadBoolean();
					if (readNewItem)
						ReadItem(newItem, reader);
					else
						newItem = new Item();
					bool readOldItem = reader.ReadBoolean();
					if (readOldItem)
					{
						oldItem = new Item(ItemID.CopperShortsword);
						ReadItem(oldItem, reader);
					}
					wePlayer.UpdatePotionBuffs(ref newItem, ref oldItem);
					wePlayer.UpdatePlayerStats(ref newItem, ref oldItem);
					break;
				case PacketIDs.Enchantment:
					byte enchantmentSlotNumber = reader.ReadByte();
					byte slotNumber = reader.ReadByte();
					short bank = -1;
					if(slotNumber >= 50 && slotNumber < 90)
						bank = reader.ReadByte();
					short itemType = reader.ReadInt16();
					Item item = new Item();
					switch(slotNumber)
					{
						case < 50:
							item = Main.player[whoAmI].inventory[slotNumber];
							break;
						case < 90:
							switch(bank)
							{
								case -2:
									item = Main.player[whoAmI].bank.item[slotNumber - 50];
									break;
								case -3:
									item = Main.player[whoAmI].bank2.item[slotNumber - 50];
									break;
								case -4:
									item = Main.player[whoAmI].bank3.item[slotNumber - 50];
									break;
								case -5:
									item = Main.player[whoAmI].bank4.item[slotNumber - 50];
									break;
							}
							break;
						case 90:
							item = Main.player[whoAmI].GetModPlayer<WEPlayer>().enchantingTableUI.itemSlotUI[0].Item;
							break;
						case <= 100:
							item = Main.player[whoAmI].armor[slotNumber - 91];
							break;
						default:
							item = null;
							break;
					}
					if(item != null && !item.IsAir)
					{
						if(item.TryGetGlobalItem(out EnchantedItem iGlobal))
						{
							item.G().enchantments[enchantmentSlotNumber] = new Item(itemType);
							AllForOneEnchantmentBasic enchantment = (AllForOneEnchantmentBasic)item.G().enchantments[enchantmentSlotNumber].ModItem;
							item.UpdateEnchantment(Main.player[whoAmI], ref enchantment, enchantmentSlotNumber);
						}
						else
						{
							Item enchantmentItem = new Item(itemType);
							($"unable to update enchantment from packet: {enchantmentItem.S()} on item: {item.S()} due to failing to get GlobalItem EnchantedItem.  player whoAmI: {whoAmI} player: {Main.player[whoAmI].S()} enchantmentSlotNumber: {enchantmentSlotNumber} slotNumber: {slotNumber} bank: {bank} \n\t\tPlease notify andro951").Log();
						}
					}
					else
					{
						Item enchantmentItem = new Item(itemType);
						($"unable to update enchantment from packet: {enchantmentItem.S()} on item: {item.S()} due to item being null or air.  player whoAmI: {whoAmI} player: {Main.player[whoAmI].S()} enchantmentSlotNumber: {enchantmentSlotNumber} slotNumber: {slotNumber} bank: {bank} \n\t\tPlease notify andro951").Log();
					}
					/*int itemWhoAmI = reader.ReadInt32();
					byte i = reader.ReadByte();
					short enchantmentType = reader.ReadInt16();
					Main.item[itemWhoAmI].G().enchantments[i] = new Item(enchantmentType);*/
					break;
				case PacketIDs.OnHitEffects:
					int npcWhoAmI = reader.ReadInt32();
					int damage = reader.ReadInt32();
					bool crit = reader.ReadBoolean();
					if(UtilityMethods.debugging) ($"\\/OnHitEffects Packet: npc: {Main.npc[npcWhoAmI]} life: {Main.npc[npcWhoAmI].life}").Log();
					for (int i = 0; i < OnHitEffectID.Count; i++)
                    {
						bool applyEffect = reader.ReadBoolean();
                        if (applyEffect)
                        {
                            switch (i)
                            {
								case OnHitEffectID.GodSlayer:
									int godSlayerDamage = reader.ReadInt32();
									WEGlobalNPC.StrikeNPC(npcWhoAmI, godSlayerDamage, crit);
									break;
								case OnHitEffectID.OneForAll:
									int oneForAllWhoAmIsCount = reader.ReadInt32();
									for (int j = 0; j < oneForAllWhoAmIsCount; j++)
                                    {
										int oneForAllWhoAmI = reader.ReadInt32();
										int oneForAllDamages = reader.ReadInt32();
										WEGlobalNPC.StrikeNPC(oneForAllWhoAmI, oneForAllDamages, crit);
									}
									break;
								case OnHitEffectID.Amaterasu:
									float amaterasuItemStrength = reader.ReadSingle();
									if (Main.npc[npcWhoAmI].G().amaterasuStrength == 0f)
										Main.npc[npcWhoAmI].G().amaterasuStrength = amaterasuItemStrength;
									Main.npc[npcWhoAmI].G().amaterasuDamage += damage * (crit ? 2 : 1);
									/*int debuffsCount = reader.ReadInt32();
									;
									for (int j = 0; j < debuffsCount; j++)
                                    {
										int debuff = reader.ReadInt32();
										int time = reader.ReadInt32();
										if(debuff == ModContent.BuffType<AmaterasuDebuff>())
                                        {
											float amaterasuItemStrength = reader.ReadSingle();
											if (Main.npc[npcWhoAmI].G().amaterasuStrength == 0f)
												Main.npc[npcWhoAmI].G().amaterasuStrength = amaterasuItemStrength;
										}
										Main.npc[npcWhoAmI].AddBuff(debuff, time);
									}*/
									break;
							}
                        }
					}
					if (UtilityMethods.debugging) ($"/\\OnHitEffects Packet: npc: {Main.npc[npcWhoAmI]} life: {Main.npc[npcWhoAmI].life}").Log();
					break;
				default:
					ModContent.GetInstance<WEMod>().Logger.Debug("*NOT RECOGNIZED*\ncase: " + type + "\n*NOT RECOGNIZED*");
					break;
			}
			if(UtilityMethods.debugging) ($"/\\HandlePacket(reader, " + whoAmI + ": " + Main.player[whoAmI].name + ") type: " + type).Log();
		}
		private void ReadItem(Item item, BinaryReader reader)
        {
            if (IsEnchantable(item))
            {
				EnchantedItem iGlobal = item.G();
				iGlobal.experience = reader.ReadInt32();
				iGlobal.powerBoosterInstalled = reader.ReadBoolean();
				for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
				{
					iGlobal.enchantments[i] = new Item(reader.ReadUInt16());
				}
				iGlobal.eStats.Clear();
				int count = reader.ReadUInt16();
				for (int i = 0; i < count; i++)
				{
					string key = reader.ReadString();
					float additive = reader.ReadSingle();
					float multiplicative = reader.ReadSingle();
					float flat = reader.ReadSingle();
					float @base = reader.ReadSingle();
					iGlobal.eStats.Add(key, new StatModifier(additive, multiplicative, flat, @base));
				}
				iGlobal.statModifiers.Clear();
				count = reader.ReadUInt16();
				for (int i = 0; i < count; i++)
				{
					string key = reader.ReadString();
					float additive = reader.ReadSingle();
					float multiplicative = reader.ReadSingle();
					float flat = reader.ReadSingle();
					float @base = reader.ReadSingle();
					iGlobal.statModifiers.Add(key, new StatModifier(additive, multiplicative, flat, @base));
				} 
			}
		}
		private void WriteItem(Item item, ModPacket packet)
        {
			EnchantedItem iGlobal = item.G();
			packet.Write(iGlobal.experience);
			packet.Write(iGlobal.powerBoosterInstalled);
			for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
			{
				packet.Write((short)iGlobal.enchantments[i].type);
			}
			short count = (short)iGlobal.eStats.Count;
			packet.Write(count);
			foreach (string key in iGlobal.eStats.Keys)
			{
				packet.Write(key);
				packet.Write(iGlobal.eStats[key].Additive);
				packet.Write(iGlobal.eStats[key].Multiplicative);
				packet.Write(iGlobal.eStats[key].Base);
				packet.Write(iGlobal.eStats[key].Flat);
			}
			count = (short)iGlobal.statModifiers.Count;
			packet.Write(count);
			foreach (string key in iGlobal.statModifiers.Keys)
			{
				packet.Write(key);
				packet.Write(iGlobal.statModifiers[key].Additive);
				packet.Write(iGlobal.statModifiers[key].Multiplicative);
				packet.Write(iGlobal.statModifiers[key].Flat);
				packet.Write(iGlobal.statModifiers[key].Base);
			}
		}
		public override void AddRecipeGroups()
		{
			RecipeGroup group = new RecipeGroup(() => "Any Common Gem", new int[]
			{
				180, 181, 178, 179, 177
			});
			RecipeGroup.RegisterGroup("WeaponEnchantments:CommonGems", group);
			group = new RecipeGroup(() => "Any Rare Gem", new int[]
			{
				999, 182
			});
			RecipeGroup.RegisterGroup("WeaponEnchantments:RareGems", group);
			group = new RecipeGroup(() => "Workbenches", new int[]
			{
				ItemID.WorkBench, ItemID.BambooWorkbench, ItemID.BlueDungeonWorkBench, ItemID.BoneWorkBench, ItemID.BorealWoodWorkBench, ItemID.CactusWorkBench, ItemID.CrystalWorkbench, ItemID.DynastyWorkBench, ItemID.EbonwoodWorkBench, ItemID.FleshWorkBench, ItemID.FrozenWorkBench, ItemID.GlassWorkBench, ItemID.GoldenWorkbench, ItemID.GothicWorkBench, ItemID.GraniteWorkBench, ItemID.GreenDungeonWorkBench, ItemID.HoneyWorkBench, ItemID.LesionWorkbench, ItemID.LihzahrdWorkBench, ItemID.LivingWoodWorkBench, ItemID.MarbleWorkBench, ItemID.MartianWorkBench, ItemID.MeteoriteWorkBench, ItemID.MushroomWorkBench, ItemID.NebulaWorkbench, ItemID.ObsidianWorkBench, ItemID.PalmWoodWorkBench, ItemID.PearlwoodWorkBench, ItemID.PinkDungeonWorkBench, ItemID.PumpkinWorkBench, ItemID.RichMahoganyWorkBench, ItemID.SandstoneWorkbench, ItemID.ShadewoodWorkBench, ItemID.SkywareWorkbench, ItemID.SlimeWorkBench, ItemID.SolarWorkbench, ItemID.SpiderWorkbench, ItemID.SpookyWorkBench, ItemID.StardustWorkbench, ItemID.SteampunkWorkBench, ItemID.VortexWorkbench
			}) ;
			RecipeGroup.RegisterGroup("WeaponEnchantments:Workbenches", group);
		}
        private delegate Item orig_ItemIOLoad(TagCompound tag);
		private delegate Item hook_ItemIOLoad(orig_ItemIOLoad orig, TagCompound tag);
		private static readonly MethodInfo ModLoaderIOItemIOLoadMethodInfo = typeof(Main).Assembly.GetType("Terraria.ModLoader.IO.ItemIO")!.GetMethod("Load", BindingFlags.Public | BindingFlags.Static, new System.Type[] { typeof(TagCompound) })!;
		public override void Load()
        {
			HookEndpointManager.Add<hook_ItemIOLoad>(ModLoaderIOItemIOLoadMethodInfo, ItemIOLoadDetour);
		}
		public override void Unload()
		{
			return;
		}
		private Item ItemIOLoadDetour(orig_ItemIOLoad orig, TagCompound tag)
        {
			Item item = orig(tag);
			if(item.ModItem is UnloadedItem)
            {
				OldItemManager.ReplaceOldItem(ref item);
			}
			return item;
        }


		/*private delegate void orig_ItemIOLoad(Item item, TagCompound tag);
		private delegate void hook_ItemIOLoad(orig_ItemIOLoad orig, Item item, TagCompound tag);
		private static readonly MethodInfo ModLoaderIOItemIOLoadMethodInfo = 
			typeof(Main).Assembly.GetType("Terraria.ModLoader.IO.ItemIO")!.GetMethod("Load", BindingFlags.Public | BindingFlags.Static, new System.Type[] { typeof(Item), typeof(TagCompound) })!;

		public override void Load()
        {
			HookEndpointManager.Add<hook_ItemIOLoad>(ModLoaderIOItemIOLoadMethodInfo, ItemIOLoadDetour);
        }
		public override void Unload()
		{
			return;
		}
		private void ItemIOLoadDetour(orig_ItemIOLoad orig, Item item, TagCompound tag)
        {
			orig(item, tag);
			if(item.ModItem is UnloadedItem)
            {
				OldItemManager.ReplaceOldItem(ref item);
				//orig(item, tag);
			 }
        }*/

		/*private delegate void orig_ItemIOLoad(Item item, TagCompound tag);
		private delegate void hook_ItemIOLoad(orig_ItemIOLoad orig, Item item, TagCompound tag);
		private static readonly MethodInfo ModLoaderIOItemIOLoadMethodInfo = 
			typeof(Main).Assembly.GetType("Terraria.ModLoader.Default.UnloadedGlobalItem")!.GetMethod("LoadData", BindingFlags.Public, new System.Type[] { typeof(Item), typeof(TagCompound) })!;
		public override void Load()
        {
			HookEndpointManager.Add<hook_ItemIOLoad>(ModLoaderIOItemIOLoadMethodInfo, ItemIOLoadDetour);
        }
		public override void Unload()
		{
			return;
		}
		private void ItemIOLoadDetour(orig_ItemIOLoad orig, Item item, TagCompound tag)
        {
			orig(item, tag);
			if(item.ModItem is UnloadedItem)
            {
				OldItemManager.ReplaceOldItem(ref item);
				//orig(item, tag);
			}
        }*/



		/*OldItemManager.ReplaceOldItem(ref item);
            if(item.Name == "Unloaded Item")
                base.LoadData(item, tag);*/
	}
}
