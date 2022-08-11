using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
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
        public static int[] calamityMaxValues = new int[] {5000, 10000, 20000, 40000, 120000, 240000, 360000, 480000, 600000, 800000, 1000000, 1100000, 1200000, 1300000, 1400000, 1500000, 2000000, 2500000};
        public const float minMaxValueMultiplier = 0.25f;

        public static void SetUpVanillaWeaponInfusionPowers() {
            Dictionary<string, List<int[]>> weaponsDict = GetItemDict(GetItemDictModeID.Weapon);
            int[] total = new int[numRarities];
            int[] count = new int[numRarities];
            
            //For each vanilla item
            foreach (int[] stats in weaponsDict["Terraria"]) {
                int rarity = stats[0];
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
                                infusionPowers[infusionPower].Add(clone.Name, itemDetails);
							}
                        }
                    }
                }
            }

            if(printList && postSetupPrintList) {
                if (mode == GetItemDictModeID.Weapon) {
                    msg += "\nWeapon, Infusion Power, Value Rarity, Rarity, Original Rarity, Value, Item ID, Damage, Use Time, DPS";
                    foreach (int infusionPower in infusionPowers.Keys) {
                        foreach(string name in infusionPowers[infusionPower].Keys) {
                            int damage = infusionPowers[infusionPower][name].Item.damage;
                            int useTime = infusionPowers[infusionPower][name].Item.useTime;
                            float dps = (float)damage * 60f / (float)useTime;
                            msg += $"\n{name}, {infusionPower}, {infusionPowers[infusionPower][name].ValueRarity}, {infusionPowers[infusionPower][name].Rarity}, {infusionPowers[infusionPower][name].Item.rare}, {infusionPowers[infusionPower][name].Item.value}, {infusionPowers[infusionPower][name].Item.type}, {damage}, {useTime}, {dps}";
                        }
                    }
                }
                    //Print list of items
                    msg.Log();
            }
            
            return itemsDict;
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
            useCalamiryValuesOnly = false;
            float rarity = sampleItem.rare;
            int sampleValue = sampleItem.value;

            //If from calamity, calculate just from value
            if (sampleItem.ModItem?.Mod.Name == "CalamityMod")
                useCalamiryValuesOnly = true;

            switch (sampleItem.type) {
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
                case ItemID.Minishark:
                case ItemID.ThornWhip:
                case ItemID.StarCannon:
                case ItemID.Starfury:
                case ItemID.ThornChakram:
                    rarity = 1f;
                    break;
                case ItemID.BloodWater:
                case ItemID.HolyWater:
                case ItemID.UnholyWater:
                    rarity = 1.2f;
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
                case ItemID.BouncingShield:
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
                case ItemID.StarWrath:
                    rarity = 10f;
                    break;
                case > ItemID.Count:
                    //Manually set rarity of an item
                    switch (sampleItem.Name) {
                        case "Primary Zenith":
                            rarity = 0f;
                            break;
                        case "Nullification Pistol":
                        case "Atomic Annie":
                            rarity = 3f;
                            break;
                        case "The Only Thing I Know For Real":
                            rarity = 9f;
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

            return multiplier > 1f ? multiplier : 1f;
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
            if (EnchantedItemStaticMethods.IsWeaponItem(item) && (EnchantedItemStaticMethods.IsWeaponItem(consumedItem) || consumedItem.IsAir)) {
                //Weapon
                if (item.GetWeaponInfusionPower() < consumedItem.GetWeaponInfusionPower() || reset) {
                    if (failedItemFind) {
                        infusedPower = cGlobal.infusionPower;
                        damageMultiplier = cGlobal.infusionDamageMultiplier;
                        consumedItemName = cGlobal.infusedItemName;
                    }
                    else {
                        consumedItemName = consumedItem.Name;
                        damageMultiplier = GetWeaponMultiplier(item, consumedItem, out infusedPower);
                    }

                    if (iGlobal.infusionPower < infusedPower || reset) {
                        if (!finalize) {
                            //item.UpdateInfusionDamage(damageMultiplier);
                            iGlobal.infusionDamageMultiplier = damageMultiplier;
                        }
                        else {
                            iGlobal.infusionPower = infusedPower;
                            iGlobal.infusionDamageMultiplier = damageMultiplier;
                            iGlobal.infusedItemName = consumedItemName;
                            int infusionValueAdded = ContentSamples.ItemsByType[consumedItem.type].value - ContentSamples.ItemsByType[item.type].value;
                            iGlobal.InfusionValueAdded = infusionValueAdded > 0 ? infusionValueAdded : 0;
                        }

                        return true;
                    }
                    else if (finalize) {
                        Main.NewText($"Your {item.Name}({iGlobal.infusionPower}) cannot gain additional power from the offered {consumedItem.Name}({infusedPower}).");
                    }
                }
                else if (finalize) {
                    Main.NewText($"The Infusion Power of the item being upgraded must be lower than the Infusion Power of the consumed item.");
                }

                return false;
            }
            else if (EnchantedItemStaticMethods.IsArmorItem(item) && ((EnchantedItemStaticMethods.IsArmorItem(consumedItem) || consumedItem.IsAir))) {
                //Armor
                if (item.GetSlotIndex() == consumedItem.GetSlotIndex()) {
                    if (item.GetInfusionArmorSlot(true) != consumedItem.GetInfusionArmorSlot()) {
                        if (failedItemFind) {
                            consumedItemName = iGlobal.infusedItemName;
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
                            iGlobal.infusedItemName = consumedItemName;
                            iGlobal.infusedArmorSlot = infusedArmorSlot;
                            int infusionValueAdded = ContentSamples.ItemsByType[consumedItem.type].value - ContentSamples.ItemsByType[item.type].value;
                            iGlobal.InfusionValueAdded = infusionValueAdded > 0 ? infusionValueAdded : 0;
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
                Main.NewText($"Infusion is only possitle between items of the same type (Weapon/Armor)");
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
        public static bool TryGetInfusionStats(this Item item) {
            if (!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return false;

            bool returnValue = TryGetInfusionStats(item, iGlobal.infusedItemName, out int infusedPower, out float damageMultiplier, out int infusedArmorSlot);
            if (returnValue) {
                iGlobal.infusionPower = infusedPower;
                iGlobal.infusionDamageMultiplier = damageMultiplier;
                iGlobal.infusedArmorSlot = infusedArmorSlot;
            }

            return returnValue;
        }
        public static bool TryGetInfusionStats(this Item item, string infusedItemName, out int infusedPower, out float damageMultiplier, out int infusedArmorSlot) {
            infusedPower = 0;
            damageMultiplier = 1f;
            infusedArmorSlot = -1;

            if (!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return false;

            if (infusedItemName != "") {
                int type = 0;
                for (int itemType = 1; itemType < ItemLoader.ItemCount; itemType++) {
                    Item foundItem = new Item(itemType);
                    if (foundItem.Name == infusedItemName) {
                        type = itemType;
                        break;
                    }
                }

                if (type > 0) {
                    GetGlotalItemStats(item, new Item(type), out infusedPower, out damageMultiplier, out infusedArmorSlot);
                    if (EnchantedItemStaticMethods.IsWeaponItem(item)) {
                        //item.UpdateInfusionDamage(damageMultiplier, false);
                        iGlobal.infusionDamageMultiplier = damageMultiplier;
                    }
                    else if (EnchantedItemStaticMethods.IsArmorItem(item)) {
                        item.UpdateArmorSlot(infusedArmorSlot);
                    }

                    return true;
                }
            }

            /*
            if (EnchantedItemStaticMethods.IsWeaponItem(item)) {
                item.UpdateInfusionDamage(damageMultiplier, false);
            }
            else*/ if (EnchantedItemStaticMethods.IsArmorItem(item)) {
                item.UpdateArmorSlot(infusedArmorSlot);
            }
            
            return false;
        }
        /*public static void UpdateInfusionDamage(this Item item, float damageMultiplier, bool updateStats = true) {
            if (!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return;

            if(damageMultiplier != 1f || iGlobal.statModifiers.ContainsKey("damage")) {
                if(damageMultiplier > 0f) {
                    if (iGlobal.statModifiers.ContainsKey("damage")) {
                        //Used for resetting non-finalized items and upgrading items that already have an infused item
                        iGlobal.statModifiers["damage"] = new StatModifier(1f, damageMultiplier);
                    }
                    else {
                        iGlobal.statModifiers.Add("damage", new StatModifier(1f, damageMultiplier));
                    }

                    if (updateStats && Main.netMode < NetmodeID.Server) {
                        if(Main.LocalPlayer.TryGetModPlayer(out WEPlayer wePlayer)) {
                            wePlayer.UpdateItemStats(ref item);
                        }
						else {
                            $"Failed to UpdateInfusionDamage on item: {item.S()} due to Main.LocalPlayer being null.".LogNT(ChatMessagesIDs.FailedUpdateItemStats);
						}
                    }
                }
                else {
                    $"Prevented an issue that would cause your item: {item.S()} to be set to 0 damage.".LogNT(ChatMessagesIDs.PreventInfusionDamageMultLessThan0);
                }
            }
        }*/
        public static void UpdateArmorSlot(this Item item, int infusedArmorSlot) {
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
            if (!getCurrent && item.TryGetEnchantedItem(out EnchantedItem iGlobal) && iGlobal.infusedArmorSlot != -1) {
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
}
