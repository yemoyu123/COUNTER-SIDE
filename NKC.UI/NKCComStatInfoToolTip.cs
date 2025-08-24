using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI;

public class NKCComStatInfoToolTip : MonoBehaviour
{
	private NKCUIComStateButton btn;

	private NKCUIComRaycastTarget raycastTarget;

	private string statDesc;

	private bool m_bNegative;

	private bool bInit;

	private NKM_STAT_TYPE m_StatType;

	private void Init()
	{
		if (btn == null)
		{
			btn = base.gameObject.GetComponent<NKCUIComStateButton>();
			if (btn == null)
			{
				btn = base.gameObject.AddComponent<NKCUIComStateButton>();
			}
		}
		if (btn != null)
		{
			btn.PointerDown.RemoveAllListeners();
			btn.PointerDown.AddListener(OnTouchStatInfo);
		}
		if (raycastTarget == null)
		{
			raycastTarget = base.gameObject.GetComponent<NKCUIComRaycastTarget>();
			if (raycastTarget == null)
			{
				raycastTarget = base.gameObject.AddComponent<NKCUIComRaycastTarget>();
			}
		}
	}

	public void SetType(NKM_STAT_TYPE type, bool bNegative = false)
	{
		if (!bInit)
		{
			Init();
			bInit = true;
		}
		m_bNegative = bNegative;
		foreach (NKCStatInfoTemplet value in NKCStatInfoTemplet.Values)
		{
			if (value != null && string.Equals(type.ToString(), value.Stat_ID))
			{
				m_StatType = type;
				if (m_bNegative && !string.IsNullOrEmpty(value.Stat_Negative_DESC))
				{
					statDesc = NKCStringTable.GetString(value.Stat_Negative_DESC);
				}
				else
				{
					statDesc = NKCStringTable.GetString(value.Stat_Desc);
				}
				break;
			}
		}
	}

	private void OnTouchStatInfo(PointerEventData e)
	{
		NKCUITooltip.Instance.Open(NKCUISlot.eSlotMode.Etc, NKCUtilString.GetStatShortName(m_StatType, m_bNegative), statDesc, e.position);
	}
}
