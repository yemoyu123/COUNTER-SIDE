using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKC.UI;

public class NKCUIOverlayCaption : NKCUIBase
{
	public List<NKCUIComCaption> m_lstCaptions = new List<NKCUIComCaption>();

	private List<NKCUIComCaption.CaptionData> m_lstCaptionSound = new List<NKCUIComCaption.CaptionData>();

	private List<NKCUIComCaption.CaptionDataTime> m_lstCaptionTime = new List<NKCUIComCaption.CaptionDataTime>();

	private Coroutine m_delayCoroutine;

	private float m_fNextCheckTime;

	private long m_fCaptionTimer;

	private int m_iTimeCaptionKey;

	public override eMenutype eUIType => eMenutype.Overlay;

	public override string MenuName => "";

	public override void CloseInternal()
	{
		CloseAllCaption();
		StopDelayWaiting();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void OpenCaption(string caption, int soundUID, float delay = 0f)
	{
		StopDelayWaiting();
		if (m_lstCaptions.Count == 0 || !NKCUtil.CheckFinalCaptionEnabled() || string.IsNullOrWhiteSpace(caption))
		{
			return;
		}
		if (m_lstCaptionSound.Count >= m_lstCaptions.Count)
		{
			bool flag = false;
			for (int i = 0; i < m_lstCaptions.Count; i++)
			{
				if (!m_lstCaptions[i].IsActive)
				{
					m_lstCaptionSound.RemoveAt(i);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				m_lstCaptionSound.RemoveAt(0);
			}
		}
		float forcePlaySecond = ((soundUID == 0) ? (1f + (float)caption.Length * 0.1f) : 0f);
		NKCUIComCaption.CaptionData item = new NKCUIComCaption.CaptionData(caption, soundUID, forcePlaySecond);
		m_lstCaptionSound.Add(item);
		NKCUtil.SetGameobjectActive(base.gameObject, m_lstCaptionSound.Count > 0);
		if (base.gameObject.activeSelf && delay > 0f)
		{
			CloseAllCaption();
			StartDelayWaiting(delay);
		}
		else
		{
			RefreshData();
		}
	}

	public void OpenCaption(List<NKCUIComCaption.CaptionDataTime> lstCaption)
	{
		StopDelayWaiting();
		if (lstCaption.Count == 0)
		{
			return;
		}
		foreach (NKCUIComCaption.CaptionDataTime item in lstCaption)
		{
			if (string.IsNullOrWhiteSpace(item.caption) || item.startTime < 0 || item.endTime < 0)
			{
				return;
			}
		}
		m_lstCaptionSound.Clear();
		m_lstCaptionTime.Clear();
		m_lstCaptionTime = lstCaption;
		m_iTimeCaptionKey = 0;
		foreach (NKCUIComCaption.CaptionDataTime item2 in m_lstCaptionTime)
		{
			item2.ConvertTimeToTick();
		}
		NKCUtil.SetGameobjectActive(base.gameObject, m_lstCaptionTime.Count > 0);
	}

	private void StopDelayWaiting()
	{
		if (base.gameObject.activeSelf && m_delayCoroutine != null)
		{
			StopCoroutine(m_delayCoroutine);
			m_delayCoroutine = null;
		}
	}

	private void StartDelayWaiting(float delay)
	{
		m_delayCoroutine = StartCoroutine(DelayWaiting(delay));
	}

	private IEnumerator DelayWaiting(float delay)
	{
		float timer = 0f;
		while (timer < delay)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		RefreshData();
	}

	private void RefreshData()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, m_lstCaptionSound.Count > 0);
		for (int i = 0; i < m_lstCaptions.Count; i++)
		{
			if (i < m_lstCaptionSound.Count)
			{
				m_lstCaptions[i].SetData(m_lstCaptionSound[i]);
			}
			else
			{
				m_lstCaptions[i].CloseCaption();
			}
			m_lstCaptions[i].transform.SetAsLastSibling();
		}
	}

	private void RefreshData(int iKey)
	{
		if (m_lstCaptionTime.Count < iKey || (iKey > 0 && m_iTimeCaptionKey == iKey))
		{
			return;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, m_lstCaptionTime.Count > 0);
		for (int i = 0; i < m_lstCaptions.Count; i++)
		{
			if (m_lstCaptionTime.Count > 0)
			{
				if (i < m_lstCaptionTime.Count)
				{
					m_lstCaptions[i].SetData(m_lstCaptionTime[iKey]);
					m_iTimeCaptionKey = iKey;
				}
				else
				{
					m_lstCaptions[i].CloseCaption();
				}
			}
			m_lstCaptions[i].transform.SetAsLastSibling();
		}
	}

	public void CloseCaption(int key)
	{
		for (int i = 0; i < m_lstCaptions.Count; i++)
		{
			if (m_lstCaptions[i].GetCaptionData() != null && m_lstCaptions[i].GetCaptionData().key == key)
			{
				m_lstCaptions[i].CloseCaption();
				break;
			}
		}
		RefreshData();
	}

	public void CloseAllCaption()
	{
		for (int i = 0; i < m_lstCaptions.Count; i++)
		{
			m_lstCaptions[i].CloseCaption();
		}
		m_lstCaptionTime.Clear();
	}

	private void Update()
	{
		if (m_lstCaptionSound.Count > 0 && m_fNextCheckTime <= Time.time)
		{
			m_fNextCheckTime = Time.time + 1f;
			for (int i = 0; i < m_lstCaptions.Count; i++)
			{
				if (i < m_lstCaptionSound.Count)
				{
					if (!(m_lstCaptionSound[i].forcePlayTime >= Time.time) && !NKCSoundManager.IsPlayingVoice(m_lstCaptionSound[i].key))
					{
						m_lstCaptions[i].CloseCaption();
						m_lstCaptionSound.RemoveAt(i);
						RefreshData();
						break;
					}
				}
				else if (m_lstCaptions[i].IsActive)
				{
					m_lstCaptions[i].CloseCaption();
				}
			}
		}
		if (m_lstCaptionTime.Count <= 0 || !(m_fNextCheckTime <= Time.time))
		{
			return;
		}
		m_fNextCheckTime = Time.time + 0.1f;
		m_fCaptionTimer = DateTime.Now.Ticks;
		for (int j = 0; j < m_lstCaptions.Count; j++)
		{
			if (m_iTimeCaptionKey < m_lstCaptionTime.Count)
			{
				if (m_lstCaptionTime[m_iTimeCaptionKey].startTime < m_fCaptionTimer)
				{
					RefreshData(m_iTimeCaptionKey);
				}
				if (m_lstCaptionTime[m_iTimeCaptionKey].endTime < m_fCaptionTimer)
				{
					m_lstCaptions[j].CloseCaption();
				}
				if (m_lstCaptionTime.Count > m_iTimeCaptionKey + 1 && m_lstCaptionTime[m_iTimeCaptionKey + 1].startTime < m_fCaptionTimer)
				{
					RefreshData(m_iTimeCaptionKey + 1);
				}
			}
			else if (m_lstCaptions[j].IsActive)
			{
				m_lstCaptions[j].CloseCaption();
			}
		}
	}
}
