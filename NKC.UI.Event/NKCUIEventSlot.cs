using System;
using NKM;
using NKM.Event;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventSlot : MonoBehaviour
{
	public delegate void OnSelect(NKMEventTabTemplet tabTemplet);

	public Image m_imgBg;

	public Text m_lbTitle;

	public GameObject m_objRemainTime;

	public Text m_lbRemainTime;

	public GameObject m_objRedDot;

	public NKCUIComToggle m_tgl;

	private int m_EventID;

	private NKM_EVENT_TYPE m_EventType;

	private bool m_bShowRemainTime;

	private long m_EndTick;

	private float m_fDeltaTime;

	public Color m_colSelectedText = new Color(0.003921569f, 9f / 85f, 0.23137255f);

	private OnSelect dOnSelect;

	public int EventID => m_EventID;

	public NKM_EVENT_TYPE EventType => m_EventType;

	public NKMEventTabTemplet EventTabTemplet { get; private set; }

	public bool SetData(NKMEventTabTemplet tabTemplet, NKCUIComToggleGroup tglGroup, bool bSelected, OnSelect onSelect)
	{
		if (tabTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return false;
		}
		EventTabTemplet = tabTemplet;
		m_EventID = tabTemplet.m_EventID;
		m_EventType = tabTemplet.m_EventType;
		m_tgl.SetToggleGroup(tglGroup);
		m_tgl.OnValueChanged.RemoveAllListeners();
		m_tgl.OnValueChanged.AddListener(OnClick);
		SetToggle(bSelected, bForce: true, bImmediate: true);
		dOnSelect = onSelect;
		NKCUtil.SetImageSprite(m_imgBg, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("ab_ui_nkm_ui_event_texture", tabTemplet.m_EventTabImage)));
		NKCUtil.SetLabelText(m_lbTitle, tabTemplet.GetTitle());
		if (tabTemplet.HasTimeLimit)
		{
			m_bShowRemainTime = true;
			m_EndTick = tabTemplet.TimeLimit.Ticks;
			SetRemainTime(m_EndTick);
		}
		else
		{
			m_bShowRemainTime = false;
			m_EndTick = 0L;
		}
		NKCUtil.SetGameobjectActive(m_objRemainTime, m_bShowRemainTime);
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		return true;
	}

	private void SetRemainTime(long endTick)
	{
		if (NKCSynchronizedTime.GetTimeLeft(EventTabTemplet.EventDateEndUtc).TotalDays > (double)NKCSynchronizedTime.UNLIMITD_REMAIN_DAYS)
		{
			NKCUtil.SetLabelText(m_lbRemainTime, NKCUtilString.GET_STRING_EVENT_DATE_UNLIMITED_TEXT);
			return;
		}
		string msg = NKCStringTable.GetString("SI_DP_TIMER_REMAIN", NKCUtilString.GetRemainTimeString(new DateTime(endTick), 1));
		NKCUtil.SetLabelText(m_lbRemainTime, msg);
	}

	public void CheckRedDot()
	{
		if (EventTabTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objRedDot, NKMEventManager.CheckRedDot(EventTabTemplet));
		}
	}

	public void SetToggle(bool bSelect, bool bForce, bool bImmediate)
	{
		m_tgl.Select(bSelect, bForce, bImmediate);
		if (bSelect)
		{
			NKCUtil.SetLabelTextColor(m_lbTitle, m_colSelectedText);
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbTitle, Color.white);
		}
	}

	public void OnClick(bool bSelect)
	{
		if (bSelect)
		{
			dOnSelect?.Invoke(EventTabTemplet);
		}
	}

	private void Update()
	{
		if (m_bShowRemainTime)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRemainTime(m_EndTick);
			}
		}
	}
}
