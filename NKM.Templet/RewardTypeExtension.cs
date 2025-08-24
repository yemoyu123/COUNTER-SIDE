namespace NKM.Templet;

public static class RewardTypeExtension
{
	public static string ParseName(this NKM_REWARD_TYPE rewardType, int id)
	{
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(id);
			if (unitTempletBase == null)
			{
				return string.Empty;
			}
			return unitTempletBase.Name;
		}
		case NKM_REWARD_TYPE.RT_MISC:
		case NKM_REWARD_TYPE.RT_MISSION_POINT:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(id);
			if (itemMiscTempletByID == null)
			{
				return string.Empty;
			}
			return itemMiscTempletByID.m_ItemMiscName;
		}
		case NKM_REWARD_TYPE.RT_USER_EXP:
			return "사장 경험치";
		case NKM_REWARD_TYPE.RT_EQUIP:
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(id);
			if (equipTemplet == null)
			{
				return string.Empty;
			}
			return equipTemplet.m_ItemEquipName;
		}
		case NKM_REWARD_TYPE.RT_MOLD:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(id);
			if (itemMoldTempletByID == null)
			{
				return string.Empty;
			}
			return itemMoldTempletByID.m_MoldName;
		}
		case NKM_REWARD_TYPE.RT_SKIN:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(id);
			if (skinTemplet == null)
			{
				return string.Empty;
			}
			return skinTemplet.m_Title;
		}
		case NKM_REWARD_TYPE.RT_BUFF:
		{
			NKMCompanyBuffTemplet companyBuffTemplet = NKMCompanyBuffManager.GetCompanyBuffTemplet(id);
			if (companyBuffTemplet == null)
			{
				return string.Empty;
			}
			return companyBuffTemplet.m_CompanyBuffTitle;
		}
		case NKM_REWARD_TYPE.RT_EMOTICON:
		{
			NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(id);
			if (nKMEmoticonTemplet == null)
			{
				return string.Empty;
			}
			return nKMEmoticonTemplet.m_EmoticonStrID;
		}
		default:
			return string.Empty;
		}
	}
}
