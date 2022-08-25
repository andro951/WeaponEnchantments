using System;
using System.Reflection.Emit;

namespace KokoLib.Emitters;

internal abstract  class BaseEmitter : ModHandlerEmitter
{
	public override void EmitWrite(ILGenerator il, int index)
	{
		il.Emit(OpCodes.Ldloc_0);
		il.Emit(OpCodes.Ldarg_S, index);
		var m = ModPacket.GetMethod("Write", new[] { Type });
		il.Emit(OpCodes.Callvirt, m!);
	}
}

internal class IntEmitter : BaseEmitter
{
	public override Type Type => typeof(int);

	public override void EmitRead(ILGenerator il)
	{
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, BinaryReader.GetMethod("ReadInt32")!);
	}
}

internal class StringEmitter : BaseEmitter
{
	public override Type Type => typeof(string);

	public override void EmitRead(ILGenerator il)
	{
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, BinaryReader.GetMethod("ReadString")!);
	}
}


internal class BoolEmitter : BaseEmitter
{
	public override Type Type => typeof(bool);

	public override void EmitRead(ILGenerator il)
	{
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, BinaryReader.GetMethod("ReadBoolean")!);
	}
}

internal class ByteEmitter : BaseEmitter
{
	public override Type Type => typeof(byte);

	public override void EmitRead(ILGenerator il)
	{
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, BinaryReader.GetMethod("ReadByte")!);
	}
}

internal class SbyteEmitter : BaseEmitter
{
	public override Type Type => typeof(sbyte);

	public override void EmitRead(ILGenerator il)
	{
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, BinaryReader.GetMethod("ReadSByte")!);
	}
}


internal class ShortEmitter : BaseEmitter
{
	public override Type Type => typeof(short);

	public override void EmitRead(ILGenerator il)
	{
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, BinaryReader.GetMethod("ReadInt16")!);
	}
}

internal class FloatEmitter : BaseEmitter
{
	public override Type Type => typeof(float);

	public override void EmitRead(ILGenerator il)
	{
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, BinaryReader.GetMethod("ReadSingle")!);
	}
}

internal class DoubleEmitter : BaseEmitter
{
	public override Type Type => typeof(double);

	public override void EmitRead(ILGenerator il)
	{
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Call, BinaryReader.GetMethod("ReadDouble")!);
	}
}
