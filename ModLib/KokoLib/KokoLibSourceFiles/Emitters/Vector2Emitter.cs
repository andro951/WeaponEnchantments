using System.IO;
using Microsoft.Xna.Framework;
namespace KokoLib.Emitters;

class Vector2Emitter : ModHandlerEmitter<Vector2>
{
	public override Vector2 Read(BinaryReader reader)
	{
		return new(reader.ReadSingle(), reader.ReadSingle());
	}

	public override void Write(BinaryWriter writer, Vector2 ins)
	{
		writer.Write(ins.X);
		writer.Write(ins.Y);
	}
}
class NumVector2Emitter : ModHandlerEmitter<System.Numerics.Vector2>
{
	public override System.Numerics.Vector2 Read(BinaryReader reader)
	{
		return new(reader.ReadSingle(), reader.ReadSingle());
	}

	public override void Write(BinaryWriter writer, System.Numerics.Vector2 ins)
	{
		writer.Write(ins.X);
		writer.Write(ins.Y);
	}
}