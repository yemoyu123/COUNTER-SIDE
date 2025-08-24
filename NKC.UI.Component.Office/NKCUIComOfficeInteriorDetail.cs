using System.Collections.Generic;
using NKM;
using NKM.Templet.Office;
using UnityEngine;

namespace NKC.UI.Component.Office;

public class NKCUIComOfficeInteriorDetail : MonoBehaviour
{
	public GameObject m_objInteraction;

	public GameObject m_objAnimation;

	public GameObject m_objAnimationDot;

	public GameObject m_objSound;

	public GameObject m_objSoundDot;

	public GameObject m_objEffect;

	public GameObject m_objEffectDot;

	public GameObject m_objBGM;

	public GameObject m_objBGMDot;

	public void SetData(int itemID)
	{
		NKMOfficeInteriorTemplet data = NKMItemMiscTemplet.FindInterior(itemID);
		SetData(data);
	}

	public void SetData(NKMItemMiscTemplet miscItemTemplet)
	{
		if (miscItemTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		if (miscItemTemplet.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_INTERIOR)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKMOfficeInteriorTemplet data = NKMItemMiscTemplet.FindInterior(miscItemTemplet.m_ItemMiscID);
		SetData(data);
	}

	public void SetData(NKMOfficeInteriorTemplet templet)
	{
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		bool flag = false;
		NKCUtil.SetGameobjectActive(m_objInteraction, templet.HasInteraction);
		flag = flag || templet.HasInteraction;
		NKCUtil.SetGameobjectActive(m_objAnimationDot, flag);
		NKCUtil.SetGameobjectActive(m_objAnimation, templet.Animation);
		flag = flag || templet.Animation;
		NKCUtil.SetGameobjectActive(m_objSoundDot, flag);
		NKCUtil.SetGameobjectActive(m_objSound, templet.HasSound);
		flag = flag || templet.HasSound;
		NKCUtil.SetGameobjectActive(m_objEffectDot, flag);
		NKCUtil.SetGameobjectActive(m_objEffect, templet.Effect);
		flag = flag || templet.Effect;
		NKCUtil.SetGameobjectActive(m_objBGMDot, flag);
		NKCUtil.SetGameobjectActive(m_objBGM, templet.HasBGM);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void SetData(IEnumerable<int> lstInteriorID)
	{
		List<NKMOfficeInteriorTemplet> list = new List<NKMOfficeInteriorTemplet>();
		foreach (int item in lstInteriorID)
		{
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(item);
			if (nKMOfficeInteriorTemplet != null)
			{
				list.Add(nKMOfficeInteriorTemplet);
			}
		}
		SetData(list);
	}

	public void SetData(IEnumerable<NKMOfficeInteriorTemplet> lstTemplet)
	{
		if (lstTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		foreach (NKMOfficeInteriorTemplet item in lstTemplet)
		{
			if (item != null)
			{
				flag |= item.HasInteraction;
				flag2 |= item.Animation;
				flag3 |= item.HasSound;
				flag4 |= item.Effect;
				flag5 |= item.HasBGM;
			}
		}
		bool flag6 = false;
		NKCUtil.SetGameobjectActive(m_objInteraction, flag);
		flag6 = flag6 || flag;
		NKCUtil.SetGameobjectActive(m_objAnimationDot, flag6);
		NKCUtil.SetGameobjectActive(m_objAnimation, flag2);
		flag6 = flag6 || flag2;
		NKCUtil.SetGameobjectActive(m_objSoundDot, flag6);
		NKCUtil.SetGameobjectActive(m_objSound, flag3);
		flag6 = flag6 || flag3;
		NKCUtil.SetGameobjectActive(m_objEffectDot, flag6);
		NKCUtil.SetGameobjectActive(m_objEffect, flag4);
		flag6 = flag6 || flag4;
		NKCUtil.SetGameobjectActive(m_objBGMDot, flag6);
		NKCUtil.SetGameobjectActive(m_objBGM, flag5);
		NKCUtil.SetGameobjectActive(base.gameObject, flag || flag2 || flag3 || flag4 || flag5);
	}
}
