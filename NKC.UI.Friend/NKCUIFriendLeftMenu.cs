using System.Collections.Generic;
using ClientPacket.Community;
using NKM;
using NKM.Contract2;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.UI.Friend;

public class NKCUIFriendLeftMenu : MonoBehaviour
{
	public NKCUIFriendTopMenu m_NKCUIFriendTopMenu;

	public NKCUIFriendMyProfile m_NKCUIFriendMyProfile;

	public NKCUIComToggle m_ManageBtn;

	public NKCUIComToggle m_RegisterBtn;

	public NKCUIComToggle m_NKM_UI_FRIEND_MENU_MANAGEMENT;

	public NKCUIComToggle m_NKM_UI_FRIEND_MENU_ADD;

	public GameObject m_NKM_UI_FRIEND_MENU_ADD_NEW;

	public NKCUIComToggle m_NKM_UI_FRIEND_MENU_PROFILE;

	public NKCUIComButton m_NKM_UI_FRIEND_MENU_SHOP;

	public void Init()
	{
		m_NKCUIFriendTopMenu.Init();
		m_NKCUIFriendMyProfile.Init();
		if (m_NKM_UI_FRIEND_MENU_MANAGEMENT != null)
		{
			m_NKM_UI_FRIEND_MENU_MANAGEMENT.OnValueChanged.RemoveAllListeners();
			m_NKM_UI_FRIEND_MENU_MANAGEMENT.OnValueChanged.AddListener(OnClickFriendManagement);
		}
		if (m_NKM_UI_FRIEND_MENU_ADD != null)
		{
			m_NKM_UI_FRIEND_MENU_ADD.OnValueChanged.RemoveAllListeners();
			m_NKM_UI_FRIEND_MENU_ADD.OnValueChanged.AddListener(OnClickFriendRegister);
		}
		if (m_NKM_UI_FRIEND_MENU_PROFILE != null)
		{
			m_NKM_UI_FRIEND_MENU_PROFILE.OnValueChanged.RemoveAllListeners();
			m_NKM_UI_FRIEND_MENU_PROFILE.OnValueChanged.AddListener(OnClickMyProfile);
		}
		if (m_NKM_UI_FRIEND_MENU_SHOP != null)
		{
			m_NKM_UI_FRIEND_MENU_SHOP.PointerClick.RemoveAllListeners();
			m_NKM_UI_FRIEND_MENU_SHOP.PointerClick.AddListener(OnClickFriendshipShop);
		}
	}

	public void Reset()
	{
		m_ManageBtn.Select(bSelect: false, bForce: true);
		m_ManageBtn.Select(bSelect: true);
	}

	public void Close()
	{
		m_NKCUIFriendTopMenu.CloseInstance();
	}

	public void SetAddReceiveNew(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENU_ADD_NEW, bSet);
		m_NKCUIFriendTopMenu.SetAddReceiveNew(bSet);
	}

	public void OnClickFriendManagement(bool bSet)
	{
		m_NKCUIFriendTopMenu.Open(NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE.FTMT_MANAGE);
		m_NKCUIFriendMyProfile.Close();
	}

	public void ForceClickRegisterBtn()
	{
		m_RegisterBtn.Select(bSelect: false, bForce: true);
		m_RegisterBtn.Select(bSelect: true);
	}

	public void OnClickFriendRegister(bool bSet)
	{
		m_NKCUIFriendTopMenu.Open(NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE.FTMT_REGISTER);
		m_NKCUIFriendMyProfile.Close();
	}

	public void OnClickMyProfile(bool bSet)
	{
		if (bSet)
		{
			m_NKM_UI_FRIEND_MENU_PROFILE.Select(bSelect: true, bForce: true);
		}
		m_NKCUIFriendTopMenu.Close();
		m_NKCUIFriendMyProfile.Open();
	}

	public void OnClickFriendshipShop()
	{
		string shortCutParam = "";
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		foreach (ContractTempletBase value in NKMTempletContainer<ContractTempletBase>.Values)
		{
			if (NKCSynchronizedTime.IsEventTime(value.EventIntervalTemplet) && (nKCContractDataMgr == null || nKCContractDataMgr.CheckOpenCond(value)))
			{
				HashSet<int> priceItemIDSet = value.GetPriceItemIDSet();
				if (priceItemIDSet != null && priceItemIDSet.Contains(8))
				{
					shortCutParam = value.ContractStrID;
					break;
				}
			}
		}
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_CONTRACT, shortCutParam);
	}

	public void CloseSortMenu()
	{
		m_NKCUIFriendTopMenu.CloseSortMenu(bAnimate: false);
	}

	public NKCUIFriendSlot.FRIEND_SLOT_TYPE GetCurrentSlotType()
	{
		return m_NKCUIFriendTopMenu.GetCurrentSlotType();
	}

	public void OnRecv(NKMPacket_FRIEND_LIST_ACK cNKMPacket_FRIEND_LIST_ACK)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_LIST_ACK);
	}

	public void OnRecv(NKMPacket_SET_EMBLEM_ACK cNKMPacket_SET_EMBLEM_ACK)
	{
		if (!(m_NKCUIFriendMyProfile == null))
		{
			m_NKCUIFriendMyProfile.CheckNKCPopupEmblemListAndClose();
			if (m_NKCUIFriendMyProfile.IsOpen())
			{
				m_NKCUIFriendMyProfile.OnRecv(cNKMPacket_SET_EMBLEM_ACK);
			}
		}
	}

	public void OnRecv(NKMPacket_FRIEND_ACCEPT_NOT cNKMPacket_FRIEND_ACCEPT_NOT)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_ACCEPT_NOT);
	}

	public void OnRecv(NKMPacket_FRIEND_DELETE_NOT cNKMPacket_FRIEND_DEL_NOT)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_DEL_NOT);
	}

	public void OnRecv(NKMPacket_FRIEND_REQUEST_NOT cNKMPacket_FRIEND_ADD_NOT)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_ADD_NOT);
	}

	public void OnRecv(NKMPacket_FRIEND_CANCEL_REQUEST_NOT cNKMPacket_FRIEND_ADD_CANCEL_NOT)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_ADD_CANCEL_NOT);
	}

	public void OnRecv(NKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK)
	{
		if (m_NKCUIFriendMyProfile.IsOpen())
		{
			m_NKCUIFriendMyProfile.UpdateCommentUI();
		}
	}

	public void OnRecv(NKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK cNKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK)
	{
		if (m_NKCUIFriendMyProfile.IsOpen())
		{
			m_NKCUIFriendMyProfile.UpdateDeckUI();
		}
	}

	public void UpdateMainCharUI()
	{
		if (m_NKCUIFriendMyProfile.IsOpen())
		{
			m_NKCUIFriendMyProfile.UpdateMainCharUI();
			m_NKCUIFriendMyProfile.CheckNKCPopupEmblemListAndClose();
		}
	}

	public void OnRecv(NKMPacket_FRIEND_DELETE_ACK cNKMPacket_FRIEND_DEL_ACK)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_DEL_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_SEARCH_ACK cNKMPacket_FRIEND_SEARCH_ACK)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_SEARCH_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_RECOMMEND_ACK cNKMPacket_FRIEND_RECOMMEND_ACK)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_RECOMMEND_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_REQUEST_ACK cNKMPacket_FRIEND_ADD_ACK)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_ADD_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_ACCEPT_ACK cNKMPacket_FRIEND_ACCEPT_ACK)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_ACCEPT_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_CANCEL_REQUEST_ACK cNKMPacket_FRIEND_ADD_CANCEL_ACK)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_ADD_CANCEL_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_BLOCK_ACK cNKMPacket_FRIEND_BLOCK_ACK)
	{
		m_NKCUIFriendTopMenu.OnRecv(cNKMPacket_FRIEND_BLOCK_ACK);
	}

	public void RefreshNickname()
	{
		m_NKCUIFriendMyProfile.RefreshNickname();
	}

	public void OnGuildDataChanged()
	{
		m_NKCUIFriendMyProfile.UpdateGuildData();
	}

	public int DecreaseWaitingRespondCount()
	{
		return --m_NKCUIFriendMyProfile.WaitingRespondCount;
	}
}
