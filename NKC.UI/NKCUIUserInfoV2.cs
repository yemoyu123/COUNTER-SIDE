using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Community;
using ClientPacket.User;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUserInfoV2 : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_user_info";

	private const string UI_ASSET_NAME = "NKM_UI_USER_INFO_V2";

	private static NKCUIUserInfoV2 m_Instance;

	private List<int> ResourceList = new List<int>();

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public NKCUISlotProfile m_slotProfile;

	public TMP_Text m_lbNickName;

	public TMP_Text m_lbLevel;

	public Text m_lbUID;

	public Slider m_sliderExp;

	public Text m_lbExp;

	public InputField m_IFComment;

	public NKCUIComStateButton m_csbtnCommentChange;

	public NKCUISlotTitle m_slotTitle;

	public GameObject m_objBirthday;

	public Text m_lbBirthday;

	public NKCUIComStateButton m_btnChangeBirthday;

	public GameObject m_objBirthdayReddot;

	[Header("\ufffd\ufffd\ufffdҽÿ\ufffd")]
	public GameObject m_objGuild;

	public NKCUIGuildBadge m_BadgeUI;

	public TMP_Text m_lbGuildName;

	public TMP_Text m_lbGuildLevel;

	public GameObject m_objNoGuild;

	[Header("\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public TMP_Text m_lbEterniumCount;

	public TMP_Text m_lbUnitCount;

	public TMP_Text m_lbShipCount;

	public TMP_Text m_lbEquipCount;

	public GameObject m_objOperator;

	public TMP_Text m_lbOperatorCount;

	public TMP_Text m_lbAchievementPoint;

	public TMP_Text m_lbSupportUnitCount;

	[Header("\ufffd\ufffdǥ \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public NKCUISlot m_slotMainUnit;

	public GameObject m_objMainUnitBG;

	public NKCUISlot m_slotSubUnit;

	public GameObject m_objSubUnitBG;

	public NKCUISlot m_slotMainShip;

	public NKCUISlot m_slotBackground;

	public NKCUIComStateButton m_btnChangeLobby;

	public Text m_lbBgmName;

	[Header("\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public Image m_ANIM_SHIP_IMG;

	public NKCUIOperatorDeckSlot m_OperatorSlot;

	public List<NKCDeckViewUnitSlot> m_lstNKCDeckViewUnitSlot;

	public NKCUIComStateButton m_btnProfileDeckChange;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public List<NKCUISlot> m_lstEmblem;

	public List<GameObject> m_lstEmblemEffect;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objAssistUnit;

	public NKCUIUnitSelectListSlotAssist m_AssistUnit;

	[Space]
	public NKCUIComStateButton m_btnProfileUnitChange;

	public NKCUIComStateButton m_btnCopy;

	public NKCUIComStateButton m_btnNicknameChange;

	public NKCUIComStateButton m_btnAssistUnitChagen;

	private int m_EmblemIndexToChange = -1;

	private bool m_bCommentChangeButtonClicked;

	private static string m_sComment = "";

	public static NKCUIUserInfoV2 Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIUserInfoV2>("ab_ui_nkm_ui_user_info", "NKM_UI_USER_INFO_V2", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIUserInfoV2>();
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

	public override List<int> UpsideMenuShowResourceList => ResourceList;

	private NKMUserData UserData => NKCScenManager.CurrentUserData();

	public int WaitingRespondCount { get; set; }

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

	private void Init()
	{
		m_slotMainUnit?.Init();
		m_slotSubUnit?.Init();
		m_slotMainShip?.Init();
		m_slotBackground?.Init();
		m_slotProfile?.Init();
		m_AssistUnit?.Init(OnClickAssistUnitChange);
		NKCUtil.SetButtonClickDelegate(m_csbtnCommentChange, OnClickChangeComment);
		NKCUtil.SetButtonClickDelegate(m_btnChangeLobby, OnTouchChangeLobby);
		NKCUtil.SetButtonClickDelegate(m_btnProfileDeckChange, OpenMainDeckSelectWindow);
		NKCUtil.SetButtonClickDelegate(m_btnProfileUnitChange, OpenProfileImageChange);
		NKCUtil.SetButtonClickDelegate(m_btnCopy, OnClickCopy);
		NKCUtil.SetButtonClickDelegate(m_btnNicknameChange, OnClickChangeNickname);
		NKCUtil.SetButtonClickDelegate(m_btnChangeBirthday, OnClickChangeBirthday);
		NKCUtil.SetButtonClickDelegate(m_btnAssistUnitChagen, OnClickAssistUnitChange);
		if (m_IFComment != null)
		{
			m_IFComment.onValidateInput = NKCFilterManager.FilterEmojiInput;
			m_IFComment.onValueChanged.RemoveAllListeners();
			m_IFComment.onValueChanged.AddListener(OnValueChangedComment);
			m_IFComment.onEndEdit.RemoveAllListeners();
			m_IFComment.onEndEdit.AddListener(OnDoneComment);
		}
		m_OperatorSlot?.Init(delegate
		{
			OpenMainDeckSelectWindow();
		});
		if (m_lstNKCDeckViewUnitSlot != null)
		{
			int count = m_lstNKCDeckViewUnitSlot.Count;
			for (int num = 0; num < count; num++)
			{
				if (!(m_lstNKCDeckViewUnitSlot[num] == null))
				{
					m_lstNKCDeckViewUnitSlot[num].Init(num, bEnableDrag: false);
					NKCUtil.SetButtonClickDelegate(m_lstNKCDeckViewUnitSlot[num].m_NKCUIComButton, OpenMainDeckSelectWindow);
				}
			}
		}
		NKCUtil.SetButtonClickDelegate(m_ANIM_SHIP_IMG?.GetComponent<NKCUIComButton>(), OpenMainDeckSelectWindow);
		if (m_lstEmblem != null)
		{
			int count2 = m_lstEmblem.Count;
			for (int num2 = 0; num2 < count2; num2++)
			{
				m_lstEmblem[num2]?.Init();
			}
		}
		base.gameObject.SetActive(value: false);
	}

	public override void UnHide()
	{
		base.UnHide();
		UpdateMainUnitSlot(UserData);
		UpdateSubUnitSlot(UserData);
		UpdateBackgroundSlot(UserData);
		UpdateBgmName(UserData.backGroundInfo);
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMUserData userData)
	{
		NKMArmyData armyData = userData.m_ArmyData;
		m_EmblemIndexToChange = -1;
		WaitingRespondCount = 0;
		m_slotProfile.SetProfiledata(userData.UserProfileData, null, NKCTacticUpdateUtil.IsMaxTacticLevel(userData.UserProfileData.commonProfile.mainUnitTacticLevel));
		m_slotTitle.SetData(userData.UserProfileData.commonProfile.titleId, showEmpty: true, showLock: false);
		NKCUtil.SetLabelText(m_lbNickName, userData.m_UserNickName);
		NKCUtil.SetLabelText(m_lbLevel, userData.UserLevel.ToString());
		NKCUtil.SetLabelText(m_lbUID, NKCUtilString.GetFriendCode(userData.m_FriendCode));
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
		NKCUtil.SetLabelText(m_lbAchievementPoint, userData.GetMissionAchievePoint().ToString());
		SetGuildData();
		NKCUtil.SetLabelText(m_lbUnitCount, $"{armyData.GetCurrentUnitCount()} / {armyData.m_MaxUnitCount}");
		NKCUtil.SetLabelText(m_lbShipCount, $"{armyData.GetCurrentShipCount()} / {armyData.m_MaxShipCount}");
		NKMInventoryData inventoryData = userData.m_InventoryData;
		NKCUtil.SetLabelText(m_lbEquipCount, $"{inventoryData.GetCountEquipItemTypes()} / {inventoryData.m_MaxItemEqipCount}");
		NKCUtil.SetLabelText(m_lbOperatorCount, $"{armyData.GetCurrentOperatorCount()} / {armyData.m_MaxOperatorCount}");
		NKCUtil.SetGameobjectActive(m_objOperator, !NKCOperatorUtil.IsHide());
		NKCUtil.SetGameobjectActive(m_objAssistUnit, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.USE_SUPPORT_UNIT));
		NKCUtil.SetGameobjectActive(m_btnAssistUnitChagen, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.USE_SUPPORT_UNIT));
		UpdateBirthday(userData);
		UpdateEterniumValue(userData);
		UpdateCommentUI();
		UpdateDeckUI();
		UpdateMainUnitSlot(userData);
		UpdateSubUnitSlot(userData);
		UpdateBackgroundSlot(userData);
		UpdateBgmName(userData.backGroundInfo);
		UpdateEmblemUI();
		DeactivateEmblemEffect();
		UpdateSupportUnitUI();
		UIOpened();
	}

	private void SetGuildData()
	{
		NKCUtil.SetGameobjectActive(m_objGuild, NKCGuildManager.HasGuild());
		NKCUtil.SetGameobjectActive(m_objNoGuild, !NKCGuildManager.HasGuild());
		if (m_objGuild != null && m_objGuild.activeSelf)
		{
			m_BadgeUI.SetData(NKCGuildManager.MyGuildData.badgeId);
			NKCUtil.SetLabelText(m_lbGuildName, NKCGuildManager.MyGuildData.name);
			NKCUtil.SetLabelText(m_lbGuildLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, NKCGuildManager.MyGuildData.guildLevel));
		}
	}

	private void OnEnable()
	{
		DeactivateEmblemEffect();
	}

	private void DeactivateEmblemEffect()
	{
		if (m_lstEmblemEffect != null)
		{
			for (int i = 0; i < m_lstEmblemEffect.Count; i++)
			{
				NKCUtil.SetGameobjectActive(m_lstEmblemEffect[i], bValue: false);
			}
		}
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

	private NKCUISlot.OnClick GetOnClickMethod(int index)
	{
		return index switch
		{
			0 => OnClickChangeEmblem0, 
			1 => OnClickChangeEmblem1, 
			2 => OnClickChangeEmblem2, 
			_ => OnClickChangeEmblem0, 
		};
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
					m_lstNKCDeckViewUnitSlot[i].SetData(nKMUnitData);
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
		NKCUtil.SetImageSprite(m_ANIM_SHIP_IMG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_DECK_VIEW_TEXTURE", "NKM_DECK_VIEW_SHIP_UNKNOWN"));
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

	public void OnRecv(NKMPacket_SET_EMBLEM_ACK cNKMPacket_SET_EMBLEM_ACK)
	{
		if (NKCPopupEmblemList.IsInstanceOpen)
		{
			NKCPopupEmblemList.Instance.Close();
		}
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
			slot.SetUnitData(operatorFromUId, bShowName: false, bShowLevel: false, bEnableLayoutElement: false, OnTouchSlot);
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

	private void UpdateBgmName(NKMBackgroundInfo bgInfo)
	{
		NKCBGMInfoTemplet nKCBGMInfoTemplet = NKMTempletContainer<NKCBGMInfoTemplet>.Find(bgInfo.backgroundBgmId);
		if (nKCBGMInfoTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbBgmName, NKCStringTable.GetString(nKCBGMInfoTemplet.m_BgmNameStringID));
			return;
		}
		NKCBackgroundTemplet nKCBackgroundTemplet = NKCBackgroundTemplet.Find(bgInfo.backgroundItemId);
		string text = "";
		if (nKCBackgroundTemplet != null)
		{
			foreach (NKCBGMInfoTemplet value in NKMTempletContainer<NKCBGMInfoTemplet>.Values)
			{
				if (string.Equals(value.m_BgmAssetID, nKCBackgroundTemplet.m_Background_Music))
				{
					text = NKCStringTable.GetString(value.m_BgmNameStringID);
					break;
				}
			}
		}
		NKCUtil.SetLabelText(m_lbBgmName, (!string.IsNullOrEmpty(text)) ? text : NKCUtilString.GET_STRING_JUKEBOX_MUSIC_DEFAULT);
	}

	public void UpdateMainCharUI()
	{
		NKMUserProfileData nKMUserProfileData = NKCScenManager.CurrentUserData()?.UserProfileData;
		if (nKMUserProfileData != null)
		{
			m_slotProfile?.SetProfiledata(nKMUserProfileData, null, NKCTacticUpdateUtil.IsMaxTacticLevel(nKMUserProfileData.commonProfile.mainUnitTacticLevel));
		}
		if (NKCPopupEmblemList.IsInstanceOpen)
		{
			NKCPopupEmblemList.Instance.Close();
		}
	}

	public void UpdateTitle()
	{
		NKMUserProfileData nKMUserProfileData = NKCScenManager.CurrentUserData()?.UserProfileData;
		if (nKMUserProfileData != null)
		{
			m_slotTitle.SetData(nKMUserProfileData.commonProfile.titleId, showEmpty: true, showLock: false);
		}
		if (NKCPopupEmblemList.IsInstanceOpen)
		{
			NKCPopupEmblemList.Instance.Close();
		}
	}

	public void RefreshNickname()
	{
		NKMUserData nKMUserData = NKCScenManager.GetScenManager()?.GetMyUserData();
		if (nKMUserData != null)
		{
			NKCUtil.SetLabelText(m_lbNickName, nKMUserData.m_UserNickName);
		}
	}

	private void OnTouchSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		OnTouchChangeLobby();
	}

	private void OnTouchChangeLobby()
	{
		NKCUIChangeLobby.Instance.Open(UserData);
	}

	public void UpdateBirthday(NKMUserData userData)
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.BIRTHDAY))
		{
			NKCUtil.SetGameobjectActive(m_objBirthday, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objBirthday, bValue: true);
		if (userData.m_BirthDayData == null)
		{
			NKCUtil.SetLabelText(m_lbBirthday, "-");
			NKCUtil.SetGameobjectActive(m_btnChangeBirthday, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBirthdayReddot, bValue: true);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbBirthday, GetBirthdayString(userData.m_BirthDayData));
			NKCUtil.SetGameobjectActive(m_btnChangeBirthday, bValue: false);
			NKCUtil.SetGameobjectActive(m_objBirthdayReddot, bValue: false);
		}
	}

	private string GetBirthdayString(NKMUserBirthDayData birthdayData)
	{
		if (birthdayData == null)
		{
			return "-";
		}
		string monthString = NKCUtilString.GetMonthString(birthdayData.BirthDay.Month);
		return string.Format(NKCUtilString.GET_STRING_PROFILE_BIRTHDAY_INFO, monthString, birthdayData.BirthDay.Day);
	}

	private void UpdateEterniumValue(NKMUserData userData)
	{
		NKCUtil.SetLabelText(m_lbEterniumCount, $"{userData.GetEternium()} / {userData.GetEterniumCap()}");
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

	private void OnClickChangeComment()
	{
		if (m_bCommentChangeButtonClicked)
		{
			m_bCommentChangeButtonClicked = false;
			return;
		}
		m_IFComment.Select();
		m_IFComment.ActivateInputField();
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
				m_bCommentChangeButtonClicked = RectTransformUtility.RectangleContainsScreenPoint(m_csbtnCommentChange.GetComponent<RectTransform>(), Input.mousePosition, NKCUIManager.FrontCanvas.worldCamera);
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

	private void OpenMainDeckSelectWindow()
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

	private void OnClickMainDeckSelect(NKMDeckIndex selectedDeckIndex, long supportUserUID = 0L)
	{
		NKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ nKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ = new NKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ();
		nKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ.deckIndex = selectedDeckIndex;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_FRIEND_PROFILE_MODIFY_DECK_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void OpenProfileImageChange()
	{
		NKCPopupEmblemList.Instance.OpenProfileEdit(SendMainUnitChangeREQ);
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

	private void OnClickCopy()
	{
		NKMUserProfileData nKMUserProfileData = NKCScenManager.CurrentUserData()?.UserProfileData;
		if (nKMUserProfileData != null)
		{
			TextEditor textEditor = new TextEditor();
			textEditor.text = nKMUserProfileData.commonProfile.friendCode.ToString();
			textEditor.OnFocus();
			textEditor.Copy();
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

	private void OnClickChangeBirthday()
	{
		if (NKCScenManager.CurrentUserData().m_BirthDayData == null)
		{
			NKCPopupChangeBirthday.Instance.Open();
		}
		else
		{
			NKCPopupChangeBirthday.Instance.Open(NKCScenManager.CurrentUserData().m_BirthDayData.BirthDay.Month, NKCScenManager.CurrentUserData().m_BirthDayData.BirthDay.Day);
		}
	}

	private void OnClickAssistUnitChange()
	{
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL);
		options.bDescending = true;
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.lstSortOption = new List<NKCUnitSortSystem.eSortOption> { NKCUnitSortSystem.eSortOption.Rarity_High };
		options.bPushBackUnselectable = false;
		options.bShowRemoveSlot = true;
		options.bShowHideDeckedUnitMenu = false;
		options.bHideDeckedUnit = false;
		options.bCanSelectUnitInMission = true;
		options.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_TARGET_TO_SELECT;
		options.strUpsideMenuName = "Assist Unit Change";
		options.m_bUseFavorite = true;
		options.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		options.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		options.setShipFilterCategory = NKCUnitSortSystem.setDefaultShipFilterCategory;
		options.setShipSortCategory = NKCUnitSortSystem.setDefaultShipSortCategory;
		options.setOperatorFilterCategory = NKCPopupFilterOperator.MakeDefaultFilterCategory(NKCPopupFilterOperator.FILTER_OPEN_TYPE.NORMAL);
		options.setOperatorSortCategory = NKCOperatorSortSystem.setDefaultOperatorSortCategory;
		options.eUpsideMenuMode = NKCUIUpsideMenu.eMode.BackButtonOnly;
		options.bMultipleSelect = false;
		options.bHideUnitMissionStatus = true;
		options.bTouchHoldEventToCollection = true;
		NKCUIUnitSelectList.Instance.Open(options, ChangeUnit);
	}

	private void ChangeUnit(List<long> listUnitUID)
	{
		if (listUnitUID.Count == 1)
		{
			NKCPacketSender.Send_NKMPacket_SET_MY_SUPPORT_UNIT_REQ(listUnitUID[0]);
		}
	}

	public void OnRecv(NKMPacket_SET_MY_SUPPORT_UNIT_ACK sPacket)
	{
		if (sPacket.supportUnitData != null)
		{
			UpdateSupportUnitUI();
		}
	}

	private void UpdateSupportUnitUI()
	{
		NKMSupportUnitData supportUnitData = NKCScenManager.CurrentUserData().SupportUnitData;
		if (supportUnitData == null)
		{
			m_AssistUnit.SetData(null, 0L);
			NKCUtil.SetLabelText(m_lbSupportUnitCount, "0");
			return;
		}
		NKMUnitData unitFromUID = NKCScenManager.CurrentArmyData().GetUnitFromUID(supportUnitData.asyncUnitEquip.asyncUnit.unitUid);
		if (unitFromUID == null)
		{
			m_AssistUnit.SetData(null, 0L);
			NKCUtil.SetLabelText(m_lbSupportUnitCount, "0");
			return;
		}
		NKMAsyncUnitData asyncUnit = supportUnitData.asyncUnitEquip.asyncUnit;
		NKCUtil.MakeDummyUnit(asyncUnit.unitId, asyncUnit.unitLevel, (short)asyncUnit.limitBreakLevel, asyncUnit.tacticLevel, asyncUnit.reactorLevel).m_SkinID = asyncUnit.skinId;
		List<NKMEquipItemData> equips = supportUnitData.asyncUnitEquip.equips;
		NKMEquipItemData weapon = null;
		NKMEquipItemData defence = null;
		NKMEquipItemData nKMEquipItemData = null;
		NKMEquipItemData accessory = null;
		foreach (NKMEquipItemData item in equips)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(item.m_ItemEquipID);
			if (equipTemplet == null)
			{
				continue;
			}
			switch (equipTemplet.m_ItemEquipPosition)
			{
			case ITEM_EQUIP_POSITION.IEP_WEAPON:
				weapon = item;
				break;
			case ITEM_EQUIP_POSITION.IEP_DEFENCE:
				defence = item;
				break;
			case ITEM_EQUIP_POSITION.IEP_ACC:
			case ITEM_EQUIP_POSITION.IEP_ACC2:
				if (nKMEquipItemData == null)
				{
					nKMEquipItemData = item;
				}
				else
				{
					accessory = item;
				}
				break;
			}
		}
		NKMEquipmentSet equipSet = new NKMEquipmentSet(weapon, defence, nKMEquipItemData, accessory);
		m_AssistUnit.SetData(unitFromUID, equipSet, supportUnitData.userUid, OnClickSlot);
		NKCUtil.SetLabelText(m_lbSupportUnitCount, supportUnitData.usedCount.ToString());
	}

	private void OnClickSlot(long userUID)
	{
		OnClickAssistUnitChange();
	}
}
