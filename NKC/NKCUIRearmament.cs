using ClientPacket.Unit;
using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKCUIRearmament : NKCUIBase
{
	public enum REARM_TYPE
	{
		RT_NONE,
		RT_LIST,
		RT_PROCESS,
		RT_EXTRACT
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_rearm";

	private const string UI_ASSET_NAME = "AB_UI_REARM_UI";

	private static NKCUIRearmament m_Instance;

	public GameObject m_objRearm;

	public GameObject m_objExtract;

	public GameObject m_objShortCut;

	[Header("숏컷")]
	public NKCUIComToggle m_ctgRearm;

	public NKCUIComToggle m_ctgExtract;

	public NKCUIRearmamentExtract m_RearmExtract;

	public NKCUIRearmamentProcess m_RearmProcess;

	[Header("Guide Templet ID")]
	public string REARM_EXTRACT_ID = "ARTICLE_EXTRACT_INFO";

	public string REARM_LIST_ID = "ARTICLE_REARM_INFO";

	public string REARM_PROCESS_ID = "ARTICLE_REARM_INFO";

	private REARM_TYPE m_preUIState;

	private REARM_TYPE m_curUIType;

	public Animator m_AniRearmProcess;

	private const string ANI_LIST_INTRO = "LIST_INTRO";

	private const string ANI_LIST_TO_PROCESS = "LIST_TO_PROCESS";

	private const string ANI_PROCESS_TO_LIST = "PROCESS_TO_LIST";

	private const string ANI_DIRECT_IN_PROCESS = "DIRECT_IN_PROCESS";

	private const string ANI_DIRECT_IN_LIST = "DIRECT_IN_LIST";

	public static NKCUIRearmament Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIRearmament>("ab_ui_rearm", "AB_UI_REARM_UI", NKCUIManager.eUIBaseRect.UIFrontCommon, OnCleanupInstance).GetInstance<NKCUIRearmament>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => GetCurrentMenuName();

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public static bool IsInstanceLoaded => m_Instance != null;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override string GuideTempletID => GetCurrentGuideName();

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public override void CloseInternal()
	{
		m_preUIState = REARM_TYPE.RT_NONE;
		m_RearmExtract.Clear();
		m_RearmProcess.Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public static void OnCleanupInstance()
	{
		m_Instance = null;
	}

	public override void UnHide()
	{
		base.UnHide();
		UpdateAni(m_curUIType);
	}

	public override void OnBackButton()
	{
		if (m_curUIType == REARM_TYPE.RT_PROCESS)
		{
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_UNIT_LIST)
			{
				Close();
			}
			else
			{
				ChangeState(REARM_TYPE.RT_LIST);
			}
		}
		else
		{
			base.OnBackButton();
		}
	}

	private string GetCurrentMenuName()
	{
		if (m_curUIType == REARM_TYPE.RT_EXTRACT)
		{
			return NKCUtilString.GET_STRING_REARM_EXTRACT_TITLE;
		}
		return NKCUtilString.GET_STRING_REARM_PROCESS_TITLE;
	}

	private string GetCurrentGuideName()
	{
		return m_curUIType switch
		{
			REARM_TYPE.RT_LIST => REARM_LIST_ID, 
			REARM_TYPE.RT_PROCESS => REARM_PROCESS_ID, 
			_ => REARM_EXTRACT_ID, 
		};
	}

	private void InitUI()
	{
		NKCUtil.SetToggleValueChangedDelegate(m_ctgRearm, OnClickToggleRearm);
		NKCUtil.SetToggleValueChangedDelegate(m_ctgExtract, OnClickToggleExtract);
		m_RearmExtract?.Init();
		m_RearmProcess?.Init(ChangeState);
	}

	private void OnClickToggleRearm(bool bVal)
	{
		if (bVal)
		{
			if (m_preUIState != REARM_TYPE.RT_NONE)
			{
				ChangeState(m_preUIState);
			}
			else
			{
				ChangeState(REARM_TYPE.RT_LIST);
			}
		}
	}

	private void OnClickToggleExtract(bool bVal)
	{
		if (bVal)
		{
			m_preUIState = m_curUIType;
			ChangeState(REARM_TYPE.RT_EXTRACT);
		}
	}

	public void SetReserveRearmData(int iTargetRearmTypeUnitID, long iResourceRearmUnitUID)
	{
		m_RearmProcess.SetReserveRearmData(iTargetRearmTypeUnitID, iResourceRearmUnitUID);
	}

	public void Open(REARM_TYPE type = REARM_TYPE.RT_LIST)
	{
		m_curUIType = REARM_TYPE.RT_NONE;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		ChangeState(type);
		if (type == REARM_TYPE.RT_EXTRACT)
		{
			m_ctgExtract?.Select(bSelect: true, bForce: true);
		}
		else
		{
			m_ctgRearm?.Select(bSelect: true, bForce: true);
		}
		m_ctgRearm.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.REARM));
		m_ctgExtract.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.EXTRACT));
		NKCUtil.SetGameobjectActive(m_ctgExtract, NKCRearmamentUtil.CanUseExtract());
		UIOpened();
	}

	public void ChangeState(REARM_TYPE newType)
	{
		if (m_curUIType != newType)
		{
			NKCUtil.SetGameobjectActive(m_objRearm, newType != REARM_TYPE.RT_EXTRACT);
			NKCUtil.SetGameobjectActive(m_objExtract, newType == REARM_TYPE.RT_EXTRACT);
			if (newType == REARM_TYPE.RT_EXTRACT)
			{
				m_RearmExtract.Open();
			}
			else
			{
				m_RearmProcess.Open(newType);
			}
			UpdateAni(newType);
			m_curUIType = newType;
			UpdateUpsideMenu();
			CheckTutorial();
		}
	}

	private void UpdateAni(REARM_TYPE newType)
	{
		string text = "";
		if (newType == REARM_TYPE.RT_LIST && m_curUIType == REARM_TYPE.RT_PROCESS)
		{
			text = "PROCESS_TO_LIST";
		}
		else if (m_curUIType == REARM_TYPE.RT_LIST && newType == REARM_TYPE.RT_PROCESS)
		{
			text = "LIST_TO_PROCESS";
		}
		else if (m_curUIType == REARM_TYPE.RT_NONE && newType == REARM_TYPE.RT_LIST)
		{
			text = "LIST_INTRO";
		}
		else if ((m_curUIType == REARM_TYPE.RT_NONE && newType == REARM_TYPE.RT_PROCESS) || (m_curUIType == REARM_TYPE.RT_EXTRACT && newType == REARM_TYPE.RT_PROCESS) || (m_curUIType == REARM_TYPE.RT_PROCESS && newType == REARM_TYPE.RT_PROCESS))
		{
			text = "DIRECT_IN_PROCESS";
		}
		else if (m_curUIType == REARM_TYPE.RT_EXTRACT && newType == REARM_TYPE.RT_LIST)
		{
			text = "LIST_INTRO";
		}
		else if (m_curUIType == REARM_TYPE.RT_LIST && newType == REARM_TYPE.RT_LIST)
		{
			text = "DIRECT_IN_LIST";
		}
		else
		{
			if (newType == REARM_TYPE.RT_EXTRACT)
			{
				return;
			}
			Debug.LogError($"<color=red>모르는 애니메이션 !!! : m_curUIType : {m_curUIType}, newType : {newType}  </color>");
		}
		if (!string.IsNullOrEmpty(text))
		{
			m_AniRearmProcess.SetTrigger(text);
		}
	}

	public void OnRecv(NKMPacket_EXTRACT_UNIT_ACK sPacket)
	{
		if (m_curUIType == REARM_TYPE.RT_EXTRACT)
		{
			m_RearmExtract.OnRecv(sPacket);
		}
	}

	public void CheckTutorial()
	{
		if (m_curUIType == REARM_TYPE.RT_EXTRACT)
		{
			NKCTutorialManager.TutorialRequired(TutorialPoint.Extract);
		}
		else
		{
			NKCTutorialManager.TutorialRequired(TutorialPoint.Rearm);
		}
	}

	public RectTransform GetRearmSlotRectTransform(int rearmUnitID)
	{
		if (m_curUIType == REARM_TYPE.RT_LIST)
		{
			return m_RearmProcess.GetRearmSlotRectTransform(rearmUnitID);
		}
		Debug.LogError("재무장 리스트 슬롯 데이터 확인 불가");
		return null;
	}

	public RectTransform GetExtractSlotRectTransform(int extractSlotIdx)
	{
		if (m_curUIType == REARM_TYPE.RT_EXTRACT)
		{
			return m_RearmExtract.GetExtractSlotRectTransform(extractSlotIdx);
		}
		Debug.LogError("재무장 추출 슬롯 데이터 확인 불가");
		return null;
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		m_RearmProcess.OnInventoryChange();
	}
}
