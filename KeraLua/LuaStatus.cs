namespace KeraLua;

public enum LuaStatus
{
	OK,
	Yield,
	ErrRun,
	ErrSyntax,
	ErrMem,
	ErrErr
}
