using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace WeaponEnchantments.Common.Configs
{
    [Label("Server Config")]
    public class ServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //Server Config
        [Header("Server Config")]
        [Label("Presets and Multipliers")]
        [ReloadRequired]
        public PresetData presetData;

        [Header("Individual Enchantment Strengths")]

        [Label("Individual Strengths Enabled")]
        [Tooltip("Enabling this will cause the Indvidual strength values selected below to overite all other settings.")]
        [ReloadRequired]
        [DefaultValue(false)]
        public bool individualStrengthsEnabled;

        [Label("Individual Strengths")]
        [Tooltip("Modify individual enchantment strengths by value\n(NOT PERCENTAGE!)\n(Overrides all other options)")]
        public List<Pair> individualStrengths = new List<Pair>();

        //Enchantment Settings
        [Header("Enchantment Settings")]
        [Label("Damage type converting enchantments always override.")]
        [Tooltip("Some mods like Stars Above change weapon damage types.  If this option is enabled, Enchantments that change the damage type will always change the weapon's damage type.\n" +
			"If not selected, the damage type will only be changed if the weapon is currently it's original damage type.")]
        [DefaultValue(true)]
        public bool AlwaysOverrideDamageType;

        [Label("Life Steal Enchantment limiting (Affect on Vanilla Life Steal Limit) (%)")]
        [Tooltip("Use a value above 100% to limmt lifesteal more, less than 100% to limit less.  0 to have not limit.\n" +
            "Vanilla Terraria uses a lifesteal limiting system: In the below example, the values used are in normal mode(Expert/Master mode values in parenthesis)\n" +
            "It has a pool of 80(70) that is saved for you to gain lifestea from.  Gaining life through lifesteal reduces this pool.\n" +
            "The pool is restored by 36(30) points per second.  If the pool value is negative, you cannot gain life from lifesteal.\n" +
            "This config value changes how much the life you heal from lifesteal enchantments affects this limit.\n" +
            "Example: 200%  You gain 200 life from lifesteal.  200 * 200% = 400.  80(70) pool - 400 healed = -320(-330) pool.\n" +
            "It will take 320/36(330/30) seconds -> 8.9(11) seconds for the pool to be positive again so you can gain life from lifesteal again.\n" +
            "Note: the mechanic does not have a cap on how much you can gain at once.  It will just take longer to recover the more you gain.")]
        [DefaultValue(100)]
        [Range(0, 10000)]
        public int AffectOnVanillaLifeStealLimmit;

        [Label("Speed Enchantment Auto Reuse Enabled (%)")]
        [Tooltip("The strength that a Speed Enchantment will start giving the Auto Reuse stat.\n" +
			"Set to 0 for all Speed enchantments to give auto reuse.  Set to 10000 to to prevent any gaining auto reuse (unless you strength multiplier is huge)")]
        [Range(0, 10000)]
        [DefaultValue(10)]
        public int SpeedEnchantmentAutoReuseSetpoint;

        //Essence and Experience
        [Header("Essence and Experience")]
        [Label("Boss Essence Multiplier(%)")]
        [Tooltip("Modify the ammount of essence recieved by bosses.")]
        [Range(0, 10000)]
        [DefaultValue(100)]
        [ReloadRequired]
        public int BossEssenceMultiplier;

        [Label("Non-Boss Essence Multiplier(%)")]
        [Tooltip("Modify the ammount of essence recieved by non-boss enemies.")]
        [Range(0, 10000)]
        [DefaultValue(100)]
        [ReloadRequired]
        public int EssenceMultiplier;

        [Label("Boss Experience Multiplier(%)")]
        [Tooltip("Modify the ammount of experience recieved by bosses.")]
        [Range(0, 10000)]
        [DefaultValue(100)]
        public int BossExperienceMultiplier;

        [Label("Non-Boss Experience Multiplier(%)")]
        [Tooltip("Modify the ammount of experience recieved by non-boss enemies.")]
        [Range(0, 10000)]
        [DefaultValue(100)]
        public int ExperienceMultiplier;

        [Label("Gathering Experience Multiplier(%)")]
        [Tooltip("Modify the ammount of experience recieved from Mining/chopping/fishing")]
        [Range(0, 10000)]
        [DefaultValue(100)]
        public int GatheringExperienceMultiplier;

        //Enchantment Drop Rates(%)
        [Header("Enchantment Drop Rates(%)")]
        [Label("Boss Enchantment Drop Rate")]
        [Tooltip("Adjust the drop rate of enchantments from bosses.\n(Default is 50%)")]
        [Range(0, 100)]
        [DefaultValue(50)]
        [ReloadRequired]
        public int BossEnchantmentDropChance;

        [Label("Non-Boss Enchantment Drop Rate(%)")]
        [Tooltip("Adjust the drop rate of enchantments from non -boss enemies.\n(Default is 100%)")]
        [Range(0, 1000)]
        [DefaultValue(100)]
        [ReloadRequired]
        public int EnchantmentDropChance;

        //Other Drop Rates
        [Header("Other Drop Rates")]
        [Label("Prevent pre-hard mode bosses from dropping power boosters.")]
        [Tooltip("Does not include wall of flesh.")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PreventPowerBoosterFromPreHardMode;

        //Enchanting Table Options
        [Header("Enchanting Table Options")]
        [Label("Recieve ores up to Chlorophyte from Offering items.")]
        [Tooltip("Disabling this option only allows you to recieve Iron, Silver, Gold (Or their equivelents based on world gen.).\n" +
			"(Only Works in hard mode.  Chlorophyte only after killing a mechanical boss.)")]
        [DefaultValue(true)]
        public bool AllowHighTierOres;

        [Label("Percentage of offered Item value converted to essence.")]
        [DefaultValue(50)]
        [Range(0, 100)]
        public int PercentOfferEssence;

        //General Game Changes
        [Header("General Game Changes")]
        [Label("Convert excess armor penetration to bonus damage")]
        [Tooltip("Example: Enemy has 4 defense, Your weapon has 10 armor penetration.\n" +
			"10 - 4 = 6 excess armor penetration (not doing anything)\nGain 3 bonus damage (6/2 = 3)")]
        [DefaultValue(true)]
        public bool ArmorPenetration;

        [Label("Disable Minion Critical hits")]
        [Tooltip("In vanilla, minions arent affected by weapon critical chance.\n" +
			"Weapon Enchantments gives minions a critical hit chance based on weapon crit chance.\n" +
			"This option disables the crits(vanilla mechanics)")]
        [DefaultValue(false)]
        public bool DisableMinionCrits;

        [Label("Disable Item critical strike chance per level")]
        [Tooltip("Items gain critical strike chance equal to thier level * Enchantment strength multiplier.")]
        [DefaultValue(false)]
        public bool CritPerLevelDisabled;

        [Label("Multiplicative critical hits past the first.")]
        [Tooltip("Weapon Enchantments makes use of critical strike chance past 100% to allow you to crit again.\n" +
            "By default, this is an additive bonus: 1st crit 200% damage, 2nd 300% damage, 3rd 400% damage.....\n" +
            "Enabling this makes them multiplicative instead: 1st crit 200% damage, 2nd crit 400% damage, 3rd crit 400% damage... ")]
        [DefaultValue(false)]
        public bool MultiplicativeCriticalHits;

        [Label("Infusion Damage Multiplier (Divides by 1000, 1 -> 0.001)")]
        [DefaultValue(1300)]
        [Range(1000, 2000)]
        [Tooltip("Changes the damage multiplier from infusion.  DamageMultiplier = InfusionDamageMultiplier^((InfusionPower - BaseInfusionPower) / 100)\n" +
			"Example: Iron Broadsword, Damage = 10, BaseInfusionPower = 31  infused with a Meowmere, Infusion Power 1100.\n" +
			"Iron Broadsword damage = 10 * 1.3^((1100 - 31) / 100) = 10 * 1.3^10.69 = 10 * 16.52 = 165 damage")]
        public int InfusionDamageMultiplier;

        [Label("Minion Life Steal Multiplier (%)")]
        [Tooltip("Allows you to reduce the ammount of healing recieved by minions with the Lifesteal Enchantment.")]
        [DefaultValue(50)]
        [Range(0, 100)]
        public int MinionLifeStealMultiplier;

        //Random Extra Stuff
        [Header("Random Extra Stuff")]
        [Label("Start with a Drill Containment Unit")]
        [Tooltip("All players will get a Drill Containment Unit when they first spawn.\nThis is just for fun when you feel like a faster playthrough.")]
        [DefaultValue(false)]
        public bool DCUStart;

        public ServerConfig() {
            presetData = new PresetData();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context) {
            // If you change ModConfig fields between versions, your users might notice their configuration is lost when they update their mod.
            // We can use [JsonExtensionData] to capture serialized data and manually restore them to new fields.
            // Imagine in a previous version of this mod, we had a field "OldmodifiedEnchantmentStrengths" and we want to preserve that data in "modifiedEnchantmentStrengths".
            // To test this, insert the following into ExampleMod_ModConfigShowcase.json: "OldmodifiedEnchantmentStrengths": [ 99, 999],
            /*if (_additionalData.TryGetValue("OldmodifiedEnchantmentStrengths", out var token))
            {
                var OldmodifiedEnchantmentStrengths = token.ToObject<List<int>>();
                modifiedEnchantmentStrengths.AddRange(OldmodifiedEnchantmentStrengths);
            }
            _additionalData.Clear(); // make sure to clear this or it'll crash.*/
        }
    }

    [Label("ClientConfig")]
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        //Enchanting Table Options
        [Header("Enchanting Table Options")]
        [Label("Automatically send essence to UI")]
        [Tooltip("Automatically send essence from your inventory to the UI essence slots.\n(Disables while the UI is open.)")]
        [DefaultValue(true)]
        public bool teleportEssence;
        
        [Label("Offer all of the same item.")]
        [Tooltip("Search your inventory for all items of the same type that was offered and offer them too if they have 0 experience and no power booster installed.")]
        [DefaultValue(false)]
        public bool OfferAll;

        [Label("Always display Infusion Power")]
        [Tooltip("Enable to display item's Infusion Power always instead of just when the enchanting table is open.")]
        [DefaultValue(false)]
        public bool AlwaysDisplayInfusionPower;

        //Display Settings
        [Header("Display Settings")]
        [Label("Use Original Rarity Names")]
        [Tooltip("Use Original Rarity Names: Rare, Super Rare, Ultra Rare")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool UseOldRarityNames;

        [Label("\"Points\" instead of \"Enchantment Capacity\"")]
        [Tooltip("Tooltips will show Points Available instead of Enchantment Capacity Available")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool UsePointsAsTooltip;

        [Label("Use Alternate Enchantment Essence Textures")]
        [Tooltip("The default colors are color blind friendly.  The alternate textures have minor differences, but were voted to be kept.")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool UseAlternateEnchantmentEssenceTextures;
    }
    public class Pair
    {
        [Tooltip("Only Select Enchantment Items.\nLikely to cause an error if selecting any other item.")]
        [Label("Enchantment")]
        [ReloadRequired]
        public ItemDefinition itemDefinition;

        [Label("Strength (1000 = 1, 10 = 1%)")]
        [Tooltip("Take care when adjusting this value.\nStrength is the exact value used.\nExample 40% Damage enchantment is 0.4\n10 Defense is 10")]
        [Range(0, 100000)]
        [ReloadRequired]
        public int Strength;

        public override string ToString(){
            return $"Enchantment: {(itemDefinition != null && itemDefinition.Type != 0 ? $"{itemDefinition.Name}: {Strength / 10}%" : "None Selected")}";
        }

        public override bool Equals(object obj){
            if (obj is Pair other)
                return itemDefinition == other.itemDefinition && Strength == other.Strength;
            
            return base.Equals(obj);
        }

        public override int GetHashCode(){
            return new { itemDefinition, Strength }.GetHashCode();
        }
    }
    public class PresetData
    {
        [JsonIgnore]
        public static List<int> presetValues = new List<int> { 250, 100, 50, 25 };

        [JsonIgnore]
        public static List<string> presetNames = new List<string>() { "Journey", "Normal", "Expert", "Master" };

        //Presets
        [Header("Presets")]
        [DrawTicks]
        [OptionStrings(new string[] { "Journey", "Normal", "Expert", "Master", "Custom" })]
        [DefaultValue("Normal")]
        [Tooltip("Journey, Normal, Expert, Master, Custom \n(Custom can't be selected here.  It is set automatically when adjusting the Recomended Strength Multiplier.)")]
        [ReloadRequired]
        public string Preset {
            get => presetValues.Contains(recomendedStrengthMultiplier) ? presetNames[presetValues.IndexOf(recomendedStrengthMultiplier)] : "Custom";
            set {
                if (presetNames.Contains(value)) {
                    recomendedStrengthMultiplier = presetValues[presetNames.IndexOf(value)];
                    linearStrengthMultiplier = 100;
                }
            }
        }

        //Multipliers
        [Header("Multipliers")]
        [Label("Recomended Strength Multiplier(%)")]
        [Range(1, 250)]
        [DefaultValue(100)]
        [Tooltip("Adjusts all enchantment strengths based on recomended enchantment changes." +
            "\nUses the same calculations as the presets but allows you to pick a different number." +
            "\npreset values are; Journey: 250, Normal: 100, Expert: 50, Master: 25 (Overides Ppreset)")]
        [ReloadRequired]
        public int recomendedStrengthMultiplier { get; set; }

        [Label("Linear Strength Multiplier(%)")]
        [Range(1, 250)]
        [DefaultValue(100)]
        [Tooltip("Adjusts all enchantment strengths linearly\n(Overides Recomended Strength Multiplier and above)")]
        [ReloadRequired]
        public int linearStrengthMultiplier { get; set; }

        public PresetData() {
            Preset = "Normal";
        }

        public override bool Equals(object obj) {
            if (obj is PresetData other)
                return Preset == other.Preset && recomendedStrengthMultiplier == other.recomendedStrengthMultiplier && linearStrengthMultiplier == other.linearStrengthMultiplier;
            
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return new { Preset, recomendedStrengthMultiplier, linearStrengthMultiplier }.GetHashCode();
        }
    }

    public class ComplexData
    {
        [Label("Strength Presets")]
        [Tooltip("Adjust all enchantment strengths to one of 4 recomended preset values.")]
        [ReloadRequired]
        public PresetData nestedSimple = new PresetData();

        public override bool Equals(object obj) {
            if (obj is ComplexData other)
                return nestedSimple.Equals(other.nestedSimple);
            
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return new { nestedSimple }.GetHashCode();
        }
    }
}
