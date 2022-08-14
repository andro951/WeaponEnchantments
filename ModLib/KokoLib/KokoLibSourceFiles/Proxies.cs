using KokoLib.Nets;

namespace KokoLib;

public static partial class Net
{
	public static IText Text => Net<IText>.proxy;


}
