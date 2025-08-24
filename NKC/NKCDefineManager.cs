namespace NKC;

public class NKCDefineManager
{
	public static bool DEFINE_SERVICE()
	{
		return true;
	}

	public static bool DEFINE_UNITY_STANDALONE()
	{
		return true;
	}

	public static bool DEFINE_UNITY_STANDALONE_WIN()
	{
		return true;
	}

	public static bool DEFINE_FULL_BUILD()
	{
		return true;
	}

	public static bool DEFINE_SEMI_FULL_BUILD()
	{
		return true;
	}

	public static bool DEFINE_ANDROID()
	{
		return false;
	}

	public static bool DEFINE_IOS()
	{
		return false;
	}

	public static bool DEFINE_UNITY_DEBUG_LOG()
	{
		return true;
	}

	public static bool DEFINE_UNITY_EDITOR()
	{
		return false;
	}

	public static bool DEFINE_USE_CHEAT()
	{
		return false;
	}

	public static bool DEFINE_USE_TOUCH_DELAY()
	{
		return false;
	}

	public static bool DEFINE_NO_CONSOLE_LOG()
	{
		return false;
	}

	public static bool DEFINE_ZLONG()
	{
		return false;
	}

	public static bool DEFINE_CHECKVERSION()
	{
		return false;
	}

	public static bool DEFINE_ZLONG_SEA()
	{
		return false;
	}

	public static bool DEFINE_ZLONG_CHN()
	{
		return false;
	}

	public static bool DEFINE_OBB()
	{
		return false;
	}

	public static bool DEFINE_CAN_ONLY_LOAD_MIN_TEMPLET()
	{
		if (!DEFINE_SERVICE() || DEFINE_ZLONG() || DEFINE_SB_GB())
		{
			return true;
		}
		return false;
	}

	public static bool DEFINE_NX_PC()
	{
		return false;
	}

	public static bool DEFINE_NX_PC_TEST()
	{
		return false;
	}

	public static bool DEFINE_NX_PC_STAGE()
	{
		return false;
	}

	public static bool DEFINE_NX_PC_LIVE()
	{
		return false;
	}

	public static bool DEFINE_WEBVIEW_TEST()
	{
		return false;
	}

	public static bool DEFINE_DOWNLOAD_CONFIG()
	{
		return false;
	}

	public static bool DEFINE_USE_CUSTOM_SERVERS()
	{
		return false;
	}

	public static bool DEFINE_PC_EXTRA_DOWNLOAD_IN_EXE_FOLDER()
	{
		return true;
	}

	public static bool DEFINE_EXTRA_ASSET()
	{
		return true;
	}

	public static bool DEFINE_PC_FORCE_VERSION_UP()
	{
		return false;
	}

	public static bool DEFINE_SB_GB()
	{
		return false;
	}

	public static bool DEFINE_LB()
	{
		return false;
	}

	public static bool DEFINE_NXTOY()
	{
		return false;
	}

	public static bool DEFINE_NXTOY_JP()
	{
		return false;
	}

	public static bool DEFINE_ALLOW_MULTIPC()
	{
		return false;
	}

	public static bool DEFINE_PATCH_SKIP()
	{
		return false;
	}

	public static bool DEFINE_FBANALYTICS()
	{
		return false;
	}

	public static bool DEFINE_USE_COMPILED_LUA()
	{
		return true;
	}

	public static bool DEFINE_USE_CONVERTED_FILENAME()
	{
		return true;
	}

	public static bool DEINFE_USE_CONVERTED_FILENAME_TO_UPPERCASE()
	{
		return false;
	}

	public static bool DEFINE_STEAM()
	{
		return true;
	}

	public static bool DEFINE_JPPC()
	{
		return false;
	}

	public static bool DEFINE_ENCRYPTION_TEST()
	{
		return false;
	}

	public static bool DEFINE_SAVE_LOG()
	{
		return true;
	}

	public static bool DEFINE_PURE_LOG()
	{
		return false;
	}

	public static bool DEFINE_USE_DEV_SCRIPT()
	{
		return false;
	}

	public static bool DEFINE_ONESTORE()
	{
		return false;
	}

	public static bool DEFINE_CLIENT_KOR()
	{
		return false;
	}

	public static bool DEFINE_CLIENT_GBL()
	{
		return false;
	}

	public static bool DEFINE_CLIENT_CHN()
	{
		return false;
	}

	public static bool DEFINE_CLIENT_TWN()
	{
		return false;
	}

	public static bool DEFINE_CLIENT_SEA()
	{
		return false;
	}

	public static bool DEFINE_CLIENT_JPN()
	{
		return false;
	}

	public static bool DEFINE_SELECT_SERVER()
	{
		return true;
	}

	public static bool DEFINE_PATCH_OPTIMIZATION()
	{
		return true;
	}

	public static bool DEFINE_GLOBALQA()
	{
		return false;
	}

	public static bool USE_PATCHERCHECKER()
	{
		return true;
	}

	public static bool USE_ANDROIDSERVICE()
	{
		DEFINE_ANDROID();
		return false;
	}
}
