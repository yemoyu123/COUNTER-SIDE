using System;

namespace NKC;

[Serializable]
public struct ConsoleSubMenu
{
	public DEV_CONSOLE_SUB_MENU type;

	public string strKey;

	public SUB_MENU_TYPE stype;

	public bool bWarning;

	public ConsoleSubMenu(DEV_CONSOLE_SUB_MENU _type, string _strKey, SUB_MENU_TYPE _stype, bool _warning = false)
	{
		type = _type;
		strKey = _strKey;
		stype = _stype;
		bWarning = _warning;
	}
}
