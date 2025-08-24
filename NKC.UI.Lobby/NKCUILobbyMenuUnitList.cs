using DG.Tweening;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuUnitList : NKCUILobbyMenuButtonBase
{
	public NKCUIComStateButton m_csbtnMenu;

	public GameObject m_objNotify;

	public Text m_lbUnitCount;

	public Text m_lbUnitMaxCount;

	public Image m_imgUnitFillrate;

	public Text m_lbShipCount;

	public Text m_lbShipMaxCount;

	public Image m_imgShipFillrate;

	public Text m_lbOperatorCount;

	public Text m_lbOperatorMaxCount;

	public Image m_imgOperatorFillrate;

	private int m_UnitCount;

	private int m_UnitMaxCount;

	private int m_ShipCount;

	private int m_ShipMaxCount;

	private int m_OperatorCount;

	private int m_OperatorMaxCount;

	public GameObject m_NKM_UI_LOBBY_RIGHT_MENU_2_UNITLIST_OperatorCount;

	private const float m_fAnimTime = 0.6f;

	private const Ease m_eAnimEase = Ease.InCubic;

	public float UnitFillrate
	{
		get
		{
			if (!(m_imgUnitFillrate != null))
			{
				return 0f;
			}
			return m_imgUnitFillrate.fillAmount;
		}
		set
		{
			if (m_imgUnitFillrate != null)
			{
				m_imgUnitFillrate.fillAmount = value;
			}
		}
	}

	public float ShipFillrate
	{
		get
		{
			if (!(m_imgShipFillrate != null))
			{
				return 0f;
			}
			return m_imgShipFillrate.fillAmount;
		}
		set
		{
			if (m_imgShipFillrate != null)
			{
				m_imgShipFillrate.fillAmount = value;
			}
		}
	}

	public float OperatorFillrate
	{
		get
		{
			if (!(m_imgOperatorFillrate != null))
			{
				return 0f;
			}
			return m_imgOperatorFillrate.fillAmount;
		}
		set
		{
			if (m_imgOperatorFillrate != null)
			{
				m_imgOperatorFillrate.fillAmount = value;
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
		m_UnitCount = userData.m_ArmyData.GetCurrentUnitCount();
		m_UnitMaxCount = userData.m_ArmyData.m_MaxUnitCount;
		NKCUtil.SetLabelText(m_lbUnitCount, m_UnitCount.ToString());
		NKCUtil.SetLabelText(m_lbUnitMaxCount, "/" + m_UnitMaxCount);
		UnitFillrate = GetFillRate(m_UnitCount, m_UnitMaxCount);
		m_ShipCount = userData.m_ArmyData.GetCurrentShipCount();
		m_ShipMaxCount = userData.m_ArmyData.m_MaxShipCount;
		NKCUtil.SetLabelText(m_lbShipCount, m_ShipCount.ToString());
		NKCUtil.SetLabelText(m_lbShipMaxCount, "/" + m_ShipMaxCount);
		ShipFillrate = GetFillRate(m_ShipCount, m_ShipMaxCount);
		if (!NKCOperatorUtil.IsHide() && NKCOperatorUtil.IsActive())
		{
			m_OperatorCount = userData.m_ArmyData.GetCurrentOperatorCount();
			m_OperatorMaxCount = userData.m_ArmyData.m_MaxOperatorCount;
			NKCUtil.SetLabelText(m_lbOperatorCount, m_OperatorCount.ToString());
			NKCUtil.SetLabelText(m_lbOperatorMaxCount, "/" + m_OperatorMaxCount);
			OperatorFillrate = GetFillRate(m_OperatorCount, m_OperatorMaxCount);
			NKCUtil.SetGameobjectActive(m_NKM_UI_LOBBY_RIGHT_MENU_2_UNITLIST_OperatorCount, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_LOBBY_RIGHT_MENU_2_UNITLIST_OperatorCount, bValue: false);
		}
		SetNotify(value: false);
	}

	public override void PlayAnimation(bool bActive)
	{
		m_lbUnitCount.DOKill();
		m_imgUnitFillrate.DOKill();
		m_lbShipCount.DOKill();
		m_imgShipFillrate.DOKill();
		bool flag = NKCOperatorUtil.IsActive() && !NKCOperatorUtil.IsHide();
		if (flag)
		{
			m_lbOperatorCount.DOKill();
			m_imgOperatorFillrate.DOKill();
		}
		if (bActive)
		{
			m_lbUnitCount.DOText(m_UnitCount.ToString(), 0.6f, richTextEnabled: false, ScrambleMode.Numerals).SetEase(Ease.InCubic);
			UnitFillrate = 0f;
			m_imgUnitFillrate.DOFillAmount(GetFillRate(m_UnitCount, m_UnitMaxCount), 0.6f).SetEase(Ease.InCubic);
			m_lbShipCount.DOText(m_ShipCount.ToString(), 0.6f, richTextEnabled: false, ScrambleMode.Numerals).SetEase(Ease.InCubic);
			ShipFillrate = 0f;
			m_imgShipFillrate.DOFillAmount(GetFillRate(m_ShipCount, m_ShipMaxCount), 0.6f).SetEase(Ease.InCubic);
			if (flag)
			{
				m_lbOperatorCount.DOText(m_OperatorCount.ToString(), 0.6f, richTextEnabled: false, ScrambleMode.Numerals).SetEase(Ease.InCubic);
				OperatorFillrate = 0f;
				m_imgOperatorFillrate.DOFillAmount(GetFillRate(m_OperatorCount, m_OperatorMaxCount), 0.6f).SetEase(Ease.InCubic);
			}
		}
		else
		{
			NKCUtil.SetLabelText(m_lbUnitCount, m_UnitCount.ToString());
			UnitFillrate = GetFillRate(m_UnitCount, m_UnitMaxCount);
			NKCUtil.SetLabelText(m_lbShipCount, m_ShipCount.ToString());
			ShipFillrate = GetFillRate(m_ShipCount, m_ShipMaxCount);
			if (flag)
			{
				NKCUtil.SetLabelText(m_lbOperatorCount, m_OperatorCount.ToString());
				OperatorFillrate = GetFillRate(m_OperatorCount, m_OperatorMaxCount);
			}
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
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_UNIT_LIST, bForce: false);
	}
}
