using System.Collections.Generic;
using NKC.Office;
using NKC.Templet.Office;
using NKM;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component.Office;

public class NKCUIComOfficeInteriorInteractionBubble : MonoBehaviour
{
	public Image m_ImgUnitIcon;

	public Text m_lbUnitName;

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
		List<NKCOfficeFurnitureInteractionTemplet> interactionTempletList = NKCOfficeFurnitureInteractionTemplet.GetInteractionTempletList(templet, NKCOfficeFurnitureInteractionTemplet.ActType.Common);
		if (interactionTempletList == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCOfficeFurnitureInteractionTemplet nKCOfficeFurnitureInteractionTemplet = null;
		int num = 0;
		foreach (NKCOfficeFurnitureInteractionTemplet item in interactionTempletList)
		{
			num = item.GetFirstExclusiveActTarget();
			if (num > 0)
			{
				nKCOfficeFurnitureInteractionTemplet = item;
				break;
			}
		}
		if (nKCOfficeFurnitureInteractionTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		switch (nKCOfficeFurnitureInteractionTemplet.eActTargetType)
		{
		case ActTargetType.Skin:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(num);
			if (skinTemplet == null)
			{
				Debug.LogError($"skin {num} does not exists!");
				NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
				return;
			}
			Sprite sp2 = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, skinTemplet);
			NKCUtil.SetImageSprite(m_ImgUnitIcon, sp2);
			NKMUnitTempletBase nKMUnitTempletBase2 = NKMUnitTempletBase.Find(skinTemplet.m_SkinEquipUnitID);
			NKCUtil.SetLabelText(m_lbUnitName, nKMUnitTempletBase2.GetUnitName());
			break;
		}
		case ActTargetType.Unit:
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(num);
			Sprite sp = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, nKMUnitTempletBase);
			NKCUtil.SetImageSprite(m_ImgUnitIcon, sp);
			NKCUtil.SetLabelText(m_lbUnitName, nKMUnitTempletBase.GetUnitName());
			break;
		}
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}
}
