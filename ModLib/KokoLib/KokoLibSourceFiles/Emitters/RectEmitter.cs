using Microsoft.Xna.Framework;
using System.IO;

namespace KokoLib.Emitters;

class RectEmitter : ModHandlerEmitter<Rectangle>
{
	public override Rectangle Read(BinaryReader reader) => 
		new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

	public override void Write(BinaryWriter writer, Rectangle ins)
	{
		writer.Write(ins.X);
		writer.Write(ins.Y);
		writer.Write(ins.Width);
		writer.Write(ins.Height);
	}
}
