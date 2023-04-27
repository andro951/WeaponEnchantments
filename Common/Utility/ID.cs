using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using WeaponEnchantments.Content.NPCs;
using static Terraria.Localization.GameCulture;

namespace WeaponEnchantments.Common.Utility
{
    public enum DamageClassID {
		Default,
		Generic,
		Melee,
		MeleeNoSpeed,
		Ranged,
		Magic,
		Summon,
		Whip,
		MagicSummonHybrid,
		Throwing,
		Rogue,
		Ki
	} //Located in DamageClassLoader.cs
    public enum EItemType
    {
        None,
        Weapons,
        Armor,
        Accessories,
        FishingPoles,
        Tools
    }
    public enum ArmorSlotSpecificID {
		Head,
		Body,
		Legs
	}

    public enum EnchantmentStat : byte
    {
        None,
        AllForOne,
        AmmoCost,
        AttackSpeed,
        PercentArmorPenetration,
        AutoReuse,
		BonusManaRegen,
		BuffDuration,
		CatastrophicRelease,
        Channel,
        CrateChance,
        CriticalStrikeChance,
        CriticalStrikeDamage,
        Damage,
        DamageAfterDefenses,
		DamageReduction,
		DayEventUpdateRate,
		DayTileUpdateRate,
		DayTimeRate,
		Defense,
        EnemyMaxSpawns,
        EnemySpawnRate,
		FishingEnemySpawnChance,
        FishingPower,
        GodSlayer,
        InfinitePenetration,
        JumpSpeed,
		KiDamage,
		KiRegen,
        Knockback,
        LavaFishing,
        LifeRegen,
        LifeSteal,
        Luck,
        ManaUsage,
        ManaRegen,
        MaxFallSpeed,
		MaxKi,
        MaxLife,
        MaxMinions,
        MaxMP,
		Melee,
		MiningSpeed,
		MovementAcceleration,
        MovementSlowdown,
        MovementSpeed,
        Multishot,
		NightEventUpdateRate,
		NightTileUpdateRate,
		NightTimeRate,
		NPCHitCooldown,
        OneForAll,
		PrideOfTheWeak,
		ProjectileVelocity,
        QuestFishChance,
        Size,
		WingTime,
        WhipRange,
		YoyoStringLength
	}
    public static class ID_Dictionaries
    {
        public static List<EnchantmentStat> WeaponStatDict = new List<EnchantmentStat>() {
            EnchantmentStat.AttackSpeed,
            EnchantmentStat.PercentArmorPenetration,
            EnchantmentStat.AutoReuse,
            EnchantmentStat.CriticalStrikeChance,
            EnchantmentStat.Damage,
            EnchantmentStat.Knockback,
            EnchantmentStat.ManaUsage,
            EnchantmentStat.Size,
        };
    }
    public enum WeaponStat : byte
    {
        AttackSpeed = 3,
        ArmorPenetration,
        AutoReuse,
        CriticalStrikeChance = 7,
        Damage = 9,
        Knockback = 17,
        ManaUsage = 20,
        Size = 33,
    }
    public enum BuffStyle
	{
        OnTickPlayerBuff,
        OnTickPlayerDebuff,
        OnTickAreaTeamBuff,
        OnTickAreaTeamDebuff,
        OnTickEnemyBuff,
        OnTickEnemyDebuff,
        OnTickAreaEnemyBuff,
        OnTickAreaEnemyDebuff,
        OnHitPlayerBuff,
        OnHitPlayerDebuff,
        OnHitEnemyBuff,
        OnHitEnemyDebuff,
        OnHitAreaTeamBuff,
        OnHitAreaTeamDebuff,
        OnHitAreaEnemyBuff,
        OnHitAreaEnemyDebuff,
		All
    }
    public enum PermenantItemFields : short
	{
        
    }
    public enum PermenantItemProperties : short
	{
        ArmorPenetration,
        DamageType
    }

    public enum ChestID
    {
        None = -1,
        Chest_Normal,
        Gold,
        Gold_Locked,
        Shadow,
        Shadow_Locked,
        RichMahogany = 8,
        Ivy = 10,
        Frozen,
        LivingWood,
        Skyware,
        WebCovered = 15,
        Lihzahrd,
        Water,
        Jungle_Dungeon = 23,
        Corruption_Dungeon,
        Crimson_Dungeon,
        Hallowed_Dungeon,
        Ice_Dungeon,
        Mushroom = 32,
        Granite = 40,
        Marble,
        Gold_DeadMans = 104,
        SandStone = 110,
        Desert_Dungeon = 113
    }
    public static class ChestIDMethods
	{
        public static int GetItemType(this ChestID id) {
			switch (id) {
                case ChestID.Chest_Normal:
                    return ItemID.Chest;
                case ChestID.Gold:
					return ItemID.GoldChest;
				case ChestID.Gold_Locked:
					return ItemID.Fake_GoldChest;
                case ChestID.Shadow:
                    return ItemID.ShadowChest;
				case ChestID.Shadow_Locked:
					return ItemID.Fake_ShadowChest;
				case ChestID.RichMahogany:
                    return ItemID.RichMahoganyChest;
                case ChestID.Ivy:
                    return ItemID.IvyChest;
                case ChestID.Frozen:
                    return ItemID.FrozenChest;
                case ChestID.LivingWood:
                    return ItemID.LivingWoodChest;
                case ChestID.Skyware:
                    return ItemID.SkywareChest;
                case ChestID.WebCovered:
                    return ItemID.WebCoveredChest;
                case ChestID.Lihzahrd:
                    return ItemID.LihzahrdChest;
                case ChestID.Water:
                    return ItemID.WaterChest;
                case ChestID.Jungle_Dungeon:
                    return ItemID.JungleChest;
                case ChestID.Corruption_Dungeon:
                    return ItemID.CorruptionChest;
                case ChestID.Crimson_Dungeon:
                    return ItemID.CrimsonChest;
                case ChestID.Hallowed_Dungeon:
                    return ItemID.HallowedChest;
                case ChestID.Ice_Dungeon:
                    return ItemID.IceChest;
                case ChestID.Mushroom:
                    return ItemID.MushroomChest;
                case ChestID.Granite:
                    return ItemID.GraniteChest;
                case ChestID.Marble:
                    return ItemID.MarbleChest;
                case ChestID.Gold_DeadMans:
                    return ItemID.DeadMansChest;
                case ChestID.SandStone:
                    return ItemID.DesertChest;
                case ChestID.Desert_Dungeon:
                    return ItemID.DungeonDesertChest;
                default:
                    return -1;
            }
		}
	}
    public enum CrateID
	{
        None = -1,
        Wooden = ItemID.WoodenCrate,
        Iron = ItemID.IronCrate,
        Golden = ItemID.GoldenCrate,
        Jungle = ItemID.JungleFishingCrate,
        Sky = ItemID.FloatingIslandFishingCrate,
        Corrupt = ItemID.CorruptFishingCrate,
        Crimson = ItemID.CrimsonFishingCrate,
        Hallowed = ItemID.HallowedFishingCrate,
        Dungeon = ItemID.DungeonFishingCrate,
        Frozen = ItemID.FrozenCrate,
        Oasis = ItemID.OasisCrate,
        Obsidian = ItemID.LavaCrate,
        Ocean = ItemID.OceanCrate,
        Pearlwood_WoodenHard = ItemID.WoodenCrateHard,
        Mythril_IronHard = ItemID.IronCrateHard,
        Titanium_GoldenHard = ItemID.GoldenCrateHard,
        Bramble_JungleHard = ItemID.JungleFishingCrateHard,
        Azure_SkyHard = ItemID.FloatingIslandFishingCrateHard,
        Defiled_CorruptHard = ItemID.CorruptFishingCrateHard,
        Hematic_CrimsonHard = ItemID.CrimsonFishingCrateHard,
        Divine_HallowedHard = ItemID.HallowedFishingCrateHard,
        Stockade_DungeonHard = ItemID.DungeonFishingCrateHard,
        Boreal_FrozenHard = ItemID.FrozenCrateHard,
        Mirage_OasisHard = ItemID.OasisCrateHard,
        Hellstone_ObsidianHard = ItemID.LavaCrateHard,
        Seaside_OceanHard = ItemID.OasisCrateHard,

        Golden_LockBox = ItemID.LockBox,
        Obsidian_LockBox = ItemID.ObsidianLockbox
	}
    public enum DashID : byte
	{
        NinjaTabiDash = 1,
        EyeOfCthulhuShieldDash,
        SolarDash,
        CrystalNinjaDash = 5
    }
    public enum SellCondition
	{
		IgnoreCondition,
        Never,
        Always,
        AnyTime,
        AnyTimeRare,
        PostKingSlime,
        PostEyeOfCthulhu,
        PostEaterOfWorldsOrBrainOfCthulhu,
        PostSkeletron,
        PostQueenBee,
        PostDeerclops,
        PostGoblinInvasion,
        Luck,
        HardMode,
        PostQueenSlime,
        PostPirateInvasion,
        PostTwins,
        PostDestroyer,
        PostSkeletronPrime,
        PostPlantera,
        PostGolem,
        PostMartianInvasion,
        PostDukeFishron,
        PostEmpressOfLight,
        PostCultist,
        PostSolarTower,
        PostNebulaTower,
        PostStardustTower,
        PostVortexTower,
        PostMoonLord,
	}
    public static class SellConditionMethods
	{
        public static bool CanSell(this SellCondition condition) {
			switch (condition) {
                case SellCondition.Always:
                case SellCondition.AnyTime:
                case SellCondition.AnyTimeRare:
                    return true;
				case SellCondition.PostKingSlime:
                    return NPC.downedSlimeKing;
                case SellCondition.PostEyeOfCthulhu:
                    return NPC.downedBoss1;
                case SellCondition.PostEaterOfWorldsOrBrainOfCthulhu:
                    return NPC.downedBoss2;
                case SellCondition.PostSkeletron:
                    return NPC.downedBoss3;
                case SellCondition.PostQueenBee:
                    return NPC.downedQueenBee;
                case SellCondition.PostQueenSlime:
                    return NPC.downedQueenSlime;
                case SellCondition.PostNebulaTower:
                    return NPC.downedTowerNebula;
                case SellCondition.PostSolarTower:
                    return NPC.downedTowerSolar;
                case SellCondition.PostStardustTower:
                    return NPC.downedTowerStardust;
                case SellCondition.PostVortexTower:
                    return NPC.downedTowerVortex;
                case SellCondition.PostDeerclops:
                    return NPC.downedDeerclops;
                case SellCondition.PostGoblinInvasion:
                    return NPC.downedGoblins;
                case SellCondition.HardMode:
                    return Main.hardMode;
                case SellCondition.PostGolem:
                    return NPC.downedGolemBoss;
                case SellCondition.PostTwins:
                    return NPC.downedMechBoss1;
                case SellCondition.PostDestroyer:
                    return NPC.downedMechBoss2;
                case SellCondition.PostSkeletronPrime:
                    return NPC.downedMechBoss3;
                case SellCondition.PostPirateInvasion:
                    return NPC.downedPirates;
                case SellCondition.PostPlantera:
                    return NPC.downedPlantBoss;
                case SellCondition.PostEmpressOfLight:
                    return NPC.downedEmpressOfLight;
                case SellCondition.PostDukeFishron:
                    return NPC.downedFishron;
                case SellCondition.PostCultist:
                    return NPC.downedAncientCultist;
                case SellCondition.PostMoonLord:
                    return NPC.downedMoonlord;
				case SellCondition.Never:
                default:
                    return false;
            }
		}
	}
	public enum TownNPCTypeID
	{
        Guide = 22,
        Merchant = 17,
        Nurse = 18,
        Demolitionist = 38,
        Angler = 369,
        Dryad = 20,
        ArmsDealer = 19,
        DyeTrader = 207,
        Painter = 227,
        Stylist = 353,
        Zoologist = 633,
        Tavernkeep = 550,
        Golfer = 588,
        GoblinTinkerer = 107,
        WitchDoctor = 228,
        Mechanic = 124,
        Clothier = 54,
        Wizard = 108,
        Steampunker = 178,
        Pirate = 229,
        Truffle = 160,
        TaxCollector = 441,
        Cyborg = 209,
        PartyGirl = 208,
        Princess = 663,
        SantaClaus = 142,
        Cat = 637,
        Dog = 638,
        Bunny = 656,
        TravelingMerchant = 368,
        SkeletonMerchant = 453,
        OldMan = 37
    }
    public enum L_ID1 {
		Items,
		Tooltip,
		Buffs,
		Dialogue,
        NPCNames,
        Bestiary,
        TownNPCMood,
		Ores,
		TableText,
		EnchantmentStorageText,
		Config
	}
    public enum L_ID2 {
        None,
        Witch,
        EffectDisplayName,
        EnchantmentEffects,
        EnchantmentCustomTooltips,
		EnchantmentTypeNames,
		EnchantmentShortTooltip,
		EnchantmentGeneralTooltips,
		ItemType,
		ArmorSlotNames,
		DamageClassNames,
		VanillaBuffs,
		Header,
		DisplayName
	}
	public enum L_ID3 {
		Label,
		Tooltip
	}
    public enum L_ID_V {
        Item,
        Projectile,
        NPC,
        Buff,
		BuffDescription
	}
	public enum EnchantmentGeneralTooltipsID
	{
		LevelCost,
		Unique,
		Only,
		ArmorSlotOnly,
		NotAllowed,
		Max1,
		Utility,
		Or,
		OnlyAllowedOn,
		AllowedOn,
		And
	}
	public enum TableTextID
	{
		Yes,
		No,
		LootAll,
		Syphon,
		LevelUp,
		xp,
		Offer,
		Infusion,
		Finalize,
		Cancel,
		AreYouSure,
		ExchangeEssence,
		ExchangeOres,
		ExchangeEssenceAndOres,
		Item,
		Enchantments,
		Storage,
		weapon0,
		general1,
		general2,
		general3,
		enchantment0,
		enchantment4,
		utility0,
		essence0,
	}
	public enum EnchantmentStorageTextID
	{
		LootAll,
		DepositAll,
		QuickStack,
		Sort,
		ToggleVacuum,
		ToggleMarkTrash,
		UncraftAllTrash,
		RevertAllToBasic,
		ManageTrash,
		ManageOfferedItems,
		QuickCraft,
		//Do not place anything besides buttons before this
		EnchantmentStorage,
		Search,
		OreBag,
		EnchantmentLoadouts,
		Edit,
		Weapon
	}
	public enum DialogueID {
        StandardDialogue,
        BloodMoon,
        BirthdayParty,
        Storm,
        QueenBee,
        Content,
        NoHome,
        LoveSpace,
        FarFromHome,
        DislikeCrowded,
        HateCrowded,
        LikeBiome,
        LoveBiome,
        DislikeBiome,
        HateBiome,
        LikeNPC,
        LoveNPC,
        DislikeNPC,
        HateNPC
    }
    public enum BiomeID
	{
        Beach,
        Corrupt,
        Crimson,
        Desert,
        DirtLayerHeight,
        Dungeon,
        Forest,
        GemCave,
        Glowshroom,
        Granite,
        Graveyard,
        Hallow,
        Hive,
        Jungle,
        LihzhardTemple,
        Marble,
        Meteor,
        OldOneArmy,
        OverworldHeight,
        PeaceCandle,
        Rain,
        RockLayerHeight,
        Sandstorm,
        SkyHeight,
        Snow,
        TowerNebula,
        TowerStardust,
        TowerSolar,
        TowerVortex,
        UndergroundDesert,
        UnderworldHeight,
        WaterCandle
    }
	public enum DropRestrictionsID
	{
        None,
        HardModeBosses,
		PostPlanteraBosses
	}
    public enum WikiTypeID
	{
        CraftingMaterial,
        Containments,
		CursedEssence,
        EnchantingTables,
        Enchantments,
        EnchantmentEssence,
        Furniture,
        CraftingStation,
        Storage,
        Armor,
        Set,
        Weapon,
        Tool,
        Mechanism,
        LightSource,
        PowerBooster,
		NPC
	}
	public enum InvasionID
	{
		Goblin_Army,
		Pirate_Invasion,
		Martian_Madness
	}

	public static class WikiExtensionMethods
	{
        public static string GetLinkText(this WikiTypeID id, out bool external) {
            external = true;
			switch (id) {
                case WikiTypeID.CraftingMaterial:
                    return "https://terraria.fandom.com/wiki/Category:Crafting_material_items";
                case WikiTypeID.Furniture:
                    return "https://terraria.fandom.com/wiki/Furniture";
                case WikiTypeID.CraftingStation:
                    return "https://terraria.fandom.com/wiki/Crafting_stations";
                case WikiTypeID.Storage:
                    return "https://terraria.fandom.com/wiki/Storage_items";
                case WikiTypeID.Armor:
                    return "https://terraria.fandom.com/wiki/Armor";
                case WikiTypeID.Set:
                    return "https://terraria.fandom.com/wiki/Armor";
                case WikiTypeID.Weapon:
                    return "https://terraria.fandom.com/wiki/Weapons";
                case WikiTypeID.Tool:
                    return "https://terraria.fandom.com/wiki/Tools";
                case WikiTypeID.Mechanism:
                    return "https://terraria.fandom.com/wiki/Mechanisms";
                case WikiTypeID.LightSource:
                    return "https://terraria.fandom.com/wiki/Light_sources";
				case WikiTypeID.NPC:
					return "https://terraria.fandom.com/wiki/NPCs";
                default:
                    external = false;
                    return id.ToString().AddSpaces();
			}
		}
		public static string GetPNGLink(this IShoppingBiome shoppingBiome) {
			switch (shoppingBiome.NameKey) {
				case "Jungle":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a8/Bestiary_The_Jungle.png";
				case "Hallow":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b7/Bestiary_The_Hallow.png";
				case "Dungeon":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/81/Bestiary_The_Dungeon.png";
				case "Corruption":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/ab/Bestiary_The_Corruption.png";
				case "Crimson":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/63/Bestiary_The_Crimson.png";
				case "Glowing Mushroom":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/66/Bestiary_Surface_Mushroom.png";
				case "Snow":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fa/Bestiary_Snow.png";
				case "Ocean":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/aa/Bestiary_Ocean.png";
				case "Desert":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a8/Bestiary_Desert.png";
				case "Underground":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/79/Bestiary_Underground.png";
				case "Cavern":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/52/Bestiary_Caverns.png";
				case "The Underworld":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/30/Bestiary_The_Underworld.png";
				case "Forest":
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/42/Bestiary_Surface.png";
				default:
					return $"{shoppingBiome.NameKey} Not Found";
			}
		}
		public static string GetLinkText(this IShoppingBiome shoppingBiome) {
			return $"https://terraria.fandom.com/wiki/{shoppingBiome.NameKey}";
		}
		public static string ToLanguageName(this CultureName id) {
			switch (id) {
				case CultureName.English:
					return "en-US";
				case CultureName.French:
					return "fr-FR";
				case CultureName.German:
					return "de-DE";
				case CultureName.Italian:
					return "it-IT";
				case CultureName.Spanish:
					return "es-ES";
				case CultureName.Russian:
					return "ru-RU";
				case CultureName.Chinese:
					return "zh-Hans";
				case CultureName.Portuguese:
					return "pt-BR";
				case CultureName.Polish:
					return "pl-PL";
				default:
					return "CultureNameNotFound";
			}
		}
	}
    public enum FloatID
	{
        none,
        left,
        middle,
        right,
	}
	public enum CombineModeID
	{
		Normal,
		MultiplicativePartOf1
	}

	public static class NPCIDMethods {
        public static string GetNPCPNGLink(this int id) {
			switch (id) {
				case NPCID.BigHornetStingy://-65 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/94/Hornet_5.png/revision/latest?cb=20170422125834&format=original";
				case NPCID.LittleHornetStingy://-64 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/94/Hornet_5.png/revision/latest/scale-to-width-down/40?cb=20170422125834&format=original";
				case NPCID.BigHornetSpikey://-63 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2a/Spikey_Hornet.png/revision/latest?cb=20170422125808&format=original";
				case NPCID.LittleHornetSpikey://-62 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2a/Spikey_Hornet.png/revision/latest/scale-to-width-down/34?cb=20170422125808&format=original";
				case NPCID.BigHornetLeafy://-61 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e3/Leafy_Hornet.png/revision/latest?cb=20170422125730&format=original";
				case NPCID.LittleHornetLeafy://-60 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e3/Leafy_Hornet.png/revision/latest/scale-to-width-down/39?cb=20170422125730&format=original";
				case NPCID.BigHornetHoney://-59 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b7/Honey_Hornet.png/revision/latest?cb=20170422125704&format=original";
				case NPCID.LittleHornetHoney://-58 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b7/Honey_Hornet.png/revision/latest/scale-to-width-down/34?cb=20170422125704&format=original";
				case NPCID.BigHornetFatty://-57 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fc/Fatty_Hornet.png/revision/latest?cb=20170422125636&format=original";
				case NPCID.LittleHornetFatty://-56 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fc/Fatty_Hornet.png/revision/latest/scale-to-width-down/36?cb=20170422125636&format=original";
				case NPCID.BigRainZombie://-55 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5c/Raincoat_Zombie.png/revision/latest?cb=20170805211221&format=original";
				case NPCID.SmallRainZombie://-54 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5c/Raincoat_Zombie.png/revision/latest/scale-to-width-down/31?cb=20170805211221&format=original";
				case NPCID.BigPantlessSkeleton://-53 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/be/Pantless_Skeleton.png/revision/latest?cb=20170422124010&format=original";
				case NPCID.SmallPantlessSkeleton://-52 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/be/Pantless_Skeleton.png/revision/latest/scale-to-width-down/29?cb=20170422124010&format=original";
				case NPCID.BigMisassembledSkeleton://-51 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7b/Misassembled_Skeleton.png/revision/latest?cb=20170422123940&format=original";
				case NPCID.SmallMisassembledSkeleton://-50 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7b/Misassembled_Skeleton.png/revision/latest?cb=20170422123940&format=original";
				case NPCID.BigHeadacheSkeleton://-49 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/62/Headache_Skeleton.png/revision/latest?cb=20170422123913&format=original";
				case NPCID.SmallHeadacheSkeleton://-48 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/62/Headache_Skeleton.png/revision/latest/scale-to-width-down/32?cb=20170422123913&format=original";
				case NPCID.BigSkeleton://-47 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/23/Skeleton.png/revision/latest?cb=20170420012637&format=original";
				case NPCID.SmallSkeleton://-46 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/23/Skeleton.png/revision/latest/scale-to-width-down/27?cb=20170420012637&format=original";
				case NPCID.BigFemaleZombie://-45 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a1/Female_Zombie.png/revision/latest?cb=20170422121903&format=original";
				case NPCID.SmallFemaleZombie://-44 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a1/Female_Zombie.png/revision/latest/scale-to-width-down/30?cb=20170422121903&format=original";
				case NPCID.DemonEye2://-43 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9d/Demon_Eye.png/revision/latest?cb=20170420003551&format=original";
				case NPCID.PurpleEye2://-42 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7b/Purple_Eye.png/revision/latest?cb=20170422122804&format=original";
				case NPCID.GreenEye2://-41 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2e/Green_Eye.png/revision/latest/scale-to-width-down/31?cb=20170422122705&format=original";
				case NPCID.DialatedEye2://-40 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/af/Dilated_Eye.png/revision/latest/scale-to-width-down/32?cb=20170422122641&format=original";
				case NPCID.SleepyEye2://-39 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/29/Sleepy_Eye.png/revision/latest?cb=20170422122738&format=original";
				case NPCID.CataractEye2://-38 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/de/Cataract_Eye.png/revision/latest?cb=20170422122610&format=original";
				case NPCID.BigTwiggyZombie://-37 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/75/Twiggy_Zombie.png/revision/latest?cb=20170422121749&format=original";
				case NPCID.SmallTwiggyZombie://-36 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/75/Twiggy_Zombie.png/revision/latest/scale-to-width-down/31?cb=20170422121749&format=original";
				case NPCID.BigSwampZombie://-35 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a7/Swamp_Zombie.png/revision/latest?cb=20170422121752&format=original";
				case NPCID.SmallSwampZombie://-34 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a7/Swamp_Zombie.png/revision/latest/scale-to-width-down/30?cb=20170422121752&format=original";
				case NPCID.BigSlimedZombie://-33 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/57/Slimed_Zombie.png/revision/latest?cb=20161120175703&format=original";
				case NPCID.SmallSlimedZombie://-32 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/57/Slimed_Zombie.png/revision/latest/scale-to-width-down/30?cb=20161120175703&format=original";
				case NPCID.BigPincushionZombie://-31 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/96/Pincushion_Zombie.png/revision/latest?cb=20170422122209&format=original";
				case NPCID.SmallPincushionZombie://-30 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/96/Pincushion_Zombie.png/revision/latest/scale-to-width-down/32?cb=20170422122209&format=original";
				case NPCID.BigBaldZombie://-29 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7a/Bald_Zombie.png/revision/latest?cb=20161120175629&format=original";
				case NPCID.SmallBaldZombie://-28 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7a/Bald_Zombie.png/revision/latest/scale-to-width-down/29?cb=20161120175629&format=original";
				case NPCID.BigZombie://-27 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c3/Zombie.png/revision/latest?cb=20171102011214&format=original";
				case NPCID.SmallZombie://-26 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c3/Zombie.png/revision/latest/scale-to-width-down/31?cb=20171102011214&format=original";
				case NPCID.BigCrimslime://-25 Crimslime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c9/Crimslime.png/revision/latest?cb=20150708221108&format=original";
				case NPCID.LittleCrimslime://-24 Crimslime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c9/Crimslime.png/revision/latest/scale-to-width-down/37?cb=20150708221108&format=original";
				case NPCID.BigCrimera://-23 Crimera
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3c/Crimera.png/revision/latest?cb=20200731032152&format=original";
				case NPCID.LittleCrimera://-22 Crimera
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3c/Crimera.png/revision/latest/scale-to-width-down/32?cb=20200731032152&format=original";
				case NPCID.GiantMossHornet://-21 Moss Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c4/Moss_Hornet.png/revision/latest?cb=20170421222615&format=original";
				case NPCID.BigMossHornet://-20 Moss Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c4/Moss_Hornet.png/revision/latest?cb=20170421222615&format=original";
				case NPCID.LittleMossHornet://-19 Moss Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c4/Moss_Hornet.png/revision/latest/scale-to-width-down/43?cb=20170421222615&format=original";
				case NPCID.TinyMossHornet://-18 Moss Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c4/Moss_Hornet.png/revision/latest/scale-to-width-down/38?cb=20170421222615&format=original";
				case NPCID.BigStinger://-17 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/72/Hornet.png/revision/latest?cb=20170420020349&format=original";
				case NPCID.LittleStinger://-16 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/72/Hornet.png/revision/latest/scale-to-width-down/36?cb=20170420020349&format=original";
				case NPCID.HeavySkeleton://-15 Armored Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7f/Armored_Skeleton.png/revision/latest?cb=20170421211538&format=original";
				case NPCID.BigBoned://-14 Angry Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f6/Angry_Bones_1.png/revision/latest?cb=20200530060826&format=original";
				case NPCID.ShortBones://-13 Angry Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f6/Angry_Bones_1.png/revision/latest/scale-to-width-down/27?cb=20200530060826&format=original";
				case NPCID.BigEater://-12 Eater of Souls
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ea/Eater_of_Souls.png/revision/latest?cb=20170420005422&format=original";
				case NPCID.LittleEater://-11 Eater of Souls
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ea/Eater_of_Souls.png/revision/latest/scale-to-width-down/36?cb=20170420005422&format=original";
				case NPCID.JungleSlime://-10 Jungle Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5d/Jungle_Slime.png/revision/latest?cb=20200730134909&format=original";
				case NPCID.YellowSlime://-9 Yellow Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c4/Yellow_Slime.png/revision/latest?cb=20160925081607&format=original";
				case NPCID.RedSlime://-8 Red Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/87/Red_Slime.png/revision/latest?cb=20110828163005&format=original";
				case NPCID.PurpleSlime://-7 Purple Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/27/Purple_Slime.png/revision/latest?cb=20160925081604&format=original";
				case NPCID.BlackSlime://-6 Black Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5c/Black_Slime.png/revision/latest?cb=20110828163020&format=original";
				case NPCID.BabySlime://-5 Baby Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6d/Baby_Slime.png/revision/latest?cb=20170121233645&format=original";
				case NPCID.Pinky://-4 Pinky
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/17/Pinky.png/revision/latest?cb=20111003022635&format=original";
				case NPCID.GreenSlime://-3 Green Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7b/Green_Slime.png/revision/latest?cb=20141106201737&format=original";
				case NPCID.Slimer2://-2 Slimer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/45/Corrupt_Slime.png/revision/latest/scale-to-width-down/40?cb=20171130024032&format=original";
				case NPCID.Slimeling://-1 Slimeling
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ee/Slimeling.png/revision/latest?cb=20161002152710&format=original";
				case NPCID.None://0 
					return "";
				case NPCID.BlueSlime://1 Blue Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/da/Blue_Slime.png/revision/latest?cb=20110828163020&format=original";
				case NPCID.DemonEye://2 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9d/Demon_Eye.png/revision/latest?cb=20170420003551&format=original";
				case NPCID.Zombie://3 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c3/Zombie.png/revision/latest?cb=20171102011214&format=original";
				case NPCID.EyeofCthulhu://4 Eye of Cthulhu
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/70/Eye_of_Cthulhu_%28Phase_1%29.gif/revision/latest?cb=20211114181102&format=original";
				case NPCID.ServantofCthulhu://5 Servant of Cthulhu
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/63/Servant_of_Cthulhu.png/revision/latest?cb=20170420005232&format=original";
				case NPCID.EaterofSouls://6 Eater of Souls
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f5/Eater_of_Souls.gif/revision/latest?cb=20170420010950&format=original";
				case NPCID.DevourerHead://7 Devourer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/25/Devourer_Head.png/revision/latest?cb=20170420005727&format=original";
				case NPCID.DevourerBody://8 Devourer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6a/Devourer_Body.png/revision/latest?cb=20170420005751&format=original";
				case NPCID.DevourerTail://9 Devourer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/77/Devourer_Tail.png/revision/latest?cb=20170420005829&format=original";
				case NPCID.GiantWormHead://10 Giant Worm
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b3/Giant_Worm_Head.png/revision/latest?cb=20170420005945&format=original";
				case NPCID.GiantWormBody://11 Giant Worm
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f6/Giant_Worm_Body.png/revision/latest?cb=20170420010026&format=original";
				case NPCID.GiantWormTail://12 Giant Worm
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3b/Giant_Worm_Tail.png/revision/latest?cb=20170420010057&format=original";
				case NPCID.EaterofWorldsHead://13 Eater of Worlds
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0f/Eater_of_Worlds_Head.png/revision/latest?cb=20170420010144&format=original";
				case NPCID.EaterofWorldsBody://14 Eater of Worlds
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0a/Eater_of_Worlds_Body.png/revision/latest?cb=20170420010209&format=original";
				case NPCID.EaterofWorldsTail://15 Eater of Worlds
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/92/Eater_of_Worlds_Tail.png/revision/latest?cb=20170420010233&format=original";
				case NPCID.MotherSlime://16 Mother Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5c/Mother_Slime.png/revision/latest?cb=20160925081648&format=original";
				case NPCID.Merchant://17 Merchant
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/19/Merchant.png/revision/latest?cb=20211003230931&format=original";
				case NPCID.Nurse://18 Nurse
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/cc/Nurse.png/revision/latest?cb=20161005060102&format=original";
				case NPCID.ArmsDealer://19 Arms Dealer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9e/Arms_Dealer.png/revision/latest?cb=20161004000744&format=original";
				case NPCID.Dryad://20 Dryad
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5c/Dryad.png/revision/latest?cb=20161004000507&format=original";
				case NPCID.Skeleton://21 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/23/Skeleton.png/revision/latest?cb=20170420012637&format=original";
				case NPCID.Guide://22 Guide
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7f/Guide.png/revision/latest?cb=20191003231144&format=original";
				case NPCID.MeteorHead://23 Meteor Head
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a1/Meteor_Head.png/revision/latest?cb=20170420013145&format=original";
				case NPCID.FireImp://24 Fire Imp
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/af/Fire_Imp.png/revision/latest?cb=20170420013443&format=original";
				case NPCID.BurningSphere://25 Burning Sphere
					return "";
				case NPCID.GoblinPeon://26 Goblin Peon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/61/Goblin_Peon.png/revision/latest?cb=20200518224850&format=original";
				case NPCID.GoblinThief://27 Goblin Thief
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/09/Goblin_Thief.png/revision/latest?cb=20200518230423&format=original";
				case NPCID.GoblinWarrior://28 Goblin Warrior
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b4/Goblin_Warrior.png/revision/latest?cb=20200518230412&format=original";
				case NPCID.GoblinSorcerer://29 Goblin Sorcerer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/43/Goblin_Sorcerer.png/revision/latest?cb=20200518230446&format=original";
				case NPCID.ChaosBall://30 Chaos Ball
					return "";
				case NPCID.AngryBones://31 Angry Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f6/Angry_Bones_1.png/revision/latest?cb=20200530060826&format=original";
				case NPCID.DarkCaster://32 Dark Caster
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c1/Dark_Caster.png/revision/latest?cb=20171104013247&format=original";
				case NPCID.WaterSphere://33 Water Sphere
					return "";
				case NPCID.CursedSkull://34 Cursed Skull
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/40/Cursed_Skull.png/revision/latest?cb=20200731025412&format=original";
				case NPCID.SkeletronHead://35 Skeletron
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a3/Skeletron_Head.png/revision/latest?cb=20191003231538&format=original";
				case NPCID.SkeletronHand://36 Skeletron
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0a/Skeletron_Hand_%28NPC%29.png/revision/latest?cb=20170420014655&format=original";
				case NPCID.OldMan://37 Old Man
					return "";
				case NPCID.Demolitionist://38 Demolitionist
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6e/Demolitionist.png/revision/latest?cb=20200330043525&format=original";
				case NPCID.BoneSerpentHead://39 Bone Serpent
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1c/Bone_Serpent_Head.png/revision/latest?cb=20170420015651&format=original";
				case NPCID.BoneSerpentBody://40 Bone Serpent
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3d/Bone_Serpent_Body.png/revision/latest?cb=20170420015713&format=original";
				case NPCID.BoneSerpentTail://41 Bone Serpent
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/05/Bone_Serpent_Tail.png/revision/latest?cb=20170420015737&format=original";
				case NPCID.Hornet://42 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/72/Hornet.png/revision/latest?cb=20170420020349&format=original";
				case NPCID.ManEater://43 Man Eater
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a5/Man_Eater.png/revision/latest?cb=20170420020846&format=original";
				case NPCID.UndeadMiner://44 Undead Miner
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/73/Undead_Miner.png/revision/latest?cb=20170420021204&format=original";
				case NPCID.Tim://45 Tim
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ea/Tim.png/revision/latest?cb=20171104013044&format=original";
				case NPCID.Bunny://46 Bunny
					return "";
				case NPCID.CorruptBunny://47 Corrupt Bunny
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/42/Corrupt_Bunny.png/revision/latest?cb=20160929095411&format=original";
				case NPCID.Harpy://48 Harpy
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1b/Harpy.png/revision/latest?cb=20200603151211&format=original";
				case NPCID.CaveBat://49 Cave Bat
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0d/Cave_Bat.gif/revision/latest?cb=20211209033748&format=original";
				case NPCID.KingSlime://50 King Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/93/King_Slime.gif/revision/latest?cb=20200523113558&format=original";
				case NPCID.JungleBat://51 Jungle Bat
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/94/Jungle_Bat.png/revision/latest?cb=20161130084556&format=original";
				case NPCID.DoctorBones://52 Doctor Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/24/Doctor_Bones.png/revision/latest?cb=20170420102814&format=original";
				case NPCID.TheGroom://53 The Groom
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/cd/The_Groom.png/revision/latest?cb=20170420102844&format=original";
				case NPCID.Clothier://54 Clothier
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d2/Clothier.png/revision/latest?cb=20161009093143&format=original";
				case NPCID.Goldfish://55 Goldfish
					return "";
				case NPCID.Snatcher://56 Snatcher
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2b/Snatcher.png/revision/latest?cb=20170420104001&format=original";
				case NPCID.CorruptGoldfish://57 Corrupt Goldfish
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6e/Corrupt_Goldfish.png/revision/latest?cb=20161012150337&format=original";
				case NPCID.Piranha://58 Piranha
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/43/Piranha.png/revision/latest?cb=20161126013943&format=original";
				case NPCID.LavaSlime://59 Lava Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c5/Lava_Slime.png/revision/latest?cb=20160925082050&format=original";
				case NPCID.Hellbat://60 Hellbat
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/26/Hellbat.png/revision/latest?cb=20161113060925&format=original";
				case NPCID.Vulture://61 Vulture
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c7/Vulture_%28flying%29.png/revision/latest?cb=20210725202149&format=original";
				case NPCID.Demon://62 Demon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c4/Demon.png/revision/latest?cb=20200526231750&format=original";
				case NPCID.BlueJellyfish://63 Blue Jellyfish
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/98/Blue_Jellyfish.png/revision/latest?cb=20200803163235&format=original";
				case NPCID.PinkJellyfish://64 Pink Jellyfish
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5d/Pink_Jellyfish.png/revision/latest?cb=20200517031622&format=original";
				case NPCID.Shark://65 Shark
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/14/Shark.png/revision/latest?cb=20170420104945&format=original";
				case NPCID.VoodooDemon://66 Voodoo Demon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/00/Voodoo_Demon.png/revision/latest?cb=20200517031810&format=original";
				case NPCID.Crab://67 Crab
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/63/Crab.png/revision/latest?cb=20170420105630&format=original";
				case NPCID.DungeonGuardian://68 Dungeon Guardian
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a4/Dungeon_Guardian.png/revision/latest?cb=20150227080514&format=original";
				case NPCID.Antlion://69 Antlion
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/ff/Antlion.png/revision/latest?cb=20191128180152&format=original";
				case NPCID.SpikeBall://70 Spike Ball
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9f/Spike_Ball.png/revision/latest?cb=20200807032629&format=original";
				case NPCID.DungeonSlime://71 Dungeon Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/50/Dungeon_Slime_%28Key%29.png/revision/latest?cb=20210430065342&format=original";
				case NPCID.BlazingWheel://72 Blazing Wheel
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/65/Blazing_Wheel.png/revision/latest?cb=20170421211040&format=original";
				case NPCID.GoblinScout://73 Goblin Scout
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/27/Goblin_Scout.png/revision/latest?cb=20200517031944&format=original";
				case NPCID.Bird://74 Bird
					return "";
				case NPCID.Pixie://75 Pixie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/22/Pixie.png/revision/latest?cb=20170421211325&format=original";
				case NPCID.None2://76 NPCName.None2
					return "";
				case NPCID.ArmoredSkeleton://77 Armored Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7f/Armored_Skeleton.png/revision/latest?cb=20170421211538&format=original";
				case NPCID.Mummy://78 Mummy
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/70/Mummy.png/revision/latest?cb=20211207225057&format=original";
				case NPCID.DarkMummy://79 Dark Mummy
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f1/Dark_Mummy_%28old%29.png/revision/latest?cb=20210205225723&format=original";
				case NPCID.LightMummy://80 Light Mummy
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2a/Light_Mummy_%28old%29.png/revision/latest?cb=20210205225744&format=original";
				case NPCID.CorruptSlime://81 Corrupt Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/45/Corrupt_Slime.png/revision/latest?cb=20171130024032&format=original";
				case NPCID.Wraith://82 Wraith
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7c/Wraith.png/revision/latest?cb=20191128174013&format=original";
				case NPCID.CursedHammer://83 Cursed Hammer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/77/Cursed_Hammer.png/revision/latest?cb=20170420181531&format=original";
				case NPCID.EnchantedSword://84 Enchanted Sword
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ed/Enchanted_Sword_%28NPC%29.gif/revision/latest?cb=20180309231258&format=original";
				case NPCID.Mimic://85 Mimic
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a8/Mimic.png/revision/latest?cb=20170421213035&format=original";
				case NPCID.Unicorn://86 Unicorn
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7e/Unicorn.png/revision/latest?cb=20200821032944&format=original";
				case NPCID.WyvernHead://87 Wyvern
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1d/Wyvern_Head.png/revision/latest?cb=20170421215825&format=original";
				case NPCID.WyvernLegs://88 Wyvern
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a0/Wyvern_Legs.png/revision/latest?cb=20170421215904&format=original";
				case NPCID.WyvernBody://89 Wyvern
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/10/Wyvern_Body.png/revision/latest?cb=20170421220400&format=original";
				case NPCID.WyvernBody2://90 Wyvern
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/82/Wyvern_Body_2.png/revision/latest?cb=20170421220323&format=original";
				case NPCID.WyvernBody3://91 Wyvern
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/aa/Wyvern_Body_3.png/revision/latest?cb=20170421220433&format=original";
				case NPCID.WyvernTail://92 Wyvern
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/35/Wyvern_Tail.png/revision/latest?cb=20170421220524&format=original";
				case NPCID.GiantBat://93 Giant Bat
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3f/Giant_Bat.png/revision/latest?cb=20191128181401&format=original";
				case NPCID.Corruptor://94 Corruptor
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0e/Corruptor.gif/revision/latest?cb=20211216225509&format=original";
				case NPCID.DiggerHead://95 Digger
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3f/Digger_Head.png/revision/latest?cb=20170421222702&format=original";
				case NPCID.DiggerBody://96 Digger
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/91/Digger_Body.png/revision/latest?cb=20170421222727&format=original";
				case NPCID.DiggerTail://97 Digger
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/27/Digger_Tail.png/revision/latest?cb=20170421222751&format=original";
				case NPCID.SeekerHead://98 World Feeder
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c8/World_Feeder_Head.png/revision/latest?cb=20170421223257&format=original";
				case NPCID.SeekerBody://99 World Feeder
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e8/World_Feeder_Body.png/revision/latest?cb=20170421223327&format=original";
				case NPCID.SeekerTail://100 World Feeder
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a2/World_Feeder_Tail.png/revision/latest?cb=20170421223351&format=original";
				case NPCID.Clinger://101 Clinger
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/34/Clinger.png/revision/latest?cb=20170421224440&format=original";
				case NPCID.AnglerFish://102 Angler Fish
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/af/Angler_Fish.png/revision/latest?cb=20170421222605&format=original";
				case NPCID.GreenJellyfish://103 Green Jellyfish
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b0/Green_Jellyfish.png/revision/latest?cb=20200517031702&format=original";
				case NPCID.Werewolf://104 Werewolf
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f5/Werewolf.png/revision/latest?cb=20170420210112&format=original";
				case NPCID.BoundGoblin://105 Bound Goblin
					return "";
				case NPCID.BoundWizard://106 Bound Wizard
					return "";
				case NPCID.GoblinTinkerer://107 Goblin Tinkerer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/86/Goblin_Tinkerer.png/revision/latest?cb=20150705070124&format=original";
				case NPCID.Wizard://108 Wizard
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c7/Wizard.png/revision/latest?cb=20151018113651&format=original";
				case NPCID.Clown://109 Clown
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/ff/Clown.png/revision/latest?cb=20170421224900&format=original";
				case NPCID.SkeletonArcher://110 Skeleton Archer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/da/Skeleton_Archer.png/revision/latest?cb=20170421225035&format=original";
				case NPCID.GoblinArcher://111 Goblin Archer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d4/Goblin_Archer.png/revision/latest?cb=20200517032400&format=original";
				case NPCID.VileSpit://112 Vile Spit
					return "";
				case NPCID.WallofFlesh://113 Wall of Flesh
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fd/Wall_of_Flesh_Mouth.png/revision/latest?cb=20170422001243&format=original";
				case NPCID.WallofFleshEye://114 Wall of Flesh
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/01/Wall_of_Flesh_Eye.png/revision/latest?cb=20170422001207&format=original";
				case NPCID.TheHungry://115 The Hungry
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/80/The_Hungry.png/revision/latest?cb=20210728032344&format=original";
				case NPCID.TheHungryII://116 The Hungry
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a1/The_Hungry_II.png/revision/latest?cb=20210728032519&format=original";
				case NPCID.LeechHead://117 Leech
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8f/Leech_Head.png/revision/latest?cb=20170422001834&format=original";
				case NPCID.LeechBody://118 Leech
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/90/Leech_Body.png/revision/latest?cb=20170422001857&format=original";
				case NPCID.LeechTail://119 Leech
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4b/Leech_Tail.png/revision/latest?cb=20170422001928&format=original";
				case NPCID.ChaosElemental://120 Chaos Elemental
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a9/Chaos_Elemental.png/revision/latest?cb=20170421223414&format=original";
				case NPCID.Slimer://121 Slimer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c6/Slimer.png/revision/latest?cb=20170422002239&format=original";
				case NPCID.Gastropod://122 Gastropod
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/73/Gastropod.png/revision/latest?cb=20211210033410&format=original";
				case NPCID.BoundMechanic://123 Bound Mechanic
					return "";
				case NPCID.Mechanic://124 Mechanic
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/55/Mechanic.png/revision/latest?cb=20151018120500&format=original";
				case NPCID.Retinazer://125 Retinazer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/af/Retinazer_%28Second_Form%29.png/revision/latest?cb=20170421155457&format=original";
				case NPCID.Spazmatism://126 Spazmatism
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b9/Spazmatism_%28Second_Form%29.png/revision/latest?cb=20220227045610&format=original";
				case NPCID.SkeletronPrime://127 Skeletron Prime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/60/Skeletron_Prime.png/revision/latest?cb=20170928200428&format=original";
				case NPCID.PrimeCannon://128 Prime Cannon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5e/Prime_Cannon.png/revision/latest?cb=20170421151706&format=original";
				case NPCID.PrimeSaw://129 Prime Saw
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3b/Prime_Saw.png/revision/latest?cb=20170421151701&format=original";
				case NPCID.PrimeVice://130 Prime Vice
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/30/Prime_Vice.png/revision/latest?cb=20170421151703&format=original";
				case NPCID.PrimeLaser://131 Prime Laser
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1e/Prime_Laser.png/revision/latest?cb=20170421151658&format=original";
				case NPCID.BaldZombie://132 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7a/Bald_Zombie.png/revision/latest?cb=20161120175629&format=original";
				case NPCID.WanderingEye://133 Wandering Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a1/Wandering_Eye.png/revision/latest?cb=20161120175931&format=original";
				case NPCID.TheDestroyer://134 The Destroyer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/27/The_Destroyer_Head.png/revision/latest?cb=20150708221902&format=original";
				case NPCID.TheDestroyerBody://135 The Destroyer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/20/The_Destroyer_Body.png/revision/latest?cb=20150708221907&format=original";
				case NPCID.TheDestroyerTail://136 The Destroyer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b9/The_Destroyer_Tail.png/revision/latest?cb=20150708221909&format=original";
				case NPCID.IlluminantBat://137 Illuminant Bat
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2f/Illuminant_Bat.gif/revision/latest?cb=20211115004135&format=original";
				case NPCID.IlluminantSlime://138 Illuminant Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/55/Illuminant_Slime.png/revision/latest?cb=20171130024631&format=original";
				case NPCID.Probe://139 Probe
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/da/Probe.png/revision/latest?cb=20150708201700&format=original";
				case NPCID.PossessedArmor://140 Possessed Armor
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/55/Possessed_Armor.png/revision/latest?cb=20140526012145&format=original";
				case NPCID.ToxicSludge://141 Toxic Sludge
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/cf/Toxic_Sludge.gif/revision/latest?cb=20200808162634&format=original";
				case NPCID.SantaClaus://142 Santa Claus
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/58/Santa_Claus.png/revision/latest?cb=20201013025452&format=original";
				case NPCID.SnowmanGangsta://143 Snowman Gangsta
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7a/Snowman_Gangsta.png/revision/latest?cb=20210620022429&format=original";
				case NPCID.MisterStabby://144 Mister Stabby
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e5/Mister_Stabby.png/revision/latest?cb=20170422003215&format=original";
				case NPCID.SnowBalla://145 Snow Balla
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/dc/Snow_Balla.png/revision/latest?cb=20170422003414&format=original";
				case NPCID.None3://146 NPCName.None3
					return "";
				case NPCID.IceSlime://147 Ice Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5a/Ice_Slime.png/revision/latest?cb=20170422004036&format=original";
				case NPCID.Penguin://148 Penguin
					return "";
				case NPCID.PenguinBlack://149 Penguin
					return "";
				case NPCID.IceBat://150 Ice Bat
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8f/Ice_Bat.png/revision/latest?cb=20170420001843&format=original";
				case NPCID.Lavabat://151 Lava Bat
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c5/Lava_Bat.png/revision/latest?cb=20170420001653&format=original";
				case NPCID.GiantFlyingFox://152 Giant Flying Fox
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/44/Giant_Flying_Fox.png/revision/latest?cb=20170421020011&format=original";
				case NPCID.GiantTortoise://153 Giant Tortoise
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/03/Giant_Tortoise.png/revision/latest?cb=20170421020231&format=original";
				case NPCID.IceTortoise://154 Ice Tortoise
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f1/Ice_Tortoise.png/revision/latest?cb=20170421020410&format=original";
				case NPCID.Wolf://155 Wolf
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c8/Wolf.png/revision/latest?cb=20200517032614&format=original";
				case NPCID.RedDevil://156 Red Devil
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/06/Red_Devil.png/revision/latest?cb=20170422150043&format=original";
				case NPCID.Arapaima://157 Arapaima
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/06/Arapaima.png/revision/latest?cb=20170422005426&format=original";
				case NPCID.VampireBat://158 Vampire
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/89/Vampire_Bat.png/revision/latest?cb=20170421221803&format=original";
				case NPCID.Vampire://159 Vampire
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4e/Vampire.png/revision/latest?cb=20170422005918&format=original";
				case NPCID.Truffle://160 Truffle
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f2/Truffle.png/revision/latest?cb=20200704192524&format=original";
				case NPCID.ZombieEskimo://161 Frozen Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/cf/Frozen_Zombie.png/revision/latest?cb=20170422010132&format=original";
				case NPCID.Frankenstein://162 Frankenstein
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d9/Frankenstein.png/revision/latest?cb=20220304235913&format=original";
				case NPCID.BlackRecluse://163 Black Recluse
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a9/Black_Recluse_%28ground%29.png/revision/latest?cb=20191019095158&format=original";
				case NPCID.WallCreeper://164 Wall Creeper
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8c/Wall_Creeper_%28ground%29.png/revision/latest?cb=20210620011544&format=original";
				case NPCID.WallCreeperWall://165 Wall Creeper
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/89/Wall_Creeper.png/revision/latest?cb=20200804000512&format=original";
				case NPCID.SwampThing://166 Swamp Thing
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fe/Swamp_Thing.png/revision/latest?cb=20191019095727&format=original";
				case NPCID.UndeadViking://167 Undead Viking
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/83/Undead_Viking.png/revision/latest?cb=20220305001117&format=original";
				case NPCID.CorruptPenguin://168 Corrupt Penguin
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/da/Corrupt_Penguin.png/revision/latest?cb=20200730150902&format=original";
				case NPCID.IceElemental://169 Ice Elemental
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/35/Ice_Elemental.png/revision/latest?cb=20170422120617&format=original";
				case NPCID.PigronCorruption://170 Pigron
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/18/Corruption_Pigron.png/revision/latest?cb=20150714194850&format=original";
				case NPCID.PigronHallow://171 Pigron
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8e/Hallow_Pigron.png/revision/latest?cb=20150714194917&format=original";
				case NPCID.RuneWizard://172 Rune Wizard
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e3/Rune_Wizard.png/revision/latest?cb=20170524102422&format=original";
				case NPCID.Crimera://173 Crimera
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4d/Crimera.gif/revision/latest?cb=20211216224331&format=original";
				case NPCID.Herpling://174 Herpling
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5e/Herpling.png/revision/latest?cb=20170421162114&format=original";
				case NPCID.AngryTrapper://175 Angry Trapper
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a6/Angry_Trapper.png/revision/latest?cb=20200808223845&format=original";
				case NPCID.MossHornet://176 Moss Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c4/Moss_Hornet.png/revision/latest?cb=20170421222615&format=original";
				case NPCID.Derpling://177 Derpling
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/34/Derpling.png/revision/latest?cb=20170421203737&format=original";
				case NPCID.Steampunker://178 Steampunker
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/82/Steampunker.png/revision/latest?cb=20200702150220&format=original";
				case NPCID.CrimsonAxe://179 Crimson Axe
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/df/Crimson_Axe.png/revision/latest?cb=20170420181950&format=original";
				case NPCID.PigronCrimson://180 Pigron
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/75/Crimson_Pigron.png/revision/latest?cb=20210210223359&format=original";
				case NPCID.FaceMonster://181 Face Monster
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f6/Face_Monster.png/revision/latest?cb=20170421204044&format=original";
				case NPCID.FloatyGross://182 Floaty Gross
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/87/Floaty_Gross.png/revision/latest?cb=20170422121522&format=original";
				case NPCID.Crimslime://183 Crimslime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c9/Crimslime.png/revision/latest?cb=20150708221108&format=original";
				case NPCID.SpikedIceSlime://184 Spiked Ice Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/bc/Spiked_Ice_Slime.png/revision/latest?cb=20170422121847&format=original";
				case NPCID.SnowFlinx://185 Snow Flinx
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/cb/Snow_Flinx.png/revision/latest?cb=20170421233536&format=original";
				case NPCID.PincushionZombie://186 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/96/Pincushion_Zombie.png/revision/latest?cb=20170422122209&format=original";
				case NPCID.SlimedZombie://187 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/57/Slimed_Zombie.png/revision/latest?cb=20161120175703&format=original";
				case NPCID.SwampZombie://188 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a7/Swamp_Zombie.png/revision/latest?cb=20170422121752&format=original";
				case NPCID.TwiggyZombie://189 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/75/Twiggy_Zombie.png/revision/latest?cb=20170422121749&format=original";
				case NPCID.CataractEye://190 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/de/Cataract_Eye.png/revision/latest?cb=20170422122610&format=original";
				case NPCID.SleepyEye://191 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/29/Sleepy_Eye.png/revision/latest?cb=20170422122738&format=original";
				case NPCID.DialatedEye://192 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/af/Dilated_Eye.png/revision/latest?cb=20170422122641&format=original";
				case NPCID.GreenEye://193 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2e/Green_Eye.png/revision/latest?cb=20170422122705&format=original";
				case NPCID.PurpleEye://194 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7b/Purple_Eye.png/revision/latest?cb=20170422122804&format=original";
				case NPCID.LostGirl://195 Lost Girl
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b3/Lost_Girl.png/revision/latest?cb=20170421233129&format=original";
				case NPCID.Nymph://196 Nymph
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/62/Nymph.png/revision/latest?cb=20170421233126&format=original";
				case NPCID.ArmoredViking://197 Armored Viking
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e3/Armored_Viking.png/revision/latest?cb=20220305001234&format=original";
				case NPCID.Lihzahrd://198 Lihzahrd
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ee/Lihzahrd.png/revision/latest?cb=20170422123231&format=original";
				case NPCID.LihzahrdCrawler://199 Lihzahrd
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c5/Lihzahrd_%28crawler%29.png/revision/latest?cb=20211222232440&format=original";
				case NPCID.FemaleZombie://200 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a1/Female_Zombie.png/revision/latest?cb=20170422121903&format=original";
				case NPCID.HeadacheSkeleton://201 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/62/Headache_Skeleton.png/revision/latest?cb=20170422123913&format=original";
				case NPCID.MisassembledSkeleton://202 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7b/Misassembled_Skeleton.png/revision/latest?cb=20170422123940&format=original";
				case NPCID.PantlessSkeleton://203 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/be/Pantless_Skeleton.png/revision/latest?cb=20170422124010&format=original";
				case NPCID.SpikedJungleSlime://204 Spiked Jungle Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fe/Spiked_Jungle_Slime.png/revision/latest?cb=20171130030805&format=original";
				case NPCID.Moth://205 Moth
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/09/Moth.png/revision/latest?cb=20170422123155&format=original";
				case NPCID.IcyMerman://206 Icy Merman
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4f/Icy_Merman.png/revision/latest?cb=20170421020536&format=original";
				case NPCID.DyeTrader://207 Dye Trader
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/51/Dye_Trader.png/revision/latest?cb=20161009093013&format=original";
				case NPCID.PartyGirl://208 Party Girl
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a8/Party_Girl.png/revision/latest?cb=20161130010012&format=original";
				case NPCID.Cyborg://209 Cyborg
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a3/Cyborg.png/revision/latest?cb=20161004001101&format=original";
				case NPCID.Bee://210 Bee
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/56/Bee.png/revision/latest?cb=20170422124353&format=original";
				case NPCID.BeeSmall://211 Bee
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b3/Bee_%28Small%29.png/revision/latest?cb=20170422124331&format=original";
				case NPCID.PirateDeckhand://212 Pirate Deckhand
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b8/Pirate_Deckhand.png/revision/latest?cb=20210625214940&format=original";
				case NPCID.PirateCorsair://213 Pirate Corsair
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e5/Pirate_Corsair.png/revision/latest?cb=20210625214836&format=original";
				case NPCID.PirateDeadeye://214 Pirate Deadeye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7b/Pirate_Deadeye.png/revision/latest?cb=20170421234512&format=original";
				case NPCID.PirateCrossbower://215 Pirate Crossbower
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7b/Pirate_Crossbower.png/revision/latest?cb=20170421234501&format=original";
				case NPCID.PirateCaptain://216 Pirate Captain
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/37/Pirate_Captain.png/revision/latest?cb=20210914050020&format=original";
				case NPCID.CochinealBeetle://217 Cochineal Beetle
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9b/Cochineal_Beetle.png/revision/latest?cb=20200523235211&format=original";
				case NPCID.CyanBeetle://218 Cyan Beetle
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6c/Cyan_Beetle.png/revision/latest?cb=20200523235324&format=original";
				case NPCID.LacBeetle://219 Lac Beetle
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/61/Lac_Beetle.png/revision/latest?cb=20200808224342&format=original";
				case NPCID.SeaSnail://220 Sea Snail
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/41/Sea_Snail.png/revision/latest?cb=20171104195216&format=original";
				case NPCID.Squid://221 Squid
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/81/Squid.png/revision/latest?cb=20140905021230&format=original";
				case NPCID.QueenBee://222 Queen Bee
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/60/Queen_Bee.gif/revision/latest?cb=20200523215116&format=original";
				case NPCID.ZombieRaincoat://223 Raincoat Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5c/Raincoat_Zombie.png/revision/latest?cb=20170805211221&format=original";
				case NPCID.FlyingFish://224 Flying Fish
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/bc/Flying_Fish.png/revision/latest?cb=20170305130407&format=original";
				case NPCID.UmbrellaSlime://225 Umbrella Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d9/Umbrella_Slime.png/revision/latest?cb=20160726020032&format=original";
				case NPCID.FlyingSnake://226 Flying Snake
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/82/Flying_Snake.png/revision/latest?cb=20161024041552&format=original";
				case NPCID.Painter://227 Painter
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/24/Painter.png/revision/latest?cb=20150705103620&format=original";
				case NPCID.WitchDoctor://228 Witch Doctor
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/ac/Witch_Doctor.png/revision/latest?cb=20170108122024&format=original";
				case NPCID.Pirate://229 Pirate
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7d/Pirate.png/revision/latest?cb=20170421220847&format=original";
				case NPCID.GoldfishWalker://230 Goldfish
					return "";
				case NPCID.HornetFatty://231 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fc/Fatty_Hornet.png/revision/latest?cb=20170422125636&format=original";
				case NPCID.HornetHoney://232 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b7/Honey_Hornet.png/revision/latest?cb=20170422125704&format=original";
				case NPCID.HornetLeafy://233 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e3/Leafy_Hornet.png/revision/latest?cb=20170422125730&format=original";
				case NPCID.HornetSpikey://234 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2a/Spikey_Hornet.png/revision/latest?cb=20170422125808&format=original";
				case NPCID.HornetStingy://235 Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/94/Hornet_5.png/revision/latest?cb=20170422125834&format=original";
				case NPCID.JungleCreeper://236 Jungle Creeper
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9e/Jungle_Creeper_%28ground%29.png/revision/latest?cb=20201014050951&format=original";
				case NPCID.JungleCreeperWall://237 Jungle Creeper
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e9/Jungle_Creeper.png/revision/latest?cb=20201014050936&format=original";
				case NPCID.BlackRecluseWall://238 Black Recluse
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/26/Black_Recluse.png/revision/latest?cb=20210620010827&format=original";
				case NPCID.BloodCrawler://239 Blood Crawler
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/81/Blood_Crawler_%28ground%29.png/revision/latest?cb=20200521181748&format=original";
				case NPCID.BloodCrawlerWall://240 Blood Crawler
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7e/Blood_Crawler.png/revision/latest?cb=20200804000419&format=original";
				case NPCID.BloodFeeder://241 Blood Feeder
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/69/Blood_Feeder.png/revision/latest?cb=20200521181749&format=original";
				case NPCID.BloodJelly://242 Blood Jelly
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7c/Blood_Jelly.png/revision/latest?cb=20200803162612&format=original";
				case NPCID.IceGolem://243 Ice Golem
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8a/Ice_Golem.png/revision/latest?cb=20200708133205&format=original";
				case NPCID.RainbowSlime://244 Rainbow Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/87/Rainbow_Slime.gif/revision/latest?cb=20210921162633&format=original";
				case NPCID.Golem://245 Golem
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/37/Golem_Core.png/revision/latest?cb=20200517202317&format=original";
				case NPCID.GolemHead://246 Golem Head
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9f/Golem_Head.png/revision/latest?cb=20200602093043&format=original";
				case NPCID.GolemFistLeft://247 Golem Fist
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f5/Left_Golem_Fist.png/revision/latest?cb=20200529235418&format=original";
				case NPCID.GolemFistRight://248 Golem Fist
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f7/Right_Golem_Fist.png/revision/latest?cb=20200602093336&format=original";
				case NPCID.GolemHeadFree://249 Golem Head
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f9/Free_Golem_Head.png/revision/latest?cb=20200602094652&format=original";
				case NPCID.AngryNimbus://250 Angry Nimbus
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/db/Angry_Nimbus.png/revision/latest?cb=20171130030045&format=original";
				case NPCID.Eyezor://251 Eyezor
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b6/Eyezor.png/revision/latest?cb=20210611165016&format=original";
				case NPCID.Parrot://252 Parrot
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/16/Parrot.png/revision/latest?cb=20170421215823&format=original";
				case NPCID.Reaper://253 Reaper
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6a/Reaper.png/revision/latest?cb=20171130031709&format=original";
				case NPCID.ZombieMushroom://254 Spore Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/13/Spore_Zombie.png/revision/latest?cb=20200521181815&format=original";
				case NPCID.ZombieMushroomHat://255 Spore Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b1/Mushroom_Zombie.png/revision/latest?cb=20200521181747&format=original";
				case NPCID.FungoFish://256 Fungo Fish
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/be/Fungo_Fish.png/revision/latest?cb=20200521181744&format=original";
				case NPCID.AnomuraFungus://257 Anomura Fungus
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f0/Anomura_Fungus.png/revision/latest?cb=20200521181927&format=original";
				case NPCID.MushiLadybug://258 Mushi Ladybug
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/85/Mushi_Ladybug.png/revision/latest?cb=20200521181746&format=original";
				case NPCID.FungiBulb://259 Fungi Bulb
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f5/Fungi_Bulb.png/revision/latest?cb=20200521175303&format=original";
				case NPCID.GiantFungiBulb://260 Giant Fungi Bulb
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5f/Giant_Fungi_Bulb.png/revision/latest?cb=20200521175310&format=original";
				case NPCID.FungiSpore://261 Fungi Spore
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/99/Fungi_Spore.png/revision/latest?cb=20200521174943&format=original";
				case NPCID.Plantera://262 Plantera
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9e/Plantera_%28First_form%29.gif/revision/latest?cb=20200706212844&format=original";
				case NPCID.PlanterasHook://263 Plantera's Hook
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/30/Plantera%27s_Hook.png/revision/latest?cb=20200521175019&format=original";
				case NPCID.PlanterasTentacle://264 Plantera's Tentacle
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a5/Plantera%27s_Tentacle.png/revision/latest?cb=20200521174809&format=original";
				case NPCID.Spore://265 Spore
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a9/Spore.png/revision/latest?cb=20200521174616&format=original";
				case NPCID.BrainofCthulhu://266 Brain of Cthulhu
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/18/Brain_of_Cthulhu_%28First_Phase%29.gif/revision/latest?cb=20211114173255&format=original";
				case NPCID.Creeper://267 Creeper
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0a/Creeper.png/revision/latest?cb=20200521174446&format=original";
				case NPCID.IchorSticker://268 Ichor Sticker
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8e/Ichor_Sticker.png/revision/latest?cb=20200521181745&format=original";
				case NPCID.RustyArmoredBonesAxe://269 Rusty Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/74/Rusty_Armored_Bones_1.png/revision/latest?cb=20200811222335&format=original";
				case NPCID.RustyArmoredBonesFlail://270 Rusty Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/54/Rusty_Armored_Bones_2.png/revision/latest?cb=20200811222519&format=original";
				case NPCID.RustyArmoredBonesSword://271 Rusty Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d8/Rusty_Armored_Bones_3.png/revision/latest?cb=20200811222656&format=original";
				case NPCID.RustyArmoredBonesSwordNoArmor://272 Rusty Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8b/Rusty_Armored_Bones_4.png/revision/latest?cb=20200811222738&format=original";
				case NPCID.BlueArmoredBones://273 Blue Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d7/Blue_Armored_Bones_1.png/revision/latest?cb=20200530060830&format=original";
				case NPCID.BlueArmoredBonesMace://274 Blue Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9f/Blue_Armored_Bones_2.png/revision/latest?cb=20200530060831&format=original";
				case NPCID.BlueArmoredBonesNoPants://275 Blue Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/05/Blue_Armored_Bones_3.png/revision/latest?cb=20200530060833&format=original";
				case NPCID.BlueArmoredBonesSword://276 Blue Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/53/Blue_Armored_Bones_4.png/revision/latest?cb=20200530060834&format=original";
				case NPCID.HellArmoredBones://277 Hell Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/56/Hell_Armored_Bones_1.png/revision/latest?cb=20200518132724&format=original";
				case NPCID.HellArmoredBonesSpikeShield://278 Hell Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2e/Hell_Armored_Bones_2.png/revision/latest?cb=20200518132721&format=original";
				case NPCID.HellArmoredBonesMace://279 Hell Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d7/Hell_Armored_Bones_3.png/revision/latest?cb=20200518132718&format=original";
				case NPCID.HellArmoredBonesSword://280 Hell Armored Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/05/Hell_Armored_Bones_4.png/revision/latest?cb=20200518132730&format=original";
				case NPCID.RaggedCaster://281 Ragged Caster
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/40/Ragged_Caster_1.png/revision/latest?cb=20220312055614&format=original";
				case NPCID.RaggedCasterOpenCoat://282 Ragged Caster
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fc/Ragged_Caster_2.png/revision/latest?cb=20220312055704&format=original";
				case NPCID.Necromancer://283 Necromancer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4e/Necromancer_1.png/revision/latest?cb=20220312053825&format=original";
				case NPCID.NecromancerArmored://284 Necromancer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3a/Necromancer_2.png/revision/latest?cb=20220312053710&format=original";
				case NPCID.DiabolistRed://285 Diabolist
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6e/Diabolist_1.png/revision/latest?cb=20220312060327&format=original";
				case NPCID.DiabolistWhite://286 Diabolist
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/75/Diabolist_2.png/revision/latest?cb=20220312060429&format=original";
				case NPCID.BoneLee://287 Bone Lee
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/75/Bone_Lee.png/revision/latest?cb=20200803161640&format=original";
				case NPCID.DungeonSpirit://288 Dungeon Spirit
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b7/Dungeon_Spirit.png/revision/latest?cb=20171130033849&format=original";
				case NPCID.GiantCursedSkull://289 Giant Cursed Skull
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/97/Giant_Cursed_Skull.png/revision/latest?cb=20200521174503&format=original";
				case NPCID.Paladin://290 Paladin
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b2/Paladin.png/revision/latest?cb=20200521173908&format=original";
				case NPCID.SkeletonSniper://291 Skeleton Sniper
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4d/Skeleton_Sniper.png/revision/latest?cb=20200518190711&format=original";
				case NPCID.TacticalSkeleton://292 Tactical Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1b/Skeleton_Commando.png/revision/latest?cb=20200604133711&format=original";
				case NPCID.SkeletonCommando://293 Skeleton Commando
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1b/Skeleton_Commando.png/revision/latest?cb=20200604133711&format=original";
				case NPCID.AngryBonesBig://294 Angry Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/33/Angry_Bones_2.png/revision/latest?cb=20200530060827&format=original";
				case NPCID.AngryBonesBigMuscle://295 Angry Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b9/Angry_Bones_3.png/revision/latest?cb=20200530060828&format=original";
				case NPCID.AngryBonesBigHelmet://296 Angry Bones
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/56/Angry_Bones_4.png/revision/latest?cb=20200530060829&format=original";
				case NPCID.BirdBlue://297 Blue Jay
					return "";
				case NPCID.BirdRed://298 Cardinal
					return "";
				case NPCID.Squirrel://299 Squirrel
					return "";
				case NPCID.Mouse://300 Mouse
					return "";
				case NPCID.Raven://301 Raven
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/77/Raven_%28flying%29.png/revision/latest?cb=20210914024621&format=original";
				case NPCID.SlimeMasked://302 Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a4/Bunny_Slime.png/revision/latest?cb=20151229024026&format=original";
				case NPCID.BunnySlimed://303 Bunny
					return "";
				case NPCID.HoppinJack://304 Hoppin' Jack
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/43/Hoppin%27_Jack.png/revision/latest?cb=20131025174453&format=original";
				case NPCID.Scarecrow1://305 Scarecrow
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e6/Scarecrow_1.png/revision/latest?cb=20141219002114&format=original";
				case NPCID.Scarecrow2://306 Scarecrow
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/bd/Scarecrow_2.png/revision/latest?cb=20141219002212&format=original";
				case NPCID.Scarecrow3://307 Scarecrow
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/74/Scarecrow_3.png/revision/latest?cb=20141219002229&format=original";
				case NPCID.Scarecrow4://308 Scarecrow
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/ff/Scarecrow_4.png/revision/latest?cb=20141219002245&format=original";
				case NPCID.Scarecrow5://309 Scarecrow
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2d/Scarecrow_5.png/revision/latest?cb=20141219002324&format=original";
				case NPCID.Scarecrow6://310 Scarecrow
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/75/Scarecrow.png/revision/latest?cb=20141219002655&format=original";
				case NPCID.Scarecrow7://311 Scarecrow
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4b/Scarecrow_7.png/revision/latest?cb=20141219002347&format=original";
				case NPCID.Scarecrow8://312 Scarecrow
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/34/Scarecrow_8.png/revision/latest?cb=20141219002435&format=original";
				case NPCID.Scarecrow9://313 Scarecrow
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f7/Scarecrow_9.png/revision/latest?cb=20141219002459&format=original";
				case NPCID.Scarecrow10://314 Scarecrow
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/62/Scarecrow_10.png/revision/latest?cb=20131025174054&format=original";
				case NPCID.HeadlessHorseman://315 Headless Horseman
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3d/Headless_Horseman.png/revision/latest?cb=20210625213223&format=original";
				case NPCID.Ghost://316 Ghost
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/70/Ghost_%28enemy%29.png/revision/latest?cb=20131025175525&format=original";
				case NPCID.DemonEyeOwl://317 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d7/Demon_Eye_Halloween_Variant_1.png/revision/latest?cb=20170422125859&format=original";
				case NPCID.DemonEyeSpaceship://318 Demon Eye
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6a/Demon_Eye_Halloween_Variant_2.png/revision/latest?cb=20170422125929&format=original";
				case NPCID.ZombieDoctor://319 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6c/Zombie_Halloween_Variant_1.png/revision/latest?cb=20170422121435&format=original";
				case NPCID.ZombieSuperman://320 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/26/Zombie_Halloween_Variant_2.png/revision/latest?cb=20170422121433&format=original";
				case NPCID.ZombiePixie://321 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0c/Zombie_Halloween_Variant_3.png/revision/latest?cb=20170422121437&format=original";
				case NPCID.SkeletonTopHat://322 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9e/Skeleton_Halloween_Variant_1.png/revision/latest?cb=20171104202519&format=original";
				case NPCID.SkeletonAstonaut://323 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/65/Skeleton_Halloween_Variant_2.png/revision/latest?cb=20200808224709&format=original";
				case NPCID.SkeletonAlien://324 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/68/Skeleton_Halloween_Variant_3.png/revision/latest?cb=20171104202707&format=original";
				case NPCID.MourningWood://325 Mourning Wood
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/90/Mourning_Wood.gif/revision/latest?cb=20190125130555&format=original";
				case NPCID.Splinterling://326 Splinterling
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/88/Splinterling.png/revision/latest?cb=20171104210227&format=original";
				case NPCID.Pumpking://327 Pumpking
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d8/Pumpking.png/revision/latest?cb=20210914051940&format=original";
				case NPCID.PumpkingBlade://328 Pumpking
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c5/Pumpking_Hand.png/revision/latest?cb=20141218234517&format=original";
				case NPCID.Hellhound://329 Hellhound
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3e/Hellhound.png/revision/latest?cb=20171213013510&format=original";
				case NPCID.Poltergeist://330 Poltergeist
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/bb/Poltergeist.png/revision/latest?cb=20131025170412&format=original";
				case NPCID.ZombieXmas://331 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/da/Zombie_Christmas_Variant_1.png/revision/latest?cb=20170422121212&format=original";
				case NPCID.ZombieSweater://332 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b2/Zombie_Christmas_Variant_2.png/revision/latest?cb=20170422121209&format=original";
				case NPCID.SlimeRibbonWhite://333 Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/59/White_Present_Slime.png/revision/latest?cb=20170217233818&format=original";
				case NPCID.SlimeRibbonYellow://334 Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c9/Yellow_Present_Slime.png/revision/latest?cb=20170217233853&format=original";
				case NPCID.SlimeRibbonGreen://335 Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1d/Green_Present_Slime.png/revision/latest?cb=20170217233925&format=original";
				case NPCID.SlimeRibbonRed://336 Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/bf/Red_Present_Slime.png/revision/latest?cb=20170217234001&format=original";
				case NPCID.BunnyXmas://337 Bunny
					return "";
				case NPCID.ZombieElf://338 Zombie Elf
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fb/Zombie_Elf.png/revision/latest?cb=20200702150744&format=original";
				case NPCID.ZombieElfBeard://339 Zombie Elf
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9e/Zombie_Elf_Beard.png/revision/latest?cb=20200702150736&format=original";
				case NPCID.ZombieElfGirl://340 Zombie Elf
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e0/Zombie_Elf_Girl.png/revision/latest?cb=20200702150727&format=original";
				case NPCID.PresentMimic://341 Present Mimic
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8b/Present_Mimic_Forms.png/revision/latest?cb=20200730152725&format=original";
				case NPCID.GingerbreadMan://342 Gingerbread Man
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2e/Gingerbread_Man.png/revision/latest?cb=20200708164201&format=original";
				case NPCID.Yeti://343 Yeti
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7d/Yeti.png/revision/latest?cb=20200518194832&format=original";
				case NPCID.Everscream://344 Everscream
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e3/Everscream.gif/revision/latest?cb=20200605002358&format=original";
				case NPCID.IceQueen://345 Ice Queen
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9f/Ice_Queen.png/revision/latest?cb=20171213014122&format=original";
				case NPCID.SantaNK1://346 Santa-NK1
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ee/Santa-NK1.png/revision/latest?cb=20131220135050&format=original";
				case NPCID.ElfCopter://347 Elf Copter
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/45/Elf_Copter.png/revision/latest?cb=20200518194558&format=original";
				case NPCID.Nutcracker://348 Nutcracker
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/38/Nutcracker.png/revision/latest?cb=20200911122316&format=original";
				case NPCID.NutcrackerSpinning://349 Nutcracker
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ec/Nutcracker_2.png/revision/latest?cb=20200911122308&format=original";
				case NPCID.ElfArcher://350 Elf Archer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d3/Elf_Archer.png/revision/latest?cb=20200803161148&format=original";
				case NPCID.Krampus://351 Krampus
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8f/Krampus.png/revision/latest?cb=20200702145123&format=original";
				case NPCID.Flocko://352 Flocko
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/62/Flocko.png/revision/latest?cb=20200518194055&format=original";
				case NPCID.Stylist://353 Stylist
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/16/Stylist.png/revision/latest?cb=20151031152652&format=original";
				case NPCID.WebbedStylist://354 Webbed Stylist
					return "";
				case NPCID.Firefly://355 Firefly
					return "";
				case NPCID.Butterfly://356 Butterfly
					return "";
				case NPCID.Worm://357 Worm
					return "";
				case NPCID.LightningBug://358 Lightning Bug
					return "";
				case NPCID.Snail://359 Snail
					return "";
				case NPCID.GlowingSnail://360 Glowing Snail
					return "";
				case NPCID.Frog://361 Frog
					return "";
				case NPCID.Duck://362 Duck
					return "";
				case NPCID.Duck2://363 Duck
					return "";
				case NPCID.DuckWhite://364 Duck
					return "";
				case NPCID.DuckWhite2://365 Duck
					return "";
				case NPCID.ScorpionBlack://366 Scorpion
					return "";
				case NPCID.Scorpion://367 Scorpion
					return "";
				case NPCID.TravellingMerchant://368 Traveling Merchant
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/37/Traveling_Merchant.png/revision/latest?cb=20150704081454&format=original";
				case NPCID.Angler://369 Angler
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/bf/Angler.png/revision/latest?cb=20200702150720&format=original	";
				case NPCID.DukeFishron://370 Duke Fishron
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0b/Duke_Fishron.png/revision/latest?cb=20180705150806&format=original";
				case NPCID.DetonatingBubble://371 Detonating Bubble
					return "";
				case NPCID.Sharkron://372 Sharkron
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ef/Sharkron.png/revision/latest?cb=20140508212606&format=original";
				case NPCID.Sharkron2://373 Sharkron
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ef/Sharkron.png/revision/latest?cb=20140508212606&format=original";
				case NPCID.TruffleWorm://374 Truffle Worm
					return "";
				case NPCID.TruffleWormDigger://375 Truffle Worm
					return "";
				case NPCID.SleepingAngler://376 Sleeping Angler
					return "";
				case NPCID.Grasshopper://377 Grasshopper
					return "";
				case NPCID.ChatteringTeethBomb://378 Chattering Teeth Bomb
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/60/Chattering_Teeth_Bomb.gif/revision/latest?cb=20210627211221&format=original";
				case NPCID.CultistArcherBlue://379 Cultist Archer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/25/Cultist_Archer.png/revision/latest?cb=20150701095329&format=original";
				case NPCID.CultistArcherWhite://380 Cultist Archer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/30/White_Cultist_Archer.png/revision/latest?cb=20150701095459&format=original";
				case NPCID.BrainScrambler://381 Brain Scrambler
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/38/Brain_Scrambler.png/revision/latest?cb=20200704161941&format=original";
				case NPCID.RayGunner://382 Ray Gunner
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d7/Ray_Gunner.png/revision/latest?cb=20150701095421&format=original";
				case NPCID.MartianOfficer://383 Martian Officer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b7/Martian_Officer.png/revision/latest?cb=20150701010423&format=original";
				case NPCID.ForceBubble://384 Bubble Shield
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/da/Bubble_Shield.png/revision/latest?cb=20150701010521&format=original";
				case NPCID.GrayGrunt://385 Gray Grunt
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8e/Gray_Grunt.png/revision/latest?cb=20150701095347&format=original";
				case NPCID.MartianEngineer://386 Martian Engineer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/15/Martian_Engineer.png/revision/latest?cb=20210914031152&format=original";
				case NPCID.MartianTurret://387 Tesla Turret
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9f/Tesla_Turret.gif/revision/latest?cb=20211204005336&format=original";
				case NPCID.MartianDrone://388 Martian Drone
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ea/Martian_Drone.png/revision/latest?cb=20150701135039&format=original";
				case NPCID.GigaZapper://389 Gigazapper
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/29/Gigazapper.png/revision/latest?cb=20210914030825&format=original";
				case NPCID.ScutlixRider://390 Scutlix Gunner
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5a/Scutlix_Gunner.png/revision/latest?cb=20150701010937&format=original";
				case NPCID.Scutlix://391 Scutlix
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e7/Scutlix_%28creature%29.png/revision/latest?cb=20150701171708&format=original";
				case NPCID.MartianSaucer://392 Martian Saucer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6f/Martian_Saucer.png/revision/latest?cb=20150702004716&format=original";
				case NPCID.MartianSaucerTurret://393 Martian Saucer Turret
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4c/Martian_Saucer_Turret.png/revision/latest?cb=20150701095504&format=original";
				case NPCID.MartianSaucerCannon://394 Martian Saucer Cannon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e3/Martian_Saucer_Cannon.png/revision/latest?cb=20150701095409&format=original";
				case NPCID.MartianSaucerCore://395 Martian Saucer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/cf/Martian_Saucer_Core.png/revision/latest?cb=20150701010958&format=original";
				case NPCID.MoonLordHead://396 Moon Lord
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/84/Moon_Lord%27s_Head.gif/revision/latest?cb=20160712055422&format=original";
				case NPCID.MoonLordHand://397 Moon Lord's Hand
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/11/Moon_Lord%27s_Hand.gif/revision/latest?cb=20160712053505&format=original";
				case NPCID.MoonLordCore://398 Moon Lord's Core
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ea/Moon_Lord%27s_Core.gif/revision/latest?cb=20160712055841&format=original";
				case NPCID.MartianProbe://399 Martian Probe
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f8/Martian_Probe.png/revision/latest?cb=20150710011512&format=original";
				case NPCID.MoonLordFreeEye://400 True Eye of Cthulhu
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/56/True_Eye_of_Cthulhu.png/revision/latest?cb=20150804093912&format=original";
				case NPCID.MoonLordLeechBlob://401 Moon Leech Clot
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/16/Moon_Leech_Clot.png/revision/latest?cb=20150701095300&format=original";
				case NPCID.StardustWormHead://402 Milkyway Weaver
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0b/Milkyway_Weaver_%28Head%29.png/revision/latest?cb=20150701095415&format=original";
				case NPCID.StardustWormBody://403 NPCName.StardustWormBody
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/91/Milkyway_Weaver_%28Body%29.png/revision/latest?cb=20150701095433&format=original";
				case NPCID.StardustWormTail://404 NPCName.StardustWormTail
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/38/Milkyway_Weaver_%28Tail%29.png/revision/latest?cb=20150701095427&format=original";
				case NPCID.StardustCellBig://405 Star Cell
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/43/Star_Cell.png/revision/latest?cb=20150701095341&format=original";
				case NPCID.StardustCellSmall://406 Star Cell
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/43/Star_Cell.png/revision/latest?cb=20150701095341&format=original";
				case NPCID.StardustJellyfishBig://407 Flow Invader
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/95/Flow_Invader.png/revision/latest?cb=20150701141020&format=original";
				case NPCID.StardustJellyfishSmall://408 NPCName.StardustJellyfishSmall
					return "";
				case NPCID.StardustSpiderBig://409 Twinkle Popper
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4c/Twinkle_Popper.png/revision/latest?cb=20150701095448&format=original";
				case NPCID.StardustSpiderSmall://410 Twinkle
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/94/Twinkle.png/revision/latest?cb=20150701095454&format=original";
				case NPCID.StardustSoldier://411 Stargazer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2b/Stargazer.png/revision/latest?cb=20150701135045&format=original";
				case NPCID.SolarCrawltipedeHead://412 Crawltipede
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/61/Crawltipede_%28Head%29.png/revision/latest?cb=20150701100750&format=original";
				case NPCID.SolarCrawltipedeBody://413 Crawltipede
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b8/Crawltipede_%28Body%29.png/revision/latest?cb=20150701100745&format=original";
				case NPCID.SolarCrawltipedeTail://414 Crawltipede
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/71/Crawltipede_%28Tail%29.png/revision/latest?cb=20150701100755&format=original";
				case NPCID.SolarDrakomire://415 Drakomire
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1b/Drakomire.png/revision/latest?cb=20150701100807&format=original";
				case NPCID.SolarDrakomireRider://416 Drakomire Rider
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/28/Drakomire_Rider.png/revision/latest?cb=20150701100801&format=original";
				case NPCID.SolarSroller://417 Sroller
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/51/Sroller.png/revision/latest?cb=20150701100850&format=original";
				case NPCID.SolarCorite://418 Corite
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/05/Corite.png/revision/latest?cb=20150701100738&format=original";
				case NPCID.SolarSolenian://419 Selenian
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/48/Selenian.png/revision/latest?cb=20150701100845&format=original";
				case NPCID.NebulaBrain://420 Nebula Floater
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5a/Nebula_Floater.png/revision/latest?cb=20150701135041&format=original";
				case NPCID.NebulaHeadcrab://421 Brain Suckler
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fb/Brain_Suckler.png/revision/latest?cb=20201017034837&format=original";
				case NPCID.LunarTowerVortex://422 Vortex Pillar
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8d/Vortex_Pillar.png/revision/latest?cb=20150701100906&format=original";
				case NPCID.NebulaBeast://423 Evolution Beast
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/86/Evolution_Beast.png/revision/latest?cb=20201017035139&format=original";
				case NPCID.NebulaSoldier://424 Predictor
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/30/Predictor.png/revision/latest?cb=20150701135043&format=original";
				case NPCID.VortexRifleman://425 Storm Diver
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b8/Storm_Diver.png/revision/latest?cb=20150701135046&format=original";
				case NPCID.VortexHornetQueen://426 Alien Queen
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d8/Alien_Queen.png/revision/latest?cb=20150701100644&format=original";
				case NPCID.VortexHornet://427 Alien Hornet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4d/Alien_Hornet.png/revision/latest?cb=20150701100634&format=original";
				case NPCID.VortexLarva://428 Alien Larva
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5d/Alien_Larva.png/revision/latest?cb=20150701100639&format=original";
				case NPCID.VortexSoldier://429 Vortexian
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a0/Vortexian.png/revision/latest?cb=20150701100912&format=original";
				case NPCID.ArmedZombie://430 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d9/Armed_Zombie.png/revision/latest?cb=20170422131025&format=original";
				case NPCID.ArmedZombieEskimo://431 Frozen Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3c/Armed_Frozen_Zombie.png/revision/latest?cb=20170422131256&format=original";
				case NPCID.ArmedZombiePincussion://432 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e4/Armed_Pincushion_Zombie.png/revision/latest?cb=20170422131048&format=original";
				case NPCID.ArmedZombieSlimed://433 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/64/Armed_Slimed_Zombie.png/revision/latest?cb=20170422131112&format=original";
				case NPCID.ArmedZombieSwamp://434 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/48/Armed_Swamp_Zombie.png/revision/latest?cb=20170422131135&format=original";
				case NPCID.ArmedZombieTwiggy://435 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/da/Armed_Twiggy_Zombie.png/revision/latest?cb=20170422131200&format=original";
				case NPCID.ArmedZombieCenx://436 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/2c/Armed_Female_Zombie.png/revision/latest?cb=20170422131225&format=original";
				case NPCID.CultistTablet://437 Mysterious Tablet
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/53/Mysterious_Tablet.gif/revision/latest?cb=20190228020510&format=original";
				case NPCID.CultistDevote://438 Lunatic Devotee
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3a/Lunatic_Devotee.gif/revision/latest?cb=20150814182852&format=original";
				case NPCID.CultistBoss://439 Lunatic Cultist
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1c/Lunatic_Cultist.gif/revision/latest?cb=20190125125946&format=original";
				case NPCID.CultistBossClone://440 Lunatic Cultist
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/94/Ancient_Cultist.png/revision/latest?cb=20160318233643&format=original";
				case NPCID.TaxCollector://441 Tax Collector
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/75/Tax_Collector.png/revision/latest?cb=20150701011232&format=original";
				case NPCID.GoldBird://442 Gold Bird
					return "";
				case NPCID.GoldBunny://443 Gold Bunny
					return "";
				case NPCID.GoldButterfly://444 Gold Butterfly
					return "";
				case NPCID.GoldFrog://445 Gold Frog
					return "";
				case NPCID.GoldGrasshopper://446 Gold Grasshopper
					return "";
				case NPCID.GoldMouse://447 Gold Mouse
					return "";
				case NPCID.GoldWorm://448 Gold Worm
					return "";
				case NPCID.BoneThrowingSkeleton://449 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/23/Skeleton.png/revision/latest?cb=20170420012637&format=original";
				case NPCID.BoneThrowingSkeleton2://450 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/62/Headache_Skeleton.png/revision/latest?cb=20170422123913&format=original";
				case NPCID.BoneThrowingSkeleton3://451 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7b/Misassembled_Skeleton.png/revision/latest?cb=20170422123940&format=original";
				case NPCID.BoneThrowingSkeleton4://452 Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/be/Pantless_Skeleton.png/revision/latest?cb=20170422124010&format=original";
				case NPCID.SkeletonMerchant://453 Skeleton Merchant
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/37/Skeleton_Merchant.png/revision/latest?cb=20150701011353&format=original";
				case NPCID.CultistDragonHead://454 Phantasm Dragon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/30/Phantasm_Dragon_%28Head%29.png/revision/latest?cb=20150701103037&format=original";
				case NPCID.CultistDragonBody1://455 Phantasm Dragon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d0/Phantasm_Dragon_%28Body1%29.png/revision/latest?cb=20150701103013&format=original";
				case NPCID.CultistDragonBody2://456 Phantasm Dragon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b2/Phantasm_Dragon_%28Body2%29.png/revision/latest?cb=20150701103019&format=original";
				case NPCID.CultistDragonBody3://457 Phantasm Dragon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/53/Phantasm_Dragon_%28Body3%29.png/revision/latest?cb=20150701103024&format=original";
				case NPCID.CultistDragonBody4://458 Phantasm Dragon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4b/Phantasm_Dragon_%28Body4%29.png/revision/latest?cb=20150701103031&format=original";
				case NPCID.CultistDragonTail://459 Phantasm Dragon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d7/Phantasm_Dragon_%28Tail%29.png/revision/latest?cb=20150709000911&format=original";
				case NPCID.Butcher://460 Butcher
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fb/Butcher.png/revision/latest?cb=20150701102715&format=original";
				case NPCID.CreatureFromTheDeep://461 Creature from the Deep
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/91/Creature_from_the_Deep.png/revision/latest?cb=20150629182153&format=original";
				case NPCID.Fritz://462 Fritz
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e8/Fritz.png/revision/latest?cb=20150629214920&format=original";
				case NPCID.Nailhead://463 Nailhead
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/73/Nailhead.png/revision/latest?cb=20150701102948&format=original";
				case NPCID.CrimsonBunny://464 Vicious Bunny
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/99/Vicious_Bunny.png/revision/latest?cb=20171124231053&format=original";
				case NPCID.CrimsonGoldfish://465 Vicious Goldfish
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4d/Vicious_Goldfish.png/revision/latest?cb=20150701102758&format=original";
				case NPCID.Psycho://466 Psycho
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9c/Psycho.png/revision/latest?cb=20210620015106&format=original";
				case NPCID.DeadlySphere://467 Deadly Sphere
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6e/Deadly_Sphere.png/revision/latest?cb=20211218000054&format=original";
				case NPCID.DrManFly://468 Dr. Man Fly
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d9/Dr._Man_Fly.png/revision/latest?cb=20150701102809&format=original";
				case NPCID.ThePossessed://469 The Possessed
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5c/The_Possessed.png/revision/latest?cb=20150629184855&format=original";
				case NPCID.CrimsonPenguin://470 Vicious Penguin
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/77/Vicious_Penguin.png/revision/latest?cb=20200730170604&format=original";
				case NPCID.GoblinSummoner://471 Goblin Summoner
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/24/Goblin_Warlock.png/revision/latest?cb=20200708163902&format=original";
				case NPCID.ShadowFlameApparition://472 Shadowflame Apparition
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b1/Shadowflame_Apparition.png/revision/latest?cb=20150701103137&format=original";
				case NPCID.BigMimicCorruption://473 Corrupt Mimic
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9c/Corrupt_Mimic2.png/revision/latest?cb=20150723182451&format=original";
				case NPCID.BigMimicCrimson://474 Crimson Mimic
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/da/Crimson_Mimic2.png/revision/latest?cb=20150723182523&format=original";
				case NPCID.BigMimicHallow://475 Hallowed Mimic
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5e/Hallowed_Mimic2.png/revision/latest?cb=20150723182552&format=original";
				case NPCID.BigMimicJungle://476 Jungle Mimic
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6b/Jungle_Mimic2.png/revision/latest?cb=20150723182625&format=original";
				case NPCID.Mothron://477 Mothron
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1a/Mothron.png/revision/latest?cb=20150629182437&format=original";
				case NPCID.MothronEgg://478 Mothron Egg
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/14/Mothron_Egg.png/revision/latest?cb=20150701102942&format=original";
				case NPCID.MothronSpawn://479 Baby Mothron
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/87/Baby_Mothron.png/revision/latest?cb=20150701102702&format=original";
				case NPCID.Medusa://480 Medusa
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c3/Medusa.png/revision/latest?cb=20150701102935&format=original";
				case NPCID.GreekSkeleton://481 Hoplite
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6b/Hoplite.png/revision/latest?cb=20171209201110&format=original";
				case NPCID.GraniteGolem://482 Granite Golem
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4e/Granite_Golem.png/revision/latest?cb=20150701102858&format=original";
				case NPCID.GraniteFlyer://483 Granite Elemental
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/04/Granite_Elemental.gif/revision/latest?cb=20150501004036&format=original";
				case NPCID.EnchantedNightcrawler://484 Enchanted Nightcrawler
					return "";
				case NPCID.Grubby://485 Grubby
					return "";
				case NPCID.Sluggy://486 Sluggy
					return "";
				case NPCID.Buggy://487 Buggy
					return "";
				case NPCID.TargetDummy://488 Target Dummy
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/06/Target_Dummy_%28placed%29.gif/revision/latest?cb=20150701180254&format=original";
				case NPCID.BloodZombie://489 Blood Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/59/Blood_Zombie.png/revision/latest?cb=20171104201710&format=original";
				case NPCID.Drippler://490 Drippler
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/69/Drippler.gif/revision/latest?cb=20200831184238&format=original";
				case NPCID.PirateShip://491 Flying Dutchman
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/ba/Flying_Dutchman.png/revision/latest/scale-to-width-down/260?cb=20180705152731&format=original";
				case NPCID.PirateShipCannon://492 Dutchman Cannon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/ce/Dutchman_Cannon.png/revision/latest?cb=20150701102833&format=original";
				case NPCID.LunarTowerStardust://493 Stardust Pillar
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9f/Stardust_Pillar.png/revision/latest?cb=20180125013518&format=original";
				case NPCID.Crawdad://494 Crawdad
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/53/Crawdad.png/revision/latest?cb=20150701102733&format=original";
				case NPCID.Crawdad2://495 Crawdad
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9e/Crawdad2.png/revision/latest?cb=20150701102739&format=original";
				case NPCID.GiantShelly://496 Giant Shelly
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/dd/Giant_Shelly.png/revision/latest?cb=20150701102840&format=original";
				case NPCID.GiantShelly2://497 Giant Shelly
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8d/Giant_Shelly2.png/revision/latest?cb=20150701102846&format=original";
				case NPCID.Salamander://498 Salamander
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/53/Salamander.png/revision/latest?cb=20150701103048&format=original";
				case NPCID.Salamander2://499 Salamander
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/00/Salamander_2.png/revision/latest?cb=20180831035120&format=original";
				case NPCID.Salamander3://500 Salamander
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/88/Salamander_3.png/revision/latest?cb=20180831035112&format=original";
				case NPCID.Salamander4://501 Salamander
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fb/Salamander_4.png/revision/latest?cb=20180831035103&format=original";
				case NPCID.Salamander5://502 Salamander
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8a/Salamander_5.png/revision/latest?cb=20180831035036&format=original";
				case NPCID.Salamander6://503 Salamander
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4e/Salamander_6.png/revision/latest?cb=20180831035047&format=original";
				case NPCID.Salamander7://504 Salamander
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/75/Salamander_7.png/revision/latest?cb=20180831035142&format=original";
				case NPCID.Salamander8://505 Salamander
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e9/Salamander_8.png/revision/latest?cb=20180831035151&format=original";
				case NPCID.Salamander9://506 Salamander
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c1/Salamander_9.png/revision/latest?cb=20180831035203&format=original";
				case NPCID.LunarTowerNebula://507 Nebula Pillar
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a9/Nebula_Pillar.png/revision/latest?cb=20150701102641&format=original";
				case NPCID.GiantWalkingAntlion://508 Giant Antlion Charger
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/01/Giant_Antlion_Charger.png/revision/latest?cb=20191112012549&format=original";
				case NPCID.GiantFlyingAntlion://509 Giant Antlion Swarmer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9c/Giant_Antlion_Swarmer.png/revision/latest?cb=20150701102655&format=original";
				case NPCID.DuneSplicerHead://510 Dune Splicer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/da/Dune_Splicer_%28Head%29.png/revision/latest?cb=20150701102822&format=original";
				case NPCID.DuneSplicerBody://511 Dune Splicer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ef/Dune_Splicer_%28Body%29.png/revision/latest?cb=20150701102816&format=original";
				case NPCID.DuneSplicerTail://512 Dune Splicer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b6/Dune_Splicer_%28Tail%29.png/revision/latest?cb=20150701102828&format=original";
				case NPCID.TombCrawlerHead://513 Tomb Crawler
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8f/Tomb_Crawler_%28Head%29.png/revision/latest?cb=20200530045928&format=original";
				case NPCID.TombCrawlerBody://514 Tomb Crawler
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0f/Tomb_Crawler_%28Body%29.png/revision/latest?cb=20200530050002&format=original";
				case NPCID.TombCrawlerTail://515 Tomb Crawler
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/70/Tomb_Crawler_%28Tail%29.png/revision/latest?cb=20200530045949&format=original";
				case NPCID.SolarFlare://516 Solar Flare
					return "";
				case NPCID.LunarTowerSolar://517 Solar Pillar
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/50/Solar_Pillar.png/revision/latest?cb=20180125005222&format=original";
				case NPCID.SolarSpearman://518 Drakanian
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f2/Drakanian.png/revision/latest?cb=20150701103316&format=original";
				case NPCID.SolarGoop://519 Solar Fragment
					return "";
				case NPCID.MartianWalker://520 Martian Walker
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f1/Martian_Walker.png/revision/latest?cb=20150701103346&format=original";
				case NPCID.AncientCultistSquidhead://521 Ancient Vision
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e0/Ancient_Vision.png/revision/latest?cb=20150701103258&format=original";
				case NPCID.AncientLight://522 Ancient Light
					return "";
				case NPCID.AncientDoom://523 Ancient Doom
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/26/Ancient_Doom.png/revision/latest?cb=20150701103247&format=original";
				case NPCID.DesertGhoul://524 Ghoul
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e3/Ghoul.png/revision/latest?cb=20150701103329&format=original";
				case NPCID.DesertGhoulCorruption://525 Vile Ghoul
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/fa/Vile_Ghoul.png/revision/latest?cb=20150701103435&format=original";
				case NPCID.DesertGhoulCrimson://526 Tainted Ghoul
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/89/Tainted_Ghoul.png/revision/latest?cb=20150701103417&format=original";
				case NPCID.DesertGhoulHallow://527 Dreamer Ghoul
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/01/Dreamer_Ghoul.png/revision/latest?cb=20150701103323&format=original";
				case NPCID.DesertLamiaLight://528 Lamia
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5c/Lamia.png/revision/latest?cb=20150701103335&format=original";
				case NPCID.DesertLamiaDark://529 Lamia
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e5/Lamia2.png/revision/latest?cb=20150701103341&format=original";
				case NPCID.DesertScorpionWalk://530 Sand Poacher
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1e/Sand_Poacher.png/revision/latest?cb=20150701103353&format=original";
				case NPCID.DesertScorpionWall://531 Sand Poacher
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/9d/Sand_Poacher2.png/revision/latest?cb=20150701103358&format=original";
				case NPCID.DesertBeast://532 Basilisk
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b1/Basilisk.png/revision/latest?cb=20150701103303&format=original";
				case NPCID.DesertDjinn://533 Desert Spirit
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/35/Desert_Spirit.png/revision/latest?cb=20150701103311&format=original";
				case NPCID.DemonTaxCollector://534 Tortured Soul
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/83/Tortured_Soul.png/revision/latest?cb=20150701103428&format=original";
				case NPCID.SlimeSpiked://535 Spiked Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/10/Spiked_Slime.png/revision/latest?cb=20150701103411&format=original";
				case NPCID.TheBride://536 The Bride
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3f/The_Bride.png/revision/latest?cb=20210620013648&format=original";
				case NPCID.SandSlime://537 Sand Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f2/Sand_Slime.png/revision/latest?cb=20160224071115&format=original";
				case NPCID.SquirrelRed://538 Red Squirrel
					return "";
				case NPCID.SquirrelGold://539 Gold Squirrel
					return "";
				case NPCID.PartyBunny://540 Bunny
					return "";
				case NPCID.SandElemental://541 Sand Elemental
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/aa/Sand_Elemental.png/revision/latest?cb=20160909175739&format=original";
				case NPCID.SandShark://542 Sand Shark
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/9/95/Sand_Shark.png/revision/latest?cb=20160909154452&format=original";
				case NPCID.SandsharkCorrupt://543 Bone Biter
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/7c/Bone_Biter.png/revision/latest?cb=20160909201152&format=original";
				case NPCID.SandsharkCrimson://544 Flesh Reaver
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/85/Flesh_Reaver.png/revision/latest?cb=20160909201148&format=original";
				case NPCID.SandsharkHallow://545 Crystal Thresher
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5a/Crystal_Thresher.png/revision/latest?cb=20160909201150&format=original";
				case NPCID.Tumbleweed://546 Angry Tumbler
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/40/Angry_Tumbler.png/revision/latest?cb=20190205230614&format=original";
				case NPCID.DD2AttackerTest://547 ???
					return "";
				case NPCID.DD2EterniaCrystal://548 Eternia Crystal
					return "";
				case NPCID.DD2LanePortal://549 Mysterious Portal
					return "";
				case NPCID.DD2Bartender://550 Tavernkeep
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/81/Tavernkeep.png/revision/latest?cb=20161115191006&format=original";
				case NPCID.DD2Betsy://551 Betsy
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/ec/Betsy.png/revision/latest?cb=20161117001359&format=original";
				case NPCID.DD2GoblinT1://552 Etherian Goblin
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8c/Etherian_Goblin.png/revision/latest?cb=20161116123728&format=original";
				case NPCID.DD2GoblinT2://553 Etherian Goblin
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/42/Etherian_Goblin_2.png/revision/latest?cb=20161116123746&format=original";
				case NPCID.DD2GoblinT3://554 Etherian Goblin
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/3a/Etherian_Goblin_3.png/revision/latest?cb=20161116123808&format=original";
				case NPCID.DD2GoblinBomberT1://555 Etherian Goblin Bomber
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/79/Etherian_Goblin_Bomber.png/revision/latest?cb=20161116124106&format=original";
				case NPCID.DD2GoblinBomberT2://556 Etherian Goblin Bomber
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5c/Etherian_Goblin_Bomber_2.png/revision/latest?cb=20161116124101&format=original";
				case NPCID.DD2GoblinBomberT3://557 Etherian Goblin Bomber
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/21/Etherian_Goblin_Bomber_3.png/revision/latest?cb=20161116124109&format=original";
				case NPCID.DD2WyvernT1://558 Etherian Wyvern
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/8b/Etherian_Wyvern.png/revision/latest?cb=20210625221330&format=original";
				case NPCID.DD2WyvernT2://559 Etherian Wyvern
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/34/Etherian_Wyvern_2.png/revision/latest?cb=20210625221449&format=original";
				case NPCID.DD2WyvernT3://560 Etherian Wyvern
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/14/Etherian_Wyvern_3.png/revision/latest?cb=20210625221540&format=original";
				case NPCID.DD2JavelinstT1://561 Etherian Javelin Thrower
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/06/Etherian_Javelin_Thrower.png/revision/latest?cb=20161116125303&format=original";
				case NPCID.DD2JavelinstT2://562 Etherian Javelin Thrower
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/55/Etherian_Javelin_Thrower_2.png/revision/latest?cb=20161116125322&format=original";
				case NPCID.DD2JavelinstT3://563 Etherian Javelin Thrower
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/ae/Etherian_Javelin_Thrower_3.png/revision/latest?cb=20161116125327&format=original";
				case NPCID.DD2DarkMageT1://564 Dark Mage
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/33/Dark_Mage.png/revision/latest?cb=20161116171947&format=original";
				case NPCID.DD2DarkMageT3://565 Dark Mage
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/3/33/Dark_Mage.png/revision/latest?cb=20161116171947&format=original";
				case NPCID.DD2SkeletonT1://566 Old One's Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f5/Old_One%27s_Skeleton.png/revision/latest?cb=20161116124228&format=original";
				case NPCID.DD2SkeletonT3://567 Old One's Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f5/Old_One%27s_Skeleton.png/revision/latest?cb=20161116124228&format=original";
				case NPCID.DD2WitherBeastT2://568 Wither Beast
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/00/Wither_Beast.png/revision/latest?cb=20161116124330&format=original";
				case NPCID.DD2WitherBeastT3://569 Wither Beast
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/00/Wither_Beast.png/revision/latest?cb=20161116124330&format=original";
				case NPCID.DD2DrakinT2://570 Drakin
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f5/Drakin.png/revision/latest?cb=20161116124454&format=original";
				case NPCID.DD2DrakinT3://571 Drakin
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0e/Drakin_2.png/revision/latest?cb=20161116124450&format=original";
				case NPCID.DD2KoboldWalkerT2://572 Kobold
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d2/Kobold.png/revision/latest?cb=20161116125025&format=original";
				case NPCID.DD2KoboldWalkerT3://573 Kobold
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d2/Kobold.png/revision/latest?cb=20161116125025&format=original";
				case NPCID.DD2KoboldFlyerT2://574 Kobold Glider
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/49/Kobold_Glider.png/revision/latest?cb=20161116123143&format=original";
				case NPCID.DD2KoboldFlyerT3://575 Kobold Glider
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/49/Kobold_Glider.png/revision/latest?cb=20161116123143&format=original";
				case NPCID.DD2OgreT2://576 Ogre
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c5/Ogre.png/revision/latest?cb=20161115192150&format=original";
				case NPCID.DD2OgreT3://577 Ogre
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c5/Ogre.png/revision/latest?cb=20161115192150&format=original";
				case NPCID.DD2LightningBugT3://578 Etherian Lightning Bug
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1d/Etherian_Lightning_Bug.png/revision/latest?cb=20161116123152&format=original";
				case NPCID.BartenderUnconscious://579 Unconscious Man
					return "";
				case NPCID.WalkingAntlion://580 Antlion Charger
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6c/Antlion_Charger.png/revision/latest?cb=20200517034253&format=original";
				case NPCID.FlyingAntlion://581 Antlion Swarmer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/11/Antlion_Swarmer.png/revision/latest?cb=20200521230735&format=original";
				case NPCID.LarvaeAntlion://582 Antlion Larva
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c8/Antlion_Larva.png/revision/latest?cb=20200517034039&format=original";
				case NPCID.FairyCritterPink://583 Pink Fairy
					return "";
				case NPCID.FairyCritterGreen://584 Green Fairy
					return "";
				case NPCID.FairyCritterBlue://585 Blue Fairy
					return "";
				case NPCID.ZombieMerman://586 Zombie Merman
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/d/d9/Zombie_Merman.png/revision/latest?cb=20220305001421&format=original";
				case NPCID.EyeballFlyingFish://587 Wandering Eye Fish
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/53/Wandering_Eye_Fish.png/revision/latest?cb=20200516195725&format=original";
				case NPCID.Golfer://588 Golfer
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1a/Golfer.png/revision/latest?cb=20200516183144&format=original";
				case NPCID.GolferRescue://589 Golfer
					return "";
				case NPCID.TorchZombie://590 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f8/Torch_Zombie.png/revision/latest?cb=20200516195725&format=original";
				case NPCID.ArmedTorchZombie://591 Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f6/Armed_Torch_Zombie.png/revision/latest?cb=20200516195727&format=original";
				case NPCID.GoldGoldfish://592 Gold Goldfish
					return "";
				case NPCID.GoldGoldfishWalker://593 Gold Goldfish
					return "";
				case NPCID.WindyBalloon://594 Windy Balloon
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/1a/Windy_Balloon.png/revision/latest?cb=20200516200240&format=original";
				case NPCID.BlackDragonfly://595 Dragonfly
					return "";
				case NPCID.BlueDragonfly://596 Dragonfly
					return "";
				case NPCID.GreenDragonfly://597 Dragonfly
					return "";
				case NPCID.OrangeDragonfly://598 Dragonfly
					return "";
				case NPCID.RedDragonfly://599 Dragonfly
					return "";
				case NPCID.YellowDragonfly://600 Dragonfly
					return "";
				case NPCID.GoldDragonfly://601 Gold Dragonfly
					return "";
				case NPCID.Seagull://602 Seagull
					return "";
				case NPCID.Seagull2://603 Seagull
					return "";
				case NPCID.LadyBug://604 Ladybug
					return "";
				case NPCID.GoldLadyBug://605 Gold Ladybug
					return "";
				case NPCID.Maggot://606 Maggot
					return "";
				case NPCID.Pupfish://607 Pupfish
					return "";
				case NPCID.Grebe://608 Grebe
					return "";
				case NPCID.Grebe2://609 Grebe
					return "";
				case NPCID.Rat://610 Rat
					return "";
				case NPCID.Owl://611 Owl
					return "";
				case NPCID.WaterStrider://612 Water Strider
					return "";
				case NPCID.GoldWaterStrider://613 Gold Water Strider
					return "";
				case NPCID.ExplosiveBunny://614 Explosive Bunny
					return "";
				case NPCID.Dolphin://615 Dolphin
					return "";
				case NPCID.Turtle://616 Turtle
					return "";
				case NPCID.TurtleJungle://617 Jungle Turtle
					return "";
				case NPCID.BloodNautilus://618 Dreadnautilus
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/59/Dreadnautilus.png/revision/latest?cb=20200517123621&format=original";
				case NPCID.BloodSquid://619 Blood Squid
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a3/Blood_Squid.png/revision/latest?cb=20200516201131&format=original";
				case NPCID.GoblinShark://620 Hemogoblin Shark
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/a9/Hemogoblin_Shark.png/revision/latest?cb=20200516201155&format=original";
				case NPCID.BloodEelHead://621 Blood Eel
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/6c/Blood_Eel_Head.png/revision/latest?cb=20200516201211&format=original";
				case NPCID.BloodEelBody://622 Blood Eel
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/0/0e/Blood_Eel_Body.png/revision/latest?cb=20200516201210&format=original";
				case NPCID.BloodEelTail://623 Blood Eel
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/bb/Blood_Eel_Tail.png/revision/latest?cb=20200516201212&format=original";
				case NPCID.Gnome://624 Gnome
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/78/Gnome.png/revision/latest?cb=20200516203041&format=original";
				case NPCID.SeaTurtle://625 Sea Turtle
					return "";
				case NPCID.Seahorse://626 Seahorse
					return "";
				case NPCID.GoldSeahorse://627 Gold Seahorse
					return "";
				case NPCID.Dandelion://628 Angry Dandelion
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/e/e2/Angry_Dandelion.png/revision/latest?cb=20200516192425&format=original";
				case NPCID.IceMimic://629 Ice Mimic
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/1/18/Ice_Mimic.png/revision/latest?cb=20170421213009&format=original";
				case NPCID.BloodMummy://630 Blood Mummy
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/2/28/Blood_Mummy.png/revision/latest?cb=20200516192426&format=original";
				case NPCID.RockGolem://631 Rock Golem
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/80/Rock_Golem.png/revision/latest?cb=20200516192425&format=original";
				case NPCID.MaggotZombie://632 Maggot Zombie
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/a/ab/Maggot_Zombie.png/revision/latest?cb=20200516193041&format=original";
				case NPCID.BestiaryGirl://633 Zoologist
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/6/61/Zoologist.png/revision/latest?cb=20200516192903&format=original";
				case NPCID.SporeBat://634 Spore Bat
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b7/Spore_Bat.png/revision/latest?cb=20200516193907&format=original";
				case NPCID.SporeSkeleton://635 Spore Skeleton
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/b/b5/Spore_Skeleton.png/revision/latest?cb=20200516193020&format=original";
				case NPCID.HallowBoss://636 Empress of Light
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/c/c9/Empress_of_Light.gif/revision/latest?cb=20210501121017&format=original";
				case NPCID.TownCat://637 Cat
					return "";
				case NPCID.TownDog://638 Dog
					return "";
				case NPCID.GemSquirrelAmethyst://639 Amethyst Squirrel
					return "";
				case NPCID.GemSquirrelTopaz://640 Topaz Squirrel
					return "";
				case NPCID.GemSquirrelSapphire://641 Sapphire Squirrel
					return "";
				case NPCID.GemSquirrelEmerald://642 Emerald Squirrel
					return "";
				case NPCID.GemSquirrelRuby://643 Ruby Squirrel
					return "";
				case NPCID.GemSquirrelDiamond://644 Diamond Squirrel
					return "";
				case NPCID.GemSquirrelAmber://645 Amber Squirrel
					return "";
				case NPCID.GemBunnyAmethyst://646 Amethyst Bunny
					return "";
				case NPCID.GemBunnyTopaz://647 Topaz Bunny
					return "";
				case NPCID.GemBunnySapphire://648 Sapphire Bunny
					return "";
				case NPCID.GemBunnyEmerald://649 Emerald Bunny
					return "";
				case NPCID.GemBunnyRuby://650 Ruby Bunny
					return "";
				case NPCID.GemBunnyDiamond://651 Diamond Bunny
					return "";
				case NPCID.GemBunnyAmber://652 Amber Bunny
					return "";
				case NPCID.HellButterfly://653 Hell Butterfly
					return "";
				case NPCID.Lavafly://654 Lavafly
					return "";
				case NPCID.MagmaSnail://655 Magma Snail
					return "";
				case NPCID.TownBunny://656 Bunny
					return "";
				case NPCID.QueenSlimeBoss://657 Queen Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/7/79/Queen_Slime.png/revision/latest?cb=20200524022713&format=original";
				case NPCID.QueenSlimeMinionBlue://658 Crystal Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/4/4f/Crystal_Slime.png/revision/latest?cb=20200516194836&format=original";
				case NPCID.QueenSlimeMinionPink://659 Bouncy Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/5a/Bouncy_Slime.png/revision/latest?cb=20200516194817&format=original";
				case NPCID.QueenSlimeMinionPurple://660 Heavenly Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/81/Heavenly_Slime.png/revision/latest?cb=20200712033312&format=original";
				case NPCID.EmpressButterfly://661 Prismatic Lacewing
					return "";
				case NPCID.PirateGhost://662 Pirate's Curse
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/5/59/Pirate%27s_Curse.png/revision/latest?cb=20200730145829&format=original";
				case NPCID.Princess://663 Princess
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f2/Princess.png/revision/latest?cb=20201013172546&format=original";
				case NPCID.TorchGod://664 The Torch God
					return "";
				case NPCID.ChaosBallTim://665 Chaos Ball
					return "";
				case NPCID.VileSpitEaterOfWorlds://666 Vile Spit
					return "";
				case NPCID.GoldenSlime://667 Golden Slime
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/f/f9/Golden_Slime.png/revision/latest?cb=20210516134412&format=original";
				case NPCID.Deerclops://668 Deerclops
					return "https://static.wikia.nocookie.net/terraria_gamepedia/images/8/85/Deerclops.png/revision/latest?cb=20211118192944&format=original";
				default:
					return "";
			}
		}
		public static string GetModNpcLink(this string modNpcFullName) {
			switch (modNpcFullName) {
				case "CalamityMod/DesertScourgeHead":
					return "https://calamitymod.wiki.gg/wiki/Desert_Scourge";
				case "ThoriumMod/TheGrandThunderBirdv2":
					return "https://thoriummod.wiki.gg/wiki/The_Grand_Thunder_Bird";
				case "ThoriumMod/TheBuriedWarrior":
					return "https://thoriummod.wiki.gg/wiki/Buried_Champion";
				case "ThoriumMod/GraniteEnergyStorm":
					return "https://thoriummod.wiki.gg/wiki/Granite_Energy_Storm";
				case "ThoriumMod/QueenJelly":
					return "https://thoriummod.wiki.gg/wiki/Queen_Jellyfish";
				default:
					return "";
			}
		}
	}

	public static class GameModeMethods {
		public static string ToGameModeIDName(this short id) {
			switch (id) {
				case GameModeID.Normal:
					return "Normal".Lang(L_ID1.Config);
				case GameModeID.Expert:
					return "Expert".Lang(L_ID1.Config);
				case GameModeID.Master:
					return "Master".Lang(L_ID1.Config);
				case GameModeID.Creative:
					return "Journey".Lang(L_ID1.Config);
				default:
					return "";
			}
		}
	}
}
