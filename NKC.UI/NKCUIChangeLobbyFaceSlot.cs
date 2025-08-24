using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIChangeLobbyFaceSlot : MonoBehaviour
{
	public NKCUIComToggle m_Toggle;

	public Text[] m_lbLoyality;

	public Text[] m_lbName;

	public void Init(NKCUIComToggle.ValueChangedWithData onToggle, NKCUIComToggleGroup tglgrp)
	{
		if (m_Toggle != null)
		{
			m_Toggle.OnValueChangedWithData = onToggle;
			m_Toggle.SetToggleGroup(tglgrp);
		}
	}

	public void SetData(NKMUnitData unitData, NKMLobbyFaceTemplet templet)
	{
		if (templet != null)
		{
			SetName(NKCStringTable.GetString(templet.strFaceName));
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
			if (unitTempletBase != null && unitTempletBase.HasUnitStyleType(NKM_UNIT_STYLE_TYPE.NUST_TRAINER))
			{
				SetNoLoyality();
			}
			else
			{
				SetLoyality(templet.reqLoyalty);
			}
			m_Toggle.m_DataInt = templet.Key;
			m_Toggle.SetLock(!templet.CanUseFace(unitData));
		}
	}

	public void SetData(NKMOperator operatorData, NKMLobbyFaceTemplet templet)
	{
		if (templet != null)
		{
			SetName(NKCStringTable.GetString(templet.strFaceName));
			SetNoLoyality();
			m_Toggle.m_DataInt = templet.Key;
			m_Toggle.UnLock();
		}
	}

	public void SetSelected(bool value)
	{
		m_Toggle.Select(value, bForce: true);
	}

	private void SetName(string value)
	{
		for (int i = 0; i < m_lbName.Length; i++)
		{
			NKCUtil.SetLabelText(m_lbName[i], value);
		}
	}

	private void SetLoyality(int value)
	{
		string msg = ((value <= 10000) ? (value / 100).ToString() : NKCStringTable.GetString("SI_DP_STRING_VOICE_CATEGORY_LIFETIME"));
		for (int i = 0; i < m_lbLoyality.Length; i++)
		{
			NKCUtil.SetLabelText(m_lbLoyality[i], msg);
		}
	}

	private void SetNoLoyality()
	{
		for (int i = 0; i < m_lbLoyality.Length; i++)
		{
			NKCUtil.SetLabelText(m_lbLoyality[i], "-");
		}
	}
}
