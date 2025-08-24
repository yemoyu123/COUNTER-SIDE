using System.Collections.Generic;
using ClientPacket.Common;
using NKC.UI.Event;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupTournamentFinalUserList : NKCUIBase
{
	public const string UI_ASSET_BUNDLE_NAME = "UI_SINGLE_TOURNAMENT";

	public const string UI_ASSET_NAME = "UI_SINGLE_POPUP_TOURNAMENT_USERLIST";

	private static NKCPopupTournamentFinalUserList m_Instance;

	public NKCUITournamentPlayerSlot m_pfbSlot;

	public NKCUIComStateButton m_btnClose;

	public ScrollRect m_srList;

	private List<NKCUITournamentPlayerSlot> m_lstVisibleSlot = new List<NKCUITournamentPlayerSlot>();

	public static NKCPopupTournamentFinalUserList Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupTournamentFinalUserList>("UI_SINGLE_TOURNAMENT", "UI_SINGLE_POPUP_TOURNAMENT_USERLIST", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanUpInstance).GetInstance<NKCPopupTournamentFinalUserList>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

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

	public override string MenuName => "";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanUpInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_lstVisibleSlot.Clear();
	}

	public void Open()
	{
		if (m_lstVisibleSlot.Count != NKCTournamentManager.m_lstFinalUserInfos.Count)
		{
			BuildSlot(NKCTournamentManager.m_lstFinalUserInfos.Count);
		}
		for (int i = 0; i < m_lstVisibleSlot.Count; i++)
		{
			if (!(m_lstVisibleSlot[i] == null))
			{
				NKCUITournamentPlayerSlotTree.ProfileDataSet profildDataSet = GetProfildDataSet(NKCTournamentManager.m_lstFinalUserInfos[i], i);
				m_lstVisibleSlot[i].SetData(profildDataSet.slotIndex, profildDataSet.profildData, profildDataSet.profildDataPredict, NKMTournamentGroups.None);
			}
		}
		UIOpened();
	}

	private void BuildSlot(int slotCount)
	{
		for (int i = 0; i < m_lstVisibleSlot.Count; i++)
		{
			Object.Destroy(m_lstVisibleSlot[i].gameObject);
		}
		m_lstVisibleSlot.Clear();
		for (int j = 0; j < slotCount; j++)
		{
			NKCUITournamentPlayerSlot nKCUITournamentPlayerSlot = Object.Instantiate(m_pfbSlot, m_srList.content);
			nKCUITournamentPlayerSlot.Init(OnClickSlot);
			m_lstVisibleSlot.Add(nKCUITournamentPlayerSlot);
		}
	}

	protected NKCUITournamentPlayerSlotTree.ProfileDataSet GetProfildDataSet(NKMTournamentProfileData profileData, int slotIndex)
	{
		NKCUITournamentPlayerSlotTree.ProfileDataSet result = default(NKCUITournamentPlayerSlotTree.ProfileDataSet);
		result.profildData = profileData;
		result.profildDataPredict = null;
		result.slotIndex = slotIndex;
		result.group = NKMTournamentGroups.Finals;
		return result;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickSlot(int slotIndex, long userUId)
	{
		NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(userUId, NKM_DECK_TYPE.NDT_NORMAL);
	}
}
