using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;

namespace KokoLib.Emitters;

public static class EmitterHelper
{
	internal static List<TypedModHandlerEmitter> TypedEmitters = new();

	public static object CallReadEmitter(BinaryReader reader, ushort index)
	{
		return TypedEmitters[index].InternalRead(reader);
	}
	
	public static void CallWriteEmitter(BinaryWriter writer, ushort index, object ins)
	{
		TypedEmitters[index].InternalWrite(writer, ins);
	}

	public static void SendAndReset(ModPacket packet)
	{
		packet.Send(Net.ToClient, Net.IgnoreClient);
		Net.ToClient = -1;
		Net.IgnoreClient = -1;
	}
}