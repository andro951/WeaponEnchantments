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
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using WeaponEnchantments.UI;
using System.Runtime.CompilerServices;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments
{
    public class WEMod : Mod
    {
		internal static ServerConfig serverConfig = ModContent.GetInstance<ServerConfig>();
		internal static ClientConfig clientConfig = ModContent.GetInstance<ClientConfig>();
		public static bool calamity = false;
		public static List<Item> consumedItems = new List<Item>();
		internal static bool IsEnchantable(Item item)
        {
			if (IsWeaponItem(item) || IsArmorItem(item) || IsAccessoryItem(item))
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
			if(item.ModItem is Enchantment)
            {
                if (utility)
                {
                    if (((Enchantment)item.ModItem).Utility)
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
			if (item.ModItem is EnchantmentEssence)
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
			/*public const byte TeleportItemSetting = 4;
			public const byte PickUpEssence = 5;*/
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
			if(LogMethods.debugging) ($"\\/HandlePacket(reader, " + whoAmI + ": " + Main.player[whoAmI].name + ") type: " + type).Log();
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
							item.GetEnchantedItem().enchantments[enchantmentSlotNumber] = new Item(itemType);
							Enchantment enchantment = (Enchantment)item.GetEnchantedItem().enchantments[enchantmentSlotNumber].ModItem;
							item.UpdateEnchantment(ref enchantment, enchantmentSlotNumber);
						}
						else
						{
							Item enchantmentItem = new Item(itemType);
							string errorMessage = $"unable to update enchantment from packet: {enchantmentItem.S()} on item: {item.S()} due to failing to get GlobalItem EnchantedItem.  player whoAmI: {whoAmI} player: {Main.player[whoAmI].S()} enchantmentSlotNumber: {enchantmentSlotNumber} slotNumber: {slotNumber} bank: {bank} \n\t\tPlease note exactly what you were doing when this occured and notify andro951(Weapon Enchantments)";
							errorMessage.Log();
							Main.NewText(errorMessage);
						}
					}
					else
					{
						Item enchantmentItem = new Item(itemType);
						string errorMessage = $"unable to update enchantment from packet: {enchantmentItem.S()} on item: {item.S()} due to item being null or air.  player whoAmI: {whoAmI} player: {Main.player[whoAmI].S()} enchantmentSlotNumber: {enchantmentSlotNumber} slotNumber: {slotNumber} bank: {bank} \n\t\tPlease notify andro951(Weapon Enchantments)";
						errorMessage.Log();
						Main.NewText(errorMessage);
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
					if(LogMethods.debugging) ($"\\/OnHitEffects Packet: npc: {Main.npc[npcWhoAmI]} life: {Main.npc[npcWhoAmI].life}").Log();
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
									if (Main.npc[npcWhoAmI].GetWEGlobalNPC().amaterasuStrength == 0f)
										Main.npc[npcWhoAmI].GetWEGlobalNPC().amaterasuStrength = amaterasuItemStrength;
									Main.npc[npcWhoAmI].GetWEGlobalNPC().amaterasuDamage += damage * (crit ? 2 : 1);
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
					if (LogMethods.debugging) ($"/\\OnHitEffects Packet: npc: {Main.npc[npcWhoAmI]} life: {Main.npc[npcWhoAmI].life}").Log();
					break;
				/*case PacketIDs.TeleportItemSetting:
					string name = reader.ReadString();
					bool teleportItemSetting = reader.ReadBoolean();
					if (playerTeleportItemSetting.ContainsKey(name))
						playerTeleportItemSetting[name] = teleportItemSetting;
					else
						playerTeleportItemSetting.Add(name, teleportItemSetting);
					break;
				case PacketIDs.PickUpEssence:
					int rarity = (int)reader.ReadByte();
					int stack = reader.ReadInt32();
					//Main.player[whoAmI].G().PickUpEssence(rarity, stack);
					break;*/
				default:
					ModContent.GetInstance<WEMod>().Logger.Debug("*NOT RECOGNIZED*\ncase: " + type + "\n*NOT RECOGNIZED*");
					break;
			}
			if(LogMethods.debugging) ($"/\\HandlePacket(reader, " + whoAmI + ": " + Main.player[whoAmI].name + ") type: " + type).Log();
		}
		private void ReadItem(Item item, BinaryReader reader)
        {
            if (IsEnchantable(item))
            {
				EnchantedItem iGlobal = item.GetEnchantedItem();
				iGlobal.Experience = reader.ReadInt32();
				iGlobal.PowerBoosterInstalled = reader.ReadBoolean();
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
			EnchantedItem iGlobal = item.GetEnchantedItem();
			packet.Write(iGlobal.Experience);
			packet.Write(iGlobal.PowerBoosterInstalled);
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

		
		private delegate Item orig_ItemIOLoad(TagCompound tag);
		private delegate Item hook_ItemIOLoad(orig_ItemIOLoad orig, TagCompound tag);
		private static readonly MethodInfo ModLoaderIOItemIOLoadMethodInfo = typeof(Main).Assembly.GetType("Terraria.ModLoader.IO.ItemIO")!.GetMethod("Load", BindingFlags.Public | BindingFlags.Static, new System.Type[] { typeof(TagCompound) })!;
		public override void Load()
        {
			HookEndpointManager.Add<hook_ItemIOLoad>(ModLoaderIOItemIOLoadMethodInfo, ItemIOLoadDetour);
			IL.Terraria.Recipe.FindRecipes += HookFindRecipes;
			IL.Terraria.Recipe.Create += HookCreate;
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

		public static int counter = 0;
		private const bool debuggingHookFindRecipes = false;
		private const bool debuggingHookCreate = false;
		private static void HookCreate(ILContext il) {
			counter = 0;
			var c = new ILCursor(il);

			if (debuggingHookCreate) {
				while (c.Next != null) {
					bool catchingExceptions = true;
					ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString());
					while (catchingExceptions) {
						c.Index++;
						try {
							if (c.Next != null) {
								string tempString = c.Next.ToString();
							}
							catchingExceptions = false;
						}
						catch (Exception e) {
							ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString().Substring(0, 20));
						}
					}
				}
				c.Index = 0;



				bool searching = true;
				int line = 0;
				int j = 0;
				int jNext = c.Context.ToString().Substring(0).IndexOf("IL_");
				while (searching) {
					j = jNext;
					//Debug.WriteLine("length: " + c.Context.ToString().Length.ToString() + " jNext + 1: " + (jNext + 1).ToString());
					jNext = c.Context.ToString().Substring(jNext + 1).IndexOf("IL_") + j + 1;
					//Debug.WriteLine("substring: " + c.Context.ToString().Substring(j, jNext - j - 2) + ", length: " + c.Context.ToString().Substring(j, jNext - j - 2).Length.ToString());
					if (jNext == j) {
						ModContent.GetInstance<WEMod>().Logger.Info(line + " " + c.Context.ToString().Substring(j));
						//Debug.WriteLine(line + " " + c.Context.ToString().Substring(j));
						searching = false;
					}
					else {
						ModContent.GetInstance<WEMod>().Logger.Info(line + " " + c.Context.ToString().Substring(j, jNext - j - 2));
						//Debug.WriteLine(line + " " + c.Context.ToString().Substring(j, jNext - j - 2));
					}
					line++;
				}
			}

			if (!c.TryGotoNext(MoveType.Before,
				i => i.MatchLdsfld(out _),
				i => i.MatchLdsfld(out _),
				i => i.MatchLdelemRef(),
				i => i.MatchLdfld(out _),
				i => i.MatchLdcI4(-1),
				i => i.MatchBeq(out _)

			)) { throw new Exception("Failed to find instructions HookCreate 3"); }

			var incomingLabels = c.IncomingLabels;
			foreach (ILLabel cursorIncomingLabel in incomingLabels)
				c.MarkLabel(cursorIncomingLabel);

			c.Emit(OpCodes.Ldloc, 3);
			c.Emit(OpCodes.Ldloc, 4);

			c.EmitDelegate((Item item, int num) => {
				WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
				if (wePlayer.usingEnchantingTable) {
					for (int i = 0; i < EnchantingTable.maxEssenceItems; i++) {
						Item slotItem = wePlayer.enchantingTableUI.essenceSlotUI[i].Item;
						if (item.type == slotItem.type) {
							slotItem.stack -= num;
						}
					}
				}
			});
		}
		private static void HookFindRecipes(ILContext il)
		{
			var c = new ILCursor(il);

			if (debuggingHookFindRecipes)
			{
				while (c.Next != null)
				{
					bool catchingExceptions = true;
					ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString());
					while (catchingExceptions)
					{
						c.Index++;
						try
						{
							if (c.Next != null)
							{
								string tempString = c.Next.ToString();
							}
							catchingExceptions = false;
						}
						catch (Exception e)
						{
							ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString().Substring(0, 20));
						}
					}
				}
				c.Index = 0;



				bool searching = true;
				int line = 0;
				int j = 0;
				int jNext = c.Context.ToString().Substring(0).IndexOf("IL_");
				while (searching)
				{
					j = jNext;
					//Debug.WriteLine("length: " + c.Context.ToString().Length.ToString() + " jNext + 1: " + (jNext + 1).ToString());
					jNext = c.Context.ToString().Substring(jNext + 1).IndexOf("IL_") + j + 1;
					//Debug.WriteLine("substring: " + c.Context.ToString().Substring(j, jNext - j - 2) + ", length: " + c.Context.ToString().Substring(j, jNext - j - 2).Length.ToString());
					if (jNext == j)
					{
						ModContent.GetInstance<WEMod>().Logger.Info(line + " " + c.Context.ToString().Substring(j));
						//Debug.WriteLine(line + " " + c.Context.ToString().Substring(j));
						searching = false;
					}
					else
					{
						ModContent.GetInstance<WEMod>().Logger.Info(line + " " + c.Context.ToString().Substring(j, jNext - j - 2));
						//Debug.WriteLine(line + " " + c.Context.ToString().Substring(j, jNext - j - 2));
					}
					line++;
				}
			}

			if (!c.TryGotoNext(MoveType.After,
				i => i.MatchLdloc(12),
				i => i.MatchLdcI4(1),
				i => i.MatchAdd(),
				i => i.MatchStloc(12),
				i => i.MatchLdloc(12),
				i => i.MatchLdcI4(40),
				i => i.MatchBlt(out _)
			)) { throw new Exception("Failed to find instructions HookFindRecipes"); }

			if (debuggingHookFindRecipes) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

			var incomingLabels = c.IncomingLabels;
			foreach (ILLabel cursorIncomingLabel in incomingLabels)
				c.MarkLabel(cursorIncomingLabel);

			if (debuggingHookFindRecipes) try { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " Instruction: " + c.Next.ToString()); } catch (Exception e) { ModContent.GetInstance<WEMod>().Logger.Info("c.Index: " + c.Index.ToString() + " exception: " + e.ToString()); }

			c.Emit(OpCodes.Ldloc, 6);
			c.EmitDelegate((Dictionary<int, int> dictionary) =>
			{
				WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
				if (wePlayer.usingEnchantingTable)
				{
					if (debuggingHookFindRecipes)
					{
						counter++;
						Main.NewText("counter: " + counter.ToString());
						ModContent.GetInstance<WEMod>().Logger.Info("counter: " + counter.ToString());
					}
					for (int i = 0; i < EnchantingTable.maxEssenceItems; i++)
					{
						Item item = wePlayer.enchantingTableUI.essenceSlotUI[i].Item;
						if (item != null && item.stack > 0)
						{
							if (dictionary.ContainsKey(item.netID))
								dictionary[item.netID] += item.stack;
							else
								dictionary[item.netID] = item.stack;
						}
					}
				}
			});
		}
	}
}
