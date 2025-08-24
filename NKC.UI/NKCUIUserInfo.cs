using ClientPacket.User;
using NKC.UI.Guild;
using NKC.UI.NPC;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUserInfo : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_user_info";

	private const string UI_ASSET_NAME = "NKM_UI_USER_INFO";

	private static NKCUIUserInfo m_Instance;

	[Header("메인 정보")]
	public NKCUISlotProfile m_slotProfile;

	public Text m_lbNickName;

	public Text m_lbLevel;

	public Text m_lbUID;

	public Text m_lbJoinDate;

	public Slider m_sliderExp;

	public Text m_lbExp;

	public NKCUIComStateButton m_csbtnEditProfile;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_BadgeUI;

	public Text m_lbGuildName;

	[Header("PVP Score")]
	public GameObject m_objRecordOn;

	public Text m_lbRank;

	public NKCUILeagueTier m_NKCUILeagueTier;

	public Image m_imgTierBG;

	public Text m_lbRankStanding;

	public Text m_lbPVPScore;

	public GameObject m_objRecordOff;

	public Text m_lbPVPBestTier;

	public Text m_lbPVPBestScore;

	[Header("기업 정보")]
	public Text m_lbEterniumCount;

	public Text m_lbUnitCount;

	public Text m_lbShipCount;

	public Text m_lbEquipCount;

	public GameObject m_objOperator;

	public Text m_lbOperatorCount;

	public Text m_lbVictoryPoint;

	public Text m_lbInformationPoint;

	public Text m_lbAchievementPoint;

	[Header("대표 유닛 슬롯")]
	public NKCUISlot m_slotMainUnit;

	public GameObject m_objMainUnitBG;

	public NKCUISlot m_slotSubUnit;

	public GameObject m_objSubUnitBG;

	public NKCUISlot m_slotMainShip;

	public NKCUISlot m_slotBackground;

	public NKCUIComStateButton m_btnChangeLobby;

	[Header("NPC")]
	public GameObject m_objNPC_KimHana_TouchArea;

	private NKCUINPCManagerKimHaNa m_UINPCKimHana;

	private bool m_bChangeCommentClicked;

	private static string m_sComment = "";

	public static NKCUIUserInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIUserInfo>("ab_ui_nkm_ui_user_info", "NKM_UI_USER_INFO", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIUserInfo>();
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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_PROFILE;

	public override eTransitionEffectType eTransitionEffect => eTransitionEffectType.FadeInOut;

	private NKMUserData UserData => NKCScenManager.CurrentUserData();

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public static void SetComment(string comment)
	{
		m_sComment = comment;
	}

	public override void OnCloseInstance()
	{
		base.OnCloseInstance();
		m_UINPCKimHana = null;
	}

	public override void Hide()
	{
		NKCSoundManager.StopAllSound(SOUND_TRACK.VOICE);
		base.Hide();
	}

	public override void UnHide()
	{
		base.UnHide();
		UpdateMainUnitSlot(UserData);
		UpdateSubUnitSlot(UserData);
		UpdateBackgroundSlot(UserData);
	}

	public override void CloseInternal()
	{
		NKCSoundManager.StopAllSound(SOUND_TRACK.VOICE);
		base.gameObject.SetActive(value: false);
	}

	public void Init()
	{
		m_slotMainUnit.Init();
		m_slotSubUnit.Init();
		m_slotMainShip.Init();
		m_slotBackground.Init();
		if (m_UINPCKimHana == null)
		{
			m_UINPCKimHana = m_objNPC_KimHana_TouchArea.GetComponent<NKCUINPCManagerKimHaNa>();
			m_UINPCKimHana.Init();
		}
		m_slotProfile.Init();
		NKCUtil.SetButtonClickDelegate(m_csbtnEditProfile, OnBtnEditProfile);
		m_btnChangeLobby?.PointerClick.RemoveAllListeners();
		m_btnChangeLobby?.PointerClick.AddListener(OnTouchChangeLobby);
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMUserData userData)
	{
		NKMArmyData armyData = userData.m_ArmyData;
		m_UINPCKimHana.Init();
		m_UINPCKimHana.PlayAni(NPC_ACTION_TYPE.ENTER_PROFILE);
		m_slotProfile.SetProfiledata(userData.UserProfileData, null, NKCTacticUpdateUtil.IsMaxTacticLevel(userData.UserProfileData.commonProfile.mainUnitTacticLevel));
		m_lbNickName.text = userData.m_UserNickName;
		m_lbLevel.text = userData.UserLevel.ToString();
		m_lbUID.text = NKCUtilString.GetFriendCode(userData.m_FriendCode);
		m_lbJoinDate.text = userData.m_NKMUserDateData.m_RegisterTime.ToLocalTime().ToString(NKCUtilString.GET_STRING_REGISTERTIME_DATE);
		long currentExp = NKCExpManager.GetCurrentExp(userData);
		long num = NKCExpManager.GetRequiredExp(userData);
		m_lbExp.text = $"{currentExp} / {num}";
		if (num == 0L)
		{
			m_sliderExp.value = 1f;
		}
		else
		{
			m_sliderExp.value = (float)currentExp / (float)num;
		}
		m_lbAchievementPoint.text = userData.GetMissionAchievePoint().ToString();
		SetPVP(userData);
		SetGuildData();
		m_lbUnitCount.text = $"{armyData.GetCurrentUnitCount()} / {armyData.m_MaxUnitCount}";
		m_lbShipCount.text = $"{armyData.GetCurrentShipCount()} / {armyData.m_MaxShipCount}";
		NKMInventoryData inventoryData = userData.m_InventoryData;
		m_lbEquipCount.text = $"{inventoryData.GetCountEquipItemTypes()} / {inventoryData.m_MaxItemEqipCount}";
		m_lbOperatorCount.text = $"{armyData.GetCurrentOperatorCount()} / {armyData.m_MaxOperatorCount}";
		NKCUtil.SetGameobjectActive(m_objOperator, !NKCOperatorUtil.IsHide());
		UpdateEterniumValue(userData);
		UpdateMainUnitSlot(userData);
		UpdateSubUnitSlot(userData);
		UpdateBackgroundSlot(userData);
		UIOpened();
		m_bOpen = true;
	}

	private void SetGuildData()
	{
		if (m_objGuild != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuild, NKCGuildManager.HasGuild());
			if (m_objGuild.activeSelf)
			{
				m_BadgeUI.SetData(NKCGuildManager.MyGuildData.badgeId);
				NKCUtil.SetLabelText(m_lbGuildName, NKCGuildManager.MyGuildData.name);
			}
		}
	}

	private void SetPVP(NKMUserData userData)
	{
		PvpState pvpState;
		NKM_GAME_TYPE gameType;
		if (userData.m_PvpData.Score >= userData.m_AsyncData.Score)
		{
			pvpState = userData.m_PvpData;
			gameType = NKM_GAME_TYPE.NGT_PVP_RANK;
			m_imgTierBG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_USER_INFO_SPRITE", "NKM_UI_USER_INFO_GAUNTLETBG");
		}
		else
		{
			pvpState = userData.m_AsyncData;
			gameType = NKM_GAME_TYPE.NGT_ASYNC_PVP;
			m_imgTierBG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_USER_INFO_SPRITE", "NKM_UI_USER_INFO_GAUNTLETBG_ASYNC");
		}
		int num = NKCPVPManager.FindPvPSeasonID(gameType, NKCSynchronizedTime.GetServerUTCTime());
		NKCUtil.SetGameobjectActive(m_objRecordOff, pvpState.SeasonID != num);
		NKCUtil.SetGameobjectActive(m_objRecordOn, pvpState.SeasonID == num);
		int leagueScore = pvpState.Score;
		if (pvpState.SeasonID != num)
		{
			m_lbRankStanding.text = NKCUtilString.GET_STRING_NO_RANK;
			leagueScore = NKCPVPManager.GetResetScore(pvpState.SeasonID, pvpState.Score, gameType);
			NKMPvpRankTemplet rankTempletByScore = NKCPVPManager.GetRankTempletByScore(gameType, num, leagueScore);
			if (rankTempletByScore != null)
			{
				m_NKCUILeagueTier.SetUI(rankTempletByScore);
				m_lbRank.text = rankTempletByScore.GetLeagueName();
			}
			m_lbPVPBestTier.text = "-";
			m_lbPVPBestScore.text = "-";
		}
		else
		{
			m_lbRankStanding.text = string.Format(NKCUtilString.GET_STRING_TOTAL_RANK_ONE_PARAM, pvpState.Rank);
			NKMPvpRankTemplet rankTempletByTier = NKCPVPManager.GetRankTempletByTier(gameType, num, pvpState.LeagueTierID);
			if (rankTempletByTier != null)
			{
				m_NKCUILeagueTier.SetUI(rankTempletByTier);
				m_lbRank.text = rankTempletByTier.GetLeagueName();
			}
			NKMPvpRankTemplet rankTempletByTier2 = NKCPVPManager.GetRankTempletByTier(gameType, num, pvpState.MaxLeagueTierID);
			if (rankTempletByTier2 != null)
			{
				m_lbPVPBestTier.text = rankTempletByTier2.GetLeagueName();
			}
			m_lbPVPBestScore.text = pvpState.MaxScore.ToString();
		}
		m_lbPVPScore.text = leagueScore.ToString();
	}

	private void UpdateLobbyUnitSlot(int index, NKCUISlot slot, GameObject objUnitBG, NKMUserData userData)
	{
		NKMBackgroundUnitInfo backgroundUnitInfo = userData.GetBackgroundUnitInfo(index);
		if (backgroundUnitInfo == null)
		{
			slot?.SetEmpty(OnTouchSlot);
			return;
		}
		long num = backgroundUnitInfo?.unitUid ?? 0;
		switch (backgroundUnitInfo.unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
		case NKM_UNIT_TYPE.NUT_SHIP:
		{
			NKMUnitData unitOrShipFromUID = userData.m_ArmyData.GetUnitOrShipFromUID(num);
			slot.SetUnitData(unitOrShipFromUID, bShowName: false, bShowLevel: false, bEnableLayoutElement: false, OnTouchSlot);
			slot.SetMaxLevelTacticFX(unitOrShipFromUID);
			slot.SetSeized(unitOrShipFromUID?.IsSeized ?? false);
			break;
		}
		case NKM_UNIT_TYPE.NUT_OPERATOR:
		{
			NKMOperator operatorFromUId = userData.m_ArmyData.GetOperatorFromUId(num);
			if (operatorFromUId != null)
			{
				slot.SetUnitData(operatorFromUId, bShowName: false, bShowLevel: false, bEnableLayoutElement: false, OnTouchSlot);
			}
			break;
		}
		default:
			slot.SetEmpty(OnTouchSlot);
			break;
		}
		NKCUtil.SetGameobjectActive(objUnitBG, backgroundUnitInfo.backImage);
	}

	public void UpdateMainUnitSlot(NKMUserData userData)
	{
		UpdateLobbyUnitSlot(0, m_slotMainUnit, m_objMainUnitBG, userData);
	}

	public void UpdateSubUnitSlot(NKMUserData userData)
	{
		UpdateLobbyUnitSlot(1, m_slotSubUnit, m_objSubUnitBG, userData);
	}

	public void UpdateBackgroundSlot(NKMUserData userData)
	{
		m_slotBackground.SetMiscItemData(userData.BackgroundID, 1L, bShowName: false, bShowCount: false, bEnableLayoutElement: true, OnTouchSlot);
	}

	private void OnTouchSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		OnTouchChangeLobby();
	}

	private void OnBtnEditProfile()
	{
		Close();
		NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_FRIEND_MYPROFILE, "");
	}

	private void OnTouchChangeLobby()
	{
		NKCUIChangeLobby.Instance.Open(UserData);
	}

	public void RefreshNickname()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			NKCUtil.SetLabelText(m_lbNickName, myUserData.m_UserNickName);
		}
	}

	private void UpdateEterniumValue(NKMUserData userData)
	{
		m_lbEterniumCount.text = $"{userData.GetEternium()} / {userData.GetEterniumCap()}";
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (itemData.ItemID == 2)
		{
			NKMUserData userData = NKCScenManager.CurrentUserData();
			UpdateEterniumValue(userData);
		}
	}

	public override void OnUserLevelChanged(NKMUserData userData)
	{
		UpdateEterniumValue(userData);
	}

	public override void OnGuildDataChanged()
	{
		SetGuildData();
	}
}
