namespace NKM;

public static class NKMDataVersion
{
	public static int DataVersion;

	public const string FILENAME = "LUA_DATA_VERSION";

	public static bool LoadFromLUA()
	{
		using (NKMLua nKMLua = new NKMLua())
		{
			if (nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_DATA_VERSION") && nKMLua.OpenTable("m_DataVersion"))
			{
				nKMLua.GetData("DataVersion", ref DataVersion);
				nKMLua.CloseTable();
			}
		}
		return true;
	}
}
