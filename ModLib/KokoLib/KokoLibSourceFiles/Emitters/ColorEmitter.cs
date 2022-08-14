using Microsoft.Xna.Framework;
using System.IO;

namespace KokoLib.Emitters;

public class ColorEmitter : ModHandlerEmitter<Color>
{
	public override Color Read(BinaryReader reader)
	{
		var packet = reader.ReadUInt32();
		var r = (byte)packet;
		var g = (byte)(packet >> 8);
		var b = (byte)(packet >> 16);
		var a = (byte)(packet >> 24);
		return new Color(r, g, b, a);
	}

	public override void Write(BinaryWriter writer, Color ins) => writer.Write(ins.PackedValue);
}