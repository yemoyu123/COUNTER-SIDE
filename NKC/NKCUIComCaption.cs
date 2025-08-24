using System;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIComCaption : MonoBehaviour
{
	public class CaptionData
	{
		public string caption;

		public int key;

		public float forcePlayTime;

		public CaptionData(string _caption, int _key, float _forcePlaySecond = 0f)
		{
			caption = _caption;
			key = _key;
			forcePlayTime = Time.time + _forcePlaySecond;
			if (key == 0)
			{
				key = ((caption != null) ? _caption.GetHashCode() : 0);
			}
		}
	}

	public class CaptionDataTime : CaptionData
	{
		public long startTime;

		public long endTime;

		public bool hideBackground;

		public CaptionDataTime(string _caption, int _key, long _startTime, long _endTime, bool _hideBackground)
			: base(_caption, _key)
		{
			startTime = _startTime;
			endTime = _endTime;
			hideBackground = _hideBackground;
		}

		public void ConvertTimeToTick()
		{
			endTime = DateTime.Now.AddSeconds(startTime + endTime).Ticks;
			startTime = DateTime.Now.AddSeconds(startTime).Ticks;
		}
	}

	public Text m_lbCaption;

	private const int INVALID_SOUND_UID = int.MinValue;

	private CaptionData m_CaptionData;

	public Image m_Background;

	private Vector2 m_OrgTextPos;

	private bool m_bReservedTextOffset;

	private Vector2 m_ReservedTextOffsetPos;

	public bool IsActive
	{
		get
		{
			if (m_CaptionData is CaptionDataTime)
			{
				return true;
			}
			if (m_CaptionData.key == int.MinValue)
			{
				return base.gameObject.activeSelf;
			}
			return true;
		}
	}

	public CaptionData GetCaptionData()
	{
		return m_CaptionData;
	}

	private void Awake()
	{
		m_Background = base.gameObject.GetComponentInChildren<Image>();
	}

	public void SetEnableBackgound(bool bEnable)
	{
		if (null != m_Background)
		{
			m_Background.enabled = bEnable;
		}
	}

	private void SaveOrgTextPos()
	{
		if (m_lbCaption != null)
		{
			m_OrgTextPos = m_lbCaption.rectTransform.anchoredPosition;
		}
	}

	public void RestoreTextPos()
	{
		if (m_lbCaption != null)
		{
			m_lbCaption.rectTransform.anchoredPosition = m_OrgTextPos;
		}
	}

	public void SetTextPosOffset(float offsetX, float offsetY)
	{
		m_ReservedTextOffsetPos = new Vector2(offsetX, offsetY);
		m_bReservedTextOffset = true;
	}

	public void ResetCaption()
	{
		RestoreTextPos();
	}

	public bool SetData(string caption, int soundUID, int forcePlaySecond = 0)
	{
		return SetData(new CaptionData(caption, soundUID, forcePlaySecond));
	}

	public bool SetData(CaptionData captionData)
	{
		if (string.IsNullOrEmpty(captionData.caption) || captionData.key == int.MinValue)
		{
			CloseCaption();
			return false;
		}
		if (m_bReservedTextOffset)
		{
			m_bReservedTextOffset = false;
			if (m_lbCaption != null)
			{
				m_lbCaption.rectTransform.anchoredPosition = new Vector2(m_OrgTextPos.x + m_ReservedTextOffsetPos.x, m_OrgTextPos.y + m_ReservedTextOffsetPos.y);
			}
		}
		SetEnableBackgound(bEnable: true);
		m_CaptionData = captionData;
		string msg = NKCUtil.TextSplitLine(captionData.caption, m_lbCaption);
		NKCUtil.SetLabelText(m_lbCaption, msg);
		NKCUtil.SetGameobjectActive(this, bValue: true);
		return true;
	}

	public bool SetData(CaptionDataTime captionData)
	{
		if (string.IsNullOrEmpty(captionData.caption))
		{
			CloseCaption();
			return false;
		}
		m_CaptionData = captionData;
		SetEnableBackgound(!captionData.hideBackground);
		string msg = NKCUtil.TextSplitLine(captionData.caption, m_lbCaption);
		NKCUtil.SetLabelText(m_lbCaption, msg);
		NKCUtil.SetGameobjectActive(this, bValue: true);
		return true;
	}

	public void CloseCaption()
	{
		if (m_CaptionData != null)
		{
			m_CaptionData.key = int.MinValue;
		}
		NKCUtil.SetGameobjectActive(this, bValue: false);
	}
}
