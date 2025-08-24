namespace NKM;

public static class NKM_USER_COMMON
{
	public static int USER_NICK_MIN_LENGTH = 4;

	public static int USER_NICK_MAX_LENGTH = 16;

	public static bool CheckNickName(string nickName)
	{
		if (string.IsNullOrEmpty(nickName))
		{
			return false;
		}
		int nickNameLength = GetNickNameLength(nickName);
		if (nickNameLength < USER_NICK_MIN_LENGTH || nickNameLength > USER_NICK_MAX_LENGTH)
		{
			return false;
		}
		return true;
	}

	public static bool CheckNickNameForServer(string nickName)
	{
		if (string.IsNullOrEmpty(nickName))
		{
			return false;
		}
		if (GetNickNameLength(nickName) < USER_NICK_MIN_LENGTH)
		{
			return false;
		}
		return true;
	}

	public static int GetNickNameLength(string str)
	{
		int num = 0;
		for (int i = 0; i < str.Length; i++)
		{
			num = ((str[i] > 'ÿ') ? (num + 2) : (num + 1));
		}
		return num;
	}
}
