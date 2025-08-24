namespace NKM;

public static class AuthLevelExt
{
	public static bool IsNormalUser(this NKM_USER_AUTH_LEVEL value)
	{
		return value == NKM_USER_AUTH_LEVEL.NORMAL_USER;
	}
}
