using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupFilterSubUIOperatorPassive : MonoBehaviour
{
	public delegate void OnClickSkillSlot(int skillID);

	public NKCPopupFilterSubUIOperatorPassiveSlot m_pfbSlot;

	public ScrollRect m_sr;

	public Transform m_trContents;

	public Transform m_trObjPool;

	private List<NKCPopupFilterSubUIOperatorPassiveSlot> m_lstVisible = new List<NKCPopupFilterSubUIOperatorPassiveSlot>();

	private Stack<NKCPopupFilterSubUIOperatorPassiveSlot> m_stkSlot = new Stack<NKCPopupFilterSubUIOperatorPassiveSlot>();

	public void Open(int selectedSkillID, OnClickSkillSlot onClickStatSlot, OperatorSkillType openType)
	{
		ResetSlot();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		foreach (NKMOperatorSkillTemplet value in NKMTempletContainer<NKMOperatorSkillTemplet>.Values)
		{
			if (value.m_OperSkillType == openType && value.UseFilter)
			{
				NKCPopupFilterSubUIOperatorPassiveSlot slot = GetSlot();
				slot.transform.SetParent(m_trContents);
				slot.GetButton().PointerClick.RemoveAllListeners();
				slot.GetButton().PointerClick.AddListener(delegate
				{
					Close();
					onClickStatSlot(slot.GetPassiveTemplet().m_OperSkillID);
				});
				m_lstVisible.Add(slot);
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				bool flag = selectedSkillID == value.m_OperSkillID;
				slot.SetData(value, flag);
				if (flag)
				{
					slot.GetButton().Select(bSelect: true, bForce: true, bImmediate: true);
				}
				else
				{
					slot.GetButton().Select(bSelect: false, bForce: true, bImmediate: true);
				}
			}
		}
	}

	private NKCPopupFilterSubUIOperatorPassiveSlot GetSlot()
	{
		if (m_stkSlot.Count > 0)
		{
			return m_stkSlot.Pop();
		}
		return Object.Instantiate(m_pfbSlot, m_trContents);
	}

	public void Close()
	{
		ResetSlot();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void ResetSlot()
	{
		for (int i = 0; i < m_lstVisible.Count; i++)
		{
			m_lstVisible[i].transform.SetParent(m_trObjPool);
			NKCUtil.SetGameobjectActive(m_lstVisible[i], bValue: false);
			m_stkSlot.Push(m_lstVisible[i]);
		}
		m_lstVisible.Clear();
	}

	public void Clean()
	{
		while (m_stkSlot.Count > 0)
		{
			Object.Destroy(m_stkSlot.Pop());
		}
	}
}
