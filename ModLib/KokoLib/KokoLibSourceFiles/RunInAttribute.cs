using System;

namespace KokoLib;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RunInAttribute : Attribute
{
	public RunInAttribute(HandlerMode client)
	{
		Client = client;
	}

	public HandlerMode Client { get; }
}