using System;

namespace KokoLib;

[Flags]
public enum HandlerMode
{
	Singleplayer = 1,
	ServerOnly = 2,
	ClientOnly = 4,
	Server = Singleplayer | ServerOnly,
	Client = Singleplayer | ClientOnly,
	Both = Server | Client
}