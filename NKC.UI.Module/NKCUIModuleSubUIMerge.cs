using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NKC.UI.Result;
using NKM;
using NKM.Event;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Module;

public class NKCUIModuleSubUIMerge : NKCUIModuleSubUIBase
{
	public List<NKCUISlot> m_lstSelectedItems;

	[Header("\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd \ufffd\ufffd\ufffdԺ\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdʼ\ufffd")]
	public List<NKCUIComToggle> m_ctgRecipes;

	public NKCUIComStateButton m_csbtnMerge;

	public NKCUIComStateButton m_csbtnAuto;

	public Animator m_BackAni;

	public List<Image> m_lstCommonImg = new List<Image>();

	public List<Image> m_lstOtherImg = new List<Image>();

	public GameObject m_objAwakeFX;

	[Space]
	public Image m_imgAlpha037;

	public Image m_imgAlpha047;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd(\ufffdʿ䰹\ufffd\ufffd)/\ufffd\ufffd\ufffdϵ\ufffd\u07ba\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public List<Text> m_lstMergeInputCnt = new List<Text>();

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd(\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd)/\ufffd\ufffd\ufffdϵ\ufffd\u07ba\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public List<Text> m_lstMergeHaveCnt = new List<Text>();

	[Header("\ufffd÷\ufffd \ufffd\ufffdȯ \ufffdӵ\ufffd")]
	public float m_fBlendDelayTime = 0.4f;

	[Header("\ufffd\ufffdȰ\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd")]
	public float m_fDisableImgAlphaValue = 0.5f;

	private Dictionary<int, List<NKMEventCollectionDetailTemplet>> m_dicRecipeGroup = new Dictionary<int, List<NKMEventCollectionDetailTemplet>>();

	private NKMEventCollectionMergeTemplet m_curMergeTemplet;

	private int m_iCurTargetGradeGroupID;

	private int m_iCurMergeInputCnt;

	private const string CLICK_TAB = "CLICK_TAB";

	private List<long> m_lstSelectedUnit = new List<long>();

	private NKCUIUnitSelectList m_UIUnitSelectList;

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

	public override void Init()
	{
		foreach (NKCUISlot lstSelectedItem in m_lstSelectedItems)
		{
			lstSelectedItem.Init();
			lstSelectedItem.SetEmpty(OnClickSlot);
		}
		NKCUtil.SetBindFunction(m_csbtnAuto, OnClickAuto);
		NKCUtil.SetBindFunction(m_csbtnMerge, OnClickMerge);
	}

	public override void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
		if (templet == null)
		{
			return;
		}
		m_curMergeTemplet = NKMTempletContainer<NKMEventCollectionMergeTemplet>.Find(templet.CollectionMergeId);
		if (m_curMergeTemplet == null)
		{
			return;
		}
		m_dicRecipeGroup.Clear();
		foreach (NKMEventCollectionTemplet value in NKMTempletContainer<NKMEventCollectionTemplet>.Values)
		{
			if (m_curMergeTemplet.Key != value.CollectionMergeId)
			{
				continue;
			}
			foreach (NKMEventCollectionDetailTemplet collectionDataTemplet in value.Details)
			{
				NKMEventCollectionMergeRecipeTemplet nKMEventCollectionMergeRecipeTemplet = m_curMergeTemplet.RecipeTemplets.Find((NKMEventCollectionMergeRecipeTemplet e) => e.MergeRecipeGroupId == collectionDataTemplet.CollectionGradeGroupId);
				if (nKMEventCollectionMergeRecipeTemplet != null)
				{
					if (!m_dicRecipeGroup.ContainsKey(collectionDataTemplet.CollectionGradeGroupId))
					{
						m_dicRecipeGroup.Add(collectionDataTemplet.CollectionGradeGroupId, new List<NKMEventCollectionDetailTemplet> { collectionDataTemplet });
					}
					else
					{
						m_dicRecipeGroup[collectionDataTemplet.CollectionGradeGroupId].Add(collectionDataTemplet);
					}
				}
			}
		}
		if (m_dicRecipeGroup.Count <= 0)
		{
			return;
		}
		foreach (NKCUIComToggle ctgRecipe in m_ctgRecipes)
		{
			ctgRecipe.OnValueChanged.RemoveAllListeners();
			NKCUtil.SetGameobjectActive(ctgRecipe, bValue: false);
		}
		int num = 0;
		foreach (KeyValuePair<int, List<NKMEventCollectionDetailTemplet>> data in m_dicRecipeGroup)
		{
			if (m_ctgRecipes.Count > num)
			{
				NKCUtil.SetGameobjectActive(m_ctgRecipes[num], bValue: true);
				NKCUtil.SetToggleValueChangedDelegate(m_ctgRecipes[num], delegate(bool b)
				{
					OnClickTab(b, data.Key);
				});
				num++;
			}
		}
		m_ctgRecipes[0].Select(bSelect: true, bForce: true, bImmediate: true);
		OnClickTab(bSelect: true, m_dicRecipeGroup.First().Key);
		UpdateRecipeUI();
		ClearSlots();
	}

	public override void Refresh()
	{
		ClearSlots();
		UpdateRecipeUI();
	}

	public override void UnHide()
	{
		UpdateBackFX();
	}

	public void OnCompleteMerge(int mergeID, List<NKMUnitData> lstUnit)
	{
		foreach (NKMEventCollectionIndexTemplet collectionIdxTemplet in NKMTempletContainer<NKMEventCollectionIndexTemplet>.Values)
		{
			if (!collectionIdxTemplet.IsOpen || collectionIdxTemplet.CollectionMergeId != mergeID)
			{
				continue;
			}
			NKCUIModuleResult moduleResult = NKCUIModuleResult.MakeInstance(collectionIdxTemplet.EventMergeResultPrefabID, collectionIdxTemplet.EventMergeResultPrefabID);
			if (!(moduleResult != null))
			{
				continue;
			}
			moduleResult.Open(delegate
			{
				NKMRewardData nKMRewardData = new NKMRewardData();
				nKMRewardData.SetUnitData(lstUnit);
				moduleResult?.Close();
				NKCUIModuleResult.CheckInstanceAndClose();
				moduleResult = null;
				if (!string.IsNullOrEmpty(collectionIdxTemplet.EventResultPrefabID))
				{
					NKCUIPopupModuleResult moduleResultPopup = NKCUIPopupModuleResult.MakeInstance(collectionIdxTemplet.EventResultPrefabID, collectionIdxTemplet.EventResultPrefabID);
					if (null != moduleResultPopup)
					{
						moduleResultPopup.Init();
						moduleResultPopup.Open(nKMRewardData, delegate
						{
							if (moduleResultPopup.IsOpen)
							{
								moduleResultPopup.Close();
								moduleResultPopup = null;
							}
						});
					}
				}
				else
				{
					NKCUIResult.Instance.OpenBoxGain(NKCScenManager.CurrentUserData().m_ArmyData, nKMRewardData, NKCUtilString.GET_STRING_CONTRACT_SLOT_UNIT, null, bDisplayUnitGet: true, 1, bDefaultSort: false);
				}
			});
		}
	}

	public void OnClickTab(bool bSelect, int MergeInputGradeGroupID)
	{
		if (m_dicRecipeGroup.ContainsKey(MergeInputGradeGroupID) && m_curMergeTemplet != null)
		{
			NKMEventCollectionMergeRecipeTemplet nKMEventCollectionMergeRecipeTemplet = m_curMergeTemplet.RecipeTemplets.Find((NKMEventCollectionMergeRecipeTemplet e) => e.MergeRecipeGroupId == MergeInputGradeGroupID);
			if (nKMEventCollectionMergeRecipeTemplet != null)
			{
				m_iCurMergeInputCnt = nKMEventCollectionMergeRecipeTemplet.MergeInputValue;
				m_iCurTargetGradeGroupID = MergeInputGradeGroupID;
				ClearSlots();
				UpdateBackFX();
			}
		}
	}

	private void UpdateRecipeUI()
	{
		if (m_curMergeTemplet == null)
		{
			return;
		}
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		int num = 0;
		foreach (NKMEventCollectionMergeRecipeTemplet recipeTemplet in m_curMergeTemplet.RecipeTemplets)
		{
			if (m_lstMergeInputCnt.Count > num)
			{
				NKCUtil.SetLabelText(m_lstMergeInputCnt[num], string.Format(NKCUtilString.GET_STRING_MODULE_MERGE_INPUT_COUNT, recipeTemplet.MergeInputValue));
			}
			int num2 = 0;
			if (m_lstMergeHaveCnt.Count > num)
			{
				if (!m_dicRecipeGroup.ContainsKey(recipeTemplet.MergeInputGradeGroupId))
				{
					continue;
				}
				foreach (NKMEventCollectionDetailTemplet item in m_dicRecipeGroup[recipeTemplet.MergeInputGradeGroupId])
				{
					List<NKMUnitData> trophyListByUnitID = armyData.GetTrophyListByUnitID(item.Key);
					if (trophyListByUnitID.Count > 0)
					{
						List<NKMUnitData> list = trophyListByUnitID.FindAll((NKMUnitData x) => !x.m_bLock);
						if (list != null)
						{
							num2 += list.Count;
						}
					}
				}
				NKCUtil.SetLabelText(m_lstMergeHaveCnt[num], string.Format(NKCUtilString.GET_STRING_MODULE_MERGE_INPUT_UNIT_HAVE_COUNT, num2));
			}
			num++;
		}
	}

	private void UpdateBackFX()
	{
		m_BackAni.SetTrigger("CLICK_TAB");
		Color color = Color.white;
		Color color2 = Color.white;
		int num = 0;
		NKCUtil.SetGameobjectActive(m_objAwakeFX, bValue: false);
		foreach (KeyValuePair<int, List<NKMEventCollectionDetailTemplet>> item in m_dicRecipeGroup)
		{
			if (item.Key == m_iCurTargetGradeGroupID)
			{
				switch (num)
				{
				case 1:
					color = NKCUtil.GetColor("#09a9ff");
					color2 = NKCUtil.GetColor("#1D89D4");
					break;
				case 2:
					color = NKCUtil.GetColor("#b409ff");
					color2 = NKCUtil.GetColor("#B74A9B");
					break;
				case 3:
					color = NKCUtil.GetColor("#FFBA44");
					color2 = NKCUtil.GetColor("#FF6631");
					break;
				case 4:
					color = NKCUtil.GetColor("#FFBA44");
					color2 = NKCUtil.GetColor("#FF6631");
					NKCUtil.SetGameobjectActive(m_objAwakeFX, bValue: true);
					break;
				default:
					color = NKCUtil.GetColor("#AAB5BC");
					color2 = NKCUtil.GetColor("#718787");
					break;
				}
				break;
			}
			num++;
		}
		foreach (Image item2 in m_lstCommonImg)
		{
			Color endValue = color;
			if (null != m_imgAlpha037 && item2.name == m_imgAlpha037.name)
			{
				endValue.a = 0.39f;
			}
			else if (null != m_imgAlpha047 && item2.name == m_imgAlpha047.name)
			{
				endValue.a = 0.47f;
			}
			item2.DOBlendableColor(endValue, m_fBlendDelayTime);
		}
		foreach (Image item3 in m_lstOtherImg)
		{
			Color endValue2 = color2;
			if (null != m_imgAlpha037 && item3.name == m_imgAlpha037.name)
			{
				endValue2.a = 0.39f;
			}
			else if (null != m_imgAlpha047 && item3.name == m_imgAlpha047.name)
			{
				endValue2.a = 0.47f;
			}
			item3.DOBlendableColor(endValue2, m_fBlendDelayTime);
		}
	}

	private void ClearSlots()
	{
		m_lstSelectedUnit.Clear();
		for (int i = 0; i < m_lstSelectedItems.Count && !(null == m_lstSelectedItems[i]); i++)
		{
			m_lstSelectedItems[i].SetEmpty(OnClickSlot);
			if (i >= m_iCurMergeInputCnt)
			{
				m_lstSelectedItems[i].TurnOffExtraUI();
				m_lstSelectedItems[i].SetDisable(disable: true);
				m_lstSelectedItems[i].SetBGVisible(bSet: false);
				m_lstSelectedItems[i].SetAlphaColorDisableImg(m_fDisableImgAlphaValue);
			}
			else
			{
				m_lstSelectedItems[i].SetBGVisible(bSet: true);
			}
		}
	}

	private void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (!m_dicRecipeGroup.ContainsKey(m_iCurTargetGradeGroupID))
		{
			return;
		}
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: true, NKM_DECK_TYPE.NDT_NORMAL);
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.lstSortOption = new List<NKCUnitSortSystem.eSortOption>
		{
			NKCUnitSortSystem.eSortOption.Rarity_High,
			NKCUnitSortSystem.eSortOption.UID_First
		};
		options.bDescending = false;
		options.bShowRemoveSlot = false;
		options.iMaxMultipleSelect = m_iCurMergeInputCnt;
		options.bExcludeLockedUnit = false;
		options.bExcludeDeckedUnit = false;
		options.m_SortOptions.bUseDeckedState = true;
		options.m_SortOptions.bUseLockedState = true;
		options.m_SortOptions.bUseDormInState = true;
		options.setOnlyIncludeUnitID = new HashSet<int>();
		foreach (NKMEventCollectionDetailTemplet item in m_dicRecipeGroup[m_iCurTargetGradeGroupID])
		{
			options.setOnlyIncludeUnitID.Add(item.Key);
		}
		options.bShowHideDeckedUnitMenu = false;
		options.bEnableLockUnitSystem = false;
		options.setSelectedUnitUID = new HashSet<long>();
		foreach (long item2 in m_lstSelectedUnit)
		{
			options.setSelectedUnitUID.Add(item2);
		}
		options.strEmptyMessage = NKCUtilString.GET_STRING_MOUDLE_MERGE_TARGET_EMPTY;
		options.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		options.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		options.bHideDeckedUnit = true;
		options.m_bUseFavorite = true;
		options.eTargetUnitType = NKCUIUnitSelectList.TargetTabType.Trophy;
		UnitSelectList.Open(options, OnUnitSelected);
	}

	public void OnUnitSelected(List<long> selectedList)
	{
		if (m_UIUnitSelectList != null && m_UIUnitSelectList.IsOpen)
		{
			m_UIUnitSelectList.Close();
		}
		m_lstSelectedUnit = selectedList;
		if (m_lstSelectedUnit.Count == 0)
		{
			ClearSlots();
			return;
		}
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		for (int i = 0; i < m_lstSelectedItems.Count; i++)
		{
			if (i < m_lstSelectedUnit.Count)
			{
				NKMUnitData trophyFromUID = armyData.GetTrophyFromUID(m_lstSelectedUnit[i]);
				if (trophyFromUID != null)
				{
					NKMUnitManager.GetUnitTempletBase(trophyFromUID);
					m_lstSelectedItems[i].SetData(NKCUISlot.SlotData.MakeUnitData(trophyFromUID), bEnableLayoutElement: true, OnClickSlot);
				}
			}
			else if (i >= m_iCurMergeInputCnt)
			{
				m_lstSelectedItems[i].TurnOffExtraUI();
				m_lstSelectedItems[i].SetDisable(disable: true);
				m_lstSelectedItems[i].SetBGVisible(bSet: false);
				m_lstSelectedItems[i].SetAlphaColorDisableImg(m_fDisableImgAlphaValue);
			}
			else
			{
				m_lstSelectedItems[i].SetBGVisible(bSet: true);
				m_lstSelectedItems[i].SetEmpty(OnClickSlot);
			}
		}
	}

	private void OnClickAuto()
	{
		if (m_iCurTargetGradeGroupID != 0 && m_dicRecipeGroup.ContainsKey(m_iCurTargetGradeGroupID))
		{
			NKCScenManager.CurrentUserData();
			NKCGenericUnitSort obj = new NKCGenericUnitSort(options: new NKCUnitSortSystem.UnitListOptions
			{
				bExcludeLockedUnit = true,
				bExcludeDeckedUnit = true,
				setOnlyIncludeUnitID = new HashSet<int>(m_dicRecipeGroup[m_iCurTargetGradeGroupID].Select((NKMEventCollectionDetailTemplet x) => x.Key)),
				bIncludeUndeckableUnit = true
			}, userData: NKCScenManager.CurrentUserData(), lstUnitData: NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyTrophy.Values);
			HashSet<long> hashSet = new HashSet<long>(m_lstSelectedUnit);
			List<long> collection = (from x in obj.AutoSelect(hashSet, m_iCurMergeInputCnt - hashSet.Count)
				select x.m_UnitUID).ToList();
			m_lstSelectedUnit.AddRange(collection);
			OnUnitSelected(m_lstSelectedUnit);
		}
	}

	private void OnClickMerge()
	{
		if (m_iCurMergeInputCnt > m_lstSelectedUnit.Count)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_MOUDLE_MERGE_NO_ENOUGH_COUNT);
		}
		else
		{
			NKCPopupMergeConfirmBox.Instance.Open(m_curMergeTemplet.Key, m_iCurTargetGradeGroupID, m_lstSelectedUnit, Send_NKMPacket_EVENT_COLLECTION_MERGE_REQ);
		}
	}

	private void Send_NKMPacket_EVENT_COLLECTION_MERGE_REQ()
	{
		if (NKCPopupMergeConfirmBox.IsInstanceOpen)
		{
			NKCPopupMergeConfirmBox.Instance.Close();
		}
		NKCPacketSender.Send_NKMPacket_EVENT_COLLECTION_MERGE_REQ(m_curMergeTemplet.Key, m_iCurTargetGradeGroupID, m_lstSelectedUnit);
	}
}
