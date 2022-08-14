using System;
using System.IO;
using System.Linq;
using Terraria.ModLoader;

namespace KokoLib;

public abstract class ModHandler<T> : ModHandler where T : class
{
	public Action<BinaryReader, T>[] Methods;

	protected sealed override void Register()
	{
		ModTypeLookup<ModHandler<T>>.Register(this);
		Net.Register(this, Handler);
	}

	public abstract T Handler { get; }

	public sealed override void SetupContent()
	{
		SetStaticDefaults();
	}

	internal sealed override void CreateProxy()
	{
		Net<T>.CreateProxy(Net.ModuleBuilder , Type);
	}
	
	internal sealed override void CreateMethods()
	{
		var t = Net.GetAllInterfaceMethods(typeof(T)).ToList();
		t.Sort((m, o) => string.Compare(m.Name, o.Name, StringComparison.Ordinal));
		Methods = t.Select(Net<T>.WrapMethod).ToArray();
		t.Clear();
	}

	public override void Handle(BinaryReader reader, byte method)
	{
		Methods[method](reader, Handler);
	}
}

public abstract class ModHandler : ModType
{
	public byte Type;
	public int WhoAmI;

	internal abstract void CreateProxy();
	internal abstract void CreateMethods();

	public abstract void Handle(BinaryReader reader, byte method);
}