using System.Collections;
using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Result;

public class NKCUIResultSubUIRearmament : NKCUIResultSubUIBase
{
	[Header("재무장")]
	public GameObject m_ObjSynergy;

	public NKCUISlot m_SynergyRewardSlot;

	public RectTransform m_rtExtractRewardSlotParent;

	private bool m_bFinished;

	private List<NKCUISlot> m_lstRewardSlot = new List<NKCUISlot>();

	public void SetData(NKMRewardData rewardData, NKMRewardData synergyRewardData, bool bIgnoreAutoClose = false)
	{
		if (rewardData == null)
		{
			base.ProcessRequired = false;
			return;
		}
		foreach (NKCUISlot.SlotData item in NKCUISlot.MakeSlotDataListFromReward(rewardData))
		{
			NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_rtExtractRewardSlotParent);
			if (newInstance != null)
			{
				newInstance.SetData(item);
				NKCUtil.SetGameobjectActive(newInstance.gameObject, bValue: true);
				m_lstRewardSlot.Add(newInstance);
			}
		}
		NKCUtil.SetGameobjectActive(m_ObjSynergy, synergyRewardData != null);
		if (synergyRewardData != null)
		{
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeMiscItemData(synergyRewardData.MiscItemDataList[0]);
			if (slotData != null)
			{
				m_SynergyRewardSlot.Init();
				m_SynergyRewardSlot.SetData(slotData);
			}
		}
		base.ProcessRequired = true;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_bIgnoreAutoClose = bIgnoreAutoClose;
	}

	public override void FinishProcess()
	{
		if (base.gameObject.activeInHierarchy)
		{
			m_bFinished = true;
			for (int i = 0; i < m_lstRewardSlot.Count; i++)
			{
				Object.Destroy(m_lstRewardSlot[i]);
			}
			m_lstRewardSlot.Clear();
			StopAllCoroutines();
		}
	}

	public override bool IsProcessFinished()
	{
		return m_bFinished;
	}

	protected override IEnumerator InnerProcess(bool bAutoSkip)
	{
		m_bFinished = false;
		m_bHadUserInput = false;
		yield return null;
	}

	public override void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}
}
