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

namespace WeaponEnchantments.Common
{
    public class StaticStatStruct
    {
        public string Name { get; private set; }
        public float Additive { get; private set; }
        public float Multiplicative { get; private set; }
        public float BaseValueFloat {get; set;}
        public StaticStatStruct(string name, float additive = 0f, float multiplicative = 0f)
        {
            Name = name;
            Additive = additive;
            Multiplicative = multiplicative;
            BaseValueFloat = 0f;
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
        public void UpdateStat(ref Item item, string name, bool remove, bool property = false) 
        {
            EnchantedItem iGlobal = item.GetGlobalItem<EnchantedItem>();
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
            if (remove)
            {
                add *= -1;
                mult *= -1;
            }
            additiveDenom = iGlobal.statMultipliers[name].Additive;
            multiplicativeDenom = iGlobal.statMultipliers[name].Multiplicative;
            iGlobal.statMultipliers[name] += add;
            iGlobal.statMultipliers[name] += mult;
            additive = iGlobal.statMultipliers[name].Additive;
            multiplicative = iGlobal.statMultipliers[name].Multiplicative;
            if (additive == 1 && multiplicative == 1)
            {
                iGlobal.statMultipliers.Remove(name);
            }
            Item contentSampleItem = ContentSamples.ItemsByType[item.type].Clone();
            Type contentSampleItemType = contentSampleItem.GetType();
            float multiplier = (additive)/(additiveDenom) * (multiplicative)/(multiplicativeDenom);
            float baseValue = (float)item.GetType().GetField(name).GetValue(item);
            /*if (remove)
                finalMultiplier = 1 / multiplier;
            else
                finalMultiplier = multiplier;*/
            if (!property)
            {
                if (!remove)
                {
                    FieldInfo contentSampleItemField = contentSampleItemType.GetField(name);
                    float contentSampleItemFieldValue = (float)(contentSampleItemField.GetValue(contentSampleItem));
                    BaseValueFloat = contentSampleItemFieldValue;
                }
                item.GetType().GetField(name).SetValue(item, baseValue * multiplier);
                float tempForBreakpointCheck = (float)item.GetType().GetField(name).GetValue(item);
            }
            else
            {
                if (!remove)
                {
                    PropertyInfo contentSampleItemproperty = contentSampleItemType.GetProperty(name);
                    float contentSampleItemPropertyValue = (float)(contentSampleItemproperty.GetValue(contentSampleItem));
                    BaseValueFloat = contentSampleItemPropertyValue;
                }
                item.GetType().GetProperty(name).SetValue(item, baseValue * multiplier);
                float tempForBreakpointCheck = (float)item.GetType().GetProperty(name).GetValue(item);
            }
        }
        /*public void RemoveFrom(ref Item item, string name, float baseValue, bool property = false)
        {
            float multiplier = (1f + Additive) * Multiplicative - 1f;
            float finalMultiplier = 1f / (1f + multiplier * BaseValueFloat / baseValue);
            if (!property)
            {
                item.GetType().GetField(name).SetValue(item, baseValue * finalMultiplier);
                float tempForBreakpointCheck = (float)item.GetType().GetField(name).GetValue(item);
            }
            else
            {
                item.GetType().GetProperty(name).SetValue(item, baseValue * finalMultiplier);
                float tempForBreakpointCheck = (float)item.GetType().GetProperty(name).GetValue(item);
            }
        }*/
        /*public void ApplyTo(ref Item item, string name, int baseValue, bool remove, bool property = false) 
        {
            Item contentSampleItem = ContentSamples.ItemsByType[item.type].Clone();
            Type contentSampleItemType = contentSampleItem.GetType();
            float multiplier = (1f + Additive) * Multiplicative - 1f;
            float finalMultiplier = 1f;
            if(remove)
                finalMultiplier = 1f / (1f + multiplier * BaseValueFloat / baseValue);
            if (!property)
            {
                FieldInfo contentSampleItemField = contentSampleItemType.GetField(name);
                int contentSampleItemFieldValue = (int)(contentSampleItemField.GetValue(contentSampleItem));
                finalMultiplier = 1f + multiplier * contentSampleItemFieldValue / baseValue;
                item.GetType().GetField(name).SetValue(item, (int)(baseValue * finalMultiplier + 5E-06f));
                int tempForBreakpointCheck = (int)item.GetType().GetField(name).GetValue(item);
                BaseValueInt = contentSampleItemFieldValue;
            }
            else
            {
                PropertyInfo contentSampleItemproperty = contentSampleItemType.GetProperty(name);
                int contentSampleItemPropertyValue = (int)(contentSampleItemproperty.GetValue(contentSampleItem));
                finalMultiplier = 1f + multiplier * contentSampleItemPropertyValue / baseValue;
                item.GetType().GetProperty(name).SetValue(item, (int)(baseValue * finalMultiplier + 5E-06f));
                int tempForBreakpointCheck = (int)item.GetType().GetProperty(name).GetValue(item);
                BaseValueInt = contentSampleItemPropertyValue;
            }
        }*/
        /*public void RemoveFrom(ref Item item, string name, int baseValue, bool property = false)
        {
            float multiplier = (1f + Additive) * Multiplicative - 1f;
            float finalMultiplier = 1f / (1f + multiplier * BaseValueFloat / baseValue);
            if (!property)
            {
                item.GetType().GetField(name).SetValue(item, (int)(baseValue * finalMultiplier + 5E-06f));
                int tempForBreakpointCheck = (int)item.GetType().GetField(name).GetValue(item);
            }
            else
            {
                item.GetType().GetProperty(name).SetValue(item, (int)(baseValue * finalMultiplier + 5E-06f));
                int tempForBreakpointCheck = (int)item.GetType().GetProperty(name).GetValue(item);
            }
        }*/
    }
}
