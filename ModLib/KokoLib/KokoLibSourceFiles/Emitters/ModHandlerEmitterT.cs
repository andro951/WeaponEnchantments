using System;
using System.IO;
using System.Reflection.Emit;
namespace KokoLib.Emitters;

public abstract class ModHandlerEmitter<T> : TypedModHandlerEmitter
{
	public override Type Type => typeof(T);

	internal sealed override object InternalRead(BinaryReader reader)
	{
		return Read(reader);
	}

	internal sealed override void InternalWrite(BinaryWriter writer, object ins)
	{
		Write(writer, (T)ins);
	}

	public abstract T Read(BinaryReader reader);
	
	public abstract void Write(BinaryWriter writer, T ins);
}

public abstract class TypedModHandlerEmitter : ModHandlerEmitter
{
	public ushort TypedIndex;

	public override void Load()
	{
		TypedIndex = (ushort)EmitterHelper.TypedEmitters.Count;
		EmitterHelper.TypedEmitters.Add(this);
	}

	public sealed override void EmitRead(ILGenerator il)
	{
		var read = typeof(EmitterHelper).GetMethod(nameof(EmitterHelper.CallReadEmitter));

		// load the reader to stack
		il.Emit(OpCodes.Ldarg_0);

		// load the index of this instance
		il.Emit(OpCodes.Ldc_I4_S, TypedIndex);

		// call the reader
		il.Emit(OpCodes.Call, read!);

		// now we have a Object in the stack, convert to our type
		il.Emit(OpCodes.Unbox_Any, Type);
	}

	public sealed override void EmitWrite(ILGenerator il, int index)
	{
		var write = typeof(EmitterHelper).GetMethod(nameof(EmitterHelper.CallWriteEmitter));

		// load the writer
		il.Emit(OpCodes.Ldloc_0);

		// load the index of this instance
		il.Emit(OpCodes.Ldc_I4_S, TypedIndex);

		// load the instance to write
		il.Emit(OpCodes.Ldarg_S, index);

		// if we want to pass a value type we need to box into a object
		if (Type.IsValueType) il.Emit(OpCodes.Box, Type);

		//call the writer
		il.Emit(OpCodes.Call, write!);
	}

	internal abstract object InternalRead(BinaryReader reader);
	
	internal abstract void InternalWrite(BinaryWriter writer, object ins);
}