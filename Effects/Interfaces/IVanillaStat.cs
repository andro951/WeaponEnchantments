using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.Effects {
    //Maybe replace these with statmodifier combinewith type methods instead?
    public interface IVanillaStat : IApplyStats {
        public virtual bool SetVanillaStatEqualToValue => false;
    }
    public interface INonVanillaStat : IApplyStats {

	}
    public interface IApplyStats {
        public void ApplyTo(ref float stat) {

        }
        public void Reset(ref float stat) {

        }
        public void ApplyTo(ref int stat) {

        }
        public void Reset(ref int stat) {

        }
        public void ApplyTo(ref bool stat) {

        }
        public void Reset(ref bool stat) {

        }
    }
}
