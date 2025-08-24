using System.Collections.Generic;
using ClientPacket.Unit;
using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIRearmamentExtract : MonoBehaviour
{
	public NKCUIComStateButton[] m_lstSelectBtn;

	public List<GameObject> m_lstPiece;

	public List<NKCUIRearmamentExtractUnitSlot> m_lstExtractUnitSlot;

	public GameObject m_objLeftSynergy;

	public GameObject m_objRightResult;

	public GameObject m_objRightReady;

	[Space]
	public RectTransform m_rtTacticsInfo;

	private List<NKCUISlot> m_lstExtractItem = new List<NKCUISlot>();

	[Header("추출 버튼")]
	public Text m_lbExtractBtn;

	public Image m_imgExtractBtn;

	public NKCUIComStateButton m_csbtnExtract;

	[Header("시너지 보너스")]
	public List<GameObject> m_lstSynergyBonusOnFx;

	public List<GameObject> m_lstSynergyBonusOff;

	public List<GameObject> m_lstSynergyBonusOn;

	public NKCUISlot m_SynergyBonusSlot;

	public NKCUIComStateButton m_csbtnSynergyInfo;

	public Text m_lbSynergyBounsDesc;

	[Header("애니메이션")]
	public Animator m_AniExtract;

	public Animator m_AniSynergyBouns;

	[Header("시너지 보너스 아이템 코드(아이콘 세팅 용)")]
	public int m_MiscItemCode = 1053;

	public int m_MiscItemCodeDisable = 1054;

	[Header("시너지 활성화 사운드")]
	public bool m_bSynergyBoundSoundLoop;

	public string m_strSynergyBounsSoundName = "FX_UI_DIVE_START_MOVIE_FRONT";

	private List<NKCUISlot.SlotData> m_lstExtractItemData = new List<NKCUISlot.SlotData>();

	private bool m_bActiveSynergyBounds;

	private NKCUIUnitSelectList m_UIUnitSelectList;

	private const string ANI_ACTIVE = "ACTIVE";

	private const string ANI_DE_ACTIVE = "DEACTIVE";

	private int m_iSynergyBoundSoundUID;

	private List<long> m_lstSelectedUnitsUID = new List<long>();

	private NKCUIUnitSelectList UnitSelectList
	{
		get
		{
			if (m_UIUnitSelectList == null)
			{
				m_UIUnitSelectList = NKCUIUnitSelectList.OpenNewInstance();
			}
			return m_UIUnitSelectList;
		}
	}

	public void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnExtract, OnClickExtract);
		NKCUtil.SetBindFunction(m_csbtnSynergyInfo, OnClickSynergyInfo);
		NKCUIComStateButton[] lstSelectBtn = m_lstSelectBtn;
		for (int i = 0; i < lstSelectBtn.Length; i++)
		{
			NKCUtil.SetBindFunction(lstSelectBtn[i], OnClickSelectList);
		}
		if (null != m_SynergyBonusSlot)
		{
			m_SynergyBonusSlot.Init();
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(m_MiscItemCode, 1L);
			m_SynergyBonusSlot.SetData(data, bEnableLayoutElement: true, OnClickExtractReward);
		}
	}

	public void Clear()
	{
		UnitSelectList?.Close();
		m_UIUnitSelectList = null;
	}

	public void Open()
	{
		m_lstSelectedUnitsUID.Clear();
		m_AniExtract?.SetTrigger("ACTIVE");
		UpdateUI();
	}

	public void UpdateUI()
	{
		UpdateUnitSlotUI();
		UpdateExtractItemData();
		UpdateExtractResultItemUI();
		UpdateSynergyUI();
	}

	public void OnRecv(NKMPacket_EXTRACT_UNIT_ACK sPacket)
	{
		m_lstSelectedUnitsUID.Clear();
		UpdateUI();
	}

	private void UpdateUnitSlotUI()
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		for (int i = 0; i < NKMCommonConst.MaxExtractUnitSelect; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstPiece[i], m_lstSelectedUnitsUID.Count > i);
			NKMUnitData data = null;
			if (m_lstSelectedUnitsUID.Count > i)
			{
				data = armyData.GetUnitFromUID(m_lstSelectedUnitsUID[i]);
			}
			m_lstExtractUnitSlot[i].SetData(data);
		}
	}

	private void UpdateExtractItemData()
	{
		m_lstExtractItemData.Clear();
		if (m_lstSelectedUnitsUID.Count <= 0)
		{
			Debug.Log("선택된 유닛이 없슈");
			return;
		}
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (long item in m_lstSelectedUnitsUID)
		{
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(item);
			if (unitFromUID == null)
			{
				continue;
			}
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID);
			if (unitTempletBase == null)
			{
				continue;
			}
			foreach (NKMRewardInfo extractReward in unitTempletBase.ExtractRewards)
			{
				if (extractReward.ID != 0 && extractReward.Count != 0)
				{
					int iD = extractReward.ID;
					int count = extractReward.Count;
					if (dictionary.ContainsKey(iD))
					{
						dictionary[iD] += count;
					}
					else
					{
						dictionary.Add(iD, count);
					}
				}
			}
			if (unitFromUID.FromContract && unitTempletBase.ExtractRewardFromContract != null)
			{
				NKMRewardInfo extractRewardFromContract = unitTempletBase.ExtractRewardFromContract;
				if (dictionary.ContainsKey(extractRewardFromContract.ID))
				{
					dictionary[extractRewardFromContract.ID] += extractRewardFromContract.Count;
				}
				else
				{
					dictionary.Add(extractRewardFromContract.ID, extractRewardFromContract.Count);
				}
			}
		}
		if (dictionary.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<int, int> item2 in dictionary)
		{
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_MISC, item2.Key, item2.Value);
			if (slotData != null)
			{
				m_lstExtractItemData.Add(slotData);
			}
		}
	}

	private void UpdateExtractResultItemUI()
	{
		foreach (NKCUISlot item in m_lstExtractItem)
		{
			item.CleanUp();
			Object.Destroy(item.gameObject);
		}
		m_lstExtractItem.Clear();
		if (m_lstExtractItemData.Count <= 0)
		{
			NKCUtil.SetGameobjectActive(m_objRightReady, bValue: true);
			NKCUtil.SetGameobjectActive(m_objRightResult, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objRightReady, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRightResult, bValue: true);
		foreach (NKCUISlot.SlotData lstExtractItemDatum in m_lstExtractItemData)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(lstExtractItemDatum.ID);
			if (itemMiscTempletByID == null)
			{
				continue;
			}
			if (itemMiscTempletByID.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_PIECE)
			{
				Debug.LogError("NKCUIRearmamentExtract::UpdateExtractResultItemUI() - Can not support imt_piece type");
			}
			else
			{
				if (!(m_rtTacticsInfo != null))
				{
					continue;
				}
				NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_rtTacticsInfo);
				if (null != newInstance)
				{
					NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(NKM_REWARD_TYPE.RT_MISC, lstExtractItemDatum.ID, (int)lstExtractItemDatum.Count);
					if (slotData != null)
					{
						newInstance.SetData(slotData);
						NKCUtil.SetGameobjectActive(newInstance.gameObject, bValue: true);
					}
					m_lstExtractItem.Add(newInstance);
				}
			}
		}
	}

	private void UpdateSynergyUI()
	{
		m_bActiveSynergyBounds = false;
		if (m_lstSelectedUnitsUID.Count >= NKMCommonConst.MaxExtractUnitSelect)
		{
			NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
			List<NKM_UNIT_ROLE_TYPE> lstSelectedUnitsRole = new List<NKM_UNIT_ROLE_TYPE>();
			foreach (long item in m_lstSelectedUnitsUID)
			{
				NKMUnitData unitFromUID = armyData.GetUnitFromUID(item);
				if (unitFromUID != null)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID);
					lstSelectedUnitsRole.Add(unitTempletBase.m_NKM_UNIT_ROLE_TYPE);
				}
			}
			NKM_UNIT_ROLE_TYPE firstRole = lstSelectedUnitsRole[0];
			List<NKM_UNIT_ROLE_TYPE> list = lstSelectedUnitsRole.FindAll((NKM_UNIT_ROLE_TYPE x) => x.Equals(firstRole));
			if (list != null)
			{
				m_bActiveSynergyBounds = list.Count >= NKMCommonConst.MaxExtractUnitSelect;
			}
			if (!m_bActiveSynergyBounds)
			{
				bool bActiveSynergyBounds = true;
				int iCnt = 0;
				while (iCnt < lstSelectedUnitsRole.Count)
				{
					if (lstSelectedUnitsRole.Count > iCnt)
					{
						List<NKM_UNIT_ROLE_TYPE> list2 = lstSelectedUnitsRole.FindAll((NKM_UNIT_ROLE_TYPE x) => x.Equals(lstSelectedUnitsRole[iCnt]));
						if (list2 != null && list2.Count > 1)
						{
							bActiveSynergyBounds = false;
							break;
						}
					}
					int num = iCnt + 1;
					iCnt = num;
				}
				m_bActiveSynergyBounds = bActiveSynergyBounds;
			}
		}
		NKCUtil.SetGameobjectActive(m_objLeftSynergy, m_bActiveSynergyBounds);
		foreach (GameObject item2 in m_lstSynergyBonusOff)
		{
			NKCUtil.SetGameobjectActive(item2, !m_bActiveSynergyBounds);
		}
		foreach (GameObject item3 in m_lstSynergyBonusOn)
		{
			NKCUtil.SetGameobjectActive(item3, m_bActiveSynergyBounds);
		}
		foreach (GameObject item4 in m_lstSynergyBonusOnFx)
		{
			NKCUtil.SetGameobjectActive(item4, bValue: false);
			if (m_bActiveSynergyBounds)
			{
				NKCUtil.SetGameobjectActive(item4, bValue: true);
			}
		}
		if (m_bActiveSynergyBounds)
		{
			int synergyIncreasePercentage = NKCRearmamentUtil.GetSynergyIncreasePercentage(m_lstSelectedUnitsUID);
			NKCUtil.SetLabelText(m_lbSynergyBounsDesc, string.Format(NKCUtilString.GET_STRING_REARM_EXTRACT_CONFIRM_POPUP_SYNERGY_BONUS, synergyIncreasePercentage));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbSynergyBounsDesc, NKCUtilString.GET_STRING_REARM_EXTRACT_NOT_ACTIVE_SYNERGY_BOUNS);
		}
		Color uITextColor = NKCUtil.GetUITextColor(m_lstSelectedUnitsUID.Count > 0);
		NKCUtil.ButtonColor type = NKCUtil.ButtonColor.BC_GRAY;
		if (m_lstSelectedUnitsUID.Count > 0)
		{
			type = NKCUtil.ButtonColor.BC_YELLOW;
		}
		Sprite buttonSprite = NKCUtil.GetButtonSprite(type);
		NKCUtil.SetLabelTextColor(m_lbExtractBtn, uITextColor);
		NKCUtil.SetImageSprite(m_imgExtractBtn, buttonSprite);
		if (null != m_SynergyBonusSlot)
		{
			int itemID = m_MiscItemCode;
			if (!m_bActiveSynergyBounds)
			{
				itemID = m_MiscItemCodeDisable;
			}
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeMiscItemData(itemID, 1L);
			if (slotData != null)
			{
				m_SynergyBonusSlot.SetData(slotData, bEnableLayoutElement: true, OnClickExtractReward);
			}
		}
		UpdateSynergyAni(m_bActiveSynergyBounds);
	}

	private void OnClickSelectList()
	{
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.lstSortOption = new List<NKCUnitSortSystem.eSortOption>
		{
			NKCUnitSortSystem.eSortOption.Level_Low,
			NKCUnitSortSystem.eSortOption.Rarity_Low,
			NKCUnitSortSystem.eSortOption.ID_First,
			NKCUnitSortSystem.eSortOption.UID_Last
		};
		options.bDescending = false;
		options.bShowRemoveSlot = false;
		options.iMaxMultipleSelect = NKMCommonConst.MaxExtractUnitSelect;
		options.bExcludeLockedUnit = false;
		options.bExcludeDeckedUnit = false;
		options.m_SortOptions.bUseDeckedState = true;
		options.m_SortOptions.bUseLockedState = true;
		options.m_SortOptions.bUseDormInState = true;
		options.bShowHideDeckedUnitMenu = false;
		options.bHideDeckedUnit = false;
		options.dOnAutoSelectFilter = null;
		options.bUseRemoveSmartAutoSelect = false;
		options.setSelectedUnitUID = new HashSet<long>();
		foreach (long item in m_lstSelectedUnitsUID)
		{
			options.setSelectedUnitUID.Add(item);
		}
		options.strEmptyMessage = NKCUtilString.GET_STRING_REARM_EXTRACT_NOT_TARGET_UNIT;
		options.bCanSelectUnitInMission = false;
		options.m_SortOptions.bIncludeSeizure = false;
		options.dOnClose = null;
		options.bPushBackUnselectable = false;
		options.m_SortOptions.bIgnoreWorldMapLeader = false;
		options.setUnitFilterCategory = new HashSet<NKCUnitSortSystem.eFilterCategory>
		{
			NKCUnitSortSystem.eFilterCategory.UnitType,
			NKCUnitSortSystem.eFilterCategory.UnitRole,
			NKCUnitSortSystem.eFilterCategory.UnitMoveType,
			NKCUnitSortSystem.eFilterCategory.UnitTargetType,
			NKCUnitSortSystem.eFilterCategory.Rarity,
			NKCUnitSortSystem.eFilterCategory.Cost,
			NKCUnitSortSystem.eFilterCategory.Decked,
			NKCUnitSortSystem.eFilterCategory.Locked
		};
		options.setUnitSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>
		{
			NKCUnitSortSystem.eSortCategory.IDX,
			NKCUnitSortSystem.eSortCategory.Rarity,
			NKCUnitSortSystem.eSortCategory.UnitSummonCost
		};
		options.setExcludeUnitUID = new HashSet<long>();
		options.setExcludeUnitID = NKCUnitSortSystem.GetDefaultExcludeUnitIDs();
		options.bOpenedAtRearmExtract = true;
		options.m_bHideUnitCount = true;
		options.m_bUseFavorite = true;
		new List<NKMUnitData>();
		foreach (NKMUnitData value in NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyUnit.Values)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(value.m_UnitID);
			if (nKMUnitTempletBase != null)
			{
				if (nKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
				{
					options.setExcludeUnitUID.Add(value.m_UnitUID);
				}
				if (nKMUnitTempletBase.m_NKM_UNIT_GRADE < NKM_UNIT_GRADE.NUG_SR)
				{
					options.setExcludeUnitUID.Add(value.m_UnitUID);
				}
			}
		}
		UnitSelectList.Open(options, OnSelectedUnits);
	}

	private void OnSelectedUnits(List<long> lstUnits)
	{
		if (UnitSelectList.IsOpen)
		{
			UnitSelectList.Close();
		}
		m_lstSelectedUnitsUID = lstUnits;
		UpdateUI();
	}

	private void UpdateSynergyAni(bool bActive)
	{
		if (bActive)
		{
			m_iSynergyBoundSoundUID = NKCSoundManager.PlaySound(m_strSynergyBounsSoundName, 1f, 0f, 0f, m_bSynergyBoundSoundLoop);
			m_AniSynergyBouns?.SetTrigger("ACTIVE");
		}
		else
		{
			NKCSoundManager.StopSound(m_iSynergyBoundSoundUID);
			m_AniSynergyBouns.SetTrigger("DEACTIVE");
		}
	}

	private void OnClickExtract()
	{
		if (m_lstExtractItemData.Count <= 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REARM_EXTRACT_LACK_TARGET_UNIT_COUNT);
		}
		else if (NKCScenManager.CurrentUserData().m_ArmyData.GetCurrentUnitCount() - m_lstSelectedUnitsUID.Count <= 8)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_REARM_EXTRACT_UNIT_LIMIT_UNDER_8);
		}
		else
		{
			NKCUIPopupRearmamentExtractConfirm.Instance.Open(m_lstExtractItemData, m_lstSelectedUnitsUID, m_bActiveSynergyBounds);
		}
	}

	private void OnClickSynergyInfo()
	{
		NKCUIPopupRearmamentExtractSynergyInfo.Instance.Open();
	}

	private void OnClickExtractReward(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKCUIPopupRearmamentExtractSynergyInfo.Instance.Open();
	}

	public RectTransform GetExtractSlotRectTransform(int iExtractSlotIdx)
	{
		if (iExtractSlotIdx >= 0 && m_lstExtractUnitSlot.Count > iExtractSlotIdx)
		{
			return m_lstExtractUnitSlot[iExtractSlotIdx].GetComponent<RectTransform>();
		}
		return null;
	}
}
