using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Terraria.ModLoader;
using WeaponEnchantments;

namespace KokoLib;

public static partial class Net
{
	internal const string ProxyModuleName = "KokoLib.Proxys";
	internal static readonly ModuleBuilder ModuleBuilder;

	/// <summary>
	/// This is the mod we use to generate and receive packages, by default it is Kokolib but it could be changed
	/// </summary>
	public static Mod HandlerMod;

	// Capacity
	public static int PacketCapacity = 256;
	public static int DefaultPacketCapacity = 256;

	// Target
	public static int ToClient = -1;
	public static int IgnoreClient = -1;

	static Net()
	{
		// create a dynamic assembly to host all the proxies
		var assemblyName = new AssemblyName(ProxyModuleName);
		var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
		ModuleBuilder = assemblyBuilder.DefineDynamicModule(ProxyModuleName);
	}

	/// <summary>
	/// 
	/// </summary>
	internal static void Register<T>(ModHandler modHandler, T handler) where T : class
	{
		if (modHandler.Mod.Side != ModSide.Both)
		{
			throw new("Handlers can only be used in mods with \"Both\" side");
		}
		ValidateInterface<T>();
		ValidateMethodsNames<T>();
		ValidateMethodsParameters<T>();

		WEMod.Handlers.Add(modHandler);
		HandlerMod ??= ModLoader.GetMod("WeaponEnchantments");
		Net<T>.mod = modHandler.Mod;
		Net<T>.handler = handler;
	}
	
	private static void ValidateMethodsNames<T>() where T : class
	{
		var methods = GetAllInterfaceMethods(typeof(T)).GroupBy(m => m.Name);
		if (methods.Any(g => g.Count() > 1))
			throw new("ModHandler methods cant have overloads, use diferent method names");
	}

	static void ValidateMethodsParameters<T>() where T : class
	{
		var methods = GetAllInterfaceMethods(typeof(T));
		foreach (var method in methods)
		{
			if(method.ReturnType != typeof(void))
			{
				throw new("ModHandler methods cant return");
			}
			if(method.IsGenericMethod || method.IsGenericMethodDefinition)
			{
				throw new("ModHandler methods cant be generic");
			}
			foreach (var param in method.GetParameters())
			{
				if (!TypeEmitter.IsSupported(param.ParameterType))
				{
					throw new($"Parameter {param.Name} has an unsupported type");
				}
			}
		}
	}

	internal static IEnumerable<MethodInfo> GetAllInterfaceMethods(Type interfaceType)
	{
		foreach (var parent in interfaceType.GetInterfaces())
		{
			foreach (var parentMethod in GetAllInterfaceMethods(parent))
			{
				yield return parentMethod;
			}
		}

		foreach (var method in interfaceType.GetMethods())
		{
			yield return method;
		}
	}

	static void ValidateInterface<T>() where T : class
	{
		if(!typeof(T).IsInterface)
			throw new ArgumentException("Generic type in ModHandler<T> must be an interface");
		// interface must be public to implement
		if(!typeof(T).IsPublic || typeof(T).IsNotPublic)
			throw new ArgumentException("Generic type in ModHandler<T> must be public");
	}
}