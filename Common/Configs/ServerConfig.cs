using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using WeaponEnchantments;
using WeaponEnchantments.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace WeaponEnchantments.Common.Configs
{
    [Label("Server Config")]
    public class ServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [Header("Server Config")]
        [Label("Presets and Multipliers")]
        [ReloadRequired]
        public PresetData presetData;

        /*public ComplexData complexData = new ComplexData();*/
        [JsonIgnore]//Not ready
        //[Label("Group Strength Multiplier")]
        //[Tooltip("Modify all strength multipliers for enchantments in the group. (Overrides Linear Strength Multiplier and above)")]
        [Range(1, 250)]
        [DefaultValue(100)]
        [ReloadRequired]
        public int strengthGroupMultiplier;

        [JsonIgnore]//Not ready
        //[Label("Strength Group")]
        //[Tooltip("Select multiple enchantments here to adjust all of their strengths by the chosen percentage. (Overrides Linear Strength Multiplier and above)")]
        [ReloadRequired]
        public HashSet<ItemDefinition> strengthGroups = new HashSet<ItemDefinition>();//Maybe enchantment strength catagories

        [Header("Individual Item Strengths")]
        [Label("Individual Strengths Enabled")]
        [Tooltip("Enabling this will cause the Indvidual strength values selected below to overite all other settings.")]
        [ReloadRequired]
        [DefaultValue(false)]
        public bool individualStrengthsEnabled;

        [Label("Individual Strengths")]
        [Tooltip("Modify individual enchantment strengths by value\n(NOT PERCENTAGE!)\n(Overrides all other options)")]
        [ReloadRequired]
        public List<Pair> individualStrengths = new List<Pair>();

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

        [Header("General Game Changes")]
        [Label("Convert excess armor penetration to bonus damage")]
        [Tooltip("Example: Enemy has 4 defense, Your weapon has 10 armor penetration.\n10 - 4 = 6 excess armor penetration (not doing anything)\nGain 3 bonus damage (6/2 = 3)")]
        [DefaultValue(true)]
        public bool TeleportEssence;

        [Header("Random Extra Stuff")]
        [Label("Start with a Drill Containment Unit")]
        [Tooltip("All players will get a Drill Containment Unit when they first spawn.\nThis is just for fun when you feel like a faster playthrough.")]
        [DefaultValue(false)]
        public bool DCUStart;

        //[Header("Client Config")]
        //public ClientConfig clientConfig = new();
        /*public class ClientConfig : ModConfig
        {
            public override ConfigScope Mode => ConfigScope.ClientSide;

            [Label("Automatically send essence to UI")]
            [Tooltip("Automatically send essence from your inventory to the UI essence slots.\n(Disables while the UI is open.)")]
            [DefaultValue(false)]
            public bool teleportEssence;

            [Label("Use Original Rarity Names")]
            [Tooltip("Use Original Rarity Names: Rare, Super Rare, Ultra Rare")]
            [DefaultValue(false)]
            [ReloadRequired]
            public bool UseOldRarityNames;
        }*/

        //[DefaultDictionaryKeyValue(0f)]
        /*[DefaultListValue(0f)]
        [Range(0f, 100f)]
        [ReloadRequired]
        public Dictionary<ItemDefinition, float> individualStrengths = new Dictionary<ItemDefinition, float>();//Maybe individual enchantment values*/

        /*[Label("ListOfPair2 label")]
        public List<Pair> ListOfPair2 = new List<Pair>();
        public Pair pairExample2 = new Pair();*/

        // This annotation allows the UI to null out this class. You need to make sure to initialize fields without the NullAllowed annotation in constructor or initializer or you might have issues. Of course, if you allow nulls, you'll need to make sure the rest of your mod will handle them correctly. Try to avoid null unless you have a good reason to use them, as null objects will only complicate the rest of your code.
        /*[NullAllowed]
        [JsonDefaultValue("{\"boost\": 777}")] // With NullAllowed, you can specify a default value like this.
        public PresetData presetData2;*/

        /*[JsonExtensionData]
        private IDictionary<string, JToken> _additionalData = new Dictionary<string, JToken>();*/

        // See _additionalData usage in OnDeserializedMethod to see how this modifiedEnchantmentStrengths can be populated from old versions of this mod.
        /*public List<int> modifiedEnchantmentStrengths = new List<int>();*/

        public ServerConfig()
        {
            presetData = new PresetData();
            /*presetData.boost = 32;
            presetData.percent = 0.7f;*/
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            // If you change ModConfig fields between versions, your users might notice their configuration is lost when they update their mod.
            // We can use [JsonExtensionData] to capture un-de-serialized data and manually restore them to new fields.
            // Imagine in a previous version of this mod, we had a field "OldmodifiedEnchantmentStrengths" and we want to preserve that data in "modifiedEnchantmentStrengths".
            // To test this, insert the following into ExampleMod_ModConfigShowcase.json: "OldmodifiedEnchantmentStrengths": [ 99, 999],
            /*if (_additionalData.TryGetValue("OldmodifiedEnchantmentStrengths", out var token))
            {
                var OldmodifiedEnchantmentStrengths = token.ToObject<List<int>>();
                modifiedEnchantmentStrengths.AddRange(OldmodifiedEnchantmentStrengths);
            }
            _additionalData.Clear(); // make sure to clear this or it'll crash.*/
        }
        /*public override bool NeedsReload(ModConfig pendingConfig)
        {
            return true;
        }*/
    }
    [Label("ClientConfig")]
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Automatically send essence to UI")]
        [Tooltip("Automatically send essence from your inventory to the UI essence slots.\n(Disables while the UI is open.)")]
        [DefaultValue(false)]
        public bool teleportEssence;

        [Label("Use Original Rarity Names")]
        [Tooltip("Use Original Rarity Names: Rare, Super Rare, Ultra Rare")]
        [DefaultValue(false)]
        [ReloadRequired]
        public bool UseOldRarityNames;
    }

    //[BackgroundColor(0, 255, 255)]
    //[Label("Pair label")]
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

        // If you override ToString, it will show up appended to the Label in the ModConfig UI.
        public override string ToString()
        {
            return $"Enchantment: {(itemDefinition != null && itemDefinition.Type != 0 ? $"{itemDefinition.Name}: {Strength / 10}%" : "None Selected")}";
        }

        // Implementing Equals and GetHashCode are critical for any classes you use.
        public override bool Equals(object obj)
        {
            if (obj is Pair other)
                return itemDefinition == other.itemDefinition && Strength == other.Strength;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { itemDefinition, Strength }.GetHashCode();
        }
    }
    //[BackgroundColor(255, 7, 7)]
    public class PresetData
    {
        [JsonIgnore]
        public static List<int> presetValues = new List<int> { 250, 100, 50, 25 };
        [JsonIgnore]
        public static List<string> presetNames = new List<string>() { "Journey", "Normal", "Expert", "Master" };

        [Header("Presets")]
        [DrawTicks]
        [OptionStrings(new string[] { "Journey", "Normal", "Expert", "Master", "Custom" })]
        [DefaultValue("Normal")]
        [Tooltip("Journey, Normal, Expert, Master, Custom \n(Custom can't be selected here.  It is set automatically when adjusting the Recomended Strength Multiplier.)")]
        [ReloadRequired]
        public string Preset
        {
            get => presetValues.Contains(recomendedStrengthMultiplier) ? presetNames[presetValues.IndexOf(recomendedStrengthMultiplier)] : "Custom";
            set
            {
                if (presetNames.Contains(value))
                {
                    recomendedStrengthMultiplier = presetValues[presetNames.IndexOf(value)];
                    linearStrengthMultiplier = 100;
                }
            }
        }

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

        public PresetData()
        {
            Preset = "Normal";
        }

        public override bool Equals(object obj)
        {
            if (obj is PresetData other)
                return Preset == other.Preset && recomendedStrengthMultiplier == other.recomendedStrengthMultiplier && linearStrengthMultiplier == other.linearStrengthMultiplier;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { Preset, recomendedStrengthMultiplier, linearStrengthMultiplier }.GetHashCode();
        }
    }

    public class ComplexData
    {
        [Label("Strength Presets")]
        [Tooltip("Adjust all enchantment strengths to one of 4 recomended preset values.")]
        [ReloadRequired]
        public PresetData nestedSimple = new PresetData();

        /*[Range(1, 250)]
        public List<int> modifiedEnchantmentStrengths = new List<int>();//Maybe use for individual enchantment strengths*/


        /*[Range(2f, 3f)]
        [Increment(.25f)]
        [DrawTicks]
        [DefaultValue(2f)]
        public float IncrementalFloat = 2f;*/
        public override bool Equals(object obj)
        {
            if (obj is ComplexData other)
                return /*modifiedEnchantmentStrengths.SequenceEqual(other.modifiedEnchantmentStrengths) && IncrementalFloat == other.IncrementalFloat && */nestedSimple.Equals(other.nestedSimple);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return new { /*modifiedEnchantmentStrengths, */nestedSimple/*, IncrementalFloat*/ }.GetHashCode();
        }
    }
}
