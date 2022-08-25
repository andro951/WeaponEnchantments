using System.IO;
using Terraria;

namespace KokoLib.Emitters;

class PlayerEmitter : ModHandlerEmitter<Player>
{
	public override Player Read(BinaryReader reader) => Main.player[reader.ReadInt32()];

	public override void Write(BinaryWriter writer, Player ins) => writer.Write(ins.whoAmI);
}