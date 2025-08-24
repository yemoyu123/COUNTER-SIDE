using ClientPacket.Community;
using NKM;

namespace NKC.UI.Friend;

public class NKCUIFriend : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_friend";

	public const string UI_ASSET_NAME = "NKM_UI_FRIEND";

	private static NKCUIManager.LoadedUIData s_LoadedUIData;

	public NKCUIFriendLeftMenu m_NKCUIFriendLeftMenu;

	public static bool IsInstanceOpen
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsUIOpen;
			}
			return false;
		}
	}

	public static bool IsInstanceLoaded
	{
		get
		{
			if (s_LoadedUIData != null)
			{
				return s_LoadedUIData.IsLoadComplete;
			}
			return false;
		}
	}

	public override string GuideTempletID => "ARTICLE_SYSTEM_FRIEND";

	public override string MenuName => NKCUtilString.GET_STRING_FRIEND;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		if (!NKCUIManager.IsValid(s_LoadedUIData))
		{
			s_LoadedUIData = NKCUIManager.OpenNewInstanceAsync<NKCUIFriend>("ab_ui_nkm_ui_friend", "NKM_UI_FRIEND", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
		}
		return s_LoadedUIData;
	}

	public static NKCUIFriend GetInstance()
	{
		if (s_LoadedUIData != null && s_LoadedUIData.IsLoadComplete)
		{
			return s_LoadedUIData.GetInstance<NKCUIFriend>();
		}
		return null;
	}

	public static void CleanupInstance()
	{
		s_LoadedUIData = null;
	}

	public void InitUI()
	{
		m_NKCUIFriendLeftMenu.Init();
	}

	public void Open(NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE openRegisterTab)
	{
		if (!base.IsOpen)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			switch (openRegisterTab)
			{
			case NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE.FTMT_REGISTER:
				m_NKCUIFriendLeftMenu.ForceClickRegisterBtn();
				break;
			case NKCUIFriendTopMenu.FRIEND_TOP_MENU_TYPE.FTMT_MY_PROFILE:
				m_NKCUIFriendLeftMenu.OnClickMyProfile(bSet: true);
				break;
			default:
				m_NKCUIFriendLeftMenu.Reset();
				break;
			}
			UIOpened();
		}
	}

	public override void CloseInternal()
	{
		if (base.gameObject.activeSelf)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			m_NKCUIFriendLeftMenu.CloseSortMenu();
			m_NKCUIFriendLeftMenu?.Close();
		}
	}

	public NKCUIFriendSlot.FRIEND_SLOT_TYPE GetCurrentSlotType()
	{
		return m_NKCUIFriendLeftMenu.GetCurrentSlotType();
	}

	public void SetAddReceivedNew(bool bSet)
	{
		m_NKCUIFriendLeftMenu.SetAddReceiveNew(bSet);
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
	}

	public void UpdateMyMainCharUI()
	{
		m_NKCUIFriendLeftMenu?.UpdateMainCharUI();
	}

	public void OnRecv(NKMPacket_FRIEND_LIST_ACK cNKMPacket_FRIEND_LIST_ACK)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_LIST_ACK);
	}

	public void OnRecv(NKMPacket_SET_EMBLEM_ACK cNKMPacket_SET_EMBLEM_ACK)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_SET_EMBLEM_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_RECOMMEND_ACK cNKMPacket_FRIEND_RECOMMEND_ACK)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_RECOMMEND_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_ACCEPT_NOT cNKMPacket_FRIEND_ACCEPT_NOT)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_ACCEPT_NOT);
	}

	public void OnRecv(NKMPacket_FRIEND_DELETE_NOT cNKMPacket_FRIEND_DEL_NOT)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_DEL_NOT);
	}

	public void OnRecv(NKMPacket_FRIEND_REQUEST_NOT cNKMPacket_FRIEND_ADD_NOT)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_ADD_NOT);
	}

	public void OnRecv(NKMPacket_FRIEND_CANCEL_REQUEST_NOT cNKMPacket_FRIEND_ADD_CANCEL_NOT)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_ADD_CANCEL_NOT);
	}

	public void OnRecv(NKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_PROFILE_MODIFY_INTRO_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK cNKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_PROFILE_MODIFY_DECK_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_DELETE_ACK cNKMPacket_FRIEND_DEL_ACK)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_DEL_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_SEARCH_ACK cNKMPacket_FRIEND_SEARCH_ACK)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_SEARCH_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_REQUEST_ACK cNKMPacket_FRIEND_ADD_ACK)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_ADD_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_ACCEPT_ACK cNKMPacket_FRIEND_ACCEPT_ACK)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_ACCEPT_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_CANCEL_REQUEST_ACK cNKMPacket_FRIEND_ADD_CANCEL_ACK)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_ADD_CANCEL_ACK);
	}

	public void OnRecv(NKMPacket_FRIEND_BLOCK_ACK cNKMPacket_FRIEND_BLOCK_ACK)
	{
		m_NKCUIFriendLeftMenu.OnRecv(cNKMPacket_FRIEND_BLOCK_ACK);
	}

	public void RefreshNickname()
	{
		m_NKCUIFriendLeftMenu.RefreshNickname();
	}

	public override void OnGuildDataChanged()
	{
		m_NKCUIFriendLeftMenu?.OnGuildDataChanged();
	}

	public int DecreaseWaitingRespondCount()
	{
		return m_NKCUIFriendLeftMenu.DecreaseWaitingRespondCount();
	}
}
