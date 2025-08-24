using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeHiddenOptionSlot : MonoBehaviour
{
	public GameObject m_objStat;

	public Text m_lbStatDesc;

	public Text m_lbStatDetail;

	public GameObject m_objStat_2;

	public Text m_lbStatDesc_2;

	public Text m_lbStatDetail_2;

	public GameObject m_objLock;

	public Text m_lbLockDesc;

	public GameObject m_objOpenEnable;

	public Color m_openEnableTextColor;

	public Color m_openedStatTextColor;

	public GameObject[] m_objSocketSimbol;

	public NKCUIComStateButton m_btnSelect;

	public GameObject m_objSelected;

	public Text m_lbSocketNum;

	[Header("이펙트")]
	public GameObject m_fxStatMax;

	public GameObject m_fxStatOver90;

	public GameObject m_fxRerollResult;

	public GameObject m_fxRerollConfirm;

	private int m_SocketIndex;

	public void Init()
	{
		int siblingIndex = base.transform.GetSiblingIndex();
		if (m_objSocketSimbol != null)
		{
			int num = m_objSocketSimbol.Length;
			for (int i = 0; i < num; i++)
			{
				NKCUtil.SetGameobjectActive(m_objSocketSimbol[i], i <= siblingIndex);
			}
		}
		if (m_btnSelect != null)
		{
			m_btnSelect.PointerClick.RemoveAllListeners();
			m_btnSelect.PointerClick.AddListener(OnClickSelect);
		}
	}

	private void DeactiveAllFx()
	{
		NKCUtil.SetGameobjectActive(m_fxStatMax, bValue: false);
		NKCUtil.SetGameobjectActive(m_fxStatOver90, bValue: false);
		NKCUtil.SetGameobjectActive(m_fxRerollResult, bValue: false);
		NKCUtil.SetGameobjectActive(m_fxRerollConfirm, bValue: false);
	}

	public void SetEmpty()
	{
		NKCUtil.SetGameobjectActive(m_objStat, bValue: false);
		NKCUtil.SetGameobjectActive(m_objStat_2, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOpenEnable, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSelected, bValue: false);
		for (int i = 0; i < m_objSocketSimbol.Length; i++)
		{
			NKCUtil.SetGameobjectActive(m_objSocketSimbol[i], bValue: false);
		}
		m_btnSelect.enabled = false;
		m_SocketIndex = -1;
		NKCUtil.SetLabelText(m_lbSocketNum, string.Format(NKCUtilString.GET_STRING_FACTORY_HIDDEN_OPTION_SOCKET, "-"));
		DeactiveAllFx();
	}

	public void Lock(int requiredEnchantLevel, bool openEnable)
	{
		NKCUtil.SetGameobjectActive(m_objStat, openEnable);
		NKCUtil.SetGameobjectActive(m_objStat_2, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLock, !openEnable);
		NKCUtil.SetGameobjectActive(m_objOpenEnable, openEnable);
		DeactiveAllFx();
		if (!openEnable)
		{
			NKCUtil.SetLabelText(m_lbLockDesc, string.Format(NKCUtilString.GET_STRING_EQUIP_POTENTIAL_REQUIRED_ENCHANT_LV, requiredEnchantLevel));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbStatDesc, NKCUtilString.GET_STRING_EQUIP_POTENTIAL_OPEN_ENABLE);
			NKCUtil.SetLabelTextColor(m_lbStatDesc, m_openEnableTextColor);
			NKCUtil.SetLabelText(m_lbStatDetail, "");
			NKCUtil.SetLabelTextColor(m_lbStatDetail, m_openEnableTextColor);
		}
		m_btnSelect.enabled = false;
		m_SocketIndex = -1;
	}

	public void Unlocked(int socketIndex, List<NKMPotentialOption> lstPotentialOptions, int candidatePrecision = -1)
	{
		NKCUtil.SetGameobjectActive(m_objStat, bValue: true);
		NKCUtil.SetGameobjectActive(m_objStat_2, lstPotentialOptions.Count > 1);
		NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOpenEnable, bValue: false);
		DeactiveAllFx();
		float num = lstPotentialOptions[0].sockets[socketIndex].statValue;
		int precision = lstPotentialOptions[0].sockets[socketIndex].precision;
		if (candidatePrecision >= 0)
		{
			num = NKMPotentialOptionTemplet.Find(lstPotentialOptions[0].optionKey).sockets[socketIndex].CalcStatValue(candidatePrecision);
		}
		bool flag = num < 0f;
		if (NKCUtilString.IsNameReversedIfNegative(lstPotentialOptions[0].statType) && flag)
		{
			num = Mathf.Abs(num);
		}
		string text = ((socketIndex == 0) ? NKCUtilString.GetStatShortName(lstPotentialOptions[0].statType, flag) : "");
		NKCUtil.SetLabelText(m_lbStatDesc, text + " " + NKCUtil.GetPotentialSocketStatText(NKMUnitStatManager.IsPercentStat(lstPotentialOptions[0].statType), num));
		NKCUtil.SetLabelTextColor(m_lbStatDesc, m_openedStatTextColor);
		NKCUtil.SetLabelText(m_lbStatDetail, "(" + NKCUtil.GetPotentialSocketStatText(NKMUnitStatManager.IsPercentStat(lstPotentialOptions[0].statType), num, bShowDetail: true) + ")");
		NKCUtil.SetLabelTextColor(m_lbStatDetail, m_openedStatTextColor);
		NKCUtil.SetLabelTextColor(m_lbStatDesc, NKCUtil.GetPotentialOptionColor(precision));
		NKCUtil.SetLabelTextColor(m_lbStatDetail, NKCUtil.GetPotentialOptionColor(precision));
		if (lstPotentialOptions.Count > 1)
		{
			num = lstPotentialOptions[1].sockets[socketIndex].statValue;
			if (candidatePrecision >= 0)
			{
				num = NKMPotentialOptionTemplet.Find(lstPotentialOptions[1].optionKey).sockets[socketIndex].CalcStatValue(candidatePrecision);
			}
			flag = num < 0f;
			if (NKCUtilString.IsNameReversedIfNegative(lstPotentialOptions[1].statType) && flag)
			{
				num = Mathf.Abs(num);
			}
			text = ((socketIndex == 0) ? NKCUtilString.GetStatShortName(lstPotentialOptions[1].statType, flag) : "");
			NKCUtil.SetLabelText(m_lbStatDesc_2, text + " " + NKCUtil.GetPotentialSocketStatText(NKMUnitStatManager.IsPercentStat(lstPotentialOptions[1].statType), num));
			NKCUtil.SetLabelTextColor(m_lbStatDesc_2, m_openedStatTextColor);
			NKCUtil.SetLabelText(m_lbStatDetail_2, "(" + NKCUtil.GetPotentialSocketStatText(NKMUnitStatManager.IsPercentStat(lstPotentialOptions[1].statType), num, bShowDetail: true) + ")");
			NKCUtil.SetLabelTextColor(m_lbStatDetail_2, m_openedStatTextColor);
			NKCUtil.SetLabelTextColor(m_lbStatDesc_2, NKCUtil.GetPotentialOptionColor(precision));
			NKCUtil.SetLabelTextColor(m_lbStatDetail_2, NKCUtil.GetPotentialOptionColor(precision));
		}
		if (precision == 100)
		{
			NKCUtil.SetGameobjectActive(m_fxStatMax, bValue: true);
		}
		else if (precision > 90)
		{
			NKCUtil.SetGameobjectActive(m_fxStatOver90, bValue: true);
		}
		m_btnSelect.enabled = true;
		m_SocketIndex = socketIndex;
	}

	public void SetRerollData(int socketIndex, List<NKMPotentialOption> lstPotentialOptions, int precision)
	{
		NKCUtil.SetGameobjectActive(m_objStat, bValue: true);
		NKCUtil.SetGameobjectActive(m_objStat_2, lstPotentialOptions.Count > 1);
		NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOpenEnable, bValue: false);
		NKCUtil.SetLabelText(m_lbSocketNum, string.Format(NKCUtilString.GET_STRING_FACTORY_HIDDEN_OPTION_SOCKET, socketIndex + 1));
		DeactiveAllFx();
		if (lstPotentialOptions.Count > 0)
		{
			NKMPotentialOptionTemplet nKMPotentialOptionTemplet = NKMPotentialOptionTemplet.Find(lstPotentialOptions[0].optionKey);
			if (nKMPotentialOptionTemplet != null)
			{
				float num = nKMPotentialOptionTemplet.sockets[socketIndex].CalcStatValue(precision);
				bool flag = num < 0f;
				if (NKCUtilString.IsNameReversedIfNegative(nKMPotentialOptionTemplet.StatType) && flag)
				{
					num = Mathf.Abs(num);
				}
				NKMPotentialSocketTemplet nKMPotentialSocketTemplet = nKMPotentialOptionTemplet.sockets[socketIndex];
				bool bPercent = NKMUnitStatManager.IsPercentStat(nKMPotentialOptionTemplet.StatType);
				string statRangeString = NKCUtilString.GetStatRangeString(nKMPotentialOptionTemplet.StatType, nKMPotentialSocketTemplet.MinStat, nKMPotentialSocketTemplet.MaxStat, bPercent);
				NKCUtil.SetLabelText(m_lbStatDesc, NKCUtil.GetPotentialSocketStatText(NKMUnitStatManager.IsPercentStat(nKMPotentialOptionTemplet.StatType), num) + " (" + statRangeString + ")");
				NKCUtil.SetLabelTextColor(m_lbStatDesc, m_openedStatTextColor);
				NKCUtil.SetLabelText(m_lbStatDetail, "(" + NKCUtil.GetPotentialSocketStatText(NKMUnitStatManager.IsPercentStat(nKMPotentialOptionTemplet.StatType), num, bShowDetail: true) + ")");
				NKCUtil.SetLabelTextColor(m_lbStatDetail, m_openedStatTextColor);
				NKCUtil.SetLabelTextColor(m_lbStatDesc, NKCUtil.GetPotentialOptionColor(precision));
				NKCUtil.SetLabelTextColor(m_lbStatDetail, NKCUtil.GetPotentialOptionColor(precision));
				if (lstPotentialOptions.Count > 1)
				{
					nKMPotentialOptionTemplet = NKMPotentialOptionTemplet.Find(lstPotentialOptions[1].optionKey);
					num = nKMPotentialOptionTemplet.sockets[socketIndex].CalcStatValue(precision);
					flag = num < 0f;
					if (NKCUtilString.IsNameReversedIfNegative(nKMPotentialOptionTemplet.StatType) && flag)
					{
						num = Mathf.Abs(num);
					}
					nKMPotentialSocketTemplet = nKMPotentialOptionTemplet.sockets[socketIndex];
					bPercent = NKMUnitStatManager.IsPercentStat(nKMPotentialOptionTemplet.StatType);
					statRangeString = NKCUtilString.GetStatRangeString(nKMPotentialOptionTemplet.StatType, nKMPotentialSocketTemplet.MinStat, nKMPotentialSocketTemplet.MaxStat, bPercent);
					NKCUtil.SetLabelText(m_lbStatDesc_2, NKCUtil.GetPotentialSocketStatText(NKMUnitStatManager.IsPercentStat(nKMPotentialOptionTemplet.StatType), num) + " (" + statRangeString + ")");
					NKCUtil.SetLabelTextColor(m_lbStatDesc_2, m_openedStatTextColor);
					NKCUtil.SetLabelText(m_lbStatDetail_2, "(" + NKCUtil.GetPotentialSocketStatText(NKMUnitStatManager.IsPercentStat(nKMPotentialOptionTemplet.StatType), num, bShowDetail: true) + ")");
					NKCUtil.SetLabelTextColor(m_lbStatDetail_2, m_openedStatTextColor);
					NKCUtil.SetLabelTextColor(m_lbStatDesc_2, NKCUtil.GetPotentialOptionColor(precision));
					NKCUtil.SetLabelTextColor(m_lbStatDetail_2, NKCUtil.GetPotentialOptionColor(precision));
				}
			}
			for (int i = 0; i < m_objSocketSimbol.Length; i++)
			{
				NKCUtil.SetGameobjectActive(m_objSocketSimbol[i], i <= socketIndex);
			}
			if (precision == 100)
			{
				NKCUtil.SetGameobjectActive(m_fxStatMax, bValue: true);
			}
			else if (precision > 90)
			{
				NKCUtil.SetGameobjectActive(m_fxStatOver90, bValue: true);
			}
		}
		m_btnSelect.enabled = false;
		m_SocketIndex = -1;
	}

	public void SetSelected(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objSelected, bValue);
	}

	public void SetActiveFxRerollResult()
	{
		NKCUtil.SetGameobjectActive(m_fxRerollResult, bValue: false);
		NKCUtil.SetGameobjectActive(m_fxRerollResult, bValue: true);
	}

	public void SetActiveFxRerollConfirm()
	{
		NKCUtil.SetGameobjectActive(m_fxRerollConfirm, bValue: false);
		NKCUtil.SetGameobjectActive(m_fxRerollConfirm, bValue: true);
	}

	private void OnClickSelect()
	{
		if (m_SocketIndex >= 0 && NKCUIForge.IsInstanceOpen)
		{
			NKCUIForge.Instance.OnSelectPotentialSlot(m_SocketIndex);
		}
	}
}
