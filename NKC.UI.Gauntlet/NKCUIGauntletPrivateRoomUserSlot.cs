using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Pvp;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletPrivateRoomUserSlot : MonoBehaviour
{
	public delegate void OnDragBegin();

	public delegate bool OnSelect(long userUId);

	[Header("빈 슬롯")]
	public GameObject m_objEmptySlot;

	[Header("유저 슬롯")]
	public GameObject m_objUserSlot;

	public NKCUISlotProfile m_NKCUISlot;

	public NKCUISlotTitle m_TitleSlot;

	public NKCUIComStateButton m_csbtnDeckEdit;

	public GameObject m_objDeckEditFx;

	public Image m_imgTierIcon;

	public TMP_Text m_lbTierNumber;

	[Space]
	public TMP_Text m_lbLevel;

	public TMP_Text m_lbName;

	public TMP_Text m_lbUID;

	public NKCUIComStateButton m_csbtnSimpleUserInfoSlot;

	public GameObject m_objMySlot;

	public GameObject m_objHost;

	public GameObject m_objSelectable;

	public GameObject m_objSelectKick;

	public GameObject m_objBattleTag;

	[Header("길드")]
	public GameObject m_objGuild;

	public NKCUIGuildBadge m_GuildBadgeUI;

	public TMP_Text m_lbGuildName;

	[Header("로딩표시")]
	public GameObject m_objLoading;

	[Header("플레이어 표시")]
	public GameObject m_objPlayer1Tag;

	public GameObject m_objPlayer2Tag;

	private long m_UserUID;

	private PvpPlayerRole m_PlayerRole;

	private OnDragBegin m_dOnDragBegin;

	private OnSelect m_dOnSelect;

	private NKCAssetInstanceData m_InstanceData;

	private NKMCommonProfile m_CommonProfile;

	private NKMGuildSimpleData m_GuildSimpleData;

	private NKMDeckIndex m_curDeckIndex;

	public long UserUID => m_UserUID;

	public bool IsEmpty => m_objEmptySlot.activeSelf;

	public static NKCUIGauntletPrivateRoomUserSlot GetNewInstance(Transform parent, OnDragBegin onDragBegin)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_PRIVATE_ROOM_USER_SLOT");
		NKCUIGauntletPrivateRoomUserSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIGauntletPrivateRoomUserSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIGauntletPrivateRoomUserSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localScale = new Vector3(1f, 1f, 1f);
		component.m_dOnDragBegin = onDragBegin;
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDragBeginImpl()
	{
		if (m_dOnDragBegin != null)
		{
			m_dOnDragBegin();
		}
	}

	private void OnClick()
	{
		if (m_UserUID > 0 && (m_dOnSelect == null || !m_dOnSelect(m_UserUID)))
		{
			NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(m_UserUID, NKM_DECK_TYPE.NDT_PVP);
		}
	}

	public void Init(OnSelect dOnSelect)
	{
		NKCUtil.SetGameobjectActive(m_objEmptySlot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objUserSlot, bValue: false);
		NKCUtil.SetButtonClickDelegate(m_csbtnDeckEdit, OnClickDeckEdit);
		NKCUtil.SetGameobjectActive(m_objLoading, bValue: false);
		m_dOnSelect = dOnSelect;
	}

	public void SetPlayerRole(PvpPlayerRole playerRole)
	{
		m_PlayerRole = playerRole;
	}

	public void SetSelectableActive(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objSelectable, value);
	}

	public void SetKickSelectActive(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objSelectKick, value);
	}

	public void SetEmptyUI()
	{
		m_UserUID = 0L;
		m_csbtnSimpleUserInfoSlot?.PointerClick.RemoveAllListeners();
		NKCUtil.SetGameobjectActive(m_objEmptySlot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objUserSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objHost, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnDeckEdit, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSelectKick, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSelectable, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBattleTag, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLoading, bValue: false);
	}

	public void SetUI(NKMPvpGameLobbyUserState userState)
	{
		if (userState != null)
		{
			bool bValue = NKCPrivatePVPRoomMgr.IsHost(NKCPrivatePVPRoomMgr.GetMyPvpGameLobbyUserState());
			NKCUtil.SetGameobjectActive(m_objHost, bValue);
			if (m_UserUID != userState.profileData.commonProfile.userUid)
			{
				NKCUtil.SetGameobjectActive(m_objDeckEditFx, bValue: true);
			}
			SetUI(userState.profileData, userState.isHost);
			long num = NKCScenManager.CurrentUserData()?.m_UserUID ?? 0;
			NKCUtil.SetGameobjectActive(m_csbtnDeckEdit, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnDeckEdit, num == m_UserUID && NKCPrivatePVPRoomMgr.IsPlayer(m_UserUID) && !userState.isReady);
			NKCUtil.SetGameobjectActive(m_objSelectKick, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSelectable, bValue: false);
			NKCUtil.SetGameobjectActive(m_objBattleTag, NKCPrivatePVPRoomMgr.IsPlayer(m_UserUID));
			NKCUtil.SetGameobjectActive(m_objLoading, userState.playerState != LobbyPlayerState.Lobby);
			NKCUtil.SetGameobjectActive(m_objPlayer1Tag, NKCPrivatePVPRoomMgr.GetPlayerSlotIndex(userState) == 0);
			NKCUtil.SetGameobjectActive(m_objPlayer2Tag, NKCPrivatePVPRoomMgr.GetPlayerSlotIndex(userState) == 1);
			m_curDeckIndex = userState.deckIndex;
		}
	}

	private void SetUI(NKMUserProfileData userProfileData, bool bHost)
	{
		if (userProfileData != null)
		{
			m_CommonProfile = userProfileData.commonProfile;
			m_GuildSimpleData = userProfileData.guildData;
			m_TitleSlot?.SetData(userProfileData.commonProfile.titleId, showEmpty: false, showLock: false);
			NKCUtil.SetButtonClickDelegate(m_csbtnSimpleUserInfoSlot, OnClick);
			NKMPvpRankTemplet rankTempletByScore = NKCPVPManager.GetRankTempletByScore(NKM_GAME_TYPE.NGT_PVP_RANK, userProfileData.rankPvpData.seasonId, userProfileData.rankPvpData.score);
			string text = "";
			Sprite sprite = null;
			if (rankTempletByScore != null)
			{
				text = rankTempletByScore.LeagueTierIconNumber.ToString();
				NKCUtil.SetLabelText(m_lbTierNumber, rankTempletByScore.LeagueTierIconNumber.ToString());
				sprite = NKCUtil.GetTierIconBig(rankTempletByScore.LeagueTierIcon);
			}
			else
			{
				int seasonID = NKCPVPManager.FindPvPSeasonID(NKM_GAME_TYPE.NGT_PVP_RANK, DateTime.Now);
				text = NKCPVPManager.GetTierNumberByScore(NKM_GAME_TYPE.NGT_PVP_RANK, seasonID, 0).ToString();
				sprite = NKCUtil.GetTierIconBig(LEAGUE_TIER_ICON.LTI_BRONZE);
			}
			NKCUtil.SetImageSprite(m_imgTierIcon, sprite);
			NKCUtil.SetLabelText(m_lbTierNumber, text);
			SetUIData(bHost);
		}
	}

	private void SetUIData(bool bHost)
	{
		NKCUtil.SetGameobjectActive(m_objEmptySlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUserSlot, bValue: true);
		m_UserUID = m_CommonProfile.userUid;
		m_NKCUISlot.SetProfiledata(m_CommonProfile, null);
		NKCUtil.SetLabelText(m_lbName, m_CommonProfile.nickname);
		NKCUtil.SetLabelText(m_lbUID, NKCUtilString.GetFriendCode(m_CommonProfile.friendCode, bOpponent: true));
		NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, m_CommonProfile.level));
		bool flag = false;
		if (m_UserUID == NKCScenManager.CurrentUserData().m_UserUID)
		{
			flag = true;
		}
		NKCUtil.SetGameobjectActive(m_objMySlot, flag);
		if (flag)
		{
			NKCUtil.SetLabelTextColor(m_lbName, NKCUtil.GetColor("#FFDF5D"));
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbName, Color.white);
		}
		SetGuildData();
	}

	private void SetGuildData()
	{
		if (m_objGuild != null)
		{
			GameObject objGuild = m_objGuild;
			NKMGuildSimpleData guildSimpleData = m_GuildSimpleData;
			NKCUtil.SetGameobjectActive(objGuild, guildSimpleData != null && guildSimpleData.guildUid > 0);
			if (m_objGuild.activeSelf)
			{
				m_GuildBadgeUI.SetData(m_GuildSimpleData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, m_GuildSimpleData.guildName);
			}
		}
	}

	private void OnClickDeckEdit()
	{
		if (NKCPrivatePVPRoomMgr.CanEditDeck())
		{
			NKCUIDeckViewer.DeckViewerOption options = new NKCUIDeckViewer.DeckViewerOption
			{
				MenuName = NKCUtilString.GET_STRING_GAUNTLET,
				eDeckviewerMode = NKCUIDeckViewer.DeckViewerMode.PrivatePvPReady,
				dOnSideMenuButtonConfirm = OnTouchDeckSelect,
				dOnBackButton = OnTouchDeckSelect,
				dOnChangeDeckIndex = OnChangeDeckIndex
			};
			if (NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetDeckData(m_curDeckIndex) == null)
			{
				m_curDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_PVP, 0);
			}
			options.DeckIndex = m_curDeckIndex;
			options.SelectLeaderUnitOnOpen = true;
			options.bEnableDefaultBackground = false;
			options.bUpsideMenuHomeButton = false;
			options.upsideMenuShowResourceList = new List<int>();
			options.StageBattleStrID = string.Empty;
			NKCUIDeckViewer.Instance.Open(options);
			NKCPacketSender.Send_NKMPacket_PRIVATE_PVP_STATE_REQ(LobbyPlayerState.Setting);
		}
	}

	private void OnTouchDeckSelect(NKMDeckIndex deckIndex, long supportUserUID = 0L)
	{
		if (m_curDeckIndex != deckIndex)
		{
			NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_SYNC_DECK_INDEX_REQ(deckIndex);
		}
		m_curDeckIndex = deckIndex;
		NKCUIDeckViewer.Instance.Close();
		NKCUtil.SetGameobjectActive(m_objDeckEditFx, bValue: false);
	}

	private void OnTouchDeckSelect()
	{
		OnTouchDeckSelect(m_curDeckIndex, 0L);
	}

	private void OnChangeDeckIndex(NKMDeckIndex deckIndex)
	{
		m_curDeckIndex = deckIndex;
		NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_SYNC_DECK_INDEX_REQ(deckIndex);
	}
}
