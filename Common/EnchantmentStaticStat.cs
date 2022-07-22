using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common
{
    public class EnchantmentStaticStat
    {
        public string Name { get; private set; }
        public float Additive { get; private set; }
        public float Multiplicative { get; private set; }
        public float BaseValueFloat {get; set;}
        public float Flat { get; private set; }
        public float Base { get; private set; }
        public bool PreventBoolStat { get; private set; }
        //public bool Inverse { get; private set; }
        public EnchantmentStaticStat(string name, float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f)
        {
            Additive = additive;
            Multiplicative = multiplicative;
            BaseValueFloat = 0f;
            Flat = flat;
            Base = @base;
            /*PreventBoolStat = name.Substring(0, 2) == "P_";
            Inverse = name.Substring(0, 2) == "I_";
            Name = PreventBoolStat || Inverse ? name.Substring(2) : name;*/
            //Inverse = name.Substring(0, 2) == "I_";
            //Name = Inverse ? name.Substring(2) : name;
            Name = name;
        }
        /*public void UpdateStat(ref Item item, string name, bool remove, bool property = false) 
        {
            Item contentSampleItem = ContentSamples.ItemsByType[item.type].Clone();
            Type contentSampleItemType = contentSampleItem.GetType();
            float multiplier = (1f + Additive) * Multiplicative - 1f;
            float finalMultiplier = 1f;
            var baseValue = item.GetType().GetField(name).GetValue(item);
            if (remove)
                finalMultiplier = 1f / (1f + multiplier * BaseValueFloat / baseValue);
            if (!property)
            {
                if (!remove)
                {
                    FieldInfo contentSampleItemField = contentSampleItemType.GetField(name);
                    float contentSampleItemFieldValue = ()(contentSampleItemField.GetValue(contentSampleItem));
                    finalMultiplier = 1f + multiplier * contentSampleItemFieldValue / baseValue;
                    BaseValueFloat = contentSampleItemFieldValue;
                }
                item.GetType().GetField(name).SetValue(item, baseValue * finalMultiplier);
            }
            else
            {
                if (!remove)
                {
                    PropertyInfo contentSampleItemproperty = contentSampleItemType.GetProperty(name);
                    float contentSampleItemPropertyValue = (float)(contentSampleItemproperty.GetValue(contentSampleItem));
                    finalMultiplier = 1f + multiplier * contentSampleItemPropertyValue / baseValue;
                    BaseValueFloat = contentSampleItemPropertyValue;
                }
                item.GetType().GetProperty(name).SetValue(item, baseValue * finalMultiplier);
            }
        }*/
        
        /*public void UpdatePlayerStat(ref Item item, string name, bool remove, bool boolStat, bool boolRestricted, AllForOneEnchantmentBasic enchantment, bool property = false) 
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            EnchantedItem iGlobal = item.G();
            Item contentSampleItem = ContentSamples.ItemsByType[item.type].Clone();
            Player contentSamplePlayer = new Player();
            if (boolStat)
            {
                if (boolRestricted)
                {
                    wePlayer.boolPreventedFields.EditBoolField(name, remove);
                    
                }
                wePlayer.boolFields.EditBoolField(name, remove);

                bool updatePlayer = !property ? wePlayer.Player.GetType().GetField(name) != null : wePlayer.Player.GetType().GetProperty(name) != null;
                if (updatePlayer)
                {
                    bool boolStatFinalValue = wePlayer.boolFields.ContainsKey(name);
                    bool restricted = wePlayer.boolPreventedFields.ContainsKey(name);
                    bool defaultValue = (bool)contentSamplePlayer.GetType().GetField(name).GetValue(contentSamplePlayer);
                    bool boolStatSetValue = (defaultValue || boolStatFinalValue) && !restricted;
                    if (!property)
                    {
                        wePlayer.Player.GetType().GetField(name).SetValue(wePlayer.Player, boolStatSetValue);
                    }
                    else
                    {
                        wePlayer.Player.GetType().GetProperty(name).SetValue(wePlayer.Player, boolStatSetValue);
                    }
                }
            }
            else
            {
                float additive;
                float multiplicative;
                float additiveDenom;
                float multiplicativeDenom;
                /*if (!iGlobal.statModifiers.ContainsKey(name))
                {
                    iGlobal.statModifiers.Add(name, new StatModifier(1f, 1f));
                }*//*
                if (!iGlobal.statModifiers.ContainsKey(name))
                {
                    iGlobal.statModifiers.Add(name, new StatModifier(1f, 1f));
                }
                float add = Additive;
                float mult = Multiplicative;
                float f = Flat;
                float b = Base;
                //UtilityMethods.ApplyAllowedList(item, enchantment, ref add, ref mult, ref f, ref b);
                if (remove)
                {
                    add *= -1;
                    mult *= -1;
                    f *= -1;
                    b *= -1;
                }
                additiveDenom = iGlobal.statModifiers[name].Additive;
                multiplicativeDenom = iGlobal.statModifiers[name].Multiplicative;
                float previousFlat = iGlobal.statModifiers[name].Flat;
                float previousBase = iGlobal.statModifiers[name].Base;
                StatModifier newStatModifier = new StatModifier(1f + add, 1f + mult, f, b);
                iGlobal.statModifiers[name] = iGlobal.statModifiers[name].CombineWith(newStatModifier);
                float flat = iGlobal.statModifiers[name].Flat;
                float @base = iGlobal.statModifiers[name].Base;
                additive = iGlobal.statModifiers[name].Additive;
                multiplicative = iGlobal.statModifiers[name].Multiplicative;
                if (additive == 1 && multiplicative == 1 && flat == 0f && @base == 0f)
                {
                    iGlobal.statModifiers.Remove(name);
                }
                if (Inverse)
                {
                    additiveDenom = 1f / additiveDenom;
                    multiplicativeDenom = 1f / multiplicativeDenom;
                    additive = 1f / additive;
                    multiplicative = 1f / multiplicative;
                    flat *= -1;
                    @base *= -1;
                    previousFlat *= -1;
                    previousBase *= -1;
                }
                Type contentSampleItemType;
                if (!property)
                    contentSampleItemType = contentSampleItem.GetType().GetField(name).FieldType;
                else
                    contentSampleItemType = contentSampleItem.GetType().GetProperty(name).PropertyType;
                float sampleValue = 0f;
                float baseValue = 0f;
                if (contentSampleItemType == typeof(float))
                {
                    baseValue = (float)item.GetType().GetField(name).GetValue(item);
                    sampleValue = (float)contentSampleItem.GetType().GetField(name).GetValue(contentSampleItem);
                }
                else if (contentSampleItemType == typeof(int))
                {
                    int baseValueInt = (int)item.GetType().GetField(name).GetValue(item);
                    baseValue = (float)baseValueInt;
                    int sampleValueInt = (int)contentSampleItem.GetType().GetField(name).GetValue(contentSampleItem);
                    sampleValue = (float)sampleValueInt;
                }
                float sampleValueTemp = sampleValue;
                BaseValueFloat = baseValue;
                float finalValue = ((baseValue / additiveDenom / multiplicativeDenom) + @base - previousBase) * additive * multiplicative + flat - previousFlat;
                if (!property)
                {
                    if (contentSampleItemType == typeof(float))
                    {
                        item.GetType().GetField(name).SetValue(item, finalValue);
                    }
                    else if (contentSampleItemType == typeof(int))
                    {
                        item.GetType().GetField(name).SetValue(item, (int)Math.Round(finalValue + 5E-6));
                    }
                    var tempForBreakpointCheck = item.GetType().GetField(name).GetValue(item);
                }
                else
                {
                    if (contentSampleItemType == typeof(float))
                    {
                        item.GetType().GetProperty(name).SetValue(item, finalValue);
                    }
                    else if (contentSampleItemType == typeof(int))
                    {
                        item.GetType().GetProperty(name).SetValue(item, (int)Math.Round(finalValue + 5E-6));
                    }
                    var tempForBreakpointCheck = item.GetType().GetProperty(name).GetValue(item);
                }
            }
            /*WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            EnchantedItem iGlobal = item.G();
            Item contentSampleItem = ContentSamples.ItemsByType[item.type].Clone();
            if (boolStat)
            {
                if (boolRestricted)
                {
                    iGlobal.boolPreventedFields.EditBoolField(name, remove);
                }
                iGlobal.boolFields.EditBoolField(name, remove);
                bool boolStatFinalValue = iGlobal.boolFields.ContainsKey(name);
                bool restricted = iGlobal.boolPreventedFields.ContainsKey(name);
                bool defaultValue = (bool)contentSampleItem.GetType().GetField(name).GetValue(contentSampleItem);
                bool boolStatSetValue = (defaultValue || boolStatFinalValue) && !restricted;
                if (!property)
                    item.GetType().GetField(name).SetValue(item, boolStatSetValue);
                else
                    item.GetType().GetProperty(name).SetValue(item, boolStatSetValue);
            }
            else
            {
                float additive;
                float multiplicative;
                float additiveDenom;
                float multiplicativeDenom;
                /*if (!iGlobal.statModifiers.ContainsKey(name))
                {
                    iGlobal.statModifiers.Add(name, new StatModifier(1f, 1f));
                }*//*
            if (!iGlobal.statModifiers.ContainsKey(name))
            {
                iGlobal.statModifiers.Add(name, new StatModifier(1f, 1f));
            }
            float add = Additive;
            float mult = Multiplicative;
            float f = Flat;
            float b = Base;
            UtilityMethods.ApplyAllowedList(item, enchantment, ref add, ref mult, ref f, ref b);
            if (remove)
            {
                add *= -1;
                mult *= -1;
                f *= -1;
                b *= -1;
            }
            additiveDenom = iGlobal.statModifiers[name].Additive;
            multiplicativeDenom = iGlobal.statModifiers[name].Multiplicative;
            float previousFlat = iGlobal.statModifiers[name].Flat;
            float previousBase = iGlobal.statModifiers[name].Base;
            StatModifier newStatModifier = new StatModifier(1f + add, 1f + mult, f, b);
            iGlobal.statModifiers[name] = iGlobal.statModifiers[name].CombineWith(newStatModifier);
            float flat = iGlobal.statModifiers[name].Flat;
            float @base = iGlobal.statModifiers[name].Base;
            additive = iGlobal.statModifiers[name].Additive;
            multiplicative = iGlobal.statModifiers[name].Multiplicative;
            if (additive == 1 && multiplicative == 1 && flat == 0f && @base == 0f)
            {
                iGlobal.statModifiers.Remove(name);
            }
            if (Inverse)
            {
                additiveDenom = 1f / additiveDenom;
                multiplicativeDenom = 1f / multiplicativeDenom;
                additive = 1f / additive;
                multiplicative = 1f / multiplicative;
                flat *= -1;
                @base *= -1;
                previousFlat *= -1;
                previousBase *= -1;
            }
            Type contentSampleItemType;
            if (!property)
                contentSampleItemType = contentSampleItem.GetType().GetField(name).FieldType;
            else
                contentSampleItemType = contentSampleItem.GetType().GetProperty(name).PropertyType;
            float sampleValue = 0f;
            float baseValue = 0f;
            if (contentSampleItemType == typeof(float))
            {
                baseValue = (float)item.GetType().GetField(name).GetValue(item);
                sampleValue = (float)contentSampleItem.GetType().GetField(name).GetValue(contentSampleItem);
            }
            else if (contentSampleItemType == typeof(int))
            {
                int baseValueInt = (int)item.GetType().GetField(name).GetValue(item);
                baseValue = (float)baseValueInt;
                int sampleValueInt = (int)contentSampleItem.GetType().GetField(name).GetValue(contentSampleItem);
                sampleValue = (float)sampleValueInt;
            }
            float sampleValueTemp = sampleValue;
            BaseValueFloat = baseValue;
            float finalValue = ((baseValue / additiveDenom / multiplicativeDenom) + @base - previousBase) * additive * multiplicative + flat - previousFlat;
            if (!property)
            {
                if (contentSampleItemType == typeof(float))
                {
                    item.GetType().GetField(name).SetValue(item, finalValue);
                }
                else if (contentSampleItemType == typeof(int))
                {
                    item.GetType().GetField(name).SetValue(item, (int)Math.Round(finalValue + 5E-6));
                }
                var tempForBreakpointCheck = item.GetType().GetField(name).GetValue(item);
            }
            else
            {
                if (contentSampleItemType == typeof(float))
                {
                    item.GetType().GetProperty(name).SetValue(item, finalValue);
                }
                else if (contentSampleItemType == typeof(int))
                {
                    item.GetType().GetProperty(name).SetValue(item, (int)Math.Round(finalValue + 5E-6));
                }
                var tempForBreakpointCheck = item.GetType().GetProperty(name).GetValue(item);
            }
        }*/
        /*public void UpdateBoolStat(ref Item item, string name, bool remove, bool boolStat, bool boolRestricted, AllForOneEnchantmentBasic enchantment, bool property = false)
        {
            if (boolStat)
            {
                bool updateItem = !property ? item.GetType().GetField(name) != null : item.GetType().GetProperty(name) != null;
                if (updateItem)
                {
                    iGlobal.boolPreventedFields.EditBoolField(name, remove, wePlayer.boolPreventedFields);
                    iGlobal.boolFields.EditBoolField(name, remove, wePlayer.boolFields);
                    bool boolStatFinalValue = iGlobal.boolFields.ContainsKey(name);
                    bool restricted = iGlobal.boolPreventedFields.ContainsKey(name);
                    bool defaultValue = (bool)contentSampleItem.GetType().GetField(name).GetValue(contentSampleItem);
                    bool boolStatSetValue = (defaultValue || boolStatFinalValue) && !restricted;
                    if (!property)
                    {
                        item.GetType().GetField(name).SetValue(item, boolStatSetValue);
                    }
                    else
                    {
                        item.GetType().GetProperty(name).SetValue(item, boolStatSetValue);
                    }
                }
            }
        }*/
    }
}
