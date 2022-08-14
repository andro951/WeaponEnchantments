using System;

namespace KokoLib;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class BroadcastAttribute : Attribute
{

}