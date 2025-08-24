using System.Collections.Generic;
using ClientPacket.Common;
using DG.Tweening;
using NKC.UI.Result;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupOpSkipProcess : NKCUIBase
{
	private enum AniState
	{
		BASE,
		FIGHT,
		END,
		STOP
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_OPERATION";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_OPERATION SKIP";

	private static NKCPopupOpSkipProcess m_Instance;

	public LoopScrollRect m_loopScrollRect;

	public GridLayoutGroup m_gridLayoutGroup;

	public GameObject m_objCount;

	public Text m_lbCount;

	public Text m_lbTitle;

	public Animator m_aniShip;

	public GameObject m_objExplosion;

	public GameObject m_objComplete;

	public RawImage m_imgBackground;

	public RawImage m_imgBgDummy;

	public Image m_imgShip;

	public EventTrigger m_eventTriggerBg;

	public EventTrigger m_eventTriggerRewardUI;

	public EventTrigger m_eventTriggerPanel;

	public NKCUIComStateButton m_csbtnConfirm;

	public float m_fIdleTime;

	public float m_fBgScrollSpeed;

	public int m_iColumnMinCount;

	[Header("애니메이션 이름")]
	public string m_baseAniName;

	public string m_fightAniName;

	public string m_endAniName;

	[Header("배경 셰이크 효과")]
	public float m_fShakeDuration;

	public float m_fShakeStrength;

	public int m_iShakeVibration;

	[Header("아이템 슬롯 셰이크 효과")]
	public float m_fShakeItemDuration;

	public float m_fShakeItemStrength;

	public int m_iShakeItemVibration;

	[Header("아이템 슬롯 펀칭 효과")]
	public float m_fPunchItemDuration;

	public float m_fPunchItemScale;

	[Header("아이템 슬롯 스케일 효과")]
	public float m_fScaleItemDuration;

	public float m_fScaleItemInterval;

	[Header("단계별 스킵 애니메이션 타임 기준")]
	public float m_fStepSkipAniTime;

	[Header("애니메이션 스킵 키입력 유지 시간")]
	public float PressSkipTime;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private List<NKCUIResult.BattleResultData> m_lstResultDatas = new List<NKCUIResult.BattleResultData>();

	private Dictionary<int, List<NKCUISlot.SlotData>> m_dicRewardSlotData = new Dictionary<int, List<NKCUISlot.SlotData>>();

	private List<NKCUISlot.SlotData> m_lstRewardSortedSlotData = new List<NKCUISlot.SlotData>();

	private HashSet<int> m_hsShakeSlotID = new HashSet<int>();

	private List<Tween> m_lstTweenSlot = new List<Tween>();

	private AniState m_eAniState;

	private Vector2 m_vBgDummyUV;

	private float m_fPrevXoffset;

	private float m_fTimer;

	private float m_fPressSkipTimer;

	private int m_iAniRepeat;

	private int m_iFightCount;

	private int m_iFullCount;

	private bool m_bOffsetCalc;

	private bool m_bUpdateCount;

	private object m_bgTweener;

	public static NKCPopupOpSkipProcess Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupOpSkipProcess>("AB_UI_NKM_UI_OPERATION", "NKM_UI_POPUP_OPERATION SKIP", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupOpSkipProcess>();
				m_Instance?.Init();
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

	public override string MenuName => "Operaion Skip";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void Init()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetButtonClickDelegate(m_csbtnConfirm, OnClickConfirm);
		NKCUtil.SetGameobjectActive(m_imgBgDummy.gameObject, bValue: false);
		NKCUtil.SetHotkey(m_csbtnConfirm, HotkeyEventType.Confirm);
		m_loopScrollRect.dOnGetObject += GetRewardSlot;
		m_loopScrollRect.dOnReturnObject += ReturnRewardSlot;
		m_loopScrollRect.dOnProvideData += ProvideRewardData;
		m_loopScrollRect.dOnRepopulate += RepopulateScrollRect;
		m_loopScrollRect.ContentConstraintCount = m_gridLayoutGroup.constraintCount;
		NKCUtil.SetScrollHotKey(m_loopScrollRect);
		if (m_eventTriggerBg != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				OnClickConfirm();
			});
			m_eventTriggerBg.triggers.Add(entry);
		}
		if (m_eventTriggerRewardUI != null)
		{
			EventTrigger.Entry entry2 = new EventTrigger.Entry();
			entry2.eventID = EventTriggerType.PointerClick;
			entry2.callback.AddListener(delegate
			{
				OnClickRewardUI();
			});
			m_eventTriggerRewardUI.triggers.Add(entry2);
		}
		if (m_eventTriggerPanel != null)
		{
			EventTrigger.Entry entry3 = new EventTrigger.Entry();
			entry3.eventID = EventTriggerType.PointerClick;
			entry3.callback.AddListener(delegate
			{
				OnClickRewardUI();
			});
			m_eventTriggerPanel.triggers.Add(entry3);
		}
		base.gameObject.SetActive(value: false);
	}

	public override void CloseInternal()
	{
		m_lstResultDatas.Clear();
		m_dicRewardSlotData.Clear();
		m_lstRewardSortedSlotData.Clear();
		m_hsShakeSlotID.Clear();
		int count = m_lstTweenSlot.Count;
		for (int i = 0; i < count; i++)
		{
			m_lstTweenSlot[i].Kill();
		}
		m_lstTweenSlot.Clear();
		base.gameObject.SetActive(value: false);
		if (NKCContentManager.CheckLevelChanged())
		{
			NKCPopupUserLevelUp.instance.Open(NKCScenManager.CurrentUserData(), delegate
			{
				NKCContentManager.SetLevelChanged(bValue: false);
				NKCUIManager.OnBackButton();
			});
		}
		else
		{
			NKCUIManager.OnBackButton();
		}
	}

	public override void OnBackButton()
	{
	}

	public void Open(List<NKMDungeonRewardSet> rewardSetList, List<UnitLoyaltyUpdateData> lstUnitLoyaltyData, string title = null)
	{
		if (rewardSetList != null)
		{
			SetRewardData(rewardSetList, lstUnitLoyaltyData);
			base.gameObject.SetActive(value: true);
			OpenProcessCommon(rewardSetList.Count, title);
			m_NKCUIOpenAnimator?.PlayOpenAni();
			UIOpened();
		}
	}

	public void Open(List<NKMRewardData> lstRewardData, List<UnitLoyaltyUpdateData> lstUnitLoyaltyData, string title = null)
	{
		if (lstRewardData != null)
		{
			SetRewardData(lstRewardData, lstUnitLoyaltyData);
			base.gameObject.SetActive(value: true);
			OpenProcessCommon(lstRewardData.Count, title);
			m_NKCUIOpenAnimator?.PlayOpenAni();
			UIOpened();
		}
	}

	private void OpenProcessCommon(int rewardResultCount, string title)
	{
		m_fTimer = m_fIdleTime;
		m_fPressSkipTimer = 0f;
		m_iFightCount = 0;
		m_iFullCount = rewardResultCount;
		m_eAniState = AniState.BASE;
		m_aniShip.Play(m_baseAniName);
		m_fPrevXoffset = 0f;
		m_iAniRepeat = 0;
		m_bOffsetCalc = false;
		m_bUpdateCount = true;
		RepopulateScrollRect();
		m_loopScrollRect.PrepareCells();
		m_loopScrollRect.TotalCount = 0;
		m_loopScrollRect.StopMovement();
		m_loopScrollRect.SetIndexPosition(0);
		NKCUtil.SetGameobjectActive(m_objCount, bValue: true);
		SetCountText(m_iFightCount, m_iFullCount);
		if (string.IsNullOrEmpty(title))
		{
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_PF_OPERATION_SKIP_RESULT"));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbTitle, title);
		}
	}

	private void Update()
	{
		if (!base.IsOpen)
		{
			return;
		}
		m_NKCUIOpenAnimator?.Update();
		if (Input.GetKey(KeyCode.LeftControl))
		{
			m_fPressSkipTimer += Time.deltaTime;
			if (m_fPressSkipTimer >= PressSkipTime)
			{
				OnClickConfirm();
				m_fPressSkipTimer = 0f;
			}
		}
		else
		{
			m_fPressSkipTimer = 0f;
		}
		if (m_eAniState == AniState.STOP)
		{
			return;
		}
		switch (m_eAniState)
		{
		case AniState.BASE:
			AnimateBgUV(m_fBgScrollSpeed * Time.deltaTime);
			if (m_fTimer <= 0f)
			{
				if (m_iFullCount > 1)
				{
					m_eAniState = AniState.FIGHT;
					m_aniShip.Play(m_fightAniName);
				}
				else
				{
					m_eAniState = AniState.END;
					m_aniShip.Play(m_endAniName);
				}
				m_iAniRepeat = 0;
			}
			else
			{
				m_fTimer -= Time.deltaTime;
			}
			break;
		case AniState.FIGHT:
		{
			int num = (int)m_aniShip.GetCurrentAnimatorStateInfo(0).normalizedTime;
			if (m_objExplosion.activeInHierarchy)
			{
				if (m_bUpdateCount)
				{
					if (m_bgTweener != null)
					{
						DOTween.Kill(m_bgTweener);
					}
					m_bgTweener = DOTween.Shake(() => m_imgBackground.uvRect.position, delegate(Vector3 v)
					{
						Vector2 position = m_imgBackground.uvRect.position;
						Vector2 size = m_imgBackground.uvRect.size;
						m_imgBackground.uvRect = new Rect(position.x, v.y, size.x, size.y);
					}, m_fShakeDuration, m_fShakeStrength, m_iShakeVibration).target;
					RefreshRewardData(m_iFightCount);
					SetCountText(++m_iFightCount, m_iFullCount);
					m_bUpdateCount = false;
				}
			}
			else
			{
				m_bUpdateCount = true;
			}
			if (m_iAniRepeat != num)
			{
				m_iAniRepeat = num;
				if (m_iFightCount >= m_iFullCount - 1)
				{
					m_eAniState = AniState.END;
					m_aniShip.Play(m_endAniName);
					m_iAniRepeat = 0;
					m_bOffsetCalc = false;
					m_bUpdateCount = true;
					break;
				}
			}
			AnimateBgUV(m_fBgScrollSpeed * Time.deltaTime);
			break;
		}
		case AniState.END:
			if (m_bOffsetCalc)
			{
				m_fPrevXoffset = m_imgBgDummy.uvRect.position.x - m_vBgDummyUV.x;
			}
			else
			{
				m_bOffsetCalc = true;
			}
			m_vBgDummyUV = m_imgBgDummy.uvRect.position;
			AnimateBgUV(m_fPrevXoffset);
			if (m_objComplete.activeInHierarchy && m_bUpdateCount)
			{
				RefreshRewardData(m_iFightCount);
				SetCountText(++m_iFightCount, m_iFullCount);
				m_bUpdateCount = false;
			}
			if (m_aniShip.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				m_eAniState = AniState.STOP;
			}
			break;
		}
	}

	private void SetRewardData(List<NKMDungeonRewardSet> rewardSetList, List<UnitLoyaltyUpdateData> lstUnitLoyaltyData)
	{
		m_lstResultDatas.Clear();
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		int count = rewardSetList.Count;
		for (int i = 0; i < count; i++)
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(rewardSetList[i].dungeonClearData.dungeonId);
			int stageID = 0;
			if (dungeonTempletBase.StageTemplet != null)
			{
				stageID = dungeonTempletBase.StageTemplet.Key;
			}
			NKCUIResult.BattleResultData item = NKCUIResult.MakeMissionResultData(myUserData.m_ArmyData, rewardSetList[i].dungeonClearData.dungeonId, stageID, bWin: true, rewardSetList[i].dungeonClearData, NKMDeckIndex.None, null, lstUnitLoyaltyData);
			m_lstResultDatas.Add(item);
		}
	}

	private void SetRewardData(List<NKMRewardData> lstRewardData, List<UnitLoyaltyUpdateData> lstUnitLoyaltyData)
	{
		m_lstResultDatas.Clear();
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			int count = lstRewardData.Count;
			for (int i = 0; i < count; i++)
			{
				NKCUIResult.BattleResultData battleResultData = new NKCUIResult.BattleResultData();
				battleResultData.m_lstUnitLevelupData = NKCUIResult.MakeUnitLevelupExpData(myUserData.m_ArmyData, lstRewardData[i].UnitExpDataList, NKMDeckIndex.None, lstUnitLoyaltyData);
				battleResultData.m_RewardData = lstRewardData[i];
				m_lstResultDatas.Add(battleResultData);
			}
		}
	}

	private void SetCountText(int currentCount, int fullCount)
	{
		if (currentCount == fullCount)
		{
			NKCUtil.SetGameobjectActive(m_objCount, bValue: false);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbCount, string.Format(NKCStringTable.GetString("SI_DP_OPERATION_SKIP_COUNT"), currentCount, fullCount));
		}
	}

	private void AnimateBgUV(float xOffset)
	{
		Vector2 position = m_imgBackground.uvRect.position;
		Vector2 size = m_imgBackground.uvRect.size;
		float x = Mathf.Repeat(position.x + xOffset, 1f);
		m_imgBackground.uvRect = new Rect(x, position.y, size.x, size.y);
	}

	private void SetShipImage()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null || myUserData.m_ArmyData == null || !NKCUIDeckViewer.IsInstanceOpen)
		{
			return;
		}
		NKMDeckIndex selectDeckIndex = NKCUIDeckViewer.Instance.GetSelectDeckIndex();
		NKMDeckData deckData = myUserData.m_ArmyData.GetDeckData(selectDeckIndex);
		if (deckData == null)
		{
			return;
		}
		NKMUnitData shipFromUID = myUserData.m_ArmyData.GetShipFromUID(deckData.m_ShipUID);
		if (shipFromUID != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shipFromUID.m_UnitID);
			Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
			if (!(sprite == null))
			{
				NKCUtil.SetImageSprite(m_imgShip, sprite);
			}
		}
	}

	private void AddToRewardSlotDataList(NKCUISlot.SlotData slotData)
	{
		if (slotData.ID <= 0)
		{
			return;
		}
		if (!m_dicRewardSlotData.ContainsKey(slotData.ID))
		{
			m_dicRewardSlotData.Add(slotData.ID, new List<NKCUISlot.SlotData>());
		}
		switch (slotData.eType)
		{
		case NKCUISlot.eSlotMode.ItemMisc:
		case NKCUISlot.eSlotMode.UnitCount:
			if (m_dicRewardSlotData[slotData.ID].Count > 0)
			{
				m_dicRewardSlotData[slotData.ID][0].Count += slotData.Count;
				m_hsShakeSlotID.Add(slotData.ID);
			}
			else
			{
				m_dicRewardSlotData[slotData.ID].Add(slotData);
			}
			break;
		case NKCUISlot.eSlotMode.Mold:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(slotData.ID);
			if (itemMoldTempletByID != null && !itemMoldTempletByID.m_bPermanent)
			{
				if (m_dicRewardSlotData[slotData.ID].Count > 0)
				{
					m_dicRewardSlotData[slotData.ID][0].Count += slotData.Count;
					m_hsShakeSlotID.Add(slotData.ID);
				}
				else
				{
					m_dicRewardSlotData[slotData.ID].Add(slotData);
				}
			}
			else
			{
				m_dicRewardSlotData[slotData.ID].Add(slotData);
			}
			break;
		}
		default:
			m_dicRewardSlotData[slotData.ID].Add(slotData);
			break;
		}
	}

	private RectTransform GetRewardSlot(int index)
	{
		RectTransform result = null;
		NKCUISlot newInstance = NKCUISlot.GetNewInstance(null);
		if (newInstance != null)
		{
			newInstance.Init();
			result = newInstance.GetComponent<RectTransform>();
		}
		return result;
	}

	private void ReturnRewardSlot(Transform tr)
	{
		NKCUISlot component = tr.GetComponent<NKCUISlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.CleanUp();
		}
		Object.Destroy(tr.gameObject);
	}

	private void ProvideRewardData(Transform tr, int index)
	{
		NKCUISlot component = tr.GetComponent<NKCUISlot>();
		if (component == null)
		{
			return;
		}
		if (index < m_lstRewardSortedSlotData.Count)
		{
			NKCUtil.SetGameobjectActive(component, bValue: true);
			if (m_lstRewardSortedSlotData[index].eType == NKCUISlot.eSlotMode.ItemMisc)
			{
				component.SetData(m_lstRewardSortedSlotData[index], bEnableLayoutElement: true, OnClickMiscItemIcon);
			}
			else if (m_lstRewardSortedSlotData[index].eType == NKCUISlot.eSlotMode.Equip)
			{
				component.SetData(m_lstRewardSortedSlotData[index], bEnableLayoutElement: true, OnClickEquipIcon);
			}
			else
			{
				component.SetData(m_lstRewardSortedSlotData[index]);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
	}

	private void RepopulateScrollRect()
	{
		NKCUtil.CalculateContentRectSize(m_loopScrollRect, m_gridLayoutGroup, m_iColumnMinCount, m_gridLayoutGroup.cellSize, m_gridLayoutGroup.spacing);
	}

	private void OnClickMiscItemIcon(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData, null, singleOpenOnly: false, bShowCount: false, showDropInfo: false);
	}

	private void OnClickEquipIcon(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(slotData.UID);
		if (itemEquip == null)
		{
			itemEquip = NKCEquipSortSystem.MakeTempEquipData(slotData.ID, slotData.GroupID);
			NKCPopupItemEquipBox.Open(itemEquip, NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
		else
		{
			NKCPopupItemEquipBox.Open(itemEquip, NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
	}

	private void RefreshRewardData(int resultDataIndex, bool refreshScrollRect = true)
	{
		if (resultDataIndex == 0)
		{
			m_dicRewardSlotData.Clear();
		}
		m_hsShakeSlotID.Clear();
		m_lstTweenSlot.Clear();
		if (resultDataIndex >= m_lstResultDatas.Count)
		{
			return;
		}
		NKCUIResult.BattleResultData battleResultData = m_lstResultDatas[resultDataIndex];
		if (battleResultData.m_iUnitExp > 0)
		{
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeMiscItemData(502, battleResultData.m_iUnitExp, battleResultData.m_iUnitExpBonusRate);
			AddToRewardSlotDataList(slotData);
		}
		if (battleResultData.m_firstRewardData != null)
		{
			NKCUISlot.MakeSlotDataListFromReward(battleResultData.m_firstRewardData).ForEach(AddToRewardSlotDataList);
		}
		if (battleResultData.m_firstAllClearData != null)
		{
			NKCUISlot.MakeSlotDataListFromReward(battleResultData.m_firstAllClearData).ForEach(AddToRewardSlotDataList);
		}
		if (battleResultData.m_OnetimeRewardData != null)
		{
			NKCUISlot.MakeSlotDataListFromReward(battleResultData.m_OnetimeRewardData).ForEach(AddToRewardSlotDataList);
		}
		if (battleResultData.m_RewardData != null)
		{
			NKCUISlot.MakeSlotDataListFromReward(battleResultData.m_RewardData).ForEach(AddToRewardSlotDataList);
		}
		if (battleResultData.m_additionalReward != null)
		{
			NKCUISlot.MakeSlotDataListFromReward(battleResultData.m_additionalReward).ForEach(AddToRewardSlotDataList);
		}
		if (!refreshScrollRect)
		{
			return;
		}
		m_lstRewardSortedSlotData.Clear();
		foreach (KeyValuePair<int, List<NKCUISlot.SlotData>> dicRewardSlotDatum in m_dicRewardSlotData)
		{
			List<NKCUISlot.SlotData> value = dicRewardSlotDatum.Value;
			int count = value.Count;
			for (int i = 0; i < count; i++)
			{
				m_lstRewardSortedSlotData.Add(value[i]);
			}
		}
		m_lstRewardSortedSlotData.Sort(delegate(NKCUISlot.SlotData e1, NKCUISlot.SlotData e2)
		{
			if (e1.eType > e2.eType)
			{
				return 1;
			}
			return (e1.eType < e2.eType) ? (-1) : 0;
		});
		m_loopScrollRect.TotalCount = m_lstRewardSortedSlotData.Count;
		m_loopScrollRect.StopMovement();
		m_loopScrollRect.RefreshCells();
		int childCount = m_loopScrollRect.content.childCount;
		for (int num = 0; num < childCount; num++)
		{
			Transform child = m_loopScrollRect.content.GetChild(num);
			if (!child.gameObject.activeSelf)
			{
				continue;
			}
			NKCUISlot component = child.GetComponent<NKCUISlot>();
			if (!(component == null))
			{
				int iD = component.GetSlotData().ID;
				if (m_hsShakeSlotID.Contains(iD))
				{
					component.transform.DOKill(complete: true);
					m_lstTweenSlot.Add(component.transform.DOShakePosition(m_fShakeItemDuration, new Vector3(m_fShakeItemStrength, m_fShakeItemStrength, 0f), m_iShakeItemVibration));
					m_lstTweenSlot.Add(component.transform.DOPunchScale(new Vector3(m_fPunchItemScale, m_fPunchItemScale, 0f), m_fPunchItemDuration));
				}
			}
		}
		m_hsShakeSlotID.Clear();
	}

	private void OnClickConfirm()
	{
		if (m_iFightCount < m_iFullCount)
		{
			m_eAniState = AniState.STOP;
			m_aniShip.Play(m_endAniName, -1, 1f);
			for (int i = m_iFightCount; i < m_iFullCount; i++)
			{
				RefreshRewardData(i, i == m_iFullCount - 1);
			}
			m_iFightCount = m_iFullCount;
			SetCountText(m_iFullCount, m_iFullCount);
			int count = m_lstTweenSlot.Count;
			for (int j = 0; j < count; j++)
			{
				if (m_lstTweenSlot[j] != null && m_lstTweenSlot[j].IsPlaying())
				{
					m_lstTweenSlot[j].Complete();
				}
			}
			return;
		}
		bool flag = true;
		int count2 = m_lstTweenSlot.Count;
		for (int k = 0; k < count2; k++)
		{
			if (m_lstTweenSlot[k] != null && m_lstTweenSlot[k].IsPlaying())
			{
				m_lstTweenSlot[k].Complete();
				flag = false;
			}
		}
		if (flag)
		{
			Close();
		}
	}

	private void OnClickRewardUI()
	{
		if (m_eAniState == AniState.FIGHT)
		{
			int num = (int)m_aniShip.GetCurrentAnimatorStateInfo(0).normalizedTime;
			if (m_aniShip.GetCurrentAnimatorStateInfo(0).normalizedTime - (float)num < m_fStepSkipAniTime)
			{
				m_aniShip.Play(m_fightAniName, 0, (float)num + m_fStepSkipAniTime);
			}
		}
	}

	private void OnDestroy()
	{
		m_NKCUIOpenAnimator = null;
		m_lstResultDatas?.Clear();
		m_lstResultDatas = null;
		m_dicRewardSlotData?.Clear();
		m_dicRewardSlotData = null;
		m_lstRewardSortedSlotData?.Clear();
		m_lstRewardSortedSlotData = null;
		m_hsShakeSlotID?.Clear();
		m_hsShakeSlotID = null;
		m_lstTweenSlot?.Clear();
		m_lstTweenSlot = null;
	}
}
