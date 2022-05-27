using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace WeaponEnchantments.Common
{
    public struct StaticStatStruct
    {
        public string Name { get; private set; }
        public float Additive { get; private set; }
        public float Multiplicative { get; private set; }
        public float BaseValueFloat { get; private set; }
        public int BaseValueInt { get; private set; }
        public StaticStatStruct(string name, float additive = 1f, float multiplicative = 1f)
        {
            Name = name;
            Additive = additive;
            Multiplicative = multiplicative;
            BaseValueFloat = 0f;
            BaseValueInt = 0;
        }
        public void ApplyTo(ref float baseValue, int itemType) 
        {
            Item item = ContentSamples.ItemsByType[itemType].Clone();
            Type contentSampleItemType = item.GetType();
            string baseValueName = baseValue.ToString();
            PropertyInfo contentSampleItemProperty = contentSampleItemType.GetProperty(baseValueName);
            float contentSampleItemPropertyValue = (float)(contentSampleItemProperty.GetValue(item, null));//(baseValue + Base) * Additive * Multiplicative + Flat;
            float multiplier = Additive * Multiplicative - 1f;
            float finalMultiplier = 1f + multiplier * contentSampleItemPropertyValue / baseValue;
            baseValue *= finalMultiplier;
        }
        public void ApplyTo(ref int baseValue, int itemType) 
        {
            Item item = ContentSamples.ItemsByType[itemType].Clone();
            Type contentSampleItemType = item.GetType();
            string baseValueName = baseValue.ToString();
            PropertyInfo contentSampleItemProperty = contentSampleItemType.GetProperty(baseValueName);
            float contentSampleItemPropertyValue = (float)(contentSampleItemProperty.GetValue(item, null));//(baseValue + Base) * Additive * Multiplicative + Flat;
            float multiplier = Additive * Multiplicative - 1f;
            float finalMultiplier = 1f + multiplier * contentSampleItemPropertyValue / baseValue;
            baseValue = (int)(baseValue * finalMultiplier + 5E-06f);
        }
    }
}
