using System.Collections;
using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionTeamUpSlot : MonoBehaviour
{
	public delegate void OnClicked(int teamID);

	public Text m_NKM_UI_COLLECTION_TEAM_UP_SLOT_BG_TEXT_TITLE;

	public Text m_NKM_UI_COLLECTION_TEAM_UP_SLOT_BG_TEXT_COUNT;

	public RectTransform m_rtBackground;

	public LayoutElement m_LayoutElement;

	public RectTransform m_rtSlotParent;

	public NKCUISlot m_RewardSlot;

	public GameObject m_AB_ICON_SLOT_REWARD_FX;

	public GameObject m_objTeamUpReward;

	private int m_iTeamID;

	private bool m_bReadyReward;

	private List<RectTransform> m_lstRentalSlot = new List<RectTransform>();

	private OnClicked m_OnRewardClicked;

	public int GetTeamID()
	{
		return m_iTeamID;
	}

	public void Init()
	{
		m_RewardSlot.Init();
	}

	public List<RectTransform> GetRentalSlot()
	{
		return m_lstRentalSlot;
	}

	public void ClearRentalList()
	{
		m_lstRentalSlot.Clear();
	}

	public void SetData(NKCUICollectionTeamUp.TeamUpSlotData slotData, OnClicked click = null, bool bSetUnitList = true, NKCUISlot.OnClick SlotClick = null, List<RectTransform> lstSlot = null)
	{
		if (slotData == null)
		{
			Debug.Log("NKCUICollectionTeamUpSlot.SetData - wrong slot data");
			return;
		}
		NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_TEAM_UP_SLOT_BG_TEXT_TITLE, slotData.m_TeamName.ToString());
		NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_TEAM_UP_SLOT_BG_TEXT_COUNT, $"{slotData.m_HasUnitCount}/{slotData.m_RewardCriteria}");
		if (lstSlot != null)
		{
			List<int> lstUnit = slotData.m_lstUnit;
			for (int i = 0; i < lstSlot.Count; i++)
			{
				lstSlot[i].SetParent(m_rtSlotParent, worldPositionStays: false);
				lstSlot[i].GetComponent<RectTransform>().localScale = Vector3.one;
				NKCUISlot component = lstSlot[i].GetComponent<NKCUISlot>();
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeUnitData(lstUnit[i], 1, 0, slotData.m_TeamID);
				component.SetData(data, bEnableLayoutElement: true, SlotClick);
				NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
				if (armyData != null)
				{
					component.SetDenied(armyData.IsFirstGetUnit(lstUnit[i]));
				}
				NKCUtil.SetGameobjectActive(component.m_imgUpperRightIcon, bValue: false);
				m_lstRentalSlot.Add(lstSlot[i]);
			}
		}
		bool openTagCollectionTeamUp = NKCUnitMissionManager.GetOpenTagCollectionTeamUp();
		NKCUtil.SetGameobjectActive(m_objTeamUpReward, openTagCollectionTeamUp);
		if (null != m_RewardSlot && openTagCollectionTeamUp)
		{
			m_bReadyReward = false;
			NKCUISlot.SlotData slotData2 = NKCUISlot.SlotData.MakeRewardTypeData(slotData.m_RewardType, slotData.m_RewardID, slotData.m_RewardValue);
			if (slotData2 != null)
			{
				m_RewardSlot.SetData(slotData2, bEnableLayoutElement: true, OnRewardClick);
				switch (slotData.m_RewardState)
				{
				case NKCUICollectionTeamUp.eTeamUpRewardState.RS_NOT_READY:
					m_RewardSlot.SetDisable(disable: true);
					m_RewardSlot.SetClear(clear: false);
					break;
				case NKCUICollectionTeamUp.eTeamUpRewardState.RS_READY:
					m_RewardSlot.SetDisable(disable: false);
					m_RewardSlot.SetClear(clear: false);
					m_bReadyReward = true;
					break;
				case NKCUICollectionTeamUp.eTeamUpRewardState.RS_COMPLETE:
					m_RewardSlot.SetDisable(disable: false);
					m_RewardSlot.SetClear(clear: true);
					break;
				}
				NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT_REWARD_FX, slotData.m_RewardState == NKCUICollectionTeamUp.eTeamUpRewardState.RS_READY);
			}
		}
		m_iTeamID = slotData.m_TeamID;
		m_OnRewardClicked = click;
		if (null != m_rtBackground)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(m_rtBackground);
			if (base.gameObject.activeInHierarchy && base.gameObject.activeSelf)
			{
				StartCoroutine(DelayLayout());
			}
		}
	}

	private IEnumerator DelayLayout()
	{
		yield return new WaitForEndOfFrame();
		m_LayoutElement.minHeight = m_rtBackground.GetHeight();
		m_LayoutElement.preferredHeight = m_rtBackground.GetHeight();
		yield return null;
	}

	public void OnRewardClick(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (m_bReadyReward && m_OnRewardClicked != null)
		{
			m_OnRewardClicked(m_iTeamID);
		}
	}
}
