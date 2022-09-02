using System.IO;
using Terraria;
using Terraria.ID;

namespace KokoLib.Emitters;

class NpcEmitter : ModHandlerEmitter<NPC>
{
	public override NPC Read(BinaryReader reader)
	{
		var npc = Main.npc[reader.ReadInt32()];

		if(Main.netMode == NetmodeID.Server)
		{
			npc.netUpdate = true;
		}
		return npc;
	}

	public override void Write(BinaryWriter writer, NPC ins)
	{
		writer.Write(ins.whoAmI);
	}
}