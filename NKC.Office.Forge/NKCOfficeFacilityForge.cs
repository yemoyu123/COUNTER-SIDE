using System;
using NKC.UI;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.Office.Forge;

public class NKCOfficeFacilityForge : NKCOfficeFacility
{
	[Header("공방 정보")]
	public NKCOfficeFacilityFuniture m_fnEquipBuild;

	public NKCOfficeFacilityFuniture m_fnEquipEnhance;

	public NKCOfficeFacilityFuniture m_fnEquipUpgrade;

	public override void Init()
	{
		base.Init();
		if (m_fnEquipBuild != null)
		{
			NKCOfficeFacilityFuniture fnEquipBuild = m_fnEquipBuild;
			fnEquipBuild.dOnClickFuniture = (NKCOfficeFuniture.OnClickFuniture)Delegate.Combine(fnEquipBuild.dOnClickFuniture, new NKCOfficeFuniture.OnClickFuniture(OnClickEquipBuild));
		}
		if (null != m_fnEquipEnhance)
		{
			NKCOfficeFacilityFuniture fnEquipEnhance = m_fnEquipEnhance;
			fnEquipEnhance.dOnClickFuniture = (NKCOfficeFuniture.OnClickFuniture)Delegate.Combine(fnEquipEnhance.dOnClickFuniture, new NKCOfficeFuniture.OnClickFuniture(OnClickEquipEnhance));
			m_fnEquipEnhance.SetLock(!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT));
		}
		if (m_fnEquipUpgrade != null)
		{
			if (NKMOpenTagManager.IsOpened("EQUIP_UPGRADE"))
			{
				bool flag = NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY) && NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_UPGRADE);
				NKCOfficeFacilityFuniture fnEquipUpgrade = m_fnEquipUpgrade;
				fnEquipUpgrade.dOnClickFuniture = (NKCOfficeFuniture.OnClickFuniture)Delegate.Combine(fnEquipUpgrade.dOnClickFuniture, new NKCOfficeFuniture.OnClickFuniture(OnClickEquipUpgrade));
				m_fnEquipUpgrade.SetLock(!flag);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_fnEquipUpgrade, bValue: false);
			}
		}
	}

	private void OnClickEquipBuild(int id, long uid)
	{
		NKCUIForgeCraftMold.Instance.Open();
	}

	private void OnClickEquipEnhance(int id, long uid)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
		}
		else if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_ENCHANT);
		}
		else
		{
			NKCUIForge.Instance.Open(NKCUIForge.NKC_FORGE_TAB.NFT_ENCHANT, 0L);
		}
	}

	private void OnClickEquipUpgrade(int id, long uid)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
		}
		else if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_UPGRADE))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_UPGRADE);
		}
		else
		{
			NKCUIForgeUpgrade.Instance.Open();
		}
	}
}
