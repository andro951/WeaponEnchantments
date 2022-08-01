using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponEnchantments.Common
{
    public class EssenceColorAttribute : Attribute
    {
        public Color trueColor;
        public Color altColor;
        public Color color { get => WEMod.clientConfig.UseAlternateEnchantmentEssenceTextures ? altColor : trueColor; }

        public EssenceColorAttribute(int r, int g, int b, int rAlt, int gAlt, int bAlt) {
            this.trueColor = new Color(r, g, b);
            this.altColor = new Color(rAlt, gAlt, bAlt);
        }
    }
}
