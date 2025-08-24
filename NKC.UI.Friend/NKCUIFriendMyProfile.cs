using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Community;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Friend;

public class NKCUIFriendMyProfile : MonoBehaviour
{
	public Text m_NKM_UI_FRIEND_PROFILE_INFO_NAME;

	public Text m_NKM_UI_FRIEND_PROFILE_INFO_LEVEL;

	public Text m_UID_TEXT;

	public InputField m_IFComment;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_BadgeUI;

	public Text m_lbGuildName;

	public Text m_lbGuildLevel;

	public GameObject m_NKM_UI_FRIEND_PROFILE_CONSORTIUM_POLYARROW;

	public GameObject m_NKM_UI_FRIEND_PROFILE_CONSORTIUM_TEXT;

	public Image m_ANIM_SHIP_IMG;

	public NKCUIOperatorDeckSlot m_OperatorSlot;

	public List<NKCDeckViewUnitSlot> m_lstNKCDeckViewUnitSlot;

	public NKCUILeagueTier m_NKCUILeagueTier;

	public Text m_lbGauntletTier;

	public Text m_lbGauntletScore;

	public NKCUISlotProfile m_SlotMyProfile;

	public NKCUIComStateButton m_csbtnNicknameChange;

	public NKCUIComButton m_cbtnCopy;

	public NKCUIComButton m_cbtnCommentChange;

	public NKCUIComButton m_btnProfileUnitChange;

	public NKCUIComButton m_btnProfileFrameChange;

	public NKCUIComButton m_btnProfileDeckChange;

	public List<NKCUISlot> m_lstEmblem;

	public List<GameObject> m_lstEmblemEffect;

	private int m_EmblemIndexToChange = -1;

	private bool m_bCommentChangeButtonClicked;

	public bool IsNKCPopupEmblemListOpen => NKCPopupEmblemList.IsInstanceOpen;

	public int WaitingRespondCount { get; set; }

	public void CheckNKCPopupEmblemListAndClose()
	{
		if (NKCPopupEmblemList.IsInstanceOpen)
		{
			NKCPopupEmblemList.Instance.Close();
		}
	}

	public void Init()
	{
		for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_lstNKCDeckViewUnitSlot[i];
			if (nKCDeckViewUnitSlot != null)
			{
				nKCDeckViewUnitSlot.Init(i);
			}
		}
		m_csbtnNicknameChange?.PointerClick.RemoveAllListeners();
		m_csbtnNicknameChange?.PointerClick.AddListener(OnClickChangeNickname);
		m_cbtnCommentChange.PointerClick.RemoveAllListeners();
		m_cbtnCommentChange.PointerClick.AddListener(OnClickChangeComment);
		m_IFComment.onValidateInput = NKCFilterManager.FilterEmojiInput;
		m_IFComment.onValueChanged.RemoveAllListeners();
		m_IFComment.onValueChanged.AddListener(OnValueChangedComment);
		m_IFComment.onEndEdit.RemoveAllListeners();
		m_IFComment.onEndEdit.AddListener(OnDoneComment);
		m_cbtnCopy.PointerClick.RemoveAllListeners();
		m_cbtnCopy.PointerClick.AddListener(OnClickCopy);
		NKCUtil.SetButtonClickDelegate(m_btnProfileUnitChange, OpenProfileImageChange);
		NKCUtil.SetButtonClickDelegate(m_btnProfileFrameChange, OpenProfileFrameChange);
		NKCUtil.SetButtonClickDelegate(m_btnProfileDeckChange, OpenMainDeckSelectWindow);
		for (int j = 0; j < m_lstEmblem.Count; j++)
		{
			m_lstEmblem[j].Init();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD))
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_PROFILE_CONSORTIUM_POLYARROW, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_PROFILE_CONSORTIUM_TEXT, bValue: false);
		}
		if (m_OperatorSlot != null)
		{
			m_OperatorSlot.Init();
		}
	}

	private void OnOKToChange(int id)
	{
		if (m_EmblemIndexToChange >= 0)
		{
			NKCPacketSender.Send_NKMPacket_SET_EMBLEM_REQ(m_EmblemIndexToChange, id);
		}
	}

	private void OnClickChangeEmblem0(NKCUISlot.SlotData slotData, bool bLocked)
	{
		m_EmblemIndexToChange = 0;
		OpenEmblemPopup();
	}

	private void OnClickChangeEmblem1(NKCUISlot.SlotData slotData, bool bLocked)
	{
		m_EmblemIndexToChange = 1;
		OpenEmblemPopup();
	}

	private void OnClickChangeEmblem2(NKCUISlot.SlotData slotData, bool bLocked)
	{
		m_EmblemIndexToChange = 2;
		OpenEmblemPopup();
	}

	private void OpenEmblemPopup()
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData == null)
		{
			return;
		}
		bool bUseEmpty = true;
		NKCScenManager.CurrentUserData().m_InventoryData.GetEmblemData();
		for (int i = 0; i < userProfileData.emblems.Count; i++)
		{
			if (userProfileData.emblems[i] != null && i == m_EmblemIndexToChange && userProfileData.emblems[i].id == 0)
			{
				bUseEmpty = false;
			}
		}
		NKCPopupEmblemList.Instance.Open(userProfileData.emblems[m_EmblemIndexToChange].id, bUseEmpty, OnOKToChange);
	}

	public bool IsOpen()
	{
		return base.gameObject.activeSelf;
	}

	public void OnRecv(NKMPacket_SET_EMBLEM_ACK cNKMPacket_SET_EMBLEM_ACK)
	{
		if (cNKMPacket_SET_EMBLEM_ACK.index >= 0 && m_lstEmblem.Count > cNKMPacket_SET_EMBLEM_ACK.index)
		{
			int index = cNKMPacket_SET_EMBLEM_ACK.index;
			int itemId = cNKMPacket_SET_EMBLEM_ACK.itemId;
			long count = cNKMPacket_SET_EMBLEM_ACK.count;
			if (itemId > 0 && NKMItemManager.GetItemMiscTempletByID(itemId) != null)
			{
				m_lstEmblem[index].SetMiscItemData(itemId, count, bShowName: false, bShowCount: true, bEnableLayoutElement: true, GetOnClickMethod(index));
			}
			else
			{
				m_lstEmblem[index].SetEmpty(GetOnClickMethod(index));
			}
			NKCUtil.SetGameobjectActive(m_lstEmblemEffect[index], bValue: false);
			NKCUtil.SetGameobjectActive(m_lstEmblemEffect[index], bValue: true);
		}
	}

	private void OnEnable()
	{
		for (int i = 0; i < m_lstEmblemEffect.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstEmblemEffect[i], bValue: false);
		}
	}

	public NKCUISlot.OnClick GetOnClickMethod(int index)
	{
		return index switch
		{
			0 => OnClickChangeEmblem0, 
			1 => OnClickChangeEmblem1, 
			2 => OnClickChangeEmblem2, 
			_ => OnClickChangeEmblem0, 
		};
	}

	public void UpdateMainCharUI()
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData != null)
		{
			m_SlotMyProfile?.SetProfiledata(userProfileData, null, NKCTacticUpdateUtil.IsMaxTacticLevel(userProfileData.commonProfile.mainUnitTacticLevel));
		}
	}

	public void UpdateGuildData()
	{
		if (m_objGuild != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuild, NKCGuildManager.HasGuild());
			if (m_objGuild.activeSelf)
			{
				m_BadgeUI.SetData(NKCGuildManager.MyGuildData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, NKCGuildManager.MyGuildData.name);
				NKCUtil.SetLabelText(m_lbGuildLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, NKCGuildManager.MyGuildData.guildLevel));
			}
		}
	}

	public void UpdateCommentUI()
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData != null && !string.IsNullOrEmpty(userProfileData.friendIntro))
		{
			m_IFComment.text = NKCFilterManager.CheckBadChat(userProfileData.friendIntro);
		}
	}

	public void UpdateDeckUI()
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData != null && userProfileData.profileDeck != null)
		{
			NKMUnitTempletBase nKMUnitTempletBase = ((userProfileData.profileDeck.Ship != null) ? NKMUnitManager.GetUnitTempletBase(userProfileData.profileDeck.Ship.UnitId) : null);
			if (nKMUnitTempletBase != null)
			{
				m_ANIM_SHIP_IMG.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, nKMUnitTempletBase);
			}
			else
			{
				m_ANIM_SHIP_IMG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_TEXTURE", "NKM_DECK_VIEW_SHIP_UNKNOWN");
			}
			for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
			{
				if (i < 8)
				{
					NKMUnitData nKMUnitData = null;
					if (userProfileData.profileDeck.List[i] != null)
					{
						nKMUnitData = new NKMUnitData();
						nKMUnitData.FillDataFromDummy(userProfileData.profileDeck.List[i]);
					}
					m_lstNKCDeckViewUnitSlot[i].SetData(nKMUnitData, bEnableButton: false);
				}
			}
			if (NKCOperatorUtil.IsHide())
			{
				NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: true);
			if (userProfileData.profileDeck.operatorUnit == null)
			{
				m_OperatorSlot.SetLock();
				return;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(userProfileData.profileDeck.operatorUnit.UnitId);
			if (unitTempletBase != null)
			{
				m_OperatorSlot.SetData(unitTempletBase, userProfileData.profileDeck.operatorUnit.UnitLevel);
			}
			else
			{
				m_OperatorSlot.SetEmpty();
			}
			return;
		}
		NKCUtil.SetImageSprite(m_ANIM_SHIP_IMG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_SPRITE", "NKM_DECK_VIEW_SHIP_UNKNOWN"));
		for (int j = 0; j < m_lstNKCDeckViewUnitSlot.Count; j++)
		{
			if (j < 8)
			{
				m_lstNKCDeckViewUnitSlot[j].SetData(null);
			}
		}
		if (NKCOperatorUtil.IsHide())
		{
			NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: true);
		m_OperatorSlot.SetLock();
	}

	private void UpdateGauntletTierUI()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		PvpState pvpState;
		NKM_GAME_TYPE gameType;
		if (myUserData.m_PvpData.Score >= myUserData.m_AsyncData.Score)
		{
			pvpState = myUserData.m_PvpData;
			gameType = NKM_GAME_TYPE.NGT_PVP_RANK;
		}
		else
		{
			pvpState = myUserData.m_AsyncData;
			gameType = NKM_GAME_TYPE.NGT_ASYNC_PVP;
		}
		int num = NKCPVPManager.FindPvPSeasonID(gameType, NKCSynchronizedTime.GetServerUTCTime());
		int leagueScore = pvpState.Score;
		if (num != pvpState.SeasonID)
		{
			leagueScore = NKCPVPManager.GetResetScore(pvpState.SeasonID, pvpState.Score, gameType);
			NKMPvpRankTemplet rankTempletByScore = NKCPVPManager.GetRankTempletByScore(gameType, num, leagueScore);
			if (rankTempletByScore != null)
			{
				m_NKCUILeagueTier.SetUI(rankTempletByScore);
				m_lbGauntletTier.text = rankTempletByScore.GetLeagueName();
			}
			else
			{
				m_lbGauntletTier.text = "";
			}
		}
		else
		{
			NKMPvpRankTemplet rankTempletByTier = NKCPVPManager.GetRankTempletByTier(gameType, num, pvpState.LeagueTierID);
			if (rankTempletByTier != null)
			{
				m_NKCUILeagueTier.SetUI(rankTempletByTier);
				m_lbGauntletTier.text = rankTempletByTier.GetLeagueName();
			}
			else
			{
				m_lbGauntletTier.text = "";
			}
		}
		m_lbGauntletScore.text = leagueScore.ToString();
	}

	private void UpdateEmblemUI()
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData == null)
		{
			for (int i = 0; i < m_lstEmblem.Count; i++)
			{
				m_lstEmblem[i].SetEmpty();
			}
			return;
		}
		for (int j = 0; j < m_lstEmblem.Count; j++)
		{
			NKCUISlot nKCUISlot = m_lstEmblem[j];
			if (j < userProfileData.emblems.Count && userProfileData.emblems[j] != null && userProfileData.emblems[j].id > 0 && NKMItemManager.GetItemMiscTempletByID(userProfileData.emblems[j].id) != null)
			{
				if (j <= 3)
				{
					nKCUISlot.SetMiscItemData(userProfileData.emblems[j].id, userProfileData.emblems[j].count, bShowName: false, bShowCount: true, bEnableLayoutElement: true, GetOnClickMethod(j));
				}
				else
				{
					nKCUISlot.SetEmpty();
				}
			}
			else if (j <= 3)
			{
				nKCUISlot.SetEmpty(GetOnClickMethod(j));
			}
			else
			{
				nKCUISlot.SetEmpty();
			}
		}
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		UpdateMainCharUI();
		UpdateDeckUI();
		UpdateGuildData();
		UpdateCommentUI();
		UpdateGauntletTierUI();
		m_EmblemIndexToChange = -1;
		UpdateEmblemUI();
		for (int i = 0; i < m_lstEmblemEffect.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstEmblemEffect[i], bValue: false);
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			m_NKM_UI_FRIEND_PROFILE_INFO_NAME.text = myUserData.m_UserNickName;
			m_NKM_UI_FRIEND_PROFILE_INFO_LEVEL.text = string.Format(NKCUtilString.GET_STRING_FRIEND_INFO_LEVEL_ONE_PARAM, myUserData.UserLevel);
			m_UID_TEXT.text = NKCUtilString.GetFriendCode(myUserData.m_FriendCode);
		}
		NKCUtil.SetGameobjectActive(m_btnProfileFrameChange, NKCContentManager.IsContentsUnlocked(ContentsType.PROFILE_FRAME));
		WaitingRespondCount = 0;
	}

	private void OnValueChangedComment(string str)
	{
		m_IFComment.textComponent.color = NKCUtil.GetColor("#4EC2F3");
		m_IFComment.textComponent.fontStyle = FontStyle.Normal;
	}

	private void OnDoneComment(string str)
	{
		if (!EventSystem.current.alreadySelecting)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData == null)
		{
			return;
		}
		m_IFComment.text = NKCFilterManager.CheckBadChat(m_IFComment.text);
		if (m_IFComment.text == NKCFilterManager.CheckBadChat(userProfileData.friendIntro))
		{
			if (NKCUIManager.FrontCanvas != null)
			{
				m_bCommentChangeButtonClicked = RectTransformUtility.RectangleContainsScreenPoint(m_cbtnCommentChange.GetComponent<RectTransform>(), Input.mousePosition, NKCUIManager.FrontCanvas.worldCamera);
			}
			if (NKCInputManager.IsChatSubmitEnter())
			{
				m_bCommentChangeButtonClicked = false;
			}
		}
		else
		{
			NKMPacket_FRIEND_PROFILE_MODIFY_INTRO_REQ nKMPacket_FRIEND_PROFILE_MODIFY_INTRO_REQ = new NKMPacket_FRIEND_PROFILE_MODIFY_INTRO_REQ();
			if (m_IFComment.text.Length >= 20)
			{
				nKMPacket_FRIEND_PROFILE_MODIFY_INTRO_REQ.intro = m_IFComment.text.Substring(0, 20);
			}
			else
			{
				nKMPacket_FRIEND_PROFILE_MODIFY_INTRO_REQ.intro = m_IFComment.text;
			}
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_PROFILE_MODIFY_INTRO_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}

	public void RefreshNickname()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			m_NKM_UI_FRIEND_PROFILE_INFO_NAME.text = myUserData.m_UserNickName;
		}
	}

	private void OnClickChangeNickname()
	{
		if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(510) > 0)
		{
			NKCPopupNickname.Instance.Open();
		}
		else
		{
			NKCShopManager.OpenItemLackPopup(510, 1);
		}
	}

	public void OnClickChangeComment()
	{
		if (m_bCommentChangeButtonClicked)
		{
			m_bCommentChangeButtonClicked = false;
			return;
		}
		m_IFComment.Select();
		m_IFComment.ActivateInputField();
	}

	public void OnClickCopy()
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData != null)
		{
			TextEditor textEditor = new TextEditor();
			textEditor.text = userProfileData.commonProfile.friendCode.ToString();
			textEditor.OnFocus();
			textEditor.Copy();
		}
	}

	private void SendMainUnitChangeREQ(int unitId, int skinId, int frameId, int titleId)
	{
		WaitingRespondCount = 0;
		if (unitId > 0)
		{
			int waitingRespondCount = WaitingRespondCount + 1;
			WaitingRespondCount = waitingRespondCount;
			if (!NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.m_illustrateUnit.Contains(unitId))
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_PROFILE_REPRESENT_SKIN_ONLY_NO_UNIT_ERROR"));
				return;
			}
			NKCPacketSender.Send_NKMPacket_FRIEND_PROFILE_MODIFY_MAIN_CHAR_REQ(unitId, skinId);
		}
		if (frameId >= 0)
		{
			int waitingRespondCount = WaitingRespondCount + 1;
			WaitingRespondCount = waitingRespondCount;
			NKCPacketSender.Send_NKMPacket_USER_PROFILE_CHANGE_FRAME_REQ(frameId);
		}
		if (titleId >= 0)
		{
			int waitingRespondCount = WaitingRespondCount + 1;
			WaitingRespondCount = waitingRespondCount;
			NKCPacketSender.Send_NKMPacket_UPDATE_TITLE_REQ(titleId);
		}
	}

	public void OpenProfileImageChange()
	{
		NKCPopupEmblemList.Instance.OpenProfileEdit(SendMainUnitChangeREQ);
	}

	public void OpenProfileFrameChange()
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		NKCUIPopupProfileFrameChange.Instance.Open(userProfileData, OnFrameChanged);
	}

	public void OnFrameChanged(int frameID)
	{
		if (NKCScenManager.CurrentUserData().UserProfileData.commonProfile.frameId != frameID)
		{
			NKCPacketSender.Send_NKMPacket_USER_PROFILE_CHANGE_FRAME_REQ(frameID);
		}
	}

	public void OpenMainDeckSelectWindow()
	{
		NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
		{
			MenuName = NKCUtilString.GET_STRING_FRIEND_MAIN_DECK,
			eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.MainDeckSelect,
			dOnSideMenuButtonConfirm = OnClickMainDeckSelect,
			DeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_DAILY, 0),
			SelectLeaderUnitOnOpen = false,
			bEnableDefaultBackground = true,
			bUpsideMenuHomeButton = false,
			StageBattleStrID = string.Empty
		};
		NKCUIDeckViewer.Instance.Open(options);
	}

	public void OnClickMainDeckSelect(NKMDeckIndex selectedDeckIndex, long supportUserUID = 0L)
	{
		NKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ nKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ = new NKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ();
		nKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ.deckIndex = selectedDeckIndex;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		CheckNKCPopupEmblemListAndClose();
	}

	private void OnDestroy()
	{
	}
}
