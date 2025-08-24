using System.Collections.Generic;
using System.IO;
using System.Text;
using ClientPacket.User;
using Cs.Protocol;
using DG.Tweening;
using NKC.UI.Component;
using NKC.UI.Lobby;
using NKC.UI.Shop;
using NKC.Util;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIChangeLobby : NKCUIBase, IDragHandler, IEventSystemHandler
{
	private enum PinchMode
	{
		ScaleOnly,
		Free
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_user_info";

	private const string UI_ASSET_NAME = "NKM_UI_USER_INFO_LOBBY_CHANGE_RENEWAL";

	private static NKCUIChangeLobby m_Instance;

	public Transform m_trRoot;

	public Transform m_trBGRoot;

	[Header("Button")]
	public NKCUIComDraggableList m_comDraggableList;

	public List<NKCUIComToggle> m_lstTglUnit;

	public NKCUIComStateButton m_btnChangeUnit;

	public NKCUIComStateButton m_btnResetPosition;

	public NKCUIComStateButton m_btnBackground;

	public NKCUIComStateButton m_btnApplyToLobby;

	public NKCUIComStateButton m_btnJukeBox;

	public NKCUIComStateButton m_btnClose;

	[Header("유닛 디테일")]
	public NKCUIComStateButton m_btnDetailOption;

	public NKCUIComToggle m_tglUnitBG;

	public NKCUIComStateButton m_BtnSkinOption;

	public NKCUIComToggle m_tglUnitAnimation;

	public NKCUIComToggle m_tglUnitFlip;

	public NKCUIComStateButton m_btnSkinChange;

	[Header("Unit")]
	public List<NKCUICharacterView> m_lstCvUnit;

	[Header("Music")]
	public Text m_lbMusicTitle;

	public NKCUIComToggle m_ctglMusicContinue;

	[Header("Background")]
	public NKCUIChangeLobbyBackground m_ChangeLobbyBackground;

	[Header("Face")]
	public NKCUIChangeLobbyFace m_LobbyFace;

	[Header("Preset")]
	public List<NKCUIComToggle> m_lstTglPreset;

	public NKCUIComStateButton m_csbtnLoadPreset;

	public NKCUIComStateButton m_csbtnSavePreset;

	[Header("유닛 선택 관련")]
	public Color m_SelectStartColor;

	public float m_SelectScale = 1.1f;

	public float m_SelectTime = 0.4f;

	public Ease m_Ease = Ease.OutCubic;

	[Header("최대/최소 줌")]
	public float MIN_ZOOM_SCALE = 0.1f;

	public float MAX_ZOOM_SCALE = 3f;

	public float m_MouseWheelSensibility = 0.1f;

	[Header("미리보기")]
	public NKCUIComToggle m_tglPreviewOption;

	public NKCUIComStateButton m_csbtnPreview;

	public NKCUIComStateButton m_csbtnExitPreview;

	public GameObject m_objUIRoot;

	public GameObject m_objPreviewRoot;

	[Header("단축키/조작 관련")]
	public float m_fShiftMoveMagnitude = 4f;

	public float m_fCtrlMoveMagnitude = 0.1f;

	public float m_fKeyboardMoveSpeed = 20f;

	public GameObject m_objPCControl;

	private NKMBackgroundInfo m_currentBackgroundInfo;

	private int m_currentPresetIndex = -1;

	private int m_currentUnitIndex;

	private int m_skinOptionCount;

	private List<NKCBackgroundTemplet> m_listBGTemplet = new List<NKCBackgroundTemplet>();

	private List<NKCUISlot> m_listBGSlot = new List<NKCUISlot>();

	private GameObject m_objBG;

	private float m_fAngleLastFrame;

	private bool m_bBGMContinue;

	private const string BG_PRESET_SAVE_KEY = "BACKGROUND_PRESET_{0}_{1}";

	private bool m_bPreviewMode;

	[Header("Pinch")]
	public float m_fPinchAngleThreshold = 10f;

	private float m_fPinchAngleChanged;

	private PinchMode m_ePinchMode;

	public static NKCUIChangeLobby Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIChangeLobby>("ab_ui_nkm_ui_user_info", "NKM_UI_USER_INFO_LOBBY_CHANGE_RENEWAL", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIChangeLobby>();
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

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override string MenuName => "change lobby";

	private bool IsOpenSelectBackground
	{
		get
		{
			if (m_ChangeLobbyBackground != null)
			{
				return m_ChangeLobbyBackground.gameObject.activeInHierarchy;
			}
			return false;
		}
	}

	private bool IsPreviewMode => m_bPreviewMode;

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

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private NKCUICharacterView GetCharView(int index)
	{
		if (m_lstCvUnit == null)
		{
			return null;
		}
		if (index < 0)
		{
			return null;
		}
		if (index >= m_lstCvUnit.Count)
		{
			return null;
		}
		return m_lstCvUnit[index];
	}

	public override void CloseInternal()
	{
		CleanUp();
		if (m_trRoot != null)
		{
			m_trRoot.SetParent(base.transform);
			m_trRoot.localPosition = Vector3.zero;
		}
		NKCUtil.SetGameobjectActive(m_trRoot, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		if (IsPreviewMode)
		{
			SetPreview(value: false);
			return;
		}
		if (IsOpenSelectBackground)
		{
			CloseSelectBackground();
			return;
		}
		if (m_LobbyFace.IsOpen)
		{
			m_LobbyFace.Close();
			return;
		}
		base.OnBackButton();
		NKCSoundManager.PlayScenMusic();
	}

	public override void UnHide()
	{
		UpdateBackgroundMusic(m_currentBackgroundInfo);
		UpdateUnitIllust(m_currentUnitIndex, m_currentBackgroundInfo);
		base.UnHide();
	}

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_btnChangeUnit, OnChangeUnit);
		NKCUtil.SetToggleValueChangedDelegate(m_tglUnitBG, OnTglUnitBG);
		NKCUtil.SetButtonClickDelegate(m_BtnSkinOption, OnSkinOption);
		NKCUtil.SetButtonClickDelegate(m_btnSkinChange, OnSkinChange);
		NKCUtil.SetToggleValueChangedDelegate(m_tglUnitFlip, Flip);
		NKCUtil.SetToggleValueChangedDelegate(m_tglUnitAnimation, StopAnim);
		for (int i = 0; i < m_lstTglUnit.Count; i++)
		{
			if (!(m_lstTglUnit[i] == null))
			{
				m_lstTglUnit[i].m_DataInt = i;
				m_lstTglUnit[i].OnValueChangedWithData = OnSelectUnit;
			}
		}
		for (int j = 0; j < m_lstTglPreset.Count; j++)
		{
			if (!(m_lstTglPreset[j] == null))
			{
				m_lstTglPreset[j].m_DataInt = j;
				m_lstTglPreset[j].OnValueChangedWithData = OnTglPreset;
			}
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnLoadPreset, OnBtnLoadPreset);
		NKCUtil.SetButtonClickDelegate(m_csbtnSavePreset, OnBtnSavePreset);
		NKCUtil.SetButtonClickDelegate(m_btnBackground, OpenSelectBackground);
		NKCUtil.SetButtonClickDelegate(m_btnClose, base.Close);
		NKCUtil.SetButtonClickDelegate(m_btnApplyToLobby, OnConfirm);
		NKCUtil.SetButtonClickDelegate(m_btnJukeBox, OnOpenJukeBox);
		NKCUtil.SetButtonClickDelegate(m_btnDetailOption, ToggleDetailMenu);
		NKCUtil.SetButtonClickDelegate(m_btnResetPosition, ResetPosition);
		if (m_tglPreviewOption != null)
		{
			m_tglPreviewOption.Select(bSelect: true, bForce: true);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnPreview, OnBtnPreview);
		NKCUtil.SetButtonClickDelegate(m_csbtnExitPreview, OnBtnPreviewExit);
		NKCUtil.SetToggleValueChangedDelegate(m_ctglMusicContinue, OnClickBGMContinue);
		for (int k = 0; k < m_lstCvUnit.Count; k++)
		{
			m_lstCvUnit[k]?.Init(OnDragUnit, OnTouchUnit);
		}
		if (m_LobbyFace != null)
		{
			m_LobbyFace.Init(OnSelectFace);
		}
		if (m_ChangeLobbyBackground != null)
		{
			m_ChangeLobbyBackground.Init(OnChangeBackground);
		}
		if (m_comDraggableList != null)
		{
			m_comDraggableList.dOnSlotSwapped = OnSwap;
		}
		bool flag = NKCDefineManager.DEFINE_ANDROID() || NKCDefineManager.DEFINE_IOS();
		NKCUtil.SetGameobjectActive(m_objPCControl, !flag);
	}

	public void Open(NKMUserData userData)
	{
		NKMInventoryData inventoryData = userData.m_InventoryData;
		foreach (NKCBackgroundTemplet value in NKMTempletContainer<NKCBackgroundTemplet>.Values)
		{
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(value.m_ItemMiscID);
			if (nKMItemMiscTemplet == null)
			{
				continue;
			}
			NKM_ITEM_MISC_TYPE itemMiscType = nKMItemMiscTemplet.m_ItemMiscType;
			if (itemMiscType != NKM_ITEM_MISC_TYPE.IMT_BACKGROUND)
			{
				if (itemMiscType != NKM_ITEM_MISC_TYPE.IMT_INTERIOR)
				{
					continue;
				}
				NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMItemMiscTemplet.FindInterior(value.m_ItemMiscID);
				if (nKMOfficeInteriorTemplet == null || nKMOfficeInteriorTemplet.Target != InteriorTarget.Background || userData.OfficeData.GetInteriorCount(value.m_ItemMiscID) <= 0)
				{
					continue;
				}
			}
			else if (value.m_ItemMiscID != 9001 && inventoryData.GetItemMisc(value.m_ItemMiscID) == null)
			{
				continue;
			}
			m_listBGTemplet.Add(value);
		}
		SetBackgroundInfo(MakeNKMBackgroundInfo(userData.backGroundInfo), bMusic: false);
		SelectUnit(0, bPlaySelectAnim: false);
		UpdatePresetButtons();
		m_currentPresetIndex = -1;
		for (int i = 0; i < m_lstTglPreset.Count; i++)
		{
			m_lstTglPreset[i]?.Select(bSelect: false, bForce: true);
		}
		m_csbtnSavePreset?.Lock();
		m_csbtnLoadPreset?.Lock();
		UIOpened();
		if (m_comDraggableList != null)
		{
			m_comDraggableList.ResetPosition();
		}
		if (m_trRoot != null)
		{
			m_trRoot.SetParent(NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIMidCanvas));
			m_trRoot.localPosition = Vector3.zero;
		}
		NKCUtil.SetGameobjectActive(m_trRoot, bValue: true);
		ResetPinch();
		SetPreview(value: false);
	}

	private NKMBackgroundInfo MakeNKMBackgroundInfo(NKMBackgroundInfo source)
	{
		NKMBackgroundInfo nKMBackgroundInfo = new NKMBackgroundInfo();
		nKMBackgroundInfo.DeepCopyFrom(source);
		if (nKMBackgroundInfo.unitInfoList != null)
		{
			while (nKMBackgroundInfo.unitInfoList.Count < 8)
			{
				NKMBackgroundUnitInfo nKMBackgroundUnitInfo = new NKMBackgroundUnitInfo();
				nKMBackgroundUnitInfo.unitUid = 0L;
				nKMBackgroundUnitInfo.unitFace = 0;
				nKMBackgroundUnitInfo.unitPosX = 0f;
				nKMBackgroundUnitInfo.unitPosY = 0f;
				nKMBackgroundUnitInfo.unitSize = 1f;
				nKMBackgroundUnitInfo.backImage = true;
				nKMBackgroundUnitInfo.skinOption = 0;
				nKMBackgroundUnitInfo.rotation = 0f;
				nKMBackgroundUnitInfo.flip = false;
				nKMBackgroundUnitInfo.animTime = -1f;
				nKMBackgroundUnitInfo.unitType = NKM_UNIT_TYPE.NUT_NORMAL;
				nKMBackgroundInfo.unitInfoList.Add(nKMBackgroundUnitInfo);
			}
		}
		return nKMBackgroundInfo;
	}

	private void UpdateUnitList()
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		for (int i = 0; i < m_lstTglUnit.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstTglUnit[i], i < 8);
			NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(i, m_currentBackgroundInfo);
			string titleText = "-";
			if (bGUnitInfo != null && bGUnitInfo.unitUid != 0L)
			{
				switch (bGUnitInfo.unitType)
				{
				case NKM_UNIT_TYPE.NUT_OPERATOR:
				{
					NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(armyData.GetOperatorFromUId(bGUnitInfo.unitUid));
					if (unitTempletBase2 != null)
					{
						titleText = NKCStringTable.GetString(unitTempletBase2.Name);
					}
					break;
				}
				case NKM_UNIT_TYPE.NUT_NORMAL:
				case NKM_UNIT_TYPE.NUT_SHIP:
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(armyData.GetUnitOrShipFromUID(bGUnitInfo.unitUid));
					if (unitTempletBase != null)
					{
						titleText = NKCStringTable.GetString(unitTempletBase.Name);
					}
					break;
				}
				}
			}
			m_lstTglUnit[i].SetTitleText(titleText);
			m_lstTglUnit[i].Select(m_currentUnitIndex == i, bForce: true, bImmediate: true);
		}
	}

	private void SelectUnit(int index, bool bPlaySelectAnim)
	{
		if (index < 0)
		{
			index = 0;
		}
		m_currentUnitIndex = index;
		if (m_currentUnitIndex < m_lstTglUnit.Count)
		{
			m_lstTglUnit[m_currentUnitIndex]?.Select(bSelect: true, bForce: true, bImmediate: true);
		}
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(index, m_currentBackgroundInfo);
		NKCUICharacterView charView = GetCharView(index);
		if (bPlaySelectAnim && charView != null)
		{
			DOTween.CompleteAll();
			charView.SetColor(m_SelectStartColor);
			float scale = ((bGUnitInfo.unitSize > 1f) ? (bGUnitInfo.unitSize * m_SelectScale) : (bGUnitInfo.unitSize + (m_SelectScale - 1f)));
			charView.SetScale(scale);
			DOTween.To(charView.GetColor, delegate(Color col)
			{
				charView.SetColor(col);
			}, Color.white, m_SelectTime).SetEase(m_Ease);
			DOTween.To(charView.GetScale, delegate(float val)
			{
				charView.SetScale(val);
			}, bGUnitInfo.unitSize, m_SelectTime).SetEase(m_Ease);
		}
		RefreshDetailButtons();
		RefreshEmotionMenu();
	}

	private void RefreshDetailButtons()
	{
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		NKCUICharacterView charView = GetCharView(m_currentUnitIndex);
		m_btnDetailOption.SetLock(value: false);
		m_tglUnitBG.Select(GetUnitSkinBGEnabled(m_currentUnitIndex), bForce: true);
		m_tglUnitFlip.Select(bGUnitInfo.flip, bForce: true);
		m_tglUnitAnimation.Select(IsStopAnim(bGUnitInfo), bForce: true);
		m_skinOptionCount = ((bGUnitInfo.unitUid > 0 && charView.GetUnitIllust() != null) ? charView.GetUnitIllust().GetSkinOptionCount() : 0);
		if (m_BtnSkinOption != null)
		{
			m_BtnSkinOption.SetLock(m_skinOptionCount <= 0);
		}
		if (bGUnitInfo.unitType == NKM_UNIT_TYPE.NUT_NORMAL && bGUnitInfo.unitUid > 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(NKCScenManager.CurrentArmyData().GetUnitOrTrophyFromUID(bGUnitInfo.unitUid));
			m_btnSkinChange.SetLock(unitTempletBase?.IsTrophy ?? true);
		}
		else
		{
			m_btnSkinChange.SetLock(value: true);
		}
	}

	private void CleanUp()
	{
		DOTween.CompleteAll();
		for (int i = 0; i < m_lstCvUnit.Count; i++)
		{
			m_lstCvUnit[i]?.CleanUp();
		}
		m_listBGTemplet.Clear();
		if (m_objBG != null)
		{
			Object.Destroy(m_objBG);
		}
		ResetPinch();
	}

	public override void OnCloseInstance()
	{
		if (m_trRoot != null)
		{
			Object.Destroy(m_trRoot.gameObject);
		}
		base.OnCloseInstance();
	}

	private void OnConfirm()
	{
		NKCPacketSender.Send_NKMPacket_BACKGROUND_CHANGE_REQ(m_currentBackgroundInfo);
		NKCScenManager.CurrentUserData().BackgroundBGMContinue = m_bBGMContinue;
	}

	private void OnOpenJukeBox()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_JUKEBOX_CONTENTS_UNLOCK);
			return;
		}
		int selectedMusicID = ((m_currentBackgroundInfo != null) ? m_currentBackgroundInfo.backgroundBgmId : 0);
		NKCUIJukeBox.Instance.Open(bLobbyMusicSelectMode: true, selectedMusicID, OnConfirmChangeBGMInfo);
	}

	public void OnConfirmChangeBGMInfo(int bgmID)
	{
		if (NKCUIJukeBox.IsInstanceOpen)
		{
			NKCUIJukeBox.Instance.Close();
		}
		m_currentBackgroundInfo.backgroundBgmId = bgmID;
		UpdateBackgroundMusic(m_currentBackgroundInfo);
	}

	private void OnChangeUnit()
	{
		OpenChangeUnit(m_currentUnitIndex);
	}

	private void OnSelectUnit(bool value, int index)
	{
		if (value)
		{
			SelectUnit(index, bPlaySelectAnim: true);
		}
	}

	private void OpenChangeUnit(int index)
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
		options.m_SortOptions.bUseLobbyState = false;
		options.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_TARGET_TO_SELECT;
		options.strUpsideMenuName = NKCUtilString.GET_STRING_LOBBY_BG_SELECT_CAPTAIN;
		options.m_SortOptions.AdditionalUnitStateFunc = GiveLobbyState;
		options.bShowUnitShipChangeMenu = true;
		options.m_bUseFavorite = true;
		options.m_SortOptions.bIgnoreCityState = true;
		options.m_SortOptions.bIgnoreWorldMapLeader = true;
		options.m_SortOptions.bIgnoreMissionState = true;
		options.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		options.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		options.setShipFilterCategory = NKCUnitSortSystem.setDefaultShipFilterCategory;
		options.setShipSortCategory = NKCUnitSortSystem.setDefaultShipSortCategory;
		options.setOperatorFilterCategory = NKCPopupFilterOperator.MakeDefaultFilterCategory(NKCPopupFilterOperator.FILTER_OPEN_TYPE.NORMAL);
		options.setOperatorSortCategory = NKCOperatorSortSystem.setDefaultOperatorSortCategory;
		options.eUpsideMenuMode = NKCUIUpsideMenu.eMode.BackButtonOnly;
		NKCUIUnitSelectList.Instance.Open(options, ChangeUnit);
	}

	private NKCUnitSortSystem.eUnitState GiveLobbyState(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return NKCUnitSortSystem.eUnitState.NONE;
		}
		for (int i = 0; i < 8; i++)
		{
			NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(i, m_currentBackgroundInfo);
			if (bGUnitInfo != null && bGUnitInfo.unitUid == unitData.m_UnitUID)
			{
				return NKCUnitSortSystem.eUnitState.LOBBY_UNIT;
			}
		}
		return NKCUnitSortSystem.eUnitState.NONE;
	}

	private void OnSwap(int oldIndex, int newIndex)
	{
		if (oldIndex >= 0 && newIndex >= 0 && m_currentBackgroundInfo != null && m_currentBackgroundInfo.unitInfoList != null && m_currentBackgroundInfo.unitInfoList.Count > oldIndex && m_currentBackgroundInfo.unitInfoList.Count > newIndex)
		{
			NKMBackgroundUnitInfo nKMBackgroundUnitInfo = m_currentBackgroundInfo.unitInfoList[oldIndex];
			NKMBackgroundUnitInfo nKMBackgroundUnitInfo2 = m_currentBackgroundInfo.unitInfoList[newIndex];
			float scaleRatio;
			Vector2 vector = CalculateNewPosition(oldIndex, newIndex, nKMBackgroundUnitInfo.unitPosX, nKMBackgroundUnitInfo.unitPosY, out scaleRatio);
			float scaleRatio2;
			Vector2 vector2 = CalculateNewPosition(newIndex, oldIndex, nKMBackgroundUnitInfo2.unitPosX, nKMBackgroundUnitInfo2.unitPosY, out scaleRatio2);
			nKMBackgroundUnitInfo.unitPosX = vector.x;
			nKMBackgroundUnitInfo.unitPosY = vector.y;
			nKMBackgroundUnitInfo.unitSize *= scaleRatio;
			nKMBackgroundUnitInfo2.unitPosX = vector2.x;
			nKMBackgroundUnitInfo2.unitPosY = vector2.y;
			nKMBackgroundUnitInfo2.unitSize *= scaleRatio2;
			m_currentBackgroundInfo.unitInfoList[oldIndex] = nKMBackgroundUnitInfo2;
			m_currentBackgroundInfo.unitInfoList[newIndex] = nKMBackgroundUnitInfo;
			UpdateUnitIllust(oldIndex, m_currentBackgroundInfo);
			UpdateUnitIllust(newIndex, m_currentBackgroundInfo);
			UpdateUnitList();
			RefreshDetailButtons();
			RefreshEmotionMenu();
		}
	}

	private Vector2 CalculateNewPosition(int oldIndex, int newIndex, float posX, float posY, out float scaleRatio)
	{
		scaleRatio = 1f;
		Vector2 vector = new Vector2(posX, posY);
		if (oldIndex == newIndex)
		{
			return vector;
		}
		NKCUICharacterView charView = GetCharView(oldIndex);
		NKCUICharacterView charView2 = GetCharView(newIndex);
		if (charView == null || charView2 == null)
		{
			return vector;
		}
		Vector3 vector2 = charView.OffsetToWorldPos(vector);
		Vector3 vector3 = charView2.OffsetToWorldPos(Vector2.zero);
		float z = NKCCamera.GetCamera().transform.position.z;
		scaleRatio = (z - vector3.z) / (z - vector2.z);
		Vector3 vector4 = vector2 * scaleRatio;
		return charView2.WorldPosToOffset(vector4);
	}

	private void ChangeUnit(List<long> listUnitUID)
	{
		if (listUnitUID.Count != 1)
		{
			Debug.LogError("Fatal Error : UnitSelectList returned wrong list");
			return;
		}
		long num = listUnitUID[0];
		NKCUIUnitSelectList.CheckInstanceAndClose();
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		if (bGUnitInfo != null && bGUnitInfo.unitUid == num)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < 8; i++)
		{
			if (i != m_currentUnitIndex)
			{
				NKMBackgroundUnitInfo bGUnitInfo2 = GetBGUnitInfo(i, m_currentBackgroundInfo);
				if (bGUnitInfo2 != null && bGUnitInfo2.unitUid != 0L && bGUnitInfo2.unitUid == num)
				{
					flag = true;
					bGUnitInfo2.unitUid = bGUnitInfo.unitUid;
					bGUnitInfo2.unitType = bGUnitInfo.unitType;
					bGUnitInfo2.backImage = bGUnitInfo.backImage;
					bGUnitInfo2.skinOption = bGUnitInfo.skinOption;
					bGUnitInfo2.unitFace = bGUnitInfo.unitFace;
					float unitSize = bGUnitInfo2.unitSize;
					bGUnitInfo2.unitSize = bGUnitInfo.unitSize;
					bGUnitInfo.unitSize = unitSize;
					bGUnitInfo2.rotation = bGUnitInfo.rotation;
					bGUnitInfo2.flip = bGUnitInfo.flip;
					bGUnitInfo2.animTime = bGUnitInfo.animTime;
					UpdateUnitIllust(i, m_currentBackgroundInfo);
					break;
				}
			}
		}
		bGUnitInfo.unitUid = num;
		bGUnitInfo.unitType = GetUnitType(num);
		bGUnitInfo.unitFace = 0;
		bGUnitInfo.animTime = 0f;
		UpdateUnitIllust(m_currentUnitIndex, m_currentBackgroundInfo);
		if (!flag)
		{
			switch (bGUnitInfo.unitType)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
			{
				NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(num);
				if (unitFromUID != null)
				{
					NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_CAPTAIN, unitFromUID);
				}
				break;
			}
			case NKM_UNIT_TYPE.NUT_OPERATOR:
			{
				NKMOperator operatorFromUId = NKCScenManager.CurrentUserData().m_ArmyData.GetOperatorFromUId(num);
				if (operatorFromUId != null)
				{
					NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_CAPTAIN, operatorFromUId);
				}
				break;
			}
			}
		}
		m_btnDetailOption.SetLock(value: false);
		UpdateUnitList();
		RefreshDetailButtons();
		RefreshEmotionMenu();
	}

	private NKM_UNIT_TYPE GetUnitType(long unitUID)
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		if (armyData.GetUnitFromUID(unitUID) != null)
		{
			return NKM_UNIT_TYPE.NUT_NORMAL;
		}
		if (armyData.GetShipFromUID(unitUID) != null)
		{
			return NKM_UNIT_TYPE.NUT_SHIP;
		}
		if (armyData.GetOperatorFromUId(unitUID) != null)
		{
			return NKM_UNIT_TYPE.NUT_OPERATOR;
		}
		return NKM_UNIT_TYPE.NUT_NORMAL;
	}

	private NKMBackgroundUnitInfo GetBGUnitInfo(int index, NKMBackgroundInfo bgInfo)
	{
		if (index < 0)
		{
			return null;
		}
		if (bgInfo == null)
		{
			return null;
		}
		if (bgInfo.unitInfoList == null)
		{
			return null;
		}
		if (index < bgInfo.unitInfoList.Count)
		{
			return bgInfo.unitInfoList[index];
		}
		return null;
	}

	private bool IsStopAnim(int index)
	{
		return IsStopAnim(GetBGUnitInfo(index, m_currentBackgroundInfo));
	}

	private bool IsStopAnim(NKMBackgroundUnitInfo bgUnitInfo)
	{
		if (bgUnitInfo == null)
		{
			return false;
		}
		return bgUnitInfo.animTime > 0f;
	}

	public void UpdateUnitIllust(int index, NKMBackgroundInfo bgInfo)
	{
		if (NKCScenManager.CurrentUserData() != null)
		{
			NKCUICharacterView charView = GetCharView(index);
			if (!(charView == null))
			{
				NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(index, bgInfo);
				charView.SetCharacterIllust(bGUnitInfo);
				RefreshEmotionMenu();
				RefreshDetailButtons();
			}
		}
	}

	private bool GetUnitSkinBGEnabled(int index)
	{
		return GetBGUnitInfo(index, m_currentBackgroundInfo)?.backImage ?? true;
	}

	private void SetUnitSkinBGEnabled(int index, bool value)
	{
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(index, m_currentBackgroundInfo);
		if (bGUnitInfo.backImage != value)
		{
			bGUnitInfo.backImage = value;
			UpdateUnitIllust(index, m_currentBackgroundInfo);
		}
	}

	private void OnTglUnitBG(bool value)
	{
		SetUnitSkinBGEnabled(m_currentUnitIndex, value);
	}

	private void CycleSkinOption(int index)
	{
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(index, m_currentBackgroundInfo);
		if (m_skinOptionCount == 0)
		{
			bGUnitInfo.skinOption = 0;
			UpdateUnitIllust(index, m_currentBackgroundInfo);
		}
		else
		{
			int skinOption = (bGUnitInfo.skinOption + 1) % m_skinOptionCount;
			bGUnitInfo.skinOption = skinOption;
			UpdateUnitIllust(index, m_currentBackgroundInfo);
		}
	}

	private void OnSkinOption()
	{
		CycleSkinOption(m_currentUnitIndex);
	}

	private void OnSkinChange()
	{
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		if (bGUnitInfo != null && bGUnitInfo.unitType == NKM_UNIT_TYPE.NUT_NORMAL)
		{
			NKMUnitData unitOrTrophyFromUID = NKCScenManager.CurrentArmyData().GetUnitOrTrophyFromUID(bGUnitInfo.unitUid);
			if (unitOrTrophyFromUID != null)
			{
				NKCUIShopSkinPopup.Instance.OpenForUnitInfo(unitOrTrophyFromUID, bShowUpsideMenu: false);
			}
		}
	}

	private void ResetPosition()
	{
		NKCUICharacterView charView = GetCharView(m_currentUnitIndex);
		if (charView != null)
		{
			charView.SetOffset(Vector2.zero);
			charView.SetScale(1f);
			charView.SetRotation(0f);
			charView.SetFlip(value: false);
		}
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		if (bGUnitInfo != null)
		{
			bGUnitInfo.unitPosX = 0f;
			bGUnitInfo.unitPosY = 0f;
			bGUnitInfo.unitSize = 1f;
			bGUnitInfo.rotation = 0f;
			bGUnitInfo.flip = false;
			bGUnitInfo.animTime = -1f;
		}
	}

	private void ToggleDetailMenu()
	{
		if (m_LobbyFace.IsOpen)
		{
			m_LobbyFace.Close();
		}
		else
		{
			OpenDetailMenu();
		}
	}

	private void OpenDetailMenu()
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		NKCASUIUnitIllust targetIllust = GetCharView(m_currentUnitIndex)?.GetUnitIllust();
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		int currentFaceID = ((bGUnitInfo.unitFace != -10) ? bGUnitInfo.unitFace : 0);
		switch (bGUnitInfo.unitType)
		{
		case NKM_UNIT_TYPE.NUT_OPERATOR:
		{
			NKMOperator operatorFromUId = armyData.GetOperatorFromUId(bGUnitInfo.unitUid);
			m_LobbyFace.Open(operatorFromUId, currentFaceID, targetIllust);
			break;
		}
		case NKM_UNIT_TYPE.NUT_NORMAL:
		case NKM_UNIT_TYPE.NUT_SHIP:
		{
			NKMUnitData unitOrShipFromUID = armyData.GetUnitOrShipFromUID(bGUnitInfo.unitUid);
			m_LobbyFace.Open(unitOrShipFromUID, currentFaceID, targetIllust);
			break;
		}
		}
	}

	private void RefreshEmotionMenu()
	{
		if (m_LobbyFace.IsOpen)
		{
			OpenDetailMenu();
		}
	}

	private void OnSelectFace(int selectedID)
	{
		GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo).unitFace = selectedID;
		NKCUICharacterView charView = GetCharView(m_currentUnitIndex);
		NKMLobbyFaceTemplet defaultAnimation = NKMTempletContainer<NKMLobbyFaceTemplet>.Find(selectedID);
		charView.SetDefaultAnimation(defaultAnimation);
		StopAnim(value: false);
		RefreshDetailButtons();
	}

	private void OpenSelectBackground()
	{
		NKCUtil.SetGameobjectActive(m_ChangeLobbyBackground, bValue: true);
		m_ChangeLobbyBackground.UpdateData(m_currentBackgroundInfo.backgroundItemId);
	}

	private void CloseSelectBackground()
	{
		NKCUtil.SetGameobjectActive(m_ChangeLobbyBackground, bValue: false);
	}

	private void OnChangeBackground(int id)
	{
		int num = id;
		if (num == 9001)
		{
			num = 0;
		}
		UpdateBackground(num);
		UpdateBGSlotList(num);
		UpdateBackgroundMusic(m_currentBackgroundInfo);
	}

	public void SetBackgroundInfo(NKMBackgroundInfo bgInfo, bool bMusic)
	{
		m_currentBackgroundInfo = bgInfo;
		for (int i = 0; i < 8; i++)
		{
			UpdateUnitIllust(i, m_currentBackgroundInfo);
		}
		UpdateBackground(m_currentBackgroundInfo.backgroundItemId);
		UpdateBackgroundMusic(m_currentBackgroundInfo);
		UpdateBGSlotList(m_currentBackgroundInfo.backgroundItemId);
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		m_btnDetailOption.SetLock(value: false);
		m_tglUnitBG.Select(GetUnitSkinBGEnabled(m_currentUnitIndex));
		if (bGUnitInfo.unitType == NKM_UNIT_TYPE.NUT_NORMAL || bGUnitInfo.unitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			RefreshEmotionMenu();
		}
		else
		{
			m_LobbyFace.Close();
		}
		UpdateUnitList();
		RefreshDetailButtons();
		m_bBGMContinue = NKCScenManager.CurrentUserData().BackgroundBGMContinue;
		m_ctglMusicContinue.Select(m_bBGMContinue, bForce: true);
	}

	public void UpdateBackground(int backgroundItemId)
	{
		NKCBackgroundTemplet nKCBackgroundTemplet = null;
		nKCBackgroundTemplet = NKCBackgroundTemplet.Find(backgroundItemId);
		if (nKCBackgroundTemplet == null)
		{
			nKCBackgroundTemplet = NKCBackgroundTemplet.Find(9001);
		}
		if (nKCBackgroundTemplet == null)
		{
			m_currentBackgroundInfo.backgroundItemId = 0;
			SetBackground(NKMAssetName.ParseBundleName("AB_UI_BG_SPRITE_CITY_NIGHT", "AB_UI_BG_SPRITE_CITY_NIGHT"), bCamMove: true, NKCBackgroundTemplet.BgType.Background);
		}
		else
		{
			m_currentBackgroundInfo.backgroundItemId = nKCBackgroundTemplet.Key;
			SetBackground(NKMAssetName.ParseBundleName(nKCBackgroundTemplet.m_Background_Prefab, nKCBackgroundTemplet.m_Background_Prefab), nKCBackgroundTemplet.m_bBackground_CamMove, nKCBackgroundTemplet.m_BgType);
		}
	}

	private void SetBackground(NKMAssetName assetName, bool bCamMove, NKCBackgroundTemplet.BgType bgType)
	{
		if (m_objBG != null)
		{
			Object.Destroy(m_objBG);
		}
		switch (bgType)
		{
		case NKCBackgroundTemplet.BgType.Background:
			m_objBG = NKCUILobby3DV2.OpenBackgroundPrefab(assetName, bCamMove, m_trBGRoot, OnDrag, OnTouchBG);
			break;
		case NKCBackgroundTemplet.BgType.Image:
		{
			NKMAssetName assetName2 = new NKMAssetName("AB_UI_BG_SPRITE_EBENUM", "AB_UI_BG_SPRITE_EBENUM");
			m_objBG = NKCUILobby3DV2.OpenBackgroundPrefab(assetName2, bCamMove, m_trBGRoot, OnDrag, OnTouchBG);
			if (m_objBG != null)
			{
				Transform transform = m_objBG.transform.Find("Stretch/Background");
				if (transform != null)
				{
					Image component = transform.GetComponent<Image>();
					Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(assetName);
					NKCUtil.SetImageSprite(component, orLoadAssetResource, bDisableIfSpriteNull: true);
				}
			}
			break;
		}
		case NKCBackgroundTemplet.BgType.CutsceneObject:
			m_objBG = NKCUILobby3DV2.OpenBGCutscenePrefab(assetName, bCamMove, m_trBGRoot, OnDrag, OnTouchBG);
			break;
		}
	}

	public void UpdateBGSlotList(int currentBGID)
	{
		if (!(m_ChangeLobbyBackground == null))
		{
			m_ChangeLobbyBackground.UpdateData(currentBGID);
		}
	}

	private float GetCurrentAngle(Vector2 mousePos)
	{
		NKCUICharacterView charView = GetCharView(m_currentUnitIndex);
		if (charView == null)
		{
			return 0f;
		}
		Vector2 vector = NKCCamera.GetCamera().WorldToScreenPoint(charView.transform.position);
		Vector2 vector2 = mousePos - vector;
		Debug.DrawLine(Vector3.zero, vector2);
		return Quaternion.FromToRotation(Vector3.up, vector2).eulerAngles.z;
	}

	private void OnDragUnit(PointerEventData cPointerEventData)
	{
		if (IsPreviewMode || NKCScenManager.GetScenManager().GetHasPinch())
		{
			return;
		}
		NKCUICharacterView charView = GetCharView(m_currentUnitIndex);
		if (!(charView == null))
		{
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				ChangeScale((cPointerEventData.delta.x + cPointerEventData.delta.y) / (float)Screen.width);
				return;
			}
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				float currentAngle = GetCurrentAngle(cPointerEventData.position);
				float deltaRot = currentAngle - m_fAngleLastFrame;
				ChangeRotation(deltaRot);
				m_fAngleLastFrame = currentAngle;
				return;
			}
			Vector2 screenPoint = cPointerEventData.position - cPointerEventData.delta;
			Vector2 position = cPointerEventData.position;
			RectTransform component = charView.GetComponent<RectTransform>();
			RectTransformUtility.ScreenPointToLocalPointInRectangle(component, screenPoint, NKCCamera.GetCamera(), out var localPoint);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(component, position, NKCCamera.GetCamera(), out var localPoint2);
			Vector2 vector = localPoint2 - localPoint;
			Vector2 scaleVector = charView.GetScaleVector();
			vector.x *= scaleVector.x;
			vector.y *= scaleVector.y;
			vector = Quaternion.AngleAxis(0f - charView.GetRotation(), Vector3.back) * vector;
			ChangeOffset(vector);
		}
	}

	private void OnTouchUnit(PointerEventData cPointerEventData, NKCUICharacterView charView)
	{
		if (charView.GetUnitIllust() == null)
		{
			return;
		}
		int num = m_lstCvUnit.IndexOf(charView);
		if (num < 0 || num >= m_lstCvUnit.Count)
		{
			return;
		}
		if (IsPreviewMode || m_currentUnitIndex == num)
		{
			if (!IsStopAnim(num))
			{
				charView.TouchIllust();
			}
		}
		else
		{
			SelectUnit(num, bPlaySelectAnim: true);
		}
	}

	private void ChangeOffset(Vector2 delta)
	{
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		if (bGUnitInfo != null)
		{
			NKCUICharacterView charView = GetCharView(m_currentUnitIndex);
			if (!(charView == null))
			{
				Vector2 offset = charView.GetOffset() + delta;
				charView.SetOffset(offset);
				bGUnitInfo.unitPosX = offset.x;
				bGUnitInfo.unitPosY = offset.y;
			}
		}
	}

	private void ChangeScale(float deltaScale)
	{
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		if (bGUnitInfo != null)
		{
			NKCUICharacterView charView = GetCharView(m_currentUnitIndex);
			if (!(charView == null))
			{
				float value = charView.GetScale() * Mathf.Pow(5f, deltaScale);
				value = Mathf.Clamp(value, MIN_ZOOM_SCALE, MAX_ZOOM_SCALE);
				DOTween.CompleteAll();
				charView.SetScale(value);
				bGUnitInfo.unitSize = value;
			}
		}
	}

	private void ChangeRotation(float deltaRot)
	{
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		if (bGUnitInfo == null)
		{
			return;
		}
		NKCUICharacterView charView = GetCharView(m_currentUnitIndex);
		if (!(charView == null))
		{
			DOTween.CompleteAll();
			bGUnitInfo.rotation += deltaRot;
			if (bGUnitInfo.rotation < 0f)
			{
				bGUnitInfo.rotation += 360f;
			}
			if (bGUnitInfo.rotation > 360f)
			{
				bGUnitInfo.rotation -= 360f;
			}
			charView.SetRotation(bGUnitInfo.rotation);
		}
	}

	private void Flip()
	{
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		if (bGUnitInfo != null)
		{
			Flip(!bGUnitInfo.flip);
		}
	}

	private void Flip(bool value)
	{
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		if (bGUnitInfo != null)
		{
			bGUnitInfo.flip = value;
			NKCUICharacterView charView = GetCharView(m_currentUnitIndex);
			if (charView != null)
			{
				charView.SetFlip(bGUnitInfo.flip);
			}
			m_tglUnitFlip.Select(value, bForce: true);
		}
	}

	private void StopAnim(bool value)
	{
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		if (bGUnitInfo == null)
		{
			return;
		}
		NKCUICharacterView charView = GetCharView(m_currentUnitIndex);
		if (charView == null)
		{
			return;
		}
		charView.SetAnimationSpeed((!value) ? 1 : 0);
		if (value)
		{
			bGUnitInfo.animTime = charView.GetCurrentAnimationTime();
			if (bGUnitInfo.animTime == 0f)
			{
				bGUnitInfo.animTime = float.Epsilon;
			}
			if (string.Equals(charView.GetAnimationName(), "TOUCH"))
			{
				bGUnitInfo.unitFace = -10;
			}
		}
		else
		{
			bGUnitInfo.animTime = -1f;
		}
		m_tglUnitAnimation.Select(value, bForce: true);
		RefreshDetailButtons();
		RefreshEmotionMenu();
	}

	private void ToggleStopAnim()
	{
		NKMBackgroundUnitInfo bGUnitInfo = GetBGUnitInfo(m_currentUnitIndex, m_currentBackgroundInfo);
		if (bGUnitInfo != null)
		{
			bool flag = IsStopAnim(bGUnitInfo);
			StopAnim(!flag);
		}
	}

	private void Update()
	{
		if (NKCScenManager.GetScenManager().GetHasPinch())
		{
			ProcessPinch();
			return;
		}
		ResetPinch();
		if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetMouseButtonDown(0))
		{
			m_fAngleLastFrame = GetCurrentAngle(Input.mousePosition);
		}
		if (!IsOpenSelectBackground)
		{
			float y = Input.mouseScrollDelta.y;
			if (y != 0f)
			{
				ChangeScale(y * m_MouseWheelSensibility);
			}
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			Flip();
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			ToggleStopAnim();
		}
		Vector2 moveVector = NKCInputManager.GetMoveVector();
		if (moveVector.sqrMagnitude >= 0f)
		{
			moveVector = moveVector * m_fKeyboardMoveSpeed * GetKeyboardMoveMultiplier();
			ChangeOffset(moveVector);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		OnDragUnit(eventData);
	}

	public void OnTouchBG(PointerEventData eventData)
	{
		if (IsPreviewMode)
		{
			SetPreview(value: false);
		}
	}

	private void UpdateBackgroundMusic(NKMBackgroundInfo bgInfo, bool bForce = false)
	{
		NKCBGMInfoTemplet nKCBGMInfoTemplet = NKMTempletContainer<NKCBGMInfoTemplet>.Find(bgInfo.backgroundBgmId);
		if (nKCBGMInfoTemplet != null)
		{
			NKCSoundManager.PlayMusic(nKCBGMInfoTemplet.m_BgmAssetID, bLoop: true, nKCBGMInfoTemplet.BGMVolume, bForce);
			NKCUtil.SetLabelText(m_lbMusicTitle, NKCStringTable.GetString(nKCBGMInfoTemplet.m_BgmNameStringID));
			return;
		}
		NKCBackgroundTemplet nKCBackgroundTemplet = NKCBackgroundTemplet.Find(bgInfo.backgroundItemId);
		float fLocalVol = 1f;
		foreach (NKCBGMInfoTemplet value in NKMTempletContainer<NKCBGMInfoTemplet>.Values)
		{
			if (string.Equals(nKCBackgroundTemplet.m_Background_Music, value.m_BgmAssetID))
			{
				fLocalVol = value.BGMVolume;
				break;
			}
		}
		string text = "";
		foreach (NKCBGMInfoTemplet value2 in NKMTempletContainer<NKCBGMInfoTemplet>.Values)
		{
			if (string.Equals(value2.m_BgmAssetID, nKCBackgroundTemplet.m_Background_Music))
			{
				text = NKCStringTable.GetString(value2.m_BgmNameStringID);
				break;
			}
		}
		NKCSoundManager.PlayMusic(nKCBackgroundTemplet.m_Background_Music, bLoop: true, fLocalVol);
		NKCUtil.SetLabelText(m_lbMusicTitle, (!string.IsNullOrEmpty(text)) ? text : NKCUtilString.GET_STRING_JUKEBOX_MUSIC_DEFAULT);
	}

	private void OnClickBGMContinue(bool bSet)
	{
		m_bBGMContinue = bSet;
	}

	private void UpdatePresetButtons()
	{
	}

	private void TrySavePreset(NKMBackgroundInfo bgInfo)
	{
		if (m_currentPresetIndex >= 0)
		{
			SavePreset(m_currentPresetIndex, bgInfo);
		}
	}

	private void OnTglPreset(bool value, int index)
	{
		if (value)
		{
			SelectPreset(index);
		}
	}

	private void SelectPreset(int value)
	{
		m_currentPresetIndex = value;
		m_csbtnLoadPreset?.SetLock(!HasPreset(value));
		m_csbtnSavePreset?.UnLock();
	}

	private void OnBtnLoadPreset()
	{
		if (HasPreset(m_currentPresetIndex))
		{
			string title = NKCStringTable.GetString("SI_DP_CHANGE_LOBBY_PRESET_LOAD_CONFIRM_TITLE", m_currentPresetIndex + 1);
			string desc = NKCStringTable.GetString("SI_PF_USER_INFO_LOBBY_CHANGE_PRESET_LOAD");
			NKCUIPopupChangeLobbyPreview.Instance.Open(m_currentPresetIndex, title, GetPreviewThumbnailPath(m_currentPresetIndex), desc, TryLoadPreset);
		}
	}

	private void OnBtnSavePreset()
	{
		if (HasPreset(m_currentPresetIndex))
		{
			string title = NKCStringTable.GetString("SI_DP_CHANGE_LOBBY_PRESET_LOAD_CONFIRM_TITLE", m_currentPresetIndex + 1);
			string desc = NKCStringTable.GetString("SI_PF_USER_INFO_LOBBY_CHANGE_PRESET_SAVE");
			NKCUIPopupChangeLobbyPreview.Instance.Open(m_currentPresetIndex, title, GetPreviewThumbnailPath(m_currentPresetIndex), desc, SavePreset);
		}
		else
		{
			SavePreset(m_currentPresetIndex);
		}
	}

	private void SavePreset(int index)
	{
		if (index >= 0)
		{
			SavePreset(index, m_currentBackgroundInfo);
			UpdatePresetButtons();
			m_csbtnLoadPreset?.UnLock();
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_DP_LOBBY_BACKGROUND_PRESET_SAVED", m_currentPresetIndex + 1));
		}
	}

	private void TryLoadPreset(int index)
	{
		NKMBackgroundInfo nKMBackgroundInfo = LoadPreset(index);
		if (nKMBackgroundInfo != null)
		{
			SetBackgroundInfo(nKMBackgroundInfo, bMusic: true);
		}
	}

	private void SavePreset(int index, NKMBackgroundInfo bgInfo)
	{
		string value = EncodeBGInfo(bgInfo);
		PlayerPrefs.SetString($"BACKGROUND_PRESET_{NKCScenManager.CurrentUserData().m_UserUID}_{index}", value);
		string previewThumbnailPath = GetPreviewThumbnailPath(index);
		NKCScreenCaptureUtility.CaptureCamera(NKCCamera.GetCamera(), previewThumbnailPath, Screen.width / 4, Screen.height / 4);
	}

	private string GetPreviewThumbnailPath(int index)
	{
		string text = $"BACKGROUND_PRESET_{NKCScenManager.CurrentUserData().m_UserUID}_{index}";
		return Path.Combine(Application.persistentDataPath, text + ".png");
	}

	private NKMBackgroundInfo LoadPreset(int index)
	{
		if (!HasPreset(index))
		{
			return null;
		}
		string encodedString = PlayerPrefs.GetString($"BACKGROUND_PRESET_{NKCScenManager.CurrentUserData().m_UserUID}_{index}");
		return DecodeBGInfo(encodedString);
	}

	private NKMBackgroundInfo MakeEmptyBGInfo()
	{
		NKMBackgroundInfo nKMBackgroundInfo = new NKMBackgroundInfo();
		nKMBackgroundInfo.unitInfoList = new List<NKMBackgroundUnitInfo>(8);
		for (int i = 0; i < 8; i++)
		{
			nKMBackgroundInfo.unitInfoList.Add(new NKMBackgroundUnitInfo());
		}
		return nKMBackgroundInfo;
	}

	private bool HasPreset(int index)
	{
		return PlayerPrefs.HasKey($"BACKGROUND_PRESET_{NKCScenManager.CurrentUserData().m_UserUID}_{index}");
	}

	private string EncodeBGInfo(NKMBackgroundInfo bgInfo)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("BgInfo={");
		stringBuilder.AppendFormat("bgID={0},", bgInfo.backgroundItemId);
		stringBuilder.AppendFormat("bgmID={0},", bgInfo.backgroundBgmId);
		for (int i = 0; i < bgInfo.unitInfoList.Count; i++)
		{
			stringBuilder.Append("{");
			NKMBackgroundUnitInfo nKMBackgroundUnitInfo = bgInfo.unitInfoList[i];
			if (nKMBackgroundUnitInfo != null)
			{
				stringBuilder.AppendFormat("uid=\"{0}\",", nKMBackgroundUnitInfo.unitUid);
				stringBuilder.AppendFormat("type={0},", (int)nKMBackgroundUnitInfo.unitType);
				stringBuilder.AppendFormat("size={0},", nKMBackgroundUnitInfo.unitSize);
				stringBuilder.AppendFormat("face={0},", nKMBackgroundUnitInfo.unitFace);
				stringBuilder.AppendFormat("X={0},", nKMBackgroundUnitInfo.unitPosX);
				stringBuilder.AppendFormat("Y={0},", nKMBackgroundUnitInfo.unitPosY);
				stringBuilder.AppendFormat("back={0},", nKMBackgroundUnitInfo.backImage.ToString().ToLower());
				stringBuilder.AppendFormat("skinOption={0},", nKMBackgroundUnitInfo.skinOption);
				stringBuilder.AppendFormat("rotation ={0},", nKMBackgroundUnitInfo.rotation);
				stringBuilder.AppendFormat("flip ={0},", nKMBackgroundUnitInfo.flip.ToString().ToLower());
				stringBuilder.AppendFormat("animTime={0},", nKMBackgroundUnitInfo.animTime);
			}
			stringBuilder.Append("},");
		}
		stringBuilder.Append("}");
		return stringBuilder.ToString();
	}

	private NKMBackgroundInfo DecodeBGInfo(string encodedString)
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		NKMBackgroundInfo nKMBackgroundInfo = new NKMBackgroundInfo();
		using NKMLua nKMLua = new NKMLua();
		if (!nKMLua.DoString(encodedString))
		{
			return null;
		}
		if (nKMLua.OpenTable("BgInfo"))
		{
			nKMLua.GetData("bgID", ref nKMBackgroundInfo.backgroundItemId);
			nKMLua.GetData("bgmID", ref nKMBackgroundInfo.backgroundBgmId);
			nKMBackgroundInfo.unitInfoList = new List<NKMBackgroundUnitInfo>();
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKMBackgroundUnitInfo nKMBackgroundUnitInfo = new NKMBackgroundUnitInfo();
				string s = nKMLua.GetString("uid");
				nKMBackgroundUnitInfo.unitUid = long.Parse(s);
				int rValue = 0;
				nKMLua.GetData("type", ref rValue);
				nKMBackgroundUnitInfo.unitType = (NKM_UNIT_TYPE)rValue;
				nKMLua.GetData("size", ref nKMBackgroundUnitInfo.unitSize);
				nKMLua.GetData("face", ref nKMBackgroundUnitInfo.unitFace);
				nKMLua.GetData("X", ref nKMBackgroundUnitInfo.unitPosX);
				nKMLua.GetData("Y", ref nKMBackgroundUnitInfo.unitPosY);
				nKMLua.GetData("back", ref nKMBackgroundUnitInfo.backImage);
				nKMLua.GetData("skinOption", ref nKMBackgroundUnitInfo.skinOption);
				nKMLua.GetData("rotation", ref nKMBackgroundUnitInfo.rotation);
				nKMLua.GetData("flip", ref nKMBackgroundUnitInfo.flip);
				nKMLua.GetData("animTime", ref nKMBackgroundUnitInfo.animTime);
				switch (nKMBackgroundUnitInfo.unitType)
				{
				case NKM_UNIT_TYPE.NUT_OPERATOR:
					if (armyData.GetOperatorFromUId(nKMBackgroundUnitInfo.unitUid) == null)
					{
						nKMBackgroundUnitInfo.unitUid = 0L;
						nKMBackgroundUnitInfo.unitType = NKM_UNIT_TYPE.NUT_INVALID;
						nKMBackgroundUnitInfo.unitFace = 0;
					}
					break;
				case NKM_UNIT_TYPE.NUT_NORMAL:
				case NKM_UNIT_TYPE.NUT_SHIP:
					if (armyData.GetUnitOrShipFromUID(nKMBackgroundUnitInfo.unitUid) == null)
					{
						nKMBackgroundUnitInfo.unitUid = 0L;
						nKMBackgroundUnitInfo.unitType = NKM_UNIT_TYPE.NUT_INVALID;
						nKMBackgroundUnitInfo.unitFace = 0;
					}
					break;
				}
				nKMBackgroundInfo.unitInfoList.Add(nKMBackgroundUnitInfo);
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
			while (nKMBackgroundInfo.unitInfoList.Count < 8)
			{
				NKMBackgroundUnitInfo nKMBackgroundUnitInfo2 = new NKMBackgroundUnitInfo();
				nKMBackgroundUnitInfo2.unitUid = 0L;
				nKMBackgroundUnitInfo2.unitFace = 0;
				nKMBackgroundUnitInfo2.unitPosX = 0f;
				nKMBackgroundUnitInfo2.unitPosY = 0f;
				nKMBackgroundUnitInfo2.unitSize = 1f;
				nKMBackgroundUnitInfo2.backImage = true;
				nKMBackgroundUnitInfo2.skinOption = 0;
				nKMBackgroundUnitInfo2.rotation = 0f;
				nKMBackgroundUnitInfo2.flip = false;
				nKMBackgroundUnitInfo2.animTime = -1f;
				nKMBackgroundUnitInfo2.unitType = NKM_UNIT_TYPE.NUT_NORMAL;
				nKMBackgroundInfo.unitInfoList.Add(nKMBackgroundUnitInfo2);
			}
			return nKMBackgroundInfo;
		}
		return null;
	}

	private void SetPreview(bool value)
	{
		m_bPreviewMode = value;
		bool flag = !(m_tglPreviewOption != null) || m_tglPreviewOption.IsSelected;
		NKCUtil.SetGameobjectActive(m_csbtnExitPreview, value);
		NKCUtil.SetGameobjectActive(m_objPreviewRoot, value && flag);
		NKCUtil.SetGameobjectActive(m_objUIRoot, !value);
	}

	private void OnBtnPreview()
	{
		SetPreview(value: true);
	}

	private void OnBtnPreviewExit()
	{
		SetPreview(value: false);
	}

	public override void OnHotkeyHold(HotkeyEventType hotkey)
	{
		float keyboardMoveMultiplier = GetKeyboardMoveMultiplier();
		switch (hotkey)
		{
		case HotkeyEventType.Plus:
			ChangeScale(Time.deltaTime * 0.5f * keyboardMoveMultiplier);
			break;
		case HotkeyEventType.Minus:
			ChangeScale((0f - Time.deltaTime) * 0.5f * keyboardMoveMultiplier);
			break;
		case HotkeyEventType.RotateLeft:
			ChangeRotation(90f * Time.deltaTime * keyboardMoveMultiplier);
			break;
		case HotkeyEventType.RotateRight:
			ChangeRotation(-90f * Time.deltaTime * keyboardMoveMultiplier);
			break;
		}
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		switch (hotkey)
		{
		case HotkeyEventType.ShowHotkey:
			NKCUIComHotkeyDisplay.OpenInstance(base.transform, HotkeyEventType.Left, HotkeyEventType.Down, HotkeyEventType.Up, HotkeyEventType.Right, HotkeyEventType.RotateLeft, HotkeyEventType.RotateRight, HotkeyEventType.Plus, HotkeyEventType.Minus);
			if (m_btnDetailOption != null)
			{
				NKCUIComHotkeyDisplay.OpenInstance(m_btnDetailOption.transform, HotkeyEventType.NextTab);
			}
			if (m_LobbyFace.IsOpen)
			{
				if (m_tglUnitFlip != null)
				{
					NKCUIComHotkeyDisplay.OpenInstance(m_tglUnitFlip.transform, "R");
				}
				if (m_tglUnitAnimation != null)
				{
					NKCUIComHotkeyDisplay.OpenInstance(m_tglUnitAnimation.transform, "F");
				}
			}
			break;
		case HotkeyEventType.PrevTab:
		case HotkeyEventType.NextTab:
			ToggleDetailMenu();
			break;
		}
		return false;
	}

	private float GetKeyboardMoveMultiplier()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			return m_fShiftMoveMagnitude;
		}
		if (Input.GetKey(KeyCode.LeftControl))
		{
			return m_fCtrlMoveMagnitude;
		}
		return 1f;
	}

	private void ProcessPinch()
	{
		switch (m_ePinchMode)
		{
		case PinchMode.ScaleOnly:
			ChangeScale(NKCScenManager.GetScenManager().GetPinchDeltaMagnitude());
			m_fPinchAngleChanged += NKCScenManager.GetScenManager().GetPinchDeltaRotation();
			if (Mathf.Abs(m_fPinchAngleChanged) > m_fPinchAngleThreshold)
			{
				m_ePinchMode = PinchMode.Free;
			}
			break;
		case PinchMode.Free:
			ChangeScale(NKCScenManager.GetScenManager().GetPinchDeltaMagnitude());
			ChangeRotation(NKCScenManager.GetScenManager().GetPinchDeltaRotation());
			break;
		}
	}

	private void ResetPinch()
	{
		m_ePinchMode = PinchMode.ScaleOnly;
		m_fPinchAngleChanged = 0f;
	}
}
