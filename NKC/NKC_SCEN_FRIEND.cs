using ClientPacket.Community;
using NKC.UI;
using NKC.UI.Friend;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_FRIEND : NKC_SCEN_BASIC
{
	private static NKCUIManager.LoadedUIData FriendUIData;

	private NKCUIFriend m_NKCUIFriend;

	private NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE m_eReservedOpenRegisterTab;

	public NKC_SCEN_FRIEND()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_FRIEND;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (!NKCUIManager.IsValid(FriendUIData))
		{
			FriendUIData = NKCUIFriend.OpenNewInstanceAsync();
		}
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		if (m_NKCUIFriend == null)
		{
			if (FriendUIData != null && FriendUIData.CheckLoadAndGetInstance<NKCUIFriend>(out m_NKCUIFriend))
			{
				m_NKCUIFriend.InitUI();
				SetAddReceivedNew(NKCFriendManager.ReceivedREQList.Count > 0);
			}
			else
			{
				Debug.LogError("Error - NKC_SCEN_FRIEND.ScenLoadComplete() : UIFriendLoadResourceData is null");
			}
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		m_NKCUIFriend.Open(m_eReservedOpenRegisterTab);
		m_eReservedOpenRegisterTab = NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE.FTMT_MANAGE;
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		NKCPopupImageChange.CheckInstanceAndClose();
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.Close();
			m_NKCUIFriend = null;
		}
		FriendUIData?.CloseInstance();
		FriendUIData = null;
	}

	public void OpenImageChangeForUnit(NKCPopupImageChange.OnClickOK _OnClickOK)
	{
		NKCPopupImageChange.Instance.OpenForUnit(_OnClickOK);
	}

	public void CloseImageChange()
	{
		NKCPopupImageChange.CheckInstanceAndClose();
	}

	public void SetAddReceivedNew(bool bSet)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.SetAddReceivedNew(bSet);
		}
	}

	public void SetReservedTab(NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE reservedTab)
	{
		m_eReservedOpenRegisterTab = reservedTab;
	}

	public NKCUIFriendSlot.FRIEND_SLOT_TYPE GetCurrentSlotType()
	{
		if (m_NKCUIFriend != null)
		{
			return m_NKCUIFriend.GetCurrentSlotType();
		}
		return NKCUIFriendSlot.FRIEND_SLOT_TYPE.FST_FRIEND_LIST;
	}

	public void OnRecv(NKMPacket_FRIEND_LIST_ACK cNKMPacket_FRIEND_LIST_ACK)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(cNKMPacket_FRIEND_LIST_ACK);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_ACCEPT_NOT cNKMPacket_FRIEND_ACCEPT_NOT)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(cNKMPacket_FRIEND_ACCEPT_NOT);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_DELETE_NOT not)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(not);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_REQUEST_NOT not)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(not);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_CANCEL_REQUEST_NOT not)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(not);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK cNKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(cNKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_ACK cNKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_ACK)
	{
		NKCPopupImageChange.CheckInstanceAndClose();
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.UpdateMyMainCharUI();
		}
	}

	public void OnRecv(NKMPacket_FRIEND_DELETE_ACK ack)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(ack);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_SEARCH_ACK cNKMPacket_FRIEND_SEARCH_ACK)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(cNKMPacket_FRIEND_SEARCH_ACK);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_RECOMMEND_ACK cNKMPacket_FRIEND_RECOMMEND_ACK)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(cNKMPacket_FRIEND_RECOMMEND_ACK);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_REQUEST_ACK ack)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(ack);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_ACCEPT_ACK cNKMPacket_FRIEND_ACCEPT_ACK)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(cNKMPacket_FRIEND_ACCEPT_ACK);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_CANCEL_REQUEST_ACK ack)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(ack);
		}
	}

	public void OnRecv(NKMPacket_FRIEND_BLOCK_ACK cNKMPacket_FRIEND_BLOCK_ACK)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(cNKMPacket_FRIEND_BLOCK_ACK);
		}
	}

	public void OnRecv(NKMPacket_SET_EMBLEM_ACK cNKMPacket_SET_EMBLEM_ACK)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.OnRecv(cNKMPacket_SET_EMBLEM_ACK);
		}
	}

	public void OnRecv(NKMPacket_USER_PROFILE_CHANGE_FRAME_ACK sPacket)
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.UpdateMyMainCharUI();
		}
	}

	public void RefreshNickname()
	{
		if (m_NKCUIFriend != null)
		{
			m_NKCUIFriend.RefreshNickname();
		}
	}

	public int DecreaseWaitingRespondCount()
	{
		if (m_NKCUIFriend != null)
		{
			return m_NKCUIFriend.DecreaseWaitingRespondCount();
		}
		return 0;
	}
}
