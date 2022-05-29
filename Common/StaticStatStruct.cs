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

namespace WeaponEnchantments.Common
{
    public class StaticStatStruct
    {
        public string Name { get; private set; }
        public float Additive { get; private set; }
        public float Multiplicative { get; private set; }
        public float BaseValueFloat {get; set;}
        public float Flat { get; private set; }
        public float Base { get; private set; }
        public bool PreventBoolStat { get; private set; }
        public bool Inverse { get; private set; }
        public StaticStatStruct(string name, float additive = 0f, float multiplicative = 0f, float flat = 0f, float @base = 0f)
        {
            Additive = additive;
            Multiplicative = multiplicative;
            BaseValueFloat = 0f;
            Flat = flat;
            Base = @base;
            PreventBoolStat = name.Substring(0, 2) == "P_";
            Inverse = name.Substring(0, 2) == "I_";
            Name = PreventBoolStat || Inverse ? name.Substring(2) : name;
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
        
        public void UpdateStat(ref Item item, string name, bool remove, bool boolStat, bool boolRestricted, bool property = false) 
        {
            EnchantedItem iGlobal = item.GetGlobalItem<EnchantedItem>();
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
                if (!iGlobal.statMultipliers.ContainsKey(name))
                {
                    iGlobal.statMultipliers.Add(name, new StatModifier(1f, 1f));
                }
                float add = Additive;
                float mult = Multiplicative;
                float f = Flat;
                float b = Base;
                if (remove)
                {
                    add *= -1;
                    mult *= -1;
                    f *= -1;
                    b *= -1;
                }
                additiveDenom = iGlobal.statMultipliers[name].Additive;
                multiplicativeDenom = iGlobal.statMultipliers[name].Multiplicative;
                float previousFlat = iGlobal.statMultipliers[name].Flat;
                float previousBase = iGlobal.statMultipliers[name].Base;
                StatModifier newStatModifier = new StatModifier(1f + add, 1f + mult, f, b);
                iGlobal.statMultipliers[name] = iGlobal.statMultipliers[name].CombineWith(newStatModifier);
                float flat = iGlobal.statMultipliers[name].Flat;
                float @base = iGlobal.statMultipliers[name].Base;
                additive = iGlobal.statMultipliers[name].Additive;
                multiplicative = iGlobal.statMultipliers[name].Multiplicative;
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
                if (additive == 1 && multiplicative == 1)
                {
                    iGlobal.statMultipliers.Remove(name);
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
        }
    }
}
