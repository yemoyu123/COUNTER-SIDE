using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public abstract class StageRewardData
{
	protected NKCUISlot m_cSlot;

	public StageRewardData(Transform slotParent)
	{
		m_cSlot = NKCUISlot.GetNewInstance(slotParent);
	}

	public abstract void CreateSlotData(NKMStageTempletV2 stageTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow);

	public abstract void CreateSlotData(NKMDefenceTemplet defenceTemplet, NKMUserData cNKMUserData, List<int> listNRGI, List<NKCUISlot> listSlotToShow);

	public virtual void Release()
	{
		m_cSlot = null;
	}

	public void SetSlotActive(bool isActive)
	{
		if (!(m_cSlot == null))
		{
			m_cSlot.SetActive(isActive);
		}
	}

	protected void InitSlot(NKCUISlot cItemSlot)
	{
		cItemSlot.Init();
		cItemSlot.gameObject.GetComponent<RectTransform>().localScale = Vector2.one;
		Vector3 localPosition = cItemSlot.gameObject.GetComponent<RectTransform>().localPosition;
		cItemSlot.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
	}
}
