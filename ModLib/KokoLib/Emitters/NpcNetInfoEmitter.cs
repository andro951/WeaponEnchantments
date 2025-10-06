using System.IO;
using Terraria;
using Terraria.ID;
using WeaponEnchantments.ModLib.KokoLib;

namespace KokoLib.Emitters;

class NpcNetInfoEmitter : ModHandlerEmitter<NPCNetInfoCursedNPC>
{
	public override NPCNetInfoCursedNPC Read(BinaryReader reader) => new(reader);

	public override void Write(BinaryWriter writer, NPCNetInfoCursedNPC ins) => ins.Write(writer);
}