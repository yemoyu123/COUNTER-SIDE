using System.Collections.Generic;
using NKM;
using NKM.Event;

namespace NKC.UI.Event;

public class NKCPopupEventMission : NKCUIBase
{
	public delegate bool CheckTime(bool bPopup);

	public delegate void OnComplete(int eventID, int tileIndex);

	public const string ASSET_BUNDLE_NAME = "ui_single_bingo";

	public const string UI_ASSET_NAME = "UI_POPUP_SINGLE_BINGO_MISSION";

	private static NKCPopupEventMission m_Instance;

	public NKCUIComButton m_btnBack;

	public NKCUIComStateButton m_btnClose;

	public List<NKCPopupEventMissionSlot> m_listSlot;

	private NKMEventBingoTemplet m_bingoTemplet;

	private CheckTime dCheckTime;

	private OnComplete dOnComplete;

	public static NKCPopupEventMission Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventMission>("ui_single_bingo", "UI_POPUP_SINGLE_BINGO_MISSION", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventMission>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "Spacial Mission";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void PreloadInstance()
	{
		if (m_Instance == null)
		{
			NKCUtil.SetGameobjectActive(Instance, bValue: false);
		}
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		if (m_btnBack != null)
		{
			m_btnBack.PointerClick.RemoveAllListeners();
			m_btnBack.PointerClick.AddListener(base.Close);
		}
		if (m_btnClose != null)
		{
			m_btnClose.PointerClick.RemoveAllListeners();
			m_btnClose.PointerClick.AddListener(base.Close);
		}
		for (int i = 0; i < m_listSlot.Count; i++)
		{
			m_listSlot[i].Init(OnTouchProgress, OnTouchComplete);
		}
	}

	public void Open(NKMEventBingoTemplet bingoTemplet, CheckTime checkEventTime, OnComplete onComplete)
	{
		m_bingoTemplet = bingoTemplet;
		dCheckTime = checkEventTime;
		dOnComplete = onComplete;
		SetData(bingoTemplet);
		UIOpened();
	}

	public void Refresh()
	{
		SetData(m_bingoTemplet);
	}

	private void SetData(NKMEventBingoTemplet bingoTemplet)
	{
		List<NKMMissionTemplet> missionTempletListByType = NKMMissionManager.GetMissionTempletListByType(bingoTemplet.m_BingoMissionTabId);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		for (int i = 0; i < m_listSlot.Count; i++)
		{
			NKCPopupEventMissionSlot nKCPopupEventMissionSlot = m_listSlot[i];
			if (i < missionTempletListByType.Count)
			{
				nKCPopupEventMissionSlot.SetData(missionTempletListByType[i], nKMUserData.m_MissionData.GetMissionData(missionTempletListByType[i]));
				NKCUtil.SetGameobjectActive(nKCPopupEventMissionSlot, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCPopupEventMissionSlot, bValue: false);
			}
		}
	}

	private void OnTouchProgress(NKMMissionTemplet templet, NKMMissionData data)
	{
		if (templet != null)
		{
			NKCContentManager.MoveToShortCut(templet.m_ShortCutType, templet.m_ShortCut);
		}
	}

	private void OnTouchComplete(NKMMissionTemplet templet, NKMMissionData data)
	{
		if (templet != null && data != null && (dCheckTime == null || dCheckTime(bPopup: true)))
		{
			NKCPacketSender.Send_NKMPacket_MISSION_COMPLETE_REQ(templet);
		}
	}

	public void OnCompleteMision(int missionID)
	{
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(missionID);
		if (missionTemplet == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(missionTemplet.m_MissionTabId);
		if (missionTabTemplet != null && missionTabTemplet.m_MissionType == NKM_MISSION_TYPE.BINGO)
		{
			int eventID = 0;
			int tileIndex = -1;
			if (missionTemplet.m_MissionReward.Count > 0)
			{
				eventID = missionTemplet.m_MissionReward[0].reward_id;
				tileIndex = missionTemplet.m_MissionReward[0].reward_value;
			}
			if (dOnComplete != null)
			{
				dOnComplete(eventID, tileIndex);
			}
			Close();
		}
	}
}
