using DG.Tweening;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuInventory : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_csbtnMenu;

	public GameObject m_objNotify;

	public Text m_lbMiscCount;

	public Text m_lbCount;

	public Text m_lbMaxCount;

	public Image m_imgFillrate;

	private int m_MiscCount;

	private int m_EquipCount;

	private int m_EquipMaxCount;

	private const float m_fAnimTime = 0.6f;

	private const Ease m_eAnimEase = Ease.InCubic;

	public float Fillrate
	{
		get
		{
			if (!(m_imgFillrate != null))
			{
				return 0f;
			}
			return m_imgFillrate.fillAmount;
		}
		set
		{
			if (m_imgFillrate != null)
			{
				m_imgFillrate.fillAmount = value;
			}
		}
	}

	public void Init()
	{
		if (m_csbtnMenu != null)
		{
			m_csbtnMenu.PointerClick.RemoveAllListeners();
			m_csbtnMenu.PointerClick.AddListener(OnButton);
		}
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		NKCUtil.SetGameobjectActive(m_objNotify, bValue: false);
		m_MiscCount = userData.m_InventoryData.GetCountMiscExceptCurrency();
		m_EquipCount = userData.m_InventoryData.GetCountEquipItemTypes();
		m_EquipMaxCount = userData.m_InventoryData.m_MaxItemEqipCount;
		NKCUtil.SetLabelText(m_lbMiscCount, m_MiscCount.ToString());
		NKCUtil.SetLabelText(m_lbCount, m_EquipCount.ToString());
		NKCUtil.SetLabelText(m_lbMaxCount, "/" + m_EquipMaxCount);
		Fillrate = GetFillRate(m_EquipCount, m_EquipMaxCount);
		bool flag = false;
		foreach (NKMItemMiscData value in NKCScenManager.CurrentUserData().m_InventoryData.MiscItems.Values)
		{
			if (value.TotalCount > 0)
			{
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(value.ItemID);
				if (itemMiscTempletByID != null && itemMiscTempletByID.IsUsable())
				{
					flag = true;
					break;
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objNotify, flag);
		SetNotify(flag);
	}

	public override void PlayAnimation(bool bActive)
	{
		m_lbCount.DOKill();
		m_imgFillrate.DOKill();
		m_lbMiscCount.DOKill();
		if (bActive)
		{
			m_lbMiscCount.DOText(m_MiscCount.ToString(), 0.6f, richTextEnabled: false, ScrambleMode.Numerals).SetEase(Ease.InCubic);
			m_lbCount.DOText(m_EquipCount.ToString(), 0.6f, richTextEnabled: false, ScrambleMode.Numerals).SetEase(Ease.InCubic);
			Fillrate = 0f;
			m_imgFillrate.DOFillAmount(GetFillRate(m_EquipCount, m_EquipMaxCount), 0.6f).SetEase(Ease.InCubic);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbMiscCount, m_MiscCount.ToString());
			NKCUtil.SetLabelText(m_lbCount, m_EquipCount.ToString());
			Fillrate = GetFillRate(m_EquipCount, m_EquipMaxCount);
		}
	}

	private float GetFillRate(int count, int maxCount)
	{
		if (maxCount == 0)
		{
			return 0f;
		}
		return (float)count / (float)maxCount;
	}

	private void OnButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_INVENTORY, bForce: false);
	}
}
