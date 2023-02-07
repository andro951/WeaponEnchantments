using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using static WeaponEnchantments.Common.Utility.LogModSystem;

namespace WeaponEnchantments.Common
{
    public static class InfusionManager
    {
        public const int numVanillaWeaponRarities = 11;
        public const int numRarities = 18;
        public static float[] averageValues = new float[numRarities];
        public static int[] minValues = new int[numRarities];
        public static int[] maxValues = new int[numRarities];
        public static int[] calamityAverageValues = new int[numRarities];
        public static int[] calamityMinValues = new int[numRarities];
		//                                                 0     1      2      3      4       5       6       7       8       9       10       11       12       13       14       15       16       17
		public static int[] calamityMaxValues = new int[] { 23000, 52000, 87000, 128000, 175000, 240000, 360000, 480000, 600000, 800000, 1000000, 1100000, 1200000, 1300000, 1400000, 1500000, 2000000, 2500000 };
		//public static int[] calamityMaxValues = new int[] {5000, 10000, 20000, 40000, 120000, 240000, 360000, 480000, 600000, 800000, 1000000, 1100000, 1200000, 1300000, 1400000, 1500000, 2000000, 2500000};
		public const float minMaxValueMultiplier = 0.25f;

        public static void SetUpVanillaWeaponInfusionPowers() {
            Dictionary<string, List<int[]>> weaponsDict = GetItemDict(GetItemDictModeID.Weapon);
            int[] total = new int[numRarities];
            int[] count = new int[numRarities];
            
            //For each vanilla item
            foreach (int[] stats in weaponsDict["Terraria"]) {
                int rarity = stats[0];
                rarity.Clamp(0, numRarities - 1);

                int value = stats[1];
                total[rarity] += value;
                count[rarity]++;

                //Min value
                if(minValues[rarity] > value || minValues[rarity] == 0)
                    minValues[rarity] = value;

                //Max value
                if(maxValues[rarity] < value)
                    maxValues[rarity] = value;
            }

            //Rarity 0-10 averages
            for (int i = 0; i < numRarities; i++) {
                if (i < numVanillaWeaponRarities)
                    averageValues[i] = (float)total[i] / (float)count[i];
            }

            //Rarities 11-17
            for (int i = numVanillaWeaponRarities; i < numRarities; i++) {
                if (i >= 16) {
                    maxValues[i] = 2000000 + 500000 * (i - 16);
                }
                else {
                    maxValues[i] = 1100000 + 100000 * (i - numVanillaWeaponRarities);
                }

                minValues[i] = maxValues[i - 1];
                averageValues[i] = (minValues[i] + maxValues[i]) / 2;
            }

            //Calamity min and max (used for all modded items)
            for (int i = 0; i < numRarities; i++) {
                if (i == 0) {
                    calamityMinValues[i] = 0;
                }
				else {
                    calamityMinValues[i] = calamityMaxValues[i - 1];
                }

                calamityAverageValues[i] = (calamityMinValues[i] + calamityMaxValues[i]) / 2;
			}



            //Print list of items
			if (PrintListOfItems[GetItemDictModeID.Weapon]) {
                GetItemDict(GetItemDictModeID.Weapon, postSetupPrintList: true);

                string msg = "\nRarity, Average, Min, Max";
                for (int i = 0; i < numRarities; i++) {
                    msg += $"\n{i}, {averageValues[i]}, {minValues[i]}, {maxValues[i]}";
                }

                msg.Log();
            }
        }
        private struct ItemDetails {
            public Item Item;
            public float Rarity;
            public float ValueRarity;
            public ItemDetails(Item item, float rarity, float valueRarity) {
                Item = item;
                Rarity = rarity;
                ValueRarity = valueRarity;
			}
		}
        private static Dictionary<string, List<int[]>> GetItemDict(byte mode, bool postSetupPrintList = false) {
            bool printList = PrintListOfItems[mode];

            Dictionary<string, List<int[]>> itemsDict = new Dictionary<string, List<int[]>>();
            SortedDictionary<int, SortedDictionary<string, ItemDetails>> infusionPowers = new SortedDictionary<int, SortedDictionary<string, ItemDetails>>();
            string msg = "";
            for (int itemType = 1; itemType < ItemLoader.ItemCount; itemType++) {
                Item item = ContentSamples.ItemsByType[itemType];
                if (item != null) {
                    string modName = item.ModItem != null ? item.ModItem.Mod.Name : "Terraria";
                    bool weaponList = mode == GetItemDictModeID.Weapon && EnchantedItemStaticMethods.IsWeaponItem(item);
                    bool armorList = mode == GetItemDictModeID.Armor && EnchantedItemStaticMethods.IsArmorItem(item);
                    bool accessory = mode == GetItemDictModeID.Accessory && EnchantedItemStaticMethods.IsAccessoryItem(item);
                    if ( weaponList || armorList || accessory) {
                        int[] itemStats = { item.rare, item.value, item.damage };
                        if (item.rare >= numRarities)
                            $"Item above max supported rarities detected: {item.S()}, rare: {item.rare}, value: {item.value}.  It will be treated as rarity {numRarities - 1} for Infusion.".LogSimple();

                        if (!itemsDict.ContainsKey(modName))
                            itemsDict.Add(modName, new List<int[]>());

                        itemsDict[modName].Add(itemStats);

						if (printList && postSetupPrintList) {
                            Item clone = item.Clone();
                            int infusionPower = GetWeaponInfusionPower(clone, out float rarity, out float valueRarity);
                            ItemDetails itemDetails = new ItemDetails(clone, rarity, valueRarity);
                            if (!infusionPowers.ContainsKey(infusionPower)) {
                                infusionPowers.Add(infusionPower, new SortedDictionary<string, ItemDetails>() { { clone.Name, itemDetails } });
                            }
							else {
                                if (infusionPowers[infusionPower].ContainsKey(clone.Name)) {
                                    ItemDetails currentItemDetails = infusionPowers[infusionPower][clone.Name];
                                    Item currentItem = currentItemDetails.Item;
									int currentInfusionPower = currentItem.GetWeaponInfusionPower();
									($"infusionPowers[{infusionPower}] already contains key({clone.Name}).\n" +
                                        $"Current = {GetDataString(currentInfusionPower, currentItem.Name, currentItemDetails)}\n" +
                                        $"New = {GetDataString(infusionPower, clone.Name, itemDetails)}").LogSimple();
                                }
                                else {
									infusionPowers[infusionPower].Add(clone.Name, itemDetails);
								}
							}
                        }
                    }
                }
            }

            if(printList && postSetupPrintList) {
                if (mode == GetItemDictModeID.Weapon) {
                    msg += "\nMod, Weapon, Infusion Power, Value Rarity, Rarity, Original Rarity, Value, Item ID, Damage, Use Time, DPS";
                    foreach (int infusionPower in infusionPowers.Keys) {
                        foreach(string name in infusionPowers[infusionPower].Keys) {
                            msg += $"\n{GetDataString(infusionPower, name, infusionPowers[infusionPower][name])}";
                        }
                    }
                }
                    //Print list of items
                    msg.Log();
            }
            
            return itemsDict;
        }
        private static string GetDataString(int infusionPower, string name, ItemDetails itemDetails) {
			Item item = itemDetails.Item;
			string mod = item.ModItem?.Mod.Name;
			if (mod == null)
				mod = "Terraria";

			int damage = item.damage;
			int useTime = item.useTime;
			float dps = (float)damage * 60f / (float)useTime;

			return $"{mod}, {name}, {infusionPower}, {itemDetails.ValueRarity}, {itemDetails.Rarity}, {item.rare}, {item.value}, {item.type}, {damage}, {useTime}, {dps}";
		}
        public static float GetWeaponRarity(this Item item) {
            return GetWeaponRarity(item, out float rarity, out float valueRarity);
        }
        public static float GetWeaponRarity(this Item item, out float rarity, out float valueRarity) {
            Item sampleItem = ContentSamples.ItemsByType[item.type];

            rarity = GetAdjustedItemRarity(sampleItem, out bool useCalamiryValuesOnly);

            valueRarity = GetValueRarity(sampleItem, rarity, useCalamiryValuesOnly);

            float combinedRarity = rarity + valueRarity;

            return combinedRarity > 0 ? combinedRarity : 0;
        }
        public static float GetAdjustedItemRarity(Item sampleItem, out bool useCalamiryValuesOnly) {
            string sampleItemModName = sampleItem.ModItem?.Mod.Name;
            switch (sampleItemModName) {
                case null:
                case "StarsAbove":
                    useCalamiryValuesOnly = false;
					break;
                default:
                    useCalamiryValuesOnly = true;
                    break;
            }

            float rarity = sampleItem.rare;
            int sampleValue = sampleItem.value;

            //If from calamity, calculate just from value

            //string modName = sampleItem.ModItem?.Mod.Name;
			//if (modName == "CalamityMod" || modName == "ThoriumMod")
            //    useCalamiryValuesOnly = true;

            switch (sampleItem.type) {
				#region Vanilla rarities
				case ItemID.Count://April Fools Joke
                    rarity = -0.9999f;
                    break;
                case ItemID.GravediggerShovel:
                    rarity = -0.3f;
                    break;
                case ItemID.FlamingMace:
                case ItemID.FlareGun:
                case ItemID.FlintlockPistol:
                case ItemID.Katana:
                case ItemID.BlandWhip:
                case ItemID.Mace:
                case ItemID.PurpleClubberfish:
                case ItemID.RedRyder:
                case ItemID.SnowballCannon:
                case ItemID.StaffofRegrowth:
                    rarity = 0f;
                    break;
                case ItemID.ThrowingKnife:
                case ItemID.Shuriken:
                case ItemID.StarAnise:
                case ItemID.PoisonedKnife:
                    rarity = 0.2f;
                    break;
                case ItemID.BallOHurt:
                case ItemID.CrimsonRod:
                case ItemID.Musket:
                case ItemID.TheRottedFork:
                case ItemID.TheUndertaker:
                case ItemID.Vilethorn:
                    rarity = 0.4f;
                    break;
                case ItemID.AbigailsFlower:
                case ItemID.Grenade:
                case ItemID.StickyGrenade:
                    rarity = 0.5f;
                    break;
                case ItemID.BouncyGrenade:
                    rarity = 0.7f;
                    break;
                case ItemID.JungleYoyo:
                case ItemID.BatBat:
                case ItemID.BladeofGrass:
                case ItemID.BladedGlove:
                case ItemID.BloodyMachete:
                case ItemID.BoneSword:
                case ItemID.Boomstick:
                case ItemID.TaxCollectorsStickOfDoom:
                case ItemID.CombatWrench:
                case ItemID.DyeTradersScimitar:
                case ItemID.FalconBlade:
                case ItemID.FlinxStaff:
                case ItemID.AntlionClaw:
                case ItemID.PainterPaintballGun:
                case ItemID.PartyGirlGrenade:
                case ItemID.Rally:
                case ItemID.ReaverShark:
                case ItemID.Revolver:
                case ItemID.Rockfish:
                case ItemID.Sandgun:
                case ItemID.SawtoothShark:
                case ItemID.SlimeStaff:
                case ItemID.StylistKilLaKillScissorsIWish:
                case ItemID.Swordfish:
                case ItemID.TentacleSpike:
                case ItemID.TragicUmbrella:
                    rarity = 0.8f;
                    break;
                case ItemID.Code1:
                case ItemID.DiamondStaff:
                case ItemID.ZapinatorGray:
                case ItemID.Javelin:
                case ItemID.ThornWhip:
                case ItemID.Starfury:
                case ItemID.ThornChakram:
                    rarity = 1f;
                    break;
                case ItemID.BloodWater:
                case ItemID.HolyWater:
                case ItemID.UnholyWater:
				case ItemID.TheMeatball:
					rarity = 1.2f;
                    break;
                case ItemID.Minishark:
                case ItemID.StarCannon:
                case ItemID.NightmarePickaxe:
                case ItemID.TheBreaker:
                case ItemID.DeathbringerPickaxe:
                case ItemID.FleshGrinder:
		            rarity = 1.4f;
		            break;
                case ItemID.BloodRainBow:
                case ItemID.VampireFrogStaff:
                    rarity = 1.5f;
                    break;
                case ItemID.BeeKeeper:
                case ItemID.Blowgun:
                case ItemID.Bone:
                case ItemID.DemonScythe:
                case ItemID.QuadBarrelShotgun:
                case ItemID.Ruler:
                case ItemID.SpikyBall:
                case ItemID.BeesKnees:
                case ItemID.Valor:
                case ItemID.DD2FlameburstTowerT1Popper:
                case ItemID.DD2BallistraTowerT1Popper:
                case ItemID.DD2ExplosiveTrapT1Popper:
                case ItemID.DD2LightningAuraT1Popper:
                    rarity = 2f;
                    break;
                case ItemID.Beenade:
                    rarity = 2.4f;
                    break;
                case ItemID.Cascade:
                case ItemID.HornetStaff:
                    rarity = 2.5f;
                    break;
                case ItemID.Terragrim:
                    rarity = 3f;
                    break;
				case ItemID.BouncingShield:
					rarity = 3.4f;
					break;
				case ItemID.FormatC:
                case ItemID.Gradient:
                    rarity = 3.5f;
                    break;
                case ItemID.ClockworkAssaultRifle:
                case ItemID.LaserRifle:
                case ItemID.BreakerBlade:
                case ItemID.FireWhip:
                    rarity = 3.6f;
                    break;
                case ItemID.Bananarang:
                case ItemID.DD2BallistraTowerT2Popper:
                case ItemID.Bladetongue:
                case ItemID.CrystalSerpent:
                case ItemID.DaoofPow:
                case ItemID.DD2ExplosiveTrapT2Popper:
                case ItemID.DD2FlameburstTowerT2Popper:
                case ItemID.FlowerofFrost:
                case ItemID.FrostStaff:
                case ItemID.Frostbrand:
                case ItemID.Gatligator:
                case ItemID.IceBow:
                case ItemID.IceRod:
                case ItemID.IceSickle:
                case ItemID.DD2LightningAuraT2Popper:
                case ItemID.MeteorStaff:
                case ItemID.NimbusRod:
                case ItemID.ObsidianSwordfish:
                case ItemID.ZapinatorOrange:
                case ItemID.PoisonStaff:
                case ItemID.Shotgun:
                case ItemID.SkyFracture:
                case ItemID.SlapHand:
                case ItemID.Toxikarp:
                case ItemID.Uzi:
                    rarity = 4f;
                    break;
                case ItemID.ChainGuillotines:
                case ItemID.ClingerStaff:
                case ItemID.CrystalVileShard:
                case ItemID.DaedalusStormbow:
                case ItemID.DartPistol:
                case ItemID.DartRifle:
                case ItemID.FetidBaghnakhs:
                case ItemID.FlyingKnife:
                case ItemID.SoulDrain:
                case ItemID.Marrow:
                    rarity = 4.1f;
                    break;
                case ItemID.SanguineStaff:
                    rarity = 4.2f;
                    break;
                case ItemID.ShadowFlameBow:
                case ItemID.ShadowFlameHexDoll:
                case ItemID.ShadowFlameKnife:
                    rarity = 4.5f;
                    break;
                case ItemID.Cannonball:
                case ItemID.Cutlass:
                    rarity = 4.8f;
                    break;
                case ItemID.CoinGun:
                case ItemID.Arkhalis:
                case ItemID.DeathSickle:
                case ItemID.Hammush:
                case ItemID.MushroomSpear:
                case ItemID.UnholyTrident:
                    rarity = 5f;
                    break;
                case ItemID.SuperStarCannon:
                case ItemID.PirateStaff:
                case ItemID.ValkyrieYoyo:
                    rarity = 5.4f;
                    break;
                case ItemID.Flamethrower:
                case ItemID.Megashark:
                case ItemID.LightDisc:
                case ItemID.Code2:
                case ItemID.Yelets:
                case ItemID.RedsYoyo:
                    rarity = 5.5f;
                    break;
                case ItemID.HallowedRepeater:
                case ItemID.SwordWhip:
                case ItemID.Excalibur:
                case ItemID.Gungnir:
                case ItemID.HallowJoustingLance:
                    rarity = 5.6f;
                    break;
                case ItemID.Smolstar:
                    rarity = 5.7f;
                    break;
                case ItemID.BookStaff:
                case ItemID.DD2PhoenixBow:
                case ItemID.DD2SquireDemonSword:
                case ItemID.MonkStaffT1:
                case ItemID.MonkStaffT2:
                    rarity = 6f;
                    break;
                case ItemID.TrueNightsEdge:
                    rarity = 6.05f;
                    break;
                case ItemID.OpticStaff:
                    rarity = 6.12f;
                    break;
                case ItemID.RainbowRod:
                case ItemID.MagicalHarp:
                    rarity = 6.2f;
                    break;
                case ItemID.VenomStaff:
                    rarity = 6.2f;
                    break;
                case ItemID.TrueExcalibur:
                    rarity = 6.25f;
                    break;
                case ItemID.Drax:
                case ItemID.PickaxeAxe:
                    rarity = 6.35f;
                    break;
                case ItemID.ChlorophyteChainsaw:
                case ItemID.ChlorophyteClaymore:
                case ItemID.ChlorophyteDrill:
                case ItemID.ChlorophyteGreataxe:
                case ItemID.ChlorophyteJackhammer:
                case ItemID.ChlorophytePickaxe:
                case ItemID.ChlorophyteSaber:
                case ItemID.ChlorophyteShotbow:
                case ItemID.ChlorophyteWarhammer:
                    rarity = 6.4f;
                    break;
                case ItemID.ChlorophytePartisan:
                    rarity = 6.6f;
                    break;
                case ItemID.Seedler:
                    rarity = 6.6f;
                    break;
                case ItemID.TacticalShotgun:
                case ItemID.TheAxe:
                case ItemID.WaspGun:
                case ItemID.GrenadeLauncher:
                case ItemID.PygmyStaff:
                case ItemID.DD2BallistraTowerT3Popper:
                case ItemID.DD2ExplosiveTrapT3Popper:
                case ItemID.DD2FlameburstTowerT3Popper:
                case ItemID.DD2LightningAuraT3Popper:
                case ItemID.InfernoFork:
                case ItemID.PrincessWeapon:
                case ItemID.PulseBow:
                case ItemID.ProximityMineLauncher:
                case ItemID.ShroomiteDiggingClaw:
                    rarity = 6.7f;
                    break;
                case ItemID.LeafBlower:
                case ItemID.FlowerPow:
                case ItemID.Kraken:
                    rarity = 6.8f;
                    break;
                case ItemID.VenusMagnum:
                case ItemID.NettleBurst:
                    rarity = 6.9f;
                    break;
                case ItemID.Keybrand:
                case ItemID.MagnetSphere:
                case ItemID.MaceWhip:
                case ItemID.PaladinsHammer:
                case ItemID.RocketLauncher:
                case ItemID.ShadowJoustingLance:
                case ItemID.ShadowbeamStaff:
                case ItemID.SniperRifle:
                case ItemID.SpectreHamaxe:
                case ItemID.SpectrePickaxe:
                case ItemID.SpectreStaff:
                case ItemID.StormTigerStaff:
                case ItemID.PiranhaGun:
                case ItemID.RainbowGun:
                case ItemID.ScourgeoftheCorruptor:
                case ItemID.StaffoftheFrostHydra:
                case ItemID.VampireKnives:
                    rarity = 7f;
                    break;
                case ItemID.ButchersChainsaw:
                case ItemID.DeadlySphereStaff:
                case ItemID.NailGun:
                case ItemID.PsychoKnife:
                case ItemID.ToxicFlask:
                    rarity = 7.2f;
                    break;
                case ItemID.TerraBlade:
                    rarity = WEMod.calamityEnabled ? 6.25f : 7.1f;
                    break;
                case ItemID.TheEyeOfCthulhu:
                    rarity = 7.1f;
                    break;
                case ItemID.BatScepter:
                case ItemID.BlizzardStaff:
                case ItemID.CandyCornRifle:
                case ItemID.ChainGun:
                case ItemID.ChristmasTreeSword:
                case ItemID.ScytheWhip:
                case ItemID.EldMelter:
                case ItemID.JackOLanternLauncher:
                case ItemID.NorthPole:
                case ItemID.RavenStaff:
                case ItemID.Razorpine:
                case ItemID.SnowmanCannon:
                case ItemID.StakeLauncher:
                case ItemID.TheHorsemansBlade:
                    rarity = 7.5f;
                    break;
                case ItemID.GolemFist:
                case ItemID.HeatRay:
                case ItemID.PossessedHatchet:
                case ItemID.StaffofEarth:
                case ItemID.Stynger:
                case ItemID.FireworksLauncher:
                    rarity = 7.9f;
                    break;
                case ItemID.ChargedBlasterCannon:
                case ItemID.LaserDrill:
                    rarity = 8.1f;
                    break;
                case ItemID.ElectrosphereLauncher:
                case ItemID.InfluxWaver:
                case ItemID.LaserMachinegun:
                case ItemID.XenoStaff:
                case ItemID.Xenopopper:
                    rarity = 8.3f;
                    break;
                case ItemID.Picksaw:
                    rarity = 8.4f;
                    break;
                case ItemID.SparkleGuitar:
                case ItemID.EmpressBlade:
                    rarity = 8.7f;
                    break;
                case ItemID.FairyQueenRangedItem:
                case ItemID.RainbowWhip:
                case ItemID.PiercingStarlight:
                case ItemID.FairyQueenMagicItem:
                    rarity = 9f;
                    break;
                case ItemID.BubbleGun:
                case ItemID.Flairon:
                case ItemID.RazorbladeTyphoon:
                case ItemID.TempestStaff:
                case ItemID.Tsunami:
                    rarity = 9.3f;
                    break;
                case ItemID.StardustCellStaff:
                case ItemID.StardustDragonStaff:
                case ItemID.VortexBeater:
                case ItemID.Phantasm:
                case ItemID.NebulaArcanum:
                case ItemID.NebulaBlaze:
                case ItemID.SolarEruption:
                case ItemID.DayBreak:
                    rarity = 9.5f;
                    break;
				case ItemID.LunarHamaxeStardust:
                case ItemID.LunarHamaxeNebula:
                case ItemID.LunarHamaxeSolar:
                case ItemID.LunarHamaxeVortex:
                    rarity = 10.2f;
                    break;
				case ItemID.StarWrath:
                    rarity = 10f;
                    break;
				#endregion
				case > ItemID.Count:
                    //Manually set rarity of an item
                    switch (sampleItem.Name) {
                        case "Depth Crusher"://Calamity
                            rarity = -0.4f;
                            break;
                        case "Feller of Evergreens"://Calamity
                            rarity = -0.3f;
                            break;
						case "Squirrel Squire Staff"://Calamity
                        case "Frost Bolt"://Calamity
						case "Spadefish"://Calamity
							rarity = -0.2f;
                            break;
                        case "Stormjaw Staff"://Calamity
                            rarity = -0.05f;
                            break;
						case "Primary Zenith"://Primary Zenith
                            rarity = 0f;
                            break;
                        case "Sparkling Empress"://Calamity
                            rarity = 0.05f;
                            break;
                        case "Storm Surge"://Calamity
                        case "Seashine Sword"://Calamity
                        case "Riptide"://Calamity
						case "Aquamarine Staff"://Calamity
                        case "Nasty Cholla"://Calamity
                        case "Throwing Brick"://Calamity
                        case "Monstrous Knives"://Calamity
                        case "Seafoam Bomb"://Calamity
							rarity = 0.1f;
                            break;
                        case "Web Ball"://Calamity
                        case "Icicle Staff"://Calamity
                        case "Spark Spreader"://Calamity
						case "Sun Spirit Staff"://Calamity
							rarity = 0.2f;
                            break;
						case "Frost Blossom Staff"://Calamity
						case "Mana Rose"://Calamity
							rarity = 0.25f;
							break;
						case "Lead Tomahawk"://Calamity
                        case "Iron Francisca"://Calamity
						case "Harvest Staff"://Calamity
							rarity = 0.3f;
                            break;
						case "Crackshot Colt"://Calamity
						case "Gilded Dagger"://Calamity
						case "Gleaming Dagger"://Calamity
						case "Marnite Deconstructor"://Calamity
						case "Marnite Obliterator"://Calamity
							rarity = 0.35f;
                            break;
						case "Wulfrum Knife"://Calamity
                        case "Urchin Stinger"://Calamity
							rarity = 0.4f;
                            break;
                        case "Opal Striker"://Calamity
							rarity = 0.45f;
							break;
                        case "Firestorm Cannon"://Calamity
                        case "Magna Cannon"://Calamity
                        case "Meteor Fist"://Calamity
							rarity = 0.5f;
							break;
                        case "Pumpler"://Calamity
                            rarity = 0.7f;
                            break;
						case "Aegis Driver":
                            rarity = 0.8f;
                            break;
						case "Carian Dark Moon"://Stars Above
                        case "Konpaku Katana"://stars Above
							rarity = 1f;
                            break;
						case "Polyp Launcher"://Calamity
                        case "Amidias' Trident"://Calamity
						case "Magical Conch"://Calamity
						case "Urchin Flail"://Calamity
						case "Coral Cannon"://Calamity
						case "Waywasher"://Calamity
						case "Snap Clam"://Calamity
						case "Shellshooter"://Calamity
						case "Sand Dollar"://Calamity
							rarity = 1.05f;
                            break;
						case "Takonomicon"://Stars Above
						case "Ashen Ambition"://Stars Above
						case "Neo Dealmaker"://Stars Above
							rarity = 1.15f;
							break;
						case "Rad Gun"://Stars Above
                        case "Star Struck Water"://Calamity
							rarity = 1.2f;
                            break;
						case "Teardrop Cleaver"://Calamity
						case "Deathstare Rod"://Calamity
                        case "Bouncing Eyeball"://Calamity
						case "Caustic Croaker Staff"://Calamity
						case "Acid Gun"://Calamity
						case "Parasitic Scepter"://Calamity
						case "Toxibow"://Calamity
						case "Contaminated Bile"://Calamity
						case "Rusty Beacon Prototype"://Calamity
						case "Basher"://Calamity
							rarity = 1.25f;
                            break;
						case "Magnum"://Calamity
                        case "Sky Glaze"://Calamity
                        case "Ashen Stalactite"://Calamity
						case "Crystalline"://Calamity
							rarity = 1.3f;
                            break;
                            rarity = 1.35f;
                            break;
						case "Burnt Sienna"://Calamity
                            rarity = 1.4f;
                            break;
						case "Mycoroot"://Calamity
                        case "Puff Shroom"://Calamity
                        case "Hyphae Rod"://Calamity
						case "Fungicide"://Calamity
						case "Infested Clawmerang"://Calamity
						case "Mycelial Claws"://Calamity
							rarity = 1.5f;
                            break;
                        case "Belladonna Spirit Staff"://Calamity
                        case "Yateveo Bloom"://Calamity
							rarity = 1.55f;
                            break;
						case "Sludge Splotch"://Calamity
                            rarity = 1.6f;
                            break;
						case "Scab Ripper"://Calamity
                        case "Caustic Edge"://Calamity
						case "Vile Feeder"://Calamity
							rarity = 1.65f;
                            break;
						case "Carnage Ray"://Calamity
                        case "Night's Ray"://Calamity
                        case "The God's Gambit"://Calamity
							rarity = 1.8f;
                            break;
                        case "Overloaded Blaster"://Calamity
                        case "Corroslime Staff"://Calamity
                        case "Crimslime Staff"://Calamity
						case "Eldritch Tome"://Calamity
						case "Abyssal Tome"://Calamity
							rarity = 1.85f;
                            break;
						case "Bouncy Spiky Ball"://Calamity
                        case "Poison Pack"://Calamity
                        case "Sticky Spiky Ball"://Calamity
							rarity = 2.02f;
                            break;
						case "Plasma Rod"://Calamity
							rarity = 2.1f;
                            break;
                        case "Rot Ball"://Calamity
						case "Tooth Ball"://Calamity
						case "Gelpick"://Calamity
						case "Gelitic Blade"://Calamity
                        case "Goobow"://Calamity
                        case "Gunk Shot"://Calamity
                        case "Slime Puppet Staff"://Calamity
							rarity = 2.15f;
                            break;
                        case "Lunarian Bow"://Calamity
                            rarity = 2.2f;
                            break;
						case "Shadethrower"://Calamity
                        case "Perfect Dark"://Calamity
                        case "Shaderain Staff"://Calamity
                        case "Dank Staff"://Calamity
                        case "Vein Burster"://Calamity
						case "Blood Clot Staff"://Calamity
						case "Sausage Maker"://Calamity
						case "Eviscerator"://Calamity
						case "Blood Bath"://Calamity
						case "Aorta"://Calamity
							rarity = 2.25f;
                            break;
                        case "Feather Knife"://Calamity
                            rarity = 2.3f;
                            break;
						case "Pod Zero-42"://Stars Above
                        case "Persephone"://Stars Above
                        case "Aerial Hamaxe"://Calamity
                        case "Skyfringe Pickaxe"://Calamity
						case "Gauss Dagger"://Calamity
						case "Pulse Pistol"://Calamity
						case "Star Swallower Containment Unit"://Calamity
						case "Taser"://Calamity
						case "Tracking Disk"://Calamity
						case "Tradewinds"://Calamity
						case "Air Spinner"://Calamity
						case "Goldplume Spear"://Calamity
						case "Wind Blade"://Calamity
						case "Aquashard Shotgun"://Calamity
						case "Bullet-Filled Shotgun"://Calamity
						case "Galeforce"://Calamity
						case "Sky Stabber"://Calamity
						case "Turbulance"://Calamity
							rarity = 2.35f;
                            break;
						case "Misery's Company"://Stars Above
						case "The Morning Star"://Stars Above
						case "Skofnung"://Stars Above
                        case "Apalistik"://Stars Above
                        case "Kazimierz Seraphim"://Stars Above
                        case "Irminsul's Dream"://Stars Above
						case "Enchanted Axe"://Calamity
                        case "Metal Monstrosity"://Calamity
                        case "Flurrystorm Cannon"://Calamity
							rarity = 2.4f;
							break;
                        case "Hardened Honeycomb"://Calamity
                        case "Glaive"://Calamity
                        case "Bladecrest Oathsword"://Calamity
                        case "Dragoon Drizzlefish"://Calamity
                        case "Old Lord Oathsword"://Calamity
							rarity = 2.45f;
                            break;
						case "Der Freischutz"://Stars Above
                        case "Death In Four Acts"://Stars Above
                        case "Cinquedea"://Calamity
						case "Kylie"://Calamity
                        case "Shinobi Blade"://Calamity
                        case "Staff of Necrosteocytes"://Calamity
							rarity = 2.5f;
							break;
                        case "Abyss Shocker"://Calamity
                        case "Archerfish"://Calamity
                        case "Ball O' Fugu"://Calamity
                        case "Black Anurian"://Calamity
                        case "Herring Staff"://Calamity
                        case "Lionfish"://Calamity
							rarity = 2.6f;
                            break;
						case "Atomic Annie"://Calamity
							rarity = 3f;
							break;
                        case "Gel Dart"://Calamity
                        case "Infernal Kris"://Calamity
							rarity = 3.05f;
                            break;
                        case "Cinder Blossom Staff"://Calamity
                        case "Flare Bolt"://Calamity
                        case "Hellwing Staff"://Calamity
							rarity = 3.15f;
                            break;
						case "Broken Biome Blade"://Calamity
                        case "Aestheticus"://Calamity
                        case "Bloody Edge"://Calamity
                        case "Eye of Night"://Calamity
                        case "Flesh of Infidelity"://Calamity
                        case "Consecrated Water"://Calamity
                        case "Tundra Flame Blossoms Staff"://Calamity
                        case "Wave Skipper"://Calamity
                        case "Prismalline"://Calamity
                        case "Cursed Dagger"://Calamity
							rarity = 3.2f;
                            break;
                        case "Clam Crusher"://Calamity
                        case "Clamor Rifle"://Calamity
                        case "Poseidon"://Calamity
						case "Shellfish Staff"://Calamity
						case "Storm Saber"://Calamity
							rarity = 3.25f;
                            break;
						case "Meowthrower"://Calamity
                        case "Black Hawk Remote"://Calamity
                        case "Blast Barrel"://Calamity
                        case "Carnage"://Calamity
                        case "P90"://Calamity
                        case "Slick Cane"://Calamity
							rarity = 3.35f;
                            break;
                        case "Ancient Ice Chunk"://Calamity
                        case "Caustic Staff"://Calamity
                        case "Clothier's Wrath"://Calamity
                        case "Cursed Capper"://Calamity
                        case "Equanimity"://Calamity
                        case "Eutrophic Scimitar"://Calamity
                        case "Evil Smasher"://Calamity
                        case "Flarewing Bow"://Calamity
                        case "Frigidflash Bolt"://Calamity
                        case "Gacruxian Mollusk"://Calamity
                        case "Glorious End"://Calamity
                        case "Haunted Scroll"://Calamity
                        case "Axe of Purity"://Calamity
                        case "Ichor Spear"://Calamity
                        case "Lunic Eye"://Calamity
						case "Needler"://Calamity
                        case "Polaris Parrotfish"://Calamity
                        case "Roxcalibur"://Calamity
                        case "Serpentine"://Calamity
                        case "True Caustic Edge"://Calamity
                        case "Yin-Yo"://Calamity
                        case "SHPC"://Calamity
                        case "Wyvern's Call"://Calamity
                        case "Stormfront Razor"://Calamity
							rarity = 3.4f;
                            break;
                        case "Butcher"://Calamity
                        case "Bazooka"://Calamity
                        case "Forgotten Apex Wand"://Calamity
                        case "Spear of Paleolith"://Calamity
                        case "Titanium Railgun"://Calamity
                        case "Mineral Mortar"://Calamity
                        case "Forsaken Saber"://Calamity
                        case "Relic of Ruin"://Calamity
                        case "Death Valley Duster"://Calamity
						case "Igneous Exaltation"://Calamity
						case "Brimblade"://Calamity
						case "Brimstone Fury"://Calamity
						case "Brimstone Sword"://Calamity
						case "Burning Sea"://Calamity
						case "Brimrose Staff"://Calamity
                        case "Desecrated Water"://Calamity
                        case "Adamantite Particle Accelerator"://Calamity
							rarity = 3.5f;
                            break;
						case "Inugami Ripsaw"://Stars Above
                            rarity = 3.55f;
                            break;
                        case "Aftershock"://Calamity
                        case "Earthen Pike"://Calamity
                        case "Slag Magnum"://Calamity
                            rarity = 3.8f;
                            break;
                        case "Burning Strife"://Calamity
                            rarity = 3.9f;
                            break;
						case "Cobalt Kunai"://Calamity
                        case "Palladium Javelin"://Calamity
							rarity = 4.1f;
                            break;
						case "Mythril Knife"://Calamity
                        case "Golden Gun"://Calamity
                        case "Blazing Star"://Calamity
						case "Orichalcum Spiked Gemstone"://Calamity
							rarity = 4.2f;
                            break;
						case "Luminary Wand"://Stars Above
							rarity = 4.25f;
                            break;
                        case "Titanium Shuriken"://Calamity
						case "Adamantite Throwing Axe"://Calamity
							rarity = 4.3f;
                            break;
						case "Force-of-Nature"://Stars Above
                        case "Aurum Edge"://Stars Above
                        case "Every Moment Matters"://Stars Above
                        case "Nychthemeron"://Calamity
							rarity = 4.35f;
                            break;
                        case "Midas Prime"://Calamity
                            rarity = 4.4f;
                            break;
						case "Karlan Truesilver"://Stars Above
                            rarity = 4.5f;
                            break;
                        case "Ice Star"://Calamity
                            rarity = 4.7f;
                            break;
                        case "Frostbite Blaster"://Calamity
                        case "Icicle Trident"://Calamity
							rarity = 4.8f;
                            break;
						case "Celestial Claymore"://Calamity
                        case "Cold Divinity"://Calamity
                        case "Effluvium Bow"://Calamity
                        case "Avalanche"://Calamity
                        case "Snowstorm Staff"://Calamity
                        case "Icebreaker"://Calamity
							rarity = 4.9f;
                            break;
                        case "Daedalus Golem Staff"://Calamity
                        case "Darkecho Greatbow"://Calamity
                        case "Starnight Lance"://Calamity
                        case "Flarefrost Blade"://Calamity
						case "Darklight Greatsword"://Calamity
						case "Shadecrystal Barrage"://Calamity
						case "Shardlight Pickaxe"://Calamity
						case "Abyssal Warhammer"://Calamity
						case "Shimmerspark"://Calamity
                        case "Kelvin Catalyst"://Calamity
							rarity = 4.95f;
                            break;
						case "Orthocera Shell"://Calamity
						case "Scourge of the Seas"://Calamity
						case "Downpour"://Calamity
						case "Submarine Shocker"://Calamity
						case "Deepsea Staff"://Calamity
						case "Barinautical"://Calamity
						case "Flak Toxicannon"://Calamity
						case "Miasma"://Calamity
						case "Bonebreaker"://Calamity
						case "Corroded Caustibow"://Calamity
						case "Acidic Rain Barrel"://Calamity
                        case "Slithering Eels"://Calamity
                        case "Skyfin Bombers"://Calamity
                        case "Gleaming Magnolia"://Calamity
                        case "Sea's Searing"://Calamity
                        case "Sulphurous Grabber"://Calamity
						case "Belching Saxophone"://Calamity
							rarity = 5f;
                            break;
                        case "Majestic Guard"://Calamity
							rarity = 5.05f;
                            break;
						case "Inferna Cutter"://Calamity
						case "Matter Modulator"://Calamity
						case "Mounted Scanner"://Calamity
						case "Ion Blaster"://Calamity
						case "Forbidden Oathblade"://Calamity
						case "Grave Grimreaver"://Calamity
						case "Spear of Destiny"://Calamity
                        case "Totality Breakers"://Calamity
							rarity = 5.15f;
							break;
						case "Dormant Brimseeker"://Calamity
						case "Seething Discharge"://Calamity
						case "Brimlance"://Calamity
							rarity = 5.25f;
							break;
						case "Hydraulic Volt Crasher"://Calamity
						case "Frequency Manipulator"://Calamity
						case "Speed Blaster"://Calamity
							rarity = 5.3f;
							break;
						case "Cryogenic Staff"://Calamity
                        case "Arctic Bear Paw"://Calamity
						case "Cryophobia"://Calamity
							rarity = 5.4f;
                            break;
						case "Saltwater Scourge"://Stars Above
						case "Lightning Hawk"://Calamity
						case "Sun God Staff"://Calamity
						case "True Bloody Edge"://Calamity
						case "Biome Blade"://Calamity
						case "Pwnagehammer"://Calamity
						case "Catastrophe Claymore"://Calamity
							rarity = 5.5f;
							break;
						case "Dreadnought Chemtank"://Stars Above
						case "Sparkblossom's Beacon"://Stars Above
							rarity = 5.7f;
							break;
						case "Terror Talons"://Calamity
                            rarity = 5.75f;
                            break;
						case "Crystal Piercer"://Calamity
							rarity = 5.85f;
							break;
						case "Armaments of the Sky Striker"://Stars Above
						case "Umbra"://Stars Above
						case "Seaborn Apalistik"://Stars Above
						case "Havoc's Breath"://Calamity
                        case "Tyrant Yharim's Ultisword"://Calamity
                        case "Oblivion"://Calamity
                        case "Animosity"://Calamity
                        case "Entropy's Vigil"://Calamity
						case "Lashes of Chaos"://Calamity
                        case "Brimlash"://Calamity
                        case "Art Attack"://Calamity
                        case "Deep Wounder"://Calamity
                        case "Crushsaw Crasher"://Calamity
							rarity = 6f;
                            break;
						case "Gauss Pistol"://Calamity
							rarity = 6.05f;
							break;
						case "Spent Fuel Container"://Calamity
                        case "Dreadmine Staff"://Calamity
                        case "Ballistic Poison Bomb"://Calamity
                        case "Megalodon"://Calamity
                        case "Flak Kraken"://Calamity
                        case "Typhon's Greed"://Calamity
                        case "Floodtide"://Calamity
                        case "Abyss Blade"://Calamity
                        case "Undine's Retribution"://Calamity
                        case "Hadal Urn"://Calamity
                        case "Apoctolith"://Calamity
							rarity = 6.1f;
                            break;
                        case "Hellborn"://Calamity
						case "Blossom Flux"://Calamity
                        case "Plantation Staff"://Calamity
                        case "Terra Disk"://Calamity
                        case "Terra Flameburster"://Calamity
                        case "Cosmic Bolter"://Calamity
                        case "Terra Lance"://Calamity
                        case "Terra Edge"://Calamity
                        case "Terra Ray"://Calamity
                        case "Viral Sprout"://Calamity
                        case "Mangrove Chakram"://Calamity
						case "Bladedge Railbow"://Calamity
						case "Quagmire"://Calamity
						case "Hellkite"://Calamity
						case "Hellion Flower Spear"://Calamity
						case "Feralthorn Claymore"://Calamity
						case "Everglade Spray"://Calamity
						case "Berserker Waraxe"://Calamity
						case "Beastial Pickaxe"://Calamity
							rarity = 6.25f;
                            break;
                        case "Frosty Flare"://Calamity
                        case "Arbalest"://Calamity
                        case "Mantis Claws"://Calmity
							rarity = 6.3f;
                            break;
						case "Xenoblade"://Stars Above
							rarity = 6.4f;
							break;
						case "Ride the Bull"://Stars Above
						case "Drachenlance"://Stars Above
						case "Hullwrought"://Stars Above
						case "El Capitan's Hardware"://Stars Above
                        case "Exorcism"://Calamity
                        case "Valkyrie Ray"://Calamity
                        case "Hydra"://Calamity
                        case "The Ballista"://Calamity
                        case "Marksman Bow"://Calamity
                        case "Magna Striker"://Calamity
                        case "Comet Quasher"://Calamity
                        case "Primordial Earth"://Calamity
                        case "Pearl God"://Calamity
                        case "Anarchy Blade"://Calamity
                        case "Ark of the Ancients"://Calamity
                        case "Frostcrush Valari"://Calamity
                        case "Tears of Heaven"://Calamity
							rarity = 6.5f;
                            break;
                        case "Hunter's Symphony"://Stars Above
                            rarity = 6.6f;
                            break;
                        case "Sand Sharknado Staff"://Calamity
                        case "Sandslasher"://Calamity
                        case "Duststorm in a Bottle"://Calamity
                        case "Sandstorm"://Calamity
                        case "Tumbleweed"://Calamity
                        case "Shifting Sands"://Calamity
							rarity = 6.75f;
                            break;
							rarity = 6.9f;
                            break;
						case "Kifrosse"://Stars Above
						case "Shock Grenade"://Calamity
						case "Monkey Darts"://Calamity
							rarity = 7.1f;
                            break;
                        case "Absolute Zero"://Calamity
                        case "Eternal Blizzard"://Calamity
                        case "Winter's Fury"://Calamity
							rarity = 7.2f;
                            break;
						case "Voice of the Fallen"://Stars Above
                        case "Crimson Outbreak"://Stars Above
                        case "Leviatitan"://Calamity
                        case "Brackish Flask"://Calamity
						case "Greentide"://Calamity
						case "Gastric Belcher Staff"://Calamity
						case "Leviathan Teeth"://Calamity
						case "Atlantis"://Calamity
						case "Anahita's Arpeggio"://Calamity
							rarity = 7.35f;
                            break;
                        case "Fantasy Talisman"://Calamity
                        case "Alula Australis"://Calamity
                        case "Auroradical Throw"://Calamity
						case "Borealis Bomber"://Calamity
						case "Nebulash"://Calamity
						case "Aurora Blazer"://Calamity
						case "Leonid Progenitor"://Calamity
                        case "Keelhaul"://Calamity
							rarity = 7.4f;
                            break;
                        case "Aegis Blade"://Calamity
                        case "Titan Arm"://Calamity
                        case "Astral Scythe"://Calamity
                        case "Astralachnea Staff"://Calamity
						case "Stellar Cannon"://Calamity
						case "Hive Pod"://Calamity
						case "Abandoned Slime Staff"://Calamity
                        case "Stellar Knife"://Calamity
							rarity = 7.45f;
                            break;
                        case "Heavenfallen Stardisk"://Calamity
                            rarity = 7.5f;
                            break;
						case "Subduction Slicer"://Calamity
                        case "Helstorm"://Calamity
                        case "Continental Greatbow"://Calamity
						case "Vulcanite Lance"://Calamity
						case "Hellfire Flamberge"://Calamity
						case "Fault Line"://Calamity
						case "Fallen Paladin's Hammer"://Calamity
						case "Forbidden Sun"://Calamity
						case "Tectonic Truncator"://Calamity
						case "Seismic Hampick"://Calamity
						case "Resurrection Butterfly"://Calamity
						case "Clock Gatlignum"://Calamity
						case "Barracuda Gun"://Calamity
						case "Omniblade"://Calamity
						case "Lucrecia"://Calamity
						case "System Bane"://Calamity
						case "Pulse Turret Remote"://Calamity
						case "Gauss Rifle"://Calamity
						case "Gatling Laser"://Calamity
						case "Galvanizing Glaive"://Calamity
							rarity = 7.6f;
                            break;
						case "Nullification Pistol"://Calamity
                        case "Wingman"://Calamity
                        case "Plaguenade"://Calamity
							rarity = 7.8f;
							break;
                        case "Tenebreus Tides"://Calamity
                            rarity = 7.85f;
                            break;
						case "Yanmei's Knife"://Calamity
                        case "Epidemic Shredder"://Calamity
                        case "Soul Harvester"://Calamity
                        case "Wither Blossoms Staff"://Calamity
							rarity = 7.9f;
                            break;
						case "Defective Sphere"://Calamity
                            rarity = 7.95f;
                            break;
                        case "Pandemic"://Calamity
                        case "Malachite"://Calamity
						case "Infected Remote"://Calamity
						case "The Hive"://Calamity
						case "Virulence"://Calamity
						case "Diseased Pike"://Calamity
						case "Plague Staff"://Calamity
						case "Blight Spewer"://Calamity
						case "Malevolence"://Calamity
						case "Fuel Cell Bundle"://Calamity
						case "Pestilent Defiler"://Calamity
						case "The Syringe"://Calamity
						case "True Forbidden Oathblade"://Calamity
							rarity = 8f;
                            break;
						case "Stygian Nymph"://Stars Above
						case "Penthesilea's Muse"://Stars Above
						case "Vision of Euthymia"://Stars Above
                        case "Cæsura of Despair"://Stars Above
						case "The Blood Blade"://Stars Above
                        case "Kroniic Principality"://Stars Above
							rarity = 8.1f;
                            break;
							rarity = 8.25f;
                            break;
						case "Phantom in the Mirror"://Stars Above
                        case "Hollowheart Albion"://Stars Above
							rarity = 8.3f;
                            break;
						case "Mercy"://Stars Above
						case "Genocide"://Stars Above
                        case "Gloves of the Black Silence"://Stars Above
                        case "Tartaglia"://Stars Above
                        case "Plenilune Gaze"://Stars Above
                        case "Duke's Decapitator"://Calamity
                        case "Briny Baron"://Calamity
							rarity = 8.4f;
                            break;
						case "Phantom Lance"://Calamity
							rarity = 8.5f;
							break;
                        case "Infernal Rift"://Calamity
                            rarity = 8.6f;
                            break;
						case "Corpus Avertor"://Calamity
						case "Vesuvius"://Calmity
						case "Hematemesis"://Calamity
						case "Spikecrag Staff"://Calamity
						case "Cranium Smasher"://Calamity
						case "Ultimus Cleaver"://Calamity
						case "Realm Ravager"://Calamity
                            rarity = 8.7f;
                            break;
						case "Conference Call"://Calamity
                        case "Vortexpopper"://Calamity
						case "Spectralstorm Cannon"://Calamity
						case "Shroomer"://Calamity
						case "Scorpio"://Calamity
						case "The Swarmer"://Calamity
						case "Nano Purge"://Calamity
                        case "Star of Destruction"://Calmity
                        case "Luminous Striker"://Calamity
						case "God's Bellows"://Calamity
						case "Entropic Claymore"://Calamity
						case "Tome of Fates"://Calamity
						case "Cosmic Rainbow"://Calamity
                        case "Eye of Magnus"://Calamity
                        case "Storm Ruler"://Calmity
                        case "Grand Guardian"://Calamity
                        case "Baleful Harvester"://Calamity
                        case "Lazhar"://Calamity
                        case "Arch Amaryllis"://Calamity
							rarity = 9.05f;
                            break;
						case "Regulus Riot"://Calamity
                        case "Godspawn Helix Staff"://Calamity
						case "Starfall"://Calamity
						case "Star Sputter"://Calamity
						case "The Microwave"://Calamity
						case "Radiant Star"://Calamity
						case "Astral Bow"://Calamity
						case "Astral Blaster"://Calamity
						case "True Biome Blade"://Calamity
						case "Astral Pike"://Calamity
						case "Astral Blade"://Calamity
						case "Astral Staff"://Calamity
						case "Astral Pickaxe"://Calamity
						case "Astral Hamaxe"://Calamity
							rarity = 9.1f;
                            break;
						case "Genesis Pickaxe"://Calamity
                            rarity = 9.2f;
                            break;
						case "Sakura's Vengeance"://Stars Above
                        case "Arachnid Needlepoint"://Stars Above
							rarity = 9.3f;
                            break;
                        case "Elephant Killer"://Calamity
                        case "Tactical Plague Engine"://Calamity
                        case "Flowers of Mortality"://Calamity
                        case "Elemental Axe"://Calamity
                        case "Elemental Disk"://Calamity
						case "Starfleet"://Calamity
						case "Shredder"://Calamity
						case "Prideful Hunter's Planar Ripper"://Calamity
						case "Planetary Annihilation"://Calamity
						case "Onyx Chain Blaster"://Calamity
						case "Elemental Eruption"://Calamity
						case "Elemental Blaster"://Calamity
						case "Disseminator"://Calamity
						case "Clockwork Bow"://Calamity
						case "Astreal Defeat"://Calamity
						case "Stellar Striker"://Calamity
						case "Stellar Contempt"://Calamity
						case "Solstice Claymore"://Calamity
						case "Rem's Revenge"://Calamity
						case "Plague Keeper"://Calamity
						case "Greatsword of Judgement"://Calamity
						case "Elemental Shiv"://Calamity
						case "Elemental Lance"://Calamity
						case "Devastation"://Calamity
						case "Ark of the Elements"://Calamity
						case "Ultra Liquidator"://Calamity
						case "Nuclear Fury"://Calamity
						case "Genisis"://Calamity
						case "Elemental Ray"://Calamity
						case "Effervescence"://Calamity
						case "Augur of the Elements"://Calamity
						case "Asteroid Staff"://Calamity
						case "Apoctosis Array"://Calamity
                        case "Celestial Reaper"://Calamity
							rarity = 9.5f;
                            break;
						case "Key of the Sinner"://Stars Above
						case "Crimson Sakura Alpha"://Stars Above
                        case "Utensil Poker"://Calamity
							rarity = 9.7f;
                            break;
						case "Ozma Ascendant"://Stars Above
                            rarity = 9.8f;
                            break;
						case "The Only Thing I Know For Real"://Stars Above
						case "Manifestation"://Stars Above
                        case "Claimh Solais"://Stars Above
							rarity = 9.9f;
                            break;
						case "Shard of Antumbra"://Calamity
							rarity = 9.95f;
							break;
						case "Yunlai Stiletto"://Stars Above
                        case "Twin Stars of Albiero"://Stars Above
                        case "Rex Lapis"://Stars Above
                        case "Unforgotten"://Stars Above
                        case "Liberation Blazing"://Stars Above
                        case "Catalyst's Memory"://Stars Above
                        case "Lunar Kunai"://Calamity
							rarity = 10.1f;
                            break;
                        case "Eternal Star"://Stars Above
                        case "Soul Reaver"://Stars Above
							rarity = 10.15f;
                            break;
							rarity = 10.2f;
                            break;
						case "Virtue's Edge"://Stars Above
                        case "Shadowless Cerulean"://Stars Above
                        case "Naganadel"://Stars Above
						case "Vermilion Daemon"://Stars Above
                        case "Suistrume"://Stars Above
							rarity = 10.25f;
                            break;
						case "Ignition Astra"://Stars Above
                        case "Hullwrought MK. II"://Stars Above
							rarity = 10.4f;
                            break;
                        case "Celestial Princess' Genesis"://Stars Above
                            rarity = 10.6f;
                            break;
						case "Vermilion Riposte"://Stars Above
                        case "Light Unrelenting"://Stars Above
                        case "The Everlasting Pickaxe"://Stars Above
						case "Burning Desire"://Stars Above
						case "Key of the King's Law"://Stars Above
							rarity = 11.3f;
                            break;
                        case "Bury The Light"://Stars Above
                            rarity = 11.4f;
                            break;
						case "Architect's Luminance"://Stars Above
						case "Cosmic Destroyer"://Stars Above
							rarity = 11.6f;
                            break;
						case "Mjölnir"://Thorium
                            rarity = 13f;
                            break;
						default:
                            if (useCalamiryValuesOnly) {
                                int i;
                                for (i = 0; i < numRarities; i++) {
                                    float max = calamityMaxValues[i];
                                    if (max >= sampleValue) {
                                        float min = calamityMinValues[i];
                                        if (min >= sampleValue)
                                            i--;

                                        break;
                                    }
                                }

                                rarity = i;
                            }
                            else if (rarity >= 11 && sampleItem.value > maxValues[11]) {
                                int i;
                                for (i = 12; i < numRarities; i++) {
                                    float min = minValues[i];
                                    if (min >= sampleItem.value) {
                                        i--;

                                        break;
                                    }
                                }

                                rarity = i;
                            }

							if (rarity > numRarities - 1) {
								rarity = numRarities - 1;
							}
							else if (rarity < 0) {
								rarity = 0;
							}

							break;
                    }

                    break;
                default:
                    if (rarity > numRarities - 1) {
                        rarity = numRarities - 1;
                    }
                    else if (rarity < 0) {
                        rarity = 0;
                    }
                    break;
            }

            return rarity;
        }
        public static float GetValueRarity(Item sampleItem, float rarity, bool useCalamiryValuesOnly, bool usingBaseRarity = false) {
            int sampleValue = sampleItem.value;
            float valueMultiplier = 0.5f;

            int rarityInt = (int)rarity;
            if (rarityInt < 0) {
                rarityInt = 0;
			}
			else if (rarityInt >= numRarities) {
                rarityInt = numRarities - 1;
			}

            float averageValue = useCalamiryValuesOnly ? calamityAverageValues[rarityInt] : averageValues[rarityInt];
            int maxOrMin;
            if (sampleValue < averageValue) {
                if (useCalamiryValuesOnly) {
                    maxOrMin = calamityMinValues[rarityInt];
                }
                else {
                    maxOrMin = minValues[rarityInt];
                }
            }
            else {
                if (useCalamiryValuesOnly) {
                    maxOrMin = calamityMaxValues[rarityInt];
                }
                else {
                    maxOrMin = maxValues[rarityInt];
                }
            }

            float denom = Math.Abs(averageValue - maxOrMin);
            float valueRarity = valueMultiplier + valueMultiplier * (sampleValue - averageValue) / denom;
            if((valueRarity >= 1f || valueRarity <= 0f) && !usingBaseRarity && !useCalamiryValuesOnly && rarity != (float)sampleItem.rare) {
                //Get it's base valueRarity
                valueRarity = GetValueRarity(sampleItem, sampleItem.rare, useCalamiryValuesOnly, true);
            }
            else if (valueRarity < 0f) {
                valueRarity = 0f;
            }
            else if (valueRarity > 1f) {
                valueRarity = 1f;
            }

            return valueRarity;
        }
        public static float GetWeaponMultiplier(this Item item, Item consumedItem, out int infusedPower) {
            if (consumedItem.IsAir) {
                infusedPower = 0;
                return 1f;
            }

            float itemRarity = GetWeaponRarity(item);
            float consumedRarity = GetWeaponRarity(consumedItem);
            infusedPower = (int)Math.Round(consumedRarity * 100f);
            float multiplier = (float)Math.Pow(InfusionDamageMultiplier, consumedRarity - itemRarity);

            return multiplier > 1f || WEMod.clientConfig.AllowInfusingToLowerPower ? multiplier : 1f;
        }
        public static int GetInfusionPower(this EnchantedItem enchantedItem) {
            if (enchantedItem.infusedItemName != "") {
                return enchantedItem.infusionPower;
            }
            else {
                float itemRarity = GetWeaponRarity(enchantedItem.Item);
                return (int)Math.Round(itemRarity * 100f); ;
            }
        }
        public static float GetWeaponMultiplier(this Item item, int consumedItemInfusionPower) {
            float itemRarity = GetWeaponRarity(item);
            float consumedRarity = (float)consumedItemInfusionPower / 100f;
            float multiplier = (float)Math.Pow(InfusionDamageMultiplier, consumedRarity - itemRarity);

            return multiplier > 1f ? multiplier : 1f;
        }
        public static int GetWeaponInfusionPower(this Item item) {
            return GetWeaponInfusionPower(item, out float rarity, out float valueRarity);
        }
        public static int GetWeaponInfusionPower(this Item item, out float rarity, out float valueRarity) {
            rarity = float.MinValue;
            valueRarity = float.MinValue;

            if(!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return 0;

            if (iGlobal.infusedItemName != "")
                return iGlobal.infusionPower;

            float combinedRarity = GetWeaponRarity(item, out rarity, out valueRarity);
            int infusedPower = (int)Math.Round(combinedRarity * 100f);

            return infusedPower;
        }
        public static string GetInfusionItemName(this Item item) {
            if (item.TryGetEnchantedItem(out EnchantedItem iGlobal) && iGlobal.infusedItemName != "") {
                return iGlobal.infusedItemName;
            }
			else {
                return item.Name;
            }
        }
        public static bool TryInfuseItem(this Item item, Item consumedItem, bool reset = false, bool finalize = false) {
            bool failedItemFind = false;
            if (consumedItem.TryGetEnchantedItem(out EnchantedItem cGlobal) && cGlobal.infusedItemName != "") {
                if (TryInfuseItem(item, cGlobal.infusedItemName, reset, finalize)) {
                    return true;
                }
				else {
                    failedItemFind = true;
                }
            }

            if(!item.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                $"Failied to infuse item: {item.S()} with consumedItem: {consumedItem.S()}".LogNT(ChatMessagesIDs.FailedInfuseItem);
                return false;
			}
            
            int infusedPower = 0;
            float damageMultiplier = 1f;
            string consumedItemName = "";
            int infusedArmorSlot = -1;
            if (iGlobal is EnchantedWeapon enchantedWeapon && (cGlobal is EnchantedWeapon || consumedItem.IsAir)) {
                //Weapon
                if (item.GetWeaponInfusionPower() < consumedItem.GetWeaponInfusionPower() || WEMod.clientConfig.AllowInfusingToLowerPower || reset) {
                    if (failedItemFind) {
                        infusedPower = cGlobal.infusionPower;
                        consumedItemName = cGlobal.infusedItemName;
                        damageMultiplier = enchantedWeapon.infusionDamageMultiplier;
                    }
                    else {
                        consumedItemName = consumedItem.Name;
                        damageMultiplier = GetWeaponMultiplier(item, consumedItem, out infusedPower);
                    }

                    if (enchantedWeapon.infusionPower < infusedPower || WEMod.clientConfig.AllowInfusingToLowerPower || reset) {
                        if (!finalize) {
                            enchantedWeapon.infusionDamageMultiplier = damageMultiplier;
                        }
                        else {
                            enchantedWeapon.infusionDamageMultiplier = damageMultiplier;
                            enchantedWeapon.infusionPower = infusedPower;
                            enchantedWeapon.infusedItemName = consumedItemName;
                            int infusionValueAdded = ContentSamples.ItemsByType[consumedItem.type].value - ContentSamples.ItemsByType[item.type].value;
                            enchantedWeapon.InfusionValueAdded = infusionValueAdded > 0 ? infusionValueAdded : 0;
                        }

                        return true;
                    }
                    else if (finalize) {
                        Main.NewText($"Your {item.Name}({enchantedWeapon.infusionPower}) cannot gain additional power from the offered {consumedItem.Name}({infusedPower}).");
                    }
                }
                else if (finalize) {
                    Main.NewText($"The Infusion Power of the item being upgraded must be lower than the Infusion Power of the consumed item.");
                }

                return false;
            }
            else if (iGlobal is EnchantedArmor enchantedArmor && (cGlobal is EnchantedArmor || consumedItem.IsAir)) {
                //Armor
                if (item.GetSlotIndex() == consumedItem.GetSlotIndex() || reset) {
                    if (item.GetInfusionArmorSlot(true) != consumedItem.GetInfusionArmorSlot()) {
                        if (failedItemFind) {
                            consumedItemName = enchantedArmor.infusedItemName;
                            infusedArmorSlot = ContentSamples.ItemsByType[item.type].GetInfusionArmorSlot();
                        }
                        else {
                            consumedItemName = consumedItem.Name;
                            infusedArmorSlot = consumedItem.GetInfusionArmorSlot();
                        }

                        if (!finalize) {
                            item.UpdateArmorSlot(infusedArmorSlot);
                        }
                        else {
                            enchantedArmor.infusedItemName = consumedItemName;
                            enchantedArmor.infusedArmorSlot = infusedArmorSlot;
                            enchantedArmor.infusedItem = new Item(consumedItem.type);
                            int infusionValueAdded = ContentSamples.ItemsByType[consumedItem.type].value - ContentSamples.ItemsByType[item.type].value;
                            enchantedArmor.InfusionValueAdded = infusionValueAdded > 0 ? infusionValueAdded : 0;
                        }

                        return true;
                    }
                    else if (finalize && !failedItemFind) {
                        Main.NewText($"The item being upgraded has the same set bonus as the item being consumed and will have no effect.");
                    }

                    return false;
                }
                else if (finalize && !failedItemFind) {
                    Main.NewText($"You cannot infuse armor of different types such as a helmet and body.");
                }

                return false;
            }
            if (finalize && !failedItemFind && (EnchantedItemStaticMethods.IsWeaponItem(item) || EnchantedItemStaticMethods.IsArmorItem(item))) {
                Main.NewText($"Infusion is only possible between items of the same type (Weapon/Armor)");
            }

            return false;
        }
        public static bool TryInfuseItem(this Item item, string infusedItemName, bool reset = false, bool finalize = false) {
            for (int itemType = 1; itemType < ItemLoader.ItemCount; itemType++) {
                Item foundItem = new Item(itemType);
                if(foundItem.Name == infusedItemName)
                    return TryInfuseItem(item, foundItem, reset, finalize);
            }

            return TryInfuseItem(item, new Item(), reset, finalize);
        }
        public static void GetGlotalItemStats(this Item item, Item infusedItem, out int infusedPower, out float damageMultiplier, out int infusedArmorSlot) {
			if (EnchantedItemStaticMethods.IsWeaponItem(item)) {
                damageMultiplier = GetWeaponMultiplier(item, infusedItem, out infusedPower);
                infusedArmorSlot = -1;
            }
			else {
                damageMultiplier = 1f;
                infusedPower = 0;
                infusedArmorSlot = infusedItem.GetInfusionArmorSlot();
            }
        }
        public static bool TryGetInfusionStats(this EnchantedItem iGlobal) {
            if (iGlobal == null)
                return false;

            bool succededGettingStats = TryGetInfusionStats(iGlobal, iGlobal.infusedItemName, out int infusedPower, out float damageMultiplier, out int infusedArmorSlot, out Item infusedItem);
            if (succededGettingStats) {
                iGlobal.infusionPower = infusedPower;
                if (iGlobal is EnchantedWeapon enchantedWeapon) {
                    enchantedWeapon.infusionDamageMultiplier = damageMultiplier;
                }
                else if (iGlobal is EnchantedArmor enchantedArmor) {
                    enchantedArmor.infusedArmorSlot = infusedArmorSlot;
                    enchantedArmor.infusedItem = infusedItem;
                }
            }
            else if (iGlobal is EnchantedWeapon enchantedWeapon) {
                //Damage Multiplier (If failed to Get Global Item Stats)
                enchantedWeapon.infusionDamageMultiplier = iGlobal.Item.GetWeaponMultiplier(iGlobal.infusionPower);
            }
                

            return succededGettingStats;
        }
        public static bool TryGetInfusionStats(this EnchantedItem iGlobal, string infusedItemName, out int infusedPower, out float damageMultiplier, out int infusedArmorSlot, out Item infusedItem) {
            infusedPower = 0;
            damageMultiplier = 1f;
            infusedArmorSlot = -1;
            infusedItem = null;

            if (infusedItemName != "") {
                int type = 0;
                for (int itemType = 1; itemType < ItemLoader.ItemCount; itemType++) {
                    Item foundItem = ContentSamples.ItemsByType[itemType];
                    if (foundItem.Name == infusedItemName) {
                        type = itemType;
                        infusedItem = new Item(itemType);
                        break;
                    }
                }

                if (type > 0) {
                    GetGlotalItemStats(iGlobal.Item, new Item(type), out infusedPower, out damageMultiplier, out infusedArmorSlot);
                    if (iGlobal is EnchantedWeapon enchantedWeapon) {
                        //item.UpdateInfusionDamage(damageMultiplier, false);
                        enchantedWeapon.infusionDamageMultiplier = damageMultiplier;
                    }
                    else if (iGlobal is EnchantedArmor enchantedArmor2) {
                        enchantedArmor2.Item.UpdateArmorSlot(infusedArmorSlot);
                    }

                    return true;
                }
            }

            if (iGlobal is EnchantedArmor enchantedArmor) {
                enchantedArmor.Item.UpdateArmorSlot(infusedArmorSlot);
            }
            
            return false;
        }
        public static void UpdateArmorSlot(this Item item, int infusedArmorSlot) {
            if (WEMod.serverConfig.DisableArmorInfusion)
                return;

            Item sampleItem = ContentSamples.ItemsByType[item.type];
            item.headSlot = sampleItem.headSlot;
            item.bodySlot = sampleItem.bodySlot;
            item.legSlot = sampleItem.legSlot;
            if (infusedArmorSlot != -1) {
                if (item.headSlot != -1) {
                    item.headSlot = infusedArmorSlot;
                }
                else if (item.bodySlot != -1) {
                    item.bodySlot = infusedArmorSlot;
                }
                else if (item.legSlot != -1) {
                    item.legSlot = infusedArmorSlot;
                }
            }
		}
        public static int GetInfusionArmorSlot(this Item item, bool checkBase = false, bool getCurrent = false) {
            if (!getCurrent && item.TryGetEnchantedArmor(out EnchantedArmor iGlobal) && iGlobal.infusedArmorSlot != -1) {
                return iGlobal.infusedArmorSlot;
            }
			else
            {
                if (checkBase) {
                    return ContentSamples.ItemsByType[item.type].GetInfusionArmorSlot();
                }
                else
                {
                    if (item.headSlot != -1) {
                        return item.headSlot;
                    }
                    else if (item.bodySlot != -1) {
                        return item.bodySlot;
                    }
                    else if (item.legSlot != -1) {
                        return item.legSlot;
                    }
					else {
                        return -1;
                    }
                }
            }
        }
        public static int GetSlotIndex(this Item item) {
            Item SampleItem = ContentSamples.ItemsByType[(item.type)];
            if (SampleItem.headSlot != -1) {
                return 0;
            }
            else if (SampleItem.bodySlot != -1) {
                return 1;
            }
            else if (SampleItem.legSlot != -1) {
                return 2;
            }
			else {
                return -1;
            }
        }
    }

    public static class InfusionStaticClasses {
        public static bool InfusionAllowed(this Item item, out bool configAllowed) {
            bool weapon = EnchantedItemStaticMethods.IsWeaponItem(item);
            bool armor = EnchantedItemStaticMethods.IsArmorItem(item);
            bool WeaponAndWeaponInfusionAllowed = weapon && WEMod.serverConfig.InfusionDamageMultiplier > 1000;
			bool ArmorAndArmorInfusionAllowed = armor && !WEMod.serverConfig.DisableArmorInfusion;
			configAllowed = WeaponAndWeaponInfusionAllowed || ArmorAndArmorInfusionAllowed;

			return weapon || armor;
		}
	}
}
