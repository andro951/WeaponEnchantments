using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments;
using WeaponEnchantments.Common;

namespace WeaponEnchantments.Debuffs
{
    public class AmaterasuDebuff : ModBuff
    {
        private int damage = 60;
        int[] notImmuneBuffs = new int[] {BuffID.OnFire, BuffID.CursedInferno, BuffID.ShadowFlame, BuffID.OnFire3, BuffID.Oiled};
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            foreach (int notImmuneBuff in notImmuneBuffs)
                npc.buffImmune[notImmuneBuff] = false;
            int buffTime = npc.buffTime[buffIndex];
            if (buffTime > -1)
            {
                npc.G().amaterasuDamage += buffTime;
                npc.buffTime[buffIndex] = -1;
            }
            //npc.G().amaterasuDamage += damage;
            //Make not immune to the other buffs that WorldAblaze applies
        }
        /*public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            //npc.buffTime[buffIndex] = -1;
            npc.G().amaterasuDamage += time;
            return true;
        }*/
    }
}
