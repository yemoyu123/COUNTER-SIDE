using System;
using NKM;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public abstract class NKCUIUnitSelectListSlotBase : MonoBehaviour
{
	public enum eUnitSlotMode
	{
		Character,
		Empty,
		Denied,
		SelectResource,
		Closed,
		ClearAll,
		AutoComplete,
		Add,
		Random
	}

	public delegate void OnSelectThisSlot(NKMUnitData unitData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState);

	public delegate void OnSelectThisOperatorSlot(NKMOperator operatorData, NKMUnitTempletBase unitTempletBase, NKMDeckIndex deckIndex, NKCUnitSortSystem.eUnitState slotState, NKCUIUnitSelectList.eUnitSlotSelectState unitSlotSelectState);

	protected NKMUnitData m_NKMUnitData;

	private int m_UnitID;

	private int m_SkinID;

	protected NKMDeckIndex m_DeckIndex;

	[NonSerialized]
	protected NKMUnitTempletBase m_NKMUnitTempletBase;

	protected NKMOperator m_OperatorData;

	private int m_OperatorID;

	public NKCUIComButton m_cbtnSlot;

	[Header("슬롯 상태 오브젝트")]
	public GameObject m_objCardRoot;

	public GameObject m_objSlotStatus;

	public Image m_imgSlotStatus;

	public Sprite m_spEmpty;

	public Sprite m_spDenied;

	public Sprite m_spClosed;

	public Sprite m_spSelectResource;

	public Sprite m_spAdd;

	public Sprite m_spRandom;

	[Header("배경")]
	public Image m_imgBG;

	public Sprite m_spBG_N;

	public Sprite m_spBG_R;

	public Sprite m_spBG_SR;

	public Sprite m_spBG_SSR;

	[Header("레어리티")]
	public Image m_imgRarity;

	public Sprite m_spSSR;

	public Sprite m_spSR;

	public Sprite m_spR;

	public Sprite m_spN;

	[Header("유닛 기본 정보")]
	public Image m_imgUnitPortrait;

	public Text m_lbName;

	public Text m_lbLevel;

	public GameObject m_objLevelInfo;

	public GameObject m_objMaxExp;

	public Image m_imgAttackType;

	public GameObject m_objSourceType;

	public Image m_imgSourceType;

	public Image m_imgSourceTypeSub;

	public NKCUIComStarRank m_comStarRank;

	public GameObject m_objRearm;

	[Header("부대 번호")]
	public GameObject m_objShipNumberRoot;

	public Text m_lbShipNumber;

	[Header("사옥 번호")]
	public GameObject m_objOfficeNumberRoot;

	public Text m_lbOfficeNumber;

	public LayoutElement m_layoutElement;

	[Header("선택 표시 관련")]
	public GameObject m_objBusyRoot;

	public Text m_lbBusyText;

	public GameObject m_objBusyDisable;

	public GameObject m_objInCityMission;

	public Text m_lbMissionStatus;

	public GameObject m_objChecked;

	public GameObject m_objLeagueBanned;

	public GameObject m_objLeaguePicked;

	public Image m_imgLeaguePicked;

	public Color m_colorLeaguePickedLeft;

	public Color m_colorLeaguePickedRight;

	public Image m_imgUsedIcon;

	public GameObject m_objSeized;

	public Sprite m_spUsedCity;

	public Sprite m_spUsedSeized;

	[Header("선택된 하이라이트")]
	public GameObject m_objSelectedSlotHighlight;

	public GameObject m_objSelectedSlotHighlightCastingBan;

	[Header("더 이상 선택안됨 표시")]
	public GameObject m_objDisableSelectSlot;

	[Header("잠김/삭제 마크")]
	public GameObject m_objLocked;

	public GameObject m_objLockBig;

	public GameObject m_objDelete;

	[Header("즐겨찾기")]
	public GameObject m_objFavorite;

	[Header("New 마크")]
	public GameObject m_objNew;

	[Header("정렬 기준/수치")]
	public GameObject m_objSortingType;

	public Text m_lbSortingType;

	public Text m_lbSortingValue;

	[Header("보유 확인")]
	public NKCUIComStateButton m_btnHave;

	[Header("보유 갯수")]
	public GameObject m_objHaveCount;

	public Text m_lbHaveCount;

	[Header("밴 표시")]
	public GameObject m_objBan;

	public Text m_lbBanLevel;

	public Text m_lbBanApplyDesc;

	[Header("레벨 표시")]
	public GameObject m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT_LEVEL;

	[Header("각성 애니메이션")]
	public Animator m_animAwakenFX;

	[Header("격전지원")]
	public GameObject m_NKM_UI_UNIT_SELECT_LIST_FIERCE_BATTLE;

	protected OnSelectThisSlot dOnSelectThisSlot;

	protected OnSelectThisOperatorSlot dOnSelectThisOperatorSlot;

	private int m_iPowerCache;

	protected NKCUnitSortSystem.eUnitState m_eUnitSlotState;

	protected NKCUIUnitSelectList.eUnitSlotSelectState m_eUnitSelectState;

	protected bool m_bEnableShowBan;

	protected NKCBanManager.BAN_DATA_TYPE m_eBanDataType = NKCBanManager.BAN_DATA_TYPE.FINAL;

	protected bool m_bEnableShowUpUnit;

	protected bool m_bEnableShowCastingBan;

	public NKMUnitData NKMUnitData => m_NKMUnitData;

	public NKMUnitTempletBase NKMUnitTempletBase => m_NKMUnitTempletBase;

	public NKMOperator NKMOperatorData => m_OperatorData;

	protected virtual NKCResourceUtility.eUnitResourceType UseResourceType => NKCResourceUtility.eUnitResourceType.FACE_CARD;

	public int PowerCache => m_iPowerCache;

	public NKCUIUnitSelectList.eUnitSlotSelectState UnitSelectState => m_eUnitSelectState;

	public void SetEnableShowBan(bool bSet)
	{
		m_bEnableShowBan = bSet;
		if (!bSet)
		{
			NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		}
	}

	public void SetEnableLeagueBan(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objLeagueBanned, bSet);
	}

	public void SetBanDataType(NKCBanManager.BAN_DATA_TYPE BanDataType)
	{
		m_eBanDataType = BanDataType;
	}

	public void SetEnableShowUpUnit(bool bSet)
	{
		m_bEnableShowUpUnit = bSet;
		if (!bSet)
		{
			NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		}
	}

	public void SetEnableShowCastingBanSelectedObject(bool bSet)
	{
		m_bEnableShowCastingBan = bSet;
	}

	public void Init(bool resetLocalScale = false)
	{
		if (resetLocalScale)
		{
			base.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
		}
		m_cbtnSlot.PointerClick.RemoveAllListeners();
		m_cbtnSlot.PointerClick.AddListener(OnClick);
		if (m_btnHave != null)
		{
			m_btnHave.PointerClick.RemoveAllListeners();
			m_btnHave.PointerClick.AddListener(OnClickHave);
		}
	}

	protected virtual void SetMode(eUnitSlotMode mode)
	{
		NKCUtil.SetGameobjectActive(m_objCardRoot, mode == eUnitSlotMode.Character);
		NKCUtil.SetGameobjectActive(m_objSlotStatus, mode != eUnitSlotMode.Character);
		if (mode != eUnitSlotMode.Character)
		{
			SetLock(bLocked: false, bBig: true);
			SetFavorite(bFavorite: false);
		}
		if (m_imgSlotStatus != null)
		{
			switch (mode)
			{
			case eUnitSlotMode.Closed:
				m_imgSlotStatus.sprite = m_spClosed;
				break;
			case eUnitSlotMode.Empty:
				m_imgSlotStatus.sprite = m_spEmpty;
				break;
			case eUnitSlotMode.SelectResource:
				m_imgSlotStatus.sprite = m_spSelectResource;
				break;
			case eUnitSlotMode.Denied:
				m_imgSlotStatus.sprite = m_spDenied;
				break;
			case eUnitSlotMode.Add:
				m_imgSlotStatus.sprite = m_spAdd;
				break;
			case eUnitSlotMode.Random:
				m_imgSlotStatus.sprite = m_spRandom;
				break;
			}
		}
		if (mode != eUnitSlotMode.Character)
		{
			SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState.NONE);
			SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
		}
	}

	protected Sprite GetBGSprite(NKM_UNIT_GRADE unitGrade)
	{
		return unitGrade switch
		{
			NKM_UNIT_GRADE.NUG_R => m_spBG_R, 
			NKM_UNIT_GRADE.NUG_SR => m_spBG_SR, 
			NKM_UNIT_GRADE.NUG_SSR => m_spBG_SSR, 
			_ => m_spBG_N, 
		};
	}

	protected Sprite GetRaritySprite(NKM_UNIT_GRADE unitGrade)
	{
		return unitGrade switch
		{
			NKM_UNIT_GRADE.NUG_R => m_spR, 
			NKM_UNIT_GRADE.NUG_SR => m_spSR, 
			NKM_UNIT_GRADE.NUG_SSR => m_spSSR, 
			_ => m_spN, 
		};
	}

	public virtual void SetData(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, int officeID = 0)
	{
		RestoreSprite();
		dOnSelectThisSlot = onSelectThisSlot;
		bool flag = m_NKMUnitData == null || cNKMUnitData == null || m_NKMUnitData.m_UnitUID != cNKMUnitData.m_UnitUID || cNKMUnitData.m_UnitID != m_UnitID || m_SkinID != cNKMUnitData.m_SkinID;
		m_NKMUnitData = cNKMUnitData;
		SetMode(eUnitSlotMode.Character);
		if (cNKMUnitData != null)
		{
			m_UnitID = cNKMUnitData.m_UnitID;
			m_SkinID = cNKMUnitData.m_SkinID;
			m_NKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData.m_UnitID);
			if (flag)
			{
				SetTempletData(m_NKMUnitTempletBase);
				Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(UseResourceType, m_NKMUnitData);
				if (sprite != null)
				{
					m_imgUnitPortrait.sprite = sprite;
					m_imgUnitPortrait.enabled = true;
				}
				else
				{
					m_imgUnitPortrait.enabled = false;
				}
			}
			if (m_lbLevel != null)
			{
				NKCUIComTextUnitLevel nKCUIComTextUnitLevel = m_lbLevel as NKCUIComTextUnitLevel;
				if (nKCUIComTextUnitLevel != null)
				{
					nKCUIComTextUnitLevel.SetLevel(cNKMUnitData, 0);
				}
				else
				{
					m_lbLevel.text = cNKMUnitData.m_UnitLevel.ToString();
				}
			}
			NKCUtil.SetGameobjectActive(m_objMaxExp, bValue: false);
			Sprite orLoadUnitRoleAttackTypeIcon = NKCResourceUtility.GetOrLoadUnitRoleAttackTypeIcon(cNKMUnitData, bSmall: true);
			NKCUtil.SetImageSprite(m_imgAttackType, orLoadUnitRoleAttackTypeIcon, bDisableIfSpriteNull: true);
			NKCUtil.SetGameobjectActive(m_objSourceType, NKMOpenTagManager.IsOpened("UNIT_SOURCE_TYPE") && m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
			if (m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				Sprite orLoadUnitSourceTypeIcon = NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE, bSmall: true);
				NKCUtil.SetImageSprite(m_imgSourceType, orLoadUnitSourceTypeIcon, bDisableIfSpriteNull: true);
				Sprite orLoadUnitSourceTypeIcon2 = NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB, bSmall: true);
				NKCUtil.SetImageSprite(m_imgSourceTypeSub, orLoadUnitSourceTypeIcon2, bDisableIfSpriteNull: true);
			}
			SetDeckIndex(deckIndex);
			SetOfficeRoomID(officeID);
			if (NKCScenManager.CurrentUserData() != null)
			{
				m_iPowerCache = m_NKMUnitData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData);
			}
			SetLock(m_NKMUnitData.m_bLock);
			SetFavorite(m_NKMUnitData);
		}
		m_cbtnSlot.Select(bSelect: false, bForce: false, bImmediate: true);
		if (m_layoutElement != null)
		{
			m_layoutElement.enabled = bEnableLayoutElement;
		}
		m_eUnitSlotState = NKCUnitSortSystem.eUnitState.NONE;
		m_eUnitSelectState = NKCUIUnitSelectList.eUnitSlotSelectState.NONE;
		NKCUtil.SetGameobjectActive(m_objSelectedSlotHighlight, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSelectedSlotHighlightCastingBan, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDisableSelectSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNew, bValue: false);
		SetContractedUnitMark(value: false);
	}

	public virtual void SetData(NKMOperator operatorData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisOperatorSlot onSelectThisSlot)
	{
		RestoreSprite();
		dOnSelectThisOperatorSlot = onSelectThisSlot;
		bool flag = m_OperatorData == null || operatorData == null || m_OperatorData.uid != operatorData.uid || m_OperatorData.id != m_OperatorID;
		m_OperatorData = operatorData;
		SetMode(eUnitSlotMode.Character);
		if (operatorData != null)
		{
			m_OperatorID = operatorData.id;
			m_NKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
			if (flag)
			{
				SetTempletData(m_NKMUnitTempletBase);
				Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(UseResourceType, operatorData);
				if (sprite != null)
				{
					m_imgUnitPortrait.sprite = sprite;
					m_imgUnitPortrait.enabled = true;
				}
				else
				{
					m_imgUnitPortrait.enabled = false;
				}
			}
			if (m_lbLevel != null)
			{
				m_lbLevel.text = operatorData.level.ToString();
			}
			NKCUtil.SetGameobjectActive(m_objMaxExp, bValue: false);
			SetDeckIndex(deckIndex);
			if (NKCScenManager.CurrentUserData() != null)
			{
				m_iPowerCache = operatorData.CalculateOperatorOperationPower();
			}
			SetLock(operatorData.bLock);
			SetFavorite(operatorData);
		}
		m_cbtnSlot.Select(bSelect: false, bForce: false, bImmediate: true);
		if (m_layoutElement != null)
		{
			m_layoutElement.enabled = bEnableLayoutElement;
		}
		m_eUnitSlotState = NKCUnitSortSystem.eUnitState.NONE;
		m_eUnitSelectState = NKCUIUnitSelectList.eUnitSlotSelectState.NONE;
		NKCUtil.SetGameobjectActive(m_objSelectedSlotHighlight, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSelectedSlotHighlightCastingBan, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDisableSelectSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNew, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
		SetContractedUnitMark(value: false);
	}

	public void SetData(NKMUnitTempletBase templetBase, int levelToDisplay, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot)
	{
		SetData(templetBase, levelToDisplay, 0, bEnableLayoutElement, onSelectThisSlot);
	}

	public void SetOperatorData(NKMUnitTempletBase templetBase, int levelToDisplay, bool bEnableLayoutElement, OnSelectThisOperatorSlot onSelectThisSlot)
	{
		SetData(templetBase, levelToDisplay, 0, bEnableLayoutElement, onSelectThisSlot);
	}

	public virtual void SetData(NKMUnitTempletBase templetBase, int levelToDisplay, int skinID, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot)
	{
		dOnSelectThisSlot = onSelectThisSlot;
		SetData(templetBase, levelToDisplay, skinID, bEnableLayoutElement);
	}

	public virtual void SetData(NKMUnitTempletBase templetBase, int levelToDisplay, int skinID, bool bEnableLayoutElement, OnSelectThisOperatorSlot onSelectThisSlot)
	{
		dOnSelectThisOperatorSlot = onSelectThisSlot;
		SetData(templetBase, levelToDisplay, skinID, bEnableLayoutElement);
	}

	private void SetData(NKMUnitTempletBase templetBase, int levelToDisplay, int skinID, bool bEnableLayoutElement)
	{
		RestoreSprite();
		m_NKMUnitData = null;
		m_OperatorData = null;
		SetMode(eUnitSlotMode.Character);
		if (templetBase != null)
		{
			m_UnitID = templetBase.m_UnitID;
			m_SkinID = skinID;
			bool num = m_NKMUnitTempletBase == null || m_NKMUnitTempletBase.m_UnitID != templetBase.m_UnitID || m_SkinID != skinID;
			m_NKMUnitTempletBase = templetBase;
			if (num)
			{
				SetTempletData(m_NKMUnitTempletBase);
				if (m_imgUnitPortrait != null)
				{
					Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(UseResourceType, templetBase.m_UnitID, skinID);
					if (sprite != null)
					{
						m_imgUnitPortrait.sprite = sprite;
						m_imgUnitPortrait.enabled = true;
					}
					else
					{
						m_imgUnitPortrait.enabled = false;
					}
				}
			}
			if (m_lbLevel != null)
			{
				NKCUIComTextUnitLevel nKCUIComTextUnitLevel = m_lbLevel as NKCUIComTextUnitLevel;
				if (nKCUIComTextUnitLevel != null)
				{
					nKCUIComTextUnitLevel.SetText(levelToDisplay.ToString(), 0);
				}
				else
				{
					m_lbLevel.text = levelToDisplay.ToString();
				}
				if (m_lbLevel.transform.parent != null)
				{
					NKCUtil.SetGameobjectActive(m_lbLevel.transform.parent, levelToDisplay > 0);
				}
			}
			NKCUtil.SetGameobjectActive(m_objMaxExp, bValue: false);
			Sprite orLoadUnitRoleAttackTypeIcon = NKCResourceUtility.GetOrLoadUnitRoleAttackTypeIcon(templetBase, bSmall: true);
			NKCUtil.SetImageSprite(m_imgAttackType, orLoadUnitRoleAttackTypeIcon, bDisableIfSpriteNull: true);
			NKCUtil.SetGameobjectActive(m_objSourceType, m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE);
			if (m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
			{
				Sprite orLoadUnitSourceTypeIcon = NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE, bSmall: true);
				NKCUtil.SetImageSprite(m_imgSourceType, orLoadUnitSourceTypeIcon, bDisableIfSpriteNull: true);
				Sprite orLoadUnitSourceTypeIcon2 = NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(m_NKMUnitTempletBase.m_NKM_UNIT_SOURCE_TYPE_SUB, bSmall: true);
				NKCUtil.SetImageSprite(m_imgSourceTypeSub, orLoadUnitSourceTypeIcon2, bDisableIfSpriteNull: true);
			}
			m_comStarRank?.SetStarRank(templetBase, levelToDisplay);
			SetDeckIndex(new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE));
			m_iPowerCache = 0;
		}
		SetLock(bLocked: false);
		SetFavorite(bFavorite: false);
		m_cbtnSlot.Select(bSelect: false, bForce: false, bImmediate: true);
		if (m_layoutElement != null)
		{
			m_layoutElement.enabled = bEnableLayoutElement;
		}
		m_eUnitSlotState = NKCUnitSortSystem.eUnitState.NONE;
		m_eUnitSelectState = NKCUIUnitSelectList.eUnitSlotSelectState.NONE;
		NKCUtil.SetGameobjectActive(m_objSelectedSlotHighlight, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSelectedSlotHighlightCastingBan, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDisableSelectSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNew, bValue: false);
		SetContractedUnitMark(value: false);
	}

	public virtual void SetDataForBan(NKMUnitTempletBase templetBase, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, bool bUp = false, bool bSetOriginalCost = false)
	{
	}

	public virtual void SetDataForBan(NKMOperator operData, bool bEnableLayoutElement, OnSelectThisOperatorSlot onSelectThisSlot)
	{
	}

	public virtual void SetEquipData(NKMEquipmentSet equipSet)
	{
	}

	public virtual void SetDataForContractSelection(NKMUnitData cNKMUnitData, bool bHave = true)
	{
	}

	public virtual void SetDataForContractSelection(NKMOperator cNKMOperData)
	{
	}

	public virtual void SetDataForCollection(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, OnSelectThisSlot onSelectThisSlot, bool bEnable = false)
	{
	}

	public virtual void SetDataForCollection(NKMOperator cNKMUnitData, NKMDeckIndex deckIndex, OnSelectThisOperatorSlot onSelectThisSlot, bool bEnable = false)
	{
	}

	public virtual void SetDataForRearm(NKMUnitData unitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, bool bShowEqup = true, bool bShowLevel = false, bool bUnable = false)
	{
	}

	public virtual void SetDataForDummyUnit(NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, int officeID = 0)
	{
	}

	protected void ProcessBanUIForUnit()
	{
		if (m_NKMUnitTempletBase != null)
		{
			if (m_bEnableShowBan && NKCBanManager.IsBanUnit(m_NKMUnitTempletBase.m_UnitID, m_eBanDataType))
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
				int unitBanLevel = NKCBanManager.GetUnitBanLevel(m_NKMUnitTempletBase.m_UnitID, m_eBanDataType);
				NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, unitBanLevel));
				m_lbBanLevel.color = Color.red;
			}
			else if (m_bEnableShowUpUnit && NKCBanManager.IsUpUnit(m_NKMUnitTempletBase.m_UnitID))
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
				int unitUpLevel = NKCBanManager.GetUnitUpLevel(m_NKMUnitTempletBase.m_UnitID);
				NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_UP_LEVEL_ONE_PARAM, unitUpLevel));
				m_lbBanLevel.color = NKCBanManager.UP_COLOR;
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
			}
		}
	}

	protected void ProcessBanUIForOperator()
	{
		if (m_NKMUnitTempletBase != null)
		{
			if (m_bEnableShowBan && NKCBanManager.IsBanOperator(m_NKMUnitTempletBase.m_UnitID, m_eBanDataType))
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: true);
				int operBanLevel = NKCBanManager.GetOperBanLevel(m_NKMUnitTempletBase.m_UnitID, m_eBanDataType);
				NKCUtil.SetLabelText(m_lbBanLevel, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, operBanLevel));
				m_lbBanLevel.color = Color.red;
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objBan, bValue: false);
			}
		}
	}

	protected virtual void SetTempletData(NKMUnitTempletBase templetBase)
	{
		if (m_imgBG != null)
		{
			m_imgBG.sprite = GetBGSprite(templetBase.m_NKM_UNIT_GRADE);
		}
		if (m_imgRarity != null)
		{
			NKCUtil.SetGameobjectActive(m_imgRarity, bValue: true);
			m_imgRarity.sprite = GetRaritySprite(templetBase.m_NKM_UNIT_GRADE);
		}
		NKCUtil.SetGameobjectActive(m_objRearm, m_NKMUnitTempletBase.IsRearmUnit);
		NKCUtil.SetAwakenFX(m_animAwakenFX, templetBase);
		NKCUtil.SetLabelText(m_lbName, templetBase.GetUnitName());
		SetEnableLevelInfo(!templetBase.IsTrophy);
	}

	public void SetDeckIndex(NKMDeckIndex deckIndex)
	{
		m_DeckIndex = deckIndex;
		NKCUtil.SetGameobjectActive(m_objShipNumberRoot, deckIndex.m_eDeckType != NKM_DECK_TYPE.NDT_NONE);
		NKCUtil.SetLabelText(m_lbShipNumber, NKCUtilString.GetDeckNumberString(deckIndex));
	}

	public void SetOfficeRoomID(int roomID)
	{
		if (roomID != 0)
		{
			NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(roomID);
			if (nKMOfficeRoomTemplet != null)
			{
				NKCUtil.SetLabelText(m_lbOfficeNumber, NKCStringTable.GetString(nKMOfficeRoomTemplet.Name));
			}
			NKCUtil.SetGameobjectActive(m_objShipNumberRoot, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objOfficeNumberRoot, roomID != 0);
	}

	public virtual void SetLock(bool bLocked, bool bBig = false)
	{
		NKCUtil.SetGameobjectActive(m_objLocked, bLocked);
		NKCUtil.SetGameobjectActive(m_objLockBig, bLocked && bBig);
	}

	public virtual void SetFavorite(NKMUnitData unitData)
	{
		SetFavorite(unitData?.isFavorite ?? false);
	}

	public virtual void SetFavorite(NKMOperator operatorData)
	{
		SetFavorite(bFavorite: false);
	}

	public virtual void SetFavorite(bool bFavorite)
	{
		NKCUtil.SetGameobjectActive(m_objFavorite, bFavorite);
	}

	public virtual void SetDelete(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objDelete, bSet);
	}

	public void SetChecked(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objChecked, bSet);
	}

	public void SetMode(eUnitSlotMode mode, bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, OnSelectThisOperatorSlot onSelectThisOperatorSlot = null)
	{
		if (mode == eUnitSlotMode.Character)
		{
			Debug.LogWarning("Dont's use Setmode(Character), use SetData instead");
			return;
		}
		m_NKMUnitData = null;
		m_NKMUnitTempletBase = null;
		m_OperatorData = null;
		m_DeckIndex = NKMDeckIndex.None;
		SetMode(mode);
		dOnSelectThisSlot = onSelectThisSlot;
		dOnSelectThisOperatorSlot = onSelectThisOperatorSlot;
		if (m_layoutElement != null)
		{
			m_layoutElement.enabled = bEnableLayoutElement;
		}
		ClearTouchHoldEvent();
		NKCUtil.SetAwakenFX(m_animAwakenFX, null);
	}

	public void SetMode(eUnitSlotMode mode, bool bEnableLayoutElement, OnSelectThisOperatorSlot onSelectThisSlot)
	{
		if (mode == eUnitSlotMode.Character)
		{
			Debug.LogWarning("Dont's use Setmode(Character), use SetData instead");
			return;
		}
		m_NKMUnitData = null;
		m_OperatorData = null;
		m_DeckIndex = NKMDeckIndex.None;
		SetMode(mode);
		dOnSelectThisOperatorSlot = onSelectThisSlot;
		if (m_layoutElement != null)
		{
			m_layoutElement.enabled = bEnableLayoutElement;
		}
		ClearTouchHoldEvent();
		NKCUtil.SetAwakenFX(m_animAwakenFX, null);
	}

	public void SetClosed(bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot)
	{
		SetMode(eUnitSlotMode.Closed, bEnableLayoutElement, onSelectThisSlot);
	}

	public void SetRandom(bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot)
	{
		SetMode(eUnitSlotMode.Random, bEnableLayoutElement, onSelectThisSlot);
	}

	public virtual void SetEmpty(bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot, OnSelectThisOperatorSlot onSelectThisOperatorSlot = null)
	{
		SetMode(eUnitSlotMode.Empty, bEnableLayoutElement, onSelectThisSlot, onSelectThisOperatorSlot);
	}

	public void SetDenied(bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot)
	{
		SetMode(eUnitSlotMode.Denied, bEnableLayoutElement, onSelectThisSlot);
	}

	public void SetSelectResource(bool bEnableLayoutElement, OnSelectThisSlot onSelectThisSlot)
	{
		SetMode(eUnitSlotMode.SelectResource, bEnableLayoutElement, onSelectThisSlot);
	}

	public virtual void SetSlotSelectState(NKCUIUnitSelectList.eUnitSlotSelectState eUnitSelectState)
	{
		m_eUnitSelectState = eUnitSelectState;
		NKCUtil.SetGameobjectActive(m_objSelectedSlotHighlight, !m_bEnableShowCastingBan && m_eUnitSelectState == NKCUIUnitSelectList.eUnitSlotSelectState.SELECTED);
		NKCUtil.SetGameobjectActive(m_objSelectedSlotHighlightCastingBan, m_bEnableShowCastingBan && m_eUnitSelectState == NKCUIUnitSelectList.eUnitSlotSelectState.SELECTED);
		NKCUtil.SetGameobjectActive(m_objDisableSelectSlot, m_eUnitSelectState == NKCUIUnitSelectList.eUnitSlotSelectState.DISABLE);
		NKCUtil.SetGameobjectActive(m_objDelete, m_eUnitSelectState == NKCUIUnitSelectList.eUnitSlotSelectState.DELETE);
	}

	public virtual void SetSlotState(NKCUnitSortSystem.eUnitState eUnitSlotState)
	{
		m_eUnitSlotState = eUnitSlotState;
		switch (m_eUnitSlotState)
		{
		case NKCUnitSortSystem.eUnitState.MAINUNIT:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBusyDisable, bValue: true);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			NKCUtil.SetLabelText(m_lbBusyText, NKCUtilString.GET_STRING_DECK_UNIT_STATE_MAINUNIT);
			break;
		case NKCUnitSortSystem.eUnitState.DECKED:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBusyDisable, bValue: true);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			NKCUtil.SetLabelText(m_lbBusyText, NKCUtilString.GET_STRING_DECK_UNIT_STATE_DECKED);
			break;
		case NKCUnitSortSystem.eUnitState.LOCKED:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBusyDisable, bValue: true);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			NKCUtil.SetLabelText(m_lbBusyText, NKCUtilString.GET_STRING_DECK_UNIT_STATE_LOCKED);
			break;
		case NKCUnitSortSystem.eUnitState.DUPLICATE:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBusyDisable, bValue: true);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			NKCUtil.SetLabelText(m_lbBusyText, NKCUtilString.GET_STRING_UNIT_SELECT_IMPOSSIBLE_DUPLICATE_ORGANIZE);
			break;
		case NKCUnitSortSystem.eUnitState.CITY_SET:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSeized, bValue: false);
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_WORLDMAP_CITY_LEADER);
			NKCUtil.SetImageSprite(m_imgUsedIcon, m_spUsedCity);
			break;
		case NKCUnitSortSystem.eUnitState.CITY_MISSION:
		case NKCUnitSortSystem.eUnitState.WARFARE_BATCH:
		case NKCUnitSortSystem.eUnitState.DIVE_BATCH:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSeized, bValue: false);
			NKCUtil.SetLabelText(m_lbMissionStatus, NKCUtilString.GET_STRING_WORLDMAP_CITY_MISSION_DOING);
			NKCUtil.SetImageSprite(m_imgUsedIcon, m_spUsedCity);
			break;
		case NKCUnitSortSystem.eUnitState.SEIZURE:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSeized, bValue: true);
			NKCUtil.SetLabelText(m_lbMissionStatus, "");
			NKCUtil.SetImageSprite(m_imgUsedIcon, m_spUsedSeized);
			break;
		case NKCUnitSortSystem.eUnitState.LOBBY_UNIT:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBusyDisable, bValue: false);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			NKCUtil.SetLabelText(m_lbBusyText, NKCUtilString.GET_STRING_LOBBY_UNIT_CAPTAIN);
			break;
		case NKCUnitSortSystem.eUnitState.DUNGEON_RESTRICTED:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBusyDisable, bValue: true);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			NKCUtil.SetLabelText(m_lbBusyText, NKCStringTable.GetString("SI_DP_DECK_UNIT_STATE_CANNOT_USE"));
			break;
		case NKCUnitSortSystem.eUnitState.LEAGUE_BANNED:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objBusyDisable, bValue: false);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLeagueBanned, bValue: true);
			NKCUtil.SetGameobjectActive(m_objLeaguePicked, bValue: false);
			NKCUtil.SetLabelText(m_lbBusyText, "");
			break;
		case NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_LEFT:
		case NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_RIGHT:
		{
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objBusyDisable, bValue: false);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLeagueBanned, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLeaguePicked, bValue: true);
			NKCUtil.SetLabelText(m_lbBusyText, "");
			Color color = ((m_eUnitSlotState == NKCUnitSortSystem.eUnitState.LEAGUE_DECKED_LEFT) ? m_colorLeaguePickedLeft : m_colorLeaguePickedRight);
			NKCUtil.SetImageColor(m_imgLeaguePicked, color);
			break;
		}
		case NKCUnitSortSystem.eUnitState.OFFICE_DORM_IN:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBusyDisable, bValue: true);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			NKCUtil.SetLabelText(m_lbBusyText, NKCUtilString.GET_STRING_OFFICE_ROOM_IN);
			break;
		default:
			NKCUtil.SetGameobjectActive(m_objBusyRoot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objInCityMission, bValue: false);
			break;
		}
		NKCUtil.SetGameobjectActive(m_objChecked, m_eUnitSlotState == NKCUnitSortSystem.eUnitState.CHECKED);
	}

	public virtual void SetCityLeaderMark(bool value)
	{
	}

	public virtual void SetCityMissionStatus(bool value)
	{
	}

	public void SetNewMark(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objNew, value);
	}

	public void SetNameColor(Color color)
	{
		if (m_lbName != null)
		{
			m_lbName.color = color;
		}
	}

	public void SetHaveCount(int count, bool bShowBtn = true)
	{
		if (m_lbHaveCount != null)
		{
			if (count > 0)
			{
				NKCUtil.SetGameobjectActive(m_objHaveCount, bValue: true);
				NKCUtil.SetLabelText(m_lbHaveCount, count.ToString());
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objHaveCount, bValue: false);
			}
		}
		if (m_btnHave != null)
		{
			NKCUtil.SetGameobjectActive(m_btnHave, bShowBtn && count > 0);
		}
	}

	public void SetSortingTypeValue(bool bSet, NKCUnitSortSystem.eSortOption sortOption, int sortValue = 0, string format = "")
	{
		NKCUtil.SetGameobjectActive(m_objSortingType, bSet);
		if (bSet)
		{
			m_lbSortingType.text = GetSortName(sortOption);
			if (m_NKMUnitTempletBase != null && m_NKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR && NKCOperatorUtil.IsPercentageStat(sortOption))
			{
				m_lbSortingValue.text = NKCOperatorUtil.GetStatPercentageString(sortValue);
			}
			else
			{
				m_lbSortingValue.text = sortValue.ToString(format);
			}
		}
	}

	public void SetSortingTypeValue(bool bSet, NKCOperatorSortSystem.eSortOption sortOption = NKCOperatorSortSystem.eSortOption.Level_High, int sortValue = 0, string format = "")
	{
		NKCUtil.SetGameobjectActive(m_objSortingType, bSet);
		if (bSet)
		{
			m_lbSortingType.text = GetSortName(sortOption);
			if (m_NKMUnitTempletBase != null && m_NKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR && NKCOperatorUtil.IsPercentageStat(sortOption))
			{
				m_lbSortingValue.text = NKCOperatorUtil.GetStatPercentageString(sortValue);
			}
			else
			{
				m_lbSortingValue.text = sortValue.ToString(format);
			}
		}
	}

	public void SetSortingTypeValue(bool bSet, NKCUnitSortSystem.eSortOption sortOption, string sortValue)
	{
		NKCUtil.SetGameobjectActive(m_objSortingType, bSet);
		if (bSet)
		{
			m_lbSortingType.text = GetSortName(sortOption);
			m_lbSortingValue.text = sortValue;
		}
	}

	public void SetSortingTypeValue(bool bSet, NKCOperatorSortSystem.eSortOption sortOption, string sortValue)
	{
		NKCUtil.SetGameobjectActive(m_objSortingType, bSet);
		if (bSet)
		{
			m_lbSortingType.text = GetSortName(sortOption);
			m_lbSortingValue.text = sortValue;
		}
	}

	private string GetSortName(NKCOperatorSortSystem.eSortOption sortOption)
	{
		return GetSortName(NKCOperatorSortSystem.ConvertSortOption(sortOption));
	}

	private string GetSortName(NKCUnitSortSystem.eSortOption sortOption)
	{
		return NKCUnitSortSystem.GetSortName(sortOption);
	}

	protected virtual void SetFierceBattleOtherBossAlreadyUsed(bool bVal)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_SELECT_LIST_FIERCE_BATTLE, bVal);
	}

	protected virtual void OnClick()
	{
		if (dOnSelectThisSlot != null)
		{
			dOnSelectThisSlot(m_NKMUnitData, m_NKMUnitTempletBase, m_DeckIndex, m_eUnitSlotState, m_eUnitSelectState);
		}
	}

	protected virtual void OnClickHave()
	{
		if (m_NKMUnitData != null)
		{
			NKCPopupHaveInfo.Instance.Open(m_NKMUnitData.m_UnitID);
		}
	}

	public void InvokeClick()
	{
		OnClick();
	}

	public void ClearTouchHoldEvent()
	{
		m_cbtnSlot.dOnPointerHolding = null;
	}

	public void SetTouchHoldEvent(UnityAction<NKMUnitData> holdAction)
	{
		if (holdAction == null)
		{
			m_cbtnSlot.dOnPointerHolding = null;
			return;
		}
		m_cbtnSlot.dOnPointerHolding = delegate
		{
			holdAction(m_NKMUnitData);
		};
	}

	public void SetTouchHoldEvent(UnityAction<NKMOperator> holdAction)
	{
		if (holdAction == null)
		{
			m_cbtnSlot.dOnPointerHolding = null;
			return;
		}
		m_cbtnSlot.dOnPointerHolding = delegate
		{
			holdAction(m_OperatorData);
		};
	}

	protected virtual void RestoreSprite()
	{
	}

	public virtual void SetContractedUnitMark(bool value)
	{
	}

	public virtual void SetRecall(bool bValue)
	{
	}

	protected void SetEnableLevelInfo(bool bEnable)
	{
		NKCUtil.SetGameobjectActive(m_objLevelInfo, bEnable);
	}
}
