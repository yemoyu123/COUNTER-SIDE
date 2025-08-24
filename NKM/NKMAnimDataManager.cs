namespace NKM;

public class NKMAnimDataManager
{
	public static NKMObjectAnimTimeBundleData m_NKMObjectAnimTimeBundleData = new NKMObjectAnimTimeBundleData();

	public static NKMObjectAnimTimeBundleData GetBundleData => m_NKMObjectAnimTimeBundleData;

	public static bool LoadFromLUA(string fileName)
	{
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT_ANIM_DATA", fileName) && nKMLua.OpenTable("m_dicUnitAnim"))
		{
			m_NKMObjectAnimTimeBundleData.LoadFromLUA(nKMLua);
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		return true;
	}

	public static float GetAnimTimeMax(string bundleName, string objectName, string animName)
	{
		return m_NKMObjectAnimTimeBundleData.GetAnimTimeMax(bundleName, objectName, animName);
	}

	public static void SetAnimTimeMax(string bundleName, string objectName, string animName, float animTimeMax)
	{
		m_NKMObjectAnimTimeBundleData.SetAnimTimeMax(bundleName, objectName, animName, animTimeMax);
	}

	public static bool LoadFromLUABasePath(string fileName)
	{
		NKMLua nKMLua = new NKMLua();
		string errorMessage = "";
		if (nKMLua.LoadCommonPathBase("AB_SCRIPT_ANIM_DATA", fileName, bAddCompiledLuaPostFix: true, bUseDevScript: false, ref errorMessage) && nKMLua.OpenTable("m_dicUnitAnim"))
		{
			m_NKMObjectAnimTimeBundleData.LoadFromLUA(nKMLua);
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		return true;
	}
}
