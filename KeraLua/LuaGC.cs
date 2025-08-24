using System;

namespace KeraLua;

public enum LuaGC
{
	Stop = 0,
	Restart = 1,
	Collect = 2,
	Count = 3,
	Countb = 4,
	Step = 5,
	[Obsolete("Deprecatad since Lua 5.4, Use Incremental instead")]
	SetPause = 6,
	[Obsolete("Deprecatad since Lua 5.4, Use Incremental instead")]
	SetStepMultiplier = 7,
	IsRunning = 9,
	Generational = 10,
	Incremental = 11
}
