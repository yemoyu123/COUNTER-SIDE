using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCUIFierceBattleBossListSlot : MonoBehaviour
{
	public static class NKCFierceUI
	{
		public static Color BossClearColorNotProgress => NKCUtil.GetColor("#818181");

		public static Color BossClearColorClear => NKCUtil.GetColor("#1DABE0");

		public static Color BossClearColorCanTry => NKCUtil.GetColor("#FFCF3B");
	}

	private NKCAssetInstanceData m_InstanceData;

	[Header("격전지원 진행 가능 상태")]
	public GameObject m_BOSS_LIST_SLOT_BUTTON_BASIC;

	public GameObject m_BOSS_LIST_SLOT_BUTTON_BASIC_Normal;

	public GameObject m_BOSS_LIST_SLOT_BUTTON_BASIC_Select;

	[Space]
	public GameObject m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE;

	public GameObject m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE_Normal;

	public GameObject m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE_Select;

	[Header("격전지원 진행 불가 상태")]
	public GameObject m_BOSS_LIST_SLOT_BUTTON_END;

	public GameObject m_BOSS_LIST_SLOT_BUTTON_END_Normal;

	public GameObject m_BOSS_LIST_SLOT_BUTTON_END_Select;

	[Space]
	public GameObject m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE_END;

	public GameObject m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE_END_Normal;

	public GameObject m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE_END_Select;

	[Header("보스 이미지")]
	public List<Image> m_bossImage;

	[Space]
	public List<Image> m_bossNightImage;

	public GameObject m_objBossClearCnt;

	[Header("클리어 상태")]
	public List<Image> m_lstClearImage;

	public int m_fierceBossGroupID;

	[Header("기타")]
	public NKCUIComStateButton m_csbtnBtn;

	public GameObject m_objNoneRecord;

	private bool m_bSeason;

	public static NKCUIFierceBattleBossListSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_fierce_battle", "NKM_UI_FIERCE_BATTLE_BOSS_LIST_SLOT");
		NKCUIFierceBattleBossListSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIFierceBattleBossListSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIFierceBattleBossListSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.transform.localScale = new Vector3(1f, 1f, 1f);
		return component;
	}

	public void SetData(bool bSeason, int bossGroupdID, int playableBossLv)
	{
		Sprite sp = null;
		bool flag = false;
		if (NKMFierceBossGroupTemplet.Groups.ContainsKey(bossGroupdID))
		{
			foreach (NKMFierceBossGroupTemplet item in NKMFierceBossGroupTemplet.Groups[bossGroupdID])
			{
				if (item.Level == playableBossLv)
				{
					flag = item.UI_HellModeCheck;
					sp = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_fierce_battle_boss_thumbnail", item.UI_BossFaceSlot);
					break;
				}
			}
			if (!flag)
			{
				foreach (Image item2 in m_bossImage)
				{
					NKCUtil.SetImageSprite(item2, sp);
				}
			}
			else
			{
				foreach (Image item3 in m_bossNightImage)
				{
					NKCUtil.SetImageSprite(item3, sp);
				}
			}
		}
		int clearLevel = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().GetClearLevel(bossGroupdID);
		for (int i = 0; i < m_lstClearImage.Count; i++)
		{
			Color color = NKCFierceUI.BossClearColorNotProgress;
			if (i < clearLevel || (i == 2 && clearLevel == 3))
			{
				color = NKCFierceUI.BossClearColorClear;
			}
			else if (i == clearLevel)
			{
				color = NKCFierceUI.BossClearColorCanTry;
			}
			NKCUtil.SetImageColor(m_lstClearImage[i], color);
		}
		NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_BASIC, bSeason && !flag);
		NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE, bSeason && flag);
		NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_END, !bSeason && !flag);
		NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE_END, !bSeason && flag);
		NKCUtil.SetGameobjectActive(m_objNoneRecord, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBossClearCnt, bValue: true);
		m_bSeason = bSeason;
		m_fierceBossGroupID = bossGroupdID;
	}

	public void SetData(int bossGroupdID)
	{
		Sprite sp = null;
		bool flag = false;
		if (NKMFierceBossGroupTemplet.Groups.ContainsKey(bossGroupdID))
		{
			foreach (NKMFierceBossGroupTemplet item in NKMFierceBossGroupTemplet.Groups[bossGroupdID])
			{
				sp = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_fierce_battle_boss_thumbnail", item.UI_BossFaceSlot);
			}
			if (!flag)
			{
				foreach (Image item2 in m_bossImage)
				{
					NKCUtil.SetImageSprite(item2, sp);
				}
			}
			else
			{
				foreach (Image item3 in m_bossNightImage)
				{
					NKCUtil.SetImageSprite(item3, sp);
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_BASIC, bValue: true);
		NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE, bValue: false);
		NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_END, bValue: false);
		NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE_END, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBossClearCnt, bValue: false);
		m_bSeason = true;
		m_fierceBossGroupID = bossGroupdID;
	}

	public void OnClicked(bool bSelected)
	{
		if (m_bSeason)
		{
			NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE_Normal, !bSelected);
			NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE_Select, bSelected);
			NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_BASIC_Normal, !bSelected);
			NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_BASIC_Select, bSelected);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_END_Normal, !bSelected);
			NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_END_Select, bSelected);
			NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE_END_Normal, !bSelected);
			NKCUtil.SetGameobjectActive(m_BOSS_LIST_SLOT_BUTTON_NIGHTMARE_END_Select, bSelected);
		}
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	public void SetHasRecord(bool bHas)
	{
		NKCUtil.SetGameobjectActive(m_objNoneRecord, !bHas);
	}
}
