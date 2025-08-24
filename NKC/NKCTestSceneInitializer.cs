using AssetBundles;
using NKC.Localization;
using NKC.UI;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public class NKCTestSceneInitializer : MonoBehaviour
{
	public enum eInitMode
	{
		NKCMainAll,
		BasicTempletOnly,
		None
	}

	public eInitMode m_bTempletInitMode = eInitMode.BasicTempletOnly;

	public bool m_bUseSountManager = true;

	[Header("이 옵션 켤때는 NKM_SCEN_UI 오브젝트 통채로 카피해와서 붙여둘 것")]
	public bool m_bUseUIManager;

	[Header("F5 to reload")]
	public bool m_bLoadAnimationTemplet = true;

	private void Start()
	{
		NKCAssetResourceManager.Init();
		NKCStringTable.LoadFromLUA(NKM_NATIONAL_CODE.NNC_KOREA);
		AssetBundleManager.ActiveVariants = NKCLocalization.GetVariants(NKM_NATIONAL_CODE.NNC_KOREA, NKC_VOICE_CODE.NVC_KOR);
		if (m_bTempletInitMode == eInitMode.NKCMainAll)
		{
			NKCMain.NKCInit();
		}
		else if (m_bTempletInitMode == eInitMode.BasicTempletOnly)
		{
			NKMContentsVersionManager.LoadDefaultVersion();
			if (NKCDefineManager.DEFINE_UNITY_EDITOR())
			{
				NKCContentsVersionManager.TryRecoverTag();
			}
			NKMCommonConst.LoadFromLUA("LUA_COMMON_CONST");
			NKCClientConst.LoadFromLUA("LUA_CLIENT_CONST");
			NKMTempletContainer<NKMIntervalTemplet>.Load("AB_SCRIPT", "LUA_INTERVAL_TEMPLET_V2", "INTERVAL_TEMPLET", NKMIntervalTemplet.LoadFromLUA, (NKMIntervalTemplet e) => e.StrKey);
			string[] fileNames = new string[4] { "LUA_UNIT_TEMPLET_BASE", "LUA_UNIT_TEMPLET_BASE2", "LUA_UNIT_TEMPLET_BASE_SD", "LUA_UNIT_TEMPLET_BASE_OPR" };
			NKMTempletContainer<NKMUnitTempletBase>.Load("AB_SCRIPT_UNIT_DATA", fileNames, "m_dicNKMUnitTempletBaseByStrID", NKMUnitTempletBase.LoadFromLUA, (NKMUnitTempletBase e) => e.m_UnitStrID);
			NKMSkinManager.LoadFromLua();
		}
		if (m_bUseSountManager)
		{
			if (GameObject.Find("NKM_SOUND") == null)
			{
				new GameObject("NKM_SOUND");
			}
			NKCSoundManager.Init();
		}
		if (m_bUseUIManager)
		{
			NKCUIManager.Init();
		}
		if (m_bLoadAnimationTemplet)
		{
			NKCAnimationEventManager.LoadFromLua();
		}
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.F5))
		{
			NKCAnimationEventManager.LoadFromLua();
		}
	}
}
