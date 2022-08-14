using System;
using System.IO;
using System.Reflection.Emit;
using Terraria.ModLoader;

namespace KokoLib.Emitters;

public abstract class ModHandlerEmitter : ModType, IIndexed
{
	public static readonly Type BinaryReader = typeof(BinaryReader);

	public static readonly Type ModPacket = typeof(ModPacket);

	/// <summary>
	/// The type this emitter adds support for, you should not define more than one emitter with the same type.
	/// </summary>
	public abstract Type Type { get; }

	protected sealed override void Register()
	{
		TypeEmitter.RegisterForMod(this);
	} 

	public abstract void EmitRead(ILGenerator il);

	public abstract void EmitWrite(ILGenerator il, int index);

	public ushort Index { get; internal set; }
}
